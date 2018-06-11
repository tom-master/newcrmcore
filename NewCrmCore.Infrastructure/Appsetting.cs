using System;

namespace NewCrmCore.Infrastructure
{
	public class Appsetting
	{
		public static String Database
		{
			get
			{
				var str = GetUserVar("NewCrmDatabase");
				return str ?? "";
			}
		}

		public static String Redis
		{
			get
			{
				var str = GetUserVar("NewCrmRedis");
				return str ?? "";
			}
		}

		public static String Mongodb
		{
			get
			{
				var str = GetUserVar("NewCrmMongodb");
				return str ?? "";
			}
		}

		public static String UploadUrl
		{
			get
			{
				var str = GetUserVar("NewCrmFileUploadUrl");
				return str ?? "";
			}
		}

		public static String FileUrl
		{
			get
			{
				var str = GetUserVar("NewCrmFileUrl");
				return str ?? "";
			}
		}


		private static String GetUserVar(String varKey)
		{
			return Environment.GetEnvironmentVariable(varKey, EnvironmentVariableTarget.User);
		}
	}
}
