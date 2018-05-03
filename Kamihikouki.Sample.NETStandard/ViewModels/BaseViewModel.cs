using Kamihikouki.NETStandard;
using System.ComponentModel;

namespace Kamihikouki.Sample.NETStandard.ViewModels
{
    public abstract class BaseViewModel : IStateSerializable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public abstract byte[] StateSerialize();

        public abstract void StateDeserialize(byte[] jsonRaw);
    }
}
