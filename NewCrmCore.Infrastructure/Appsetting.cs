using System;
using Microsoft.Extensions.Configuration;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore;
namespace NewCrmCore.Infrastructure
{
    public class Appsetting
    {
        public static String Database
        {
            get
            {
                try
                {
                    var str = ConfigReader.GetHostVar("NewCrmDatabase");
                    return str ?? "";
                }
                catch (System.Exception)
                {
                    throw new EnvVariableGetFailException($@"获取数据库连接字符串失败");
                }
            }
        }

        public static String Redis
        {
            get
            {
                try
                {
                    var str = ConfigReader.GetHostVar("NewCrmRedis");
                    return str ?? "";
                }
                catch (System.Exception)
                {
                    throw new EnvVariableGetFailException($@"获取redis连接字符串失败");
                }
            }
        }

        public static String UploadUrl
        {
            get
            {
                try
                {
                    var str = ConfigReader.GetHostVar("NewCrmFileUploadUrl");
                    return str ?? "";
                }
                catch (System.Exception)
                {
                    throw new EnvVariableGetFailException($@"获取上传Url字符串失败");
                }
            }
        }

        public static String FileUrl
        {
            get
            {
                try
                {
                    var str = ConfigReader.GetHostVar("NewCrmFileUrl");
                    return str ?? "";
                }
                catch (System.Exception)
                {
                    throw new EnvVariableGetFailException($@"获取文件Url字符串失败");
                }
            }
        }

        public static String FileStorage
        {
            get
            {
                try
                {
                    var str = ConfigReader.GetHostVar("NewCrmFileStorage");
                    return str ?? "";
                }
                catch (System.Exception)
                {
                    throw new EnvVariableGetFailException($@"获取文件存储路径失败");
                }
            }
        }

        public static String Skin
        {
            get
            {
                try
                {
                    var str = ConfigReader.GetHostVar("NewCrmSkin");
                    return str ?? "";
                }
                catch (System.Exception)
                {
                    throw new EnvVariableGetFailException($@"获取皮肤失败");
                }
            }
        }

        public static String SecurityKey
        {
            get
            {
                try
                {
                    var str = ConfigReader.GetHostVar("SecurityKey");
                    return str ?? "";
                }
                catch (System.Exception)
                {
                    throw new EnvVariableGetFailException($@"获取SecurityKey失败");
                }
            }
        }
        public static String Domain
        {
            get
            {
                try
                {
                    var str = ConfigReader.GetHostVar("Domain");
                    return str ?? "";
                }
                catch (System.Exception)
                {
                    throw new EnvVariableGetFailException($@"获取Domain失败");
                }
            }
        }

    }
}
