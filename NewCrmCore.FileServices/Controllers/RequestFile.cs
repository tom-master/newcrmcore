using System;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using NewCrmCore.Infrastructure;

namespace NewCrmCore.FileServices.Controllers
{
	internal sealed class RequestFile
	{
		public String Path { get; set; }

		public String Name { get; set; }

		public String FullPath { get; set; }

		public String Url { get; set; }

		public Image Image { get; set; }

		public FileType FileType { get; set; }

		public void ResetUrl()
		{
			if (String.IsNullOrEmpty(FullPath))
			{
				return;
			}

			Url = FullPath.Replace(Appsetting.FileStorage, "");
		}

		public FileType GetFileType(string fileType)
		{
			if (fileType.ToLower() == FileType.Wallpaper.ToString().ToLower())
			{
				return FileType.Wallpaper;
			}
			else if (fileType.ToLower() == FileType.Face.ToString().ToLower())
			{
				return FileType.Face;
			}
			else if (fileType.ToLower() == FileType.Icon.ToString().ToLower())
			{
				return FileType.Icon;
			}

			throw new Exception($@"{fileType}:未被识别为有效的上传类型");
		}

		public static string GetFileExtension(IFormFile file)
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

			return fileExtension.ToLower();
		}

		public void GetReducedImage(int width, int height)
		{
			// 源图宽度及高度 
			var imageFromWidth = Image.Width;
			var imageFromHeight = Image.Height;

			try
			{
				// 生成的缩略图实际宽度及高度.如果指定的高和宽比原图大，则返回原图；否则按照指定高宽生成图片
				if (width >= imageFromWidth && height >= imageFromHeight)
				{
					return;
				}
				else
				{
					Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(() => { return false; });

					//调用Image对象自带的GetThumbnailImage()进行图片缩略
					var reducedImage = Image.GetThumbnailImage(width, height, callb, IntPtr.Zero);

					//将图片以指定的格式保存到到指定的位置
					var newName = $@"small_{Guid.NewGuid().ToString().Replace("-", "")}.png";
					var newFileFullPath = $@"{Path}{newName}";
					FullPath = newFileFullPath;
					ResetUrl();
					reducedImage.Save(newFileFullPath, ImageFormat.Png);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.ToString());
			}
		}

		public String Calculate(Stream stream)
		{
			if (stream == null)
			{
				return "";
			}
			var md5 = new MD5CryptoServiceProvider();
			md5.ComputeHash(stream);
			var b = md5.Hash;
			md5.Clear();
			var sb = new StringBuilder(32);
			foreach (var t in b)
			{
				sb.Append(t.ToString("X2"));
			}

			return sb.ToString();
		}
	}

	internal enum FileType
	{
		Wallpaper = 1,
		Face = 2,
		Icon = 3
	}
}