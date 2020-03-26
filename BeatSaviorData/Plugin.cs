using IPA;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;
using BS_Utils.Utilities;
using BeatSaberMarkupLanguage.Settings;

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
			bool a = SettingsMenu.instance.DisableFails;
			a = SettingsMenu.instance.DisablePass;

			if (!init)
			{
				BSEvents.levelCleared += UploadData;
				BSEvents.levelFailed += UploadData;
				/*BSEvents.levelQuit += OnLevelQuit;
				BSEvents.levelRestarted += OnLevelRestarted;*/
				init = true;
			}

			BSMLSettings.instance.AddSettingsMenu("BeatSaviorData", "BeatSaviorData.UI.Views.SettingsView.bsml", SettingsMenu.instance);
		}

		private void UploadData(StandardLevelScenesTransitionSetupDataSO data, LevelCompletionResults results)
		{
			if (songData != null)
			{
				if (!songData.IsAReplay() && !songData.IsPraticeMode()) {
					string json = songData.FinalizeData(results);
					ShowData(json);

					if (results.levelEndStateType == LevelCompletionResults.LevelEndStateType.Cleared && SettingsMenu.instance.DisablePass)
						Logger.log.Info("Pass upload is disabled.");
					else if (results.levelEndStateType == LevelCompletionResults.LevelEndStateType.Failed && SettingsMenu.instance.DisableFails)
						Logger.log.Info("Fail upload is disabled.");
					else
						HTTPManager.uploadJson(json);
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
			if(!SettingsMenu.instance.DisableFails || !SettingsMenu.instance.DisablePass)
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
