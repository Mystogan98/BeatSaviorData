using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaviorData.Trackers
{
	class ScoreTracker : ITracker
	{
		public float score, personalBest;
		public void OnNoteCut(BeatmapObjectSpawnController bosc, INoteController data, NoteCutInfo info)
		{
		}

		public void OnNoteMissed(BeatmapObjectSpawnController bosc, INoteController data)
		{
		}
	}
}
