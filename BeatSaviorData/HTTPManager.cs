using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaviorData
{
	class HTTPManager
	{
		public static readonly HttpClient client = new HttpClient();

		public static bool uploadJson(string json)
		{
			//client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

			if (client.PostAsync(PrivateKeys.BeatSaviorUploadUrl, content).Result.IsSuccessStatusCode)
				return true;
			return false;
		}
	}
}
