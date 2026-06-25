using System;

namespace Game.Views.Encyclopedia.Event
{
	// Token: 0x02000A80 RID: 2688
	public interface IEventHub
	{
		// Token: 0x060083EC RID: 33772
		void AddListener(int eventId, IEventListener listener);

		// Token: 0x060083ED RID: 33773
		void RemoveListener(int eventId, IEventListener listener);

		// Token: 0x060083EE RID: 33774
		void Dispatch(int eventId, IEventArgs args);
	}
}
