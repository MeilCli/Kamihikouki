using Kamihikouki.NETStandard.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Kamihikouki.NETStandard
{
    public sealed class CachedNavigator : BaseNavigator
    {
        public static INavigator Inject(INavigationProvider navigationProvider, object view, params object[] viewModels)
        {
            var navigator = new CachedNavigator(navigationProvider);
            navigator.ClassInject(view, viewModels);
            navigator.MethodInject(view, viewModels);
            return navigator;
        }

        public CachedNavigator(INavigationProvider navigationProvider) : base(navigationProvider) { }

        public override void ClassInject(object view, params object[] viewModels)
        {
            if (NavigationProvider == null)
            {
                throw new InvalidOperationException($"{nameof(NavigationProvider)} is null");
            }

            var navigationProviderMethods = NavigationProvider.GetType()
                .GetRuntimeMethods()
                .Select(x => new { method = x, attributes = x.GetCustomAttributes<NavigationProviderAttribute>() })
                // 属性を持っているかどうか
                .Where(x => x.attributes?.Any() ?? false)
                // 返り値がTaskかどうか
                .Where(x => x.method.ReturnType == typeof(Task))
                // 型パラメーターは1つかどうか
                .Where(x => x.method.IsGenericMethod && x.method.IsGenericMethodDefinition && x.method.GetGenericArguments().Length == 1)
                .ToList();

            var navigationAttributes = view.GetType().GetTypeInfo().GetCustomAttributes<BaseNavigationAttribute>();
            var items = new List<((string, Type[], Type), INavigationAttribute, INavigationRequest)>();
            foreach (var navigationAttribute in navigationAttributes)
            {
                var navigationProviderMethod = navigationProviderMethods
                    // INavigationProviderに実装されているメソッドの属性に対象の属性があるかどうか
                    .Where(x => x.attributes.Any(attribute => attribute.NavigationAttributeType == navigationAttribute.GetType()))
                    // INavigationProviderに実装されているメソッドの引数が対象の属性に適しているかどうか
                    .Where(x => x.method.GetParameters().Length == shoudMethodParameterCount(navigationAttribute))
                    .FirstOrDefault();
                if (navigationProviderMethod == null)
                {
                    throw new InvalidOperationException("not provided method, " +
                        $"ViewModelType: {navigationAttribute.ViewModelType.FullName}," +
                        $" BindingPropertyName: {navigationAttribute.BindingPropertyName}");
                }

                object viewModel = getViewModel(navigationAttribute, viewModels);
                INavigationRequest navigationRequest = getNavigationRequest(navigationAttribute, viewModel);
                Type parameter1Type = GetTypeArgument(navigationRequest);
                Type[] typeArguments = new[] { parameter1Type };

                items.Add(((navigationProviderMethod.method.Name, typeArguments, parameter1Type), navigationAttribute, navigationRequest));
            }

            var func = createDelegate(items, NavigationProvider);
            var navigationAction = new NavigationAction(func);

            foreach (var item in items)
            {
                item.Item3.NavigationAction = navigationAction;
            }
        }

        public override void MethodInject(object view, params object[] viewModels)
        {
            var items = new List<((string, Type[], Type), INavigationAttribute, INavigationRequest)>();
            foreach (var info in view.GetType()
                .GetRuntimeMethods()
                .Select(x => new { method = x, methodParameters = x.GetParameters(), attributes = x.GetCustomAttributes<NavigationAttribute>() })
                // 属性を持っているかどうか
                .Where(x => x.attributes?.Any() ?? false)
                // 返り値がTaskかどうか
                .Where(x => x.method.ReturnType == typeof(Task))
                // 引数が1つかどうか
                .Where(x => x.methodParameters.Length == 1))
            {
                foreach (var attribute in info.attributes)
                {
                    object viewModel = getViewModel(attribute, viewModels);
                    INavigationRequest navigationRequest = getNavigationRequest(attribute, viewModel);

                    var method = info.method;
                    (string, Type[], Type) methodTuple = (method.Name, new Type[0], info.methodParameters[0].ParameterType);
                    items.Add((methodTuple, attribute, navigationRequest));
                }
            }

            var func = createDelegate(items, view);
            var navigationAction = new NavigationAction(func);

            foreach (var item in items)
            {
                item.Item3.NavigationAction = navigationAction;
            }
        }

        /// <summary>
        /// NavigationActionへ渡すためのデリゲートを作成
        /// </summary>
        /// <param name="items">デリゲートとするメソッド群に関する情報</param>
        /// <param name="instance">デリゲートとする実際のメソッド群が定義されているオブジェクトのインスタンス</param>
        /// <returns></returns>
        private Func<object, INavigationRequest, Task> createDelegate(
            List<((string name, Type[] typeArguments, Type parameter1Type) method, INavigationAttribute attribute, INavigationRequest request)> items,
            object instance)
        {
            // object x, INavigationRequest navigationRequest => {}
            ParameterExpression x = Expression.Parameter(typeof(object), "x");
            ParameterExpression navigationRequest = Expression.Parameter(typeof(INavigationRequest), "navigationRequest");
            // Task task;
            ParameterExpression task = Expression.Variable(typeof(Task), "task");

            // task = Task.CompletedTask;
            BinaryExpression taskAssignDefault = Expression.Assign(task, Expression.Constant(Task.CompletedTask));

            ConstantExpression instanceExpression = Expression.Constant(instance);
            // ifstatements + taskAssingDefalt + return task
            var statements = new Expression[items.Count + 2];
            statements[0] = taskAssignDefault;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                // if(x == item.request) task = instance.method<T>((T)x, TargetViewType);
                // if(x == item.request) task = instance.method((T)x);
                BinaryExpression test = Expression.Equal(navigationRequest, Expression.Constant(item.request));
                // get ""(T)""x;
                Type methodArgument1Type = item.method.parameter1Type;
                UnaryExpression castedX = Expression.Convert(x, methodArgument1Type);
                Type[] typeArguments = item.method.typeArguments;
                Expression[] methodArguments = getMethodArguments(castedX, item.attribute);
                MethodCallExpression method = Expression.Call(instanceExpression, item.method.name, typeArguments, methodArguments);
                BinaryExpression taskAssignResult = Expression.Assign(task, method);
                ConditionalExpression ifStatement = Expression.IfThen(test, taskAssignResult);
                // i + ""1"" is offset for taskAssginDefault
                statements[i + 1] = ifStatement;
            }
            // return Task task;
            statements[statements.Length - 1] = task;

            BlockExpression block = Expression.Block(typeof(Task), new[] { task }, statements);
            var lamda = Expression.Lambda<Func<object, INavigationRequest, Task>>(block, x, navigationRequest);

            return lamda.Compile();
        }

        /// <summary>
        /// メソッドの引数の配列を返却する
        /// </summary>
        /// <param name="x"></param>
        /// <param name="navigationAttribute"></param>
        /// <returns></returns>
        private Expression[] getMethodArguments(UnaryExpression x, INavigationAttribute navigationAttribute)
        {
            if (navigationAttribute is ITargetableNavigationAttribute targetableNavigationAttribute)
            {
                return new Expression[] { x, Expression.Constant(targetableNavigationAttribute.TargetViewType) };
            }
            return new[] { x };
        }
    }
}
