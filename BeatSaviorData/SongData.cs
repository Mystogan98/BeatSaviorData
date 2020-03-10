using BS_Utils.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BeatSaviorData.Trackers;

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

		private readonly BeatmapObjectSpawnController BOSC;
		private readonly GameplayCoreSceneSetupData GCSSD;
		private readonly ScoreController scoreController;
		private readonly GameplayModifiersModelSO modifierData;
		private readonly PlayerDataModelSO PlayerData;

		private bool isNotAReplay;

		public SongData()
		{
			BOSC = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().First();
			GCSSD = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData;
			modifierData = Resources.FindObjectsOfTypeAll<GameplayModifiersModelSO>().First();
			PlayerData = Resources.FindObjectsOfTypeAll<PlayerDataModelSO>().First();
			scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().First();

			playerID = GetUserInfo.GetUserID();
			songID = GCSSD.difficultyBeatmap.level.levelID;
			songDifficulty = GCSSD.difficultyBeatmap.difficulty.ToString().ToLower();

			scoreController.scoreDidChangeEvent += IsNotAReplay;

			/*bombCut = 0;
			noteACut = 0;
			noteBCut = 0;
			AccA = 0;
			AccB = 0;
			accGrid = new int[12];
			cutGrid = new int[12];
			thisIsBullshit = new Dictionary<SaberSwingRatingCounter, KeyValuePair<NoteCutInfo, INoteController>>();*/
		
			foreach (ITracker t in trackers.Values)
				RegisterTracker(t);
		}

		private void RegisterTracker(ITracker tracker)
		{
			BOSC.noteWasCutEvent += tracker.OnNoteCut;
			BOSC.noteWasMissedEvent += tracker.OnNoteMissed;
		}

		private void IsNotAReplay(int score, int modifiedScore)
		{
			isNotAReplay = true;
		}

		public bool IsItAReplay() => !isNotAReplay;
	}
}
