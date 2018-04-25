using System;

namespace Kamihikouki.NETStandard
{
    interface ITargetableNavigationAttribute : INavigationAttribute
    {
        Type TargetViewType { get; }
    }
}
