using System;
using CharacterDataMonitor;
using GameData.Domains.Character;
using TMPro;

namespace Game.Views.Buildings.Migrate
{
	// Token: 0x02000BC9 RID: 3017
	[Obsolete]
	public class BuildingOverviewCharacterHappiness : BuildingOverviewCharacterInfoBase
	{
		// Token: 0x1700104C RID: 4172
		// (get) Token: 0x06009828 RID: 38952 RVA: 0x0046F098 File Offset: 0x0046D298
		private DetailInfoMonitor Item
		{
			get
			{
				return this.MonitorDataItem as DetailInfoMonitor;
			}
		}

		// Token: 0x06009829 RID: 38953 RVA: 0x0046F0A8 File Offset: 0x0046D2A8
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DetailInfoMonitor>(charId, false);
		}

		// Token: 0x0600982A RID: 38954 RVA: 0x0046F0C6 File Offset: 0x0046D2C6
		internal override void BindEvent()
		{
			this.Item.AddOnHappinessListener(new Action(this.RefreshInfo));
		}

		// Token: 0x0600982B RID: 38955 RVA: 0x0046F0E1 File Offset: 0x0046D2E1
		public override void UnbindEvent()
		{
			this.Item.RemoveOnHappinessListener(new Action(this.RefreshInfo));
		}

		// Token: 0x0600982C RID: 38956 RVA: 0x0046F0FC File Offset: 0x0046D2FC
		private void RefreshInfo()
		{
			bool flag = this.Item == null || !this.Item.Init;
			if (!flag)
			{
				sbyte happinessLevel = HappinessType.GetHappinessType(this.Item.Happiness);
				this.icon.SetSprite(this.UseNewIcon ? ("ui9_icon_happiness_big_" + happinessLevel.ToString()) : CommonUtils.GetHappinessIconLegacy(happinessLevel), false, null);
				this.infoValue.text = CommonUtils.GetHappinessString(happinessLevel);
			}
		}

		// Token: 0x0600982D RID: 38957 RVA: 0x0046F17B File Offset: 0x0046D37B
		public override void ResetToEmpty()
		{
			this.icon.SetSprite(string.Empty, false, null);
			this.infoValue.text = string.Empty;
		}

		// Token: 0x040074F8 RID: 29944
		public CImage icon;

		// Token: 0x040074F9 RID: 29945
		public TextMeshProUGUI infoValue;

		// Token: 0x040074FA RID: 29946
		public bool UseNewIcon = false;
	}
}
