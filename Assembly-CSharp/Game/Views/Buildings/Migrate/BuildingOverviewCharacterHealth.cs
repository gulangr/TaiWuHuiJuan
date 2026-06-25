using System;
using CharacterDataMonitor;
using TMPro;
using UnityEngine;

namespace Game.Views.Buildings.Migrate
{
	// Token: 0x02000BCA RID: 3018
	[Obsolete]
	public class BuildingOverviewCharacterHealth : BuildingOverviewCharacterInfoBase
	{
		// Token: 0x1700104D RID: 4173
		// (get) Token: 0x0600982F RID: 38959 RVA: 0x0046F1B2 File Offset: 0x0046D3B2
		private AgeHealthMonitor Item
		{
			get
			{
				return this.MonitorDataItem as AgeHealthMonitor;
			}
		}

		// Token: 0x06009830 RID: 38960 RVA: 0x0046F1C0 File Offset: 0x0046D3C0
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AgeHealthMonitor>(charId, false);
		}

		// Token: 0x06009831 RID: 38961 RVA: 0x0046F1E0 File Offset: 0x0046D3E0
		internal override void BindEvent()
		{
			this.Item.AddOnHealthChangeEventListener(new Action(this.RefreshInfo));
			SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DisorderOfQiMonitor>(this.Item.CharacterId, false).AddDisorderOfQiListener(new Action(this.Item.Refresh));
			bool init = this.Item.Init;
			if (init)
			{
				this.RefreshInfo();
			}
		}

		// Token: 0x06009832 RID: 38962 RVA: 0x0046F24C File Offset: 0x0046D44C
		public override void UnbindEvent()
		{
			this.Item.RemoveOnHealthChangeEventListener(new Action(this.RefreshInfo));
			SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DisorderOfQiMonitor>(this.Item.CharacterId, false).RemoveDisorderOfQiListener(new Action(this.Item.Refresh));
		}

		// Token: 0x06009833 RID: 38963 RVA: 0x0046F2A0 File Offset: 0x0046D4A0
		private void RefreshInfo()
		{
			bool flag = this.Item == null || !this.Item.Init;
			if (!flag)
			{
				ValueTuple<string, float, int> info = CommonUtils.GetCharacterHealthInfo(this.Item.Health, this.Item.LeftMaxHealth, base.CharacterId);
				this.stateLabel.text = info.Item1;
			}
		}

		// Token: 0x06009834 RID: 38964 RVA: 0x0046F301 File Offset: 0x0046D501
		public override void ResetToEmpty()
		{
			this.stateLabel.text = string.Empty;
		}

		// Token: 0x040074FB RID: 29947
		[SerializeField]
		private TextMeshProUGUI stateLabel;
	}
}
