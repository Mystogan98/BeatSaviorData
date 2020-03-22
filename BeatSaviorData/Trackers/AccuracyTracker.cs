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
			// This doesn't take into account the number of note hit by each hand, gotta change that
			averageAcc = (accRight + accLeft) / 2;
		}

		public void OnNoteCut(BeatmapObjectSpawnController bosc, INoteController data, NoteCutInfo info)
		{
			if (info.allIsOK && data.noteData.noteType != NoteType.Bomb)
			{
				thisIsBullshit.Add(info.swingRatingCounter, new KeyValuePair<NoteCutInfo, int>(info, data.noteData.lineIndex + 4 * (int)data.noteData.noteLineLayer));
				// For some reason it doesn't work without that
				info.swingRatingCounter.didFinishEvent -= WaitForSwing;
				info.swingRatingCounter.didFinishEvent += WaitForSwing;
			}
		}

		public void RegisterTracker(SongData data)
		{
			data.GetBOSC().noteWasCutEvent += OnNoteCut;
		}

		private void WaitForSwing(SaberSwingRatingCounter s)
		{
			Saber.SaberType type = thisIsBullshit[s].Key.saberType;

			ScoreController.RawScoreWithoutMultiplier(thisIsBullshit[s].Key, out int before, out int after, out int acc);

			if (type == Saber.SaberType.SaberA)
			{
				cutLeft++;
				accLeft += before + acc + after;
			}
			else if (type == Saber.SaberType.SaberB)
			{
				cutRight++;
				accRight += before + acc + after;
			}

			gridCut[thisIsBullshit[s].Value]++;
			gridAcc[thisIsBullshit[s].Value] += before + acc + after;

			thisIsBullshit.Remove(s);
		}
	}
}
