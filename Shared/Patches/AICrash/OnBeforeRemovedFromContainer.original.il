// c45f29b9
ldarg.0
ldfld VRage.Library.Collections.DictionaryWrapper`2<System.Int64, Sandbox.Game.Entities.Cube.MyTerminalBlock> SpaceEngineers.Game.EntityComponents.Blocks.MyOffensiveWithWeaponsCombatComponent::m_boundedBlocks
callvirt System.Void VRage.Library.Collections.DictionaryWrapper`2<System.Int64, Sandbox.Game.Entities.Cube.MyTerminalBlock>::Clear()
ldarg.0
call Sandbox.Game.GameSystems.MyAiBlockSystem SpaceEngineers.Game.EntityComponents.Blocks.MyOffensiveCombatAbstractComponent::get_AiBlockSystem()
ldarg.0
ldftn System.Void SpaceEngineers.Game.EntityComponents.Blocks.MyOffensiveWithWeaponsCombatComponent::OnGridCopy(Sandbox.Game.Entities.MyCubeGrid original, VRage.Game.MyObjectBuilder_CubeGrid obj)
newobj System.Void System.Action`2<Sandbox.Game.Entities.MyCubeGrid, VRage.Game.MyObjectBuilder_CubeGrid>::.ctor(System.Object object, System.IntPtr method)
callvirt System.Void Sandbox.Game.GameSystems.MyAiBlockSystem::remove_OnGridCopy(System.Action`2<Sandbox.Game.Entities.MyCubeGrid, VRage.Game.MyObjectBuilder_CubeGrid> value)
ldarg.0
call Sandbox.Game.Entities.MyCubeGrid SpaceEngineers.Game.EntityComponents.Blocks.MyOffensiveCombatAbstractComponent::get_Grid()
stloc.0
ldloc.0
brfalse.s L0
ldloc.0
callvirt System.Boolean VRage.Game.Entity.MyEntity::get_IsPreview()
brtrue.s L1
ldloc.0
callvirt Sandbox.Game.Entities.Cube.MyGridPhysics Sandbox.Game.Entities.MyCubeGrid::get_Physics()
brfalse.s L2
ldloc.0
ldarg.0
ldftn System.Void SpaceEngineers.Game.EntityComponents.Blocks.MyOffensiveWithWeaponsCombatComponent::OnConnectionChangeCompleted(Sandbox.Game.Entities.MyCubeGrid arg1, VRage.Game.ModAPI.GridLinkTypeEnum arg2)
newobj System.Void System.Action`2<Sandbox.Game.Entities.MyCubeGrid, VRage.Game.ModAPI.GridLinkTypeEnum>::.ctor(System.Object object, System.IntPtr method)
callvirt System.Void Sandbox.Game.Entities.MyCubeGrid::add_OnConnectionChangeCompleted(System.Action`2<Sandbox.Game.Entities.MyCubeGrid, VRage.Game.ModAPI.GridLinkTypeEnum> value)
L0:
L1:
L2:
ldarg.0
call virtual System.Void SpaceEngineers.Game.EntityComponents.Blocks.MyOffensiveCombatAbstractComponent::OnBeforeRemovedFromContainer()
ret
