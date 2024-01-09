# LethalCompanyInstantLoot
Steal all loot from a map. Also can help pull loot out from the ship at the company. If you want only quality of life mod, switch your usage to FireSale [thunderstore](https://thunderstore.io/c/lethal-company/p/bozzobrain/FireSale/)/[github](https://github.com/bozzobrain/LethalCompanyInstantLoot) mod by bozzobrain.

This mod was intended to be a way to hack all of the loot to the ship, and it will continue to be that. Additional features for loot management at the company are going to be more frequently published to my other app. 

I will try to update this app to keep it current, but the other app will be my main focus.

# Features
- You can teleport loot around with ease.
- Activate the teleporting with the 'l'

# Usage
- If you are on the ship on a map with loot, any loot is teleported into the ship and automatically collected.
- If you are at the company, stand a bit away from the counter and you can teleport the loot to the counter for easy collection.
- Recall items back to the ship automatically by pressing 'l' when on the ship after selling some of your loot.
- Keybinding can be modified in the cfg file

## Config File Parameters
- FireSaleKey
	- Keypress used to activate the mod

- ItemGrouping
	- Whether to group the items in tight clusters or to spread them out by value
	- Options
		- [Value]
			- Spread items up the ship by the value
		- [Stack]
			- Keep items stacked on top of one another to reduce clutter

- TwoHandedItemLocation
	- Where to place the two handed items, and inherrently where to place the single handed objects.
	- Options
		- [Front]
			- Two handed items to the front of the ship 
			- One handed items to the back of the ship
		- [Back]
			- Two handed items to the back of the ship 
			- One handed items to the front of the ship

- OrganizationTechniques
	- Options
		- [Loose]
			- Spread items accross the ship from left to right
		- [Tight]
			- Pack the items to the side of the ship with the suit rack.

- ClosetLocationOverride
	- A List of objects to force into the closet on ship cleanup
		- Enter a list of item names in comma separated form to force these items to be placed in the closet instead of on the floor.

- SortingDisabledList
	- Items on this list will be ignored during the sorting process
		- Enter a list of item names in comma separated form to ignore these items during organization.

# Changes
- V2.1.1
	- Patched ship organizing features developed under [ShipMaid](https://github.com/bozzobrain/LethalCompanyShipMaid).
	- Virtually no overhaul to core functionality
	- Look at the configuration settings if the ship sorting feels different.
- V2.1.0
	- More updates for better networking
- V2.0.0
	- Made the system networking friendly

# Installation
1. Install BepInEx
2. Run game once with BepInEx installed to generate folders/files
3. Drop the DLL inside of the BepInEx/plugins folder
4. No further steps needed

# Feedback
- Feel free to leave feedback or requests at [my github](https://github.com/bozzobrain/LethalCompanyInstantLoot).

# Buy me a Beer
[Buy me a Beer](https://www.buymeacoffee.com/bozzobrain)
