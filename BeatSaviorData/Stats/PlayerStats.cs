﻿using BS_Utils.Gameplay;
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
		public long totalScore;
		public float timePlayed;
		public SerializableColor saberAColor, saberBColor, lightAColor, lightBColor, obstacleColor;

		public PlayerStats()
		{
			if (UserIDFix.UserIDIsReady)
				GetAndUpload();
			else
				UserIDFix.UserIDReady += GetAndUpload;
		}

		private void GetAndUpload()
		{
			UserIDFix.UserIDReady -= GetAndUpload;
			playerID = UserIDFix.UserID;

			// Register the login in DB
			// HTTPManager.client.GetAsync(PrivateKeys.BSDRegisterUrl + playerID);

			PlayerData playerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First().playerData;
			ColorScheme colors = playerData.colorSchemesSettings.GetSelectedColorScheme();
			PlayerAllOverallStatsData.PlayerOverallStatsData playerStats = playerData.playerAllOverallStatsData.allOverallStatsData;

			averageCutScore = (int) playerStats.totalScore / playerStats.goodCutsCount;
			badCutsCount = playerStats.badCutsCount;
			clearedLevelsCount = playerStats.clearedLevelsCount;
			failedLevelsCount = playerStats.failedLevelsCount;
			fullComboCount = playerStats.fullComboCount;
			goodCutsCount = playerStats.goodCutsCount;
			handDistanceTravelled = playerStats.handDistanceTravelled;
			missedCutsCount = playerStats.missedCutsCount;
			playedLevelsCount = playerStats.playedLevelsCount;
			totalScore = playerStats.totalScore;
			timePlayed = playerStats.timePlayed;

			saberAColor = colors.saberAColor;
			saberBColor = colors.saberBColor;
			lightAColor = colors.environmentColor0;
			lightBColor = colors.environmentColor1;
			obstacleColor = colors.obstaclesColor;

			string json = JsonConvert.SerializeObject(this, Formatting.None);

			FileManager.SavePlayerStats(json);
			// HTTPManager.UploadPlayerStats(json);
		}
	}
}
