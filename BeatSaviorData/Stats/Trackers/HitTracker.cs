namespace BeatSaviorData.Trackers
{
	class HitTracker : ITracker
	{
		public int leftNoteHit, rightNoteHit, bombHit, maxCombo, nbOfWallHit;
		// "miss" is the total number of errors, "missedNotes" is the number of actual miss
		// (couldn't change the name or else it would break the website)
		public int miss, missedNotes, badCuts, leftMiss, leftBadCuts, rightMiss, rightBadCuts;

		public void EndOfSong(LevelCompletionResults results, SongData data) {
			DataCollector collector = data.GetDataCollector();

			maxCombo = collector.maxCombo;
			bombHit = collector.bombHit;
			nbOfWallHit = collector.nbOfWallHit;

			foreach(Note n in collector.notes)
			{
				if (n.IsAMiss())
				{
					miss++;
					if (n.cutType == CutType.miss)
					{
						missedNotes++;
						if (n.noteType == BSD_NoteType.left)
							leftMiss++;
						else
							rightMiss++;
					}
					else if (n.cutType == CutType.badCut)
					{
						badCuts++;
						if (n.GetInfo().saberType == SaberType.SaberA)
							leftBadCuts++;
						else
							rightBadCuts++;
					}

				}
				else if (n.noteType == BSD_NoteType.left)
					leftNoteHit++;
				else if (n.noteType == BSD_NoteType.right)
					rightNoteHit++;
			}
		}
	}
}
