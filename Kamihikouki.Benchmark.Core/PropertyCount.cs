using BenchmarkDotNet.Attributes.Jobs;
using Kamihikouki.NETStandard;
using Kamihikouki.NETStandard.Attributes;

using BBehchmark = BenchmarkDotNet.Attributes.BenchmarkAttribute;

namespace Kamihikouki.Benchmark.Core
{

    [CoreJob]
    public class PropertyCount
    {
        private NavigationProvider navigationProvider = NavigationProvider.Instance;
        private PropertyCountView5 view5 = new PropertyCountView5();
        private PropertyCountView10 view10 = new PropertyCountView10();

        [BBehchmark]
        public void PropertyCount5()
        {
            for (int i = 0; i < 10; i++)
            {
                view5.Inject(navigationProvider);
            }
        }

        [BBehchmark]
        public void PropertyCount10()
        {
            for (int i = 0; i < 10; i++)
            {
                view10.Inject(navigationProvider);
            }
        }
    }

    [Navigation(typeof(PropertyCountViewModel5), nameof(PropertyCountViewModel5.Request1))]
    [Navigation(typeof(PropertyCountViewModel5), nameof(PropertyCountViewModel5.Request2))]
    [Navigation(typeof(PropertyCountViewModel5), nameof(PropertyCountViewModel5.Request3))]
    [Navigation(typeof(PropertyCountViewModel5), nameof(PropertyCountViewModel5.Request4))]
    [Navigation(typeof(PropertyCountViewModel5), nameof(PropertyCountViewModel5.Request5))]
    class PropertyCountView5
    {
        private PropertyCountViewModel5 viewModel = new PropertyCountViewModel5();

        public void Inject(NavigationProvider navigationProvider)
        {
            new Navigator(navigationProvider).ClassInject(this, viewModel);
        }
    }

    [Navigation(typeof(PropertyCountViewModel10), nameof(PropertyCountViewModel10.Request1))]
    [Navigation(typeof(PropertyCountViewModel10), nameof(PropertyCountViewModel10.Request2))]
    [Navigation(typeof(PropertyCountViewModel10), nameof(PropertyCountViewModel10.Request3))]
    [Navigation(typeof(PropertyCountViewModel10), nameof(PropertyCountViewModel10.Request4))]
    [Navigation(typeof(PropertyCountViewModel10), nameof(PropertyCountViewModel10.Request5))]
    [Navigation(typeof(PropertyCountViewModel10), nameof(PropertyCountViewModel10.Request6))]
    [Navigation(typeof(PropertyCountViewModel10), nameof(PropertyCountViewModel10.Request7))]
    [Navigation(typeof(PropertyCountViewModel10), nameof(PropertyCountViewModel10.Request8))]
    [Navigation(typeof(PropertyCountViewModel10), nameof(PropertyCountViewModel10.Request9))]
    [Navigation(typeof(PropertyCountViewModel10), nameof(PropertyCountViewModel10.Request10))]
    class PropertyCountView10
    {
        private PropertyCountViewModel10 viewModel = new PropertyCountViewModel10();

        public void Inject(NavigationProvider navigationProvider)
        {
            new Navigator(navigationProvider).ClassInject(this, viewModel);
        }
    }

    class PropertyCountViewModel5
    {
        public INavigationRequest Request1 { get; } = new NavigationRequest();
        public INavigationRequest Request2 { get; } = new NavigationRequest();
        public INavigationRequest Request3 { get; } = new NavigationRequest();
        public INavigationRequest Request4 { get; } = new NavigationRequest();
        public INavigationRequest Request5 { get; } = new NavigationRequest();
    }

    class PropertyCountViewModel10
    {
        public INavigationRequest Request1 { get; } = new NavigationRequest();
        public INavigationRequest Request2 { get; } = new NavigationRequest();
        public INavigationRequest Request3 { get; } = new NavigationRequest();
        public INavigationRequest Request4 { get; } = new NavigationRequest();
        public INavigationRequest Request5 { get; } = new NavigationRequest();
        public INavigationRequest Request6 { get; } = new NavigationRequest();
        public INavigationRequest Request7 { get; } = new NavigationRequest();
        public INavigationRequest Request8 { get; } = new NavigationRequest();
        public INavigationRequest Request9 { get; } = new NavigationRequest();
        public INavigationRequest Request10 { get; } = new NavigationRequest();
    }
}
