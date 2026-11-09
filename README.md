# CSE3902 - Terraria Boss Rush

## Created by

- Nathan Cheng
- Anna Mansur
- Adam Rehan
- Jacob Su

Github page: [CSE3902](https://github.com/TheMagneticDude/CSE3902.git)

## Summary

This program uses MonoGame framework and C# to create a 2D boss-rush game inspired by Terraria. End goal will be to defeat a series of bosses with different weapons, items, and movement abilities. The game includes multiple bosses, player movement and attacking, and a hotbar system with usable items and weapons.

As of Sprint 2, this program includes a basic terraria guide character with movement, attacking, and item use abilities. There are three boss types with movement, animations, and player tracking abilities. You can also cycle through the current blocks and items.

As of Sprint 3, this program includes three different boss levels that you can swap between with J and K. Hotbar UI has been added along with player, block, and boss collisions. The player and enemies can now take damage and die as well. Special blocks now exist and have functions like causing player damage or slowing the player down.

## Game Controls

| Action | Keyboard / Mouse |
| --- | --- |
| Jump | `W`, `↑`, `Space` |
| Move Left | `A`, `←` |
| Move Right | `D`, `→` |
| Attack | `Z`, `N`, or Left Mouse Button |
| Swap to Sword | `1`, Numpad `1` |
| Swap to Dagger | `2`, Numpad `2` |
| Swap to Bow | `3`, Numpad `3` |
| Swap to Health Potion | `4`, Numpad `4` |
| Swap to Recall Potion | `5`, Numpad `5` |
| Swap to Staff | `6`, Numpad `6` |
| Swap to Yo-yo | `7`, Numpad `7` |
| Swap to Whip | `8`, Numpad `8` |
| Swap to Fire Bow | `9`, Numpad `9` |
| Swap to Frost Staff | `0`, Numpad `0` |
| Use Potions | Right Mouse Button |
| Reset | `R` |
| Exit | `Q` or `Esc` |
| Show Hitboxes | `F3` + `G` |
| Next Level | `K` |
| Previous Level | `J` |

- Run project from root with: dotnet run --project terraria

## Known Bugs

- Health and Recall potions appear simultaneously when pressing 4 and 5 together
- Weapons can still be used while using Health or Recall potions
- Daggers appear oddly in hand

## Extra Tools/Processes

## Credits

- Sprite sheets from [The Spriters Resource](https://www.spriters-resource.com/pc_computer/terraria/)
