using IPA;
using IPALogger = IPA.Logging.Logger;
using BS_Utils.Utilities;
using BeatSaberMarkupLanguage.Settings;

namespace BeatSaviorData
{
	[Plugin(RuntimeOptions.SingleStartInit)]
	public class Plugin
	{
		internal static string Name => "BeatSaviorData";

		private SongData songData;

		[Init]
		public void Init(IPALogger logger) { Logger.log = logger; }

		[OnStart]
		public void OnApplicationStart()
		{
			BS_Utils.Utilities.BSEvents.gameSceneLoaded += GameSceneLoaded;

			BSEvents.levelCleared += UploadData;
			BSEvents.levelFailed += UploadData;
			/*BSEvents.levelQuit += OnLevelQuit;
			BSEvents.levelRestarted += OnLevelRestarted;*/

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

		public void GameSceneLoaded()
		{
			if(!SettingsMenu.instance.DisableFails || !SettingsMenu.instance.DisablePass)
				songData = new SongData();
		}

		private void ShowData(string json)
		{
			Logger.log.Info("***************************");
			Logger.log.Info(json);
			Logger.log.Info("***************************");
		}
	}
}
