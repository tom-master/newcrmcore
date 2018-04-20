using System;
using System.Collections.Generic;
using System.Text;

namespace NewCrmCore.Dto
{
	public class Settings
	{
		public Database Database { get; set; }

		public String Skin { get; set; }

		public String FileUrl { get; set; }

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
