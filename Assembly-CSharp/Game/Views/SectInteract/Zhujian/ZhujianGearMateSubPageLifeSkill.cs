using System;
using System.Collections.Generic;
using GameData.Domains.Story;
using GameData.Domains.World.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using UnityEngine;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009D3 RID: 2515
	public class ZhujianGearMateSubPageLifeSkill : ZhujianGearMateSubPage
	{
		// Token: 0x06007A78 RID: 31352 RVA: 0x0038E0D0 File Offset: 0x0038C2D0
		public override void Init(ViewZhujianGearMate parent)
		{
			base.Init(parent);
			this.skillBase.Init(parent, false, new Action(this.RequestGearMateData));
		}

		// Token: 0x06007A79 RID: 31353 RVA: 0x0038E0F5 File Offset: 0x0038C2F5
		protected override void OnShowDataRequest()
		{
			this.skillBase.ResetToggleGroupItemSource();
			this.RequestGearMateData();
		}

		// Token: 0x06007A7A RID: 31354 RVA: 0x0038E10C File Offset: 0x0038C30C
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

		// Token: 0x06007A7B RID: 31355 RVA: 0x0038E144 File Offset: 0x0038C344
		public override void OnListenerIdReady()
		{
			base.OnListenerIdReady();
			this.RequestGearMateData();
		}

		// Token: 0x06007A7C RID: 31356 RVA: 0x0038E158 File Offset: 0x0038C358
		private void RequestGearMateData()
		{
			bool flag = this.GearMateId < 0 || this.ListenerId == 0;
			if (!flag)
			{
				StoryDomainMethod.Call.GetSectZhujianGearMateSkillDisplayData(this.ListenerId, this.GearMateId, false);
			}
		}

		// Token: 0x06007A7D RID: 31357 RVA: 0x0038E194 File Offset: 0x0038C394
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

		// Token: 0x06007A7E RID: 31358 RVA: 0x0038E23C File Offset: 0x0038C43C
		private void Refresh()
		{
			this.skillBase.Refresh(this._displayData);
		}

		// Token: 0x04005CD1 RID: 23761
		[SerializeField]
		private ZhujianGearMateSubPageSkillBase skillBase;

		// Token: 0x04005CD2 RID: 23762
		private SectZhujianGearMateSkillDisplayData _displayData;

		// Token: 0x04005CD3 RID: 23763
		private const bool IsCombatSkill = false;
	}
}
