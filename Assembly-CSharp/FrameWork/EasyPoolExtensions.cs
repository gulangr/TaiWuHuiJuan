using System;
using System.Collections.Generic;

namespace FrameWork
{
	// Token: 0x02000FE6 RID: 4070
	public static class EasyPoolExtensions
	{
		// Token: 0x0600B9E1 RID: 47585 RVA: 0x0054AD80 File Offset: 0x00548F80
		public static List<T> ToPoolList<T>(this IEnumerable<T> enumerable)
		{
			List<T> list = EasyPool.Get<List<T>>();
			list.Clear();
			list.AddRange(enumerable);
			return list;
		}
	}
}
