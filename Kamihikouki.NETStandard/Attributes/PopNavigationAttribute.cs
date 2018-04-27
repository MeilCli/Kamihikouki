using System;

namespace Kamihikouki.NETStandard.Attributes
{
    public class PopNavigationAttribute : BaseNavigationAttribute, ITargetableNavigationAttribute
    {
        public Type TargetViewType { get; }

        public PopNavigationAttribute(Type viewModelType, string bindingPropertyName, Type targetViewType)
            : base(viewModelType, bindingPropertyName)
        {
            TargetViewType = targetViewType;
        }
    }
}
