using System;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.GameDataBridge;
using TMPro;
using UnityEngine;

// Token: 0x020003CC RID: 972
public class MapBlockCharGrave : MapBlockCharBase
{
	// Token: 0x06003AA7 RID: 15015 RVA: 0x001DC544 File Offset: 0x001DA744
	public void Init(bool canInteract, MapBlockData mapBlock, GraveDisplayData graveDisplayData)
	{
		base.Init(canInteract, mapBlock, null);
		this._graveDisplayData = graveDisplayData;
		this.Refresh();
	}

	// Token: 0x06003AA8 RID: 15016 RVA: 0x001DC55F File Offset: 0x001DA75F
	protected override void Refresh()
	{
		base.Refresh();
		this.RefreshIcon();
		this.RefreshExperience();
		this.RefreshDurability();
	}

	// Token: 0x06003AA9 RID: 15017 RVA: 0x001DC580 File Offset: 0x001DA780
	protected override void RefreshName()
	{
		string nameContent = NameCenter.GetCharMonasticTitleOrNameByGraveData(this._graveDisplayData, false, false);
		this.nameText.text = nameContent;
	}

	// Token: 0x06003AAA RID: 15018 RVA: 0x001DC5AC File Offset: 0x001DA7AC
	protected override void RefreshOrganization()
	{
		OrganizationInfo organizationInfo = new OrganizationInfo(this._graveDisplayData.NameData.OrgTemplateId, this._graveDisplayData.NameData.OrgGrade, this._graveDisplayData.Principal, this._graveDisplayData.OrgSettlementId);
		this.organizationText.text = CommonUtils.GetOrganizationGradeString(organizationInfo, this._graveDisplayData.NameData.Gender, -1, -1);
		this.organizationIcon.gameObject.SetActive(true);
		this.organizationIcon.SetSprite(CommonUtils.GetIdentityIcon(this._graveDisplayData.NameData.OrgGrade), false, null);
	}

	// Token: 0x06003AAB RID: 15019 RVA: 0x001DC650 File Offset: 0x001DA850
	public void RefreshAvatar()
	{
		SingletonObject.getInstance<CharacterMonitorModel>().RefreshTargetMonitorAliveState(this._graveDisplayData.Id, true);
		this.avatarImage.SetSprite("NPCFace_tomb", false, null);
		this.avatarImage.gameObject.SetActive(true);
	}

	// Token: 0x06003AAC RID: 15020 RVA: 0x001DC68F File Offset: 0x001DA88F
	private void RefreshIcon()
	{
		this.iconImage.SetSprite(MapBlockCharBase.GetIconGrave((int)this._graveDisplayData.Level), false, null);
	}

	// Token: 0x06003AAD RID: 15021 RVA: 0x001DC6B0 File Offset: 0x001DA8B0
	protected override void OnClickButton()
	{
		bool isMoving = base.IsMoving;
		if (!isMoving)
		{
			GameDataBridge.AddMethodCall<int>(-1, 12, 28, this._graveDisplayData.Id);
		}
	}

	// Token: 0x06003AAE RID: 15022 RVA: 0x001DC6E0 File Offset: 0x001DA8E0
	private void RefreshExperience()
	{
		TooltipInvoker tooltipInvoker = this.experienceTip;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		this.experienceTip.RuntimeParam.Clear();
		this.experienceTip.RuntimeParam.SetObject("GraveDisplayData", this._graveDisplayData);
		this.experienceTip.Refresh(true, -1);
	}

	// Token: 0x06003AAF RID: 15023 RVA: 0x001DC746 File Offset: 0x001DA946
	private void RefreshDurability()
	{
		this.durabilityText.text = this._graveDisplayData.Durability.ToString();
	}

	// Token: 0x04002A3E RID: 10814
	[SerializeField]
	protected CImage avatarImage;

	// Token: 0x04002A3F RID: 10815
	[SerializeField]
	protected CImage iconImage;

	// Token: 0x04002A40 RID: 10816
	[SerializeField]
	private TooltipInvoker experienceTip;

	// Token: 0x04002A41 RID: 10817
	[SerializeField]
	private TextMeshProUGUI durabilityText;

	// Token: 0x04002A42 RID: 10818
	private GraveDisplayData _graveDisplayData;
}
