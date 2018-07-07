using System;
using NewLibCore.Data.Mapper.MapperExtension;
using NewLibCore.Data.Mapper.PropertyExtension;

namespace NewCrmCore.Domain.Entitys
{
	[Serializable]
	public abstract class DomainModelBase: PropertyMonitor
	{

		#region private fields
		#endregion

		#region ctor

		protected DomainModelBase()
		{
			IsDeleted = false;
		}

		#endregion

		#region public property

		public Int32 Id { get; protected set; }

		[PropertyDefaultValue(typeof(Boolean), false)]
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

		#endregion
	}
}