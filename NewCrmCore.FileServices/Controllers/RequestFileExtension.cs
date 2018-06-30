using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NewCrmCore.FileServices.Controllers
{

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
}
