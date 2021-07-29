using IPA.Utilities;
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
		public int maxCombo, bombHit, nbOfPause, nbOfWallHit;

		private int combo, multiplier = 1, multiplierProgress = 0;
		private BeatmapObjectManager bom;
		private ScoreController sc;
		private PlayerHeadAndObstacleInteraction phaoi;

		public void RegisterCollector(SongData data)
		{
			Note.ResetID();
			SwingTranspilerHandler.Reset();

			bom = data.GetBOM();
			sc = data.GetScoreController();
			bom.noteWasCutEvent += OnNoteCut;
			bom.noteWasMissedEvent += OnNoteMiss;
			sc.comboBreakingEventHappenedEvent += BreakCombo;
			BS_Utils.Utilities.BSEvents.songPaused += SongPaused;
		}

		public void UnregisterCollector(SongData data)
		{
			if (combo > maxCombo)
				maxCombo = combo;

			bom.noteWasCutEvent -= OnNoteCut;
			bom.noteWasMissedEvent -= OnNoteMiss;
			sc.comboBreakingEventHappenedEvent -= BreakCombo;
			BS_Utils.Utilities.BSEvents.songPaused -= SongPaused;
		}

		private void OnNoteCut(NoteController controller, in NoteCutInfo info)
		{
			// (data.colorType != ColorType.None) checks if it is not a bomb
			if (info.allIsOK && controller.noteData.colorType != ColorType.None)
			{
				combo++;
				ComputeMultiplier(true);
				notes.Add(new Note(controller, CutType.cut, info, multiplier));
			}
			else if (controller.noteData.colorType != ColorType.None)
			{
				ComputeMultiplier(false);
				notes.Add(new Note(controller, CutType.badCut, info, multiplier));
			} 
			else if (controller.noteData.colorType == ColorType.None)
			{
				ComputeMultiplier(false);
				bombHit++;
			}
		}

		private void OnNoteMiss(NoteController controller)
		{
			if (controller.noteData.colorType != ColorType.None)
			{
				ComputeMultiplier(false);
				notes.Add(new Note(controller, CutType.miss, multiplier));
			}
		}

		private void ComputeMultiplier(bool goodHit)
		{
			if(!goodHit)
			{
				if(multiplier > 1)
					multiplier /= 2;
				multiplierProgress = 0;
			} else if (multiplier < 8)
			{
				multiplierProgress++;
				if(multiplierProgress == multiplier * 2)
				{
					multiplierProgress = 0;
					multiplier *= 2;
				}
			}
		}

		private void BreakCombo()
		{
			phaoi = phaoi ?? sc.GetField<PlayerHeadAndObstacleInteraction, ScoreController>("_playerHeadAndObstacleInteraction");

			if (phaoi != null && phaoi.intersectingObstacles.Count > 0)
			{
				// We only reset multiplier on walls hit because we already count miss, badcuts and bombs in other events
				ComputeMultiplier(false);
				nbOfWallHit++;
			}

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
