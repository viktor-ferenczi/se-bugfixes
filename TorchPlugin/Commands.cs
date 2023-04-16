using Shared.Config;
using Shared.Plugin;
using Torch.Commands;
using Torch.Commands.Permissions;
using VRage.Game.ModAPI;

namespace TorchPlugin
{
    public class Commands : CommandModule
    {
        private static IPluginConfig Config => Common.Config;

        private void Respond(string message)
        {
            Context?.Respond(message);
        }

        private void RespondWithInfo()
        {
            var config = Plugin.Instance.Config;
            Respond($"Bugfixes plugin is enabled: {Format(config.Enabled)}");
            Respond($"turret_nan: {Format(config.TurretNan)}");
Respond($"ai_crash: {Format(config.AiCrash)}");
//BOOL_OPTION Respond($"option_name: {Format(config.OptionName)}");
        }

        // Custom formatters
        private static string Format(bool value) => value ? "Yes" : "No";

        // ReSharper disable once UnusedMember.Global
        [Command("Bugfixes info", "Bugfixes: Prints the current settings")]
        [Permission(MyPromoteLevel.None)]
        public void Info()
        {
            RespondWithInfo();
        }

        // ReSharper disable once UnusedMember.Global
        [Command("Bugfixes enable", "Bugfixes: Enables the plugin")]
        [Permission(MyPromoteLevel.Admin)]
        public void Enable()
        {
            Config.Enabled = true;
            RespondWithInfo();
        }

        // ReSharper disable once UnusedMember.Global
        [Command("Bugfixes disable", "Bugfixes: Disables the plugin")]
        [Permission(MyPromoteLevel.Admin)]
        public void Disable()
        {
            Config.Enabled = false;
            RespondWithInfo();
        }

        // ReSharper disable once UnusedMember.Global
        [Command("fix", "Enables or disables a fix")]
        [Permission(MyPromoteLevel.Admin)]
        public void Fix(string name, string flag)
        {
            if (!TryParseBool(flag, out var parsedFlag))
            {
                Respond($"Invalid boolean value: {flag}");
                return;
            }

            switch (name)
            {
                case "turret_nan":
                    Config.TurretNan = parsedFlag;
                    break;

                case "ai_crash":
                    Config.AiCrash = parsedFlag;
                    break;

                /*BOOL_OPTION
                case "option_name":
                    Config.OptionName = parsedFlag;
                    break;

                BOOL_OPTION*/
                default:
                    Respond($"Unknown fix: {name}");
                    Respond($"Valid fix names:");
                    Respond($"  laser_antenna");
                    Respond($"  turret_nan");
Respond($"  ai_crash");
//BOOL_OPTION Respond($"  option_name");
                    return;
            }

            RespondWithInfo();
        }
        
        // Helper methods
        private static bool TryParseBool(string text, out bool result)
        {
            switch (text.ToLower())
            {
                case "1":
                case "on":
                case "yes":
                case "y":
                case "true":
                case "t":
                    result = true;
                    return true;

                case "0":
                case "off":
                case "no":
                case "n":
                case "false":
                case "f":
                    result = false;
                    return true;
            }

            result = false;
            return false;
        }
    }
}