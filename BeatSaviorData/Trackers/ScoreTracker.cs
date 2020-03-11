using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaviorData.Trackers
{
	class ScoreTracker : ITracker
	{
		public float score, personalBest, ratio, personalBestRatio;

		public void EndOfSong()
		{
		}

		public void RegisterTracker(SongData data)
		{
			data.GetScoreController().scoreDidChangeEvent += UpdateScore;

			GameplayModifiers gameplayMods = data.GetPlayerData().playerData.gameplayModifiers;
			IDifficultyBeatmap beatmap = data.GetGCSSD().difficultyBeatmap;
			PlayerLevelStatsData stats = data.GetPlayerData().playerData.GetPlayerLevelStatsData(
				beatmap.level.levelID, beatmap.difficulty, beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic);
			int maxRawScore = ScoreController.MaxRawScoreForNumberOfNotes(beatmap.beatmapData.notesCount);
			_maxPossibleScore = Mathf.RoundToInt(maxRawScore * gameplayModsModel.GetTotalMultiplier(gameplayMods));
			beginningPB = stats.highScore / (float)_maxPossibleScore;
			highScore = stats.highScore;
		}

		private int GetTotalMultiplier(int maxRawScore, GameplayModifiers modifiers)
		{
			return 0;
		}

		private void UpdateScore(int _score, int modified)
		{
			score = modified;
		}
	}
}
