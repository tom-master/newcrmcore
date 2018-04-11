using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NewCrmCore.Infrastructure.CommonTools.CustomHelper
{
	public class BingHelper
	{
		private static readonly String _bingUrlPrefix = "https://www.bing.com/";
		private static DateTime _dateTime;
		private static String _image;

		public static async Task<String> GetEverydayBackgroundImageAsync()
		{
			if ((DateTime.Now - _dateTime).Days < 1)
			{
				return _image;
			}

			var httpClient = new HttpClient();
			var httpResponseMessage = await httpClient.GetAsync($@"{_bingUrlPrefix}HPImageArchive.aspx?format=js&idx=0&n=1");
			var response = JsonConvert.DeserializeObject<dynamic>(await httpResponseMessage.Content.ReadAsStringAsync());
			var value = ((JObject)response).First.First.First;
			_image = $@"{_bingUrlPrefix}{((JProperty)value.ToList()[3]).Value}";
			_dateTime = DateTime.Now;
			return _image;
		}
	}
}
