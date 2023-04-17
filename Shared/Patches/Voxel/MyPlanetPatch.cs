using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Sandbox.Game.Entities;
using Shared.Config;
using Shared.Plugin;
using Shared.Tools;

namespace Shared.Patches.Voxel
{
    // ReSharper disable once UnusedType.Global
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [HarmonyPatch]
    public static class MyPlanetPatch
    {
        // CreateVoxelMap
        
        private static IPluginConfig Config => Common.Config;
        private static bool enabled;
        
        // private static readonly FieldInfo  = AccessTools.FieldRef<>

        public static void Configure()
        {
            enabled = Config.Enabled && Config.VoxelOom;
        }


        // public static IEnumerable<MethodBase> TargetMethods()
        // {
        //     foreach (var methodInfo in AccessTools.GetDeclaredMethods(typeof(MyPlanet)))
        //     {
        //         yield return methodInfo;
        //     }
        // }
        
        // private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        // {
        //     if (!enabled)
        //         return instructions;
        //
        //     foreach (var i in instructions)
        //     {
        //         var opcode = i.opcode;
        //         if (opcode == OpCodes.Ldfld)
        //         {
        //             
        //         } else if (opcode == OpCodes.Stfld)
        //         {
        //             
        //         }
        //         
        //         switch (i.opcode)
        //         {
        //             case OpCodes.Ldfld:
        //                 
        //         }
        //     }
        //
        //     var il = instructions.ToList();
        //     il.RecordOriginalCode();
        //
        //     var eventInfo = typeof(MyCubeGrid).GetEvent(nameof(MyCubeGrid.OnConnectionChangeCompleted), AccessTools.allDeclared);
        //     Debug.Assert(eventInfo != null);
        //     il = il.MethodReplacer(eventInfo.AddMethod, eventInfo.RemoveMethod).ToList();
        //
        //     il.RecordPatchedCode();
        //     return il;
        // }
        
    }
}