using Kamihikouki.NETStandard.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace Kamihikouki.NETStandard
{
    public abstract class BaseNavigator : INavigator
    {

        private protected INavigationProvider NavigationProvider { get; set; }

        public BaseNavigator(INavigationProvider navigationProvider)
        {
            NavigationProvider = navigationProvider;
        }

        public abstract void MethodInject(object view, params object[] viewModels);
        public abstract void ClassInject(object view, params object[] viewModels);

        /// <summary>
        /// INavigationProviderで実装するメソッドに必要な引数の数を返す
        /// </summary>
        /// <param name="navigationAttribute"></param>
        /// <returns></returns>
        private protected int shoudMethodParameterCount(INavigationAttribute navigationAttribute)
        {
            if (navigationAttribute is ITargetableNavigationAttribute)
            {
                return 2;
            }
            return 1;
        }

        private protected object getViewModel(INavigationAttribute navigationAttribute, object[] viewModels)
        {
            foreach (var viewModel in viewModels)
            {
                if (viewModel.GetType() == navigationAttribute.ViewModelType)
                {
                    return viewModel;
                }
            }
            throw new InvalidOperationException($"dose not have target ViewModel: {navigationAttribute.ViewModelType.FullName}");
        }

        private protected INavigationRequest getNavigationRequest(INavigationAttribute navigationAttribute, object viewModel)
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
    }
}
