using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
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
        private static int budget = 100;

        public static void Update(long tick)
        {
            if (tick % 60 == 0)
            {
                Interlocked.Increment(ref budget);
            }
        }

        private static void ThrottledWarning(string text)
        {
            if (budget <= 0)
                return;

            Interlocked.Decrement(ref budget);
            
            Log.Warning(text);
        }

        [HarmonyPrefix]
        [HarmonyPatch("UpdatePlanetPhysics")]
        private static bool UpdatePlanetPhysicsPrefix(MyPlanet __instance, ref BoundingBoxD box)
        {
            if (!enabled)
                return true;

            var size = box.Size;
            const double limit = 4_000_000;  // Allow up to 4000km planets
            if (size.X > limit || size.Y > limit || size.Y > limit)
            {
                ThrottledWarning($"UpdatePlanetPhysics: Too large box: size = {box}; WorldAABB = {__instance.PositionComp.WorldAABB}; planet: {__instance.DebugNameNoId()}");
                // Workaround: Shortcut the execution here, so the server does not crash at least
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("GeneratePhysicalShapeForBox")]
        private static bool GeneratePhysicalShapeForBoxPrefix(MyPlanet __instance, ref Vector3I increment, ref BoundingBoxD shapeBox)
        {
            if (!enabled)
                return true;

            var clustersIntersection = (List<BoundingBoxD>)ClustersIntersectionField.GetValue(__instance);
            if (clustersIntersection.Count >= 10000)
            {
                ThrottledWarning($"GeneratePhysicalShapeForBox: Too many items in m_clustersIntersection: {clustersIntersection.Count}; planet: {__instance.DebugNameNoId()}");
                return false;
            }

            var fixedIncrement = new Vector3I(1024, 1024, 1024);
            if (increment != fixedIncrement)
            {
                ThrottledWarning($"GeneratePhysicalShapeForBox: Invalid increment: {increment}; planet: {__instance.DebugNameNoId()}");
            }

            var size = shapeBox.Size;
            const double limit = 1_000_000;
            if (size.X > limit || size.Y > limit || size.Y > limit)
            {
                ThrottledWarning($"GeneratePhysicalShapeForBox: Too large shapeBox: size = {shapeBox.Max - shapeBox.Min}; shapeBox = {shapeBox}; planet: {__instance.DebugNameNoId()}");
                // Actual example from Prozon:
                // Too large shapeBox: {Min:X:-670875314.731042 Y:298284.217203573 Z:-570371416.13524 Max:X:1118183955.68551 Y:1660129266.60366 Z:1268623598.29293}
                // Workaround: Shortcut the execution here, so the server does not crash at least
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("CreateVoxelPhysics")]
        private static bool CreateVoxelPhysicsPrefix(MyPlanet __instance, ref Vector3I increment, ref Vector3I_RangeIterator it)
        {
            if (!enabled)
                return true;

            var fixedIncrement = new Vector3I(1024, 1024, 1024);
            if (increment != fixedIncrement)
            {
                ThrottledWarning($"CreateVoxelPhysics: Invalid increment: {increment}; planet: {__instance.DebugNameNoId()}");
            }

            var start = (Vector3I)StartField.GetValue(it);
            var end = (Vector3I)EndField.GetValue(it);
            var inner = end - start;
            var outer = inner + new Vector3I(1, 1, 1);
            if (inner.X < 0 || inner.Y < 0 || inner.Z < 0)
            {
                ThrottledWarning($"CreateVoxelPhysics: Negative iterator box size: {inner}; start = {start}; end = {end}; planet: {__instance.DebugNameNoId()}");
                return false;
            }

            if (inner.X >= 1000 || inner.Y >= 1000 || inner.Z >= 1000 || outer.X * outer.Y * outer.Z >= 50 * 50 * 50)
            {
                ThrottledWarning($"CreateVoxelPhysics: Too large iterator box: size = {inner}; start = {start}; end = {end}; planet: {__instance.DebugNameNoId()}");
                // Actual example from Prozon:
                // Too large iterator box size: [X:1747127, Y:1620929, Z:1795892]; start = [X:-655135, Y:14, Z:-551420]; end = [X:1091992, Y:1620943, Z:1244472]
                return false;
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
                Log.Warning($"{nameof(MyPlanetPatch)}.{nameof(CreateVoxelMapTranspiler)}: Code change detected [{il.HashInstructionsHex()}], ignoring patch (this should be harmless)");
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
                Log.Warning($"  {entity.GetType().Name} [{entity.EntityId}]: {entity.DebugNameNoId()} @ {entity.DebugPosition()}");
            }

            Log.Warning("End of entity list.");
        }
    }
}