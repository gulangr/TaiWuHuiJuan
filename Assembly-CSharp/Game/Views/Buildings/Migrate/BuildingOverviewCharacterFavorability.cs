using System;
using CharacterDataMonitor;
using TMPro;
using UnityEngine;

namespace Game.Views.Buildings.Migrate
{
	// Token: 0x02000BC7 RID: 3015
	[Obsolete]
	public class BuildingOverviewCharacterFavorability : BuildingOverviewCharacterInfoBase
	{
		// Token: 0x1700104A RID: 4170
		// (get) Token: 0x0600981A RID: 38938 RVA: 0x0046EDDF File Offset: 0x0046CFDF
		private BasicInfoMonitor Item
		{
			get
			{
				return this.MonitorDataItem as BasicInfoMonitor;
			}
		}

		// Token: 0x0600981B RID: 38939 RVA: 0x0046EDEC File Offset: 0x0046CFEC
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<BasicInfoMonitor>(charId, false);
		}

		// Token: 0x0600981C RID: 38940 RVA: 0x0046EE0A File Offset: 0x0046D00A
		internal override void BindEvent()
		{
			this.Item.AddDebtsOfTaiwuListener(new Action(this.RefreshInfo));
		}

		// Token: 0x0600981D RID: 38941 RVA: 0x0046EE25 File Offset: 0x0046D025
		public override void UnbindEvent()
		{
			this.Item.RemoveDebtsOfTaiwuListener(new Action(this.RefreshInfo));
		}

		// Token: 0x0600981E RID: 38942 RVA: 0x0046EE40 File Offset: 0x0046D040
		private void RefreshInfo()
		{
			bool flag = this.Item == null || !this.Item.Init;
			if (!flag)
			{
				bool isTaiwu = base.IsTaiwu;
				if (isTaiwu)
				{
					this.icon.SetSprite("sp_icon_favorability_loveself", false, null);
					this.infoValue.text = "-";
					this.favorDebt.gameObject.SetActive(false);
				}
				else
				{
					short favorabilityToTaiwu = this.Item.FavorabilityToTaiwu;
					bool isInteractedCharacter = this.Item.IsInteractedCharacter;
					bool flag2 = !this.Item.IsInteractedCharacter;
					if (flag2)
					{
						favorabilityToTaiwu = short.MinValue;
					}
					this.icon.SetSprite(CommonUtils.GetFavorIconByInteractedLegacy(favorabilityToTaiwu, isInteractedCharacter), false, null);
					this.infoValue.text = CommonUtils.GetFavorString(favorabilityToTaiwu);
					this.favorDebt.gameObject.SetActive(this.Item.HasAlertness);
					string debtIcon = CommonUtils.GetDebtIcon((long)this.Item.Alertness);
					this.favorDebt.SetSprite(debtIcon, true, null);
				}
			}
		}

		// Token: 0x0600981F RID: 38943 RVA: 0x0046EF51 File Offset: 0x0046D151
		public override void ResetToEmpty()
		{
			this.favorDebt.gameObject.SetActive(false);
			this.icon.SetSprite(string.Empty, false, null);
			this.infoValue.text = "-";
		}

		// Token: 0x040074F3 RID: 29939
		[SerializeField]
		private CImage icon;

		// Token: 0x040074F4 RID: 29940
		[SerializeField]
		private TextMeshProUGUI infoValue;

		// Token: 0x040074F5 RID: 29941
		[SerializeField]
		private CImage favorDebt;
	}
}
