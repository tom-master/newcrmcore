using System;

namespace NewCrmCore.Infrastructure
{
	public class Appsetting
	{
		public static String Database
		{
			get
			{
				var str = GetVar("NewCrmDatabase");
				return str ?? "";
			}
		}

		public static String Redis
		{
			get
			{
				var str = GetVar("NewCrmRedis");
				return str ?? "";
			}
		}

		public static String Mongodb
		{
			get
			{
				var str = GetVar("NewCrmMongodb");
				return str ?? "";
			}
		}

		public static String UploadUrl
		{
			get
			{
				var str = GetVar("NewCrmFileUploadUrl");
				return str ?? "";
			}
		}

		public static String FileUrl
		{
			get
			{
				var str = GetVar("NewCrmFileUrl");
				return str ?? "";
			}
		}

		public static String FileStorage
		{
			get
			{
				var str = GetVar("NewCrmCoreFileStorage");
				return str ?? "";
			}
		}

		public static String Skin
		{
			get
			{
				var str = GetVar("NewCrmSkin");
				return str ?? "";
			}
		}


		private static String GetVar(String varKey)
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
