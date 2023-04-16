// ArithmeticException: Function does not accept floating point Not-a-Number values
// https://support.keenswh.com/spaceengineers/pc/topic/27973-arithmeticexception-function-does-not-accept-floating-point-not-a-number-values
// https://github.com/viktor-ferenczi/LookAtNanRepro

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Shared.Config;
using Shared.Plugin;
using SpaceEngineers.Game.Entities.Blocks;
using VRageMath;

namespace Shared.Patches.TurretNan
{
    // ReSharper disable once UnusedType.Global
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [HarmonyPatch(typeof(MyTurretControlBlock))]
    public static class MyTurretControlBlockPatch
    {
        private static IPluginConfig Config => Common.Config;
        private static bool enabled;

        public static void Configure()
        {
            enabled = Config.Enabled && Config.TurretNan;
            Config.PropertyChanged += OnConfigChanged;
        }
        
        private static void OnConfigChanged(object sender, PropertyChangedEventArgs e)
        {
            enabled = Config.Enabled && Config.TurretNan;
        }

        [HarmonyPostfix]
        [HarmonyPatch("LookAt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void LookAtPostfix(ref Vector3 __result)
        {
            if (!enabled)
                return;

            if (__result.IsValid())
                return;

            var result = __result;
            if (!result.X.IsValid())
                result.X = 0f;
            if (!result.Y.IsValid())
                result.Y = 0f;
            if (!result.Z.IsValid())
                result.Z = 0f;

            __result = result;
        }
    }
}