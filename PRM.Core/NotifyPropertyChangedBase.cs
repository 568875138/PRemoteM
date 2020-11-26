﻿using System.ComponentModel;
using System.Windows.Forms.VisualStyles;

namespace PRM.Core
{
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        protected bool NotifyPropertyChangedEnabled = true;

        public void SetNotifyPropertyChangedEnabled(bool isEnabled)
        {
            NotifyPropertyChangedEnabled = isEnabled;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (NotifyPropertyChangedEnabled)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void SetAndNotifyIfChanged<T>(string propertyName, ref T oldValue, T newValue)
        {
            if (oldValue == null && newValue == null) return;
            if (oldValue != null && oldValue.Equals(newValue)) return;
            if (newValue != null && newValue.Equals(oldValue)) return;
            oldValue = newValue;
            RaisePropertyChanged(propertyName);
        }


        protected virtual string[] GetStrArgs(object arg)
        {
            var paramstr = arg as string;
            return paramstr.Split('\r');
        }
    }
}