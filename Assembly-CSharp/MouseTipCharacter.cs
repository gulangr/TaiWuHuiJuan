using System;
using System.Collections.Generic;
using CharacterDataMonitor;
using Config;
using DG.Tweening;
using FrameWork;
using Game.Components.Avatar;
using Game.Views.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Alertness;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020003FC RID: 1020
public class MouseTipCharacter : MouseTipBase
{
	// Token: 0x17000636 RID: 1590
	// (get) Token: 0x06003D10 RID: 15632 RVA: 0x001EB387 File Offset: 0x001E9587
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06003D11 RID: 15633 RVA: 0x001EB38C File Offset: 0x001E958C
	protected override void Init(ArgumentBox argsBox)
	{
		this.InitPropItems();
		this._characterMonitorModel = SingletonObject.getInstance<CharacterMonitorModel>();
		argsBox.Get("isDreamBack", out this._isDreamBack);
		argsBox.Get("ShowDeadAsAlive", out this._showDeadAsAlive);
		AvatarRelatedData avatarRelatedData;
		bool flag = argsBox.Get<AvatarRelatedData>("avatar", out avatarRelatedData);
		if (flag)
		{
			string charName;
			argsBox.Get("charName", out charName);
			this._isLocationShown = false;
			this._charId = -1;
			this.NeedWaitData = false;
			this.ShowAvatarOnly(charName, avatarRelatedData, -1);
		}
		else
		{
			argsBox.Get("charId", out this._charId);
			this._avatarInfoMonitor = this._characterMonitorModel.GetMonitorItem<AvatarInfoMonitor>(this._charId, false);
			bool flag2 = !argsBox.Get("locationShow", out this._isLocationShown);
			if (flag2)
			{
				this._isLocationShown = false;
			}
			this.NeedWaitData = true;
			bool isDreamBack = this._isDreamBack;
			if (isDreamBack)
			{
				ExtraDomainMethod.AsyncCall.GetDreamBackCharacterDisplayDataList(this, new List<int>
				{
					this._charId
				}, new AsyncMethodCallbackDelegate(this.OnGetCharDisplayData));
			}
			else
			{
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, new List<int>
				{
					this._charId
				}, new AsyncMethodCallbackDelegate(this.OnGetCharDisplayData));
				if (this._characterAvatar == null)
				{
					this._characterAvatar = new CharacterAvatar(this.avatar, !this._showDeadAsAlive);
				}
				this._characterAvatar.CharacterId = this._charId;
			}
		}
		this.locationRoot.SetActive(this._isLocationShown);
		this.locationRoot.GetComponent<CanvasGroup>().alpha = 0f;
	}

	// Token: 0x06003D12 RID: 15634 RVA: 0x001EB520 File Offset: 0x001E9720
	private void InitPropItems()
	{
		CommonUtils.PrepareEnoughChildren(this.propLayout, this.propTemplate.gameObject, 12, null);
		this._propItems = new ComponentIconTitleValue[12];
		for (int i = 0; i < 12; i++)
		{
			this._propItems[i] = this.propLayout.GetChild(i).GetComponent<ComponentIconTitleValue>();
		}
	}

	// Token: 0x06003D13 RID: 15635 RVA: 0x001EB587 File Offset: 0x001E9787
	private void SetProp(MouseTipCharacter.PropType type, string icon, string title, string value)
	{
		this._propItems[(int)type].Set(icon, title, value);
	}

	// Token: 0x06003D14 RID: 15636 RVA: 0x001EB59C File Offset: 0x001E979C
	private void SetPropValueOnly(MouseTipCharacter.PropType type, string title, string value)
	{
		this._propItems[(int)type].SetValue(title, value);
	}

	// Token: 0x06003D15 RID: 15637 RVA: 0x001EB5B0 File Offset: 0x001E97B0
	private void SetAllPropsUnknown(string unknownStr = "-")
	{
		Debug.Log(string.Format("MouseTipCharacter test SetAllPropsUnknown; _isDreamBack:{0}", this._isDreamBack));
		this.SetProp(MouseTipCharacter.PropType.Title, string.Empty, LanguageKey.LK_Main_SummaryInfo_Title.Tr(), unknownStr);
		this.SetProp(MouseTipCharacter.PropType.Gender, "taiwuevent_01_wenhao_0", LanguageKey.LK_Main_SummaryInfo_Gender.Tr(), unknownStr);
		this.SetProp(MouseTipCharacter.PropType.Age, string.Empty, LanguageKey.LK_Char_Age.Tr(), unknownStr);
		this.SetProp(MouseTipCharacter.PropType.Health, "taiwuevent_01_wenhao_0", LanguageKey.LK_Health.Tr(), unknownStr);
		this.SetProp(MouseTipCharacter.PropType.Charm, "taiwuevent_01_wenhao_0", LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), unknownStr);
		this.SetProp(MouseTipCharacter.PropType.Behavior, "taiwuevent_01_wenhao_0", LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), unknownStr);
		this.SetPropValueOnly(MouseTipCharacter.PropType.Organization, LanguageKey.LK_Main_SummaryInfo_Organization.Tr(), unknownStr);
		this.SetProp(MouseTipCharacter.PropType.Identity, "taiwuevent_01_wenhao_0", LanguageKey.LK_Main_SummaryInfo_Identity.Tr(), unknownStr);
		this.SetProp(MouseTipCharacter.PropType.Fame, "taiwuevent_01_wenhao_0", LanguageKey.LK_Main_SummaryInfo_Fame.Tr(), unknownStr);
		this.SetProp(MouseTipCharacter.PropType.Favorability, "taiwuevent_01_wenhao_0", LanguageKey.LK_Favorability.Tr(), unknownStr);
		this.SetProp(MouseTipCharacter.PropType.Alertness, "taiwuevent_01_wenhao_0", LanguageKey.LK_Alertness.Tr(), unknownStr);
		this.SetProp(MouseTipCharacter.PropType.Samsara, "ui9_icon_samsara_big", LanguageKey.LK_Samsara.Tr(), unknownStr);
	}

	// Token: 0x06003D16 RID: 15638 RVA: 0x001EB6F8 File Offset: 0x001E98F8
	private void ShowAvatarOnly(string charName, AvatarRelatedData avatarRelatedData, short characterTemplateId = -1)
	{
		string nameStr = string.IsNullOrEmpty(charName) ? "-" : charName;
		this.titleText.text = nameStr;
		this.nameText.text = nameStr;
		bool flag = characterTemplateId >= 0;
		if (flag)
		{
			this.avatar.Refresh(avatarRelatedData, characterTemplateId);
		}
		else
		{
			this.avatar.Refresh(avatarRelatedData);
		}
		this.SetAllPropsUnknown("-");
	}

	// Token: 0x06003D17 RID: 15639 RVA: 0x001EB768 File Offset: 0x001E9968
	private void OnGetCharDisplayData(int offset, RawDataPool dataPool)
	{
		List<CharacterDisplayData> charDisplayDataList = EasyPool.Get<List<CharacterDisplayData>>();
		charDisplayDataList.Clear();
		Serializer.Deserialize(dataPool, offset, ref charDisplayDataList);
		Assert.AreEqual(charDisplayDataList.Count, 1);
		this._displayData = charDisplayDataList[0];
		int birthMonth = this._displayData.BirthDate % 12;
		bool flag = birthMonth < 0;
		if (flag)
		{
			birthMonth += 12;
		}
		string charName = NameCenter.GetMonasticTitleOrDisplayName(this._displayData, this._charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
		MonthItem monthConfig = Month.Instance[birthMonth];
		this.titleText.text = charName;
		this.nameText.text = charName;
		bool isNonEvolutionaryType = CreatingType.IsNonEvolutionaryType(this._avatarInfoMonitor.CreatingType);
		string ageStr = (this._displayData.AliveState == 0 && (!Character.Instance[this._displayData.TemplateId].HideAge || !isNonEvolutionaryType)) ? LocalStringManager.GetFormat(LanguageKey.LK_Age, this._displayData.PhysiologicalAge) : "-";
		CommonUtils.EDisplayGender displayGender = CommonUtils.GetDisplayGender(this._displayData.Gender, this._displayData.TemplateId);
		List<short> titleIds = this._displayData.TitleIds;
		string titleStr = (titleIds != null && titleIds.Count > 0) ? CharacterTitle.Instance[this._displayData.TitleIds[0]].Name : LocalStringManager.Get(LanguageKey.LK_None);
		this.SetProp(MouseTipCharacter.PropType.Title, "ui9_back_mousetip_icon_title_0", LanguageKey.LK_Main_SummaryInfo_Title.Tr(), titleStr);
		this.SetProp(MouseTipCharacter.PropType.Gender, CommonUtils.GetGenderIconBig(displayGender), LanguageKey.LK_Main_SummaryInfo_Gender.Tr(), CommonUtils.GetGenderString(displayGender));
		this.SetProp(MouseTipCharacter.PropType.Age, "ui9_back_mousetip_age_0", LanguageKey.LK_Char_Age.Tr(), ageStr);
		this.SetProp(MouseTipCharacter.PropType.Behavior, CommonUtils.GetBehaviorTypeIcon(this._displayData.BehaviorType), LanguageKey.LK_Main_SummaryInfo_Behavior.Tr(), CommonUtils.GetBehaviorString(this._displayData.BehaviorType));
		this.SetProp(MouseTipCharacter.PropType.Organization, CommonUtils.GetOrganizationIcon((short)this._displayData.OrgInfo.OrgTemplateId), LanguageKey.LK_Main_SummaryInfo_Organization.Tr(), SingletonObject.getInstance<WorldMapModel>().GetSettlementName(this._displayData.OrgInfo));
		this.SetProp(MouseTipCharacter.PropType.Identity, CommonUtils.GetIdentityIconByLevel(this._displayData.OrgInfo.Grade), LanguageKey.LK_Main_SummaryInfo_Identity.Tr(), CommonUtils.GetIdentityStringWithSpecialCharacterConfig((int)this._displayData.TemplateId, this._displayData.OrgInfo, this._displayData.Gender, this._displayData.PhysiologicalAge, false));
		this.SetProp(MouseTipCharacter.PropType.Alertness, CommonUtils.GetAlertnessIcon((int)CharacterAlertnessData.GetLevel(this._displayData.Alertness)), LanguageKey.LK_Alertness.Tr(), CommonUtils.GetAlertnessNameByValue(this._displayData.Alertness));
		this.SetProp(MouseTipCharacter.PropType.Health, "taiwuevent_01_wenhao_0", LanguageKey.LK_Health.Tr(), "-");
		this.SetProp(MouseTipCharacter.PropType.Charm, "taiwuevent_01_wenhao_0", LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), "-");
		this.SetProp(MouseTipCharacter.PropType.Fame, "taiwuevent_01_wenhao_0", LanguageKey.LK_Main_SummaryInfo_Fame.Tr(), "-");
		this.SetProp(MouseTipCharacter.PropType.Favorability, "taiwuevent_01_wenhao_0", LanguageKey.LK_Favorability.Tr(), "-");
		this.SetProp(MouseTipCharacter.PropType.Samsara, "ui9_icon_samsara_big", LanguageKey.LK_Samsara.Tr(), "-");
		bool isDead = this._displayData.AliveState == 1;
		bool isDreamBack = this._isDreamBack;
		if (isDreamBack)
		{
			this.ShowAvatarOnly(charName, this._displayData.AvatarRelatedData, this._displayData.TemplateId);
		}
		else
		{
			CharacterAvatar characterAvatar = this._characterAvatar;
			if (characterAvatar != null)
			{
				characterAvatar.SetIsDead(isDead);
			}
		}
		bool isLocationShown = this._isLocationShown;
		if (isLocationShown)
		{
			this.RefreshLocationDisplay(isDead);
		}
		bool flag2 = !this._isDreamBack;
		if (flag2)
		{
			bool flag3 = this._displayData.AliveState == 0;
			if (flag3)
			{
				CharacterDomainMethod.AsyncCall.GetGroupCharDisplayDataList(this, new List<int>
				{
					this._charId
				}, new AsyncMethodCallbackDelegate(this.OnGetGroupCharDisplayData));
			}
			else
			{
				bool flag4 = isDead;
				if (flag4)
				{
					CharacterDomainMethod.AsyncCall.TryGetDeadCharacter(this, this._charId, new AsyncMethodCallbackDelegate(this.OnGetDeadCharacter));
				}
			}
		}
	}

	// Token: 0x06003D18 RID: 15640 RVA: 0x001EBB88 File Offset: 0x001E9D88
	private void RefreshLocationDisplay(bool isDead)
	{
		this.locationLabel.text = string.Empty;
		bool flag = ExternalRelationStateHelper.IsActive(this._displayData.ExternalRelationState, 8UL);
		if (flag)
		{
			this.locationLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Character_Location_Format_StoneHouse, Organization.Instance[16].Name);
			this.locationLabel.text = "<color=#ffea8d>" + this.locationLabel.text + "</color>";
			this.locationRoot.GetComponent<CanvasGroup>().DOFade(1f, 0.5f);
		}
		else
		{
			Location location = this._displayData.Location;
			bool flag2 = ExternalRelationStateHelper.IsActive(this._displayData.ExternalRelationState, 16UL);
			if (flag2)
			{
				location = Location.Invalid;
			}
			MapDomainMethod.AsyncCall.GetBlockFullName(null, location, delegate(int offsetData, RawDataPool poolData)
			{
				FullBlockName fullBlockName = default(FullBlockName);
				Serializer.Deserialize(poolData, offsetData, ref fullBlockName);
				string blockName = SingletonObject.getInstance<WorldMapModel>().GetFullBlockName(fullBlockName, true, true, true, true);
				string locationStr = MouseTipCharacter.GetLocationString(blockName, isDead);
				this.locationLabel.text = "<color=#ffea8d>" + locationStr + "</color>";
				this.locationRoot.GetComponent<CanvasGroup>().DOFade(1f, 0.5f);
			});
		}
	}

	// Token: 0x06003D19 RID: 15641 RVA: 0x001EBC7C File Offset: 0x001E9E7C
	private static string GetLocationString(string blockName, bool isDead)
	{
		bool flag = string.IsNullOrEmpty(blockName);
		string result;
		if (flag)
		{
			result = (isDead ? LocalStringManager.Get(LanguageKey.LK_Character_Location_Format_Dead_Invalid) : LocalStringManager.Get(LanguageKey.LK_Character_Location_Format_Invalid));
		}
		else
		{
			result = (isDead ? LocalStringManager.GetFormat(LanguageKey.LK_Character_Location_Format_Dead, blockName) : LocalStringManager.GetFormat(LanguageKey.LK_Character_Location_Format_Normal, blockName));
		}
		return result;
	}

	// Token: 0x06003D1A RID: 15642 RVA: 0x001EBCD0 File Offset: 0x001E9ED0
	private void OnGetGroupCharDisplayData(int offset, RawDataPool dataPool)
	{
		List<GroupCharDisplayData> dataList = null;
		Serializer.Deserialize(dataPool, offset, ref dataList);
		GroupCharDisplayData groupCharData = dataList[0];
		EHealthType healthType = CommonUtils.GetHealthType(groupCharData.Health, groupCharData.MaxLeftHealth, this._charId);
		this.SetProp(MouseTipCharacter.PropType.Health, CommonUtils.GetHealthIcon(healthType), LanguageKey.LK_Health.Tr(), CommonUtils.GetHealthString(healthType));
		CharacterItem charConfig = Character.Instance.GetItem(this._displayData.TemplateId);
		bool isFixedPresetType = CreatingType.IsFixedPresetType(charConfig.CreatingType);
		this.SetProp(MouseTipCharacter.PropType.Charm, CommonUtils.GetCharmLevelBigIcon(groupCharData.Charm, groupCharData.PhysiologicalAge, groupCharData.ClothDisplayId, groupCharData.FaceVisible, isFixedPresetType), LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), CommonUtils.GetCharmLevelText(groupCharData.Charm, this._displayData.Gender, groupCharData.PhysiologicalAge, groupCharData.ClothDisplayId, isFixedPresetType, groupCharData.FaceVisible));
		this.SetProp(MouseTipCharacter.PropType.Favorability, CommonUtils.GetFavorabilityIconName(groupCharData.FavorabilityToTaiwu, groupCharData.IsInteractedWithTaiwu), LanguageKey.LK_Favorability.Tr(), CommonUtils.GetFavorStringByInteracted(groupCharData.FavorabilityToTaiwu, groupCharData.IsInteractedWithTaiwu));
		sbyte fameType = FameType.GetFameType(groupCharData.Fame);
		this.SetProp(MouseTipCharacter.PropType.Fame, CommonUtils.GetFameIconName(fameType), LanguageKey.LK_Main_SummaryInfo_Fame.Tr(), CommonUtils.GetFameString(fameType));
		this.SetProp(MouseTipCharacter.PropType.Alertness, CommonUtils.GetAlertnessIcon((int)CharacterAlertnessData.GetLevel(groupCharData.Alertness)), LanguageKey.LK_Alertness.Tr(), CommonUtils.GetAlertnessNameByValue(groupCharData.Alertness));
		this.SetProp(MouseTipCharacter.PropType.Samsara, "ui9_icon_samsara_big", LanguageKey.LK_Samsara.Tr(), groupCharData.PreexistenceCharCount.ToString());
		UIElement element = this.Element;
		if (element != null)
		{
			element.ShowAfterRefresh();
		}
	}

	// Token: 0x06003D1B RID: 15643 RVA: 0x001EBE6C File Offset: 0x001EA06C
	private void OnGetDeadCharacter(int offset, RawDataPool dataPool)
	{
		DeadCharacter deadCharacter = null;
		Serializer.Deserialize(dataPool, offset, ref deadCharacter);
		bool flag = deadCharacter == null;
		if (flag)
		{
			throw new Exception(string.Format("Can't get info of {0} as a dead character", this._charId));
		}
		this.SetProp(MouseTipCharacter.PropType.Health, string.Empty, LanguageKey.LK_Health.Tr(), "-");
		CharacterItem charConfig = Character.Instance.GetItem(this._displayData.TemplateId);
		bool isFixedPresetType = CreatingType.IsFixedPresetType(charConfig.CreatingType);
		this.SetProp(MouseTipCharacter.PropType.Charm, CommonUtils.GetCharmLevelBigIcon(deadCharacter.Attraction, deadCharacter.GetActualAge(), deadCharacter.ClothingDisplayId, deadCharacter.Avatar.FaceVisible, isFixedPresetType), LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), CommonUtils.GetCharmLevelText(deadCharacter.Attraction, this._displayData.Gender, deadCharacter.GetActualAge(), deadCharacter.ClothingDisplayId, isFixedPresetType, deadCharacter.Avatar.FaceVisible));
		this.SetProp(MouseTipCharacter.PropType.Favorability, CommonUtils.GetFavorabilityIconName(0, false), LanguageKey.LK_Favorability.Tr(), "-");
		this.SetProp(MouseTipCharacter.PropType.Fame, CommonUtils.GetFameIconName(deadCharacter.FameType), LanguageKey.LK_Main_SummaryInfo_Fame.Tr(), CommonUtils.GetFameString(deadCharacter.FameType));
		this.SetProp(MouseTipCharacter.PropType.Alertness, string.Empty, LanguageKey.LK_Alertness.Tr(), "-");
		this.SetProp(MouseTipCharacter.PropType.Samsara, "ui9_icon_samsara_big", LanguageKey.LK_Samsara.Tr(), deadCharacter.PreexistenceCharIds.Count.ToString());
		UIElement element = this.Element;
		if (element != null)
		{
			element.ShowAfterRefresh();
		}
	}

	// Token: 0x06003D1C RID: 15644 RVA: 0x001EBFE8 File Offset: 0x001EA1E8
	protected override void OnDisable()
	{
		base.OnDisable();
		bool flag = this._characterAvatar != null;
		if (flag)
		{
			this._characterAvatar.CharacterId = -1;
		}
	}

	// Token: 0x06003D1D RID: 15645 RVA: 0x001EC017 File Offset: 0x001EA217
	public override void OnSticked()
	{
		CharacterAvatar characterAvatar = this._characterAvatar;
		if (characterAvatar != null)
		{
			characterAvatar.UnbindEvent();
		}
	}

	// Token: 0x04002BC7 RID: 11207
	[SerializeField]
	private Game.Components.Avatar.Avatar avatar;

	// Token: 0x04002BC8 RID: 11208
	[SerializeField]
	private TextMeshProUGUI titleText;

	// Token: 0x04002BC9 RID: 11209
	[SerializeField]
	private TextMeshProUGUI nameText;

	// Token: 0x04002BCA RID: 11210
	[SerializeField]
	private GameObject locationRoot;

	// Token: 0x04002BCB RID: 11211
	[SerializeField]
	private TextMeshProUGUI locationLabel;

	// Token: 0x04002BCC RID: 11212
	[SerializeField]
	private Transform propLayout;

	// Token: 0x04002BCD RID: 11213
	[SerializeField]
	private ComponentIconTitleValue propTemplate;

	// Token: 0x04002BCE RID: 11214
	private const int PropCount = 12;

	// Token: 0x04002BCF RID: 11215
	private const string UnknownIcon = "taiwuevent_01_wenhao_0";

	// Token: 0x04002BD0 RID: 11216
	private CharacterMonitorModel _characterMonitorModel;

	// Token: 0x04002BD1 RID: 11217
	private CharacterDisplayData _displayData;

	// Token: 0x04002BD2 RID: 11218
	private int _charId;

	// Token: 0x04002BD3 RID: 11219
	private bool _isLocationShown;

	// Token: 0x04002BD4 RID: 11220
	private AvatarInfoMonitor _avatarInfoMonitor;

	// Token: 0x04002BD5 RID: 11221
	private CharacterAvatar _characterAvatar;

	// Token: 0x04002BD6 RID: 11222
	private bool _showDeadAsAlive;

	// Token: 0x04002BD7 RID: 11223
	private bool _isDreamBack;

	// Token: 0x04002BD8 RID: 11224
	private ComponentIconTitleValue[] _propItems;

	// Token: 0x02001886 RID: 6278
	private enum PropType
	{
		// Token: 0x0400AEC8 RID: 44744
		Title,
		// Token: 0x0400AEC9 RID: 44745
		Gender,
		// Token: 0x0400AECA RID: 44746
		Age,
		// Token: 0x0400AECB RID: 44747
		Health,
		// Token: 0x0400AECC RID: 44748
		Charm,
		// Token: 0x0400AECD RID: 44749
		Behavior,
		// Token: 0x0400AECE RID: 44750
		Organization,
		// Token: 0x0400AECF RID: 44751
		Identity,
		// Token: 0x0400AED0 RID: 44752
		Fame,
		// Token: 0x0400AED1 RID: 44753
		Favorability,
		// Token: 0x0400AED2 RID: 44754
		Alertness,
		// Token: 0x0400AED3 RID: 44755
		Samsara
	}
}
