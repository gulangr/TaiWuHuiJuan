using System;
using System.Collections.Generic;
using GameData.Domains.Story;
using GameData.Domains.World.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using UnityEngine;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009CE RID: 2510
	public class ZhujianGearMateSubPageCombatSkill : ZhujianGearMateSubPage
	{
		// Token: 0x060079F4 RID: 31220 RVA: 0x0038A706 File Offset: 0x00388906
		public override void Init(ViewZhujianGearMate parent)
		{
			base.Init(parent);
			this.skillBase.Init(parent, true, new Action(this.RequestGearMateData));
		}

		// Token: 0x060079F5 RID: 31221 RVA: 0x0038A72B File Offset: 0x0038892B
		protected override void OnShowDataRequest()
		{
			this.skillBase.ResetToggleGroupItemSource();
			this.RequestGearMateData();
		}

		// Token: 0x060079F6 RID: 31222 RVA: 0x0038A744 File Offset: 0x00388944
		public override void SetGearMateId(int gearMateId)
		{
			bool flag = this.GearMateId == gearMateId;
			if (!flag)
			{
				base.SetGearMateId(gearMateId);
				bool isVisible = this.IsVisible;
				if (isVisible)
				{
					this.RequestGearMateData();
				}
			}
		}

		// Token: 0x060079F7 RID: 31223 RVA: 0x0038A77C File Offset: 0x0038897C
		public override void OnListenerIdReady()
		{
			base.OnListenerIdReady();
			this.RequestGearMateData();
		}

		// Token: 0x060079F8 RID: 31224 RVA: 0x0038A790 File Offset: 0x00388990
		private void RequestGearMateData()
		{
			bool flag = this.GearMateId < 0 || this.ListenerId == 0;
			if (!flag)
			{
				StoryDomainMethod.Call.GetSectZhujianGearMateSkillDisplayData(this.ListenerId, this.GearMateId, true);
			}
		}

		// Token: 0x060079F9 RID: 31225 RVA: 0x0038A7CC File Offset: 0x003889CC
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				bool flag = notification.Type == 1;
				if (flag)
				{
					bool flag2 = notification.DomainId == 20 && notification.MethodId == 18;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._displayData);
						this.Refresh();
						base.SetContentReady();
					}
				}
			}
		}

		// Token: 0x060079FA RID: 31226 RVA: 0x0038A874 File Offset: 0x00388A74
		private void Refresh()
		{
			this.skillBase.Refresh(this._displayData);
		}

		// Token: 0x04005C6D RID: 23661
		[SerializeField]
		private ZhujianGearMateSubPageSkillBase skillBase;

		// Token: 0x04005C6E RID: 23662
		private SectZhujianGearMateSkillDisplayData _displayData;

		// Token: 0x04005C6F RID: 23663
		private const bool IsCombatSkill = true;
	}
}
