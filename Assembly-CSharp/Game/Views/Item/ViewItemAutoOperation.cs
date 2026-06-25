using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using UnityEngine;

namespace Game.Views.Item
{
	// Token: 0x02000A14 RID: 2580
	public class ViewItemAutoOperation : UIBase
	{
		// Token: 0x06007E3A RID: 32314 RVA: 0x003AA3B9 File Offset: 0x003A85B9
		public override void OnInit(ArgumentBox argsBox)
		{
		}

		// Token: 0x06007E3B RID: 32315 RVA: 0x003AA3BC File Offset: 0x003A85BC
		public override void InitMonitorFieldIds()
		{
			base.InitMonitorFieldIds();
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 112, ulong.MaxValue, null));
		}

		// Token: 0x06007E3C RID: 32316 RVA: 0x003AA3DD File Offset: 0x003A85DD
		private void Awake()
		{
			this.buttleClose.ClearAndAddListener(new Action(this.QuickHide));
		}

		// Token: 0x06007E3D RID: 32317 RVA: 0x003AA3FC File Offset: 0x003A85FC
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 0)
				{
					bool flag = notification.Uid.DomainId == 5;
					if (flag)
					{
						bool flag2 = notification.Uid.DataId == 112;
						if (flag2)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._data);
							base.RemoveMonitorFieldId(0);
							this.Element.ShowAfterRefresh();
							this.Init();
						}
					}
				}
			}
		}

		// Token: 0x06007E3E RID: 32318 RVA: 0x003AA4C8 File Offset: 0x003A86C8
		private void Init()
		{
			this.groupDiscord.Init(this._data, true);
			this.groupDisassemble.Init(this._data, false);
		}

		// Token: 0x06007E3F RID: 32319 RVA: 0x003AA4F1 File Offset: 0x003A86F1
		public override void QuickHide()
		{
			base.QuickHide();
			GameDataBridge.AddDataModification<ItemAutoOperationSettingData>(5, 112, ulong.MaxValue, uint.MaxValue, this._data);
		}

		// Token: 0x0400605B RID: 24667
		[SerializeField]
		private ItemAutoOperationGroup groupDiscord;

		// Token: 0x0400605C RID: 24668
		[SerializeField]
		private ItemAutoOperationGroup groupDisassemble;

		// Token: 0x0400605D RID: 24669
		[SerializeField]
		private CButton buttleClose;

		// Token: 0x0400605E RID: 24670
		private ItemAutoOperationSettingData _data;
	}
}
