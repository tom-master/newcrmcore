using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace NewCrmCore.FileServices.Controllers
{
	[Route("api/filestorage")]
	public class FileController : Controller
	{
		[Route("upload"), HttpPost, HttpOptions]
		public IActionResult Upload()
		{
			var responses = new List<dynamic>();

			try
			{
				var request = Request.Form;
				var accountId = request["accountId"];

				if (String.IsNullOrEmpty(accountId))
				{
					responses.Add(new
					{
						IsSuccess = false,
						Message = $@"{nameof(accountId)}参数验证失败"
					});

					return Json(responses);
				}

				var files = request.Files;
				if (files.Count == 0)
				{
					responses.Add(new
					{
						IsSuccess = false,
						Message = "未能获取到上传的文件"
					});

					return Json(responses);
				}

				var uploadType = request["uploadtype"];
				if (String.IsNullOrEmpty(uploadType))
				{
					responses.Add(new
					{
						IsSuccess = false,
						Message = $@"{nameof(uploadType)}参数验证失败"
					});

					return Json(responses);
				}

				if (String.IsNullOrEmpty($@"C:/files"))
				{
					responses.Add(new
					{
						IsSuccess = false,
						Message = $@"未能找到服务器配置的存储路径"
					});
					return Json(responses);
				}


				for (var i = 0; i < files.Count; i++)
				{
					var file = files[i];

					var requestUpload = CreateFactory.Create(uploadType);
					var fileExtension = requestUpload.CheckFileExtension(file);
					if (String.IsNullOrEmpty(fileExtension))
					{
						responses.Add(new
						{
							IsSuccess = false,
							Message = $@"后缀名为：{fileExtension}的文件禁止上传"
						});

						continue;
					}

					var requestFile = requestUpload.BuilderRequestFile(accountId, fileExtension);
					if (!requestFile.CreateFile(file))
					{
						responses.Add(new
						{
							IsSuccess = false,
							Message = $@"文件上传失败"
						});

						continue;
					}

					responses.Add(requestFile.GetResult());
				}
			}
			catch (Exception ex)
			{
				responses.Add(new
				{
					IsSuccess = false,
					Message = ex.ToString()
				});
			}

			return Json(responses);
		}
	}
}
