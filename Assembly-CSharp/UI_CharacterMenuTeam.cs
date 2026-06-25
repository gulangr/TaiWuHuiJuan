using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using Game.Components.Avatar;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;

// Token: 0x020001D9 RID: 473
public class UI_CharacterMenuTeam : UI_CharacterMenuSubPageBase
{
	// Token: 0x17000321 RID: 801
	// (get) Token: 0x06001ED6 RID: 7894 RVA: 0x000DE919 File Offset: 0x000DCB19
	public override LanguageKey TitleKey
	{
		get
		{
			return LanguageKey.LK_CharacterMenu_Title_Team;
		}
	}

	// Token: 0x06001ED7 RID: 7895 RVA: 0x000DE920 File Offset: 0x000DCB20
	public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
	{
		return curSubPage == ECharacterSubPage.Team;
	}

	// Token: 0x17000322 RID: 802
	// (get) Token: 0x06001ED8 RID: 7896 RVA: 0x000DE936 File Offset: 0x000DCB36
	public override bool ShowCharacterList
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06001ED9 RID: 7897 RVA: 0x000DE93C File Offset: 0x000DCB3C
	public override void OnInit(ArgumentBox argsBox)
	{
		this._charIdList.Clear();
		IReadOnlyList<CharacterDisplayData> characters = base.CharacterMenu.DisplayCharacters;
		bool flag = characters != null;
		if (flag)
		{
			for (int i = 0; i < characters.Count; i++)
			{
				CharacterDisplayData item = characters[i];
				bool flag2 = item != null;
				if (flag2)
				{
					this._charIdList.Add(item.CharacterId);
				}
			}
		}
		this.NeedDataListenerId = true;
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
		{
			this._nameDict.Clear();
			this._avatarRelatedDataDict.Clear();
			this._charmDataDict.Clear();
			this._propertyValueDict.Clear();
			CharacterDomainMethod.Call.GetGroupCharDisplayDataList(this.Element.GameDataListenerId, this._charIdList);
			bool flag3 = this._sortBtn != null;
			if (flag3)
			{
				this._sortBtn = null;
				this._titleHolder = base.CGet<RectTransform>("TitleHolder");
				this.ResetTitleButtons();
			}
		}));
	}

	// Token: 0x06001EDA RID: 7898 RVA: 0x000DE9D8 File Offset: 0x000DCBD8
	public void Awake()
	{
		this._titleHolder = base.CGet<RectTransform>("TitleHolder");
		this._charScroll = base.CGet<InfinityScrollLegacy>("CharScroll");
		CToggleGroupObsolete charTogGroup = this._charScroll.GetComponent<CToggleGroupObsolete>();
		this._charScroll.OnItemRender = new Action<int, Refers>(this.OnRenderChar);
		charTogGroup.OnActiveToggleChange = delegate(CToggleObsolete togNew, CToggleObsolete togOld)
		{
			bool flag = null == togNew;
			if (!flag)
			{
				int charId = this._charIdList[togNew.Key];
				base.CharacterMenu.SelectCharacter(charId);
				this._charScroll.SelectedTogKey = togNew.Key;
			}
		};
		charTogGroup.InitPreOnToggle(-1);
		this._charScroll.SetTogGroup(charTogGroup, false, false);
		CToggleGroupObsolete subToggleGroup = base.CGet<CToggleGroupObsolete>("SubTogGroup");
		subToggleGroup.InitPreOnToggle(-1);
		for (int i = 0; i < UI_CharacterMenuTeam.ToggleGroupNameKeys.Count; i++)
		{
			CToggleObsolete toggle = subToggleGroup.Get(i);
			toggle.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = LocalStringManager.Get(UI_CharacterMenuTeam.ToggleGroupNameKeys[i]);
			toggle.Key = i;
		}
		subToggleGroup.OnActiveToggleChange = delegate(CToggleObsolete newTog, CToggleObsolete _)
		{
			bool flag = newTog != null;
			if (flag)
			{
				this.SwitchToSubPage(newTog.Key);
			}
		};
		subToggleGroup.Set(0, true, false);
		this.ResetTitleButtons();
	}

	// Token: 0x06001EDB RID: 7899 RVA: 0x000DEAE4 File Offset: 0x000DCCE4
	public override void OnLanguageChange(LocalStringManager.LanguageType languageType)
	{
		foreach (CommonTableHead tableHead in this._tableHeads)
		{
			tableHead.OnLanguageChange(languageType);
		}
	}

	// Token: 0x06001EDC RID: 7900 RVA: 0x000DEB40 File Offset: 0x000DCD40
	private void ResetTitleButtons()
	{
		this._tableHeads.Clear();
		Transform stateHolder = this._titleHolder.GetChild(0);
		Transform propertyHolder = this._titleHolder.GetChild(1);
		Transform property2Holder = this._titleHolder.GetChild(2);
		Transform lifeSkillHolder = this._titleHolder.GetChild(3);
		Transform combatSkillHolder = this._titleHolder.GetChild(4);
		Transform personalityHolder = this._titleHolder.GetChild(5);
		Transform itemHolder = this._titleHolder.GetChild(6);
		Transform commandHolder = this._titleHolder.GetChild(7);
		this.InitTableHeadTitles(stateHolder, UI_CharacterMenuTeam.StateTitleNameKeys, (int i) => 0 + i);
		this.InitTableHeadTitles(propertyHolder, UI_CharacterMenuTeam.PropertyTitleNameKeys, (int i) => (i == 0) ? 0 : (11 + i - 1));
		this.InitTableHeadTitles(property2Holder, UI_CharacterMenuTeam.Property2TitleNameKeys, (int i) => (i == 0) ? 0 : (21 + i - 1));
		this.InitTableHeadTitles(lifeSkillHolder, UI_CharacterMenuTeam.LifeSkillTitleNameKeys, (int i) => (i == 0) ? 0 : (30 + i - 1));
		this.InitTableHeadTitles(combatSkillHolder, UI_CharacterMenuTeam.CombatSkillTitleNameKeys, (int i) => (i == 0) ? 0 : (47 + i - 1));
		this.InitTableHeadTitles(personalityHolder, UI_CharacterMenuTeam.PersonalityTitleNameKeys, (int i) => (i == 0) ? 0 : (62 + i - 1));
		this.InitTableHeadTitles(itemHolder, UI_CharacterMenuTeam.ItemTitleNameKeys, (int i) => (i == 0) ? 0 : (69 + i - 1));
		this.InitTableHeadTitles(commandHolder, UI_CharacterMenuTeam.CommandTitleNameKeys, (int i) => (i == 0) ? 0 : (79 + i - 1));
	}

	// Token: 0x06001EDD RID: 7901 RVA: 0x000DED2C File Offset: 0x000DCF2C
	private void InitTableHeadTitles(Transform holder, List<LanguageKey> languageKeyList, Func<int, int> userIntCalculator)
	{
		CommonTableHead tableHead = holder.GetComponent<CommonTableHead>();
		this._tableHeads.Add(tableHead);
		for (int i = 0; i < tableHead.GetContentChildCount(false); i++)
		{
			int userInt = userIntCalculator(i);
			UI_CharacterMenuTeam.InitTitleBtn(tableHead, i, languageKeyList, userInt);
		}
		tableHead.HideAllArrowAndSequence();
	}

	// Token: 0x06001EDE RID: 7902 RVA: 0x000DED7F File Offset: 0x000DCF7F
	private new void OnDisable()
	{
		this._charIdList.Clear();
		this._charScroll.SetDataCount(0);
	}

	// Token: 0x06001EDF RID: 7903 RVA: 0x000DED9C File Offset: 0x000DCF9C
	private static void InitTitleBtn(CommonTableHead tableHead, int childIndex, List<LanguageKey> languageKeyList, int userInt)
	{
		Transform child = tableHead.GetTableHeadChild(childIndex, false);
		Refers refers = child.GetComponent<Refers>();
		refers.UserInt = userInt;
		string text = LocalStringManager.Get(languageKeyList[childIndex]);
		refers.CGet<TextMeshProUGUI>("Label").text = text;
	}

	// Token: 0x06001EE0 RID: 7904 RVA: 0x000DEDE0 File Offset: 0x000DCFE0
	public override void OnSubpageVisible()
	{
		this._charIdList.Clear();
		IReadOnlyList<CharacterDisplayData> characters = base.CharacterMenu.DisplayCharacters;
		bool flag = characters != null;
		if (flag)
		{
			for (int i = 0; i < characters.Count; i++)
			{
				CharacterDisplayData item = characters[i];
				bool flag2 = item != null;
				if (flag2)
				{
					this._charIdList.Add(item.CharacterId);
				}
			}
		}
		base.CharacterMenu.SetCurPageSubpage(this.CurTabIndex);
		CharacterDomainMethod.Call.GetGroupCharDisplayDataList(this.Element.GameDataListenerId, this._charIdList);
	}

	// Token: 0x06001EE1 RID: 7905 RVA: 0x000DEE78 File Offset: 0x000DD078
	private void SwitchToSubPage(int subPageIndex)
	{
		this._subPageType = (UI_CharacterMenuTeam.ESubPage)subPageIndex;
		for (int i = 0; i < this._titleHolder.childCount; i++)
		{
			this._titleHolder.GetChild(i).gameObject.SetActive(i == subPageIndex);
		}
		this.ResetSort();
		bool ready = this.Element.Ready;
		if (ready)
		{
			CommonHorizontalLayoutGrid itemTemplate = this.characterPrefabArray[(int)this._subPageType];
			this._charScroll.UpdateStyle(InfinityScrollLegacy.ScrollDirection.FromTop, 1, this._charScroll.Gap, this._charScroll.Padding, itemTemplate);
		}
	}

	// Token: 0x06001EE2 RID: 7906 RVA: 0x000DEF10 File Offset: 0x000DD110
	private void ResetSort()
	{
		bool flag = this._sortBtn != null;
		if (flag)
		{
			UI_CharacterMenuTeam.UpdateSorter(this._sortBtn.GetComponent<Refers>(), -1, true);
			this._sortBtn = null;
		}
		this._charIdList.Clear();
		IReadOnlyList<CharacterDisplayData> characters = base.CharacterMenu.DisplayCharacters;
		bool flag2 = characters != null;
		if (flag2)
		{
			for (int i = 0; i < characters.Count; i++)
			{
				CharacterDisplayData item = characters[i];
				bool flag3 = item != null;
				if (flag3)
				{
					this._charIdList.Add(item.CharacterId);
				}
			}
		}
		this._charScroll.UpdateData(this._charIdList.Count);
	}

	// Token: 0x06001EE3 RID: 7907 RVA: 0x000DEFC4 File Offset: 0x000DD1C4
	protected unsafe override void OnNotifyGameDataFiltered(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type9 = notification.Type;
			byte b = type9;
			if (b == 1)
			{
				bool flag = notification.DomainId == 4 && notification.MethodId == 57;
				if (flag)
				{
					List<GroupCharDisplayData> dataList = null;
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref dataList);
					for (UI_CharacterMenuTeam.TeamPropertyType type = UI_CharacterMenuTeam.TeamPropertyType.Age; type < UI_CharacterMenuTeam.TeamPropertyType.Count; type++)
					{
						this._maxValueDict[type] = int.MinValue;
						this._allValueEqual[(int)type] = true;
					}
					foreach (GroupCharDisplayData data in dataList)
					{
						int charId = data.CharacterId;
						Dictionary<UI_CharacterMenuTeam.TeamPropertyType, int> propertyDict = new Dictionary<UI_CharacterMenuTeam.TeamPropertyType, int>();
						this._templateIdDict[charId] = data.CharacterTemplateId;
						this._propertyValueDict[charId] = propertyDict;
						this._charmDataDict[charId] = new UI_CharacterMenuTeam.CharmData(data.Gender, data.PhysiologicalAge, data.ClothDisplayId, data.FaceVisible, data.CreatingType);
						this._ageDict[charId] = data.PhysiologicalAge;
						this._healthDict[charId] = data.Health;
						this._leftMaxHealthDict[charId] = data.MaxLeftHealth;
						this._interactedDict[charId] = data.IsInteractedWithTaiwu;
						this._nameDict[charId] = NameCenter.GetMonasticTitleOrDisplayName(ref data.NameData, base.CharacterMenu.IsTaiwu(charId), false);
						bool useAnonymousName = base.CharacterMenu.UseAnonymousName;
						if (useAnonymousName)
						{
							CharacterItem config = Character.Instance.GetItem(data.NameData.CharTemplateId);
							this._nameDict[charId] = config.AnonymousTitle;
						}
						this._avatarRelatedDataDict[charId] = data.AvatarRelatedData;
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.Age, (int)data.PhysiologicalAge, true);
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.ActualAge, (int)data.ActualAge, true);
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.Health, (int)data.Health, true);
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.DefeatMark, (int)data.DefeatMarkCount, true);
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.Charm, (int)data.Charm, true);
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.Behavior, (int)data.BehaviorType, true);
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.Fame, (int)data.Fame, true);
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.Happiness, (int)data.Happiness, true);
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.Favor, (int)data.FavorabilityToTaiwu, true);
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.Samsara, (int)data.PreexistenceCharCount, true);
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.AttackMedal, data.AttackMedal, true);
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.DefenceMedal, data.DefenceMedal, true);
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.WisdomMedal, data.WisdomMedal, true);
						for (int type2 = 0; type2 < 6; type2++)
						{
							this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.MainAttribute0 + type2, (int)(*(ref data.MaxMainAttributes.Items.FixedElementField + (IntPtr)type2 * 2)), true);
						}
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.OuterPenetrate, data.Penetrations.Outer, true);
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.InnerPenetrate, data.Penetrations.Inner, true);
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.OuterPenetrateResist, data.PenetrationResists.Outer, true);
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.InnerPenetrateResist, data.PenetrationResists.Inner, true);
						for (int type3 = 0; type3 < 4; type3++)
						{
							this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.Hit0 + type3, *(ref data.HitValues.Items.FixedElementField + (IntPtr)type3 * 4), true);
							this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.Avoid0 + type3, *(ref data.AvoidValues.Items.FixedElementField + (IntPtr)type3 * 4), true);
						}
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.QiDisorder, (int)(data.DisorderOfQi / 10), true);
						for (int type4 = 0; type4 < 16; type4++)
						{
							this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.LifeSkill0 + type4, (int)(*(ref data.LifeSkillQualifications.Items.FixedElementField + (IntPtr)type4 * 2)), true);
						}
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.LifeSkillGrowth, (int)data.LifeSkillGrowthType, false);
						int lifeSkillGrowthValue = (int)this.GetSkillGrowthAddValue(charId, propertyDict[UI_CharacterMenuTeam.TeamPropertyType.LifeSkillGrowth]);
						bool flag2 = this._allValueEqual[46] && this._maxValueDict[UI_CharacterMenuTeam.TeamPropertyType.LifeSkillGrowth] != int.MinValue && this._maxValueDict[UI_CharacterMenuTeam.TeamPropertyType.LifeSkillGrowth] != lifeSkillGrowthValue;
						if (flag2)
						{
							this._allValueEqual[46] = false;
						}
						this._maxValueDict[UI_CharacterMenuTeam.TeamPropertyType.LifeSkillGrowth] = Mathf.Max(this._maxValueDict[UI_CharacterMenuTeam.TeamPropertyType.LifeSkillGrowth], lifeSkillGrowthValue);
						for (int type5 = 0; type5 < 14; type5++)
						{
							this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.CombatSkill0 + type5, (int)(*(ref data.CombatSkillQualifications.Items.FixedElementField + (IntPtr)type5 * 2)), true);
						}
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.CombatSkillGrowth, (int)data.CombatSkillGrowthType, false);
						int combatSkillGrowthValue = (int)this.GetSkillGrowthAddValue(charId, propertyDict[UI_CharacterMenuTeam.TeamPropertyType.CombatSkillGrowth]);
						bool flag3 = this._allValueEqual[61] && this._maxValueDict[UI_CharacterMenuTeam.TeamPropertyType.CombatSkillGrowth] != int.MinValue && this._maxValueDict[UI_CharacterMenuTeam.TeamPropertyType.CombatSkillGrowth] != combatSkillGrowthValue;
						if (flag3)
						{
							this._allValueEqual[61] = false;
						}
						this._maxValueDict[UI_CharacterMenuTeam.TeamPropertyType.CombatSkillGrowth] = Mathf.Max(this._maxValueDict[UI_CharacterMenuTeam.TeamPropertyType.CombatSkillGrowth], combatSkillGrowthValue);
						for (int type6 = 0; type6 < 7; type6++)
						{
							this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.Personality0 + type6, (int)(*(ref data.Personalities.Items.FixedElementField + type6)), true);
						}
						for (int type7 = 0; type7 < 8; type7++)
						{
							this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.Resource0 + type7, *(ref data.Resources.Items.FixedElementField + (IntPtr)type7 * 4), true);
						}
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.InventoryLoad, data.CurrInventoryLoad, true);
						this._maxInventoryLoadDict[charId] = data.MaxInventoryLoad;
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.KidnapCount, (int)data.KidnapCount, true);
						for (int type8 = 0; type8 < 3; type8++)
						{
							int value = (int)((data.Command.Items == null || !data.Command.Items.CheckIndex(type8)) ? -1 : data.Command.Items[type8]);
							this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.Command0 + type8, value, true);
						}
						this.InitPropertyValue(propertyDict, UI_CharacterMenuTeam.TeamPropertyType.CreatingType, (int)data.CreatingType, false);
					}
					bool flag4 = this._sortBtn != null;
					if (flag4)
					{
						this._charIdList.Sort(new Comparison<int>(this.SortByProperty));
					}
					int selectedIndex = Math.Max(this._charIdList.IndexOf(base.CharacterMenu.CurCharacterId), 0);
					this._charScroll.UpdateData(this._charIdList.Count);
					this._charScroll.SelectedTogKey = selectedIndex;
					this._charScroll.ScrollTo(selectedIndex, 0.3f);
					this.Element.ShowAfterRefresh();
				}
			}
		}
	}

	// Token: 0x06001EE4 RID: 7908 RVA: 0x000DF790 File Offset: 0x000DD990
	private void InitPropertyValue(Dictionary<UI_CharacterMenuTeam.TeamPropertyType, int> propertyDict, UI_CharacterMenuTeam.TeamPropertyType type, int value, bool setMaxValue = true)
	{
		propertyDict[type] = value;
		bool flag = this._allValueEqual[(int)type] && this._maxValueDict[type] != int.MinValue && this._maxValueDict[type] != value;
		if (flag)
		{
			this._allValueEqual[(int)type] = false;
		}
		if (setMaxValue)
		{
			this._maxValueDict[type] = Mathf.Max(this._maxValueDict[type], value);
		}
	}

	// Token: 0x06001EE5 RID: 7909 RVA: 0x000DF80C File Offset: 0x000DDA0C
	protected override void OnClick(Transform btn)
	{
		int selectedCharId = this._charIdList[this._charScroll.SelectedTogKey];
		bool flag = this._sortBtn != btn;
		if (flag)
		{
			bool flag2 = this._sortBtn != null;
			if (flag2)
			{
				UI_CharacterMenuTeam.UpdateSorter(this._sortBtn.GetComponent<Refers>(), -1, true);
			}
			UI_CharacterMenuTeam.UpdateSorter(btn.GetComponent<Refers>(), 1, true);
			this._sortBtn = btn.GetComponent<CButtonObsolete>();
			this._descSort = true;
			this._charIdList.Sort(new Comparison<int>(this.SortByProperty));
		}
		else
		{
			Refers titleRefers = btn.GetComponent<Refers>();
			bool descSort = this._descSort;
			if (descSort)
			{
				UI_CharacterMenuTeam.UpdateSorter(titleRefers, 1, false);
				this._descSort = false;
				this._charIdList.Sort(new Comparison<int>(this.SortByProperty));
			}
			else
			{
				UI_CharacterMenuTeam.UpdateSorter(titleRefers, -1, false);
				this._sortBtn = null;
				this._charIdList.Clear();
				IReadOnlyList<CharacterDisplayData> characters = base.CharacterMenu.DisplayCharacters;
				bool flag3 = characters != null;
				if (flag3)
				{
					for (int i = 0; i < characters.Count; i++)
					{
						CharacterDisplayData item = characters[i];
						bool flag4 = item != null;
						if (flag4)
						{
							this._charIdList.Add(item.CharacterId);
						}
					}
				}
			}
		}
		this._charScroll.SelectedTogKey = this._charIdList.IndexOf(selectedCharId);
		this._charScroll.UpdateData(this._charIdList.Count);
	}

	// Token: 0x06001EE6 RID: 7910 RVA: 0x000DF998 File Offset: 0x000DDB98
	private static void UpdateSorter(Refers sorterRefers, int index, bool isDesc)
	{
		CommonTableHead tableHead;
		bool flag = sorterRefers.transform.parent.TryGetComponent<CommonTableHead>(out tableHead);
		if (flag)
		{
			tableHead.SetArrow(sorterRefers, index >= 0, !isDesc);
			tableHead.SetSequence(sorterRefers, index >= 0, index);
		}
		else
		{
			GameObject arrow = UI_CharacterMenuTeam.GetSortButtonArrow(sorterRefers);
			arrow.SetActive(index >= 0);
			RectTransform arrowRect = arrow.GetComponent<RectTransform>();
			arrowRect.localRotation = SortFilter.GetArrowRotation(isDesc);
		}
	}

	// Token: 0x06001EE7 RID: 7911 RVA: 0x000DFA0C File Offset: 0x000DDC0C
	private int SortByProperty(int charId1, int charId2)
	{
		UI_CharacterMenuTeam.TeamPropertyType type = (UI_CharacterMenuTeam.TeamPropertyType)this._sortBtn.GetComponent<Refers>().UserInt;
		UI_CharacterMenuTeam.TeamPropertyType teamPropertyType = type;
		UI_CharacterMenuTeam.TeamPropertyType teamPropertyType2 = teamPropertyType;
		if (teamPropertyType2 >= UI_CharacterMenuTeam.TeamPropertyType.AttackMedal)
		{
			if (teamPropertyType2 <= UI_CharacterMenuTeam.TeamPropertyType.WisdomMedal)
			{
				bool hideProperty = this._propertyValueDict[charId1][type] == 0;
				bool hideProperty2 = this._propertyValueDict[charId2][type] == 0;
				bool flag = hideProperty != hideProperty2;
				if (flag)
				{
					return hideProperty2.CompareTo(hideProperty);
				}
			}
		}
		else
		{
			if (teamPropertyType2 <= UI_CharacterMenuTeam.TeamPropertyType.LifeSkillGrowth)
			{
				if (teamPropertyType2 < UI_CharacterMenuTeam.TeamPropertyType.LifeSkill0)
				{
					if (teamPropertyType2 != UI_CharacterMenuTeam.TeamPropertyType.Age)
					{
						if (teamPropertyType2 != UI_CharacterMenuTeam.TeamPropertyType.Health)
						{
							if (teamPropertyType2 != UI_CharacterMenuTeam.TeamPropertyType.Favor)
							{
								goto IL_18C;
							}
							bool hideProperty3 = !this._interactedDict[charId1] || this._propertyValueDict[charId1][type] == -32768;
							bool hideProperty4 = !this._interactedDict[charId2] || this._propertyValueDict[charId2][type] == -32768;
							bool flag2 = hideProperty3 != hideProperty4;
							if (flag2)
							{
								return hideProperty4.CompareTo(hideProperty3);
							}
							goto IL_18C;
						}
					}
					else
					{
						bool hideAge = this.GetCharHideAge(charId1);
						bool hideAge2 = this.GetCharHideAge(charId2);
						bool flag3 = hideAge != hideAge2;
						if (flag3)
						{
							return hideAge2.CompareTo(hideAge);
						}
						goto IL_18C;
					}
				}
			}
			else if (teamPropertyType2 > UI_CharacterMenuTeam.TeamPropertyType.CombatSkillGrowth)
			{
				goto IL_18C;
			}
			bool hideProperty5 = this.GetCharHideProperty(charId1);
			bool hideProperty6 = this.GetCharHideProperty(charId2);
			bool flag4 = hideProperty5 != hideProperty6;
			if (flag4)
			{
				return hideProperty6.CompareTo(hideProperty5);
			}
		}
		IL_18C:
		if (!true)
		{
		}
		int result;
		if (type >= UI_CharacterMenuTeam.TeamPropertyType.Command0)
		{
			if (type <= UI_CharacterMenuTeam.TeamPropertyType.Command2)
			{
				result = this.SortByCommand(charId1, charId2, type);
				goto IL_221;
			}
		}
		else
		{
			switch (type)
			{
			case UI_CharacterMenuTeam.TeamPropertyType.Name:
				result = this.SortByName(charId1, charId2);
				goto IL_221;
			case UI_CharacterMenuTeam.TeamPropertyType.Age:
			case UI_CharacterMenuTeam.TeamPropertyType.DefeatMark:
				break;
			case UI_CharacterMenuTeam.TeamPropertyType.Health:
				result = this.SortByHealth(charId1, charId2);
				goto IL_221;
			case UI_CharacterMenuTeam.TeamPropertyType.Charm:
				result = this.SortByCharm(charId1, charId2);
				goto IL_221;
			case UI_CharacterMenuTeam.TeamPropertyType.Behavior:
				result = this.SortByBehavior(charId1, charId2);
				goto IL_221;
			default:
				if (type == UI_CharacterMenuTeam.TeamPropertyType.LifeSkillGrowth || type == UI_CharacterMenuTeam.TeamPropertyType.CombatSkillGrowth)
				{
					result = this.SortBySkillGrowth(charId1, charId2, type);
					goto IL_221;
				}
				break;
			}
		}
		result = this.SortByDefault(charId1, charId2, type);
		IL_221:
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06001EE8 RID: 7912 RVA: 0x000DFC48 File Offset: 0x000DDE48
	private int SortByDefault(int charId1, int charId2, UI_CharacterMenuTeam.TeamPropertyType type)
	{
		int value = this._propertyValueDict[charId1][type];
		int value2 = this._propertyValueDict[charId2][type];
		return this._descSort ? (value2 - value) : (value - value2);
	}

	// Token: 0x06001EE9 RID: 7913 RVA: 0x000DFC90 File Offset: 0x000DDE90
	private int SortByHealth(int charId1, int charId2)
	{
		float health = (float)this._healthDict[charId1] / (float)this._leftMaxHealthDict[charId1];
		float health2 = (float)this._healthDict[charId2] / (float)this._leftMaxHealthDict[charId2];
		int res = (health2 - health > 0f) ? 1 : ((health2 - health < 0f) ? -1 : 0);
		return this._descSort ? res : (-res);
	}

	// Token: 0x06001EEA RID: 7914 RVA: 0x000DFD04 File Offset: 0x000DDF04
	private int SortByCommand(int charId1, int charId2, UI_CharacterMenuTeam.TeamPropertyType type)
	{
		int command = this._propertyValueDict[charId1][type];
		int command2 = this._propertyValueDict[charId2][type];
		bool flag = command < 0 && !this._descSort;
		if (flag)
		{
			command = int.MaxValue;
		}
		bool flag2 = command2 < 0 && !this._descSort;
		if (flag2)
		{
			command2 = int.MaxValue;
		}
		bool flag3 = command == command2;
		if (flag3)
		{
			for (int i = 0; i < 3; i++)
			{
				UI_CharacterMenuTeam.TeamPropertyType tempType = UI_CharacterMenuTeam.TeamPropertyType.Command0 + i;
				bool flag4 = tempType != type;
				if (flag4)
				{
					command = this._propertyValueDict[charId1][tempType];
					command2 = this._propertyValueDict[charId2][tempType];
					bool flag5 = command < 0 && !this._descSort;
					if (flag5)
					{
						command = int.MaxValue;
					}
					bool flag6 = command2 < 0 && !this._descSort;
					if (flag6)
					{
						command2 = int.MaxValue;
					}
					bool flag7 = command != command2;
					if (flag7)
					{
						return this._descSort ? (command2 - command) : (command - command2);
					}
				}
			}
		}
		return this._descSort ? (command2 - command) : (command - command2);
	}

	// Token: 0x06001EEB RID: 7915 RVA: 0x000DFE4C File Offset: 0x000DE04C
	private int SortByName(int charId1, int charId2)
	{
		return (!this._descSort) ? Utils_Sorting.CompareByCurrentLangEncoding(this._nameDict[charId1], this._nameDict[charId2]) : Utils_Sorting.CompareByCurrentLangEncoding(this._nameDict[charId2], this._nameDict[charId1]);
	}

	// Token: 0x06001EEC RID: 7916 RVA: 0x000DFEA4 File Offset: 0x000DE0A4
	private int SortByCharm(int charId1, int charId2)
	{
		int charm = this._propertyValueDict[charId1][UI_CharacterMenuTeam.TeamPropertyType.Charm];
		int charm2 = this._propertyValueDict[charId2][UI_CharacterMenuTeam.TeamPropertyType.Charm];
		UI_CharacterMenuTeam.CharmData charmData = this._charmDataDict[charId1];
		UI_CharacterMenuTeam.CharmData charmData2 = this._charmDataDict[charId2];
		charm = this.<SortByCharm>g__MapCharm|61_0(charId1, charm, charmData);
		charm2 = this.<SortByCharm>g__MapCharm|61_0(charId2, charm2, charmData2);
		return this._descSort ? (charm2 - charm) : (charm - charm2);
	}

	// Token: 0x06001EED RID: 7917 RVA: 0x000DFF20 File Offset: 0x000DE120
	private int SortByBehavior(int charId1, int charId2)
	{
		int value = this._propertyValueDict[charId1][UI_CharacterMenuTeam.TeamPropertyType.Behavior];
		int value2 = this._propertyValueDict[charId2][UI_CharacterMenuTeam.TeamPropertyType.Behavior];
		return this._descSort ? (value - value2) : (value2 - value);
	}

	// Token: 0x06001EEE RID: 7918 RVA: 0x000DFF68 File Offset: 0x000DE168
	private int SortBySkillGrowth(int charId1, int charId2, UI_CharacterMenuTeam.TeamPropertyType type)
	{
		int addValue = (int)this.GetSkillGrowthAddValue(charId1, this._propertyValueDict[charId1][type]);
		int addValue2 = (int)this.GetSkillGrowthAddValue(charId2, this._propertyValueDict[charId2][type]);
		return this._descSort ? (addValue2 - addValue) : (addValue - addValue2);
	}

	// Token: 0x06001EEF RID: 7919 RVA: 0x000DFFC0 File Offset: 0x000DE1C0
	private void OnRenderChar(int index, Refers charRefers)
	{
		int charId = this._charIdList[index];
		CommonHorizontalLayoutGrid tableRow;
		bool flag = charRefers.TryGetComponent<CommonHorizontalLayoutGrid>(out tableRow);
		if (flag)
		{
			tableRow.SetRowBg(index);
		}
		this.UpdateCharInfo(charId, charRefers);
	}

	// Token: 0x06001EF0 RID: 7920 RVA: 0x000DFFFC File Offset: 0x000DE1FC
	private void UpdateCharInfo(int charId, Refers charRefers)
	{
		Dictionary<UI_CharacterMenuTeam.TeamPropertyType, int> valueDict;
		bool flag = !this._propertyValueDict.TryGetValue(charId, out valueDict);
		if (!flag)
		{
			bool hideProperty = this.GetCharHideProperty(charId);
			switch (this._subPageType)
			{
			case UI_CharacterMenuTeam.ESubPage.State:
				this.UpdateCharInfoState(charId, charRefers, valueDict, hideProperty);
				break;
			case UI_CharacterMenuTeam.ESubPage.Property:
				this.UpdateCharInfoProperty(charId, charRefers, valueDict);
				break;
			case UI_CharacterMenuTeam.ESubPage.Property2:
				this.UpdateCharInfoProperty2(charId, charRefers, valueDict);
				break;
			case UI_CharacterMenuTeam.ESubPage.LifeSkill:
				this.UpdateCharInfoLifeSkill(charId, charRefers, valueDict);
				break;
			case UI_CharacterMenuTeam.ESubPage.CombatSkill:
				this.UpdateCharInfoCombatSkill(charId, charRefers, valueDict);
				break;
			case UI_CharacterMenuTeam.ESubPage.Personality:
				this.UpdateCharInfoPersonality(charId, charRefers, valueDict);
				break;
			case UI_CharacterMenuTeam.ESubPage.Item:
				this.UpdateCharInfoItem(charId, charRefers, valueDict);
				break;
			case UI_CharacterMenuTeam.ESubPage.Command:
				this.UpdateCharInfoCommand(charId, charRefers, valueDict);
				break;
			}
		}
	}

	// Token: 0x06001EF1 RID: 7921 RVA: 0x000E00C8 File Offset: 0x000DE2C8
	private void UpdateCharInfoCommand(int charId, Refers charRefers, Dictionary<UI_CharacterMenuTeam.TeamPropertyType, int> valueDict)
	{
		List<Refers> commandList = charRefers.CGetList<Refers>("Command_");
		CImage attackMedalIcon = charRefers.CGet<CImage>("AttackMedalIcon");
		CImage defenceMedalIcon = charRefers.CGet<CImage>("DefenceMedalIcon");
		CImage wisdomMedalIcon = charRefers.CGet<CImage>("WisdomMedalIcon");
		TextMeshProUGUI attackMedalCountLabel = charRefers.CGet<TextMeshProUGUI>("AttackMedalCountLabel");
		TextMeshProUGUI defenceMedalCountLabel = charRefers.CGet<TextMeshProUGUI>("DefenceMedalCountLabel");
		TextMeshProUGUI wisdomMedalCountLabel = charRefers.CGet<TextMeshProUGUI>("WisdomMedalCountLabel");
		int attackMedalCount = valueDict[UI_CharacterMenuTeam.TeamPropertyType.AttackMedal];
		int defenceMedalCount = valueDict[UI_CharacterMenuTeam.TeamPropertyType.DefenceMedal];
		int wisdomMedalCount = valueDict[UI_CharacterMenuTeam.TeamPropertyType.WisdomMedal];
		CommonHorizontalLayoutGrid tableRow = charRefers.GetComponent<CommonHorizontalLayoutGrid>();
		this.UpdateCharInfoCommon(charId, charRefers);
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.AttackMedal, attackMedalCountLabel, tableRow, 0, false, (attackMedalCount != 0) ? string.Format(" x{0}", Mathf.Abs(attackMedalCount)) : "-", null);
		attackMedalIcon.gameObject.SetActive(attackMedalCount != 0);
		bool flag = attackMedalCount != 0;
		if (flag)
		{
			attackMedalIcon.SetSprite(((attackMedalCount >= 0) ? UI_CharacterMenuTeam.PositiveFeatureIcon : UI_CharacterMenuTeam.NegativeFeatureIcon)[0], false, null);
		}
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.DefenceMedal, defenceMedalCountLabel, tableRow, 1, false, (defenceMedalCount != 0) ? string.Format(" x{0}", Mathf.Abs(defenceMedalCount)) : "-", null);
		defenceMedalIcon.gameObject.SetActive(defenceMedalCount != 0);
		bool flag2 = defenceMedalCount != 0;
		if (flag2)
		{
			defenceMedalIcon.SetSprite(((defenceMedalCount >= 0) ? UI_CharacterMenuTeam.PositiveFeatureIcon : UI_CharacterMenuTeam.NegativeFeatureIcon)[1], false, null);
		}
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.WisdomMedal, wisdomMedalCountLabel, tableRow, 2, false, (wisdomMedalCount != 0) ? string.Format(" x{0}", Mathf.Abs(wisdomMedalCount)) : "-", null);
		wisdomMedalIcon.gameObject.SetActive(wisdomMedalCount != 0);
		bool flag3 = wisdomMedalCount != 0;
		if (flag3)
		{
			wisdomMedalIcon.SetSprite(((wisdomMedalCount >= 0) ? UI_CharacterMenuTeam.PositiveFeatureIcon : UI_CharacterMenuTeam.NegativeFeatureIcon)[2], false, null);
		}
		for (sbyte i = 0; i < 3; i += 1)
		{
			int value = valueDict[UI_CharacterMenuTeam.TeamPropertyType.Command0 + (int)i];
			bool valid = value > -1;
			Refers commandItem = commandList[(int)i];
			commandItem.gameObject.SetActive(valid);
			bool flag4 = !valid;
			if (!flag4)
			{
				List<TextMeshProUGUI> nameLabelList = commandItem.CGetList<TextMeshProUGUI>("NameLabel");
				TeammateCommandItem cmdConfig = TeammateCommand.Instance[value];
				foreach (TextMeshProUGUI commandNameLabel in nameLabelList)
				{
					commandNameLabel.text = cmdConfig.Name;
				}
			}
		}
	}

	// Token: 0x06001EF2 RID: 7922 RVA: 0x000E037C File Offset: 0x000DE57C
	private void UpdateCharInfoState(int charId, Refers charRefers, Dictionary<UI_CharacterMenuTeam.TeamPropertyType, int> valueDict, bool hideProperty)
	{
		TextMeshProUGUI ageLabel = charRefers.CGet<TextMeshProUGUI>("AgeLabel");
		TextMeshProUGUI behaviorLabel = charRefers.CGet<TextMeshProUGUI>("BehaviorLabel");
		TextMeshProUGUI charmLabel = charRefers.CGet<TextMeshProUGUI>("CharmLabel");
		TextMeshProUGUI fameLabel = charRefers.CGet<TextMeshProUGUI>("FameLabel");
		TextMeshProUGUI favorLabel = charRefers.CGet<TextMeshProUGUI>("FavorLabel");
		TextMeshProUGUI happinessLabel = charRefers.CGet<TextMeshProUGUI>("HappinessLabel");
		TextMeshProUGUI healthLabel = charRefers.CGet<TextMeshProUGUI>("HealthLabel");
		TextMeshProUGUI injuryLabel = charRefers.CGet<TextMeshProUGUI>("InjuryLabel");
		TextMeshProUGUI samsaraLabel = charRefers.CGet<TextMeshProUGUI>("SamsaraLabel");
		CommonHorizontalLayoutGrid tableRow = charRefers.GetComponent<CommonHorizontalLayoutGrid>();
		this.UpdateCharInfoCommon(charId, charRefers);
		UI_CharacterMenuTeam.CharmData charmData = this._charmDataDict[charId];
		bool hideAge = this.GetCharHideAge(charId);
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.Age, ageLabel, tableRow, 0, hideAge, this._ageDict[charId].ToString(), null);
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.Health, healthLabel, tableRow, 1, hideProperty, CommonUtils.GetCharacterHealthInfo((short)valueDict[UI_CharacterMenuTeam.TeamPropertyType.Health], this._leftMaxHealthDict[charId], charId).Item1, null);
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.DefeatMark, injuryLabel, tableRow, 2, false, null, null);
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.Charm, charmLabel, tableRow, 3, false, CommonUtils.GetCharmLevelText((short)valueDict[UI_CharacterMenuTeam.TeamPropertyType.Charm], charmData.Gender, charmData.PhysiologicalAge, charmData.ClothDisplayId, CreatingType.IsFixedPresetType(charmData.CreatingType), charmData.FaceVisible), null);
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.Behavior, behaviorLabel, tableRow, 4, false, CommonUtils.GetBehaviorString((sbyte)valueDict[UI_CharacterMenuTeam.TeamPropertyType.Behavior]), null);
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.Happiness, happinessLabel, tableRow, 5, false, CommonUtils.GetHappinessString(HappinessType.GetHappinessType((sbyte)valueDict[UI_CharacterMenuTeam.TeamPropertyType.Happiness])), null);
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.Favor, favorLabel, tableRow, 6, false, CommonUtils.GetFavorStringByInteracted((short)valueDict[UI_CharacterMenuTeam.TeamPropertyType.Favor], this._interactedDict[charId]), null);
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.Samsara, samsaraLabel, tableRow, 7, false, null, null);
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.Fame, fameLabel, tableRow, 8, false, CommonUtils.GetFameString(FameType.GetFameType((sbyte)valueDict[UI_CharacterMenuTeam.TeamPropertyType.Fame])), null);
	}

	// Token: 0x06001EF3 RID: 7923 RVA: 0x000E05BC File Offset: 0x000DE7BC
	private void UpdateCharInfoProperty(int charId, Refers charRefers, Dictionary<UI_CharacterMenuTeam.TeamPropertyType, int> valueDict)
	{
		List<TextMeshProUGUI> mainAttributeLabelList = charRefers.CGetList<TextMeshProUGUI>("MainAttributeLabel_");
		TextMeshProUGUI innerPenetrateLabel = charRefers.CGet<TextMeshProUGUI>("InnerPenetrateLabel");
		TextMeshProUGUI innerPenetrateResistLabel = charRefers.CGet<TextMeshProUGUI>("InnerPenetrateResistLabel");
		TextMeshProUGUI outerPenetrateLabel = charRefers.CGet<TextMeshProUGUI>("OuterPenetrateLabel");
		TextMeshProUGUI outerPenetrateResistLabel = charRefers.CGet<TextMeshProUGUI>("OuterPenetrateResistLabel");
		CommonHorizontalLayoutGrid tableRow = charRefers.GetComponent<CommonHorizontalLayoutGrid>();
		this.UpdateCharInfoCommon(charId, charRefers);
		for (sbyte i = 0; i < 6; i += 1)
		{
			TextMeshProUGUI mainAttributeLabel = mainAttributeLabelList[(int)i];
			this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.MainAttribute0 + (int)i, mainAttributeLabel, tableRow, (int)i, false, null, null);
		}
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.OuterPenetrate, outerPenetrateLabel, tableRow, 6, false, null, null);
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.InnerPenetrate, innerPenetrateLabel, tableRow, 7, false, null, null);
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.OuterPenetrateResist, outerPenetrateResistLabel, tableRow, 8, false, null, null);
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.InnerPenetrateResist, innerPenetrateResistLabel, tableRow, 9, false, null, null);
	}

	// Token: 0x06001EF4 RID: 7924 RVA: 0x000E06C4 File Offset: 0x000DE8C4
	private void UpdateCharInfoProperty2(int charId, Refers charRefers, Dictionary<UI_CharacterMenuTeam.TeamPropertyType, int> valueDict)
	{
		List<TextMeshProUGUI> avoidLabelList = charRefers.CGetList<TextMeshProUGUI>("AvoidLabel_");
		List<TextMeshProUGUI> hitLabelList = charRefers.CGetList<TextMeshProUGUI>("HitLabel_");
		TextMeshProUGUI qiDisorderLabel = charRefers.CGet<TextMeshProUGUI>("QiDisorderLabel");
		CommonHorizontalLayoutGrid tableRow = charRefers.GetComponent<CommonHorizontalLayoutGrid>();
		this.UpdateCharInfoCommon(charId, charRefers);
		for (int i = 0; i < 4; i++)
		{
			TextMeshProUGUI hitLabel = hitLabelList[i];
			this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.Hit0 + i - 0, hitLabel, tableRow, i, false, null, null);
		}
		for (int j = 4; j < 8; j++)
		{
			TextMeshProUGUI avoidLabel = avoidLabelList[j - 4];
			this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.Avoid0 + j - 4, avoidLabel, tableRow, j, false, null, null);
		}
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.QiDisorder, qiDisorderLabel, tableRow, 8, false, null, null);
	}

	// Token: 0x06001EF5 RID: 7925 RVA: 0x000E07A8 File Offset: 0x000DE9A8
	private void UpdateCharInfoLifeSkill(int charId, Refers charRefers, Dictionary<UI_CharacterMenuTeam.TeamPropertyType, int> valueDict)
	{
		List<TextMeshProUGUI> lifeSkillLabelList = charRefers.CGetList<TextMeshProUGUI>("LifeSkillLabel_");
		TextMeshProUGUI growthLabel = charRefers.CGet<TextMeshProUGUI>("GrowthLabel");
		CommonHorizontalLayoutGrid tableRow = charRefers.GetComponent<CommonHorizontalLayoutGrid>();
		this.UpdateCharInfoCommon(charId, charRefers);
		for (sbyte i = 0; i < 16; i += 1)
		{
			TextMeshProUGUI lifeSkillLabel = lifeSkillLabelList[(int)i];
			this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.LifeSkill0 + (int)i, lifeSkillLabel, tableRow, (int)i, false, null, null);
		}
		string growthString = this.GetSkillGrowthString(charId, valueDict[UI_CharacterMenuTeam.TeamPropertyType.LifeSkillGrowth]);
		sbyte skillGrowthAddValue = this.GetSkillGrowthAddValue(charId, valueDict[UI_CharacterMenuTeam.TeamPropertyType.LifeSkillGrowth]);
		bool showSpecialBg = !this._allValueEqual[46] && (int)skillGrowthAddValue >= this._maxValueDict[UI_CharacterMenuTeam.TeamPropertyType.LifeSkillGrowth];
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.LifeSkillGrowth, growthLabel, tableRow, 16, false, growthString, new bool?(showSpecialBg));
	}

	// Token: 0x06001EF6 RID: 7926 RVA: 0x000E087C File Offset: 0x000DEA7C
	private void UpdateCharInfoCombatSkill(int charId, Refers charRefers, Dictionary<UI_CharacterMenuTeam.TeamPropertyType, int> valueDict)
	{
		List<TextMeshProUGUI> combatSkillLabelList = charRefers.CGetList<TextMeshProUGUI>("CombatSkillLabel_");
		TextMeshProUGUI growthLabel = charRefers.CGet<TextMeshProUGUI>("GrowthLabel");
		CommonHorizontalLayoutGrid tableRow = charRefers.GetComponent<CommonHorizontalLayoutGrid>();
		this.UpdateCharInfoCommon(charId, charRefers);
		for (sbyte i = 0; i < 14; i += 1)
		{
			TextMeshProUGUI combatSkillLabel = combatSkillLabelList[(int)i];
			this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.CombatSkill0 + (int)i, combatSkillLabel, tableRow, (int)i, false, null, null);
		}
		string growthString = this.GetSkillGrowthString(charId, valueDict[UI_CharacterMenuTeam.TeamPropertyType.CombatSkillGrowth]);
		sbyte skillGrowthAddValue = this.GetSkillGrowthAddValue(charId, valueDict[UI_CharacterMenuTeam.TeamPropertyType.CombatSkillGrowth]);
		bool showSpecialBg = !this._allValueEqual[61] && (int)skillGrowthAddValue >= this._maxValueDict[UI_CharacterMenuTeam.TeamPropertyType.CombatSkillGrowth];
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.CombatSkillGrowth, growthLabel, tableRow, 14, false, growthString, new bool?(showSpecialBg));
	}

	// Token: 0x06001EF7 RID: 7927 RVA: 0x000E0950 File Offset: 0x000DEB50
	private void UpdateCharInfoPersonality(int charId, Refers charRefers, Dictionary<UI_CharacterMenuTeam.TeamPropertyType, int> valueDict)
	{
		List<TextMeshProUGUI> personalityLabelList = charRefers.CGetList<TextMeshProUGUI>("PersonalityLabel_");
		CommonHorizontalLayoutGrid tableRow = charRefers.GetComponent<CommonHorizontalLayoutGrid>();
		this.UpdateCharInfoCommon(charId, charRefers);
		for (sbyte i = 0; i < 7; i += 1)
		{
			TextMeshProUGUI personalityLabel = personalityLabelList[(int)i];
			this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.Personality0 + (int)i, personalityLabel, tableRow, (int)i, false, null, null);
		}
	}

	// Token: 0x06001EF8 RID: 7928 RVA: 0x000E09B4 File Offset: 0x000DEBB4
	private void UpdateCharInfoItem(int charId, Refers charRefers, Dictionary<UI_CharacterMenuTeam.TeamPropertyType, int> valueDict)
	{
		List<TextMeshProUGUI> resourceLabelList = charRefers.CGetList<TextMeshProUGUI>("ResourceLabel_");
		TextMeshProUGUI inventoryLoadLabel = charRefers.CGet<TextMeshProUGUI>("InventoryLoadLabel");
		TextMeshProUGUI kidnapCountLabel = charRefers.CGet<TextMeshProUGUI>("KidnapCountLabel");
		CommonHorizontalLayoutGrid tableRow = charRefers.GetComponent<CommonHorizontalLayoutGrid>();
		this.UpdateCharInfoCommon(charId, charRefers);
		for (sbyte i = 0; i < 8; i += 1)
		{
			TextMeshProUGUI resourceLabel = resourceLabelList[(int)i];
			this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.Resource0 + (int)i, resourceLabel, tableRow, (int)i, false, null, null);
		}
		int currLoad = valueDict[UI_CharacterMenuTeam.TeamPropertyType.InventoryLoad];
		int maxLoad = this._maxInventoryLoadDict[charId];
		string currLoadStr = ((float)currLoad / 100f).ToString("f1").SetColor(CommonUtils.GetLoadWeightValueColor(currLoad, maxLoad));
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.InventoryLoad, inventoryLoadLabel, tableRow, 8, false, string.Format("{0}/{1:f1}", currLoadStr, (float)maxLoad / 100f), null);
		this.UpdatePropertyValueTex(valueDict, UI_CharacterMenuTeam.TeamPropertyType.KidnapCount, kidnapCountLabel, tableRow, 9, false, null, null);
	}

	// Token: 0x06001EF9 RID: 7929 RVA: 0x000E0AC4 File Offset: 0x000DECC4
	private void UpdateCharInfoCommon(int charId, Refers charRefers)
	{
		Game.Components.Avatar.Avatar avatar = charRefers.CGet<Game.Components.Avatar.Avatar>("Avatar");
		TextMeshProUGUI nameLabel = charRefers.CGet<TextMeshProUGUI>("NameLabel");
		TooltipInvoker headTips = charRefers.CGet<TooltipInvoker>("HeadTips");
		CButtonObsolete selectButton = charRefers.CGet<CButtonObsolete>("SelectButton");
		GameObject selectedMark = charRefers.CGet<GameObject>("CommonSelected");
		selectedMark.SetActive(base.CharacterMenu.CurCharacterId == charId);
		avatar.Refresh(this._avatarRelatedDataDict[charId], this._templateIdDict[charId]);
		nameLabel.text = this._nameDict[charId];
		TooltipInvoker tooltipInvoker = headTips;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		headTips.RuntimeParam.Set("charId", charId);
		CButtonObsolete headButton = headTips.GetComponent<CButtonObsolete>();
		headButton.ClearAndAddListener(delegate
		{
			this.CharacterMenu.SelectCharacter(charId);
			this._charScroll.ReRender();
		});
		selectButton.ClearAndAddListener(delegate
		{
			this.CharacterMenu.SelectCharacter(charId);
			this._charScroll.ReRender();
		});
	}

	// Token: 0x06001EFA RID: 7930 RVA: 0x000E0BE4 File Offset: 0x000DEDE4
	private bool GetCharHideAge(int charId)
	{
		UI_CharacterMenuTeam.CharmData charmData = this._charmDataDict[charId];
		bool isNonEvolutionaryType = CreatingType.IsNonEvolutionaryType(charmData.CreatingType);
		return Character.Instance[this._templateIdDict[charId]].HideAge && isNonEvolutionaryType;
	}

	// Token: 0x06001EFB RID: 7931 RVA: 0x000E0C30 File Offset: 0x000DEE30
	private bool GetCharHideProperty(int charId)
	{
		UI_CharacterMenuTeam.CharmData charmData = this._charmDataDict[charId];
		return CreatingType.IsNonEvolutionaryType(charmData.CreatingType);
	}

	// Token: 0x06001EFC RID: 7932 RVA: 0x000E0C5C File Offset: 0x000DEE5C
	private void UpdatePropertyValueTex(Dictionary<UI_CharacterMenuTeam.TeamPropertyType, int> valueDict, UI_CharacterMenuTeam.TeamPropertyType type, TextMeshProUGUI valueText, CommonHorizontalLayoutGrid tableRow, int columnLogicIndex, bool convertText = false, string customStr = null, bool? customShowSpecialBg = null)
	{
		string text = valueText.text = (customStr ?? valueDict[type].ToString());
		if (convertText)
		{
			byte creatingType = (byte)valueDict[UI_CharacterMenuTeam.TeamPropertyType.CreatingType];
			text = UI_CharacterMenuTeam.GetConvertValueText(text, creatingType);
		}
		valueText.text = text;
		bool showSpecialBg = customShowSpecialBg ?? (!this._allValueEqual[(int)type] && valueDict[type] >= this._maxValueDict[type]);
		tableRow.SetSpecialBg(columnLogicIndex + 2, showSpecialBg);
	}

	// Token: 0x06001EFD RID: 7933 RVA: 0x000E0CFC File Offset: 0x000DEEFC
	private string GetSkillGrowthString(int charId, int growthType)
	{
		int addValue = (int)this.GetSkillGrowthAddValue(charId, growthType);
		StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
		strBuilder.Clear();
		strBuilder.Append((growthType == 0) ? LocalStringManager.Get("LK_Qualification_Growth_Average") : ((growthType == 1) ? LocalStringManager.Get("LK_Qualification_Growth_Precocious") : LocalStringManager.Get("LK_Qualification_Growth_LateBlooming")));
		bool flag = addValue > 0;
		if (flag)
		{
			strBuilder.Append(string.Format("+{0}", addValue).SetColor("lightblue"));
		}
		else
		{
			bool flag2 = addValue == 0;
			if (flag2)
			{
				strBuilder.Append("+0".SetColor("lightgrey"));
			}
			else
			{
				strBuilder.Append(string.Format("{0}", addValue).SetColor("red"));
			}
		}
		return strBuilder.ToString();
	}

	// Token: 0x06001EFE RID: 7934 RVA: 0x000E0DC8 File Offset: 0x000DEFC8
	private sbyte GetSkillGrowthAddValue(int charId, int growthType)
	{
		AgeEffectItem ageData = AgeEffect.Instance[Mathf.Min(this._propertyValueDict[charId][UI_CharacterMenuTeam.TeamPropertyType.ActualAge], AgeEffect.Instance.Count - 1)];
		return (growthType == 0) ? ageData.SkillQualificationAverage : ((growthType == 1) ? ageData.SkillQualificationPrecocious : ageData.SkillQualificationLateBlooming);
	}

	// Token: 0x06001EFF RID: 7935 RVA: 0x000E0E28 File Offset: 0x000DF028
	private static string GetConvertValueText(string text, byte creatingType)
	{
		return CommonUtils.ConvertValueByCreatingType(creatingType, text);
	}

	// Token: 0x06001F00 RID: 7936 RVA: 0x000E0E44 File Offset: 0x000DF044
	private static GameObject GetSortButtonArrow(Refers sortButtonRefers)
	{
		GameObject arrowObj;
		bool flag = sortButtonRefers.CTryGet<GameObject>("Arrow", out arrowObj) && arrowObj;
		GameObject result;
		if (flag)
		{
			result = arrowObj;
		}
		else
		{
			result = sortButtonRefers.CGet<RectTransform>("Arrow").gameObject;
		}
		return result;
	}

	// Token: 0x06001F06 RID: 7942 RVA: 0x000E1784 File Offset: 0x000DF984
	[CompilerGenerated]
	private int <SortByCharm>g__MapCharm|61_0(int charId, int baseCharm, UI_CharacterMenuTeam.CharmData charmData)
	{
		bool isSpecialTeammate = base.CharacterMenu.IsTaiwuSpecialTeammate(charId);
		bool flag = isSpecialTeammate;
		int result;
		if (flag)
		{
			result = baseCharm;
		}
		else
		{
			result = ((charmData.PhysiologicalAge < 16) ? -200 : ((charmData.ClothDisplayId == 0) ? -100 : baseCharm));
		}
		return result;
	}

	// Token: 0x04001752 RID: 5970
	private UI_CharacterMenuTeam.ESubPage _subPageType;

	// Token: 0x04001753 RID: 5971
	private static readonly List<LanguageKey> ToggleGroupNameKeys = new List<LanguageKey>
	{
		LanguageKey.LK_Team_Tog_State,
		LanguageKey.LK_Team_Tog_Property,
		LanguageKey.LK_Team_Tog_Property_Hit,
		LanguageKey.LK_Team_Tog_LifeSkill,
		LanguageKey.LK_Team_Tog_CombatSkill,
		LanguageKey.LK_Team_Tog_Personality,
		LanguageKey.LK_Team_Tog_Item,
		LanguageKey.LK_Team_Tog_Command
	};

	// Token: 0x04001754 RID: 5972
	private static readonly List<LanguageKey> StateTitleNameKeys = new List<LanguageKey>
	{
		LanguageKey.LK_Char_Name,
		LanguageKey.LK_Char_Age,
		LanguageKey.LK_Health,
		LanguageKey.LK_Injury,
		LanguageKey.LK_Main_SummaryInfo_Charm,
		LanguageKey.LK_Main_SummaryInfo_Behavior,
		LanguageKey.LK_Main_SummaryInfo_Happiness,
		LanguageKey.LK_Favorability,
		LanguageKey.LK_Samsara,
		LanguageKey.LK_Main_SummaryInfo_Fame,
		LanguageKey.LK_Feature_Attack,
		LanguageKey.LK_Feature_Defence,
		LanguageKey.LK_Feature_Wisdom
	};

	// Token: 0x04001755 RID: 5973
	private static readonly List<LanguageKey> PropertyTitleNameKeys = new List<LanguageKey>
	{
		LanguageKey.LK_Char_Name,
		LanguageKey.LK_Main_Attribute_Strength,
		LanguageKey.LK_Main_Attribute_Dexterity,
		LanguageKey.LK_Main_Attribute_Concentration,
		LanguageKey.LK_Main_Attribute_Vitality,
		LanguageKey.LK_Main_Attribute_Energy,
		LanguageKey.LK_Main_Attribute_Intelligence,
		LanguageKey.LK_Penetrate_Outer,
		LanguageKey.LK_Penetrate_Inner,
		LanguageKey.LK_Penetrate_Resist_Outer,
		LanguageKey.LK_Penetrate_Resist_Inner
	};

	// Token: 0x04001756 RID: 5974
	private static readonly List<LanguageKey> Property2TitleNameKeys = new List<LanguageKey>
	{
		LanguageKey.LK_Char_Name,
		LanguageKey.LK_HitType_0,
		LanguageKey.LK_HitType_1,
		LanguageKey.LK_HitType_2,
		LanguageKey.LK_HitType_3,
		LanguageKey.LK_AvoidType_0,
		LanguageKey.LK_AvoidType_1,
		LanguageKey.LK_AvoidType_2,
		LanguageKey.LK_AvoidType_3,
		LanguageKey.LK_Qi_Disorder
	};

	// Token: 0x04001757 RID: 5975
	private static readonly List<LanguageKey> LifeSkillTitleNameKeys = new List<LanguageKey>
	{
		LanguageKey.LK_Char_Name,
		LanguageKey.LK_LifeSkillType_0,
		LanguageKey.LK_LifeSkillType_1,
		LanguageKey.LK_LifeSkillType_2,
		LanguageKey.LK_LifeSkillType_3,
		LanguageKey.LK_LifeSkillType_4,
		LanguageKey.LK_LifeSkillType_5,
		LanguageKey.LK_LifeSkillType_6,
		LanguageKey.LK_LifeSkillType_7,
		LanguageKey.LK_LifeSkillType_8,
		LanguageKey.LK_LifeSkillType_9,
		LanguageKey.LK_LifeSkillType_10,
		LanguageKey.LK_LifeSkillType_11,
		LanguageKey.LK_LifeSkillType_12,
		LanguageKey.LK_LifeSkillType_13,
		LanguageKey.LK_LifeSkillType_14,
		LanguageKey.LK_LifeSkillType_15,
		LanguageKey.LK_Growth
	};

	// Token: 0x04001758 RID: 5976
	public static readonly List<LanguageKey> LifeSkillNameKeys = new List<LanguageKey>
	{
		LanguageKey.LK_LifeSkillType_0,
		LanguageKey.LK_LifeSkillType_1,
		LanguageKey.LK_LifeSkillType_2,
		LanguageKey.LK_LifeSkillType_3,
		LanguageKey.LK_LifeSkillType_4,
		LanguageKey.LK_LifeSkillType_5,
		LanguageKey.LK_LifeSkillType_6,
		LanguageKey.LK_LifeSkillType_7,
		LanguageKey.LK_LifeSkillType_8,
		LanguageKey.LK_LifeSkillType_9,
		LanguageKey.LK_LifeSkillType_10,
		LanguageKey.LK_LifeSkillType_11,
		LanguageKey.LK_LifeSkillType_12,
		LanguageKey.LK_LifeSkillType_13,
		LanguageKey.LK_LifeSkillType_14,
		LanguageKey.LK_LifeSkillType_15
	};

	// Token: 0x04001759 RID: 5977
	private static readonly List<LanguageKey> CombatSkillTitleNameKeys = new List<LanguageKey>
	{
		LanguageKey.LK_Char_Name,
		LanguageKey.LK_CombatSkillType_0,
		LanguageKey.LK_CombatSkillType_1,
		LanguageKey.LK_CombatSkillType_2,
		LanguageKey.LK_CombatSkillType_3,
		LanguageKey.LK_CombatSkillType_4,
		LanguageKey.LK_CombatSkillType_5,
		LanguageKey.LK_CombatSkillType_6,
		LanguageKey.LK_CombatSkillType_7,
		LanguageKey.LK_CombatSkillType_8,
		LanguageKey.LK_CombatSkillType_9,
		LanguageKey.LK_CombatSkillType_10,
		LanguageKey.LK_CombatSkillType_11,
		LanguageKey.LK_CombatSkillType_12,
		LanguageKey.LK_CombatSkillType_13,
		LanguageKey.LK_Growth
	};

	// Token: 0x0400175A RID: 5978
	public static readonly List<LanguageKey> CombatSkillNameKeys = new List<LanguageKey>
	{
		LanguageKey.LK_CombatSkillType_0,
		LanguageKey.LK_CombatSkillType_1,
		LanguageKey.LK_CombatSkillType_2,
		LanguageKey.LK_CombatSkillType_3,
		LanguageKey.LK_CombatSkillType_4,
		LanguageKey.LK_CombatSkillType_5,
		LanguageKey.LK_CombatSkillType_6,
		LanguageKey.LK_CombatSkillType_7,
		LanguageKey.LK_CombatSkillType_8,
		LanguageKey.LK_CombatSkillType_9,
		LanguageKey.LK_CombatSkillType_10,
		LanguageKey.LK_CombatSkillType_11,
		LanguageKey.LK_CombatSkillType_12,
		LanguageKey.LK_CombatSkillType_13
	};

	// Token: 0x0400175B RID: 5979
	private static readonly List<LanguageKey> PersonalityTitleNameKeys = new List<LanguageKey>
	{
		LanguageKey.LK_Char_Name,
		LanguageKey.LK_Personality_Calm_Name,
		LanguageKey.LK_Personality_Clever_Name,
		LanguageKey.LK_Personality_Enthusiastic_Name,
		LanguageKey.LK_Personality_Brave_Name,
		LanguageKey.LK_Personality_Firm_Name,
		LanguageKey.LK_Personality_Lucky_Name,
		LanguageKey.LK_Personality_Perceptive_Name
	};

	// Token: 0x0400175C RID: 5980
	public static readonly List<LanguageKey> PersonalityNameKeys = new List<LanguageKey>
	{
		LanguageKey.LK_Personality_Calm_Name,
		LanguageKey.LK_Personality_Clever_Name,
		LanguageKey.LK_Personality_Enthusiastic_Name,
		LanguageKey.LK_Personality_Brave_Name,
		LanguageKey.LK_Personality_Firm_Name,
		LanguageKey.LK_Personality_Lucky_Name,
		LanguageKey.LK_Personality_Perceptive_Name
	};

	// Token: 0x0400175D RID: 5981
	private static readonly List<LanguageKey> ItemTitleNameKeys = new List<LanguageKey>
	{
		LanguageKey.LK_Char_Name,
		LanguageKey.LK_Resource_Name_Food,
		LanguageKey.LK_Resource_Name_Wood,
		LanguageKey.LK_Resource_Name_Metal,
		LanguageKey.LK_Resource_Name_Jade,
		LanguageKey.LK_Resource_Name_Fabric,
		LanguageKey.LK_Resource_Name_Herb,
		LanguageKey.LK_Resource_Name_Money,
		LanguageKey.LK_Resource_Name_Authority,
		LanguageKey.LK_Inventory,
		LanguageKey.LK_Kidnap
	};

	// Token: 0x0400175E RID: 5982
	private static readonly List<LanguageKey> CommandTitleNameKeys = new List<LanguageKey>
	{
		LanguageKey.LK_Char_Name,
		LanguageKey.LK_Feature_Attack,
		LanguageKey.LK_Feature_Defence,
		LanguageKey.LK_Feature_Wisdom,
		LanguageKey.LK_Team_Property_Title_Command_0,
		LanguageKey.LK_Team_Property_Title_Command_1,
		LanguageKey.LK_Team_Property_Title_Command_2
	};

	// Token: 0x0400175F RID: 5983
	private const int CommonColumnCount = 2;

	// Token: 0x04001760 RID: 5984
	private static readonly string[] PositiveFeatureIcon = new string[]
	{
		"ui_sp_icon_characteristic_10",
		"ui_sp_icon_characteristic_9",
		"ui_sp_icon_characteristic_11"
	};

	// Token: 0x04001761 RID: 5985
	private static readonly string[] NegativeFeatureIcon = new string[]
	{
		"ui_sp_icon_characteristic_4",
		"ui_sp_icon_characteristic_3",
		"ui_sp_icon_characteristic_5"
	};

	// Token: 0x04001762 RID: 5986
	private readonly List<int> _charIdList = new List<int>();

	// Token: 0x04001763 RID: 5987
	private readonly Dictionary<int, short> _templateIdDict = new Dictionary<int, short>();

	// Token: 0x04001764 RID: 5988
	private readonly Dictionary<int, string> _nameDict = new Dictionary<int, string>();

	// Token: 0x04001765 RID: 5989
	private readonly Dictionary<int, AvatarRelatedData> _avatarRelatedDataDict = new Dictionary<int, AvatarRelatedData>();

	// Token: 0x04001766 RID: 5990
	private readonly Dictionary<int, UI_CharacterMenuTeam.CharmData> _charmDataDict = new Dictionary<int, UI_CharacterMenuTeam.CharmData>();

	// Token: 0x04001767 RID: 5991
	private readonly Dictionary<int, short> _ageDict = new Dictionary<int, short>();

	// Token: 0x04001768 RID: 5992
	private readonly Dictionary<int, short> _healthDict = new Dictionary<int, short>();

	// Token: 0x04001769 RID: 5993
	private readonly Dictionary<int, short> _leftMaxHealthDict = new Dictionary<int, short>();

	// Token: 0x0400176A RID: 5994
	private readonly Dictionary<int, bool> _interactedDict = new Dictionary<int, bool>();

	// Token: 0x0400176B RID: 5995
	private readonly Dictionary<int, int> _maxInventoryLoadDict = new Dictionary<int, int>();

	// Token: 0x0400176C RID: 5996
	private readonly Dictionary<int, Dictionary<UI_CharacterMenuTeam.TeamPropertyType, int>> _propertyValueDict = new Dictionary<int, Dictionary<UI_CharacterMenuTeam.TeamPropertyType, int>>();

	// Token: 0x0400176D RID: 5997
	private readonly Dictionary<UI_CharacterMenuTeam.TeamPropertyType, int> _maxValueDict = new Dictionary<UI_CharacterMenuTeam.TeamPropertyType, int>();

	// Token: 0x0400176E RID: 5998
	private readonly bool[] _allValueEqual = new bool[86];

	// Token: 0x0400176F RID: 5999
	private readonly HashSet<CommonTableHead> _tableHeads = new HashSet<CommonTableHead>();

	// Token: 0x04001770 RID: 6000
	private RectTransform _titleHolder;

	// Token: 0x04001771 RID: 6001
	private InfinityScrollLegacy _charScroll;

	// Token: 0x04001772 RID: 6002
	private CButtonObsolete _sortBtn;

	// Token: 0x04001773 RID: 6003
	private bool _descSort;

	// Token: 0x04001774 RID: 6004
	[SerializeField]
	private CommonHorizontalLayoutGrid[] characterPrefabArray;

	// Token: 0x02001451 RID: 5201
	private enum TeamPropertyType
	{
		// Token: 0x0400A08E RID: 41102
		Name,
		// Token: 0x0400A08F RID: 41103
		Age,
		// Token: 0x0400A090 RID: 41104
		Health,
		// Token: 0x0400A091 RID: 41105
		DefeatMark,
		// Token: 0x0400A092 RID: 41106
		Charm,
		// Token: 0x0400A093 RID: 41107
		Behavior,
		// Token: 0x0400A094 RID: 41108
		Happiness,
		// Token: 0x0400A095 RID: 41109
		Favor,
		// Token: 0x0400A096 RID: 41110
		Samsara,
		// Token: 0x0400A097 RID: 41111
		Fame,
		// Token: 0x0400A098 RID: 41112
		ActualAge,
		// Token: 0x0400A099 RID: 41113
		MainAttribute0,
		// Token: 0x0400A09A RID: 41114
		MainAttribute1,
		// Token: 0x0400A09B RID: 41115
		MainAttribute2,
		// Token: 0x0400A09C RID: 41116
		MainAttribute3,
		// Token: 0x0400A09D RID: 41117
		MainAttribute4,
		// Token: 0x0400A09E RID: 41118
		MainAttribute5,
		// Token: 0x0400A09F RID: 41119
		OuterPenetrate,
		// Token: 0x0400A0A0 RID: 41120
		InnerPenetrate,
		// Token: 0x0400A0A1 RID: 41121
		OuterPenetrateResist,
		// Token: 0x0400A0A2 RID: 41122
		InnerPenetrateResist,
		// Token: 0x0400A0A3 RID: 41123
		Hit0,
		// Token: 0x0400A0A4 RID: 41124
		Hit1,
		// Token: 0x0400A0A5 RID: 41125
		Hit2,
		// Token: 0x0400A0A6 RID: 41126
		Hit3,
		// Token: 0x0400A0A7 RID: 41127
		Avoid0,
		// Token: 0x0400A0A8 RID: 41128
		Avoid1,
		// Token: 0x0400A0A9 RID: 41129
		Avoid2,
		// Token: 0x0400A0AA RID: 41130
		Avoid3,
		// Token: 0x0400A0AB RID: 41131
		QiDisorder,
		// Token: 0x0400A0AC RID: 41132
		LifeSkill0,
		// Token: 0x0400A0AD RID: 41133
		LifeSkill1,
		// Token: 0x0400A0AE RID: 41134
		LifeSkill2,
		// Token: 0x0400A0AF RID: 41135
		LifeSkill3,
		// Token: 0x0400A0B0 RID: 41136
		LifeSkill4,
		// Token: 0x0400A0B1 RID: 41137
		LifeSkill5,
		// Token: 0x0400A0B2 RID: 41138
		LifeSkill6,
		// Token: 0x0400A0B3 RID: 41139
		LifeSkill7,
		// Token: 0x0400A0B4 RID: 41140
		LifeSkill8,
		// Token: 0x0400A0B5 RID: 41141
		LifeSkill9,
		// Token: 0x0400A0B6 RID: 41142
		LifeSkill10,
		// Token: 0x0400A0B7 RID: 41143
		LifeSkill11,
		// Token: 0x0400A0B8 RID: 41144
		LifeSkill12,
		// Token: 0x0400A0B9 RID: 41145
		LifeSkill13,
		// Token: 0x0400A0BA RID: 41146
		LifeSkill14,
		// Token: 0x0400A0BB RID: 41147
		LifeSkill15,
		// Token: 0x0400A0BC RID: 41148
		LifeSkillGrowth,
		// Token: 0x0400A0BD RID: 41149
		CombatSkill0,
		// Token: 0x0400A0BE RID: 41150
		CombatSkill1,
		// Token: 0x0400A0BF RID: 41151
		CombatSkill2,
		// Token: 0x0400A0C0 RID: 41152
		CombatSkill3,
		// Token: 0x0400A0C1 RID: 41153
		CombatSkill4,
		// Token: 0x0400A0C2 RID: 41154
		CombatSkill5,
		// Token: 0x0400A0C3 RID: 41155
		CombatSkill6,
		// Token: 0x0400A0C4 RID: 41156
		CombatSkill7,
		// Token: 0x0400A0C5 RID: 41157
		CombatSkill8,
		// Token: 0x0400A0C6 RID: 41158
		CombatSkill9,
		// Token: 0x0400A0C7 RID: 41159
		CombatSkill10,
		// Token: 0x0400A0C8 RID: 41160
		CombatSkill11,
		// Token: 0x0400A0C9 RID: 41161
		CombatSkill12,
		// Token: 0x0400A0CA RID: 41162
		CombatSkill13,
		// Token: 0x0400A0CB RID: 41163
		CombatSkillGrowth,
		// Token: 0x0400A0CC RID: 41164
		Personality0,
		// Token: 0x0400A0CD RID: 41165
		Personality1,
		// Token: 0x0400A0CE RID: 41166
		Personality2,
		// Token: 0x0400A0CF RID: 41167
		Personality3,
		// Token: 0x0400A0D0 RID: 41168
		Personality4,
		// Token: 0x0400A0D1 RID: 41169
		Personality5,
		// Token: 0x0400A0D2 RID: 41170
		Personality6,
		// Token: 0x0400A0D3 RID: 41171
		Resource0,
		// Token: 0x0400A0D4 RID: 41172
		Resource1,
		// Token: 0x0400A0D5 RID: 41173
		Resource2,
		// Token: 0x0400A0D6 RID: 41174
		Resource3,
		// Token: 0x0400A0D7 RID: 41175
		Resource4,
		// Token: 0x0400A0D8 RID: 41176
		Resource5,
		// Token: 0x0400A0D9 RID: 41177
		Resource6,
		// Token: 0x0400A0DA RID: 41178
		Resource7,
		// Token: 0x0400A0DB RID: 41179
		InventoryLoad,
		// Token: 0x0400A0DC RID: 41180
		KidnapCount,
		// Token: 0x0400A0DD RID: 41181
		AttackMedal,
		// Token: 0x0400A0DE RID: 41182
		DefenceMedal,
		// Token: 0x0400A0DF RID: 41183
		WisdomMedal,
		// Token: 0x0400A0E0 RID: 41184
		Command0,
		// Token: 0x0400A0E1 RID: 41185
		Command1,
		// Token: 0x0400A0E2 RID: 41186
		Command2,
		// Token: 0x0400A0E3 RID: 41187
		CreatingType,
		// Token: 0x0400A0E4 RID: 41188
		Count
	}

	// Token: 0x02001452 RID: 5202
	private enum ESubPage
	{
		// Token: 0x0400A0E6 RID: 41190
		State,
		// Token: 0x0400A0E7 RID: 41191
		Property,
		// Token: 0x0400A0E8 RID: 41192
		Property2,
		// Token: 0x0400A0E9 RID: 41193
		LifeSkill,
		// Token: 0x0400A0EA RID: 41194
		CombatSkill,
		// Token: 0x0400A0EB RID: 41195
		Personality,
		// Token: 0x0400A0EC RID: 41196
		Item,
		// Token: 0x0400A0ED RID: 41197
		Command
	}

	// Token: 0x02001453 RID: 5203
	private readonly struct CharmData
	{
		// Token: 0x0600CB85 RID: 52101 RVA: 0x0059437D File Offset: 0x0059257D
		public CharmData(sbyte gender, short physiologicalAge, short clothDisplayId, bool faceVisible, byte creatingType)
		{
			this.Gender = gender;
			this.PhysiologicalAge = physiologicalAge;
			this.ClothDisplayId = clothDisplayId;
			this.FaceVisible = faceVisible;
			this.CreatingType = creatingType;
		}

		// Token: 0x0400A0EE RID: 41198
		public readonly sbyte Gender;

		// Token: 0x0400A0EF RID: 41199
		public readonly short PhysiologicalAge;

		// Token: 0x0400A0F0 RID: 41200
		public readonly short ClothDisplayId;

		// Token: 0x0400A0F1 RID: 41201
		public readonly bool FaceVisible;

		// Token: 0x0400A0F2 RID: 41202
		public readonly byte CreatingType;
	}
}
