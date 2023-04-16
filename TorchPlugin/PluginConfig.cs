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
        private bool detectCodeChanges = true;
        private bool turretNan = true;
private bool aiCrash = true;
//BOOL_OPTION private bool optionName = true;

        [Display(Order = 1, GroupName = "General", Name = "Enable plugin", Description = "Enables/disables the plugin")]
        public bool Enabled
        {
            get => enabled;
            set => SetValue(ref enabled, value);
        }
        
        [Display(Order = 2, GroupName = "General", Name = "Detect code changes", Description = "Disable the plugin if any changes to the game code are detected before patching")]
        public bool DetectCodeChanges
        {
            get => detectCodeChanges;
            set => SetValue(ref detectCodeChanges, value);
        }

        [Display(Order = 3, GroupName = "Fixes", Name = "Fix NaN crash in TurretControlBlock", Description = "Fixes crash due to NaN value in TurretControlBlock")]
        public bool TurretNan
        {
            get => turretNan;
            set => SetValue(ref turretNan, value);
        }
        [Display(Order = 4, GroupName = "Fixes", Name = "Fix crash in AI blocks", Description = "Fix crash in AI (Automaton) blocks (requires restart)")]
        public bool AiCrash
        {
            get => aiCrash;
            set => SetValue(ref aiCrash, value);
        }
        /*BOOL_OPTION

        [Display(Order = 5, GroupName = "Fixes", Name = "Option label", Description = "Option tooltip")]
        public bool OptionName
        {
            get => optionName;
            set => SetValue(ref optionName, value);
        }
        BOOL_OPTION*/
    }
}