![Static Badge](https://img.shields.io/badge/ver-1.1.0-darkgreen)
![Static Badge](https://img.shields.io/badge/CSSharp-v116%2B-purple)
## `About`
Plugin gives player basic VIP privileges, but for a certain period of time.
The following are currently available:
* Health
* Armor + Helmet
* Money
* Gravity
* HealthShot
* Weapons in list
* ClanTag in scoreboard
### `Config`
addons/counterstrikesharp/configs/plugins/NightVip/NightVip.json
```json
"PluginStartTime": "20:00:00",    //  time until 24h | format: HH:MM:SS
"PluginEndTime": "06:00:00",      //  time after 24h | format: HH:MM:SS
"ScoreBoardTag": "[NightVip]",    //  tag in scoreboard
"UseScoreBoardTag": true,         //  use this tag?
"Health": 100,                    //  Amount hp
"Armor": 100,                     //  Amount armor(+helmet)
"Money": 16000,                   //  Amount money(when player spawned)
"Gravity": 0.8,                   //  Gravity... (default 1.0)
"GiveHealthShot": true,           //  give player healthshot?
"GivePlayerItem": true,           // give weapon in list:
"WeaponsList": [
    "weapon_ak47",
    "weapon_deagle",
    "weapon_hegrenade",
    "weapon_molotov",
    "weapon_smokegrenade",
    "weapon_flashbang"
  ]
"ConfigVersion": 1                //  non uses
```
Follow the data entry formats from the config example.
### `Commands`
```
!nightvip
!nvip
!nv
```
### `To do list`
- [x] Issuance of weapons (specify which one) 
- [ ] Bunnyhop
- [ ] Jump count
- [ ] Maybe something else
### `Note`
The plugin has not been tested properly due to the lack of a large number of people. If you find any bugs or errors please let me know. Thank you.
### `Support`
If you have a desire or want to support me, you can buy me tea)
 [CLICK](https://www.buymeacoffee.com/jockii)
### `Developers`
![Static Badge](https://img.shields.io/badge/Author-jockii-orange)
