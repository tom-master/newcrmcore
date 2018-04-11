using System;

namespace NewCrmCore.Infrastructure.CommonTools.CustomExtension
{
	public static class BooleanExtension
    {
		public static Int32 ParseToInt32(this Boolean boo)
		{
			return boo ? 1 : 0;
		}
    }
}
