namespace BeatSaviorData.Trackers
{
	class HitTracker : ITracker
	{
		public int leftNoteHit, rightNoteHit, bombHit, miss, maxCombo, nbOfWallHit;

		public void EndOfSong(LevelCompletionResults results, SongData data) {
			DataCollector collector = data.GetDataCollector();

			maxCombo = collector.maxCombo;
			bombHit = collector.bombHit;
			nbOfWallHit = collector.nbOfWallHit;

			foreach(Note n in collector.notes)
			{
				if (n.IsAMiss())
					miss++;
				else if (n.noteType == BSD_NoteType.left)
					leftNoteHit++;
				else if (n.noteType == BSD_NoteType.right)
					rightNoteHit++;
			}
		}
	}
}
