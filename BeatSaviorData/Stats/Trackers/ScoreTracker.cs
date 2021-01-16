using System.Collections.Generic;
using UnityEngine;

namespace BeatSaviorData.Trackers
{
	class ScoreTracker : ITracker
	{
		public int rawScore, score, personalBest; 
		public float rawRatio, modifiedRatio, personalBestRawRatio, personalBestModifiedRatio, modifiersMultiplier;
		public List<string> modifiers = new List<string>();

		private int maxScore, maxRawScore;

		public void EndOfSong(LevelCompletionResults results, SongData data)
		{
			GetMaxScores(data);

			foreach(Note n in data.GetDataCollector().notes)
			{
				rawScore += n.GetTotalScore();
			}

			score = Mathf.RoundToInt(rawScore * modifiersMultiplier);

			modifiedRatio = score / (float)maxRawScore;
			rawRatio = rawScore / (float)maxRawScore;
		}

		public void GetMaxScores(SongData data)
		{
			IDifficultyBeatmap beatmap = data.GetGCSSD().difficultyBeatmap;
			PlayerLevelStatsData stats = data.GetPlayerData().playerData.GetPlayerLevelStatsData(beatmap.level.levelID, beatmap.difficulty, beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic);
			maxRawScore = ScoreModel.MaxRawScoreForNumberOfNotes(beatmap.beatmapData.cuttableNotesType);

			modifiersMultiplier = GetTotalMultiplier(data.GetPlayerData().playerData.gameplayModifiers);
			maxScore = Mathf.RoundToInt(maxRawScore * modifiersMultiplier);
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
			if (_modifiers.enabledObstacleType == GameplayModifiers.EnabledObstacleType.NoObstacles) { multiplier -= 0.05f; modifiers.Add("NO"); }

			return multiplier;
		}
	}
}
