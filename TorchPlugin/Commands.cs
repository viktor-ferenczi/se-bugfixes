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

        private void RespondWithHelp()
        {
            Respond("Bugfixes commands:");
            Respond("  !pfi help");
            Respond("  !pfi info");
            Respond("    Prints the current configuration settings.");
            Respond("  !pfi enable");
            Respond("    Enables the plugin");
            Respond("  !pfi disable");
            Respond("    Disables the plugin");
            Respond("  !pfi fix <name> <value>");
            Respond("    Enables or disables a specific fix");
            RespondWithListOfFixes();
            Respond("Valid bool values:");
            Respond("  False: 0 off no n false f");
            Respond("  True: 1 on yes y false f");
        }

        private void RespondWithListOfFixes()
        {
            Respond("Valid fix names:");
            Respond("  turret_nan: Fixes crash due to NaN value in TurretControlBlock");
            Respond("  ai_crash: Fixes crash in AI (Automaton) blocks (requires restart)");
//BOOL_OPTION Respond("  option_name: Option tooltip");
        }

        private void RespondWithInfo()
        {
            var config = Plugin.Instance.Config;
            var status = config.Enabled ? "Enabled" : "Disabled";
            Respond($"Bugfixes plugin: {status}");
            Respond($"turret_nan: {Format(config.TurretNan)}");
            Respond($"ai_crash: {Format(config.AiCrash)}");
//BOOL_OPTION Respond($"option_name: {Format(config.OptionName)}");
        }

        // Custom formatters
        private static string Format(bool value) => value ? "Yes" : "No";

        // ReSharper disable once UnusedMember.Global
        [Command("bfi help", "Bugfixes: Prints help on usage")]
        [Permission(MyPromoteLevel.Admin)]
        public void Help()
        {
            RespondWithHelp();
        }

        // ReSharper disable once UnusedMember.Global
        [Command("bfi info", "Bugfixes: Prints the current settings")]
        [Permission(MyPromoteLevel.None)]
        public void Info()
        {
            RespondWithInfo();
        }

        // ReSharper disable once UnusedMember.Global
        [Command("bfi enable", "Bugfixes: Enables the plugin")]
        [Permission(MyPromoteLevel.Admin)]
        public void Enable()
        {
            Config.Enabled = true;
            RespondWithInfo();
        }

        // ReSharper disable once UnusedMember.Global
        [Command("bfi disable", "Bugfixes: Disables the plugin")]
        [Permission(MyPromoteLevel.Admin)]
        public void Disable()
        {
            Config.Enabled = false;
            RespondWithInfo();
        }

        // ReSharper disable once UnusedMember.Global
        [Command("bfi fix", "Bugfixes: Enables or disables a specific fix")]
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
                    Help();
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