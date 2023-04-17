// f8bb902a
ldloca.s 0 (VRageMath.BoundingBox)
ldarg.3
call static VRageMath.Vector3 VRageMath.Vector3I::op_Implicit(VRageMath.Vector3I value)
ldarg.s 4
call static VRageMath.Vector3 VRageMath.Vector3I::op_Implicit(VRageMath.Vector3I value)
call System.Void VRageMath.BoundingBox::.ctor(VRageMath.Vector3 min, VRageMath.Vector3 max)
ldarg.0
call VRage.Game.Voxels.IMyStorage Sandbox.Game.Entities.MyPlanet::get_Storage()
ldloca.s 0 (VRageMath.BoundingBox)
ldc.i4.0
callvirt abstract virtual VRageMath.ContainmentType VRage.ModAPI.IMyStorage::Intersect(VRageMath.BoundingBox& box, System.Boolean lazy)
ldc.i4.2
bne.un.s L0
newobj System.Void Sandbox.Game.Entities.MyVoxelPhysics::.ctor()
starg.s 2
ldarg.2
ldarg.0
call VRage.Game.Voxels.IMyStorage Sandbox.Game.Entities.MyVoxelBase::get_m_storage()
ldarg.0
callvirt virtual VRageMath.Vector3D Sandbox.Game.Entities.MyVoxelBase::get_PositionLeftBottomCorner()
ldarg.3
ldc.r4 1
call static VRageMath.Vector3 VRageMath.Vector3I::op_Multiply(VRageMath.Vector3I a, System.Single num)
call static VRageMath.Vector3D VRageMath.Vector3D::op_Addition(VRageMath.Vector3D value1, VRageMath.Vector3 value2)
ldarg.3
ldarg.s 4
ldarg.0
callvirt System.Void Sandbox.Game.Entities.MyVoxelPhysics::Init(VRage.Game.Voxels.IMyStorage storage, VRageMath.Vector3D positionMinCorner, VRageMath.Vector3I storageMin, VRageMath.Vector3I storageMax, Sandbox.Game.Entities.MyPlanet parent)
ldarg.2
ldc.i4.0
callvirt System.Void VRage.Game.Entity.MyEntity::set_Save(System.Boolean value)
ldarg.2
ldc.i4.1
call static System.Void Sandbox.Game.Entities.MyEntities::Add(VRage.Game.Entity.MyEntity entity, System.Boolean insertIntoScene)
L0:
ldarg.0
ldfld VRage.Collections.MyConcurrentDictionary`2<VRageMath.Vector3I, Sandbox.Game.Entities.MyVoxelPhysics> Sandbox.Game.Entities.MyPlanet::m_physicsShapes
callvirt System.Int32 VRage.Collections.MyConcurrentDictionary`2<VRageMath.Vector3I, Sandbox.Game.Entities.MyVoxelPhysics>::get_Count()
stloc.1
[EX_BeginException]
ldarg.0
ldfld VRage.Collections.MyConcurrentDictionary`2<VRageMath.Vector3I, Sandbox.Game.Entities.MyVoxelPhysics> Sandbox.Game.Entities.MyPlanet::m_physicsShapes
ldarg.1
ldfld VRageMath.Vector3I VRageMath.Vector3I_RangeIterator::Current
ldarg.2
callvirt System.Void VRage.Collections.MyConcurrentDictionary`2<VRageMath.Vector3I, Sandbox.Game.Entities.MyVoxelPhysics>::set_Item(VRageMath.Vector3I key, Sandbox.Game.Entities.MyVoxelPhysics value)
leave.s L1
[EX_BeginCatch]
pop
call static VRage.Utils.MyLog VRage.Utils.MyLog::get_Default()
ldstr "CreateVoxelMap-OOM-${0}-${1}-${2}"
ldloc.1
box System.Int32
ldloc.0
box VRageMath.BoundingBox
ldarg.1
ldfld VRageMath.Vector3I VRageMath.Vector3I_RangeIterator::Current
box VRageMath.Vector3I
call static System.String System.String::Format(System.String format, System.Object arg0, System.Object arg1, System.Object arg2)
callvirt System.Void VRage.Utils.MyLog::WriteLine(System.String msg)
[EX_EndException]
rethrow
L1:
ldarg.2
ret
