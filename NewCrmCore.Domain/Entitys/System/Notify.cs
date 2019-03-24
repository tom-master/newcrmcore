﻿using System;
using System.Collections.Generic;
using System.Text;
using NewLibCore.Data.SQL.MapperExtension;
using NewLibCore.Data.SQL.MapperExtension.PropertyExtension;

namespace NewCrmCore.Domain.Entitys.System
{

    public partial class Notify : DomainModelBase
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
        public Notify Read()
        {
            IsRead = true;
            OnPropertyChanged(nameof(IsRead));
            return this;
        }
    }
}
