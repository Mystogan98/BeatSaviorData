using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IPA;
using IPA.Config;
using IPA.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;
using BS_Utils.Gameplay;
using BS_Utils.Utilities;
using Newtonsoft.Json;

namespace BeatSaviorData
{
	public class Plugin : IBeatSaberPlugin
	{
		internal static string Name => "BeatSaviorData";

		private SongData songData;
		private bool init = false;

		public void Init(IPALogger logger) { Logger.log = logger; }

		public void OnApplicationStart()
		{
			if (!init)
			{
				BSEvents.levelCleared += UploadData;
				BSEvents.levelFailed += UploadData;
				/*BSEvents.levelQuit += OnLevelQuit;
				BSEvents.levelRestarted += OnLevelRestarted;*/
				init = true;
			}
		}

		private void UploadData(StandardLevelScenesTransitionSetupDataSO data, LevelCompletionResults results)
		{
			if (songData != null)
			{
				if (!songData.IsAReplay() && !songData.IsPraticeMode()) {
					string json = songData.FinalizeData(results);
					ShowData(json);
					// upload
					if (HTTPManager.uploadJson(json))
						Logger.log.Info("BSD : Upload succeeded !");
					else
						Logger.log.Info("BSD : Upload failed.");
				}
				else if (songData.IsAReplay()) {
					Logger.log.Info("BSD : That was a replay you cheater (╯°□°）╯︵ ┻━┻");
				}
				else if(songData.IsPraticeMode())
				{
					Logger.log.Info("BSD : Don't try to cheat in practice mode, I see you kid. :eyes:");
				}
				songData = null;
			}
		}

		public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
		{
			if (nextScene.name == "GameCore")
				songData = new SongData();
		}

		#region InterfaceImplementation
		public void OnApplicationQuit() { }

		public void OnUpdate() { }

		public void OnFixedUpdate() { }

		public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) { }

		public void OnSceneUnloaded(Scene scene) { }
		#endregion

		private void ShowData(string json)
		{
			Logger.log.Info("***************************");
			Logger.log.Info(json);
			Logger.log.Info("***************************");
		}
	}
}
