namespace BeatSaviorData.Trackers
{
	class HitTracker : ITracker
	{
		public int leftNoteHit, rightNoteHit, bombHit, miss, maxCombo;

		private int combo;

		public void EndOfSong(LevelCompletionResults results, SongData data) {
			if (combo > maxCombo)
				maxCombo = combo;

			data.GetScoreController().noteWasCutEvent -= OnNoteCut;
			data.GetScoreController().noteWasMissedEvent -= OnNoteMissed;
			data.GetScoreController().comboBreakingEventHappenedEvent -= BreakCombo;
		}

		public void OnNoteCut(NoteData data, NoteCutInfo info, int multiplier)
		{
			if(info.allIsOK)
			{
				combo++;
				switch (data.noteType)
				{
					case NoteType.NoteA:
						leftNoteHit++;
						break;
					case NoteType.NoteB:
						rightNoteHit++;
						break;
				}
			} else
			{
				if (data.noteType == NoteType.Bomb)
					bombHit++;
				else
					miss++;
			}
		}

		private void BreakCombo()
		{
			if (combo > maxCombo)
				maxCombo = combo;
			combo = 0;
		}

		public void OnNoteMissed(NoteData data, int multiplier)
		{
			if (data.noteType != NoteType.Bomb)
			{
				miss++;
			}
		}

		public void RegisterTracker(SongData data)
		{
			data.GetScoreController().noteWasCutEvent += OnNoteCut;
			data.GetScoreController().noteWasMissedEvent += OnNoteMissed;
			data.GetScoreController().comboBreakingEventHappenedEvent += BreakCombo;
		}
	}
}
