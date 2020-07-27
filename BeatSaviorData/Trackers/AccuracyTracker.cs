using System.Collections.Generic;

namespace BeatSaviorData.Trackers
{
	// Remove thisIsBullshit by a private class like in NoteTracker

	class AccuracyTracker : ITracker
	{
		public float accRight, accLeft, averageAcc;
		public float leftSpeed, rightSpeed, averageSpeed;
		public float[] gridAcc = new float[12], leftAverageCut = new float[3], rightAverageCut = new float[3], averageCut = new float[3];

		private int cutRight, cutLeft;
		private int[] gridCut = new int[12];

		public void EndOfSong(LevelCompletionResults results, SongData data)
		{
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

			data.GetScoreController().noteWasCutEvent -= OnNoteCut;
		}

		public void RegisterTracker(SongData data)
		{
			data.GetScoreController().noteWasCutEvent += OnNoteCut;
		}

		public void OnNoteCut(NoteData data, NoteCutInfo info, int multiplier)
		{
			if (info.allIsOK && data.noteType != NoteType.Bomb)
			{
				new NoteCutHandler(this, info, data.lineIndex + 4 * (int)data.noteLineLayer);
			}
		}

		private void AddNoteInfo(NoteCutInfo info, int index)
		{
			if (info == null)
				return;

			ScoreModel.RawScoreWithoutMultiplier(info, out int before, out int after, out int acc);

			if (info.saberType == SaberType.SaberA)
			{
				cutLeft++;
				accLeft += before + acc + after;
				leftAverageCut[0] += before;
				leftAverageCut[1] += acc;
				leftAverageCut[2] += after;
				leftSpeed += info.saberSpeed;
			}
			else if (info.saberType == SaberType.SaberB)
			{
				cutRight++;
				accRight += before + acc + after;
				rightAverageCut[0] += before;
				rightAverageCut[1] += acc;
				rightAverageCut[2] += after;
				rightSpeed += info.saberSpeed;
			}

			if (index < 12 && index >= 0)
			{
				gridCut[index]++;
				gridAcc[index] += before + acc + after;
			}
		}

		private class NoteCutHandler
		{
			private AccuracyTracker tracker;
			private int index;
			private NoteCutInfo info;

			public NoteCutHandler(AccuracyTracker _tracker, NoteCutInfo _info, int _index)
			{
				tracker = _tracker;
				info = _info;
				index = _index;

				info.swingRatingCounter.didFinishEvent -= WaitForSwing;
				info.swingRatingCounter.didFinishEvent += WaitForSwing;
			}

			private void WaitForSwing(SaberSwingRatingCounter s)
			{
				tracker.AddNoteInfo(info, index);

				info.swingRatingCounter.didFinishEvent -= WaitForSwing;
			}
		}
	}
}
