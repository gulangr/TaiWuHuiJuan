using System;

namespace Game.Views.Encyclopedia.Event
{
	// Token: 0x02000A81 RID: 2689
	public interface IEventListener
	{
		// Token: 0x060083EF RID: 33775
		void HandleEvent(int eventId, IEventArgs args);
	}
}
