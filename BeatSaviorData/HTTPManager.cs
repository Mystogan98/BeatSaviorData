using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaviorData
{
	class HTTPManager
	{
		public static readonly HttpClient client = new HttpClient();

		public static bool uploadJson(string json)
		{
			if(client.PostAsync(PrivateKeys.BeatSaviorUploadUrl, new StringContent(json)).Result.IsSuccessStatusCode)
				return true;
			return false;
		}
	}
}
