using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
		public float[] noteCenter, noteRotation;
		public float timeDeviation, speed, preswing, postswing, distanceToCenter;
		public float[] cutPoint, saberDir, cutNormal;
		public float timeDependence;

		private readonly NoteCutInfo info;

		private Note(GoodCutScoringElement goodcut, CutType cut)
		{
			NoteData data = goodcut.noteData;

			noteCenter = Utils.FloatArrayFromVector(goodcut.cutScoreBuffer.noteCutInfo.notePosition);
			noteRotation = Utils.FloatArrayFromVector(goodcut.cutScoreBuffer.noteCutInfo.noteRotation.eulerAngles);

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

		private Note(NoteController controller, CutType cut)
		{
			NoteData data = controller.noteData;

			noteCenter = Utils.FloatArrayFromVector(controller.noteTransform.position);
			noteRotation = Utils.FloatArrayFromVector(controller.noteTransform.rotation.eulerAngles);

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

		public Note(GoodCutScoringElement goodCut, CutType cut, NoteCutInfo _info, int _multiplier) : this(goodCut, cut)
		{
			multiplier = _multiplier;

			info = _info;

			timeDependence = Math.Abs(info.cutNormal.z);
			score = new int[] { 0, goodCut.cutScoreBuffer.centerDistanceCutScore, 0 };
			//(goodCut.cutScoreBuffer as CutScoreBuffer).GetField<SaberSwingRatingCounter, CutScoreBuffer>("_saberSwingRatingCounter").RegisterDidFinishReceiver(new WaitForSwing(this));

			goodCut.cutScoreBuffer.RegisterDidFinishReceiver(new WaitForSwing(this));

			distanceToCenter = info.cutDistanceToCenter;
		}
		
		// Bad Cut
		public Note(NoteController controller, CutType cut, NoteCutInfo _info, int _multiplier) : this(controller, cut)
		{
			multiplier = _multiplier;

			info = _info;
		}

		public Note(NoteController controller, CutType cut, int _multiplier) : this(controller, cut)
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

		private class WaitForSwing : ISaberSwingRatingCounterDidFinishReceiver, ICutScoreBufferDidFinishReceiver
		{
			private Note n;

			public WaitForSwing(Note _n)
			{
				n = _n;
			}

            public void HandleCutScoreBufferDidFinish(CutScoreBuffer cutScoreBuffer)
            {
				SwingHolder sh = SwingTranspilerHandler.GetSwing(cutScoreBuffer.GetField<SaberSwingRatingCounter, CutScoreBuffer>("_saberSwingRatingCounter"));
				if (sh != null)
				{
					n.preswing = sh.preswing;
					n.postswing = sh.postswing;
				}

				n.score[0] = cutScoreBuffer.beforeCutScore;
				n.score[2] = cutScoreBuffer.afterCutScore;
				n.timeDeviation = n.info.timeDeviation;
				n.speed = n.info.saberSpeed;
				n.cutPoint = Utils.FloatArrayFromVector(n.info.cutPoint);
				n.saberDir = Utils.FloatArrayFromVector(n.info.saberDir);
				n.cutNormal = Utils.FloatArrayFromVector(n.info.cutNormal);

				cutScoreBuffer.UnregisterDidFinishReceiver(this);
			}

            public void HandleSaberSwingRatingCounterDidFinish(ISaberSwingRatingCounter s)
			{
				//ScoreModel.RawScoreWithoutMultiplier(s, n.info.cutDistanceToCenter, out int before, out int after, out int accuracy);

				SwingHolder sh = SwingTranspilerHandler.GetSwing(s as SaberSwingRatingCounter);
				if (sh != null)
				{
					n.preswing = sh.preswing;
					n.postswing = sh.postswing;
				}

				n.score[0] = Mathf.RoundToInt(70f * s.beforeCutRating);
				n.score[2] = Mathf.RoundToInt(30f * s.afterCutRating);
				n.timeDeviation = n.info.timeDeviation;
				n.speed = n.info.saberSpeed;
				n.cutPoint = Utils.FloatArrayFromVector(n.info.cutPoint);
				n.saberDir = Utils.FloatArrayFromVector(n.info.saberDir);
				n.cutNormal = Utils.FloatArrayFromVector(n.info.cutNormal);

				s.UnregisterDidFinishReceiver(this);
			}
		}
	}
}
