using System;
using System.Collections.Generic;

namespace Game.Views.Encyclopedia.Event
{
	// Token: 0x02000A7D RID: 2685
	public class EventHub : IEventHub
	{
		// Token: 0x060083E3 RID: 33763 RVA: 0x003D55F4 File Offset: 0x003D37F4
		public void AddListener(int eventId, IEventListener listener)
		{
			bool flag = this._eventDic == null;
			if (!flag)
			{
				List<IEventListener> list = null;
				this._eventDic.TryGetValue(eventId, out list);
				bool flag2 = list == null;
				if (flag2)
				{
					list = new List<IEventListener>();
					this._eventDic[eventId] = list;
				}
				list.Add(listener);
			}
		}

		// Token: 0x060083E4 RID: 33764 RVA: 0x003D564C File Offset: 0x003D384C
		public void RemoveListener(int eventId, IEventListener listener)
		{
			bool flag = this._eventDic == null;
			if (!flag)
			{
				List<IEventListener> list = null;
				this._eventDic.TryGetValue(eventId, out list);
				bool flag2 = list != null && list.Contains(listener);
				if (flag2)
				{
					list.Remove(listener);
				}
			}
		}

		// Token: 0x060083E5 RID: 33765 RVA: 0x003D5698 File Offset: 0x003D3898
		public void Dispatch(int eventId, IEventArgs args)
		{
			bool flag = this._eventDic == null;
			if (!flag)
			{
				List<IEventListener> list = null;
				this._eventDic.TryGetValue(eventId, out list);
				bool flag2 = list == null || list.Count <= 0;
				if (!flag2)
				{
					for (int i = 0; i < list.Count; i++)
					{
						IEventListener eventListener = list[i];
						bool flag3 = eventListener != null;
						if (flag3)
						{
							eventListener.HandleEvent(eventId, args);
						}
					}
				}
			}
		}

		// Token: 0x04006504 RID: 25860
		private Dictionary<int, List<IEventListener>> _eventDic = new Dictionary<int, List<IEventListener>>();
	}
}
