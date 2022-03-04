# Space Engineers Bugfixes Plugin

**Scroll down for:**
- List of features
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

### Patreon

#### Admiral level supporters
- BetaMark
- Casinost
- wafoxxx

#### Captain level supporters
- Lotan
- ransomthetoaster
- Lazul
- mkaito
- DontFollowOrders
- Gabor

### Developers
- TBD

### Testers
- TBD

## Technical details

### Laser antenna connection issues

TBD

Please vote on the support tickets:
- [Laser antenna connection issues](https://support.keenswh.com/spaceengineers/pc/topic/laser-antenna-connection-issues)
- [Laser antennas do not work through safe zone or atmosphere](https://support.keenswh.com/spaceengineers/pc/topic/laser-antennas-dont-work-through-safe-zones-or-atmosphere_3)