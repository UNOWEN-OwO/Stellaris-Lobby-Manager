# Setllaris Lobby Manager

A desktop app that manage and configures stellaris lobby settings.

![Screenshot 2023-08-13 000146](https://github.com/UNOWEN-OwO/Stellaris-Lobby-Manager/assets/41463621/74469904-e0cb-4b27-be16-53db1969356b)

## Features
- Quick save/load lobby settings, you can export setting to share with others
- <font size="4">***SUPPORT MULTIPLAER LOBBY FOR HOST***</font>
- Can exceed setting step, upper and lower bound limit, allow you to set the value as precise as game allows
- Lock or ignore setting configure
- Auto detect game launch/exit (but probably not a good idea leaving this app open while playing)
- Supports mods that adds more galaxy size & shapes

![Screenshot 2023-08-15 161854](https://github.com/UNOWEN-OwO/Stellaris-Lobby-Manager/assets/41463621/07168861-1e47-4ed4-9a35-12cbf43228db)

## Usage
- The left column is current setting in Setllaris, the right column is the configure that loaded, use arrow to apply to game or record to config for single setting item, or use the main button to do for all
- The middle button is the lock/ignore/do nothing button, click to cycle. Lock will always apply the config setting, ignore will apply the config setting, you can record the setting normally however
- At Full Precision will allow you modify game setting in smallest unit, you can toggle it to have default view as what Setllaris sets
- Overflow disables Setllaris' auto correction for overflow setting *when game is created*, it does not provide any limit check on the lobby manager app

![Screenshot 2023-08-13 001739](https://github.com/UNOWEN-OwO/Stellaris-Lobby-Manager/assets/41463621/3db6dcf0-054a-46f3-be7f-059cc575f7fe)
<p align="center"><font size="2">wormholes everywhere</font></p>

## Why?
- All starts from that game where I want 5 fallen empire at small sized map but too lazy to find a mod

## Requirements
- .NET 6.0
- win-x64

## Note
- Non host player will not able to see setting changes made from this app, please notify them :)
- Any setting that exceed the game limit might cause unexpected behavior, too low/high value might casuing game stuck at loading, crashing or configuration not apply at all, use at your own risk
- There is a high chance that this tool will break after main game version update, fix might take some time
