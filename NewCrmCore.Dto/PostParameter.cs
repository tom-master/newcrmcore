using System;

namespace NewCrmCore.Dto
{
    public class ModifyStar
    {
        public Int32 AppId { get; set; }

        public Int32 StarCount { get; set; }
    }

    public class Install
    {
        public Int32 AppId { get; set; }

        public Int32 DeskNum { get; set; }
    }

    public class ModifyIconForApp
    {
        public Int32 AppId { get; set; }
        public String NewIcon { get; set; }
    }

    public class ModifyIconForMember
    {
        public Int32 MemberId { get; set; }
        public String NewIcon { get; set; }
    }

    public class CreateFolder
    {
        public String FolderName { get; set; }

        public String FolderImg { get; set; }

        public Int32 DeskId { get; set; }
    }

    public class MemberMove
    {
        public String MoveType { get; set; }
        public Int32 MemberId { get; set; }
        public Int32 From { get; set; }
        public Int32 To { get; set; }
    }

    public class ModifyDockPosition
    {
        public String Pos { get; set; }

        public Int32 DeskNum { get; set; }
    }

    public class ModifyFolderInfo
    {
        public String Name { get; set; }
        public String Icon { get; set; }
        public Int32 MemberId { get; set; }
    }

    public class UserLogin
    {
        public String UserName { get; set; }

        public String Password { get; set; }

        public Boolean Remember { get; set; }
    }
}