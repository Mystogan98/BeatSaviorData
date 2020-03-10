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

		/*private bool isNotAReplay = true;
		private int bombCut, noteACut, noteBCut, AccA, AccB;
		private int[] accGrid, cutGrid;*/

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
					ShowData(results);
					// upload
					songData = null;
				} else
				{
					Logger.log.Info("BSD : Don't fail the song REEEEEEEEEEEEEE");
				}
			} else
			{
				Logger.log.Info("BSD : That was a replay you cheater (╯°□°）╯︵ ┻━┻");
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

		private void ShowData(LevelCompletionResults results)
		{
			string json = JsonConvert.SerializeObject(songData, Formatting.Indented);

			/*int maxRawScore = ScoreController.MaxRawScoreForNumberOfNotes(GCSSD.difficultyBeatmap.beatmapData.notesCount);
			int maxPossibleScore = Mathf.RoundToInt(maxRawScore * modifierData.GetTotalMultiplier(PlayerData.playerData.gameplayModifiers));
			float _AccA = Utils.SafeDivide(AccA, noteACut), _AccB = Utils.SafeDivide(AccB, noteBCut);*/

			Logger.log.Info("***************************");
			/*Logger.log.Info($"BSD : end of level data : \n" +
			$"Level name : {GCSSD.difficultyBeatmap.level.songName}\n" +
			$"Level difficulty : {GCSSD.difficultyBeatmap.difficulty}\n" +
			$"Level id : {GCSSD.difficultyBeatmap.level.levelID}\n" +
			$"Number of miss : {results.missedCount}\n" +
			$"Average Acc : {Utils.SafeDivide(_AccA+_AccB, 2)}\n" +
			$"Average right Acc : {_AccB}\n" +
			$"Average left Acc : {_AccA}\n" +
			$"Average Acc grid : {Utils.SafeDivide(accGrid[0],cutGrid[0])} | {Utils.SafeDivide(accGrid[1],cutGrid[1])} | {Utils.SafeDivide(accGrid[2],cutGrid[2])} | {Utils.SafeDivide(accGrid[3],cutGrid[3])} | {Utils.SafeDivide(accGrid[4],cutGrid[4])} | {Utils.SafeDivide(accGrid[5],cutGrid[5])} | {Utils.SafeDivide(accGrid[6],cutGrid[6])} | {Utils.SafeDivide(accGrid[7],cutGrid[7])} | {Utils.SafeDivide(accGrid[8],cutGrid[8])} | {Utils.SafeDivide(accGrid[9],cutGrid[9])} | {Utils.SafeDivide(accGrid[10],cutGrid[10])} | {Utils.SafeDivide(accGrid[11],cutGrid[11])}\n" +
			$"End score : {results.modifiedScore / maxPossibleScore}\n");*/
			Logger.log.Info(json);
			Logger.log.Info("***************************");
		}

		private Dictionary<SaberSwingRatingCounter, KeyValuePair<NoteCutInfo, INoteController>> thisIsBullshit;

		/*private void OnNoteCut(BeatmapObjectSpawnController bosc, INoteController data, NoteCutInfo info)
		{
			if (info.allIsOK)
			{
				if (data.noteData.noteType == NoteType.Bomb)
					bombCut++;
				else
				{
					thisIsBullshit.Add(info.swingRatingCounter, new KeyValuePair<NoteCutInfo, INoteController>(info, data));
					info.swingRatingCounter.didFinishEvent -= WaitForSwing;
					info.swingRatingCounter.didFinishEvent += WaitForSwing;
				}
			}
		}

		private void WaitForSwing(SaberSwingRatingCounter s)
		{
			Saber.SaberType type = thisIsBullshit[s].Key.saberType;

			ScoreController.RawScoreWithoutMultiplier(thisIsBullshit[s].Key, out int before, out int after, out int acc);

			if (type == Saber.SaberType.SaberA)
			{
				noteACut++;
				AccA += before + acc + after;
			}
			else if (type == Saber.SaberType.SaberB)
			{
				noteBCut++;
				AccB += before + acc + after;
			}

			int index = thisIsBullshit[s].Value.noteData.lineIndex + 4 * (int)thisIsBullshit[s].Value.noteData.noteLineLayer;
			cutGrid[index]++;
			accGrid[index] += before + acc + after;

			thisIsBullshit.Remove(s);
		}*/
	}
}
