using System;
using Config;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.GameDataBridge;
using TMPro;
using UnityEngine;

// Token: 0x020003CF RID: 975
public class MapBlockCharInformation2 : MapBlockCharAlive2
{
	// Token: 0x170005F6 RID: 1526
	// (get) Token: 0x06003AC6 RID: 15046 RVA: 0x001DCECE File Offset: 0x001DB0CE
	private CharacterDisplayData CharacterDisplayData
	{
		get
		{
			return this.DisplayData;
		}
	}

	// Token: 0x06003AC7 RID: 15047 RVA: 0x001DCED8 File Offset: 0x001DB0D8
	public void Init(bool canInteract, MapBlockData mapBlock, CharacterDisplayDataWithInfo characterDisplayDataWithInfo)
	{
		base.Init(canInteract, mapBlock, characterDisplayDataWithInfo.CharacterDisplayData);
		this._characterInfoCountData = characterDisplayDataWithInfo.CharacterInfoCountData;
		this._charConfig = Character.Instance[this.CharacterDisplayData.TemplateId];
		this.CharId = this.CharacterDisplayData.CharacterId;
		this.Refresh();
	}

	// Token: 0x06003AC8 RID: 15048 RVA: 0x001DCF34 File Offset: 0x001DB134
	protected override void Refresh()
	{
		base.Refresh();
		this.RefreshHappinessAndFavorability();
		this.RefreshExperience();
		this.RefreshInfoCount();
	}

	// Token: 0x06003AC9 RID: 15049 RVA: 0x001DCF54 File Offset: 0x001DB154
	protected override void RefreshName()
	{
		string nameContent = NameCenter.GetMonasticTitleOrDisplayName(this.CharacterDisplayData, false);
		this.nameLabel.text = nameContent;
	}

	// Token: 0x06003ACA RID: 15050 RVA: 0x001DCF7C File Offset: 0x001DB17C
	protected override void RefreshOrganization()
	{
		this.organizationLabel.text = CommonUtils.GetOrganizationGradeString(this.CharacterDisplayData.OrgInfo, this.CharacterDisplayData.Gender, this.CharacterDisplayData.PhysiologicalAge, -1);
	}

	// Token: 0x06003ACB RID: 15051 RVA: 0x001DCFB4 File Offset: 0x001DB1B4
	protected override void OnClickButton()
	{
		bool isMoving = base.IsMoving;
		if (!isMoving)
		{
			MapBlockData mapBlock = this.MapBlock;
			short? num = (mapBlock != null) ? new short?(mapBlock.BlockId) : null;
			int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
			int currentBlockId = base.CurrentBlockId;
			bool flag = !(num2.GetValueOrDefault() == currentBlockId & num2 != null) || (int)this.CharacterDisplayData.Location.BlockId != base.CurrentBlockId;
			if (!flag)
			{
				GameDataBridge.AddMethodCall<int>(-1, 12, 13, this.CharacterDisplayData.CharacterId);
				base.OnClickButton();
			}
		}
	}

	// Token: 0x06003ACC RID: 15052 RVA: 0x001DD071 File Offset: 0x001DB271
	private void RefreshHappinessAndFavorability()
	{
	}

	// Token: 0x06003ACD RID: 15053 RVA: 0x001DD074 File Offset: 0x001DB274
	private void RefreshExperience()
	{
	}

	// Token: 0x06003ACE RID: 15054 RVA: 0x001DD077 File Offset: 0x001DB277
	private void RefreshInfoCount()
	{
		this.infoCount.text = this._characterInfoCountData.HoldInfoCount.ToString();
		this.relatedInfoCount.text = this._characterInfoCountData.HoldInfoTaiwuRelatedCount.ToString();
	}

	// Token: 0x04002A4D RID: 10829
	[SerializeField]
	private TextMeshProUGUI infoCount;

	// Token: 0x04002A4E RID: 10830
	[SerializeField]
	private TextMeshProUGUI relatedInfoCount;

	// Token: 0x04002A4F RID: 10831
	private CharacterInfoCountData _characterInfoCountData;

	// Token: 0x04002A50 RID: 10832
	private CharacterItem _charConfig;
}
