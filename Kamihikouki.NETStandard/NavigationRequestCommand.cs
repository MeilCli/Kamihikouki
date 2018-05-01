using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kamihikouki.NETStandard
{
    public class NavigationRequestCommand : INavigationRequest, ICommand, IDisposable
    {
        public event EventHandler CanExecuteChanged;

        public INavigationAction NavigationAction { private get; set; }

        private IDisposable sourceDisposable;
        private bool canExecute;
        private bool isDisposed;

        public NavigationRequestCommand(bool initialCanExcute = true)
        {
            if (initialCanExcute)
            {
                canExecute = initialCanExcute;
                CanExecuteChanged.Invoke(this, EventArgs.Empty);
            }
        }

        public NavigationRequestCommand(IObservable<bool> canExecuteSource, bool initialCanExcute = true)
        {
            if (initialCanExcute)
            {
                canExecute = initialCanExcute;
                CanExecuteChanged.Invoke(this, EventArgs.Empty);
            }
            sourceDisposable = canExecuteSource.Subscribe(new Observer(this));
        }

        bool ICommand.CanExecute(object parameter)
        {
            return canExecute && NavigationAction != null;
        }

        public bool CanExecute()
        {
            return canExecute && NavigationAction != null;
        }

        async void ICommand.Execute(object parameter)
        {
            await RaiseAsync();
        }

        public async void Execute()
        {
            await RaiseAsync();
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }
            isDisposed = true;
            sourceDisposable?.Dispose();
            canExecute = false;
        }

        public Task RaiseAsync()
        {
            INavigationAction navigationAction = NavigationAction ?? throw new InvalidOperationException("NavigationAction is null");
            return navigationAction.NavigateAsync<object>(this);
        }

        private class Observer : IObserver<bool>
        {
            private NavigationRequestCommand navigationRequestCommand;

            public Observer(NavigationRequestCommand navigationRequestCommand)
            {
                this.navigationRequestCommand = navigationRequestCommand;
            }

            public void OnCompleted()
            {
                navigationRequestCommand.canExecute = false;
                navigationRequestCommand.CanExecuteChanged.Invoke(navigationRequestCommand, EventArgs.Empty);
            }

            public void OnError(Exception error)
            {
                navigationRequestCommand.canExecute = false;
                navigationRequestCommand.CanExecuteChanged.Invoke(navigationRequestCommand, EventArgs.Empty);
            }

            public void OnNext(bool value)
            {
                navigationRequestCommand.canExecute = value;
                navigationRequestCommand.CanExecuteChanged.Invoke(navigationRequestCommand, EventArgs.Empty);
            }
        }
    }

    public class NavigationRequestCommand<T> : INavigationRequest<T>, ICommand, IDisposable
    {
        public event EventHandler CanExecuteChanged;

        public INavigationAction NavigationAction { private get; set; }

        private IDisposable sourceDisposable;
        private bool canExecute;
        private bool isDisposed;

        public NavigationRequestCommand(bool initialCanExcute = true)
        {
            if (initialCanExcute)
            {
                canExecute = initialCanExcute;
                CanExecuteChanged.Invoke(this, EventArgs.Empty);
            }
        }

        public NavigationRequestCommand(IObservable<bool> canExecuteSource, bool initialCanExcute = true)
        {
            if (initialCanExcute)
            {
                canExecute = initialCanExcute;
                CanExecuteChanged.Invoke(this, EventArgs.Empty);
            }
            sourceDisposable = canExecuteSource.Subscribe(new Observer(this));
        }

        bool ICommand.CanExecute(object parameter)
        {
            return canExecute && NavigationAction != null;
        }

        public bool CanExecute()
        {
            return canExecute && NavigationAction != null;
        }

        async void ICommand.Execute(object parameter)
        {
            await RaiseAsync((T)parameter);
        }

        public async void Execute(T parameter)
        {
            await RaiseAsync(parameter);
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }
            isDisposed = true;
            sourceDisposable?.Dispose();
            canExecute = false;
        }

        Task INavigationRequest.RaiseAsync()
        {
            return RaiseAsync(default);
        }

        public Task RaiseAsync(T param)
        {
            INavigationAction navigationAction = NavigationAction ?? throw new InvalidOperationException("NavigationAction is null");
            return navigationAction.NavigateAsync(this, param);
        }

        private class Observer : IObserver<bool>
        {
            private NavigationRequestCommand<T> navigationRequestCommand;

            public Observer(NavigationRequestCommand<T> navigationRequestCommand)
            {
                this.navigationRequestCommand = navigationRequestCommand;
            }

            public void OnCompleted()
            {
                navigationRequestCommand.canExecute = false;
                navigationRequestCommand.CanExecuteChanged.Invoke(navigationRequestCommand, EventArgs.Empty);
            }

            public void OnError(Exception error)
            {
                navigationRequestCommand.canExecute = false;
                navigationRequestCommand.CanExecuteChanged.Invoke(navigationRequestCommand, EventArgs.Empty);
            }

            public void OnNext(bool value)
            {
                navigationRequestCommand.canExecute = value;
                navigationRequestCommand.CanExecuteChanged.Invoke(navigationRequestCommand, EventArgs.Empty);
            }
        }
    }

    public static class NavigationRequestCommandExtension
    {
        public static NavigationRequestCommand ToNavigationRequestCommand(this IObservable<bool> canExecuteSource, bool initialCanExcute = true)
        {
            return new NavigationRequestCommand(canExecuteSource, initialCanExcute);
        }

        public static NavigationRequestCommand<T> ToNavigationRequestCommand<T>(this IObservable<bool> canExecuteSource, bool initialCanExcute = true)
        {
            return new NavigationRequestCommand<T>(canExecuteSource, initialCanExcute);
        }
    }
}
