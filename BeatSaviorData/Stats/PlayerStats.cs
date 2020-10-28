using BS_Utils.Gameplay;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaviorData
{
	class SerializableColor
	{
		public float r, g, b, a;

		public SerializableColor(Color color)
		{
			r = color.r;
			g = color.g;
			b = color.b;
			a = color.a;
		}

		public static implicit operator SerializableColor(Color c) => new SerializableColor(c);
	}

	class PlayerStats
	{
		public string playerID;
		public int averageCutScore, badCutsCount, clearedLevelsCount, failedLevelsCount, fullComboCount, goodCutsCount, handDistanceTravelled, missedCutsCount, playedLevelsCount;
		public long totalScore, cummulativeCutScoreWithoutMultiplier;
		public float timePlayed;
		public SerializableColor saberAColor, saberBColor, lightAColor, lightBColor, obstacleColor;

		public PlayerStats()
		{
			playerID = BSUtilsTemporaryFix.GetUserID().ToString();

			PlayerData playerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First().playerData;
			ColorScheme colors = playerData.colorSchemesSettings.GetSelectedColorScheme();
			PlayerAllOverallStatsData.PlayerOverallStatsData playerStats = playerData.playerAllOverallStatsData.allOverallStatsData;

			averageCutScore = playerStats.averageCutScore;
			badCutsCount = playerStats.badCutsCount;
			clearedLevelsCount = playerStats.cleardLevelsCount;
			failedLevelsCount = playerStats.failedLevelsCount;
			fullComboCount = playerStats.fullComboCount;
			goodCutsCount = playerStats.goodCutsCount;
			handDistanceTravelled = playerStats.handDistanceTravelled;
			missedCutsCount = playerStats.missedCutsCount;
			playedLevelsCount = playerStats.playedLevelsCount;
			totalScore = playerStats.totalScore;
			cummulativeCutScoreWithoutMultiplier = playerStats.cummulativeCutScoreWithoutMultiplier;
			timePlayed = playerStats.timePlayed;

			saberAColor = colors.saberAColor;
			saberBColor = colors.saberBColor;
			lightAColor = colors.environmentColor0;
			lightBColor = colors.environmentColor1;
			obstacleColor = colors.obstaclesColor;

			string json = JsonConvert.SerializeObject(this, Formatting.None);

			FileManager.SavePlayerStats(json);
			HTTPManager.UploadPlayerStats(json);
		}
	}
}
