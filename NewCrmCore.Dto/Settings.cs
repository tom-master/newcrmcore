using System;
using NewLibCore.Security;

namespace NewCrmCore.Dto
{
	public class Settings
	{
		private String _fileUrl;


		public Database Database { get; set; }

		public String Skin { get; set; }

		public String FileUrl
		{
			get
			{
				if (_fileUrl == null)
				{
					return "";
				}

				return SensitiveDataSafetyProvider.Decrypt(_fileUrl);
			}
			set
			{
				_fileUrl = value;
			}
		}

		public Redis Redis { get; set; }

		public String Mongodb { get; set; }

		public String UploadUrl { get; set; }
	}

	public class Database
	{
		public String Name { get; set; }

		public String Value { get; set; }
	}

	public class Redis
	{
		public Int32 DatabaseIndex { get; set; }

		public String Connection { get; set; }

		public String Prefix { get; set; }
	}

}
