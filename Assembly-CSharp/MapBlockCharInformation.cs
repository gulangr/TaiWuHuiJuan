using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.GameDataBridge;
using TMPro;
using UICommon.Character;
using UnityEngine;

// Token: 0x020003CE RID: 974
public class MapBlockCharInformation : MapBlockCharAlive
{
	// Token: 0x170005F5 RID: 1525
	// (get) Token: 0x06003ABA RID: 15034 RVA: 0x001DC993 File Offset: 0x001DAB93
	private CharacterDisplayData _characterDisplayData
	{
		get
		{
			return this.DisplayData;
		}
	}

	// Token: 0x06003ABB RID: 15035 RVA: 0x001DC99C File Offset: 0x001DAB9C
	public void Init(bool canInteract, MapBlockData mapBlock, CharacterDisplayDataWithInfo characterDisplayDataWithInfo)
	{
		base.Init(canInteract, mapBlock, characterDisplayDataWithInfo.CharacterDisplayData);
		this._characterInfoCountData = characterDisplayDataWithInfo.CharacterInfoCountData;
		this._charConfig = Character.Instance[this._characterDisplayData.TemplateId];
		this.CharId = this._characterDisplayData.CharacterId;
		this.Refresh();
	}

	// Token: 0x06003ABC RID: 15036 RVA: 0x001DC9F8 File Offset: 0x001DABF8
	protected override void Refresh()
	{
		base.Refresh();
		this.RefreshHappinessAndFavorability();
		this.RefreshExperience();
		this.RefreshInfoCount();
	}

	// Token: 0x06003ABD RID: 15037 RVA: 0x001DCA18 File Offset: 0x001DAC18
	protected override void RefreshName()
	{
		string nameContent = NameCenter.GetCharMonasticTitleOrNameByDisplayData(this._characterDisplayData, false, false);
		this.nameText.text = nameContent;
	}

	// Token: 0x06003ABE RID: 15038 RVA: 0x001DCA44 File Offset: 0x001DAC44
	protected override void RefreshOrganization()
	{
		this.organizationText.text = CommonUtils.GetOrganizationGradeString(this._characterDisplayData.OrgInfo, this._characterDisplayData.Gender, this._characterDisplayData.PhysiologicalAge, -1);
		this.organizationIcon.gameObject.SetActive(true);
		this.organizationIcon.SetSprite(CommonUtils.GetIdentityIcon(this._characterDisplayData.OrgInfo.Grade), false, null);
	}

	// Token: 0x06003ABF RID: 15039 RVA: 0x001DCABC File Offset: 0x001DACBC
	protected override void RefreshIcon()
	{
		MapBlockData mapBlock = this.MapBlock;
		bool? flag;
		if (mapBlock == null)
		{
			flag = null;
		}
		else
		{
			HashSet<int> infectedCharacterSet = mapBlock.InfectedCharacterSet;
			flag = ((infectedCharacterSet != null) ? new bool?(infectedCharacterSet.Contains(this._characterDisplayData.CharacterId)) : null);
		}
		bool? isInfected = flag;
		MapBlockData mapBlock2 = this.MapBlock;
		bool? flag2;
		if (mapBlock2 == null)
		{
			flag2 = null;
		}
		else
		{
			HashSet<int> enemyCharacterSet = mapBlock2.EnemyCharacterSet;
			flag2 = ((enemyCharacterSet != null) ? new bool?(enemyCharacterSet.Contains(this._characterDisplayData.CharacterId)) : null);
		}
		bool? isEnemy = flag2;
		bool flag3 = this._characterDisplayData.CompletelyInfected || isInfected.GetValueOrDefault() || isEnemy.GetValueOrDefault();
		if (flag3)
		{
			this.iconImage.SetSprite("blockchar_icon_diren", false, null);
		}
		else
		{
			bool flag4 = this._charConfig.XiangshuType == 1;
			if (flag4)
			{
				this.iconImage.SetSprite("map_icon_xiangshu", false, null);
			}
			else
			{
				bool flag5 = this._charConfig.XiangshuType == 3;
				if (flag5)
				{
					this.iconImage.SetSprite("map_icon_zizhu", false, null);
				}
				else
				{
					bool flag6;
					if (this._charConfig.CreatingType != 1)
					{
						MapBlockData mapBlock3 = this.MapBlock;
						flag6 = (((mapBlock3 != null) ? mapBlock3.CharacterSet : null) == null || !this.MapBlock.CharacterSet.Contains(this._characterDisplayData.CharacterId));
					}
					else
					{
						flag6 = false;
					}
					bool flag7 = flag6;
					if (flag7)
					{
						this.iconImage.SetSprite("map_icon_teshu", false, null);
					}
					else
					{
						this.iconImage.enabled = false;
					}
				}
			}
		}
	}

	// Token: 0x06003AC0 RID: 15040 RVA: 0x001DCC48 File Offset: 0x001DAE48
	protected override void OnClickButton()
	{
		bool isMoving = base.IsMoving;
		if (!isMoving)
		{
			MapBlockData mapBlock = this.MapBlock;
			short? num = (mapBlock != null) ? new short?(mapBlock.BlockId) : null;
			int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
			int currentBlockId = base.CurrentBlockId;
			bool flag = !(num2.GetValueOrDefault() == currentBlockId & num2 != null) || (int)this._characterDisplayData.Location.BlockId != base.CurrentBlockId;
			if (!flag)
			{
				GameDataBridge.AddMethodCall<int>(-1, 12, 13, this._characterDisplayData.CharacterId);
				base.OnClickButton();
			}
		}
	}

	// Token: 0x06003AC1 RID: 15041 RVA: 0x001DCD08 File Offset: 0x001DAF08
	private void RefreshHappinessAndFavorability()
	{
		Refers happinessRefers = this.iconHolder.CGet<Refers>("CharacterHappiness");
		CharacterHappiness happinessController = happinessRefers.UserObject as CharacterHappiness;
		bool flag = happinessController == null;
		if (flag)
		{
			happinessController = new CharacterHappiness(happinessRefers, false);
			happinessRefers.UserObject = happinessController;
		}
		happinessController.CharacterId = this._characterDisplayData.CharacterId;
		Refers favorRefers = this.iconHolder.CGet<Refers>("CharacterFavorability");
		CharacterFavorability characterFavorability = favorRefers.UserObject as CharacterFavorability;
		bool flag2 = characterFavorability == null;
		if (flag2)
		{
			characterFavorability = new CharacterFavorability(favorRefers, false);
			favorRefers.UserObject = characterFavorability;
		}
		characterFavorability.CharacterId = this._characterDisplayData.CharacterId;
	}

	// Token: 0x06003AC2 RID: 15042 RVA: 0x001DCDB0 File Offset: 0x001DAFB0
	public override void OnHide()
	{
		base.OnHide();
		Refers happinessRefers = this.iconHolder.CGet<Refers>("CharacterHappiness");
		CharacterHappiness happinessController = happinessRefers.UserObject as CharacterHappiness;
		bool flag = happinessController != null;
		if (flag)
		{
			happinessController.CharacterId = -1;
		}
		Refers favorRefers = this.iconHolder.CGet<Refers>("CharacterFavorability");
		CharacterHappiness characterFavorability = favorRefers.UserObject as CharacterHappiness;
		bool flag2 = characterFavorability != null;
		if (flag2)
		{
			characterFavorability.CharacterId = -1;
		}
	}

	// Token: 0x06003AC3 RID: 15043 RVA: 0x001DCE24 File Offset: 0x001DB024
	private void RefreshExperience()
	{
		TooltipInvoker tooltipInvoker = this.experienceTip;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		this.experienceTip.RuntimeParam.Clear();
		this.experienceTip.RuntimeParam.SetObject("CharacterDisplayData", this._characterDisplayData);
		this.experienceTip.Refresh(true, -1);
	}

	// Token: 0x06003AC4 RID: 15044 RVA: 0x001DCE8A File Offset: 0x001DB08A
	private void RefreshInfoCount()
	{
		this.infoCount.text = this._characterInfoCountData.HoldInfoCount.ToString();
		this.relatedInfoCount.text = this._characterInfoCountData.HoldInfoTaiwuRelatedCount.ToString();
	}

	// Token: 0x04002A47 RID: 10823
	[SerializeField]
	private Refers iconHolder;

	// Token: 0x04002A48 RID: 10824
	[SerializeField]
	private TooltipInvoker experienceTip;

	// Token: 0x04002A49 RID: 10825
	[SerializeField]
	private TextMeshProUGUI infoCount;

	// Token: 0x04002A4A RID: 10826
	[SerializeField]
	private TextMeshProUGUI relatedInfoCount;

	// Token: 0x04002A4B RID: 10827
	private CharacterInfoCountData _characterInfoCountData;

	// Token: 0x04002A4C RID: 10828
	private CharacterItem _charConfig;
}
