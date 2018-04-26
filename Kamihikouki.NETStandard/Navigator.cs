using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Kamihikouki.NETStandard
{

    public class CachedNavigator : Navigator
    {
        public static new INavigator Inject(INavigationProvider navigationProvider, object view, params object[] viewModels)
        {
            var navigator = new CachedNavigator(navigationProvider);
            navigator.ClassInject(view, viewModels);
            navigator.MethodInject(view, viewModels);
            return navigator;
        }

        public CachedNavigator(INavigationProvider navigationProvider) : base(navigationProvider) { }

        private protected override Func<object, INavigationRequest, Task> CreateDelegate(
            (MethodInfo method, INavigationAttribute attribute, INavigationRequest request)[] items,
            object instance)
        {
            // object x, INavigationRequest navigationRequest => {}
            ParameterExpression x = Expression.Parameter(typeof(object), "x");
            ParameterExpression navigationRequest = Expression.Parameter(typeof(INavigationRequest), "navigationRequest");
            // Task task;
            ParameterExpression task = Expression.Variable(typeof(Task), "task");

            // task = Task.CompletedTask;
            BinaryExpression taskAssignDefault = Expression.Assign(task, Expression.Constant(Task.CompletedTask));

            var ifStatements = new List<ConditionalExpression>();
            foreach (var item in items)
            {
                // if(x == item.request) task = instance.method<T>((T)x, TargetViewType);
                // if(x == item.request) task = instance.method((T)x);
                BinaryExpression test = Expression.Equal(navigationRequest, Expression.Constant(item.request));
                Type typeArgument = GetTypeArgument(item.request);
                Type[] typeArguments = item.method.IsGenericMethod ? new[] { typeArgument } : new Type[0];
                UnaryExpression castedX = Expression.Convert(x, typeArgument);
                Expression[] methodArguments = getMethodArguments(castedX, item.attribute);
                MethodCallExpression method = Expression.Call(Expression.Constant(instance), item.method.Name, typeArguments, methodArguments);
                BinaryExpression taskAssignResult = Expression.Assign(task, method);
                ConditionalExpression ifStatement = Expression.IfThen(test, taskAssignResult);
                ifStatements.Add(ifStatement);
            }

            var statements = new List<Expression>();
            // task = Task.CompletedTask;
            statements.Add(taskAssignDefault);
            // ~~ statements ~~ 
            statements.AddRange(ifStatements);
            // return Task task;
            statements.Add(task);
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

    public class Navigator : INavigator
    {

        public static INavigator Inject(INavigationProvider navigationProvider, object view, params object[] viewModels)
        {
            var navigator = new Navigator(navigationProvider);
            navigator.ClassInject(view, viewModels);
            navigator.MethodInject(view, viewModels);
            return navigator;
        }

        public INavigationProvider NavigationProvider { private protected get; set; }

        public Navigator(INavigationProvider navigationProvider)
        {
            NavigationProvider = navigationProvider;
        }

        public void ClassInject(object view, params object[] viewModels)
        {
            if (NavigationProvider == null)
            {
                throw new InvalidOperationException($"{nameof(NavigationProvider)} is null");
            }

            var navigationAttributes = view.GetType().GetTypeInfo().GetCustomAttributes<BaseNavigationAttribute>().ToList();
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

            var items = new List<(MethodInfo, INavigationAttribute, INavigationRequest)>();
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
                Type typeArgument = GetTypeArgument(navigationRequest);
                MethodInfo method = navigationProviderMethod.method.MakeGenericMethod(typeArgument);

                items.Add((method, navigationAttribute, navigationRequest));
            }

            var func = CreateDelegate(items.ToArray(), NavigationProvider);
            var navigationAction = new NavigationAction(func);

            foreach (var item in items)
            {
                item.Item3.NavigationAction = navigationAction;
            }
        }

        public void MethodInject(object view, params object[] viewModels)
        {
            var navigationMethods = view.GetType()
                .GetRuntimeMethods()
                .Select(x => new { method = x, attributes = x.GetCustomAttributes<NavigationAttribute>() })
                // 属性を持っているかどうか
                .Where(x => x.attributes?.Any() ?? false)
                // 返り値がTaskかどうか
                .Where(x => x.method.ReturnType == typeof(Task))
                // 引数が1つかどうか
                .Where(x => x.method.GetParameters().Length == 1)
                .ToList();

            var items = new List<(MethodInfo, INavigationAttribute, INavigationRequest)>();
            foreach (var navigationMethod in navigationMethods)
            {
                foreach (var attribute in navigationMethod.attributes)
                {
                    object viewModel = getViewModel(attribute, viewModels);
                    INavigationRequest navigationRequest = getNavigationRequest(attribute, viewModel);

                    items.Add((navigationMethod.method, attribute, navigationRequest));
                }
            }

            var func = CreateDelegate(items.ToArray(), view);
            var navigationAction = new NavigationAction(func);

            foreach (var item in items)
            {
                item.Item3.NavigationAction = navigationAction;
            }
        }

        /// <summary>
        /// INavigationProviderで実装するメソッドに必要な引数の数を返す
        /// </summary>
        /// <param name="navigationAttribute"></param>
        /// <returns></returns>
        private int shoudMethodParameterCount(INavigationAttribute navigationAttribute)
        {
            if (navigationAttribute is ITargetableNavigationAttribute)
            {
                return 2;
            }
            return 1;
        }

        private object getViewModel(INavigationAttribute navigationAttribute, object[] viewModels)
        {
            return viewModels.FirstOrDefault(x => x.GetType() == navigationAttribute.ViewModelType)
                ?? throw new InvalidOperationException($"dose not have target ViewModel: {navigationAttribute.ViewModelType.FullName}");
        }

        private INavigationRequest getNavigationRequest(INavigationAttribute navigationAttribute, object viewModel)
        {
            PropertyInfo getter = navigationAttribute.ViewModelType.GetRuntimeProperty(navigationAttribute.BindingPropertyName)
                ?? throw new InvalidOperationException($"not implement Property: {navigationAttribute.BindingPropertyName}");
            if (getter.CanRead == false)
            {
                throw new InvalidOperationException("cannot read property");
            }
            return getter.GetValue(viewModel) as INavigationRequest
                ?? throw new InvalidOperationException($"property value is not {nameof(INavigationRequest)}");
        }

        private protected Type GetTypeArgument(INavigationRequest navigationRequest)
        {
            var genericInterfaceTypes = navigationRequest.GetType()
                .GetTypeInfo()
                .GetInterfaces()
                // 型パラメーターのあるインターフェースかどうか
                .Where(x => 0 < x.GenericTypeArguments.Length);
            foreach (var type in genericInterfaceTypes)
            {
                if (type.GetGenericTypeDefinition() == typeof(INavigationRequest<>))
                {
                    return type.GenericTypeArguments[0];
                }
            }
            return typeof(object);
        }

        /// <summary>
        /// NavigationActionへ渡すためのデリゲートを作成
        /// </summary>
        /// <param name="methodInfo">デリゲートとする実際のメソッドの情報</param>
        /// <param name="navigationAttribute"></param>
        /// <param name="instance">デリゲートとする実際のメソッドが定義されているオブジェクトのインスタンス</param>
        /// <returns></returns>
        private protected virtual Func<object, INavigationRequest, Task> CreateDelegate(
            (MethodInfo method, INavigationAttribute attribute, INavigationRequest request)[] items, object instance)
        {
            return (x, request) =>
            {
                var item = items.FirstOrDefault(y => y.request == request);
                // item == nullのチェックをしたいが、ValueTupleに==演算子が定義されていない

                if (item.attribute is ITargetableNavigationAttribute targetableNavigationAttribute)
                {
                    return (Task)item.method.Invoke(instance, new[] { x, targetableNavigationAttribute.TargetViewType });
                }
                return (Task)item.method.Invoke(instance, new[] { x });
            };

        }
    }
}
