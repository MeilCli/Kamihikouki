using Kamihikouki.NETStandard;
using System;
using System.Threading.Tasks;

namespace Kamihikouki.Platform.Android
{
    public class NavigationProvider : BaseNavigationProvider
    {
        public static string ArgumentKey { get; set; } = "kamihikouki_argument";

        private ITransitionProvider transitionProvider;

        public NavigationProvider(ITransitionProvider transitionProvider)
        {
            this.transitionProvider = transitionProvider;
        }

        public override Task OnPushAsync<TParam>(TParam parameter, Type targetView)
        {
            transitionProvider.Push<TParam>(parameter, targetView);
            return Task.CompletedTask;
        }

        public override Task OnPopAsync<TParam>(TParam parameter, Type targetView)
        {
            transitionProvider.Pop<TParam>(parameter, targetView);
            return Task.CompletedTask;
        }

        public override Task OnNavigateAsync<TParam>(TParam parameter)
        {
            throw new NotImplementedException();
        }
    }
}