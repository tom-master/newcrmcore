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

		public static String FileStorage
		{
			get
			{
				var str = GetUserVar("NewCrmCoreFileStorage");
				return str ?? "";
			}
		}


		private static String GetUserVar(String varKey)
		{
			var v1 = Environment.GetEnvironmentVariable(varKey);

			if (v1 != null)
			{
				return v1;
			}

			var v2 = Environment.GetEnvironmentVariable(varKey, EnvironmentVariableTarget.Machine);
			if (v2 != null)
			{
				return v2;
			}

			var v3 = Environment.GetEnvironmentVariable(varKey, EnvironmentVariableTarget.Process);
			if (v3 != null)
			{
				return v3;
			}

			var v4 = Environment.GetEnvironmentVariable(varKey, EnvironmentVariableTarget.User);
			if (v4 != null)
			{
				return v4;
			}

			return "";
		}
	}
}
