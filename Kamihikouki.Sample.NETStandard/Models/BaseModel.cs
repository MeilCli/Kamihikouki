using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kamihikouki.Sample.NETStandard.Models
{
    public class BaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T variable, T value, [CallerMemberName]string name = "")
        {
            if (EqualityComparer<T>.Default.Equals(variable, value)) return false;
            variable = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            return true;
        }
    }
}
