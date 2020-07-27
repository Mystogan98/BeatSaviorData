using IPA;
using IPALogger = IPA.Logging.Logger;
using BS_Utils.Utilities;
using BeatSaberMarkupLanguage.Settings;
using UnityEngine;
using System.Linq;
using HarmonyLib;
using System.Collections.Generic;
using System;

namespace BeatSaviorData
{
	// Add option to disable deep profile
	// Add option to disable profile completly

	[Plugin(RuntimeOptions.SingleStartInit)]
	public class Plugin
	{
		internal static string Name => "BeatSaviorData";

		private SongData songData, storedData;
		internal static Harmony harmony;

		private bool songDataFinished = false;

		[Init]
		public void Init(IPALogger logger) { Logger.log = logger; }

		[OnStart]
		public void OnApplicationStart()
		{
			BSEvents.gameSceneLoaded += GameSceneLoaded;

			BSEvents.levelCleared += UploadData;
			BSEvents.levelFailed += UploadData;
			BSEvents.lateMenuSceneLoadedFresh += UploadStats;
			/*BSEvents.levelQuit += OnLevelQuit;
			BSEvents.levelRestarted += OnLevelRestarted;*/

			BSMLSettings.instance.AddSettingsMenu("BeatSaviorData", "BeatSaviorData.UI.Views.SettingsView.bsml", SettingsMenu.instance);

			harmony = new Harmony("com.Mystogan.BeatSaber.BeatSaviorData");

			//Patch Classes
			harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

			// Create the creator that show stats at the end of a song
			new GameObject("EndOfLevelUICreator").AddComponent<EndOfLevelUICreator>().plugin = this;
		}

		private void UploadStats(ScenesTransitionSetupDataSO obj)
		{
			new PlayerStats();	// Get and upload player related stats
			BSEvents.lateMenuSceneLoadedFresh -= UploadStats;
		}

		private void UploadData(StandardLevelScenesTransitionSetupDataSO data, LevelCompletionResults results)
		{
			if (songData != null && !songData.IsAReplay())
			{
				songData.FinalizeData(results);

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

		public void GameSceneLoaded()
		{
			songData = new SongData();
		}

		public bool IsComputeFinished() => songDataFinished;
		public SongData GetSongData() {
			songDataFinished = false;
			return storedData;
		}
	}
}
