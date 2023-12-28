![Static Badge](https://img.shields.io/badge/ver-1.5.0-darkgreen)
![Static Badge](https://img.shields.io/badge/CSSharp-v130%2B-purple)
## `About`
Plugin gives player basic VIP privileges, but for a certain period of time.
The following are currently available:
* Health
* Armor + Helmet
* Money
* Gravity
* Bhop
* Give weapons
* HealthShot
* ClanTag in scoreboard
### `Config`
addons/counterstrikesharp/configs/plugins/NightVip/NightVip.json
```json
  "PluginStartTime": "20:00:00",        // time until 24h | format HH:mm:ss
  "PluginEndTime": "06:00:00",          // time after 24h | format HH:mm:ss
  "AutoGiveVip": true,                  // Auto give vip to player
  "EnableAutoBhop": true                // Enable auto Bhop?
  "GiveHealthShot": true,               // Give healthshot?
  "GivePlayerWeapons": true,            // Give weapons to player in list?
  "UseScoreBoardTag": true,             // Use tag in "TAB" ?
  "ScoreBoardTag": "[NightVip]",        // Tag name
  "DisableVipRounds": "1,13",           // Disable vip in "X" rounds (1,2,3 ...)
  "Health": 100,                        // Amount health
  "Armor": 100,                         // Amount armor
  "Money": 16000,                       // Amount money (on spawn)
  "Gravity": 1,                         // Player gravity (default: 1.0 | custome: 0.7, 0.3 ...)
  "WeaponsList": [                      // Weapons list (weapon_nameweapon) 
    "weapon_ak47",                      // ak47
    "weapon_deagle"                     // desert eagle
  ],
  "ConfigVersion": 1                    // don`t touch it
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
- [x] Bunnyhop
- [x] Auto give vip
- [x] Disable vip in "X" round
- [ ] Jump count
- [ ] Fix weapon(when drop on ground. Its ready +-50%)
### `Note`
The plugin has not been tested properly due to the lack of a large number of people. If you find any bugs or errors please let me know. Thank you.
### `Support`
If you have a desire or want to support me, you can buy me tea)
 [CLICK](https://www.buymeacoffee.com/jockii)
### `Developers`
![Static Badge](https://img.shields.io/badge/Author-jockii-orange)
