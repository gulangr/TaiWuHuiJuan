using System;

namespace Game.Views.Encyclopedia.Event
{
	// Token: 0x02000A7B RID: 2683
	public class EventArgs<T> : IEventArgs
	{
		// Token: 0x060083E0 RID: 33760 RVA: 0x003D5590 File Offset: 0x003D3790
		public static IEventArgs CreateEventArgs(T val)
		{
			return new EventArgs<T>(val);
		}

		// Token: 0x060083E1 RID: 33761 RVA: 0x003D55A8 File Offset: 0x003D37A8
		private EventArgs(T arg)
		{
			this.arg = arg;
		}

		// Token: 0x04006503 RID: 25859
		public T arg;
	}
}
