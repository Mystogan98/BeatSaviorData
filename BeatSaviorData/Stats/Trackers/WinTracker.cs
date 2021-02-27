namespace BeatSaviorData.Trackers
{
	class WinTracker : ITracker
	{
		public bool won;
		public string rank;
		public float endTime;
		public int nbOfPause;

		public void EndOfSong(LevelCompletionResults results,  SongData data)
		{
			won = results.levelEndStateType == LevelCompletionResults.LevelEndStateType.Cleared;
			endTime = results.endSongTime;
			rank = RankModel.GetRankName(results.rank);

			if (!SettingsMenu.instance.HideNbOfPauses)
				nbOfPause = data.GetDataCollector().nbOfPause;
			else
				nbOfPause = 999;
		}
	}
}
