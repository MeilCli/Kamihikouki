using System;
using System.Threading.Tasks;

namespace Kamihikouki.NETStandard
{
    public class NavigationRequest : INavigationRequest
    {
        public INavigationAction NavigationAction { private get; set; }

        public bool CanExecute()
        {
            return NavigationAction != null;
        }

        public Task RaiseAsync()
        {
            INavigationAction navigationAction = NavigationAction ?? throw new InvalidOperationException("NavigationAction is null");
            return navigationAction.NavigateAsync<object>(this);
        }
    }

    public class NavigationRequest<TParam> : INavigationRequest<TParam>
    {
        public INavigationAction NavigationAction { private get; set; }

        public bool CanExecute()
        {
            return NavigationAction != null;
        }

        Task INavigationRequest.RaiseAsync()
        {
            return RaiseAsync(default);
        }

        public Task RaiseAsync(TParam parameter)
        {
            INavigationAction navigationAction = NavigationAction ?? throw new InvalidOperationException("NavigationAction is null");
            return navigationAction.NavigateAsync(this, parameter);
        }
    }
}
