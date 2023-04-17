// NullReferenceException in MyOffensiveWithWeaponsCombatComponent
// https://support.keenswh.com/spaceengineers/pc/topic/28104-nullreferenceexception-in-myoffensivewithweaponscombatcomponent

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using Sandbox.Game.Entities;
using Shared.Config;
using Shared.Logging;
using Shared.Plugin;
using Shared.Tools;
using SpaceEngineers.Game.EntityComponents.Blocks;

namespace Shared.Patches.AICrash
{
    // ReSharper disable once UnusedType.Global
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [HarmonyPatch(typeof(MyOffensiveWithWeaponsCombatComponent))]
    public static class MyOffensiveWithWeaponsCombatComponentPatch
    {
        private static IPluginLogger Log => Common.Logger;
        private static IPluginConfig Config => Common.Config;
        private static bool enabled;

        public static void Configure()
        {
            enabled = Config.Enabled && Config.TurretNan;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(MyOffensiveCombatAbstractComponent.OnBeforeRemovedFromContainer))]
        private static IEnumerable<CodeInstruction> OnBeforeRemovedFromContainerTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            if (!enabled)
                return instructions;

            var il = instructions.ToList();
            
            if (il.HashInstructionsHex() != "c45f29b9")
            {
                Log.Warning($"{nameof(MyOffensiveWithWeaponsCombatComponentPatch)}.{nameof(OnBeforeRemovedFromContainerTranspiler)}: Code change detected [{il.HashInstructionsHex()}], ignoring patch (this should be harmless)");
                return il;
            }
            
            il.RecordOriginalCode();

            var eventInfo = typeof(MyCubeGrid).GetEvent(nameof(MyCubeGrid.OnConnectionChangeCompleted), AccessTools.allDeclared);
            Debug.Assert(eventInfo != null);
            il = il.MethodReplacer(eventInfo.AddMethod, eventInfo.RemoveMethod).ToList();

            il.RecordPatchedCode();
            return il;
        }
    }
}