using System;

namespace Kamihikouki.NETStandard.Attributes
{
    public class PushNavigationAttribute : BaseNavigationAttribute, ITargetableNavigationAttribute
    {
        public Type TargetViewType { get; }

        public PushNavigationAttribute(Type viewModelType, string bindingPropertyName, Type targetViewType)
            : base(viewModelType, bindingPropertyName)
        {
            TargetViewType = targetViewType;
        }
    }
}
