using System.ComponentModel;

namespace Shared.Config
{
    public interface IPluginConfig: INotifyPropertyChanged
    {
        // Enables the plugin
        bool Enabled { get; set; }
        
        // Enables checking for code changes (disable this in the XML config file on Proton/Linux)
        bool DetectCodeChanges { get; set; }

        // Fixes crash due to NaN value in TurretControlBlock
        bool TurretNan { get; set; }
        
        // Fixes crash in AI (Automaton) blocks (requires restart)
        bool AiCrash { get; set; }

        // Fixes NullRef exception on saving world (requires restart)
        bool Serialize { get; set; }
        
        // Warn about crash due to OOM in MyPlanet (requires restart)
        bool VoxelOom { get; set; }
        
        /*BOOL_OPTION
        // Option tooltip
        bool OptionName { get; set; }
        
        BOOL_OPTION*/
    }
}