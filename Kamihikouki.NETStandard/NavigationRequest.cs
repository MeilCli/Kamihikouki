using System;
using System.Threading.Tasks;

namespace Kamihikouki.NETStandard
{
    public class NavigationRequest : INavigationRequest
    {
        private INavigationAction _navigationAction;

        public INavigationAction NavigationAction {
            set => _navigationAction = value;
        }

        public Task RaiseAsync()
        {
            INavigationAction navigationAction = _navigationAction ?? throw new InvalidOperationException("NavigationAction is null");
            return navigationAction.NavigateAsync<object>();
        }
    }

    public class NavigationRequest<TParam> : INavigationRequest<TParam>
    {
        private INavigationAction _navigationAction;

        public INavigationAction NavigationAction {
            set => _navigationAction = value;
        }

        Task INavigationRequest.RaiseAsync()
        {
            return RaiseAsync(default);
        }

        public Task RaiseAsync(TParam parameter)
        {
            INavigationAction navigationAction = _navigationAction ?? throw new InvalidOperationException("NavigationAction is null");
            return navigationAction.NavigateAsync(parameter);
        }
    }
}
