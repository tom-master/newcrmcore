using System;
using NewLibCore.Data.SQL.Mapper;
using NewLibCore.Data.SQL.Mapper.Validate;

namespace NewCrmCore.Domain.Entitys.System
{ 
    [TableName("newcrm_app_notify")]
    public partial class Notify : EntityBase
    {
        [Required, InputRange(4, 10)]
        public String Title { get; private set; }

        [Required, InputRange(1, 20)]
        public String Content { get; private set; }

        [DefaultValue(typeof(Boolean))]
        public Boolean IsNotify { get; private set; }

        [DefaultValue(typeof(Boolean))]
        public Boolean IsRead { get; private set; }

        [Required]
        public Int32 UserId { get; private set; }

        [Required]
        public Int32 ToUserId { get; private set; }

        public Notify(String title, String content, Int32 userId, Int32 toUserId)
        {
            Title = title;
            Content = content;
            UserId = userId;
            ToUserId = toUserId;

            IsNotify = true;
            IsRead = false;
        }

        public Notify() { }
    }

    public partial class Notify
    {
        public void Read()
        {
            IsRead = true;
            OnChanged(nameof(IsRead));
        }
    }
}
