using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NewCrmCore.FileServices.Controllers
{
	internal class RequestFile
	{
		internal static String[] ExtensionBlackList
		{
			get { return new[] { ".exe", ".bat", ".bat" }; }
		}

		internal String Path { get; set; }

		internal String Name { get; set; }

		internal String FullPath { get; set; }

		internal String Url { get; set; }

		internal FileType FileType { get; set; }

		internal void ParseToRelativePath()
		{
			if (String.IsNullOrEmpty(FullPath))
			{
				return;
			}

			Url = FullPath.Replace(Appsetting.FileStorage, "");
		}

		internal static FileType GetFileType(string fileType)
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

		internal static string GetFileExtension(IFormFile file)
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

		internal static RequestFile BuilderRequestFile(String accountId, String fileType, String fileExtension)
		{
			var internalFileType = GetFileType(fileType);
			var requestFile = new RequestFile();
			if (internalFileType == FileType.Icon || internalFileType == FileType.Wallpaper || internalFileType == FileType.Face)
			{
				requestFile = new RequestImage();
			}

			var path = $@"{Appsetting.FileStorage}/{accountId}/{internalFileType}/";
			var fileName = $@"{Guid.NewGuid().ToString().Replace("-", "")}.{fileExtension}";
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			requestFile.Path = path;
			requestFile.Name = fileName;
			requestFile.FullPath = System.IO.Path.Combine(path, fileName);
			requestFile.FileType = internalFileType;
			requestFile.ParseToRelativePath();
			return requestFile;
		}

		internal virtual Boolean CreatePhysicalFile(IFormFile file)
		{
			var stream = file.OpenReadStream();
			var bytes = new byte[stream.Length];

			using (var fileStream = new FileStream(FullPath, FileMode.Create, FileAccess.Write))
			{
				stream.Read(bytes, 0, bytes.Length);
				fileStream.Write(bytes, 0, bytes.Length);
			}

			return File.Exists(FullPath);
		}

		internal virtual dynamic GetResult()
		{
			throw new NotImplementedException();
		}
	}

	internal sealed class RequestImage : RequestFile
	{
		private Image Image { get; set; }

		internal Int32 Width { get; set; }

		internal Int32 Height { get; set; }

		internal override bool CreatePhysicalFile(IFormFile file)
		{
			try
			{
				if (!base.CreatePhysicalFile(file))
				{
					return false;
				}

				using (Image = Image.FromFile(FullPath))
				{
					if (FileType == FileType.Icon)
					{
						CreateThumbnail(49, 49);
					}
					else if (FileType == FileType.Face)
					{
						CreateThumbnail(20, 20);
					}
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		
		private void CreateThumbnail(int width, int height)
		{
			// 源图宽度及高度 
			var imageFromWidth = Image.Width;
			var imageFromHeight = Image.Height;
			Width = width;
			Height = height;
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
					var newFileFullPath = System.IO.Path.Combine(Path, newName);
					FullPath = newFileFullPath;
					ParseToRelativePath();
					reducedImage.Save(newFileFullPath, ImageFormat.Png);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.ToString());
			}
		}

		internal override dynamic GetResult()
		{
			return new
			{
				IsSuccess = true,
				Width,
				Height,
				Title = "",
				Url,
				Md5 = this.GetMD5(),
			};
		}
	}

	internal static class RequestFileExtension
	{
		public static String GetMD5(this RequestFile requestFile)
		{
			var cryptoServiceProvider = new MD5CryptoServiceProvider();
			cryptoServiceProvider.ComputeHash(File.ReadAllBytes(requestFile.FullPath));
			var sb = new StringBuilder(32);
			foreach (var t in cryptoServiceProvider.Hash)
			{
				sb.Append(t.ToString("X2"));
			}
			cryptoServiceProvider.Clear();
			return sb.ToString();
		}
	}

	internal enum FileType
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