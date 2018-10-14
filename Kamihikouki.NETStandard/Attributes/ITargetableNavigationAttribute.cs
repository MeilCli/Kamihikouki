using System;

namespace Kamihikouki.NETStandard.Attributes
{
    interface ITargetableNavigationAttribute : INavigationAttribute
    {
        Type TargetViewType { get; }
    }
}
