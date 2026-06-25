using System;

namespace Game.Views.Encyclopedia.Event
{
	// Token: 0x02000A7C RID: 2684
	public static class EventArgsExtend
	{
		// Token: 0x060083E2 RID: 33762 RVA: 0x003D55BC File Offset: 0x003D37BC
		public static T GetValue<T>(this IEventArgs args)
		{
			T result = default(T);
			bool flag = args is EventArgs<T>;
			if (flag)
			{
				result = ((EventArgs<T>)args).arg;
			}
			return result;
		}
	}
}
