using System;

namespace Kamihikouki.NETStandard.Attributes
{
    interface INavigationAttribute
    {
        Type ViewModelType { get; }

        string BindingPropertyName { get; }
    }
}
