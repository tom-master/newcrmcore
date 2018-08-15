using System;
namespace NewCrmCore.FileServices
{
    public class UploadResponseModel
    {
        public UploadResponseModel()
        {
            IsSuccess = false;
        }

        public Boolean IsSuccess { get; set; }

        public Int32 Width { get; set; }

        public Int32 Height { get; set; }

        public String Title { get; set; }

        public String Url { get; set; }

        public String Md5 { get; set; }

        public String Message { get; set; }
    }
}