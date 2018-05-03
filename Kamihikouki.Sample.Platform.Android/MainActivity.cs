using Android.App;
using Android.OS;
using Android.Widget;
using Kamihikouki.NETStandard;
using Kamihikouki.NETStandard.Attributes;
using Kamihikouki.Platform.Android;
using Kamihikouki.Sample.NETStandard.ViewModels;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Kamihikouki.Sample.Platform.Android
{
    [Activity(Label = "Kamihikouki.Sample.Platform.Android", MainLauncher = true)]
    [PushNavigation(typeof(CountViewModel), nameof(CountViewModel.PushRequest), typeof(SubActivity))]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            var countText = FindViewById<TextView>(Resource.Id.CountText);
            var incrementButton = FindViewById<Button>(Resource.Id.IncrementButton);
            var decrementButton = FindViewById<Button>(Resource.Id.DecrementButton);
            var startNextViewButton = FindViewById<Button>(Resource.Id.StartNextViewButton);

            var viewModel = new CountViewModel();

            // TextView.Text'setterを使わなければLinkerがsetterを削除してしまう
            countText.Text = "";
            countText.SetBinding(x => x.Text, viewModel.CountText);
            incrementButton.ClickAsObservable().SetCommand(viewModel.IncrementCommand);
            decrementButton.ClickAsObservable().SetCommand(viewModel.DecrementCommand);
            startNextViewButton.ClickAsObservable().SetCommand(viewModel.StartNextViewCommand);

            INavigationProvider navigationProvider = new NavigationProvider(new ActivityTransitionProvider(this));
            INavigator navigator = new CachedNavigator(navigationProvider);
            navigator.ClassInject(this, viewModel);
        }
    }
}

