using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaviorData.Trackers
{
	class WinTracker : ITracker
	{
		public bool won;
		public float endTime;

		public void EndOfSong(LevelCompletionResults results)
		{
			won = results.levelEndStateType == LevelCompletionResults.LevelEndStateType.Cleared;
			endTime = results.endSongTime;
		}

		public void RegisterTracker(SongData data)
		{
			// This one doesn't need any event
		}
	}
}
