using Newtonsoft.Json;
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
			if(!SettingsMenu.instance.DisableBeatSaviorUpload)
				await Upload(json, PrivateKeys.BeatSaviorPlayerDataUploadURL);
		}

		public async static void UploadSongJson(string json)
		{
			if (!SettingsMenu.instance.DisableBeatSaviorUpload)
				await Upload(json, PrivateKeys.BeatSaviorSongUploadUrl);
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
