using Microsoft.AspNetCore.Http;
using NewCrmCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NewCrmCore.FileServices.Controllers
{
    public abstract class ReqeustUpload
    {
        /// <summary>
        /// 完整本地路径
        /// </summary>
        public String FullPath { get; set; }

        /// <summary>
        /// 当前文件类型
        /// </summary>
        public FileType FileType { get; set; }

        /// <summary>
        /// 上传类型黑名单
        /// </summary>
        protected String[] ExtensionBlackList { get { return new[] { ".exe", ".bat", ".bat" }; } }

        /// <summary>
        /// 完整路径（不加文件名）
        /// </summary>
        /// <value></value>
        protected String Path { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        /// <value></value>
        protected String Name { get; set; }

        /// <summary>
        /// url访问链接
        /// </summary>
        /// <value></value>
        protected String Url { get; set; }

        /// <summary>
        /// 将本地路径转换成相对路径
        /// </summary>
        protected void ParseToRelativePath()
        {
            if (String.IsNullOrEmpty(FullPath))
            {
                return;
            }

            Url = FullPath.Replace(Appsetting.FileStorage, "");
        }

        /// <summary>
        /// 初始化必要参数
        /// </summary>
        public ReqeustUpload InitParameters(String accountId, String fileExtension)
        {
            var path = $@"{Appsetting.FileStorage}/files/{accountId}/{FileType.ToString().ToLower()}/";
            var fileName = $@"{Guid.NewGuid().ToString().Replace("-", "")}.{fileExtension}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Path = path;
            Name = fileName;
            FullPath = System.IO.Path.Combine(path, fileName);
            ParseToRelativePath();
            return this;
        }

        /// <summary>
        ///  检查文件扩展名
        /// </summary>
        public string CheckFileExtension(IFormFile file)
        {
            string fileExtension;
            if (file.FileName.StartsWith("__avatar"))
            {
                fileExtension = file.ContentType.Substring(file.ContentType.LastIndexOf("/", StringComparison.Ordinal) + 1);
                if (fileExtension == "jpeg")
                {
                    fileExtension = "jpg";
                }
            }
            else
            {
                fileExtension = file.FileName.Substring(file.FileName.LastIndexOf(".", StringComparison.Ordinal) + 1);
            }

            return !ExtensionBlackList.Any(d => d.ToLower() == fileExtension) ? fileExtension.ToLower() : "";
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        public Boolean CreateFile(IFormFile file)
        {
            return InternalCreateFile(file);
        }

        public abstract UploadResponseModel BuildUploadResponse();

        protected abstract Boolean InternalCreateFile(IFormFile file);
    }


    public enum FileType
    {
        [Description("壁纸")]
        Wallpaper = 1,

        [Description("头像")]
        Face = 2,

        [Description("图标")]
        Icon = 3,

        [Description("文件")]
        File = 4
    }
}
