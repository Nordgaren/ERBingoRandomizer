# Project Plans
* Update decompression to handle new file types.
* Ensure new DLC gear (weapons, spells, armor, etc.) is handled properly.
* Explore longterm ideas for what it means to be a gear randomizer.

# Equipment Randomizer
* This application has a simple UI to ensure that all players use the same randomized seed without worry about sharing the same settings.
* Starting classes are randomized: weapons, armor, stats, and spells. Class levels are fixed to 9.
* Each starting class has to have an initial piece of armor to recieve a randomized piece.
* The Prisoner starts with a staff and one or two spells, and the Confessor starts with a seal and one or two spells.
* Powers of Remembrance: randomized within the remembrance shop.
* Gear purchases: weapons are randomized with other non-remembrance weapons, sorceries at sorcery locations, incantations are split between dragon communion and all other incantation locations.
* Weapons in general are categorized by type. For example Greatswords are their own pool as are Halberds.
* Exceptions are staves and seals are not randomized, and ranged weapons are all counted as one category (bows, light bows, greatbows, crossbows and ballistas are all in the same pool). 
* As a reminder, shields are treated as weapons by the game. 
* This mod also patches the AtkParamPC params to fix a bug with SpEffectAtkPowerCorrectRate that caused swarm of flies damage to be incorrectly calculated in some circumstances.

# How To Use
* The program should find your steam library and find the eldenring.exe path. If the progam does not: shift + right click your elden ring executable, select "Copy as path", Paste into the path text box, and remove the quotation marks on either side.

Put the seed in that you want to use for randomization, and then hit the Randomize button. When it is done randomizing, the `Bingo!` button will be activated, and clicking it will launch the game with the randomized regulation bin. If a seed is not input one will be randomly created.

# Libraries
* [Andre](https://github.com/soulsmods/DSMapStudio/blob/master/src/Andre/Andre.Formats/Param.cs) Formerly FSParam and StudioUtils, a library for parsing FromSoft param formats. These libraries are under the MIT license.  
* [SoulsFormats](https://github.com/soulsmods/DSMapStudio/tree/master/src/Andre/SoulsFormats) from the `souldmods/DSMapStudio` repo. This is a version of [SoulsFormats](https://github.com/JKAnderson/SoulsFormats) updated for .Net 6.

# License
The code in this repository, aside from the libraries mentioned above, is under MIT license.  
