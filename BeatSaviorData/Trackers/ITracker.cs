using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaviorData
{
	interface ITracker
	{
		void OnNoteCut(BeatmapObjectSpawnController bosc, INoteController data, NoteCutInfo info);
		void OnNoteMissed(BeatmapObjectSpawnController bosc, INoteController data);
	}
}
