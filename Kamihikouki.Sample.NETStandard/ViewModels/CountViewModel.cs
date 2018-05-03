using Kamihikouki.NETStandard;
using Kamihikouki.Sample.NETStandard.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utf8Json;

namespace Kamihikouki.Sample.NETStandard.ViewModels
{
    public class CountViewModel : BaseViewModel, IDisposable
    {
        private CountModel model;
        private CompositeDisposable disposables = new CompositeDisposable();

        public IReadOnlyReactiveProperty<string> CountText { get; private set; }
        public ReactiveCommand IncrementCommand { get; } = new ReactiveCommand();
        public ReactiveCommand DecrementCommand { get; } = new ReactiveCommand();
        public AsyncReactiveCommand StartNextViewCommand { get; } = new AsyncReactiveCommand();
        public NavigationRequest<CountViewModel> PushRequest { get; } = new NavigationRequest<CountViewModel>();

        public CountViewModel()
        {
            model = new CountModel();
            init();
        }

        public CountViewModel(byte[] stateJsonRaw)
        {
            stateDeserialize(stateJsonRaw);
            init();
        }

        private void init()
        {
            CountText = model.ObserveProperty(x => x.Count)
                .Select(x => $"Count: {x}")
                .ToReadOnlyReactivePropertySlim()
                .AddTo(disposables);
            IncrementCommand.Subscribe(x => model.Increment()).AddTo(disposables);
            DecrementCommand.Subscribe(x => model.Decrement()).AddTo(disposables);
            StartNextViewCommand
                .Subscribe(async x =>
                {
                    if (PushRequest.CanExecute())
                    {
                        await PushRequest.RaiseAsync(this);
                    }
                })
                .AddTo(disposables);
        }

        // StateDeserializeがvirtualかつコンストラクタで呼びたいのでprivateメソッドとして定義
        private void stateDeserialize(byte[] jsonRaw)
        {
            model = JsonSerializer.Deserialize<CountModel>(jsonRaw);
        }

        public override void StateDeserialize(byte[] jsonRaw)
        {
            stateDeserialize(jsonRaw);
        }

        public override byte[] StateSerialize()
        {
            return JsonSerializer.Serialize<CountModel>(model);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}
