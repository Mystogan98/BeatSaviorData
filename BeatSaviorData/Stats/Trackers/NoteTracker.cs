using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaviorData.Trackers
{
	class NoteTracker : ITracker
	{
		public List<Note> notes;

		public void EndOfSong(LevelCompletionResults results, SongData data) {
			notes = data.GetDataCollector().notes;
		}
	}
}
