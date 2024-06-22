# ERBingoRandomizer
A Randomizer mod was made by [Nordgaren](https://github.com/Nordgaren/ERBingoRandomizer/tree/main) for [Bingo Brawlers](https://bingobrawlers.com/) Elden Ring Bingo Season 2!
* This application randomizes: starting class equipment and stats, weapons at merchant inventories, dropped weapons, weapons at map locations, sorceries at sorcery locations, and incantations at incantation locations.
* Weapons in general are categorized by type. For example Greatswords their own pool as do Halberds.
* Exceptions include Remembrance weapons being their own pool, Dragon Communion incantations are their own pool, staves and seals are not randomized, and ranged weapons are all counted as one category (bows, light bows, greatbows, crossbows and ballistas are all in the same pool).
* Starting classes all all are set to level 9 and recieve random stats from a pool of 88. Each class gets randomized armor, except the Wretch. Each character class can get any weapon, except rememberence weapons and staves/seals.
* The Prisoner starts with a staff and one or two spells, and the Confessor starts with a seal and one or two spells.  
* Shops will have weapons fully randomized with any weapon in the game, aside from rememberance weapons. Shields from merchants are treated as general weapon slots. 
* This mod also patches the AtkParamPC params to fix a bug with SpEffectAtkPowerCorrectRate that caused swarm of flies damage to be incorrectly calculated in some circumstances.   

# How To Use
* The program should find your steam library and find the eldenring.exe path. If the progam does not: shift + right click your elden ring executable, select "Copy as path", Paste into the path text box, and remove the quotation marks on either side.

Put the seed in that you want to use for randomization, and then hit the Randomize button. When it is done randomizing, the `Bingo!` button will be activated, and clicking it will launch the game with the randomized regulation bin. If a seed is not input one will be randomly created.

# Libraries
* [Andre](https://github.com/soulsmods/DSMapStudio/blob/master/src/Andre/Andre.Formats/Param.cs) Formerly FSParam and StudioUtils, a library for parsing FromSoft param formats. These libraries are under the MIT license.  
* [SoulsFormats](https://github.com/soulsmods/DSMapStudio/tree/master/src/Andre/SoulsFormats) from the `souldmods/DSMapStudio` repo. This is a version of [SoulsFormats](https://github.com/JKAnderson/SoulsFormats) updated for .Net 6.

# License
The code in this repository, aside from the libraries mentioned above, is under MIT license.  
