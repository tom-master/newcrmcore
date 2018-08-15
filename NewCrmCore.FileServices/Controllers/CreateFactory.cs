﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewCrmCore.FileServices.Controllers
{
    public class CreateFactory
    {
        public static ReqeustUpload BuildRequestEntity(String uploadType)
        {
            uploadType = uploadType.ToLower();

            if (uploadType == FileType.Wallpaper.ToString().ToLower())
            {
                return new RequestImage() { FileType = FileType.Wallpaper };
            }
            if (uploadType == FileType.Face.ToString().ToLower())
            {
                return new RequestImage() { FileType = FileType.Face };
            }
            if (uploadType == FileType.Icon.ToString().ToLower())
            {
                return new RequestImage() { FileType = FileType.Icon };
            }
            if (uploadType == FileType.File.ToString().ToLower())
            {
                return new RequestFile { FileType = FileType.File };
            }


            throw new Exception($@"{uploadType}:未被识别为有效的上传类型");
        }
    }
}
