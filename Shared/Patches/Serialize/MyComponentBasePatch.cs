// NullReferenceException in MyComponentBase.Serialize on saving game
// https://support.keenswh.com/spaceengineers/pc/topic/27952-servers-crash-on-player-join-1-202

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Shared.Config;
using Shared.Logging;
using Shared.Plugin;
using Shared.Tools;
using VRage.Game.Components;
using VRage.Game.ObjectBuilders.ComponentSystem;

namespace Shared.Patches.Serialize
{
    // ReSharper disable once UnusedType.Global
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [HarmonyPatch(typeof(MyComponentBase))]
    public static class MyComponentBasePatch
    {
        private static IPluginLogger Log => Common.Logger;
        private static IPluginConfig Config => Common.Config;
        private static bool enabled;

        public static void Configure()
        {
            enabled = Config.Enabled && Config.Serialize;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(MyComponentBase.Serialize))]
        private static IEnumerable<CodeInstruction> SerializeTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
        {
            if (!enabled)
                return instructions;
            
            var il = instructions.ToList();

            if (il.HashInstructionsHex() != "d72a00d2")
            {
                Log.Warning($"{nameof(MyComponentBase)}.{nameof(SerializeTranspiler)}: Code change detected, ignoring patch (this should be harmless)");
                return il;
            }
            
            il.RecordOriginalCode();

            il = PatchFromDude(il, gen).ToList();

            il.RecordPatchedCode();
            return il;
        }

        private static IEnumerable<CodeInstruction> PatchFromDude(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
        {
            var label = gen.DefineLabel();
            
            gen.DeclareLocal(typeof(MyObjectBuilder_ComponentBase));
            
            foreach (var code in instructions)
            {
                if (code.opcode == OpCodes.Dup)
                {
                    yield return new CodeInstruction(OpCodes.Stloc_0);
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Brfalse, label);
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                }
                else if (code.opcode == OpCodes.Ret)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_0).WithLabels(label);
                    yield return code;
                }
                else
                {
                    yield return code;
                }
            }
        }
    }
}