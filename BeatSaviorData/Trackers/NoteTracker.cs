using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaviorData.Trackers
{
	public enum BSD_NoteType
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

	class Note
	{
		private static int actualId = 0;

		public BSD_NoteType noteType;
		public NoteCutDirection noteDirection;
		public int index, id;
		public float time;
		public CutType cutType;
		public int multiplier;
		public int[] score;
		public float timeDeviation;
		public float[] cutPoint, saberDir;

		private NoteCutInfo info;

		private Note(NoteData data, CutType cut)
		{
			if (data.noteType == NoteType.NoteB)
				noteType = BSD_NoteType.right;
			else if (data.noteType == NoteType.NoteA)
				noteType = BSD_NoteType.left;

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

			try
			{
				info.swingRatingCounter.didFinishEvent -= WaitForSwing;
				info.swingRatingCounter.didFinishEvent += WaitForSwing;
			} catch	// If it is a miss, info will be null
			{
				score = new int[] { 0, 0, 0 };
			}
		}

		public Note(NoteData data, CutType cut, int _multiplier) : this(data, cut)
		{
			multiplier = _multiplier;
			score = new int[] { 0, 0, 0 };
		}

		private void WaitForSwing(SaberSwingRatingCounter s)
		{
			ScoreModel.RawScoreWithoutMultiplier(info, out int before, out int after, out int accuracy);

			score = new int[] { before, accuracy, after };
			timeDeviation = info.timeDeviation;
			cutPoint = Utils.FloatArrayFromVector(info.cutPoint);
			saberDir = Utils.FloatArrayFromVector(info.saberDir);

			info.swingRatingCounter.didFinishEvent -= WaitForSwing;
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
	}

	class NoteTracker : ITracker
	{
		public List<Note> notes = new List<Note>();

		public void EndOfSong(LevelCompletionResults results) {	}

		public void OnNoteCut(NoteData data, NoteCutInfo info, int multiplier)
		{
			if (info.allIsOK && data.noteType != NoteType.Bomb)
			{
				notes.Add(new Note(data, CutType.cut, info, multiplier));
			} else if (data.noteType != NoteType.Bomb)
			{
				notes.Add(new Note(data, CutType.badCut, info, multiplier));
			}
		}

		private void NoteTracker_noteWasMissedEvent(NoteData data, int multiplier)
		{
			if(data.noteType != NoteType.Bomb)
			{
				notes.Add(new Note(data, CutType.miss, multiplier));
			}
		}

		public void RegisterTracker(SongData data)
		{
			Note.ResetID();
			data.GetScoreController().noteWasCutEvent += OnNoteCut;
			data.GetScoreController().noteWasMissedEvent += NoteTracker_noteWasMissedEvent;
		}

	}
}
