using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BeatSaviorData
{
	class FileManager
	{
		private static bool isANewFile = false;
		private static string fixedFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Beat Savior Data\\"), filePath = "";

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

			if (files.Length > 30)
			{
				string oldestPath = "";
				DateTime oldestTime = DateTime.MaxValue, tmpdt;

				foreach(string s in files)
				{
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
	}
}
