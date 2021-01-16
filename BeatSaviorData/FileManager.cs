using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BeatSaviorData.Trackers;
using Newtonsoft.Json;

namespace BeatSaviorData
{
	class FileManager
	{
		private static bool isANewFile = false;
		private static string fixedFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Beat Savior Data\\"), filePath = "";
		private static string PBScoreGraphFileName = "_PBScoreGraphs.bsd";

		private static List<ScoreGraphHolder> PBScoreGraphs;

		private static void FindOrCreateFile()
		{
			if (!Directory.Exists(fixedFilePath))
			{
				Logger.log.Info("BSD : AppData directory created.");
				Directory.CreateDirectory(fixedFilePath);
			}

			filePath = fixedFilePath + DateTime.Today.ToString("dd-MM-yyyy") + ".bsd";
			if(!File.Exists(filePath))
			{
				Logger.log.Info("BSD : New file \"" + Path.GetFileName(filePath) + "\" created successfully.");
				File.Create(filePath).Dispose();
				isANewFile = true;
			}

			DeleteOldRecords();
		}

		private static void DeleteOldRecords()
		{
			string[] files = Directory.GetFiles(fixedFilePath);

			if (files.Length > 31)	// 30 data files + PB graph file
			{
				string oldestPath = "";
				DateTime oldestTime = DateTime.MaxValue, tmpdt;

				foreach(string s in files)
				{
					if (s == PBScoreGraphFileName)
						continue;

					if (string.IsNullOrEmpty(oldestPath))
					{
						oldestPath = s;
						oldestTime = DateTime.Parse(Path.GetFileNameWithoutExtension(s));
					}
					else
					{
						tmpdt = DateTime.Parse(Path.GetFileNameWithoutExtension(s));
						if(tmpdt < oldestTime)
						{
							oldestTime = tmpdt;
							oldestPath = s;
						}
					}
				}

				Logger.log.Info("BSD : Oldest file \"" + Path.GetFileName(filePath) + "\" deleted.");
				File.Delete(oldestPath);
			}
		}

		public static void SavePlayerStats(string json)
		{
			FindOrCreateFile();

			if (!isANewFile)
			{
				Logger.log.Info("BSD : File already exists, player stats ignored.");
				return;
			}

			File.AppendAllText(filePath, json);
			Logger.log.Info("BSD : Player stats saved successfully.");
		}

		public static void SaveSongStats(string json)
		{
			File.AppendAllText(filePath, "\n" + json);
			Logger.log.Info("BSD : Song stats saved successfully.");
		}

		public static void SavePBScoreGraph(Dictionary<float, float> graph, int score, string songHash)
		{
			string filePath = fixedFilePath + PBScoreGraphFileName;
			ScoreGraphHolder tmpGraph;

			// FindOrCreateFile has been executed first, so the file path exists.
			if (!File.Exists(filePath)) {
				File.Create(filePath).Dispose();
				PBScoreGraphs = new List<ScoreGraphHolder>();
			} else if (PBScoreGraphs == null)
				PBScoreGraphs = JsonConvert.DeserializeObject<List<ScoreGraphHolder>>(File.ReadAllText(filePath));

			tmpGraph = PBScoreGraphs.Find((g) => g.songHash == songHash);

			if(tmpGraph == null || tmpGraph.score < score)
			{
				if (tmpGraph != null)
					PBScoreGraphs.Remove(tmpGraph);
				PBScoreGraphs.Add(new ScoreGraphHolder(graph, score, songHash));
				File.WriteAllText(filePath, JsonConvert.SerializeObject(PBScoreGraphs));
			}
		}

		private class ScoreGraphHolder
		{
			public string songHash;
			public int score;
			public Dictionary<float, float> graph;

			public ScoreGraphHolder(Dictionary<float, float> graph, int score, string songHash)
			{
				this.songHash = songHash;
				this.score = score;
				this.graph = graph;
			}
		}
	}
}
