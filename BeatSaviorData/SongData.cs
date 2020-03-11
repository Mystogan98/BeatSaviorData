using BS_Utils.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BeatSaviorData.Trackers;
using Newtonsoft.Json;

namespace BeatSaviorData
{
	class SongData
	{
		public ulong playerID;
		public string songID, songDifficulty;

		public Dictionary<string, ITracker> trackers = new Dictionary<string, ITracker>()
		{
			{ "HitTracker", new HitTracker() },
			{ "AccuracyTracker", new AccuracyTracker() },
			{ "ScoreTracker", new ScoreTracker() }
		};

		private readonly BeatmapObjectSpawnController BOSC; public BeatmapObjectSpawnController GetBOSC() => BOSC;
		private readonly GameplayCoreSceneSetupData GCSSD; public GameplayCoreSceneSetupData GetGCSSD() => GCSSD;
		private readonly ScoreController scoreController; public ScoreController GetScoreController() => scoreController;
		private readonly GameplayModifiersModelSO modifierData; public GameplayModifiersModelSO GetModifierData() => modifierData;
		private readonly PlayerDataModelSO playerData; public PlayerDataModelSO GetPlayerData() => playerData;

		private bool isNotAReplay;

		public SongData()
		{
			BOSC = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().First();
			GCSSD = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData;
			modifierData = Resources.FindObjectsOfTypeAll<GameplayModifiersModelSO>().First();
			playerData = Resources.FindObjectsOfTypeAll<PlayerDataModelSO>().First();
			scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().First();

			playerID = GetUserInfo.GetUserID();
			songID = GCSSD.difficultyBeatmap.level.levelID;
			songDifficulty = GCSSD.difficultyBeatmap.difficulty.ToString().ToLower();

			scoreController.scoreDidChangeEvent += IsNotAReplay;
		
			foreach (ITracker t in trackers.Values)
				t.RegisterTracker(this);
		}

		private void IsNotAReplay(int score, int modifiedScore)
		{
			isNotAReplay = true;
		}

		public bool IsItAReplay() => !isNotAReplay;

		public string FinalizeData()
		{
			foreach (ITracker t in trackers.Values)
				t.EndOfSong();
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
