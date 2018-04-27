using System;
using System.Threading.Tasks;

namespace Kamihikouki.NETStandard
{
    class NavigationAction : INavigationAction
    {

        private Func<object, INavigationRequest, Task> func;

        public NavigationAction(Func<object, INavigationRequest, Task> func)
        {
            this.func = func;
        }

        public Task NavigateAsync<TParam>(INavigationRequest navigationRequest, TParam parameter = default)
        {
            return func(parameter, navigationRequest);
        }
    }
}
