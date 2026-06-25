using System;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.GameDataBridge;
using TMPro;
using UnityEngine;

// Token: 0x020003CD RID: 973
public class MapBlockCharGrave2 : MapBlockCharBase2
{
	// Token: 0x06003AB1 RID: 15025 RVA: 0x001DC76E File Offset: 0x001DA96E
	public void Init(bool canInteract, MapBlockData mapBlock, GraveDisplayData graveDisplayData)
	{
		base.Init(canInteract, mapBlock, null);
		this._graveDisplayData = graveDisplayData;
		this.Refresh();
	}

	// Token: 0x06003AB2 RID: 15026 RVA: 0x001DC78C File Offset: 0x001DA98C
	protected override void Refresh()
	{
		base.Refresh();
		GraveDisplayData graveDisplayData = this._graveDisplayData;
		bool flag = graveDisplayData != null && graveDisplayData.Level != -1;
		if (flag)
		{
			this.iconImage.gameObject.SetActive(true);
			this.RefreshIcon();
			this.RefreshDurability();
		}
		else
		{
			this.iconImage.gameObject.SetActive(false);
			this.durabilityText.text = "";
		}
	}

	// Token: 0x06003AB3 RID: 15027 RVA: 0x001DC807 File Offset: 0x001DAA07
	protected override void RefreshName()
	{
		this.nameLabel.text = ((this._graveDisplayData == null) ? "" : NameCenter.GetMonasticTitleOrDisplayName(this._graveDisplayData, false));
	}

	// Token: 0x06003AB4 RID: 15028 RVA: 0x001DC834 File Offset: 0x001DAA34
	protected override void RefreshOrganization()
	{
		this.organizationLabel.text = ((this._graveDisplayData == null) ? "" : CommonUtils.GetOrganizationGradeString(new OrganizationInfo(this._graveDisplayData.NameData.OrgTemplateId, this._graveDisplayData.NameData.OrgGrade, this._graveDisplayData.Principal, this._graveDisplayData.OrgSettlementId), this._graveDisplayData.NameData.Gender, -1, -1));
	}

	// Token: 0x06003AB5 RID: 15029 RVA: 0x001DC8B0 File Offset: 0x001DAAB0
	public void RefreshAvatar()
	{
		bool flag = this._graveDisplayData != null;
		if (flag)
		{
			SingletonObject.getInstance<CharacterMonitorModel>().RefreshTargetMonitorAliveState(this._graveDisplayData.Id, true);
		}
		this.avatarImage.SetSprite("NPCFace_tomb", false, null);
		this.avatarImage.gameObject.SetActive(true);
	}

	// Token: 0x06003AB6 RID: 15030 RVA: 0x001DC90C File Offset: 0x001DAB0C
	protected override void OnClickButton()
	{
		bool flag = base.IsMoving || this._graveDisplayData == null;
		if (!flag)
		{
			GameDataBridge.AddMethodCall<int>(-1, 12, 28, this._graveDisplayData.Id);
		}
	}

	// Token: 0x06003AB7 RID: 15031 RVA: 0x001DC94A File Offset: 0x001DAB4A
	private void RefreshIcon()
	{
		this.iconImage.SetSprite(MapBlockCharBase2.GetIconGrave((int)this._graveDisplayData.Level), false, null);
	}

	// Token: 0x06003AB8 RID: 15032 RVA: 0x001DC96B File Offset: 0x001DAB6B
	private void RefreshDurability()
	{
		this.durabilityText.text = this._graveDisplayData.Durability.ToString();
	}

	// Token: 0x04002A43 RID: 10819
	[SerializeField]
	protected CImage iconImage;

	// Token: 0x04002A44 RID: 10820
	[SerializeField]
	protected CImage avatarImage;

	// Token: 0x04002A45 RID: 10821
	[SerializeField]
	private TextMeshProUGUI durabilityText;

	// Token: 0x04002A46 RID: 10822
	private GraveDisplayData _graveDisplayData;
}
