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

		public void Init(IPALogger logger) { Logger.log = logger; }

		public void OnApplicationStart()
		{
			BSEvents.levelCleared += OnLevelClear;
		}

		private void OnLevelClear(StandardLevelScenesTransitionSetupDataSO data, LevelCompletionResults results)
		{
			if (songData != null/* && !songData.IsItAReplay()*/)
			{
				if (results.levelEndStateType == LevelCompletionResults.LevelEndStateType.Cleared)
				{
					string json = songData.FinalizeData();
					ShowData(json);
					// upload
				} else
				{
					Logger.log.Info("BSD : Don't fail the song REEEEEEEEEEEEEE");
				}
			} else
			{
				Logger.log.Info("BSD : That was a replay you cheater (╯°□°）╯︵ ┻━┻");
			}
			songData = null;
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
