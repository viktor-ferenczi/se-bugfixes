using System.ComponentModel;

namespace Shared.Config
{
    public interface IPluginConfig: INotifyPropertyChanged
    {
        // Enables the plugin
        bool Enabled { get; set; }
        /*BOOL_OPTION

        // Option tooltip
        bool OptionName { get; set; }
        BOOL_OPTION*/
    }
}