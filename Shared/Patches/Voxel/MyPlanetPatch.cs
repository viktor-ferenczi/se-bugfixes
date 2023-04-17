using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Sandbox.Game.Entities;
using Shared.Config;
using Shared.Logging;
using Shared.Plugin;
using Shared.Tools;
using VRage.Collections;
using VRageMath;

namespace Shared.Patches.Voxel
{
    // ReSharper disable once UnusedType.Global
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [HarmonyPatch(typeof(MyPlanet))]
    public static class MyPlanetPatch
    {
        private static IPluginLogger Log => Common.Logger;
        private static IPluginConfig Config => Common.Config;
        private static bool enabled;
        
        // private static readonly FieldInfo  = AccessTools.FieldRef<>

        public static void Configure()
        {
            enabled = Config.Enabled && Config.VoxelOom;
        }

        private static readonly FieldInfo physicsShapesFieldInfo = AccessTools.DeclaredField(typeof(MyPlanet), "m_physicsShapes"); 

        [HarmonyTranspiler]
        [HarmonyPatch("CreateVoxelMap")]
        private static IEnumerable<CodeInstruction> CreateVoxelMapTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            if (!enabled)
                return instructions;

            var il = instructions.ToList();
            
            if (il.HashInstructionsHex() != "f8bb902a")
            {
                Log.Warning($"{nameof(MyPlanetPatch)}.{nameof(CreateVoxelMapTranspiler)}: Code change detected [{il.HashInstructionsHex()}], ignoring patch (this should be harmless)");
                return il;
            }
            
            il.RecordOriginalCode();
            
            // var planet = __instance;
            // var physicsShapes = (MyConcurrentDictionary<Vector3I, object>)physicsShapesFieldInfo.GetValue(planet);
            //
            // Log.Warning($"physicsShapes.Count={physicsShapes.Count}");

            il.RecordPatchedCode();
            return il;
        }
    }
}