using System.Collections.Generic;
using System.Linq;

namespace BeatSaviorData.Trackers
{
	class AccuracyTracker : ITracker
	{
		public float accRight, accLeft, averageAcc;
		public float leftSpeed, rightSpeed, averageSpeed, leftHighestSpeed, rightHighestSpeed;
		public float leftPreswing, rightPreswing, averagePreswing, leftPostswing, rightPostswing, averagePostswing;
		public float leftTimeDependence, rightTimeDependence, averageTimeDependence;
		public float[] leftAverageCut = new float[3], rightAverageCut = new float[3], averageCut = new float[3], gridAcc = new float[12];
		public int[] gridCut = new int[12];

		private int cutRight, cutLeft;

		public void EndOfSong(LevelCompletionResults results, SongData data)
		{
			DataCollector collector = data.GetDataCollector();
			int acc;

			foreach(Note n in collector.notes)
			{
				if (n.IsAMiss())
					continue;

				acc = n.score[0] + n.score[1] + n.score[2];

				if (n.noteType == BSD_NoteType.left) {
					cutLeft++;
					accLeft += acc;
					leftAverageCut[0] += n.score[0];
					leftAverageCut[1] += n.score[1];
					leftAverageCut[2] += n.score[2];
					leftSpeed += n.speed;
					leftPreswing += n.preswing;
					leftPostswing += n.postswing;
					if (n.speed > leftHighestSpeed)
						leftHighestSpeed = n.speed;
					leftTimeDependence += n.timeDependence;
				} else {
					cutRight++;
					accRight += acc;
					rightAverageCut[0] += n.score[0];
					rightAverageCut[1] += n.score[1];
					rightAverageCut[2] += n.score[2];
					rightSpeed += n.speed;
					rightPreswing += n.preswing;
					rightPostswing += n.postswing;
					if (n.speed > rightHighestSpeed)
						rightHighestSpeed = n.speed;
					rightTimeDependence += n.timeDependence;
				}

				if (n.index < 12)	// else it's noodle
				{
					gridCut[n.index]++;
					gridAcc[n.index] += acc;
				}
			}

			accRight = Utils.SafeDivide(accRight, cutRight);
			accLeft = Utils.SafeDivide(accLeft, cutLeft);

			for (int i = 0; i < 12; i++)
				gridAcc[i] = Utils.SafeDivide(gridAcc[i], gridCut[i]);

			for (int i = 0; i < 3; i++) {
				leftAverageCut[i] = Utils.SafeDivide(leftAverageCut[i], cutLeft);
				rightAverageCut[i] = Utils.SafeDivide(rightAverageCut[i], cutRight);
				averageCut[i] = Utils.SafeAverage(rightAverageCut[i], cutRight, leftAverageCut[i], cutLeft);
			}

			averageAcc = Utils.SafeAverage(accRight, cutRight, accLeft, cutLeft);

			averageSpeed = Utils.SafeDivide(leftSpeed + rightSpeed, cutRight + cutLeft);
			leftSpeed = Utils.SafeDivide(leftSpeed, cutLeft);
			rightSpeed = Utils.SafeDivide(rightSpeed, cutRight);

			averageTimeDependence = Utils.SafeDivide(leftTimeDependence + rightTimeDependence, cutRight + cutLeft);
			leftTimeDependence = Utils.SafeDivide(leftTimeDependence, cutLeft);
			rightTimeDependence = Utils.SafeDivide(rightTimeDependence, cutRight);

			averagePreswing = Utils.SafeDivide(leftPreswing + rightPreswing, cutRight + cutLeft);
			leftPreswing = Utils.SafeDivide(leftPreswing, cutLeft);
			rightPreswing = Utils.SafeDivide(rightPreswing, cutRight);

			averagePostswing = Utils.SafeDivide(leftPostswing + rightPostswing, cutRight + cutLeft);
			leftPostswing = Utils.SafeDivide(leftPostswing, cutLeft);
			rightPostswing = Utils.SafeDivide(rightPostswing, cutRight);
		}
	}
}
