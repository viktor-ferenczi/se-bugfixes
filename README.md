# Space Engineers Bugfixes Plugin

**Scroll down for:**
- List of bugs fixed
- Their technical details
- Keen bug tickets to vote on

## Available for
- [Space Engineers](https://store.steampowered.com/app/244850/Space_Engineers/) with [Plugin Loader](https://steamcommunity.com/sharedfiles/filedetails/?id=2407984968)
- [Torch Server](https://torchapi.net/)
- [Dedicated Server](https://www.spaceengineersgame.com/dedicated-servers/)

## Installation

### Client plugin
1. Install [Plugin Loader](https://steamcommunity.com/sharedfiles/filedetails/?id=2407984968) into Space Engineers (add launch option)
2. Enable the **Bugfixes** plugin from the **Plugins** dialog
3. Apply, restart the game

### Torch Server plugin

Select the **Bugfixes** plugin from the plugin list inside the Torch GUI.

Please note, that the plugin is using Harmony to patch the game code. Once Keen fixes the issues
these patches are expected to be removed anyway, so I did not bother using Torch's patching mechanism.

### Dedicated Server plugin
1. Download the latest ZIP from [Releases](https://github.com/viktor-ferenczi/se-bugfixes/releases).
2. Open the `Bin64` folder of your Dedicated Server installation.
3. Create a `Plugins` folder if it does not exists.
4. Extract the ZIP file into the `Bin64/Plugins` folder.
5. Right click on all the DLLs extracted and select **Unblock** from the file's **Properties** dialog.
6. Start the Dedicated Server.
7. Add the `Bugfixes.dll` from the `Bin64/Plugins` folder to the Plugins list.

## Credits

*In alphabetical order*

### Patreon

#### Admiral level supporters
- BetaMark
- Bishbash777
- Casinost
- Dorimanx
- wafoxxx

#### Captain level supporters
- DontFollowOrders
- Gabor
- Lazul
- Lotan
- mkaito
- ransomthetoaster

### Developers
- Dude
- zznty

### Testers
- Dorimanx
- Dude
- zznty

## Technical details

### Crash due to NaN value in MyTurretControlBlock

Config: `TurretNan`

Log:
```
ArithmeticException: Function does not accept floating point Not-a-Number values
```

- [Support ticket](https://support.keenswh.com/spaceengineers/pc/topic/27973-arithmeticexception-function-does-not-accept-floating-point-not-a-number-values)
- [RCA and Repro](https://github.com/viktor-ferenczi/LookAtNanRepro)

### Crash on docking/undocking grids with offensive AI blocks

Config: `AiCrash`

Log:
```
System.NullReferenceException: Object reference not set to an instance of an object.
   at SpaceEngineers.Game.EntityComponents.Blocks.MyOffensiveWithWeaponsCombatComponent.OnConnectionChangeCompleted(MyCubeGrid arg1, GridLinkTypeEnum arg2)
```

Reported, root caused and patched by `zznty`. Thanks!

- [Support ticket](https://support.keenswh.com/spaceengineers/pc/topic/28104-nullreferenceexception-in-myoffensivewithweaponscombatcomponent)

### Failing to save world due to NullRef error on modded blocks

Config: `Serialize`

Log:
```
Initializer: System.NullReferenceException: Object reference not set to an instance of an object. 
   at VRage.Game.Components.MyComponentBase.Serialize(Boolean copy) at VRage.Game.Components.MyComponentContainer.Serialize(Boolean copy) 
```

Reported, root caused and patched by `Dude`. Thanks!

- [Support ticket](https://support.keenswh.com/spaceengineers/pc/topic/27952-servers-crash-on-player-join-1-202)
- [Support ticket](https://support.keenswh.com/spaceengineers/pc/topic/28104-nullreferenceexception-in-myoffensivewithweaponscombatcomponent)

### Crash on OOM due to too many voxel 

Config: `VoxelOOM`

Log:
```
Initializer: System.OutOfMemoryException: Array dimensions exceeded supported range.
   at System.Collections.Generic.Dictionary`2.Resize(Int32 newSize, Boolean forceNewHashCodes)
   at System.Collections.Generic.Dictionary`2.Insert(TKey key, TValue value, Boolean add)
   at VRage.Collections.MyConcurrentDictionary`2.set_Item(TKey key, TValue value)
   at Sandbox.Game.Entities.MyPlanet.CreateVoxelMap(Vector3I_RangeIterator it, MyVoxelPhysics voxelMap, Vector3I storageMin, Vector3I storageMax)
```

Keen catches the exception, logs some extra information and re-trows it which crashes the game.

```
CreateVoxelMap-OOM-$47995853-${Min:{X:-5.637376E+08 Y:-1.313395E+09 Z:-1.435155E+09} Max:{X:-5.637366E+08 Y:-1.313394E+09 Z:-1.435154E+09}}-$[X:-550525, Y:-1282612, Z:-1401519]
```

- TBD: Support ticket