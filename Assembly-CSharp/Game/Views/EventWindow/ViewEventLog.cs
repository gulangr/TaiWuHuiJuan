using System;
using System.Collections.Generic;
using FrameWork;
using Game.Views.InstantNotification;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.TaiwuEvent.EventLog;
using GameData.GameDataBridge;
using GameData.Serializer;
using UnityEngine;

namespace Game.Views.EventWindow
{
	// Token: 0x02000A45 RID: 2629
	public class ViewEventLog : UIBase
	{
		// Token: 0x060081D0 RID: 33232 RVA: 0x003C725C File Offset: 0x003C545C
		public override void OnInit(ArgumentBox argsBox)
		{
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		}

		// Token: 0x060081D1 RID: 33233 RVA: 0x003C7290 File Offset: 0x003C5490
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (a == "BtnEventLog")
			{
				this.QuickHide();
			}
		}

		// Token: 0x060081D2 RID: 33234 RVA: 0x003C72BF File Offset: 0x003C54BF
		public override void QuickHide()
		{
			base.QuickHide();
			this.eventLog.ClearData();
		}

		// Token: 0x060081D3 RID: 33235 RVA: 0x003C72D8 File Offset: 0x003C54D8
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					bool flag = notification.DomainId == 12;
					if (flag)
					{
						bool flag2 = notification.MethodId == 39;
						if (flag2)
						{
							EventLogData eventLogData = null;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref eventLogData);
							this.eventLog.Init(this, eventLogData);
							this.Element.ShowAfterRefresh();
						}
					}
				}
			}
		}

		// Token: 0x060081D4 RID: 33236 RVA: 0x003C7398 File Offset: 0x003C5598
		private void OnListenerIdReady()
		{
			TaiwuEventDomainMethod.Call.GetEventLogData(this.Element.GameDataListenerId);
		}

		// Token: 0x0400633E RID: 25406
		[SerializeField]
		private Game.Views.InstantNotification.EventLogHelper eventLog;
	}
}
