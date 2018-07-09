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
		public String FullPath { get; set; }

		protected virtual String[] ExtensionBlackList
		{
			get { return new[] { ".exe", ".bat", ".bat" }; }
		}

		protected String Path { get; set; }

		protected String Name { get; set; }

		protected String Url { get; set; }

		public FileType FileType { get; set; }

		public abstract dynamic GetResult();

		protected abstract Boolean InternalCreateFile(IFormFile file);

		protected void ParseToRelativePath()
		{
			if (String.IsNullOrEmpty(FullPath))
			{
				return;
			}

			Url = FullPath.Replace(Appsetting.FileStorage, "");
		}

		protected String GetMD5()
		{
			var md5 = new MD5CryptoServiceProvider();
			md5.ComputeHash(File.Open(FullPath, FileMode.Open));
			var b = md5.Hash;
			md5.Clear();
			var sb = new StringBuilder(32);
			foreach (var t in b)
			{
				sb.Append(t.ToString("X2"));
			}

			return sb.ToString();
		}

		public ReqeustUpload BuilderRequestFile(String accountId, String fileExtension)
		{
			var path = $@"{Appsetting.FileStorage}/{accountId}/{FileType}/";
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

		public Boolean CreateFile(IFormFile file)
		{
			return InternalCreateFile(file);
		}
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
