using System;

namespace Kamihikouki.NETStandard.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class NavigationAttribute : BaseNavigationAttribute
    {
        public NavigationAttribute(Type viewModelType, string bindingPropertyName)
            : base(viewModelType, bindingPropertyName) { }
    }
}
