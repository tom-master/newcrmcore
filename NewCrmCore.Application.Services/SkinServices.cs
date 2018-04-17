using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NewCrmCore.Application.Services.Interface;
using NewCrmCore.Domain.Services;
using NewCrmCore.Domain.Services.Interface;
using NewLibCore.Validate;
using static NewCrmCore.Infrastructure.CommonTools.CacheKey;

namespace NewCrmCore.Application.Services
{
	public class SkinServices: ISkinServices
	{
		private readonly ISkinContext _skinContext;

		public SkinServices(ISkinContext skinContext)
		{
			_skinContext = skinContext;
		}

		public async Task<IDictionary<String, dynamic>> GetAllSkinAsync(String skinPath)
		{
			new Parameter().Validate(skinPath);

			return await Task.Run(() =>
			{
				IDictionary<String, dynamic> dataDictionary = new Dictionary<String, dynamic>();
				Directory.GetFiles(skinPath, "*.css").ToList().ForEach(path =>
				{
					var fileName = Get(path, x => x.LastIndexOf(@"\", StringComparison.OrdinalIgnoreCase) + 1).Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0];
					dataDictionary.Add(fileName, new
					{
						cssPath = path.Substring(path.LastIndexOf("Content", StringComparison.OrdinalIgnoreCase) - 1).Replace(@"\", "/"),
						imgPath = GetLocalImagePath(fileName, skinPath)
					});
				});

				return dataDictionary;
			});
		}

		public async Task ModifySkinAsync(Int32 accountId, String newSkin)
		{
			new Parameter().Validate(accountId).Validate(newSkin);
			await _skinContext.ModifySkinAsync(accountId, newSkin);
			CacheHelper.RemoveOldKeyWhenModify(new ConfigCacheKey(accountId));
		}

		#region private method

		private String GetLocalImagePath(String fileName, String fullPath)
		{
			new Parameter().Validate(fileName).Validate(fullPath);

			var dic = Directory.GetFiles(fullPath, "preview.png", SearchOption.AllDirectories).ToList();
			foreach (var dicItem in from dicItem in dic let regex = new Regex(fileName) where regex.IsMatch(dicItem) select dicItem)
			{
				return dicItem.Substring(dicItem.LastIndexOf("Content", StringComparison.OrdinalIgnoreCase) - 1).Replace(@"\", "/");
			}

			return "";
		}

		private String Get(String path, Func<String, Int32> filterFunc)
		{
			return path.Substring(filterFunc(path));
		}

		#endregion
	}
}
