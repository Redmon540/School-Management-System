using PropertyChanged;
using System;
using System.ComponentModel;

namespace School_Manager
{
    [AddINotifyPropertyChangedInterface]
    [Serializable]
    public class BasePropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void OnPropertyChanged(string Name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(Name));
        }
    }
}
