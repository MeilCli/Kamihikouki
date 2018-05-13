using Kamihikouki.Sample.NETStandard.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kamihikouki.Sample.XamarinForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubPage : ContentPage
    {
        public SubPage(CountViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}