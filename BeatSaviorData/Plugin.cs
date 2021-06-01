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
using BeatSaviorData.Trackers;
using UnityEngine.UI;
using System.Collections;
using HMUI;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace BeatSaviorData
{
	[Plugin(RuntimeOptions.SingleStartInit)]
	public class Plugin
	{
		internal static string Name => "BeatSaviorData";
		internal static bool fish;

		private Harmony harmony;

		private SongData songData, storedData;
		private bool songDataFinished = false;

		[Init]
		public void Init(IPALogger logger) 
		{
			ServicePointManager.ServerCertificateValidationCallback = ((object message, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true);
			Logger.log = logger; 
			UserIDFix.GetUserID(); 
		}

		[OnStart]
		public void OnApplicationStart()
		{
			fish = DateTime.Now.Month == 4 && DateTime.Now.Day == 1;

			SceneManager.activeSceneChanged += OnActiveSceneChanged;

			BSEvents.levelCleared += UploadSoloData;
			BSEvents.levelFailed += UploadSoloData;
			BSEvents.lateMenuSceneLoadedFresh += UploadStats;

			BSEvents.levelQuit += DiscardSongData;
			BSEvents.levelRestarted += DiscardSongData;

			BSMLSettings.instance.AddSettingsMenu("BeatSaviorData", "BeatSaviorData.UI.Views.SettingsView.bsml", SettingsMenu.instance);

			harmony = new Harmony("BeatSaviorData");

			// Patch Classes
			harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

			// Create the creator that show stats at the end of a song
			new GameObject("EndOfLevelUICreator").AddComponent<EndOfLevelUICreator>().plugin = this;
		}

		private void UploadStats(ScenesTransitionSetupDataSO obj)
		{ 
			new PlayerStats();	// Get and upload player related stats
			BSEvents.lateMenuSceneLoadedFresh -= UploadStats;

			//SharedCoroutineStarter.instance.StartCoroutine(TMP());
		}

		/*private IEnumerator TMP()
		{
			GameObject screen = GameObject.Find("LeftScreen/GameplaySetupViewController/BSMLBackground");

			while(screen == null)
			{
				yield return new WaitForSeconds(1);
				screen = GameObject.Find("LeftScreen/GameplaySetupViewController/BSMLBackground");
			}

			GameObject go = new GameObject("BSDHistoryButton", typeof(Button));
			go.transform.SetParent(screen.transform, false);
			RectTransform rt = go.AddComponent<RectTransform>();

			rt.anchorMin = new Vector2(0.15f, -0.08f);
			rt.anchorMax = new Vector2(0.35f, -0.02f);
			rt.offsetMin = Vector2.zero;
			rt.offsetMax = Vector2.zero;

			ImageView image = go.AddComponent<ImageView>();
			//image.sprite = BeatSaberMarkupLanguage.Utilities.ImageResources.WhitePixel;
			image.sprite = Resources.FindObjectsOfTypeAll<Sprite>().Where(x => x.name == "RoundRect10").First();
			image.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;
			image.type = Image.Type.Sliced;
			image.preserveAspect = true;
			image.color = new Color32(63, 21, 69, 255);     // new Color32(140, 42, 145, 255);
		}*/

		private void DiscardSongData(StandardLevelScenesTransitionSetupDataSO data, LevelCompletionResults results)
		{
			// Prevent a nullref exception, but it shouldn't happen in the first place (might mean some trackers do not get destroyed), so some investigation required
			if (songData == null)
				return;

			songData.GetDataCollector().UnregisterCollector(songData);
			songData = null;
		}

		[OnExit]
		public void OnApplicationExit()
		{
			harmony.UnpatchAll("BeatSaviorData");

			SceneManager.activeSceneChanged -= OnActiveSceneChanged;

			BSEvents.levelCleared -= UploadSoloData;
			BSEvents.levelFailed -= UploadSoloData;
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
						Logger.log.Info("Pass upload is disabled in the settings.");
					else if (results.levelEndStateType == LevelCompletionResults.LevelEndStateType.Failed && SettingsMenu.instance.DisableFails)
						Logger.log.Info("Fail upload is disabled in the settings.");
					else
					{
						HTTPManager.UploadSongJson(songData.GetTrackersResults(), songData.GetTinyJson());
					}
				}

				FileManager.SaveSongStats(songData.GetDeepTrackersResults());
				FileManager.SavePBScoreGraph((songData.trackers["scoreGraphTracker"] as ScoreGraphTracker).graph, (songData.trackers["scoreTracker"] as ScoreTracker).score, songData.songID);

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
                if (BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer)
					return;
                    
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
