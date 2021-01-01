using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using NewLibCore;
namespace NewCrmCore.FileServices.Controllers
{
    public class RequestFile : ReqeustUpload
    {
        private String _fileName;

        protected override bool InternalCreateFile(IFormFile file)
        {
            var stream = file.OpenReadStream();
            var bytes = new byte[stream.Length];
            _fileName = file.Name;

            using (var fileStream = new FileStream(FullPath, FileMode.Create, FileAccess.Write))
            {
                stream.Read(bytes, 0, bytes.Length);
                fileStream.Write(bytes, 0, bytes.Length);
            }
            ParseToRelativePath();
            return File.Exists(FullPath);
        }

        public override UploadResponseModel BuildUploadResponse()
        {
            return new UploadResponseModel
            {
                IsSuccess = true,
                Title = _fileName,
                Url = Url,
                Md5 = MD5.Get(File.Open(FullPath, FileMode.Open))
            };
        }
    }
}