
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Kamihikouki.Platform.Android;
using Kamihikouki.Sample.NETStandard.ViewModels;
using Reactive.Bindings;

namespace Kamihikouki.Sample.Platform.Android
{
    [Activity]
    public class SubActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Sub);

            var countText = FindViewById<TextView>(Resource.Id.CountText);

            byte[] countViewModelStateJsonRaw = Intent.GetByteArrayExtra(NavigationProvider.ArgumentKey);
            var viewModel = new CountViewModel(countViewModelStateJsonRaw);

            countText.SetBinding(x => x.Text, viewModel.CountText);
        }
    }
}