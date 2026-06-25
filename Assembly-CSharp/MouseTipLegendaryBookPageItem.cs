using System;
using Config;
using FrameWork;
using Game.Components.Avatar;
using Game.Views.Building;
using Game.Views.LegendaryBook;
using GameData.Domains.LegendaryBook;
using GameData.Domains.Map;
using TMPro;
using UnityEngine;

// Token: 0x020003FE RID: 1022
public class MouseTipLegendaryBookPageItem : MouseTipBase
{
	// Token: 0x06003D21 RID: 15649 RVA: 0x001EC0C8 File Offset: 0x001EA2C8
	protected override void Init(ArgumentBox argsBox)
	{
		argsBox.Get<LegendaryBookIncrementData>("bookData", out this._bookData);
		argsBox.Get<int[]>("bookOwners", out this._bookOwners);
		sbyte currType;
		argsBox.Get("currType", out currType);
		this.characterPanel.SetActive(false);
		this.adventurePanel.SetActive(false);
		this.unknownPanel.SetActive(false);
		bool flag = this._bookOwners[(int)currType] >= 0;
		if (flag)
		{
			this.UpdateCharacterTips(currType);
			this.MapBlockViewRect.position = this.pivotUp.position;
		}
		else
		{
			Location location;
			bool flag2 = this._bookData.BookLocationMap.TryGetValue(currType, out location);
			if (flag2)
			{
				this.UpdateAdventureTips(currType);
				this.MapBlockViewRect.position = this.pivotUp.position;
			}
			else
			{
				int num;
				bool flag3 = this._bookData.BookDurationMap.TryGetValue(currType, out num);
				if (flag3)
				{
					this.UpdateUnknownTips(currType);
					this.MapBlockViewRect.position = this.pivotDown.position;
				}
			}
		}
	}

	// Token: 0x06003D22 RID: 15650 RVA: 0x001EC1D8 File Offset: 0x001EA3D8
	protected override void OnDisable()
	{
		base.OnDisable();
		this.characterPanel.SetActive(false);
		this.adventurePanel.SetActive(false);
		this.unknownPanel.SetActive(false);
	}

	// Token: 0x06003D23 RID: 15651 RVA: 0x001EC20C File Offset: 0x001EA40C
	private void UpdateCharacterTips(sbyte currBookType)
	{
		this.characterPanel.SetActive(true);
		LegendaryBookCharacterRelatedData characterData = this._bookData.CharacterMap[this._bookOwners[(int)currBookType]];
		OrganizationItem orgConfig = Organization.Instance[characterData.OrganizationInfo.OrgTemplateId];
		this.organizationProp.Set(CommonUtils.GetOrganizationIcon((short)characterData.OrganizationInfo.OrgTemplateId), LanguageKey.LK_Main_SummaryInfo_Organization.Tr(), orgConfig.Name);
		this.identityProp.Set(CommonUtils.GetIdentityIconByLevel(characterData.OrganizationInfo.Grade), LanguageKey.LK_Main_SummaryInfo_Identity.Tr(), CommonUtils.GetIdentityStringWithSpecialCharacterConfig((int)characterData.CharacterTemplateId, characterData.OrganizationInfo, characterData.Gender, characterData.PhysiologicalAge, false));
		this.fameProp.Set(CommonUtils.GetFameIconName(characterData.FameType), LanguageKey.LK_Main_SummaryInfo_Fame.Tr(), CommonUtils.GetFameString(characterData.FameType));
		this.happinessProp.Set(CommonUtils.GetHappinessIconName(characterData.HappinessType), LanguageKey.LK_Main_SummaryInfo_Happiness.Tr(), CommonUtils.GetHappinessString(characterData.HappinessType));
		string consummateIcon;
		string consummateText = CommonUtils.GetConsummateLevelShowDataFull(characterData.ConsummateLevel, out consummateIcon);
		this.consummateProp.Set(consummateIcon, LanguageKey.LK_Mousetip_Sort_Desc_ConsummateLevel.Tr(), consummateText);
		string ageValueText = LocalStringManager.GetFormat(LanguageKey.LK_LegendaryBook_CharacterAge, characterData.PhysiologicalAge);
		this.characterName.text = NameCenter.GetMonasticTitleOrDisplayName(ref characterData.NameRelatedData, false, false) + LocalStringManager.GetFormat(LanguageKey.UI_NewGame_BornDateInfo, ageValueText);
		this.avatar.Refresh(characterData.AvatarRelatedData);
		this.title.text = LocalStringManager.Get(string.Format("LK_LegendaryBook_{0}", currBookType));
		this.skillIcon.SetSprite(string.Format("{0}{1}", "ui9_back_mousetip_img_combatskill_0_", currBookType), false, null);
		this.typeName.text = CombatSkillType.Instance[currBookType].Name.SetColor("legendbook");
		this.typeIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_attainments_big_0_", currBookType), false, null);
		this.SetBlockData(currBookType);
	}

	// Token: 0x06003D24 RID: 15652 RVA: 0x001EC42C File Offset: 0x001EA62C
	private void UpdateAdventureTips(sbyte currBookType)
	{
		Refers refers = this.adventurePanel.GetComponent<Refers>();
		int duration = this._bookData.BookDurationMap[currBookType];
		this.skillIcon.SetSprite(string.Format("{0}{1}", "ui9_back_mousetip_img_combatskill_0_", currBookType), false, null);
		this.typeName.text = CombatSkillType.Instance[currBookType].Name.SetColor("legendbook");
		this.typeIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_attainments_big_0_", currBookType), false, null);
		this.title.text = LocalStringManager.Get(string.Format("LK_LegendaryBook_{0}", currBookType));
		refers.CGet<TextMeshProUGUI>("Month").text = duration.ToString();
		refers.CGet<TextMeshProUGUI>("AdventureName").text = Adventure.Instance[ViewLegendaryBook.LegendaryBookAdventures[(int)currBookType]].Name;
		this.SetBlockData(currBookType);
		this.adventurePanel.SetActive(true);
	}

	// Token: 0x06003D25 RID: 15653 RVA: 0x001EC53C File Offset: 0x001EA73C
	private void UpdateUnknownTips(sbyte currBookType)
	{
		int duration = this._bookData.BookDurationMap[currBookType];
		this.skillIcon.SetSprite(string.Format("{0}{1}", "ui9_back_mousetip_img_combatskill_0_", currBookType), false, null);
		this.typeName.text = CombatSkillType.Instance[currBookType].Name.SetColor("legendbook");
		this.typeIcon.SetSprite(string.Format("{0}{1}", "ui9_icon_attainments_big_0_", currBookType), false, null);
		this.title.text = LocalStringManager.Get(string.Format("LK_LegendaryBook_{0}", currBookType));
		this.locationName.text = LocalStringManager.Get(LanguageKey.LK_LegendaryBook_Tips_UnknownArea);
		this.mapBlockView.gameObject.SetActive(false);
		this.unknownArea.SetActive(true);
		this.unknownPanel.GetComponent<Refers>().CGet<TextMeshProUGUI>("Month").text = duration.ToString();
		this.unknownPanel.SetActive(true);
	}

	// Token: 0x06003D26 RID: 15654 RVA: 0x001EC650 File Offset: 0x001EA850
	private void SetBlockData(sbyte currBookType)
	{
		MapBlockData blockData;
		bool flag = this._bookData.BlockDataMap.TryGetValue(currBookType, out blockData);
		if (flag)
		{
			FullBlockName blockNameData = this._bookData.BlockNameDataMap[currBookType];
			this.mapBlockView.Refresh(blockData, blockData);
			this.mapBlockView.gameObject.SetActive(true);
			this.unknownArea.SetActive(false);
			this.locationName.text = SingletonObject.getInstance<WorldMapModel>().GetFullBlockName(blockNameData, true, true, true, true);
		}
		else
		{
			this.locationName.text = LocalStringManager.Get(LanguageKey.LK_LegendaryBook_Tips_UnknownArea);
			this.mapBlockView.gameObject.SetActive(false);
			this.unknownArea.SetActive(true);
		}
	}

	// Token: 0x04002BDD RID: 11229
	[SerializeField]
	private GameObject characterPanel;

	// Token: 0x04002BDE RID: 11230
	[SerializeField]
	private GameObject adventurePanel;

	// Token: 0x04002BDF RID: 11231
	[SerializeField]
	private GameObject unknownPanel;

	// Token: 0x04002BE0 RID: 11232
	[SerializeField]
	private GameObject unknownArea;

	// Token: 0x04002BE1 RID: 11233
	[SerializeField]
	private MapBlockView mapBlockView;

	// Token: 0x04002BE2 RID: 11234
	[SerializeField]
	private TextMeshProUGUI locationName;

	// Token: 0x04002BE3 RID: 11235
	[SerializeField]
	private TextMeshProUGUI title;

	// Token: 0x04002BE4 RID: 11236
	[Header("类别")]
	[SerializeField]
	private CImage skillIcon;

	// Token: 0x04002BE5 RID: 11237
	[SerializeField]
	private TextMeshProUGUI typeName;

	// Token: 0x04002BE6 RID: 11238
	[SerializeField]
	private CImage typeIcon;

	// Token: 0x04002BE7 RID: 11239
	[Header("Character")]
	[SerializeField]
	private Game.Components.Avatar.Avatar avatar;

	// Token: 0x04002BE8 RID: 11240
	[SerializeField]
	private TextMeshProUGUI characterName;

	// Token: 0x04002BE9 RID: 11241
	[SerializeField]
	private ComponentIconTitleValue organizationProp;

	// Token: 0x04002BEA RID: 11242
	[SerializeField]
	private ComponentIconTitleValue identityProp;

	// Token: 0x04002BEB RID: 11243
	[SerializeField]
	private ComponentIconTitleValue fameProp;

	// Token: 0x04002BEC RID: 11244
	[SerializeField]
	private ComponentIconTitleValue happinessProp;

	// Token: 0x04002BED RID: 11245
	[SerializeField]
	private ComponentIconTitleValue consummateProp;

	// Token: 0x04002BEE RID: 11246
	[Header("类别")]
	[SerializeField]
	private RectTransform MapBlockViewRect;

	// Token: 0x04002BEF RID: 11247
	[SerializeField]
	private RectTransform pivotDown;

	// Token: 0x04002BF0 RID: 11248
	[SerializeField]
	private RectTransform pivotUp;

	// Token: 0x04002BF1 RID: 11249
	private LegendaryBookIncrementData _bookData;

	// Token: 0x04002BF2 RID: 11250
	private int[] _bookOwners;
}
