using BS_Utils.Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BeatSaviorData.Trackers;
using Newtonsoft.Json;

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
		#region Public
		public SongDataType songDataType = SongDataType.none;
			public string playerID, songID, songDifficulty, songName, songArtist, songMapper;
			public float songSpeed = 1, songStartTime = 0, songDuration = 0;

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
			private readonly DataCollector dataCollector;
			private readonly BeatmapObjectSpawnController BOSC;
			private readonly GameplayCoreSceneSetupData GCSSD;
			private readonly ScoreController scoreController;
			private readonly GameplayModifiersModelSO modifierData;
			private readonly PlayerDataModel playerData;

			private string trackerResult, deepTrackerResult;
		#endregion

		public SongData()
		{
			if (!SettingsMenu.instance.EnableDeepTrackers)
				deepTrackers.Clear();

			BOSC = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().First();
			GCSSD = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData;
			modifierData = Resources.FindObjectsOfTypeAll<GameplayModifiersModelSO>().First();
			playerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First();
			scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().First();

			playerID = GetUserInfo.GetUserID().ToString();
			songID = GCSSD.difficultyBeatmap.level.levelID.Replace("custom_level_","").Split('_')[0];
			songDifficulty = GCSSD.difficultyBeatmap.difficulty.ToString().ToLower();
			songName = GCSSD.difficultyBeatmap.level.songName;
			songArtist = GCSSD.difficultyBeatmap.level.songAuthorName;
			songMapper = GCSSD.difficultyBeatmap.level.levelAuthorName;

			// well either way, getting the IDifficultyBeatmap of the current level, accessing the “level” property, and calling the GetImageCoverAsync or something like that is the way to go

			// Normal way: use BS utils and access the static GameplayCoreSceneSetupData
			// Auros way: Zenject

			try
			{
				songDuration = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().First().songLength;
			} catch {
				Logger.log.Error("BSD : Could not get song length !");
				songDuration = -1;
			}

			if (GCSSD.practiceSettings != null) {
				songDataType = SongDataType.practice;
				songSpeed = GCSSD.practiceSettings.songSpeedMul;
				songStartTime = GCSSD.practiceSettings.startSongTime;
			} else {
				songDataType = SongDataType.replay;         // We set it as a replay by default, then set it as a pass if IsNotAReplay() is called
				scoreController.scoreDidChangeEvent += IsNotAReplay;
			}

			dataCollector = new DataCollector();
			dataCollector.RegisterCollector(this);
		}

		private void IsNotAReplay(int score, int modifiedScore)
		{
			if (songDataType == SongDataType.replay)
				songDataType = SongDataType.none;
			scoreController.scoreDidChangeEvent -= IsNotAReplay;
		}

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
		public BeatmapObjectSpawnController GetBOSC() => BOSC;
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
