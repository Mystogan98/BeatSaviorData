using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaviorData.Trackers
{
	class ScoreGraphTracker : ITracker
	{
		public Dictionary<float, float> graph = new Dictionary<float, float>();

		public void EndOfSong(LevelCompletionResults results, SongData data)
		{
			Dictionary<float, float> rawGraph = new Dictionary<float, float>();
			Queue<float> lastGraphNodes = new Queue<float>();
			int actualScore = 0, maxScore = 0, multiplier = 1, multiplierProgress = 0, lastBeat = 0;

			foreach (Note n in data.GetDataCollector().notes)
			{
				actualScore += n.GetTotalScore();

				if (multiplier < 8)
				{
					multiplierProgress++;
					if (multiplierProgress == multiplier * 2)
					{
						multiplier *= 2;
						multiplierProgress = 0;
					}
				}

				maxScore += 115 * multiplier;

				if (Math.Truncate(n.time) > lastBeat)
				{
					rawGraph.Add((float) Math.Truncate(n.time), (float) actualScore / (float) maxScore);
					lastBeat = (int) Math.Truncate(n.time);
				}
			}

			foreach(KeyValuePair<float, float> entry in rawGraph) {

				lastGraphNodes.Enqueue(entry.Value);

				if (lastGraphNodes.Count > 5)
					lastGraphNodes.Dequeue();

				graph.Add(entry.Key, lastGraphNodes.Average());
			}
		}
	}
}
