using Kamihikouki.NETStandard.Navigation;
using System;
using System.Threading.Tasks;

namespace Kamihikouki.NETStandard
{
    public abstract class BaseNavigationProvider : INavigationProvider
    {
        [NavigationProvider(typeof(PushNavigationAttribute))]
        public abstract Task OnPushAsync<TParam>(TParam parameter, Type targetView);

        [NavigationProvider(typeof(PopNavigationAttribute))]
        public abstract Task OnPopAsync<TParam>(TParam parameter, Type targetView);

        [NavigationProvider(typeof(NavigationAttribute))]
        public abstract Task OnNavigateAsync<TParam>(TParam parameter);
    }
}
