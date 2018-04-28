using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Jobs;
using Kamihikouki.NETStandard;
using Kamihikouki.NETStandard.Attributes;
using BBehchmark = BenchmarkDotNet.Attributes.BenchmarkAttribute;

namespace Kamihikouki.Benchmark.Core
{
    [CoreJob]
    [MeanColumn, MinColumn, MaxColumn]
    [MemoryDiagnoser]
    public class NavigatorType
    {
        private NavigationProvider navigationProvider = NavigationProvider.Instance;
        private NavigatorTypeView injectView = new NavigatorTypeView();
        private NavigatorTypeView cachedRaiseView = new NavigatorTypeView();
        private NavigatorTypeView reflectionRaiseView = new NavigatorTypeView();

        public NavigatorType()
        {
            cachedRaiseView.CachedInject(navigationProvider);
            reflectionRaiseView.ReflectionInject(navigationProvider);
        }

        [BBehchmark]
        public void CachedInject()
        {
            injectView.CachedInject(navigationProvider);
        }

        [BBehchmark]
        public void ReflectionInject()
        {
            reflectionRaiseView.ReflectionInject(navigationProvider);
        }

        [BBehchmark]
        public void CachedRaise()
        {
            cachedRaiseView.Raise();
        }

        [BBehchmark]
        public void ReflectionRaise()
        {
            reflectionRaiseView.Raise();
        }
    }

    [Navigation(typeof(NavigatorTypeViewModel), nameof(NavigatorTypeViewModel.Request))]
    class NavigatorTypeView
    {
        private NavigatorTypeViewModel viewModel = new NavigatorTypeViewModel();

        public void CachedInject(NavigationProvider navigationProvider)
        {
            new CachedNavigator(navigationProvider).ClassInject(this, viewModel);
        }

        public void ReflectionInject(NavigationProvider navigationProvider)
        {
            new Navigator(navigationProvider).ClassInject(this, viewModel);
        }

        public void Raise()
        {
            viewModel.Raise();
        }
    }

    class NavigatorTypeViewModel
    {
        public INavigationRequest Request { get; } = new NavigationRequest();

        public void Raise()
        {
            Request.RaiseAsync();
        }
    }
}
