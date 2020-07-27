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

			BS_Utils.Utilities.BSEvents.songPaused -= SongPaused;
		}

		public void RegisterTracker(SongData data)
		{
			BS_Utils.Utilities.BSEvents.songPaused += SongPaused;
		}

		private void SongPaused()
		{
			nbOfPause++;
		}
	}
}
