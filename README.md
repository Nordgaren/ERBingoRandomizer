# ERBingoRandomizer
A Randomizer mod made for [Bingo Brawlers](https://bingobrawlers.com/) Elden Ring Bingo Season 2!
* This randomizer randomizes dropped and weapons on the map by category, including the ranged weapons, which are all counted as one category (bows, light bows, greatbows, crossbows and ballistas are all in the same pool).  
* It randomizes the starting classes by gives them all random stats. Each class will be level 9 and have a pool of 88 stats. Each character class can get any weapon, except rememberence weapons and staves/seals. The Prisoner will start with a staff and 1 or two spells, and the Confessor will start with a seal and one or two spells. Starting armor is also randomized.  
* Shops will have weapons fully randomized with any weapon in the game, aside from rememberance weapons. Rememberance shops are randomized with all other rememberance weapons, Spell shops are randomized with other spells shops, and the dragon incantation shops are randomized between themselves, as well.
* This mod also patches the AtkParamPC params to fix a bug with SpEffectAtkPowerCorrectRate that caused swarm of flies damage to be incorrectly calculated in some circumstances.   

# How To Use
The program should find your steam library and find the eldenring.exe path. If not, you can shift + right click your elden ring executable and select "Copy as path". Paste that into the path text box and remove the quotation marks on either side.

Put the seed in that you want to use for randomization, and then hit the Randomize button. When it is done randomizing, the `Bingo!` button will be activated, and clicking it will launch the game with the randomized regulation bin.

# Thank You
Thank you [Captain Domo](https://www.twitch.tv/captain_domo) for the commission to make this mod, and including me in this amazing event.  
Thank you to everyone who gave me feedback on the mod, including the players. Huge shoutout to the players for their patience in the early stages, and for the entertaining matches!  
Thank you [TheFifthMatt](https://github.com/thefifthmatt), an amazing modder and great resource for soulsborne randomization methodology.  
Thank you [Katalash](https://github.com/katalash), author of DS Map Studio, and other tools, and another great resource for soulsmodding.  
Thank you [TKGP](https://github.com/JKAnderson), 

# Libraries
[Andre](https://github.com/soulsmods/DSMapStudio/blob/master/src/Andre/Andre.Formats/Param.cs) Formerly FSParam and StudioUtils, a library for parsing FromSoft param formats. These libraries are under the MIT license.  
[SoulsFormats](https://github.com/soulsmods/DSMapStudio/tree/master/src/Andre/SoulsFormats) from the `souldmods/DSMapStudio` repo. This is a version of [SoulsFormats](https://github.com/JKAnderson/SoulsFormats) updated for .Net 6.

# License
The code in this repository, aside from the libraries mentioned above, is under MIT license.  
