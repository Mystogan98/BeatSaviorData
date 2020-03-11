using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaviorData.Trackers
{
	class HitTracker : ITracker
	{
		public int leftNoteHit, rightNoteHit, bombHit, miss;

		public void EndOfSong() { }

		public void OnNoteCut(BeatmapObjectSpawnController bosc, INoteController data, NoteCutInfo info)
		{
			if(info.allIsOK)
			{
				switch (data.noteData.noteType)
				{
					case NoteType.NoteA:
						leftNoteHit++;
						break;
					case NoteType.NoteB:
						rightNoteHit++;
						break;
					case NoteType.Bomb:
						bombHit++;
						break;
				}
			}
		}

		public void OnNoteMissed(BeatmapObjectSpawnController bosc, INoteController data)
		{
			if(data.noteData.noteType != NoteType.Bomb)
				miss++;
		}
	}
}
