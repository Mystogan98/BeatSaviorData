using System.Collections.Generic;

namespace BeatSaviorData.Trackers
{
	class AccuracyTracker : ITracker
	{
		public float accRight, accLeft, averageAcc;
		public float[] gridAcc = new float[12], leftAverageCut = new float[3], rightAverageCut = new float[3], averageCut = new float[3];

		private int cutRight, cutLeft;
		private int[] gridCut = new int[12];

		// I really don't like this but I don't know how to do otherwise. So for now it'll stay
		private Dictionary<SaberSwingRatingCounter, KeyValuePair<NoteCutInfo, int>> thisIsBullshit = new Dictionary<SaberSwingRatingCounter, KeyValuePair<NoteCutInfo, int>>();

		public void EndOfSong(LevelCompletionResults results)
		{
			accRight = Utils.SafeDivide(accRight, cutRight);
			accLeft = Utils.SafeDivide(accLeft, cutLeft);

			for (int i = 0; i < 12; i++)
				gridAcc[i] = Utils.SafeDivide(gridAcc[i], gridCut[i]);

			for (int i = 0; i < 3; i++) {
				leftAverageCut[i] = Utils.SafeDivide(leftAverageCut[i], cutLeft);
				rightAverageCut[i] = Utils.SafeDivide(rightAverageCut[i], cutRight);
				averageCut[i] = (rightAverageCut[i] * cutRight + leftAverageCut[i] * cutLeft) / (cutRight + cutLeft);
				averageCut[i] = Utils.SafeAverage(rightAverageCut[i], cutRight, leftAverageCut[i], cutLeft);
			}

			averageAcc = Utils.SafeAverage(accRight, cutRight, accLeft, cutLeft);
		}

		public void OnNoteCut(NoteData data, NoteCutInfo info, int multiplier)
		{
			if (info.allIsOK && data.noteType != NoteType.Bomb)
			{
				thisIsBullshit.Add(info.swingRatingCounter, new KeyValuePair<NoteCutInfo, int>(info, data.lineIndex + 4 * (int)data.noteLineLayer));
				// For some reason it doesn't work without that
				info.swingRatingCounter.didFinishEvent -= WaitForSwing;
				info.swingRatingCounter.didFinishEvent += WaitForSwing;
			}
		}

		public void RegisterTracker(SongData data)
		{
			data.GetScoreController().noteWasCutEvent += OnNoteCut;
		}

		private void WaitForSwing(SaberSwingRatingCounter s)
		{
			int index = thisIsBullshit[s].Value;
			SaberType type = thisIsBullshit[s].Key.saberType;

			ScoreModel.RawScoreWithoutMultiplier(thisIsBullshit[s].Key, out int before, out int after, out int acc);

			if (type == SaberType.SaberA)
			{
				cutLeft++;
				accLeft += before + acc + after;
				leftAverageCut[0] += before;
				leftAverageCut[1] += acc;
				leftAverageCut[2] += after;
			}
			else if (type == SaberType.SaberB)
			{
				cutRight++;
				accRight += before + acc + after;
				rightAverageCut[0] += before;
				rightAverageCut[1] += acc;
				rightAverageCut[2] += after;
			}

			if (index < 12)
			{
				gridCut[index]++;
				gridAcc[index] += before + acc + after;
			}

			thisIsBullshit.Remove(s);
		}
	}
}
