using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaviorData
{
	public class DataCollector
	{
		public List<Note> notes = new List<Note>();
		public int maxCombo, bombHit, nbOfPause;

		private int combo;

		public void RegisterCollector(SongData data)
		{
			Note.ResetID();
			data.GetScoreController().noteWasCutEvent += OnNoteCut;
			data.GetScoreController().noteWasMissedEvent += OnNoteMiss;
			data.GetScoreController().comboBreakingEventHappenedEvent += BreakCombo;
			BS_Utils.Utilities.BSEvents.songPaused += SongPaused;
		}

		public void UnregisterCollector(SongData data)
		{
			if (combo > maxCombo)
				maxCombo = combo;

			data.GetScoreController().noteWasCutEvent -= OnNoteCut;
			data.GetScoreController().noteWasMissedEvent -= OnNoteMiss;
			data.GetScoreController().comboBreakingEventHappenedEvent -= BreakCombo;
			BS_Utils.Utilities.BSEvents.songPaused -= SongPaused;
		}

		private void OnNoteCut(NoteData data, NoteCutInfo info, int multiplier)
		{
			if (info.allIsOK && data.noteType != NoteType.Bomb)
			{
				combo++;
				notes.Add(new Note(data, CutType.cut, info, multiplier));
			}
			else if (data.noteType != NoteType.Bomb)
			{
				notes.Add(new Note(data, CutType.badCut, info, multiplier));
			} 
			else if (data.noteType == NoteType.Bomb)
			{
				bombHit++;
			}
		}

		private void OnNoteMiss(NoteData data, int multiplier)
		{
			if (data.noteType != NoteType.Bomb)
			{
				notes.Add(new Note(data, CutType.miss, multiplier));
			}
		}

		private void BreakCombo()
		{
			if (combo > maxCombo)
				maxCombo = combo;
			combo = 0;
		}

		private void SongPaused()
		{
			nbOfPause++;
		}

	}
}
