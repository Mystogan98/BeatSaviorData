using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaviorData.Trackers
{
	class DistanceTracker : ITracker
	{
		public float rightSaber, leftSaber, rightHand, leftHand;

		public void EndOfSong(LevelCompletionResults results)
		{
			rightSaber = results.rightSaberMovementDistance;
			rightHand = results.rightHandMovementDistance;
			leftSaber = results.leftSaberMovementDistance;
			leftHand = results.leftHandMovementDistance;
		}

		public void RegisterTracker(SongData data) {}
	}
}
