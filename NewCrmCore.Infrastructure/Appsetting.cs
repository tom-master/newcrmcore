using System;

namespace NewCrmCore.Infrastructure
{
	public class Appsetting
	{
		public static String Database
		{
			get
			{
				var str = Environment.GetEnvironmentVariable("NewCrmDatabase");
				return str ?? "";
			}
		}

		public static String Redis
		{
			get
			{
				var str = Environment.GetEnvironmentVariable("NewCrmRedis");
				return str ?? "";
			}
		}

		public static String Mongodb
		{
			get
			{
				var str = Environment.GetEnvironmentVariable("NewCrmMongodb");
				return str ?? "";
			}
		}

		public static String UploadUrl
		{
			get
			{
				var str = Environment.GetEnvironmentVariable("NewCrmFileUploadUrl");
				return str ?? "";
			}
		}

		public static String FileUrl
		{
			get
			{
				var str = Environment.GetEnvironmentVariable("NewCrmFileUrl");
				return str ?? "";
			}
		}
	}
}
