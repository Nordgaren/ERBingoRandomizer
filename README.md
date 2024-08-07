# How To Use
The program should find your steam library and find the eldenring.exe path. If the progam does not: shift + right click your elden ring executable, select "Copy as path", Paste into the path text box, and remove the quotation marks on either side.

Put the seed in that you want to use for randomization, and then hit the Randomize button. When it is done randomizing, the `Bingo!` button will be activated, and clicking it will launch the game with the randomized regulation bin. If a seed is not input one will be randomly created.

# Equipment Randomizer
This application has a simple UI to ensure that all players use the same randomized seed without worry about having the same settings.
* Weapons in general are categorized by type. For example Greatswords are their own pool as are Halberds.
* Exceptions are staves and seals, they are not randomized. This includes the Confessor and Prisoner starting classes, merchant shops, etc.
* Ranged weapons are in grouped categories (bows & light bows, greatbows & ballistas are all in the same pool). 
* General Sorceries are randomized with sorceries and Incantations are randomized with incantations (Dragon Communion is seperated).
* As a reminder, shields are treated as weapons by the game. 

# Starting Classes
Starting classes are randomized: weapons, armor, stats, and spells. Class levels are fixed to 9, with stats totalling 88.
The Prisoner starts with its unrandomized staff and one sorcery. 
The Confessor starts with its unrandomized seal and one incantation.

# Powers of Remembrance
Powers of Remembrance are randomized within the remembrance shop. Staves are removed from the pool of eligible weapons to allow for more bingo square engagement. This means that Rennala's Remembrance gifts a non-stave weapon.

# Dragon Communion
Dragon communion incantations are only randomized within dragon communion locations.

# Weapon Pickups and Boss Drops
* Thrusting Shields are pooled with Heavy Thrusting Swords
* Great Katanas are pooled with Katanas
* Light Greatswords are pooled with Greatswords
* Backhand Blades are pooled with Curved Greatswords

# Note on Smithing Stones and Swarm of Flies
This mod also patches the AtkParamPC to fix a bug with SpEffectAtkPowerCorrectRate that caused swarm of flies damage to be incorrectly calculated in some circumstances. Smithing stone cost is also patched to be 3x stones per level creating more parity between Smithing and Somber weapons.

# Libraries
* [Andre](https://github.com/soulsmods/DSMapStudio/blob/master/src/Andre/Andre.Formats/Param.cs) Formerly FSParam and StudioUtils, a library for parsing FromSoft param formats. These libraries are under the MIT license.  
* [SoulsFormats](https://github.com/soulsmods/DSMapStudio/tree/master/src/Andre/SoulsFormats) from the `souldmods/DSMapStudio` repo. This is a version of [SoulsFormats](https://github.com/JKAnderson/SoulsFormats) updated for .Net 6.

# License
The code in this repository, aside from the libraries mentioned above, is under MIT license.  
