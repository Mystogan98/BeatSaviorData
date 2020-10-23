using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaviorData
{
	public interface ITracker
	{
		void EndOfSong(LevelCompletionResults results, SongData data);
	}
}
