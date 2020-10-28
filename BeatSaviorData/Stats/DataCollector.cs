using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BeatmapSaveData;

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
			// (data.colorType != ColorType.None) check if it is not a bomb
			if (info.allIsOK && data.colorType != ColorType.None)
			{
				combo++;
				notes.Add(new Note(data, CutType.cut, info, multiplier));
			}
			else if (data.colorType != ColorType.None)
			{
				notes.Add(new Note(data, CutType.badCut, info, multiplier));
			} 
			else if (data.colorType == ColorType.None)
			{
				bombHit++;
			}
		}

		private void OnNoteMiss(NoteData data, int multiplier)
		{
			if (data.colorType != ColorType.None)
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
