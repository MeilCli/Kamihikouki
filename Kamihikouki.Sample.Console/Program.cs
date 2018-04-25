using Kamihikouki.NETStandard;
using Kamihikouki.NETStandard.Navigation;
using System;
using System.Threading.Tasks;
using static System.Console;

namespace Kamihikouki.Sample.Console
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
            // 各プラットフォームで実装する必要がある
            var navigationProvider = new NavigationProvider();
            Navigator.Inject(navigationProvider, this, viewModel);
            viewModel.Paging();
        }

        [Navigation(typeof(ViewModel), nameof(ViewModel.Navigation))]
        Task navigate(int value)
        {
            WriteLine($"navigate value:{value}");
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
            WriteLine($"type: {typeof(TParam).FullName}, value:{parameter?.ToString()}, target:{targetViewType.FullName}");
            return Task.CompletedTask;
        }

        public override Task OnPushAsync<TParam>(TParam parameter, Type targetViewType)
        {
            WriteLine($"type: {typeof(TParam).FullName}, value:{parameter?.ToString()}, target:{targetViewType.FullName}");
            return Task.CompletedTask;
        }

        public override Task OnNavigateAsync<TParam>(TParam parameter)
        {
            WriteLine($"type: {typeof(TParam).FullName}, value:{parameter?.ToString()}");
            return Task.CompletedTask;
        }
    }
}
