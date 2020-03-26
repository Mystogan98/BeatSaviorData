using BS_Utils.Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BeatSaviorData.Trackers;
using Newtonsoft.Json;

namespace BeatSaviorData
{
	class SongData
	{
		public string playerID, songID, songDifficulty, songName, songArtist, songMapper;

		public Dictionary<string, ITracker> trackers = new Dictionary<string, ITracker>()
		{
			{ "hitTracker", new HitTracker() },
			{ "accuracyTracker", new AccuracyTracker() },
			{ "scoreTracker", new ScoreTracker() },
			{ "winTracker", new WinTracker() }
		};

		private readonly BeatmapObjectSpawnController BOSC; public BeatmapObjectSpawnController GetBOSC() => BOSC;
		private readonly GameplayCoreSceneSetupData GCSSD; public GameplayCoreSceneSetupData GetGCSSD() => GCSSD;
		private readonly ScoreController scoreController; public ScoreController GetScoreController() => scoreController;
		private readonly GameplayModifiersModelSO modifierData; public GameplayModifiersModelSO GetModifierData() => modifierData;
		private readonly PlayerDataModelSO playerData; public PlayerDataModelSO GetPlayerData() => playerData;

		private bool isNotAReplay, isNotInPracticeMode;

		public SongData()
		{
			BOSC = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().First();
			GCSSD = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData;
			modifierData = Resources.FindObjectsOfTypeAll<GameplayModifiersModelSO>().First();
			playerData = Resources.FindObjectsOfTypeAll<PlayerDataModelSO>().First();
			scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().First();

			playerID = GetUserInfo.GetUserID().ToString();
			songID = GCSSD.difficultyBeatmap.level.levelID.Replace("custom_level_","").Split('_')[0];
			songDifficulty = GCSSD.difficultyBeatmap.difficulty.ToString().ToLower();
			songName = GCSSD.difficultyBeatmap.level.songName;
			songArtist = GCSSD.difficultyBeatmap.level.songAuthorName;
			songMapper = GCSSD.difficultyBeatmap.level.levelAuthorName;

			isNotInPracticeMode = (GCSSD.practiceSettings == null);

			scoreController.scoreDidChangeEvent += IsNotAReplay;
		
			foreach (ITracker t in trackers.Values)
				t.RegisterTracker(this);
		}

		private void IsNotAReplay(int score, int modifiedScore)
		{
			isNotAReplay = true;
			scoreController.scoreDidChangeEvent -= IsNotAReplay;
		}

		public bool IsAReplay() => !isNotAReplay;
		public bool IsPraticeMode() => !isNotInPracticeMode;

		public string FinalizeData(LevelCompletionResults results)
		{
			foreach (ITracker t in trackers.Values)
				t.EndOfSong(results);
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
