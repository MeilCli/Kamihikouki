using System;

namespace Kamihikouki.NETStandard.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class BaseNavigationAttribute : Attribute, INavigationAttribute
    {

        public Type ViewModelType { get; private set; }

        public string BindingPropertyName { get; private set; }

        public BaseNavigationAttribute(Type viewModelType, string bindingPropertyName)
        {
            ViewModelType = viewModelType;
            BindingPropertyName = bindingPropertyName;
        }
    }
}
