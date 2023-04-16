using System;
using Shared.Config;
using Torch;
using Torch.Views;

namespace TorchPlugin
{
    [Serializable]
    public class PluginConfig : ViewModel, IPluginConfig
    {
        private bool enabled = true;
        private bool laserAntenna = false;
        //BOOL_OPTION private bool optionName = false;

        [Display(GroupName = "General", Name = "Enable plugin", Order = 1, Description = "Enables/disables the plugin")]
        public bool Enabled
        {
            get => enabled;
            set => SetValue(ref enabled, value);
        }
        [Display(Order = 8, GroupName = "Fixes", Name = "Fix laser antenna", Description = "Fixes laser antenna connectivity issues")]
        public bool LaserAntenna
        {
            get => laserAntenna;
            set => SetValue(ref laserAntenna, value);
        }
        /*BOOL_OPTION

        [Display(Order = 8, GroupName = "Fixes", Name = "Option label", Description = "Option tooltip")]
        public bool OptionName
        {
            get => optionName;
            set => SetValue(ref optionName, value);
        }
        BOOL_OPTION*/
    }
}