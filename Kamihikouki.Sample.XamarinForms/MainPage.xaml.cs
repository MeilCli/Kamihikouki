using Kamihikouki.NETStandard;
using Kamihikouki.NETStandard.Attributes;
using Kamihikouki.Sample.NETStandard.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kamihikouki.Sample.XamarinForms
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var viewModel = new CountViewModel();
            BindingContext = viewModel;
            var navigator = new CachedNavigator(null);
            navigator.MethodInject(this, viewModel);
        }

        [Navigation(typeof(CountViewModel), nameof(CountViewModel.PushRequest))]
        public Task PushSubPage(CountViewModel viewModel)
        {
            return Navigation.PushModalAsync(new SubPage(viewModel));
        }
    }
}
