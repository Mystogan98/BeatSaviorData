using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaviorData.Trackers
{
	class ScoreTracker : ITracker
	{
		public int score, personalBest; 
		public float rawRatio, modifiedRatio, personalBestRawRatio, personalBestModifiedRatio;
		public List<string> modifiers = new List<string>();

		private int maxScore, maxRawScore;
		private float multiplier;

		public void EndOfSong(LevelCompletionResults results)
		{
			modifiedRatio = Mathf.RoundToInt(score * multiplier) / (float)maxScore;
			rawRatio = score / (float)maxRawScore;
		}

		public void RegisterTracker(SongData data)
		{
			data.GetScoreController().scoreDidChangeEvent += UpdateScore;

			IDifficultyBeatmap beatmap = data.GetGCSSD().difficultyBeatmap;
			PlayerLevelStatsData stats = data.GetPlayerData().playerData.GetPlayerLevelStatsData(
				beatmap.level.levelID, beatmap.difficulty, beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic);
			maxRawScore = ScoreController.MaxRawScoreForNumberOfNotes(beatmap.beatmapData.notesCount);


			multiplier = GetTotalMultiplier(data.GetPlayerData().playerData.gameplayModifiers);
			maxScore = Mathf.RoundToInt(maxRawScore * multiplier);
			personalBestModifiedRatio = stats.highScore / (float)maxScore;
			personalBestRawRatio = stats.highScore / (float)maxRawScore;
			personalBest = stats.highScore;
		}
		
		private float GetTotalMultiplier(GameplayModifiers _modifiers)
		{
			float multiplier = 1;

			if (_modifiers.disappearingArrows) { multiplier += 0.02f; modifiers.Add("DA"); }
			if (_modifiers.songSpeed == GameplayModifiers.SongSpeed.Faster) { multiplier += 0.08f; modifiers.Add("FS"); }
			if (_modifiers.songSpeed == GameplayModifiers.SongSpeed.Slower) { multiplier -= 0.3f; modifiers.Add("SS"); }
			if (_modifiers.ghostNotes) { multiplier += 0.04f; modifiers.Add("GN"); }
			if (_modifiers.noArrows) { multiplier -= 0.3f; modifiers.Add("NA"); }
			if (_modifiers.noBombs) { multiplier -= 0.1f; modifiers.Add("NB"); }
			if (_modifiers.noFail) { multiplier -= 0.5f; modifiers.Add("NF"); }
			if (_modifiers.noObstacles) { multiplier -= 0.05f; modifiers.Add("NO"); }

			return Mathf.RoundToInt(maxRawScore * multiplier);
		}

		private void UpdateScore(int rawScore, int modified)
		{
			score = rawScore;
		}
	}
}
