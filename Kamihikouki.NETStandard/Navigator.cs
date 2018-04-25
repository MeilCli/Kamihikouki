using System;
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

        private protected override Func<object, Task> CreateDelegate(
            MethodInfo methodInfo, INavigationAttribute navigationAttribute, object instance)
        {
            if (navigationAttribute is ITargetableNavigationAttribute targetableNavigationAttribute)
            {
                // var targetViewType = targetableNavigationAttribute.TargetViewType;
                // x => (Task)instance.method((T)x,Type targetViewType);
                ParameterExpression x = Expression.Parameter(typeof(object), "x");
                Expression castedX = Expression.Convert(x, methodInfo.GetParameters()[0].ParameterType);
                ConstantExpression targetViewType = Expression.Constant(targetableNavigationAttribute.TargetViewType);
                MethodCallExpression method = Expression.Call(Expression.Constant(instance), methodInfo, castedX, targetViewType);
                Expression castedMethod = Expression.Convert(method, typeof(Task));
                var lambda = Expression.Lambda<Func<object, Task>>(castedMethod, x);
                return lambda.Compile();
            }
            {
                // x => (Task)instance.method((T)x);
                ParameterExpression x = Expression.Parameter(typeof(object), "x");
                Expression castedX = Expression.Convert(x, methodInfo.GetParameters()[0].ParameterType);
                MethodCallExpression method = Expression.Call(Expression.Constant(instance), methodInfo, castedX);
                Expression castedMethod = Expression.Convert(method, typeof(Task));
                var lambda = Expression.Lambda<Func<object, Task>>(castedMethod, x);
                return lambda.Compile();
            }
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
                Type typeArgument = getTypeArgument(navigationRequest);

                MethodInfo method = navigationProviderMethod.method.MakeGenericMethod(typeArgument);
                // ToDo: cache
                Func<object, Task> func = CreateDelegate(method, navigationAttribute, NavigationProvider);
                var navigationAction = new NavigationAction(func);
                navigationRequest.NavigationAction = navigationAction;
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
            foreach (var navigationMethod in navigationMethods)
            {
                foreach (var attribute in navigationMethod.attributes)
                {
                    // ToDo: cache
                    Func<object, Task> func = CreateDelegate(navigationMethod.method, attribute, view);
                    var navigationAction = new NavigationAction(func);
                    object viewModel = getViewModel(attribute, viewModels);
                    INavigationRequest navigationRequest = getNavigationRequest(attribute, viewModel);
                    navigationRequest.NavigationAction = navigationAction;
                }
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

        private Type getTypeArgument(INavigationRequest navigationRequest)
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
        private protected virtual Func<object, Task> CreateDelegate(MethodInfo methodInfo, INavigationAttribute navigationAttribute, object instance)
        {
            if (navigationAttribute is ITargetableNavigationAttribute targetableNavigationAttribute)
            {
                return x => (Task)methodInfo.Invoke(instance, new[] { x, targetableNavigationAttribute.TargetViewType });
            }
            return x => (Task)methodInfo.Invoke(instance, new[] { x });
        }
    }
}
