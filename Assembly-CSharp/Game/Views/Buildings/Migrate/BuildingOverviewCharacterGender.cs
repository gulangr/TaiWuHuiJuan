using System;
using CharacterDataMonitor;
using TMPro;
using UnityEngine;

namespace Game.Views.Buildings.Migrate
{
	// Token: 0x02000BC8 RID: 3016
	[Obsolete]
	public class BuildingOverviewCharacterGender : BuildingOverviewCharacterInfoBase
	{
		// Token: 0x1700104B RID: 4171
		// (get) Token: 0x06009821 RID: 38945 RVA: 0x0046EF93 File Offset: 0x0046D193
		private BasicInfoMonitor Item
		{
			get
			{
				return this.MonitorDataItem as BasicInfoMonitor;
			}
		}

		// Token: 0x06009822 RID: 38946 RVA: 0x0046EFA0 File Offset: 0x0046D1A0
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<BasicInfoMonitor>(charId, false);
		}

		// Token: 0x06009823 RID: 38947 RVA: 0x0046EFBE File Offset: 0x0046D1BE
		internal override void BindEvent()
		{
			this.Item.AddGenderListener(new Action(this.RefreshInfo));
		}

		// Token: 0x06009824 RID: 38948 RVA: 0x0046EFD9 File Offset: 0x0046D1D9
		public override void UnbindEvent()
		{
			this.Item.RemoveGenderListener(new Action(this.RefreshInfo));
		}

		// Token: 0x06009825 RID: 38949 RVA: 0x0046EFF4 File Offset: 0x0046D1F4
		private void RefreshInfo()
		{
			bool flag = this.Item == null || !this.Item.Init;
			if (!flag)
			{
				CommonUtils.EDisplayGender displayGender = CommonUtils.GetDisplayGender(this.Item.Gender, this.Item.NameRelatedData.CharTemplateId);
				this.icon.SetSprite(CommonUtils.GetGenderIcon(displayGender), false, null);
				this.infoValue.text = CommonUtils.GetGenderString(displayGender);
			}
		}

		// Token: 0x06009826 RID: 38950 RVA: 0x0046F068 File Offset: 0x0046D268
		public override void ResetToEmpty()
		{
			this.icon.SetSprite(string.Empty, false, null);
			this.infoValue.text = string.Empty;
		}

		// Token: 0x040074F6 RID: 29942
		[SerializeField]
		private CImage icon;

		// Token: 0x040074F7 RID: 29943
		[SerializeField]
		private TextMeshProUGUI infoValue;
	}
}
