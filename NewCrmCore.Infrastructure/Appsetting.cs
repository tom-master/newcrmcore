using System;
using Microsoft.Extensions.Configuration;
using NewCrmCore.Infrastructure.CommonTools;
using NewLibCore;
namespace NewCrmCore.Infrastructure
{
    public class Appsetting
    {
        public Appsetting()
        {

        }

        public static String Database
        {
            get
            {
                try
                {
                    var str = Host.GetHostVar("NewCrmDatabase");
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
                    var str = Host.GetHostVar("NewCrmRedis");
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
                    var str = Host.GetHostVar("NewCrmFileUploadUrl");
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
                    var str = Host.GetHostVar("NewCrmFileUrl");
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
                    var str = Host.GetHostVar("NewCrmFileStorage");
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
                    var str = Host.GetHostVar("NewCrmSkin");
                    return str ?? "";
                }
                catch (System.Exception)
                {
                    throw new EnvVariableGetFailException($@"获取皮肤失败");
                }
            }
        }
    }
}
