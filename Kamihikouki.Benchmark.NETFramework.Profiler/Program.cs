using Kamihikouki.NETStandard;
using Kamihikouki.NETStandard.Attributes;
using System;
using System.Threading.Tasks;

namespace Kamihikouki.Benchmark.NETFramework.Profiler
{
    class Program
    {
        static void Main(string[] args)
        {
            var view = new View();
            view.Main();
        }
    }

    class TargetView { }

    [PushNavigation(typeof(ViewModel), nameof(ViewModel.PushNavigation), typeof(TargetView))]
    [Navigation(typeof(ViewModel), nameof(ViewModel.PopNavigation))]
    class View
    {
        public void Main()
        {
            var viewModel = new ViewModel();
            var navigationProvider = new NavigationProvider();
            CachedNavigator.Inject(navigationProvider, this, viewModel);
            // Navigator.Inject(navigationProvider, this, viewModel);
            viewModel.Paging();
            Console.ReadKey();
        }

        [Navigation(typeof(ViewModel), nameof(ViewModel.Navigation))]
        Task navigate(int value)
        {
            return Task.CompletedTask;
        }
    }

    class ViewModel
    {
        public INavigationRequest PushNavigation { get; } = new NavigationRequest();
        public INavigationRequest<string> PopNavigation { get; } = new NavigationRequest<string>();

        public INavigationRequest<int> Navigation { get; } = new NavigationRequest<int>();

        public void Paging()
        {
            PushNavigation.RaiseAsync();
            PopNavigation.RaiseAsync("pop");
            Navigation.RaiseAsync(100);
        }
    }

    class NavigationProvider : BaseNavigationProvider
    {
        public override Task OnPopAsync<TParam>(TParam parameter, Type targetViewType)
        {
            return Task.CompletedTask;
        }

        public override Task OnPushAsync<TParam>(TParam parameter, Type targetViewType)
        {
            return Task.CompletedTask;
        }

        public override Task OnNavigateAsync<TParam>(TParam parameter)
        {
            return Task.CompletedTask;
        }
    }
}
