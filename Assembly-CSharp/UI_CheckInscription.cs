using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Global.Inscription;
using GameData.GameDataBridge;
using TMPro;
using UICommon.Character;
using UICommon.Character.Elements;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000222 RID: 546
public class UI_CheckInscription : UIBase
{
	// Token: 0x17000367 RID: 871
	// (get) Token: 0x060022C2 RID: 8898 RVA: 0x0010103C File Offset: 0x000FF23C
	private GameObject _rightPanel
	{
		get
		{
			return base.CGet<GameObject>("InfoScrollView");
		}
	}

	// Token: 0x17000368 RID: 872
	// (get) Token: 0x060022C3 RID: 8899 RVA: 0x00101049 File Offset: 0x000FF249
	private GameObject _bottomBar
	{
		get
		{
			return base.CGet<GameObject>("BottomBar");
		}
	}

	// Token: 0x17000369 RID: 873
	// (get) Token: 0x060022C4 RID: 8900 RVA: 0x00101056 File Offset: 0x000FF256
	private TextMeshProUGUI _neiliTypeTxt
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("NeiliType");
		}
	}

	// Token: 0x1700036A RID: 874
	// (get) Token: 0x060022C5 RID: 8901 RVA: 0x00101063 File Offset: 0x000FF263
	private CToggleGroupObsolete _subpageToggleGroup
	{
		get
		{
			return base.CGet<CToggleGroupObsolete>("SubpageToggleGroup");
		}
	}

	// Token: 0x060022C6 RID: 8902 RVA: 0x00101070 File Offset: 0x000FF270
	public override void OnLanguageChange(LocalStringManager.LanguageType languageType)
	{
		base.OnLanguageChange(languageType);
		bool flag = this._characterFeatureScroll == null;
		if (flag)
		{
			this.InitCharacterFeatureScroll();
		}
		CharacterFeatureFixedScroll characterFeatureScroll = this._characterFeatureScroll;
		if (characterFeatureScroll != null)
		{
			characterFeatureScroll.OnLanguageChange(languageType);
		}
	}

	// Token: 0x060022C7 RID: 8903 RVA: 0x001010B0 File Offset: 0x000FF2B0
	private void InitCharacterFeatureScroll()
	{
		this._characterFeatureScroll = new CharacterFeatureFixedScroll(base.CGet<RectTransform>("FeatureLayoutCN").transform, base.CGet<RectTransform>("FeatureLayoutEN").transform, false, new CharacterFeatureFixedScroll.ScrollItemTemplateGroup?(new CharacterFeatureFixedScroll.ScrollItemTemplateGroup(base.CGet<CharacterFeatureView>("CharacterFeatureView"), base.CGet<CharacterFeatureView>("CharacterFeatureViewWide"))));
	}

	// Token: 0x060022C8 RID: 8904 RVA: 0x0010110C File Offset: 0x000FF30C
	public override void OnInit(ArgumentBox argsBox)
	{
		bool allowConfirm = false;
		bool flag = argsBox != null;
		if (flag)
		{
			argsBox.Get("AllowConfirm", out allowConfirm);
			argsBox.Get("ShowNone", out this._showNone);
			argsBox.Get("CanDelete", out this._canDelete);
			argsBox.Get("IsSettingInclude", out this._isSettingInclude);
			bool flag2 = !argsBox.Get<InscribedCharacterKey>("PreSelectedCharKey", out this._preSelectedCharKey);
			if (flag2)
			{
				this._preSelectedCharKey = InscribedCharacterKey.Invalid;
			}
			this._currentSelectedInscribedCharKey = this._preSelectedCharKey;
		}
		this.CloseButton.gameObject.SetActive(!allowConfirm);
		this.CharsScroll.OnItemRender = new Action<int, Refers>(this.OnRender);
		this.InitCharacterFeatureScroll();
		this.InitIncludeForCreation();
		this.RefreshInscription(null);
		this.RefreshInfoScroll();
		this.RefreshConfirmButtonState();
		this._subpageToggleGroup.InitPreOnToggle(0);
		this.CharsScroll.GetComponent<CScrollRectLegacy>().Content.anchoredPosition = Vector2.zero;
	}

	// Token: 0x060022C9 RID: 8905 RVA: 0x00101214 File Offset: 0x000FF414
	private void InitIncludeForCreation()
	{
		base.CGet<GameObject>("IncludeRoot").SetActive(this._isSettingInclude);
		bool flag = !this._isSettingInclude;
		if (!flag)
		{
			this._currentSelectedInscribedCharKey = InscribedCharacterKey.Invalid;
			this._currentSelectedInscribedChar = null;
			bool flag2 = this._includedInscribedCharList.Contains(this._preSelectedCharKey);
			if (flag2)
			{
				this._includedInscribedCharList.Remove(this._preSelectedCharKey);
			}
			this._tempIncludedInscribedCharList.Clear();
			this._tempIncludedInscribedCharList.AddRange(this._includedInscribedCharList);
			this.RefreshIncludedCount();
			CToggleObsolete autoIncludeToggle = base.CGet<CToggleObsolete>("AutoIncludeToggle");
			autoIncludeToggle.gameObject.SetActive(this._isSettingInclude);
			bool isSettingInclude = this._isSettingInclude;
			if (isSettingInclude)
			{
				autoIncludeToggle.onValueChanged.RemoveAllListeners();
				autoIncludeToggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					Transform autoIncludeSelectedMark = autoIncludeToggle.transform.Find("Selected");
					autoIncludeSelectedMark.gameObject.SetActive(isOn);
					EventSystem.current.SetSelectedGameObject(null);
				});
				autoIncludeToggle.onValueChanged.Invoke(autoIncludeToggle.isOn);
				UI_CheckInscription.OnConfirmClicked = delegate(InscribedCharacterKey _, InscribedCharacter _)
				{
					this._includedInscribedCharList.Clear();
					this._includedInscribedCharList.AddRange(this._tempIncludedInscribedCharList);
					this._canIncludeInscribedCharList.Clear();
					foreach (KeyValuePair<InscribedCharacterKey, InscribedCharacter> keyValuePair in this._inscribedChars)
					{
						InscribedCharacterKey inscribedCharacterKey;
						InscribedCharacter inscribedCharacter;
						keyValuePair.Deconstruct(out inscribedCharacterKey, out inscribedCharacter);
						InscribedCharacterKey key = inscribedCharacterKey;
						bool flag3 = key.Equals(InscribedCharacterKey.Invalid) || key.Equals(this._preSelectedCharKey);
						if (!flag3)
						{
							this._canIncludeInscribedCharList.Add(key);
						}
					}
					this._canIncludeInscribedCharList.Shuffle(1);
					ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("IncludedInscribedCharList", this._includedInscribedCharList).SetObject("CanIncludeInscribedCharList", this._canIncludeInscribedCharList).Set("AutoIncludeInscribedChar", autoIncludeToggle.isOn);
					GEvent.OnEvent(UiEvents.ConfirmIncludedInscriptionChar, args);
				};
			}
		}
	}

	// Token: 0x060022CA RID: 8906 RVA: 0x00101348 File Offset: 0x000FF548
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = "ButtonCancel" == btnName || "ButtonClose" == btnName;
		if (flag)
		{
			bool flag2 = this._isSettingInclude && this._tempIncludedInscribedCharList.ContentIsDifferent(this._includedInscribedCharList);
			if (flag2)
			{
				string title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
				string content = LocalStringManager.Get(LanguageKey.UI_CheckInspcription_CancelInlcude_Tip);
				CommonUtils.ShowConfirmDialog(title, content, delegate
				{
					this._tempIncludedInscribedCharList.Clear();
					this._tempIncludedInscribedCharList.AddRange(this._includedInscribedCharList);
					this.QuickHide();
				}, null, EDialogType.None);
			}
			else
			{
				this.QuickHide();
			}
		}
		else
		{
			bool flag3 = "ButtonConfirm" == btnName;
			if (flag3)
			{
				Action<InscribedCharacterKey, InscribedCharacter> onConfirmClicked = UI_CheckInscription.OnConfirmClicked;
				if (onConfirmClicked != null)
				{
					onConfirmClicked(this._currentSelectedInscribedCharKey, this._currentSelectedInscribedChar);
				}
				this.QuickHide();
			}
		}
	}

	// Token: 0x060022CB RID: 8907 RVA: 0x00101414 File Offset: 0x000FF614
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) || CommonCommandKit.Enter.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			Action<InscribedCharacterKey, InscribedCharacter> onConfirmClicked = UI_CheckInscription.OnConfirmClicked;
			if (onConfirmClicked != null)
			{
				onConfirmClicked(this._currentSelectedInscribedCharKey, this._currentSelectedInscribedChar);
			}
			this.QuickHide();
		}
	}

	// Token: 0x060022CC RID: 8908 RVA: 0x0010147B File Offset: 0x000FF67B
	private void Awake()
	{
		GEvent.Add(UiEvents.ClearIncludedInscriptionChar, new GEvent.Callback(this.ClearIncludedInscriptionChar));
	}

	// Token: 0x060022CD RID: 8909 RVA: 0x0010149A File Offset: 0x000FF69A
	private void OnDestroy()
	{
		GEvent.Remove(UiEvents.ClearIncludedInscriptionChar, new GEvent.Callback(this.ClearIncludedInscriptionChar));
	}

	// Token: 0x060022CE RID: 8910 RVA: 0x001014B9 File Offset: 0x000FF6B9
	private void OnEnable()
	{
		GEvent.Add(EEvents.InscriptionChange, new GEvent.Callback(this.RefreshInscription));
	}

	// Token: 0x060022CF RID: 8911 RVA: 0x001014D5 File Offset: 0x000FF6D5
	private void OnDisable()
	{
		GEvent.Remove(EEvents.InscriptionChange, new GEvent.Callback(this.RefreshInscription));
		this.CharsScroll.UpdateData(0);
		this._currentSelectedInscribedCharKey = InscribedCharacterKey.Invalid;
		this._currentSelectedInscribedChar = null;
	}

	// Token: 0x060022D0 RID: 8912 RVA: 0x00101510 File Offset: 0x000FF710
	private void ClearIncludedInscriptionChar(ArgumentBox arg = null)
	{
		this._tempIncludedInscribedCharList.Clear();
		this._includedInscribedCharList.Clear();
	}

	// Token: 0x060022D1 RID: 8913 RVA: 0x0010152C File Offset: 0x000FF72C
	private void RefreshInscription(ArgumentBox arg = null)
	{
		this._currentSelectedInscribedChar = null;
		bool flag = GlobalOperations.InscribedCharacters != null;
		if (flag)
		{
			this._inscribedChars = GlobalOperations.InscribedCharacters.ToList<KeyValuePair<InscribedCharacterKey, InscribedCharacter>>();
			this._inscribedChars.Sort((KeyValuePair<InscribedCharacterKey, InscribedCharacter> a, KeyValuePair<InscribedCharacterKey, InscribedCharacter> b) => a.Value.Timestamp.CompareTo(b.Value.Timestamp));
			bool showNone = this._showNone;
			if (showNone)
			{
				this._inscribedChars.Insert(0, new KeyValuePair<InscribedCharacterKey, InscribedCharacter>(new InscribedCharacterKey(0U, -1), null));
			}
			bool exitFlag = false;
			for (int i = 0; i < this._inscribedChars.Count; i++)
			{
				bool flag2 = this._inscribedChars[i].Key.Equals(this._currentSelectedInscribedCharKey);
				if (flag2)
				{
					exitFlag = true;
					this._currentSelectedInscribedChar = this._inscribedChars[i].Value;
					break;
				}
			}
			bool flag3 = !exitFlag;
			if (flag3)
			{
				this._currentSelectedInscribedCharKey = InscribedCharacterKey.Invalid;
				this._currentSelectedInscribedChar = null;
				this.RefreshConfirmButtonState();
			}
			this.CharsScroll.UpdateData(this._inscribedChars.Count);
		}
		else
		{
			this.CharsScroll.UpdateData(0);
		}
	}

	// Token: 0x060022D2 RID: 8914 RVA: 0x00101668 File Offset: 0x000FF868
	private void OnRender(int index, Refers item)
	{
		InscribedCharView view = item as InscribedCharView;
		InscribedCharacterKey key = this._inscribedChars[index].Key;
		InscribedCharacter data = this._inscribedChars[index].Value;
		bool selected = key.Equals(this._currentSelectedInscribedCharKey);
		bool isSelectedToCreate = key.Equals(this._preSelectedCharKey) && !key.Equals(InscribedCharacterKey.Invalid);
		bool included = !isSelectedToCreate && this._tempIncludedInscribedCharList.Contains(key);
		view.Refresh(key, data, selected, included, this._canDelete, this._isSettingInclude);
		view.ClickButton.interactable = !key.Equals(this._currentSelectedInscribedCharKey);
		view.SetClickEvent(new Action<InscribedCharacterKey, InscribedCharacter>(this.OnSelectChange));
		view.OnClickRemoveCallback = delegate()
		{
			this._currentSelectedInscribedCharKey = InscribedCharacterKey.Invalid;
			this._currentSelectedInscribedChar = null;
			this.RefreshInfoScroll();
		};
		bool isSettingInclude = this._isSettingInclude;
		if (isSettingInclude)
		{
			bool isMax = this._tempIncludedInscribedCharList.Count >= GlobalConfig.Instance.InscriptionCharForCreationMaxCount;
			view.IncludeToggle.interactable = (!isSelectedToCreate && (included || !isMax));
			view.IncludeToggle.GetComponent<TooltipInvoker>().enabled = isSelectedToCreate;
			view.OnIncludeChange = delegate(bool isOn)
			{
				if (isOn)
				{
					this._tempIncludedInscribedCharList.Add(key);
				}
				else
				{
					this._tempIncludedInscribedCharList.Remove(key);
				}
				this.RefreshIncludedCount();
				this.CharsScroll.ReRender();
				EventSystem.current.SetSelectedGameObject(null);
			};
		}
	}

	// Token: 0x060022D3 RID: 8915 RVA: 0x001017E4 File Offset: 0x000FF9E4
	private void RefreshIncludedCount()
	{
		int cur = this._tempIncludedInscribedCharList.Count;
		int max = GlobalConfig.Instance.InscriptionCharForCreationMaxCount;
		string str = LocalStringManager.GetFormat(LanguageKey.UI_CheckInspcription_InlcudeCount, cur, max);
		base.CGet<TextMeshProUGUI>("IncludeCount").text = str;
	}

	// Token: 0x060022D4 RID: 8916 RVA: 0x00101834 File Offset: 0x000FFA34
	private void OnSelectChange(InscribedCharacterKey key, InscribedCharacter character)
	{
		int prevSelectIndex = -1;
		int currSelectIndex = -1;
		for (int i = 0; i < this._inscribedChars.Count; i++)
		{
			bool flag = this._inscribedChars[i].Key.Equals(this._currentSelectedInscribedCharKey);
			if (flag)
			{
				prevSelectIndex = i;
			}
			bool flag2 = this._inscribedChars[i].Key.Equals(key);
			if (flag2)
			{
				currSelectIndex = i;
			}
			bool flag3 = prevSelectIndex >= 0 && currSelectIndex >= 0;
			if (flag3)
			{
				break;
			}
		}
		this._currentSelectedInscribedCharKey = key;
		this._currentSelectedInscribedChar = character;
		bool flag4 = prevSelectIndex >= 0;
		if (flag4)
		{
			this.CharsScroll.RefreshCell(prevSelectIndex);
		}
		this.CharsScroll.RefreshCell(currSelectIndex);
		this.RefreshInfoScroll();
		this.RefreshConfirmButtonState();
		this.RefreshBottomBar();
	}

	// Token: 0x060022D5 RID: 8917 RVA: 0x0010191C File Offset: 0x000FFB1C
	private void RefreshBottomBar()
	{
		this._bottomBar.gameObject.SetActive(true);
	}

	// Token: 0x060022D6 RID: 8918 RVA: 0x00101934 File Offset: 0x000FFB34
	private void RefreshConfirmButtonState()
	{
		bool flag = this.ConfirmButton != null;
		if (flag)
		{
			bool isSettingInclude = this._isSettingInclude;
			if (isSettingInclude)
			{
				this.ConfirmButton.interactable = true;
			}
			else
			{
				bool hasSelection = !this._currentSelectedInscribedCharKey.Equals(InscribedCharacterKey.Invalid);
				this.ConfirmButton.interactable = hasSelection;
			}
		}
	}

	// Token: 0x060022D7 RID: 8919 RVA: 0x00101994 File Offset: 0x000FFB94
	private void RefreshInfoScroll()
	{
		this._rightPanel.gameObject.SetActive(false);
		bool emptyData = this._currentSelectedInscribedCharKey.Equals(InscribedCharacterKey.Invalid);
		bool flag = emptyData;
		if (!flag)
		{
			base.CGet<TextMeshProUGUI>("Name").text = this._currentSelectedInscribedChar.Surname + this._currentSelectedInscribedChar.GivenName;
			this._rightPanel.gameObject.SetActive(true);
			this.FillGender();
			this.FillNeiliType();
			this.FillCharm();
			this.FillMainAttribute();
			this.FillLifeSkill();
			this.FillCombatSkill();
			this.FillFeature();
		}
	}

	// Token: 0x060022D8 RID: 8920 RVA: 0x00101A3C File Offset: 0x000FFC3C
	private void FillGender()
	{
		Refers refers = base.CGet<Refers>("CharacterGender");
		InfoItem genderItem = refers.UserObject as InfoItem;
		bool flag = genderItem == null;
		if (flag)
		{
			genderItem = new InfoItem(refers);
			refers.UserObject = genderItem;
		}
		StringBuilder sb = new StringBuilder();
		CommonUtils.EDisplayGender displayGender = CommonUtils.GetDisplayGender(this._currentSelectedInscribedChar.Gender, -1);
		genderItem.SetIcon(CommonUtils.GetGenderIcon(displayGender));
		sb.Append(CommonUtils.GetGenderString(displayGender));
		genderItem.SetInfoName(LocalStringManager.Get(LanguageKey.UI_NewGame_AdjustTitle_0_0));
		bool flag2 = this._currentSelectedInscribedChar.Avatar.Gender != this._currentSelectedInscribedChar.Gender;
		if (flag2)
		{
			string str = (this._currentSelectedInscribedChar.Avatar.Gender == 0) ? LocalStringManager.Get(LanguageKey.UI_NewGame_FemaleLike) : LocalStringManager.Get(LanguageKey.UI_NewGame_MaleLike);
			sb.Append(" 【" + str + "】");
		}
		genderItem.SetInfoValue(sb.ToString());
	}

	// Token: 0x060022D9 RID: 8921 RVA: 0x00101B38 File Offset: 0x000FFD38
	private void FillBehavior()
	{
		Refers refers = base.CGet<Refers>("CharacterBehavior");
		InfoItem behaviorItem = refers.UserObject as InfoItem;
		bool flag = behaviorItem == null;
		if (flag)
		{
			behaviorItem = new InfoItem(refers);
			refers.UserObject = behaviorItem;
		}
		sbyte behaviorType = GameData.Domains.Character.BehaviorType.GetBehaviorType(this._currentSelectedInscribedChar.Morality);
		BehaviorTypeItem config = Config.BehaviorType.Instance.GetItem((short)behaviorType);
		behaviorItem.SetInfoName(LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Behavior));
		behaviorItem.SetInfoValue(CommonUtils.GetBehaviorString(behaviorType));
		behaviorItem.SetIcon(config.Icon);
	}

	// Token: 0x060022DA RID: 8922 RVA: 0x00101BC0 File Offset: 0x000FFDC0
	private void FillCharm()
	{
		Refers refers = base.CGet<Refers>("CharacterCharm");
		InfoItem charmItem = refers.UserObject as InfoItem;
		bool flag = charmItem == null;
		if (flag)
		{
			charmItem = new InfoItem(refers);
			refers.UserObject = charmItem;
		}
		CharacterPropertyDisplayItem config = CharacterPropertyDisplay.Instance[101];
		charmItem.SetInfoName(config.Name);
		short charmValue = this._currentSelectedInscribedChar.CalcAttraction(this._currentSelectedInscribedChar.CurrAge, this._currentSelectedInscribedChar.ClothingDisplayId);
		charmItem.SetInfoValue(CommonUtils.GetCharmLevelText(charmValue, this._currentSelectedInscribedChar.Gender, this._currentSelectedInscribedChar.CurrAge, this._currentSelectedInscribedChar.ClothingDisplayId, false, this._currentSelectedInscribedChar.Avatar.FaceVisible));
		charmItem.SetIcon(CommonUtils.GetCharmLevelIconLegacy(charmValue, this._currentSelectedInscribedChar.CurrAge, this._currentSelectedInscribedChar.ClothingDisplayId, this._currentSelectedInscribedChar.Avatar.FaceVisible));
	}

	// Token: 0x060022DB RID: 8923 RVA: 0x00101CB0 File Offset: 0x000FFEB0
	private void FillNeiliType()
	{
		MonthItem monthConfig = Month.Instance[this._currentSelectedInscribedChar.BirthMonth];
		NeiliTypeItem config = NeiliType.Instance[monthConfig.FiveElementsType];
		this._neiliTypeTxt.text = string.Format("<SpName=mousetip_shuxing_{0}>{1}", monthConfig.FiveElementsType, config.Name.SetColor(Colors.Instance.FiveElementsColors[(int)monthConfig.FiveElementsType]));
		this._neiliTypeTxt.GetComponent<TMPTextSpriteHelper>().Parse();
	}

	// Token: 0x060022DC RID: 8924 RVA: 0x00101D38 File Offset: 0x000FFF38
	private void FillMainAttribute()
	{
		List<Refers> attributeRefers = base.CGet<Refers>("MainAttributeLine").CGetList<Refers>("CommonParameterVertical_");
		for (sbyte i = 0; i < 6; i += 1)
		{
			short displayTemplateId = CharacterMajorAttribute.MainAttributeTemplateIdArray[(int)i];
			CharacterPropertyDisplayItem config = CharacterPropertyDisplay.Instance.GetItem(displayTemplateId);
			Refers refers = attributeRefers[(int)i];
			refers.CGet<CImage>("Icon").SetSprite(config.Icon, false, null);
			refers.CGet<TextMeshProUGUI>("Title").text = config.Name;
			TooltipInvoker mouseTip = refers.CGet<TooltipInvoker>("MouseTip");
			mouseTip.enabled = true;
			mouseTip.Type = TipType.Simple;
			mouseTip.IsLanguageKey = false;
			mouseTip.PresetParam = new string[]
			{
				config.Name,
				config.Desc
			};
			refers.CGet<TextMeshProUGUI>("Value").text = (ref this._currentSelectedInscribedChar.BaseMainAttributes.Items.FixedElementField + (IntPtr)i * 2).ToString();
		}
	}

	// Token: 0x060022DD RID: 8925 RVA: 0x00101E40 File Offset: 0x00100040
	private unsafe void FillLifeSkill()
	{
		List<Refers> skillReferList = base.CGet<Refers>("LifeSkillLine").CGetList<Refers>("CommonParameterVertical_");
		for (sbyte i = 0; i < 16; i += 1)
		{
			LifeSkillTypeItem config = Config.LifeSkillType.Instance.GetItem(i);
			Refers refers = skillReferList[(int)i];
			refers.CGet<CImage>("Icon").SetSprite(config.DisplayIcon, false, null);
			refers.CGet<TextMeshProUGUI>("Title").text = config.Name;
			refers.CGet<TextMeshProUGUI>("Value").text = (*(ref this._currentSelectedInscribedChar.BaseLifeSkillQualifications.Items.FixedElementField + (IntPtr)i * 2)).SetValueColor();
			refers.CGet<TooltipInvoker>("MouseTip").PresetParam = new string[]
			{
				config.Name,
				config.Desc
			};
		}
	}

	// Token: 0x060022DE RID: 8926 RVA: 0x00101F20 File Offset: 0x00100120
	private unsafe void FillCombatSkill()
	{
		List<Refers> skillReferList = base.CGet<Refers>("CombatSkillLine").CGetList<Refers>("CommonParameterVertical_");
		for (sbyte i = 0; i < 14; i += 1)
		{
			CombatSkillTypeItem config = CombatSkillType.Instance.GetItem(i);
			Refers refers = skillReferList[(int)i];
			refers.CGet<CImage>("Icon").SetSprite(config.DisplayIcon, false, null);
			refers.CGet<TextMeshProUGUI>("Title").text = config.Name;
			refers.CGet<TextMeshProUGUI>("Value").text = (*(ref this._currentSelectedInscribedChar.BaseCombatSkillQualifications.Items.FixedElementField + (IntPtr)i * 2)).SetValueColor();
			refers.CGet<TooltipInvoker>("MouseTip").PresetParam = new string[]
			{
				config.Name,
				config.Desc
			};
		}
	}

	// Token: 0x060022DF RID: 8927 RVA: 0x00102000 File Offset: 0x00100200
	private void FillFeature()
	{
		List<short> featureIdList = this._currentSelectedInscribedChar.FeatureIds;
		this._characterFeatureScroll.SetShowFeatureListFromOutside(featureIdList);
	}

	// Token: 0x060022E0 RID: 8928 RVA: 0x00102027 File Offset: 0x00100227
	public override void QuickHide()
	{
		Action onQuickHide = UI_CheckInscription.OnQuickHide;
		if (onQuickHide != null)
		{
			onQuickHide();
		}
		UI_CheckInscription.OnQuickHide = null;
		UI_CheckInscription.OnConfirmClicked = null;
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		base.QuickHide();
	}

	// Token: 0x04001AB1 RID: 6833
	private CharacterFeatureFixedScroll _characterFeatureScroll;

	// Token: 0x04001AB2 RID: 6834
	public const string ArgAllowConfirm = "AllowConfirm";

	// Token: 0x04001AB3 RID: 6835
	public const string ArgPreSelectedCharKey = "PreSelectedCharKey";

	// Token: 0x04001AB4 RID: 6836
	public const string ArgShowNone = "ShowNone";

	// Token: 0x04001AB5 RID: 6837
	public const string ArgCanDelete = "CanDelete";

	// Token: 0x04001AB6 RID: 6838
	public const string IsSettingInclude = "IsSettingInclude";

	// Token: 0x04001AB7 RID: 6839
	private bool _isSettingInclude;

	// Token: 0x04001AB8 RID: 6840
	public static Action<InscribedCharacterKey, InscribedCharacter> OnConfirmClicked;

	// Token: 0x04001AB9 RID: 6841
	public static Action OnQuickHide;

	// Token: 0x04001ABA RID: 6842
	public InfinityScrollLegacy CharsScroll;

	// Token: 0x04001ABB RID: 6843
	public CButtonObsolete CloseButton;

	// Token: 0x04001ABC RID: 6844
	public CButtonObsolete ConfirmButton;

	// Token: 0x04001ABD RID: 6845
	private List<KeyValuePair<InscribedCharacterKey, InscribedCharacter>> _inscribedChars;

	// Token: 0x04001ABE RID: 6846
	private InscribedCharacterKey _currentSelectedInscribedCharKey = InscribedCharacterKey.Invalid;

	// Token: 0x04001ABF RID: 6847
	private InscribedCharacterKey _preSelectedCharKey = InscribedCharacterKey.Invalid;

	// Token: 0x04001AC0 RID: 6848
	private InscribedCharacter _currentSelectedInscribedChar;

	// Token: 0x04001AC1 RID: 6849
	private readonly List<InscribedCharacterKey> _tempIncludedInscribedCharList = new List<InscribedCharacterKey>();

	// Token: 0x04001AC2 RID: 6850
	private readonly List<InscribedCharacterKey> _includedInscribedCharList = new List<InscribedCharacterKey>();

	// Token: 0x04001AC3 RID: 6851
	private readonly List<InscribedCharacterKey> _canIncludeInscribedCharList = new List<InscribedCharacterKey>();

	// Token: 0x04001AC4 RID: 6852
	private bool _showNone;

	// Token: 0x04001AC5 RID: 6853
	private bool _canDelete;
}
