﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.Linq;
using System.Threading.Tasks;

namespace NewCrmCore.FileServices.Controllers
{
	public class RequestImage : ReqeustUpload
	{
		private Image Image { get; set; }

		internal Int32 Width { get; set; }

		internal Int32 Height { get; set; }

		protected override bool InternalCreateFile(IFormFile file)
		{
			try
			{
				using (Image = Image.FromStream(file.OpenReadStream()))
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

		public override dynamic GetResult()
		{
			throw new NotImplementedException();
		}
	}
}
