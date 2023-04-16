using System.ComponentModel;

namespace Shared.Config
{
    public interface IPluginConfig: INotifyPropertyChanged
    {
        // Enables the plugin
        bool Enabled { get; set; }
        
        // Fixes crash due to NaN value in TurretControlBlock
        bool TurretNan { get; set; }
        /*BOOL_OPTION

        // Option tooltip
        bool OptionName { get; set; }
        BOOL_OPTION*/
    }
}