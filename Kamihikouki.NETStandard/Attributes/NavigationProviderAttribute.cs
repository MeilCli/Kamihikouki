using System;

namespace Kamihikouki.NETStandard.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class NavigationProviderAttribute : Attribute
    {
        public Type NavigationAttributeType { get; private set; }

        public NavigationProviderAttribute(Type navigationAttributeType)
        {
            NavigationAttributeType = navigationAttributeType;
        }
    }
}
