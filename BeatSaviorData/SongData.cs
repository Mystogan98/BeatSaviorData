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
		replay
	}

	public class SongData
	{
		public SongDataType songDataType = SongDataType.none;
		public string playerID, songID, songDifficulty, songName, songArtist, songMapper;
		public float songSpeed = 1, songStartTime = 0, songDuration = 0;

		public Dictionary<string, ITracker> trackers = new Dictionary<string, ITracker>()
		{
			{ "hitTracker", new HitTracker() },
			{ "accuracyTracker", new AccuracyTracker() },
			{ "scoreTracker", new ScoreTracker() },
			{ "winTracker", new WinTracker() },
			{ "distanceTracker", new DistanceTracker() }
		};

		public Dictionary<string, ITracker> deepTrackers = new Dictionary<string, ITracker>()
		{
			{ "noteTracker", new NoteTracker() }
		};

		private Dictionary<string, ITracker> iDontLikeThat;
		private readonly BeatmapObjectSpawnController BOSC; public BeatmapObjectSpawnController GetBOSC() => BOSC;
		private readonly GameplayCoreSceneSetupData GCSSD; public GameplayCoreSceneSetupData GetGCSSD() => GCSSD;
		private readonly ScoreController scoreController; public ScoreController GetScoreController() => scoreController;
		private readonly GameplayModifiersModelSO modifierData; public GameplayModifiersModelSO GetModifierData() => modifierData;
		private readonly PlayerDataModel playerData; public PlayerDataModel GetPlayerData() => playerData;

		private string trackerResult, deepTrackerResult;

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
			songDuration = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().First().songLength;

			if (GCSSD.practiceSettings != null)
			{
				songDataType = SongDataType.practice;
				songSpeed = GCSSD.practiceSettings.songSpeedMul;
				songStartTime = GCSSD.practiceSettings.startSongTime;
			} else {
				songDataType = SongDataType.replay;			// We set it as a replay by default, then set it as a pass if IsNotAReplay() is called
			}

			scoreController.scoreDidChangeEvent += IsNotAReplay;
		
			foreach (ITracker t in trackers.Values)
				t.RegisterTracker(this);
			foreach (ITracker t in deepTrackers.Values)
				t.RegisterTracker(this);
		}

		private void IsNotAReplay(int score, int modifiedScore)
		{
			if (songDataType == SongDataType.replay)
				songDataType = SongDataType.none;
			scoreController.scoreDidChangeEvent -= IsNotAReplay;
		}

		public bool IsAReplay() => songDataType == SongDataType.replay;
		public bool IsPraticeMode() => songDataType == SongDataType.practice;

		public void FinalizeData(LevelCompletionResults results)
		{
			if (songDataType == SongDataType.none && results.levelEndStateType == LevelCompletionResults.LevelEndStateType.Cleared)
				songDataType = SongDataType.pass;
			else if (songDataType == SongDataType.none && results.levelEndStateType == LevelCompletionResults.LevelEndStateType.Failed)
				songDataType = SongDataType.fail;

			foreach (ITracker t in trackers.Values)
				t.EndOfSong(results);
			foreach (ITracker t in deepTrackers.Values)
				t.EndOfSong(results);
			deepTrackerResult = JsonConvert.SerializeObject(this, Formatting.None);


			// We store DeepTrackers in a private variable so it does not get serialized. Maybe find a better way to do so ?
			iDontLikeThat = deepTrackers;
			deepTrackers = null;
			trackerResult = JsonConvert.SerializeObject(this, Formatting.None);
			deepTrackers = iDontLikeThat;
			iDontLikeThat = null;
		}

		public string GetDeepTrackersResults() => deepTrackerResult;
		public string GetTrackersResults() => trackerResult;
	}
}
