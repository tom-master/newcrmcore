using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace NewCrmCore.FileServices.Controllers
{
	public class RequestFile : ReqeustUpload
	{
		protected override bool InternalCreateFile(IFormFile file)
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
 
		public override dynamic GetResult()
		{
			throw new NotImplementedException();
		}
	}
}