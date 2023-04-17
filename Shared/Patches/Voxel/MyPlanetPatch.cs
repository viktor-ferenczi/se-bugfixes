using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Sandbox.Game.Entities;
using Shared.Config;
using Shared.Logging;
using Shared.Plugin;
using Shared.Tools;
using VRage.Game.Entity;
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

            var j = il.FindIndex(i => i.opcode == OpCodes.Stloc_1);
            il.Insert(++j, new CodeInstruction(OpCodes.Ldarg_0)); // planet (this)
            il.Insert(++j, new CodeInstruction(OpCodes.Ldloc_1)); // count
            il.Insert(++j, new CodeInstruction(OpCodes.Ldarg_3)); // storageMin
            il.Insert(++j, il.Find(i => i.opcode == OpCodes.Ldarg_S).Clone()); // storageMax
            il.Insert(++j, new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(MyPlanetPatch), nameof(Warn))));

            il.RecordPatchedCode();
            return il;
        }

        public static void Warn(MyPlanet planet, int count, Vector3I storageMin, Vector3I storageMax)
        {
            var warn = (
                count == 0 ||
                count < 100_000 && count % 10_000 == 0 ||
                count < 1_000_000 && count % 100_000 == 0 ||
                count % 1_000_000 == 0 ||
                count == 47990000 ||
                count == 47999000 ||
                count == 47999800 ||
                count == 47999850 ||
                count == 47995853);
            if (!warn)
                return;

            Log.Warning("Preliminary warning of a potential OOM issue in MyPlanet.CreateVoxelMap:");
            Log.Warning($"  planet: {planet.DebugNameNoId()}");
            Log.Warning($"  count = {count:#,##0} (crash when 47_995_853)");
            Log.Warning($"  storageMin = {storageMin}");
            Log.Warning($"  storageMax = {storageMax}");

            var position = planet.PositionLeftBottomCorner;
            var box = new BoundingBoxD(position + storageMin, position + storageMax);
            var entities = new List<MyEntity>(64);
            MyGamePruningStructure.GetTopMostEntitiesInBox(ref box, entities);
            Log.Warning($"Intersecting entities: {entities.Count}");

            if (entities.Count > 1000)
            {
                Log.Warning($"!!! Listing only the top 1000 entities !!!");
                entities.RemoveRange(1000, entities.Count - 1000);
            }

            entities.Sort((a, b) => a.EntityId.CompareTo(b.EntityId));
            foreach (var entity in entities)
            {
                Log.Warning($"  {entity.GetType().Name} [{entity.EntityId}]: {entity.DebugNameNoId()} @ {entity.DebugPosition()}");
            }

            Log.Warning("End of entity list.");
        }
    }
}