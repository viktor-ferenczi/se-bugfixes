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

        private static readonly FieldInfo ClustersIntersectionField = AccessTools.DeclaredField(typeof(MyPlanet), "m_clustersIntersection");
        private static readonly FieldInfo StartField = AccessTools.DeclaredField(typeof(Vector3I_RangeIterator), "m_start");
        private static readonly FieldInfo EndField = AccessTools.DeclaredField(typeof(Vector3I_RangeIterator), "m_end");

        [HarmonyPrefix]
        [HarmonyPatch("GeneratePhysicalShapeForBox")]
        private static bool GeneratePhysicalShapeForBox(MyPlanet __instance, ref Vector3I increment, ref BoundingBoxD shapeBox)
        {
            var clustersIntersection = (List<BoundingBoxD>)ClustersIntersectionField.GetValue(__instance);
            if (clustersIntersection.Count >= 10000)
            {
                Log.Warning($"Too many items in m_clustersIntersection: {clustersIntersection.Count}; planet: {__instance.DebugNameNoId()}");
            }
            
            var fixedIncrement = new Vector3I(1024, 1024, 1024);
            if (increment != fixedIncrement)
            {
                Log.Warning($"GeneratePhysicalShapeForBox: Invalid increment: {increment}; planet: {__instance.DebugNameNoId()}");
            }

            var size = shapeBox.Size;
            const double limit = 10000;
            if (size.X > limit || size.Y > limit || size.Y > limit)
            {
                Log.Warning($"GeneratePhysicalShapeForBox: Too large shapeBox: {shapeBox}; planet: {__instance.DebugNameNoId()}");
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("CreateVoxelPhysics")]
        private static bool CreateVoxelPhysicsPrefix(MyPlanet __instance, ref Vector3I increment, ref Vector3I_RangeIterator it)
        {
            var fixedIncrement = new Vector3I(1024, 1024, 1024);
            if (increment != fixedIncrement)
            {
                Log.Warning($"CreateVoxelPhysicsPrefix: Invalid increment: {increment}; planet: {__instance.DebugNameNoId()}");
            }

            var start = (Vector3I)StartField.GetValue(it);
            var end = (Vector3I)EndField.GetValue(it);
            var inner = end - start;
            var outer = inner + new Vector3I(1, 1, 1);
            if (inner.X < 0 || inner.Y < 0 || inner.Z < 0)
            {
                Log.Warning($"CreateVoxelPhysicsPrefix: Negative iterator box size: {inner}; start = {start}; end = {end}; planet: {__instance.DebugNameNoId()}");
            }
            else if (inner.X >= 1000 || inner.Y >= 1000 || inner.Z >= 1000 || outer.X * outer.Y * outer.Z >= 50 * 50 * 50)
            {
                Log.Warning($"CreateVoxelPhysicsPrefix: Too large iterator box size: {inner}; start = {start}; end = {end}; planet: {__instance.DebugNameNoId()}");
            }

            return true;
        }

        [HarmonyTranspiler]
        [HarmonyPatch("CreateVoxelMap")]
        private static IEnumerable<CodeInstruction> CreateVoxelMapTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            if (!enabled)
                return instructions;

            var il = instructions.ToList();

            if (il.HashInstructionsHex() != "f8bb902a")
            {
                Log.Warning(
                    $"{nameof(MyPlanetPatch)}.{nameof(CreateVoxelMapTranspiler)}: Code change detected [{il.HashInstructionsHex()}], ignoring patch (this should be harmless)");
                return il;
            }

            il.RecordOriginalCode();

            var j = il.FindIndex(i => i.opcode == OpCodes.Stloc_1);
            il.Insert(++j, new CodeInstruction(OpCodes.Ldarg_0)); // planet (this)
            il.Insert(++j, new CodeInstruction(OpCodes.Ldloc_1)); // count
            il.Insert(++j, new CodeInstruction(OpCodes.Ldarg_3)); // storageMin
            il.Insert(++j, il.Find(i => i.opcode == OpCodes.Ldarg_S).Clone()); // storageMax
            il.Insert(++j,
                new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(MyPlanetPatch), nameof(Warn))));

            il.RecordPatchedCode();
            return il;
        }

        public static void Warn(MyPlanet planet, int count, Vector3I storageMin, Vector3I storageMax)
        {
            var warn = count > 0 && (
                count < 100_000 && count % 10_000 == 0 ||
                count < 1_000_000 && count % 100_000 == 0 ||
                count % 1_000_000 == 0 ||
                count == 47_990_000 ||
                count == 47_999_000 ||
                count == 47_999_800 ||
                count == 47_999_850 ||
                count == 47_995_853);

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

            var entityLimit = count >= 47_999_800 ? 1000 : 100;
            if (entities.Count > entityLimit)
            {
                Log.Warning($"!!! Listing only {entityLimit} entities !!!");
            }

            entities.Sort((a, b) => a.EntityId.CompareTo(b.EntityId));
            foreach (var entity in entities.Take(entityLimit))
            {
                Log.Warning(
                    $"  {entity.GetType().Name} [{entity.EntityId}]: {entity.DebugNameNoId()} @ {entity.DebugPosition()}");
            }

            Log.Warning("End of entity list.");
        }
    }
}