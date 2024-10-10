/*using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaviorData
{
	class HTTPManager
	{
		public static readonly HttpClient client = new HttpClient();

		public async static void UploadPlayerStats(string json)
		{
			if (!SettingsMenu.instance.DisableBeatSaviorUpload)
			{
				try
				{
					await Upload(json, PrivateKeys.BeatSaviorPlayerDataUploadURL);
				}
				catch (Exception ex)
				{
					Logger.log.Error($"BSD : Error uploading player stats to BeatSavior");
					Logger.log.Debug(ex);
				}
			}
			else
			{
				Logger.log.Debug("Beat Savior Upload is disabled in the settings.");
			}
		}

		public async static void UploadSongJson(string json)
		{
			if (!SettingsMenu.instance.DisableBeatSaviorUpload)
			{
				try
				{
					await Upload(json, PrivateKeys.BeatSaviorSongUploadUrl);
				}
				catch (Exception ex)
				{
					Logger.log.Error($"BSD : Error uploading song results to BeatSavior");
					Logger.log.Debug(ex);
				}
			}
			else
			{
				Logger.log.Debug("Beat Savior Upload is disabled in the settings.");
			}

			if (SettingsMenu.instance.EnableCustomUrlUpload)
			{
				try
				{
					await Upload(json, SettingsMenu.instance.CustomUploadUrl);
				}
				catch (Exception ex)
				{
					Logger.log.Error($"BSD : Error uploading player stats to CustomURL");
					Logger.log.Debug(ex);
				}
			}
		}

		private async static Task Upload(string json, string url)
		{
			StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
			HttpResponseMessage success = await client.PostAsync(url, content);

			if (success.IsSuccessStatusCode)
				Logger.log.Info("BSD : Upload to BeatSavior successful !");
			else
				Logger.log.Info("BSD : Upload to BeatSavior failed.");
		}
	}
}
*/