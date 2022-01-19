using BS_Utils.Gameplay;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BeatSaviorData.Trackers;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Reflection;
using HarmonyLib;
using BS_Utils.Utilities;

namespace BeatSaviorData
{
	public enum SongDataType
	{
		none,
		pass,
		fail,
		practice,
		replay,
		campaign
	}

	public class SongData
	{
		private static MethodBase ScoreSaber_playbackEnabled = AccessTools.Method("ScoreSaber.Core.ReplaySystem.HarmonyPatches.PatchHandleHMDUnmounted:Prefix");	

		#region Public
			public SongDataType songDataType = SongDataType.none;
			public string playerID, songID, songDifficulty, songName, songArtist, songMapper, gameMode;
			public int songDifficultyRank;
			public float songSpeed = 1, songStartTime = 0, songDuration = 0, songJumpDistance = 0;

			#region Dictionarries
				public Dictionary<string, ITracker> trackers = new Dictionary<string, ITracker>()
				{
					{ "hitTracker", new HitTracker() },
					{ "accuracyTracker", new AccuracyTracker() },
					{ "scoreTracker", new ScoreTracker() },
					{ "winTracker", new WinTracker() },
					{ "distanceTracker", new DistanceTracker() },
					{ "scoreGraphTracker", new ScoreGraphTracker() }
				};

				public Dictionary<string, ITracker> deepTrackers = new Dictionary<string, ITracker>()
				{
					{ "noteTracker", new NoteTracker() }
				};
			#endregion
		#endregion

		#region Private
			// Yes, I know, I have to Zenjectify all the things
			private DataCollector dataCollector;
			private readonly BeatmapObjectSpawnController BOSC;
			private BeatmapObjectManager BOM;
			private readonly GameplayCoreSceneSetupData GCSSD;
			private readonly ScoreController scoreController;
			private readonly GameplayModifiersModelSO modifierData;
			private readonly PlayerDataModel playerData;

			private string trackerResult, deepTrackerResult;
		#endregion

		public SongData()
		{
			if (!BS_Utils.Plugin.LevelData.IsSet)
			{
				Logger.log.Error("BS_Utils level data is not present. Did you start the tutorial ?");
				return;
			}
				
			try
			{
				BOSC = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().Last();						// Does this get used for anything? || Seems not ¯\_(ツ)_/¯
				GCSSD = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData;
				modifierData = Resources.FindObjectsOfTypeAll<GameplayModifiersModelSO>().First();
				playerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First();

				BOSC.didInitEvent += BOSCDidInit;

				// Ideally, this would get used with (x => x.isActiveAndEnabled). However, when SongData is getting created, no ScoreController is active and enabled at that point in time.
				// Getting the last one might be good enough, though.
				scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().Last();
				//BOM = scoreController.GetPrivateField<BeatmapObjectManager>("_beatmapObjectManager");
				GetBOMFromScoreController();
			} catch (Exception)
			{
				Logger.log.Error("SongData couldn't be created. Did you start the tutorial ?");
				return;
			}

			if (UserIDFix.UserIDIsReady)
				playerID = UserIDFix.UserID;
			else
				UserIDFix.UserIDReady += SetUserID;

			songID = GCSSD.difficultyBeatmap.level.levelID.Replace("custom_level_","").Split('_')[0];
			songDifficulty = GCSSD.difficultyBeatmap.difficulty.ToString().ToLower();
			gameMode = GCSSD.difficultyBeatmap.level.beatmapLevelData.difficultyBeatmapSets[0].beatmapCharacteristic.serializedName;

			songDifficultyRank = GCSSD.difficultyBeatmap.difficultyRank;
			songName = GCSSD.difficultyBeatmap.level.songName;
			songArtist = GCSSD.difficultyBeatmap.level.songAuthorName;
			songMapper = GCSSD.difficultyBeatmap.level.levelAuthorName;

			WaitForAudioTimeSyncController();

			if (GCSSD.practiceSettings != null) {
				songDataType = SongDataType.practice;
				songSpeed = GCSSD.practiceSettings.songSpeedMul;
				songStartTime = GCSSD.practiceSettings.startSongTime;
			} else {
				if (ScoreSaber_playbackEnabled != null && (bool)ScoreSaber_playbackEnabled.Invoke(null, null) == false)
					songDataType = SongDataType.replay;
				else
					songDataType = SongDataType.none;
				
				// We set it as a replay by default, then set it as a pass if IsNotAReplay() is called
				//scoreController.scoreDidChangeEvent += IsNotAReplay;
			}
		}

		private async void GetBOMFromScoreController()
		{
			while(BOM == null)
			{
				await Task.Delay(50);
				BOM = scoreController.GetPrivateField<BeatmapObjectManager>("_beatmapObjectManager");
			}

			// Create the DataCollector only now because it needs the BOM
			dataCollector = new DataCollector();
			dataCollector.RegisterCollector(this);
		}

		private async void WaitForAudioTimeSyncController()
		{
			await Task.Delay(500);

			try {
				songDuration = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().LastOrDefault(x => x.isActiveAndEnabled).songLength;
			} catch {
				Logger.log.Error("BSD : Could not get song length !");
				songDuration = -1;
			}
		}

		private void SetUserID()
		{
			playerID = UserIDFix.UserID;
			UserIDFix.UserIDReady -= SetUserID;
		}

		private void BOSCDidInit()
		{
			songJumpDistance = BOSC.jumpDistance;
		}

		/*private void IsNotAReplay(int score, int modifiedScore)
		{
			if (songDataType == SongDataType.replay)
				songDataType = SongDataType.none;
			scoreController.scoreDidChangeEvent -= IsNotAReplay;
		}*/

		private Dictionary<string, ITracker> iDontLikeThat;
		public void FinalizeData(LevelCompletionResults results)
		{
			if (songDataType == SongDataType.none && results.levelEndStateType == LevelCompletionResults.LevelEndStateType.Cleared)
				songDataType = SongDataType.pass;
			else if (songDataType == SongDataType.none && results.levelEndStateType == LevelCompletionResults.LevelEndStateType.Failed)
				songDataType = SongDataType.fail;

			dataCollector.UnregisterCollector(this);

			foreach (ITracker t in trackers.Values)
				t.EndOfSong(results, this);
			foreach (ITracker t in deepTrackers.Values)
				t.EndOfSong(results, this);

			deepTrackerResult = JsonConvert.SerializeObject(this, Formatting.None);

			// We store DeepTrackers in a private variable so it does not get serialized. Maybe find a better way to do so ?
			iDontLikeThat = deepTrackers;
			deepTrackers = null;
			trackerResult = JsonConvert.SerializeObject(this, Formatting.None);
			deepTrackers = iDontLikeThat;
			iDontLikeThat = null;
		}

		#region Getters
		//public BeatmapObjectSpawnController GetBOSC() => BOSC;
		public BeatmapObjectManager GetBOM() => BOM;
		public GameplayCoreSceneSetupData GetGCSSD() => GCSSD;
		public ScoreController GetScoreController() => scoreController;
		public GameplayModifiersModelSO GetModifierData() => modifierData;
		public PlayerDataModel GetPlayerData() => playerData;
		public DataCollector GetDataCollector() => dataCollector;

		public bool IsAReplay() => songDataType == SongDataType.replay;
		public bool IsPraticeMode() => songDataType == SongDataType.practice;
		public string GetDeepTrackersResults() => deepTrackerResult;
		public string GetTrackersResults() => trackerResult;
		#endregion
		}
}
