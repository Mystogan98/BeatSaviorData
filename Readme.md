# BeatSaviorData

## What it does

BeatSaviorData is a Beat Saber plugin that collects and shows a lot of data about your plays, such as per-hand accuracy, speed, distance traveled, overswing percentage, ...

It shows a score graph at the end of songs, alongside with a data panel, directly in game. It also uploads part of that data to beatsavior.io, where you can see your recent plays on your profile page, with exclusive data.

![BSD screenshot](https://github.com/Mystogan98/BeatSaviorData/blob/master/ReadmeImage.png?raw=true)


## How to install

Go to the "Releases" tab on the right, download the latest DLL file, and drop it into your "Plugins" folder inside your Beat Saber folder.

Requires BSIPA (4.1.3 or higher), BS_Utils (1.6.5 or higher) and BeatSaberMarkupLanguage (1.4.2 or higher).


## Can I use the data myself ?

Yes ! You can use the api on the website (https://www.beatsavior.io/api/livescores/player/), or locally in your AppData folder (AppData/Roaming/Beat Savior Data). I plan on making the DataCollector directly accessible in the game too, for other mods to use it in real time. In the local files, the first line is the player's stats, then every other line corresponds to one song. Every line is a correct json object, even though the whole file isn't.

Please credit me if you use them.




## Exhaustive list of all data and trackers

#### SongData (main json object)

- songDataType : The type of song this data is about (enum, 0 = None, 1 = Pass, 2 = Fail, 3 = Practice mode, 4 = Replay, 5 = Campaign)
- playerID : Steam or Oculus ID of the player. This is equal to their ScoreSaber ID. (string)
- songID : Hash of the map. (string)
- songDifficulty : The difficulty that was played. (string)
- songName : Title of the song. (string)
- songArtist : Artist of the song. (string)
- songMapper : Name of the mapper. (string)
- songSpeed : Speed of the song. (float, practice mode only)
- songStartTime : Time in seconds of the song's start. (float, practice mode only)
- songDuration : Duration of the song in seconds. (float)


#### HitTracker

- leftNoteHit : Number of note hit by the left saber. (int)
- rightNoteHit : Number of note hit by the right saber. (int)
- bombHit : Number of bomb hit. (int)
- miss : Number of misses. (int)
- maxCombo : Maximum combo achieved during the song. (int)
- nbOfWallHit : Number of time a wall has reduced your combo. (int)


#### AccuracyTracker

- accRight : Average right accuracy, including swing points. (float)
- accLeft : Average left accuracy, including swing points. (float)
- averageAcc : Average of both stats above, weighted by number of cut. (float)

Same goes with Speed (in m/s), HighestSpeed (in m/s), Preswing (float, 1 = perfect swing), Postswing (float, 1 = perfect swing), TimeDependence (float, 0 = perfectly time independent) and AverageCut (float array : {preswing, accuracy, postswing}).

- gridAcc : Average accuracy on every position of the 4*3 grid. Very likely to be full of NaN on Noodle/Mapping extensions maps. (float array)
- gridCut : Number of cut on every position of the 4*3 grid. Very likely to be full of NaN on Noodle/Mapping extensions maps. (float array)


#### ScoreTracker

- rawScore : Score without any modifiers. (int)
- score : Score with modifiers. (int)
- personalBest : Modified score of the personal best. (int)
- rawRatio : Raw score divided by raw max score. This is what is shown during the level, and is independant of the current modifiers. (float)
- modifiedRatio : Modified score divided by raw max score. This is what is used for things like ScoreSaber's PP curve. Can be higher than 1. (float)
- personalBestRawRatio : Modified PB score divided by raw max score. This is independant of the current modifiers, and can be higher than 1. (float)
- personalBestModifiedRatio : Modified PB score divided by modified max score. Can be higher than 1. (float)
- modifiersMultiplier : Total score multiplier with current modifiers. (float)
- modifiers : Acronyms of current modifiers. (string array)


#### WinTracker

- won : If the map was successfully passed or not. (bool)
- rank : Rank name (SS, S, A, ...). (string)
- endTime : The time in second when the run ended. (float)
- nbOfPause : The number of pause made during the song. (int)


#### DistanceTracker

- rightSaber : The distance traveled by the tip of the right saber. **This is bugged in the game and shows the right hand value instead**. (float)
- leftSaber : The distance traveled by the tip of the left saber. (float)
- rightHand : The distance traveled by the right hand. (float)
- leftHand : The distance traveled by the left hand. (float)


#### ScoreGraphTracker

- graph : Records the score ratio for every beat with at least one note. (Dictionnary<string, float>)


#### NoteTracker

**This tracker is not available in the API.** Due to the size of this tracker, it is only available directly in the game, or in the local files. It will record **every** note of the song, alongside a bunch of data associated with each note.
Each record contains the following : 

- noteType : The type of the note. (enum, 0 = right, 1 = left)
- noteDirection : The direction of the note. (enum, 0 = up, 1 = down, 2 = left, 3 = right, 4 = upLeft, 5 = upRight, 6 = downLeft, 7 = downRight, 8 = any, 9 = none)
- index : The position of the note in the 4*3 grid. (int, bottom left to top right)
- id : The ID of the note, starting at 0. (int)
- time : The time of the note in beats. (float)
- cutType : The type of the cut. (enum, 0 = cut, 1 = miss, 2 = badCut)
- multiplier : The multiplier when the note was hit. Know that the multiplier is incremented before the score is calculated, despite not being shown in the NoteData. (int)
- score : The score of the note. (int array : {preswing, accuracy, postswing})
- timeDeviation : The deviation between when the note was played and when it was cut, in beats. (float)
- speed : The speed in m/s at which the note was cut. (float)
- preswing : The value of the swing before the note was cut, 1 being a perfect swing. (float)
- postswing : The value of the swing after the note was cut, 1 being a perfect swing. (float)
- cutPoint : The 3D point where the saber and the note came in contact. (float array : {x, y, z})
- saberDir : The direction of the saber when the saber and the note came in contact. (float array : {x, y, z})
- timeDependence : How time dependent your swing was, 0 being a perfectly time independent swing. (float)