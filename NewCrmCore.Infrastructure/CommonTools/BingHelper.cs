using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NewCrmCore.Infrastructure.CommonTools
{
    public class BingHelper
    {
        private static readonly String _bingUrlPrefix = "https://www.bing.com/";
        private static DateTime _dateTime;
        private static String _image;

        /// <summary>
        /// 获取每日背景图
        /// </summary>
        /// <returns></returns>
        public static async Task<String> GetEverydayBackgroundImageAsync()
        {
            if ((DateTime.Now - _dateTime).Days < 1)
            {
                return _image;
            }

            var httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.GetAsync($@"{_bingUrlPrefix}HPImageArchive.aspx?format=js&idx=0&n=1");
            var response = JsonConvert.DeserializeObject<BingBackgroundImageModel>(await httpResponseMessage.Content.ReadAsStringAsync());
            _image = $@"{_bingUrlPrefix}{response.Images[0].Url}";
            _dateTime = DateTime.Now;
            return _image;
        }
    }

    internal class BingBackgroundImageModel
    {
        public ImageInfo[] Images { get; set; }

        public ToolsTips ToolsTips { get; set; }
    }

    internal class ToolsTips
    {

        public String Loading { get; set; }

        public String Previous { get; set; }

        public String Next { get; set; }

        public String Walle { get; set; }

        public String Walls { get; set; }
    }

    internal class ImageInfo
    {
        public String StartTime { get; set; }

        public String FullStartTime { get; set; }

        public String EndDate { get; set; }

        public String Url { get; set; }

        public String UrlBase { get; set; }

        public String Copyright { get; set; }

        public String Copyrightlink { get; set; }

        public String Title { get; set; }

        public String Quiz { get; set; }

        public String Wp { get; set; }

        public String Hsh { get; set; }

        public String Drk { get; set; }

        public String Top { get; set; }

        public String Bot { get; set; }
    }

}
