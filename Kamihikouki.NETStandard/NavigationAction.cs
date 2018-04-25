using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kamihikouki.NETStandard
{
    class NavigationAction : INavigationAction
    {

        private Func<object, Task> func;

        public NavigationAction(Func<object, Task> func)
        {
            this.func = func;
        }

        public Task NavigateAsync<TParam>(TParam parameter = default)
        {
            return func(parameter);
        }
    }
}
