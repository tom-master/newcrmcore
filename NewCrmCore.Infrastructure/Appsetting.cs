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
                try
                {
                    var str = Host.GetHostVar("NewCrmDatabase");
                    return str ?? "";
                }
                catch (System.Exception)
                {

                    throw;
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
                    throw;
                }
            }
        }

        public static String Mongodb
        {
            get
            {
                try
                {
                    var str = Host.GetHostVar("NewCrmMongodb");
                    return str ?? "";
                }
                catch (System.Exception)
                {
                    throw;
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
                    throw;
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
                    throw;
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
                    throw;
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
                    throw;
                }
            }
        }
    }
}
