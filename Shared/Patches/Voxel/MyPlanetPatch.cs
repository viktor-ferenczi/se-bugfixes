// Work in progress
#if DEBUG
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Sandbox.Game.Entities;

namespace Shared.Patches.Voxel
{
    // ReSharper disable once UnusedType.Global
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [HarmonyPatch(typeof(MyPlanet))]
    public class MyPlanetPatch
    {
        // CreateVoxelMap
    }
}
#endif