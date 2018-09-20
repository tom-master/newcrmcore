using System;
using NewLibCore;

namespace NewCrmCore.Infrastructure
{
	public class Appsetting
	{
		public static String Database
		{
			get
			{
				var str =Host.GetHostVar("NewCrmDatabase");
				return str ?? "";
			}
		}

		public static String Redis
		{
			get
			{
				var str = Host.GetHostVar("NewCrmRedis");
				return str ?? "";
			}
		}

		public static String Mongodb
		{
			get
			{
				var str = Host.GetHostVar("NewCrmMongodb");
				return str ?? "";
			}
		}

		public static String UploadUrl
		{
			get
			{
				var str = Host.GetHostVar("NewCrmFileUploadUrl");
				return str ?? "";
			}
		}

		public static String FileUrl
		{
			get
			{
				var str = Host.GetHostVar("NewCrmFileUrl");
				return str ?? "";
			}
		}

		public static String FileStorage
		{
			get
			{
				var str = Host.GetHostVar("NewCrmFileStorage");
				return str ?? "";
			}
		}

		public static String Skin
		{
			get
			{
				var str = Host.GetHostVar("NewCrmSkin");
				return str ?? "";
			}
		}
	}
}
