using System;

namespace Game.Views.Encyclopedia.Event
{
	// Token: 0x02000A7E RID: 2686
	public class EventManager
	{
		// Token: 0x17000E74 RID: 3700
		// (get) Token: 0x060083E7 RID: 33767 RVA: 0x003D5730 File Offset: 0x003D3930
		public static EventManager Instance
		{
			get
			{
				bool flag = EventManager._instance == null;
				if (flag)
				{
					EventManager._instance = new EventManager();
				}
				return EventManager._instance;
			}
		}

		// Token: 0x060083E8 RID: 33768 RVA: 0x003D575F File Offset: 0x003D395F
		public EventManager()
		{
			this._eventHub = new EventHub();
		}

		// Token: 0x060083E9 RID: 33769 RVA: 0x003D5774 File Offset: 0x003D3974
		public void AddListener(int eventId, IEventListener listener)
		{
			this._eventHub.AddListener(eventId, listener);
		}

		// Token: 0x060083EA RID: 33770 RVA: 0x003D5785 File Offset: 0x003D3985
		public void Dispatch(int eventId, IEventArgs args = null)
		{
			this._eventHub.Dispatch(eventId, args);
		}

		// Token: 0x060083EB RID: 33771 RVA: 0x003D5796 File Offset: 0x003D3996
		public void RemoveListener(int eventId, IEventListener listener)
		{
			this._eventHub.RemoveListener(eventId, listener);
		}

		// Token: 0x04006505 RID: 25861
		private static EventManager _instance;

		// Token: 0x04006506 RID: 25862
		private IEventHub _eventHub;
	}
}
