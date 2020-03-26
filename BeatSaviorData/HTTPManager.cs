using System.Net.Http;
using System.Text;

namespace BeatSaviorData
{
	class HTTPManager
	{
		public static readonly HttpClient client = new HttpClient();

		public async static void uploadJson(string json)
		{
			//client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
			HttpResponseMessage success = await client.PostAsync(PrivateKeys.BeatSaviorUploadUrl, content);

			if (success.IsSuccessStatusCode)
				Logger.log.Info("BSD : Upload succeeded !");
			else
				Logger.log.Info("BSD : Upload failed.");
		}
	}
}
