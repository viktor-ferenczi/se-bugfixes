// d72a00d2
call static VRage.Game.Components.MyComponentFactory VRage.Game.Components.MyComponentFactory::get_Static()
ldarg.0
callvirt VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentBase VRage.Game.Components.MyComponentFactory::CreateObjectBuilder(VRage.Game.Components.MyComponentBase instance)
dup
ldarg.0
ldfld System.Boolean VRage.Game.Components.MyComponentBase::RemoveExistingComponentOnNewInsert
stfld System.Boolean VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentBase::RemoveExistingComponentOnNewInsert
ret
