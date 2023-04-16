using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#if !TORCH

namespace Shared.Config
{
    public class PluginConfig: IPluginConfig
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void SetValue<T>(ref T field, T value, [CallerMemberName] string propName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            field = value;

            OnPropertyChanged(propName);
        }

        private void OnPropertyChanged([CallerMemberName] string propName = "")
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;

            propertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private bool enabled = true;
        private bool laserAntenna = true;
        //BOOL_OPTION private bool optionName = true;

        public bool Enabled
        {
            get => enabled;
            set => SetValue(ref enabled, value);
        }
        public bool LaserAntenna
        {
            get => laserAntenna;
            set => SetValue(ref laserAntenna, value);
        }
        /*BOOL_OPTION

        public bool OptionName
        {
            get => optionName;
            set => SetValue(ref optionName, value);
        }
        BOOL_OPTION*/
    }
}

#endif