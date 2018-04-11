using System;
using NewLibCore.Data.Mapper.MapperExtension;

namespace NewCRM.Domain.Entitys
{
	[Serializable]
	public abstract class DomainModelBase: PropertyMonitor
	{

		#region private fields
		private Int32 _id;
		private Boolean _isDelete;
		private DateTime _addTime, _lastModifyTime;
		#endregion

		#region ctor

		protected DomainModelBase()
		{
			IsDeleted = false;
		}

		#endregion

		#region public property

		public Int32 Id
		{
			get
			{
				return _id;
			}
			protected set
			{
				_id = value;
			}
		}

		[PropertyDefaultValue(typeof(Boolean), false)]
		public Boolean IsDeleted
		{
			get
			{
				return _isDelete;
			}
			protected set
			{
				_isDelete = value;
			}
		}

		[DateTimeDefaultValue]
		public DateTime AddTime
		{
			get
			{
				return _addTime;
			}

			protected set
			{
				_addTime = value;
			}
		}

		[DateTimeDefaultValue]
		public DateTime LastModifyTime
		{
			get
			{
				return _lastModifyTime;
			}
			protected set
			{
				_lastModifyTime = value;
			}
		}

		public void Remove()
		{
			IsDeleted = true;
			OnPropertyChanged(nameof(IsDeleted));
		}

		#endregion
	}
}