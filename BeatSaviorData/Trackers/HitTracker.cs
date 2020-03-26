namespace BeatSaviorData.Trackers
{
	class HitTracker : ITracker
	{
		public int leftNoteHit, rightNoteHit, bombHit, miss, maxCombo;

		private int combo;

		public void EndOfSong(LevelCompletionResults results) {
			if (combo > maxCombo)
				maxCombo = combo;
		}

		public void OnNoteCut(BeatmapObjectSpawnController bosc, INoteController data, NoteCutInfo info)
		{
			if(info.allIsOK)
			{
				combo++;
				switch (data.noteData.noteType)
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
				if (data.noteData.noteType == NoteType.Bomb)
					bombHit++;
				else
					miss++;
			}
		}

		private void BreakComboWall()
		{
			if (combo > maxCombo)
				maxCombo = combo;
			combo = 0;
		}

		public void OnNoteMissed(BeatmapObjectSpawnController bosc, INoteController data)
		{
			if (data.noteData.noteType != NoteType.Bomb)
			{
				miss++;
			}
		}

		public void RegisterTracker(SongData data)
		{
			data.GetBOSC().noteWasCutEvent += OnNoteCut;
			data.GetBOSC().noteWasMissedEvent += OnNoteMissed;
			data.GetScoreController().comboBreakingEventHappenedEvent += BreakComboWall;
		}
	}
}
