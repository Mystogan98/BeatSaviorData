using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BeatSaviorData.Trackers;
using Newtonsoft.Json;
using System.Globalization;

namespace BeatSaviorData
{
    class FileManager
    {
        private const int MaxStatsFiles = 30;
        private const string FileDateFormat = "yyyy-MM-dd";
        private const string OldFileDateFormat = "dd-MM-yyyy";
        private static bool isANewFile = false;
        private static string fixedFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Beat Savior Data\\"), filePath = "";
        private static string PBScoreGraphFileName = "_PBScoreGraphs.bsd";

        private static List<ScoreGraphHolder> PBScoreGraphs;

        private static void FindOrCreateFile()
        {
            try
            {
                if (!Directory.Exists(fixedFilePath))
                {
                    Logger.log.Info("BSD : AppData directory created.");
                    Directory.CreateDirectory(fixedFilePath);
                }

                filePath = fixedFilePath + DateTime.Today.ToString(FileDateFormat) + ".bsd";
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Dispose();
                    isANewFile = true;
                    Logger.log.Info("BSD : New file \"" + Path.GetFileName(filePath) + "\" created successfully.");
                }

                DeleteOldRecords();
            }
            catch (Exception ex)
            {
                Logger.log.Error($"BSD : Error getting data files: {ex.Message}");
                Logger.log.Debug(ex);
            }
        }

        private static void DeleteOldRecords()
        {
            string[] files = Directory.GetFiles(fixedFilePath);
            SortedDictionary<DateTime, string> filesWithTime = new SortedDictionary<DateTime, string>();

            foreach (string s in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(s);
                if (fileName.StartsWith("_"))
                    continue;
                if (DateTime.TryParseExact(fileName, FileDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    //Logger.log.Debug($"Loaded '{fileName}' -> {date.ToString("d")}");
                    filesWithTime.Add(date, s);
                }
                else if (DateTime.TryParseExact(fileName, OldFileDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out  date))
                {
                    //Logger.log.Debug($"Loaded old format '{fileName}' -> {date.ToString("d")}");
                    filesWithTime.Add(date, s);
                }
                else
                    Logger.log.Debug($"Unexpected file: '{fileName}' ({s}) in Beat Savior Data folder.");
            }

            while (filesWithTime.Count > MaxStatsFiles)
            {
                KeyValuePair<DateTime, string> entry = filesWithTime.First();
                try
                {
                    File.Delete(entry.Value);
                    filesWithTime.Remove(entry.Key);
                    Logger.log.Info("BSD : Oldest file \"" + Path.GetFileName(entry.Value) + "\" deleted.");
                }
                catch (Exception ex)
                {
                    Logger.log.Error($"BSD : Error deleting old stats file '{Path.GetFileName(entry.Value)}': {ex.Message}");
                    Logger.log.Debug(ex);
                }
            }
        }

        public static void SavePlayerStats(string json)
        {
            try
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
            catch (Exception ex)
            {
                Logger.log.Error($"BSD : Error saving player stats: {ex.Message}");
                Logger.log.Debug(ex);
            }
        }

        public static void SaveSongStats(string json)
        {
            try
            {
                File.AppendAllText(filePath, "\n" + json);
                Logger.log.Info("BSD : Song stats saved successfully.");
            }
            catch (Exception ex)
            {
                Logger.log.Error($"BSD : Error saving song stats: {ex.Message}");
                Logger.log.Debug(ex);
            }
        }

        public static void SavePBScoreGraph(Dictionary<float, float> graph, int score, string songHash)
        {
            try
            {
                string filePath = fixedFilePath + PBScoreGraphFileName;
                ScoreGraphHolder tmpGraph;

                // FindOrCreateFile has been executed first, so the file path exists.
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Dispose();
                    PBScoreGraphs = new List<ScoreGraphHolder>();
                }
                else if (PBScoreGraphs == null)
                    PBScoreGraphs = JsonConvert.DeserializeObject<List<ScoreGraphHolder>>(File.ReadAllText(filePath));

                tmpGraph = PBScoreGraphs.Find((g) => g.songHash == songHash);

                if (tmpGraph == null || tmpGraph.score < score)
                {
                    if (tmpGraph != null)
                        PBScoreGraphs.Remove(tmpGraph);
                    PBScoreGraphs.Add(new ScoreGraphHolder(graph, score, songHash));
                    File.WriteAllText(filePath, JsonConvert.SerializeObject(PBScoreGraphs));
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error($"BSD : Error saving score graph: {ex.Message}");
                Logger.log.Debug(ex);
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
