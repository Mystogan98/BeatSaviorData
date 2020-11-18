using IPA;
using IPALogger = IPA.Logging.Logger;
using BS_Utils.Utilities;
using BeatSaberMarkupLanguage.Settings;
using UnityEngine;
using System.Linq;
using HarmonyLib;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

namespace BeatSaviorData
{
	[Plugin(RuntimeOptions.SingleStartInit)]
	public class Plugin
	{
		internal static string Name => "BeatSaviorData";

		private SongData songData, storedData;
		internal static Harmony harmony;

		private bool songDataFinished = false;

		[Init]
		public void Init(IPALogger logger) { Logger.log = logger; UserIDFix.GetUserID(); }

		[OnStart]
		public void OnApplicationStart()
		{
			BSEvents.gameSceneLoaded += GameSceneLoaded;

			SceneManager.activeSceneChanged += OnActiveSceneChanged;

			BSEvents.levelCleared += UploadSoloData;
			BSEvents.levelFailed += UploadSoloData;
			BSEvents.lateMenuSceneLoadedFresh += UploadStats;

			BSEvents.levelQuit += DiscardSongData;
			BSEvents.levelRestarted += DiscardSongData;

			BSMLSettings.instance.AddSettingsMenu("BeatSaviorData", "BeatSaviorData.UI.Views.SettingsView.bsml", SettingsMenu.instance);

			harmony = new Harmony("com.Mystogan.BeatSaber.BeatSaviorData");

			//Patch Classes
			harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

			// Create the creator that show stats at the end of a song
			new GameObject("EndOfLevelUICreator").AddComponent<EndOfLevelUICreator>().plugin = this;
		}

		private void DiscardSongData(StandardLevelScenesTransitionSetupDataSO data, LevelCompletionResults results)
		{
			songData.GetDataCollector().UnregisterCollector(songData);
			songData = null;
		}

		[OnExit]
		public void OnApplicationExit()
		{
			harmony.UnpatchAll();

			SceneManager.activeSceneChanged -= OnActiveSceneChanged;

			BSEvents.levelCleared -= UploadSoloData;
			BSEvents.levelFailed -= UploadSoloData;
			BSEvents.lateMenuSceneLoadedFresh -= UploadStats;
		}

		private void UploadStats(ScenesTransitionSetupDataSO obj)
		{
			new PlayerStats();	// Get and upload player related stats
			BSEvents.lateMenuSceneLoadedFresh -= UploadStats;
		}

		private void UploadData(StandardLevelScenesTransitionSetupDataSO data, LevelCompletionResults results, bool isCampaign)
		{
			if (songData != null && !songData.IsAReplay() && results.levelEndAction == LevelCompletionResults.LevelEndAction.None)
			{
				songData.FinalizeData(results);
				if(isCampaign)
					songData.songDataType = SongDataType.campaign;

				if (!songData.IsPraticeMode()) {
					if (results.levelEndStateType == LevelCompletionResults.LevelEndStateType.Cleared && SettingsMenu.instance.DisablePass)
						Logger.log.Info("Pass upload is disabled.");
					else if (results.levelEndStateType == LevelCompletionResults.LevelEndStateType.Failed && SettingsMenu.instance.DisableFails)
						Logger.log.Info("Fail upload is disabled.");
					else
					{
						HTTPManager.UploadSongJson(songData.GetTrackersResults());
					}
				}

				FileManager.SaveSongStats(songData.GetDeepTrackersResults());

				storedData = songData;
				songDataFinished = true;
				songData = null;
			}
		}

		private void UploadSoloData(StandardLevelScenesTransitionSetupDataSO data, LevelCompletionResults results) => UploadData(data, results, false);

		private void UploadCampaignData(MissionLevelScenesTransitionSetupDataSO data, MissionCompletionResults results) => UploadData(null, results.levelCompletionResults, true);

		public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
		{
			if (nextScene.name == "GameCore")
			{
				GameSceneLoaded();

				foreach(MissionLevelScenesTransitionSetupDataSO m in Resources.FindObjectsOfTypeAll<MissionLevelScenesTransitionSetupDataSO>())
				{
					m.didFinishEvent -= this.UploadCampaignData;
					m.didFinishEvent += this.UploadCampaignData;
				}
			}
		}

		public void GameSceneLoaded()
		{
			songData = new SongData();
			songDataFinished = false;
		}

		public bool IsComputeFinished() => songDataFinished;
		public SongData GetSongData() {
			songDataFinished = false;
			return storedData;
		}
	}
}
