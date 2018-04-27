using Kamihikouki.NETStandard;
using System;
using System.Threading.Tasks;

namespace Kamihikouki.Benchmark.Core
{
    class NavigationProvider : BaseNavigationProvider
    {

        public static NavigationProvider Instance { get; } = new NavigationProvider();

        public override Task OnNavigateAsync<TParam>(TParam parameter)
        {
            return Task.CompletedTask;
        }

        public override Task OnPopAsync<TParam>(TParam parameter, Type targetView)
        {
            return Task.CompletedTask;
        }

        public override Task OnPushAsync<TParam>(TParam parameter, Type targetView)
        {
            return Task.CompletedTask;
        }
    }
}
