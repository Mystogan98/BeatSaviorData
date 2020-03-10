using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaviorData.Trackers
{
	class AccuracyTracker : ITracker
	{
		public float accRight, accLeft, averageAcc;
		public float[] gridAcc = new float[12];

		public void OnNoteCut(BeatmapObjectSpawnController bosc, INoteController data, NoteCutInfo info)
		{
		}

		public void OnNoteMissed(BeatmapObjectSpawnController bosc, INoteController data)
		{
		}
	}
}
