using System;

namespace Kamihikouki.NETStandard
{
    interface INavigationAttribute
    {
        Type ViewModelType { get; }

        string BindingPropertyName { get; }
    }
}
