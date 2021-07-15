using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaviorData
{
	public enum BSDNoteType
	{
		right,
		left
	}

	public enum CutType
	{
		cut,
		miss,
		badCut
	}

	public class Note
	{
		private static int actualId = 0;

		public BSDNoteType noteType;
		public NoteCutDirection noteDirection;
		public int index, id;
		public float time;
		public CutType cutType;
		public int multiplier;
		public int[] score;
		public float timeDeviation, speed, preswing, postswing, distanceToCenter;
		public float[] cutPoint, saberDir, cutNormal;
		public float timeDependence;

		private readonly NoteCutInfo info;

		private Note(NoteData data, CutType cut)
		{
			if (data.colorType == ColorType.ColorB)
				noteType = BSDNoteType.right;
			else if (data.colorType == ColorType.ColorA)
				noteType = BSDNoteType.left;

			score = new int[] { 0, 0, 0 };
			index = data.lineIndex + 4 * (int)data.noteLineLayer;
			time = data.time;
			id = actualId;
			actualId++;

			noteDirection = data.cutDirection;
			cutType = cut;
		}

		public Note(NoteData data, CutType cut, NoteCutInfo _info, int _multiplier) : this(data, cut)
		{
			multiplier = _multiplier;

			info = _info;

			if (/*info != null && */info.swingRatingCounter != null)
			{
				timeDependence = Math.Abs(info.cutNormal.z);
				// info.swingRatingCounter.UnregisterDidFinishReceiver(new WaitForSwing(this));
				info.swingRatingCounter.RegisterDidFinishReceiver(new WaitForSwing(this));
				distanceToCenter = info.cutDistanceToCenter;
			}
		}

		public Note(NoteData data, CutType cut, int _multiplier) : this(data, cut)
		{
			// This is a miss
			multiplier = _multiplier;
		}

		public static void ResetID()
		{
			actualId = 0;
		}

		public int GetTotalScore()
		{
			return (score[0] + score[1] + score[2]) * multiplier;
		}

		public bool IsAMiss() => GetTotalScore() == 0;
		public NoteCutInfo GetInfo() => info;

		private class WaitForSwing : ISaberSwingRatingCounterDidFinishReceiver
		{
			private Note n;

			public WaitForSwing(Note _n)
			{
				n = _n;
			}

			public void HandleSaberSwingRatingCounterDidFinish(ISaberSwingRatingCounter s)
			{
				ScoreModel.RawScoreWithoutMultiplier(s, n.info.cutDistanceToCenter, out int before, out int after, out int accuracy);

				SwingHolder sh = SwingTranspilerHandler.GetSwing(s as SaberSwingRatingCounter);
				if (sh != null)
				{
					n.preswing = sh.preswing;
					n.postswing = sh.postswing;
				}

				n.score = new int[] { before, accuracy, after };
				n.timeDeviation = n.info.timeDeviation;
				n.speed = n.info.saberSpeed;
				n.cutPoint = Utils.FloatArrayFromVector(n.info.cutPoint);
				n.saberDir = Utils.FloatArrayFromVector(n.info.saberDir);
				n.cutNormal = Utils.FloatArrayFromVector(n.info.cutNormal);

				n.info.swingRatingCounter.UnregisterDidFinishReceiver(this);
			}
		}
	}
}
