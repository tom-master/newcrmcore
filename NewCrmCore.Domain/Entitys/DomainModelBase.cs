using System;
using NewLibCore.Data.SQL.MapperExtension;
using NewLibCore.Data.SQL.PropertyExtension;

namespace NewCrmCore.Domain.Entitys
{
    [Serializable]
    public abstract class DomainModelBase : PropertyMonitor
    {
        protected DomainModelBase()
        {
            IsDeleted = false;
        }

        public Int32 Id { get; protected set; }

        [PropertyDefaultValue(typeof(Boolean))]
        public Boolean IsDeleted { get; protected set; }

        [DateTimeDefaultValue]
        public DateTime AddTime { get; protected set; }

        [DateTimeDefaultValue]
        public DateTime LastModifyTime { get; protected set; }

        public void Remove()
        {
            IsDeleted = true;
            OnPropertyChanged(new PropertyArgs(nameof(IsDeleted), IsDeleted));
        }

        public override void SetUpdateTime()
        {
            LastModifyTime = DateTime.Now;
            OnPropertyChanged(new PropertyArgs(nameof(LastModifyTime), LastModifyTime));
        }
    }
}