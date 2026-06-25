using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using FrameWork;
using Game.Components.Avatar;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.SortFilter;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameDataExtensions;
using TMPro;
using UICommon.Character.Elements;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003B6 RID: 950
public class UI_UltimateSelectCharacter : UIBase
{
	// Token: 0x06003922 RID: 14626 RVA: 0x001CDD8C File Offset: 0x001CBF8C
	public override void OnInit(ArgumentBox argsBox)
	{
		this._triggerQuickHideBySystem = true;
		bool flag = this._displayDataCache == null;
		if (flag)
		{
			this._displayDataCache = new Dictionary<int, CharacterDisplayDataForUltimateSelect>();
		}
		string title;
		bool flag2 = argsBox.Get("Title", out title);
		if (flag2)
		{
			base.CGet<TextMeshProUGUI>("Title").text = title;
		}
		bool flag3 = !argsBox.Get<CharacterSortFilterSettings>("SortFilterSettings", out this._sortFilterSettings);
		if (flag3)
		{
			throw new ArgumentNullException("SortFilterSettings can not be null.");
		}
		bool flag4 = !argsBox.Get("SimpleViewStatType", out this._simpleViewStatType);
		if (flag4)
		{
			this._simpleViewStatType = 63;
		}
		Refers sortHolder = base.CGet<CharacterSort>("CharacterSort").CGet<Refers>("SortTypeHolder");
		sortHolder.CGet<CButtonObsolete>("ConsummateLevel").gameObject.SetActive((this._simpleViewStatType & 64) > 0);
		sortHolder.CGet<CButtonObsolete>("Fame").gameObject.SetActive((this._simpleViewStatType & 8) > 0);
		base.CGet<TMP_InputField>("SearchCharacterInput").SetTextWithoutNotify(string.Empty);
		argsBox.Get<Action<int>>("OnConfirmSelect", out this._onConfirmSelect);
		argsBox.Get<Action>("OnCancelSelect", out this._onCancelSelect);
		argsBox.Get<Action>("OnQuickHideBySystem", out this._onQuickHideBySystem);
		argsBox.Get<Action<int>>("OnClickChar", out this._onClickChar);
		argsBox.Get<Predicate<CharacterDisplayDataForUltimateSelect>>("DisableCondition", out this._disableCondition);
		argsBox.Get("DisableTips", out this._disableTips);
		argsBox.Get("ShowFiveElementType", out this._showFiveElementType);
		argsBox.Get("IsSelectInvite", out this._isSelectInvite);
		argsBox.Get("AllowQuickHide", out this._allowQuickHide);
		base.CGet<GameObject>("ConfirmSelectBtn").gameObject.SetActive(this._onConfirmSelect != null);
		base.CGet<GameObject>("CancelSelectBtn").gameObject.SetActive(this._onConfirmSelect != null);
		base.CGet<GameObject>("CloseBtn").gameObject.SetActive(this._onConfirmSelect == null);
		base.CGet<Refers>("CharacterInfo").CGet<CButtonObsolete>("ShowMainCharacterMenu").ClearAndAddListener(delegate
		{
			Action<int> onClickChar = this._onClickChar;
			if (onClickChar != null)
			{
				onClickChar(this._curSelectCharId);
			}
		});
		this._canQuickHide = (this._allowQuickHide || this._isSelectInvite || this._onCancelSelect == null);
		this.NeedDataListenerId = true;
		this.Element.OnListenerIdReady = new Action(this.InitData);
		UIElement element = this.Element;
		element.OnHide = (Action)Delegate.Combine(element.OnHide, new Action(delegate()
		{
			CharacterDomainMethod.Call.ClearCharacterSortFilter();
			TaiwuEventDomainMethod.Call.TriggerListener(" UltimateSelectCharOver", true);
		}));
	}

	// Token: 0x06003923 RID: 14627 RVA: 0x001CE034 File Offset: 0x001CC234
	public override void QuickHide()
	{
		bool canQuickHide = this._canQuickHide;
		if (canQuickHide)
		{
			base.QuickHide();
		}
		bool triggerQuickHideBySystem = this._triggerQuickHideBySystem;
		if (triggerQuickHideBySystem)
		{
			Action onQuickHideBySystem = this._onQuickHideBySystem;
			if (onQuickHideBySystem != null)
			{
				onQuickHideBySystem();
			}
		}
	}

	// Token: 0x06003924 RID: 14628 RVA: 0x001CE070 File Offset: 0x001CC270
	protected override void OnClick(Transform button)
	{
		string btnName = button.name;
		bool flag = "ConfirmSelectBtn" == btnName;
		if (flag)
		{
			Action<int> onConfirmSelect = this._onConfirmSelect;
			if (onConfirmSelect != null)
			{
				onConfirmSelect(this._curSelectCharId);
			}
			this._canQuickHide = true;
			this._triggerQuickHideBySystem = false;
			this.QuickHide();
		}
		else
		{
			bool flag2 = "CancelSelectBtn" == btnName;
			if (flag2)
			{
				Action onCancelSelect = this._onCancelSelect;
				if (onCancelSelect != null)
				{
					onCancelSelect();
				}
				this._canQuickHide = true;
				this._triggerQuickHideBySystem = false;
				this.QuickHide();
			}
			else
			{
				bool flag3 = "CloseBtn" == btnName;
				if (flag3)
				{
					this._triggerQuickHideBySystem = false;
					this.QuickHide();
				}
				else
				{
					bool flag4 = "DetailBtn" == btnName;
					if (flag4)
					{
						base.CGet<GameObject>("SimpleView").SetActive(false);
						this.RefreshCurrentScroll();
						this.FillCharacterDetailInfo();
						InfinityScrollLegacy scroll = base.CGet<InfinityScrollLegacy>("CharScroll");
						scroll.ScrollTo(this._displayResultList.GetCollection().IndexOf(this._curSelectCharId), 0.3f);
					}
					else
					{
						bool flag5 = "SimpleBtn" == btnName;
						if (flag5)
						{
							base.CGet<GameObject>("SimpleView").SetActive(true);
							this.RefreshCurrentScroll();
							InfinityScrollLegacy scroll2 = base.CGet<InfinityScrollLegacy>("SimpleScrollView");
							scroll2.ScrollTo(this._displayResultList.GetCollection().IndexOf(this._curSelectCharId), 0.3f);
						}
					}
				}
			}
		}
	}

	// Token: 0x06003925 RID: 14629 RVA: 0x001CE1EC File Offset: 0x001CC3EC
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 1)
			{
				if (notification.DomainId == 4)
				{
					bool flag = notification.MethodId == 118;
					if (flag)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._mapStateCharacterCountList);
						HorizontalPageSwitchController controller = base.CGet<HorizontalPageSwitchController>("PageSwitchController");
						controller.PageItemRefreshHandler = new Action<int, Refers>(this.OnPageSwitchControllerRenderItem);
						controller.SetItemSelectStateHandler = new Action<Refers, bool>(this.SetPageItemSelectState);
						controller.InitPageCount(this._mapStateCharacterCountList.Count, 0, false);
					}
					else
					{
						bool flag2 = notification.MethodId == 120 || notification.MethodId == 122;
						if (flag2)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._displayResultList);
							bool flag3 = !this.Element.Ready;
							if (flag3)
							{
								this._curSelectCharId = ((this._displayResultList.GetCount() > 0) ? this._displayResultList[0] : -1);
								this.Element.ShowAfterRefresh();
							}
							this.RefreshCurrentScroll();
						}
						else
						{
							bool flag4 = notification.MethodId == 124;
							if (flag4)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._maxValueCharIdList);
								this._maxValueCharIdList.ForEach(delegate(int charId)
								{
									bool flag7 = charId >= 0 && !this._needGetDisplayDataCharIdList.Contains(charId);
									if (flag7)
									{
										this._needGetDisplayDataCharIdList.Add(charId);
									}
								});
							}
							else
							{
								bool flag5 = notification.MethodId == 123;
								if (flag5)
								{
									List<CharacterDisplayDataForUltimateSelect> list = null;
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref list);
									list.ForEach(delegate(CharacterDisplayDataForUltimateSelect e)
									{
										e.InitValuesCache();
										bool flag7 = this._displayDataCache.ContainsKey(e.CharacterId);
										if (flag7)
										{
											this._displayDataCache[e.CharacterId] = e;
										}
										else
										{
											this._displayDataCache.Add(e.CharacterId, e);
										}
									});
									bool flag6 = this._maxValueCharIdList.Count > 0;
									if (flag6)
									{
										this.DecodeMaxValuesOfSortingType();
									}
									this.RefreshCurrentScroll();
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06003926 RID: 14630 RVA: 0x001CE418 File Offset: 0x001CC618
	private void Awake()
	{
		base.CGet<InfinityScrollLegacy>("SimpleScrollView").OnItemRender = new Action<int, Refers>(this.OnSimpleCharacterRender);
		base.CGet<InfinityScrollLegacy>("SimpleScrollView").OnItemHide = new Action<Refers>(this.OnSimpleCharacterHide);
		base.CGet<TMP_InputField>("SearchCharacterInput").onEndEdit.AddListener(new UnityAction<string>(this.OnSearchCharacterInputEndEdit));
		CharacterSortFilterSetting sortFilterSetting = new CharacterSortFilterSetting();
		base.CGet<CharacterSort>("CharacterSort").Init(sortFilterSetting, new Action<CharacterSortFilterSetting>(this.OnSortConfigChange));
		base.CGet<HorizontalPageSwitchController>("PageSwitchController").RegisterOnSelectIndexChangeHandler(new Action<int>(this.OnPageSwitchIndexChange));
		base.CGet<InfinityScrollLegacy>("CharScroll").OnItemRender = new Action<int, Refers>(this.OnDetailViewCharacterRender);
		base.CGet<CToggleGroupObsolete>("SubTogGroup").InitPreOnToggle(-1);
		base.CGet<CToggleGroupObsolete>("SubTogGroup").OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnDetailViewSubToggleChanged);
		this._currentShowingLineStyle = this._columnStyles[base.CGet<CToggleGroupObsolete>("SubTogGroup").GetActive().Key];
		this.InitDetailViewColumnSortButtonListener();
		this._needGetDisplayDataCharIdList = new List<int>();
		this._sortingTypeMaxValueCache = new Dictionary<int, int>();
		base.CGet<GameObject>("SimpleView").SetActive(true);
	}

	// Token: 0x06003927 RID: 14631 RVA: 0x001CE55C File Offset: 0x001CC75C
	private void LateUpdate()
	{
		bool flag = this._needGetDisplayDataCharIdList.Count > 0;
		if (flag)
		{
			CharacterDomainMethod.Call.GetCharacterDisplayDataListForUltimateSelect(this.Element.GameDataListenerId, this._needGetDisplayDataCharIdList);
			this._needGetDisplayDataCharIdList.Clear();
		}
	}

	// Token: 0x06003928 RID: 14632 RVA: 0x001CE5A1 File Offset: 0x001CC7A1
	private void OnEnable()
	{
		base.CGet<InfinityScrollLegacy>("SimpleScrollView").SetDataCount(0);
		base.CGet<InfinityScrollLegacy>("CharScroll").SetDataCount(0);
	}

	// Token: 0x06003929 RID: 14633 RVA: 0x001CE5C8 File Offset: 0x001CC7C8
	private void OnDisable()
	{
		this._displayResultList.Clear();
		this._displayDataCache.Clear();
	}

	// Token: 0x0600392A RID: 14634 RVA: 0x001CE5E4 File Offset: 0x001CC7E4
	private void InitData()
	{
		this._sortFilterSettings.FilterSubId = base.CGet<HorizontalPageSwitchController>("PageSwitchController").CurPageIndex - 1;
		CharacterSort characterSort = base.CGet<CharacterSort>("CharacterSort");
		this._sortFilterSettings.SortOrder.Clear();
		this._sortFilterSettings.SortOrder.AddRange(characterSort.GetCurrentSortConfig());
		CharacterDomainMethod.Call.UpdateSortFilterSettings(this.Element.GameDataListenerId, this._sortFilterSettings);
		CharacterDomainMethod.Call.GetFilteredCharacterCounts(this.Element.GameDataListenerId);
		CharacterDomainMethod.Call.GetMaxSortingTypeCharIds(this.Element.GameDataListenerId, this._needGetMaxValuesOfSortingTypeList, (sbyte)this._sortFilterSettings.FilterSubId);
	}

	// Token: 0x0600392B RID: 14635 RVA: 0x001CE690 File Offset: 0x001CC890
	private void RefreshCurrentScroll()
	{
		bool activeSelf = base.CGet<GameObject>("SimpleView").activeSelf;
		if (activeSelf)
		{
			base.CGet<InfinityScrollLegacy>("SimpleScrollView").SetDataCount(this._displayResultList.GetRealCount());
		}
		else
		{
			base.CGet<InfinityScrollLegacy>("CharScroll").SetDataCount(this._displayResultList.GetRealCount());
		}
	}

	// Token: 0x0600392C RID: 14636 RVA: 0x001CE6F0 File Offset: 0x001CC8F0
	private void DecodeMaxValuesOfSortingType()
	{
		this._sortingTypeMaxValueCache.Clear();
		int i = 0;
		int max = this._needGetMaxValuesOfSortingTypeList.Count;
		while (i < max)
		{
			int charId = this._maxValueCharIdList[i];
			CharacterDisplayDataForUltimateSelect displayData;
			bool flag = this._displayDataCache.TryGetValue(charId, out displayData);
			if (flag)
			{
				this._sortingTypeMaxValueCache.Add(this._needGetMaxValuesOfSortingTypeList[i], displayData[this._needGetMaxValuesOfSortingTypeList[i]].Item1);
			}
			i++;
		}
		this._maxValueCharIdList.Clear();
	}

	// Token: 0x0600392D RID: 14637 RVA: 0x001CE788 File Offset: 0x001CC988
	private void OnSortConfigChange(CharacterSortFilterSetting sortFilterSetting)
	{
		this._sortFilterSettings.SortOrder.Clear();
		this._sortFilterSettings.SortOrder.AddRange(sortFilterSetting.SortOrders);
		CharacterDomainMethod.Call.UpdateSortFilterSettings(this.Element.GameDataListenerId, this._sortFilterSettings);
	}

	// Token: 0x0600392E RID: 14638 RVA: 0x001CE7D8 File Offset: 0x001CC9D8
	private void OnPageSwitchIndexChange(int index)
	{
		this._sortFilterSettings.FilterSubId = index - 1;
		CharacterDomainMethod.Call.UpdateSortFilterSettings(this.Element.GameDataListenerId, this._sortFilterSettings);
		CharacterDomainMethod.Call.GetMaxSortingTypeCharIds(this.Element.GameDataListenerId, this._needGetMaxValuesOfSortingTypeList, (sbyte)this._sortFilterSettings.FilterSubId);
	}

	// Token: 0x0600392F RID: 14639 RVA: 0x001CE830 File Offset: 0x001CCA30
	private void OnPageSwitchControllerRenderItem(int index, Refers refers)
	{
		refers.GetComponent<CButtonObsolete>().ClearAndAddListener(delegate
		{
			this.CGet<HorizontalPageSwitchController>("PageSwitchController").SetSelect(index, true);
		});
		TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("Labels");
		sbyte filterSubType = this._sortFilterSettings.FilterSubType;
		if (!true)
		{
		}
		string text;
		switch (filterSubType)
		{
		case 0:
			text = this.GetStatePageName(index);
			goto IL_9F;
		case 2:
			text = this.GetVillagerRoleName(index);
			goto IL_9F;
		case 3:
			text = this.GetInviteStatePageName(index);
			goto IL_9F;
		}
		text = this.GetDefaultPageName(index);
		IL_9F:
		if (!true)
		{
		}
		string pageName = text;
		label.text = string.Format("{0}({1})", pageName, this._mapStateCharacterCountList[index]);
	}

	// Token: 0x06003930 RID: 14640 RVA: 0x001CE90C File Offset: 0x001CCB0C
	private string GetDefaultPageName(int index)
	{
		if (!true)
		{
		}
		string result;
		if (index != 0)
		{
			result = string.Empty;
		}
		else
		{
			result = LocalStringManager.Get(LanguageKey.LK_Common_All);
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06003931 RID: 14641 RVA: 0x001CE944 File Offset: 0x001CCB44
	private string GetStatePageName(int index)
	{
		if (!true)
		{
		}
		string result;
		if (index != 0)
		{
			if (index != 1)
			{
				result = MapState.Instance[index - 1].Name;
			}
			else
			{
				result = LocalStringManager.Get(LanguageKey.LK_Villager_WorkStatus_InTaiwuGroup);
			}
		}
		else
		{
			result = LocalStringManager.Get(LanguageKey.LK_Common_All);
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06003932 RID: 14642 RVA: 0x001CE99C File Offset: 0x001CCB9C
	private string GetInviteStatePageName(int index)
	{
		if (!true)
		{
		}
		string result;
		if (index != 0)
		{
			result = MapState.Instance[index].Name;
		}
		else
		{
			result = LocalStringManager.Get(LanguageKey.LK_Common_All);
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06003933 RID: 14643 RVA: 0x001CE9DC File Offset: 0x001CCBDC
	private string GetVillagerRoleName(int index)
	{
		if (!true)
		{
		}
		string result;
		if (index != 0)
		{
			if (index != 1)
			{
				result = OrganizationMember.Instance.GetItem(VillagerRole.Instance[index - 2].OrganizationMember).GradeName;
			}
			else
			{
				result = LocalStringManager.Get(LanguageKey.LK_Villager);
			}
		}
		else
		{
			result = LocalStringManager.Get(LanguageKey.LK_Common_All);
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06003934 RID: 14644 RVA: 0x001CEA40 File Offset: 0x001CCC40
	private void SetPageItemSelectState(Refers refers, bool selected)
	{
		refers.CGet<CImage>("Image").SetSprite(selected ? "sp_anniu_18_2" : "sp_anniu_18_0", false, null);
	}

	// Token: 0x06003935 RID: 14645 RVA: 0x001CEA68 File Offset: 0x001CCC68
	private void OnSearchCharacterInputEndEdit(string searchString)
	{
		bool flag = !string.IsNullOrEmpty(searchString);
		if (flag)
		{
			CharacterDomainMethod.Call.FindNameInCurrentSortFilter(this.Element.GameDataListenerId, searchString);
		}
		else
		{
			CharacterDomainMethod.Call.UpdateSortFilterSettings(this.Element.GameDataListenerId, this._sortFilterSettings);
		}
	}

	// Token: 0x06003936 RID: 14646 RVA: 0x001CEAB0 File Offset: 0x001CCCB0
	private void SetSimpleCharacterInfoHide(Refers refers)
	{
		TextMeshProUGUI[] labels = refers.GetComponentsInChildren<TextMeshProUGUI>(true);
		Game.Components.Avatar.Avatar avatar = refers.GetComponentInChildren<Game.Components.Avatar.Avatar>(true);
		avatar.ResetToBlank(false);
		labels.ForEach(delegate(int index, TextMeshProUGUI cell)
		{
			cell.alpha = 0f;
			return false;
		});
		refers.UserBool = true;
	}

	// Token: 0x06003937 RID: 14647 RVA: 0x001CEB04 File Offset: 0x001CCD04
	private void SetSimpleCharacterInfoShow(Refers refers)
	{
		TextMeshProUGUI[] labels = refers.GetComponentsInChildren<TextMeshProUGUI>(true);
		DOVirtual.Float(0f, 1f, 0.3f, delegate(float stepValue)
		{
			labels.ForEach(delegate(int index, TextMeshProUGUI cell)
			{
				cell.alpha = stepValue;
				return false;
			});
		}).SetUpdate(true).SetAutoKill(true);
		refers.UserBool = false;
	}

	// Token: 0x06003938 RID: 14648 RVA: 0x001CEB5C File Offset: 0x001CCD5C
	private void OnSimpleCharacterRender(int index, Refers refers)
	{
		UI_UltimateSelectCharacter.<>c__DisplayClass59_0 CS$<>8__locals1 = new UI_UltimateSelectCharacter.<>c__DisplayClass59_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.refers = refers;
		CS$<>8__locals1.index = index;
		CS$<>8__locals1.charId = this._displayResultList.GetCollection()[CS$<>8__locals1.index];
		bool flag = CS$<>8__locals1.refers.name == string.Format("Character_{0}", CS$<>8__locals1.charId);
		if (!flag)
		{
			bool flag2 = !this._displayDataCache.TryGetValue(CS$<>8__locals1.charId, out CS$<>8__locals1.displayData);
			if (flag2)
			{
				this._needGetDisplayDataCharIdList.Add(CS$<>8__locals1.charId);
				CS$<>8__locals1.refers.CGet<Game.Components.Avatar.Avatar>("Avatar").ResetToBlank(false);
			}
			else
			{
				CS$<>8__locals1.refers.CGet<Game.Components.Avatar.Avatar>("Avatar").Refresh(CS$<>8__locals1.displayData.AvatarRelatedData);
				CS$<>8__locals1.refers.CGet<TextMeshProUGUI>("CharacterName").text = CS$<>8__locals1.displayData[0].Item2;
				CS$<>8__locals1.refers.CGet<TextMeshProUGUI>("CharacterAge").text = LocalStringManager.GetFormat(LanguageKey.LK_Age, CS$<>8__locals1.displayData[2].Item1);
				CS$<>8__locals1.refers.CGet<CharacterHealthBar>("CharacterHealth").StateLabel.text = CS$<>8__locals1.displayData[3].Item2;
				CS$<>8__locals1.<OnSimpleCharacterRender>g__RenderSimpleViewStat|2("CharacterConsummateLevel", 34, 64);
				CS$<>8__locals1.<OnSimpleCharacterRender>g__RenderSimpleViewStat|2("CharacterCharm", 201, 1);
				CS$<>8__locals1.<OnSimpleCharacterRender>g__RenderSimpleViewStat|2("CharacterHappiness", 6, 2);
				CS$<>8__locals1.<OnSimpleCharacterRender>g__RenderSimpleViewStat|2("CharacterBehavior", 5, 4);
				CS$<>8__locals1.<OnSimpleCharacterRender>g__RenderSimpleViewStat|2("CharacterFame", 8, 8);
				CS$<>8__locals1.<OnSimpleCharacterRender>g__RenderSimpleViewStat|2("CharacterOrganization", 24, 16);
				CS$<>8__locals1.<OnSimpleCharacterRender>g__RenderSimpleViewStat|2("CharacterIdentity", 1, 32);
				bool isNeedTreasuryItemVillager = this._sortFilterSettings.FilterType == 4;
				string posText = isNeedTreasuryItemVillager ? LocalStringManager.GetFormat(LanguageKey.LK_VillagerNeed_Tip_Time, CS$<>8__locals1.displayData[38].Item2.SetColor("pinkyellow")) : CS$<>8__locals1.displayData[25].Item2;
				TextMeshProUGUI positionValueText = CS$<>8__locals1.refers.CGet<TextMeshProUGUI>("PositionValue");
				positionValueText.text = posText;
				bool flag3 = isNeedTreasuryItemVillager;
				if (flag3)
				{
					positionValueText.gameObject.GetOrAddComponent<TMPTextSpriteHelper>().Parse();
				}
				CS$<>8__locals1.refers.name = string.Format("Character_{0}", CS$<>8__locals1.charId);
				CToggleObsolete tog = CS$<>8__locals1.refers.GetComponent<CToggleObsolete>();
				this.InitDisabled(tog, CS$<>8__locals1.displayData);
				tog.onValueChanged.RemoveAllListeners();
				tog.isOn = (CS$<>8__locals1.charId == this._curSelectCharId);
				tog.interactable = !tog.isOn;
				tog.onValueChanged.AddListener(delegate(bool isOn)
				{
					if (isOn)
					{
						int prevCharId = CS$<>8__locals1.<>4__this._curSelectCharId;
						Transform prevRefersTrans = CS$<>8__locals1.refers.transform.parent.Find(string.Format("Character_{0}", prevCharId));
						CS$<>8__locals1.<>4__this._curSelectCharId = CS$<>8__locals1.charId;
						InfinityScrollLegacy scroll = CS$<>8__locals1.<>4__this.CGet<InfinityScrollLegacy>("SimpleScrollView");
						scroll.RefreshCell(CS$<>8__locals1.index);
						bool flag5 = null != prevRefersTrans;
						if (flag5)
						{
							scroll.RefreshCell(prevRefersTrans.GetComponent<Refers>().UserInt);
						}
					}
				});
				CButtonObsolete button = CS$<>8__locals1.refers.CGet<CButtonObsolete>("ShowMainCharacterMenu");
				button.ClearAndAddListener(delegate
				{
					Action<int> onClickChar = CS$<>8__locals1.<>4__this._onClickChar;
					if (onClickChar != null)
					{
						onClickChar(CS$<>8__locals1.charId);
					}
				});
				Refers fiveElementTypeRefers = CS$<>8__locals1.refers.CGet<Refers>("FiveElementType");
				fiveElementTypeRefers.gameObject.SetActive(this._showFiveElementType);
				bool showFiveElementType = this._showFiveElementType;
				if (showFiveElementType)
				{
					sbyte neiliType = CS$<>8__locals1.displayData.NeiliProportionOfFiveElements.GetNeiliType(CS$<>8__locals1.displayData.BirthMonth);
					NeiliTypeItem neiliTypeCfg = NeiliType.Instance[neiliType];
					sbyte fiveElementType = (sbyte)neiliTypeCfg.FiveElements;
					string fiveElementName = CommonUtils.GetFiveElementsNameByType(fiveElementType);
					string fiveElementIcon = CommonUtils.GetFiveElementsIconByType(fiveElementType);
					fiveElementTypeRefers.CGet<TextMeshProUGUI>("Label").SetText(fiveElementName, true);
					fiveElementTypeRefers.CGet<CImage>("Icon").SetSprite(fiveElementIcon, false, null);
					TooltipInvoker mouseTipDisplayer = fiveElementTypeRefers.CGet<TooltipInvoker>("Tip");
					mouseTipDisplayer.Type = TipType.Simple;
					string[] presetParam = mouseTipDisplayer.PresetParam;
					bool flag4 = presetParam == null || presetParam.Length != 2;
					if (flag4)
					{
						mouseTipDisplayer.PresetParam = new string[2];
					}
					mouseTipDisplayer.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_NeiliProportionOfFiveElements);
					mouseTipDisplayer.PresetParam[1] = UI_LifeLink.GetFiveElementsTips(CS$<>8__locals1.displayData.NeiliProportionOfFiveElements, null);
				}
			}
		}
	}

	// Token: 0x06003939 RID: 14649 RVA: 0x001CEF80 File Offset: 0x001CD180
	private void InitDisabled(CToggleObsolete toggle, CharacterDisplayDataForUltimateSelect displayData)
	{
		bool isDisabled = this._disableCondition != null && this._disableCondition(displayData);
		DisableStyleRoot disableStyleRoot = toggle.GetComponent<DisableStyleRoot>();
		disableStyleRoot.SetStyleEffect(isDisabled, false);
		toggle.interactable = !isDisabled;
		TooltipInvoker mouseTipDisplayer = toggle.GetComponent<TooltipInvoker>();
		mouseTipDisplayer.Type = TipType.SingleDesc;
		bool flag = !mouseTipDisplayer.PresetParam.CheckIndex(0);
		if (flag)
		{
			mouseTipDisplayer.PresetParam = new string[1];
		}
		mouseTipDisplayer.PresetParam[0] = this._disableTips;
		mouseTipDisplayer.enabled = isDisabled;
		bool isSelectInvite = this._isSelectInvite;
		if (isSelectInvite)
		{
			bool invited = displayData.Invited;
			LanguageKey tipsKey;
			if (invited)
			{
				tipsKey = LanguageKey.LK_Item_Invite_InvitedThisMonth;
			}
			else
			{
				bool trapped = displayData.Trapped;
				if (trapped)
				{
					tipsKey = LanguageKey.LK_Item_Invite_Traped;
				}
				else
				{
					bool prioritizeActionOverInvite = displayData.PrioritizeActionOverInvite;
					if (!prioritizeActionOverInvite)
					{
						return;
					}
					tipsKey = LanguageKey.LK_Item_Invite_Busy;
				}
			}
			mouseTipDisplayer.PresetParam[0] = LocalStringManager.Get(tipsKey);
		}
	}

	// Token: 0x0600393A RID: 14650 RVA: 0x001CF062 File Offset: 0x001CD262
	private void OnSimpleCharacterHide(Refers refers)
	{
		refers.name = string.Format("wait reuse item {0}", refers.UserInt);
	}

	// Token: 0x0600393B RID: 14651 RVA: 0x001CF081 File Offset: 0x001CD281
	private void OnDetailViewSubToggleChanged(CToggleObsolete newTog, CToggleObsolete preTog)
	{
		this._currentShowingLineStyle = this._columnStyles[newTog.Key];
		base.CGet<InfinityScrollLegacy>("CharScroll").ReRender();
		this.UpdateCurrentColumnStyleView();
	}

	// Token: 0x0600393C RID: 14652 RVA: 0x001CF0B0 File Offset: 0x001CD2B0
	private void SetLineStyle(Refers refers)
	{
		bool flag = refers.UserString == this._currentShowingLineStyle;
		if (!flag)
		{
			int titleStyleIndex = Array.IndexOf<string>(this._columnStyles, this._currentShowingLineStyle);
			bool flag2 = !this.DetailViewTitleStyles.CheckIndex(titleStyleIndex);
			if (flag2)
			{
				throw new Exception("Invalid title style index");
			}
			RectTransform titleStyleTrans = this.DetailViewTitleStyles[titleStyleIndex];
			UI_UltimateSelectCharacter.DetailColumnDisplayConfig[] displayConfigs;
			this._detailViewDisplayConfig.TryGetValue(this._currentShowingLineStyle, out displayConfigs);
			List<Transform> childrenTrans = new List<Transform>();
			childrenTrans.AddRange(refers.transform.GetComponentsInTopChildren(true));
			int i = 0;
			int max = displayConfigs.Length;
			while (i < max)
			{
				string childName = string.Format("InfoCell_{0}", i);
				UI_UltimateSelectCharacter.DetailColumnDisplayConfig config = displayConfigs[i];
				bool flag3 = !config.SpecifyItem.IsNullOrEmpty();
				if (flag3)
				{
					childName = config.SpecifyItem;
				}
				Transform child = refers.transform.Find(childName);
				bool flag4 = null != child;
				if (flag4)
				{
					childrenTrans.Remove(child);
				}
				Vector3 position = child.transform.position;
				Transform titleCell = titleStyleTrans.GetChild(i);
				position = position.SetX(titleCell.position.x);
				child.transform.position = position;
				(child as RectTransform).SetWidth((titleCell as RectTransform).rect.width);
				child.gameObject.SetActive(true);
				i++;
			}
			foreach (Transform child2 in childrenTrans)
			{
				bool flag5 = child2.name == "NameBack";
				if (!flag5)
				{
					child2.gameObject.SetActive(false);
				}
			}
			refers.UserString = this._currentShowingLineStyle;
		}
	}

	// Token: 0x0600393D RID: 14653 RVA: 0x001CF2A4 File Offset: 0x001CD4A4
	private bool IsMaxValue(int value, int sortingTypeId)
	{
		int maxValue;
		bool flag = this._sortingTypeMaxValueCache.TryGetValue(sortingTypeId, out maxValue);
		return flag && maxValue != 0 && maxValue == value;
	}

	// Token: 0x0600393E RID: 14654 RVA: 0x001CF2D8 File Offset: 0x001CD4D8
	private void OnDetailViewCharacterRender(int index, Refers refers)
	{
		this.SetLineStyle(refers);
		int charId = this._displayResultList.GetCollection()[index];
		bool flag = refers.name == string.Format("Character_{0}", charId);
		if (!flag)
		{
			CharacterDisplayDataForUltimateSelect displayData;
			bool flag2 = !this._displayDataCache.TryGetValue(charId, out displayData);
			if (flag2)
			{
				this._needGetDisplayDataCharIdList.Add(charId);
			}
			else
			{
				refers.CGet<TextMeshProUGUI>("Name").text = displayData[0].Item2;
				UI_UltimateSelectCharacter.DetailColumnDisplayConfig[] displayConfigs;
				this._detailViewDisplayConfig.TryGetValue(this._currentShowingLineStyle, out displayConfigs);
				int i = 0;
				int max = displayConfigs.Length;
				while (i < max)
				{
					string childName = string.Format("InfoCell_{0}", i);
					UI_UltimateSelectCharacter.DetailColumnDisplayConfig config = displayConfigs[i];
					bool flag3 = !config.SpecifyItem.IsNullOrEmpty();
					if (flag3)
					{
						childName = config.SpecifyItem;
					}
					Transform child = refers.transform.Find(childName);
					TextMeshProUGUI label = child.Find("Value").GetComponent<TextMeshProUGUI>();
					label.text = config.ContentPrefix + displayData[config.ColumnType].Item2;
					label.color = ((charId == this._curSelectCharId) ? Colors.Instance["pinkyellow"] : Colors.Instance["grey"]);
					string spriteName = "";
					bool flag4 = this._curSelectCharId == charId;
					if (flag4)
					{
						spriteName = (config.Background.IsNullOrEmpty() ? "charactermenu3_04_gn_shuju_0" : config.Background);
					}
					child.GetComponent<CImage>().SetSprite(spriteName, false, null);
					int value = displayData[config.ColumnType].Item1;
					child.Find("MaxMark").gameObject.SetActive(this.IsMaxValue(value, config.ColumnType));
					Transform iconTransform = child.Find("Icon");
					bool flag5 = null != iconTransform;
					if (flag5)
					{
						iconTransform.gameObject.SetActive(value != 0);
						CImage iconImg = iconTransform.GetComponent<CImage>();
						bool flag6 = config.ColumnType == 11;
						if (flag6)
						{
							iconImg.SetSprite(((value > 0) ? UI_UltimateSelectCharacter.PositiveFeatureIcon : UI_UltimateSelectCharacter.NegativeFeatureIcon)[0], false, null);
						}
						bool flag7 = config.ColumnType == 12;
						if (flag7)
						{
							iconImg.SetSprite(((value > 0) ? UI_UltimateSelectCharacter.PositiveFeatureIcon : UI_UltimateSelectCharacter.NegativeFeatureIcon)[1], false, null);
						}
						bool flag8 = config.ColumnType == 13;
						if (flag8)
						{
							iconImg.SetSprite(((value > 0) ? UI_UltimateSelectCharacter.PositiveFeatureIcon : UI_UltimateSelectCharacter.NegativeFeatureIcon)[2], false, null);
						}
						bool flag9 = value == 0;
						if (flag9)
						{
							label.text = displayData[config.ColumnType].Item2;
						}
					}
					i++;
				}
				CToggleObsolete toggle = refers.GetComponent<CToggleObsolete>();
				this.InitDisabled(toggle, displayData);
				toggle.onValueChanged.RemoveAllListeners();
				toggle.isOn = (this._curSelectCharId == charId);
				toggle.interactable = !toggle.isOn;
				toggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					if (isOn)
					{
						InfinityScrollLegacy scroll = this.CGet<InfinityScrollLegacy>("CharScroll");
						Transform prevCellTrans = refers.transform.parent.Find(string.Format("Character_{0}", this._curSelectCharId));
						this._curSelectCharId = charId;
						scroll.RefreshCell(index);
						bool flag10 = null != prevCellTrans;
						if (flag10)
						{
							scroll.RefreshCell(prevCellTrans.GetComponent<Refers>().UserInt);
						}
						this.FillCharacterDetailInfo();
					}
				});
				TooltipInvoker mouseTipDisplayer = toggle.GetComponent<TooltipInvoker>();
				mouseTipDisplayer.Type = TipType.SingleDesc;
				TooltipInvoker tooltipInvoker = mouseTipDisplayer;
				if (tooltipInvoker.PresetParam == null)
				{
					tooltipInvoker.PresetParam = new string[1];
				}
				mouseTipDisplayer.PresetParam[0] = this._disableTips;
				refers.name = string.Format("Character_{0}", charId);
			}
		}
	}

	// Token: 0x0600393F RID: 14655 RVA: 0x001CF6D0 File Offset: 0x001CD8D0
	private void FillCharacterDetailInfo()
	{
		CharacterDisplayDataForUltimateSelect displayData;
		bool flag = !this._displayDataCache.TryGetValue(this._curSelectCharId, out displayData);
		if (flag)
		{
			throw new Exception("Invalid Operation:try to show character DetailInfo without cache data");
		}
		Refers infoRefers = base.CGet<Refers>("CharacterInfo");
		infoRefers.CGet<Game.Components.Avatar.Avatar>("Avatar").Refresh(displayData.AvatarRelatedData);
		infoRefers.CGet<TextMeshProUGUI>("Name").text = displayData[0].Item2;
		infoRefers.CGet<TextMeshProUGUI>("Health").text = displayData[3].Item2;
		infoRefers.CGet<TextMeshProUGUI>("Birth").text = displayData[0].Item2;
		CImage fiveElementsIcon = infoRefers.CGet<CImage>("InnateFiveElementsType");
		TooltipInvoker fiveElementsTips = fiveElementsIcon.GetComponent<TooltipInvoker>();
		int birthMonth = displayData.BirthDate % 12;
		bool flag2 = birthMonth < 0;
		if (flag2)
		{
			birthMonth += 12;
		}
		MonthItem monthConfig = Month.Instance[birthMonth];
		infoRefers.CGet<TextMeshProUGUI>("Age").text = (Character.Instance[displayData.NameRelatedData.CharTemplateId].HideAge ? "-" : LocalStringManager.GetFormat(LanguageKey.LK_Age, displayData.PhysiologicalAge));
		infoRefers.CGet<TextMeshProUGUI>("Birth").text = LocalStringManager.GetFormat(LanguageKey.LK_Birth_Tips, monthConfig.Name);
		fiveElementsIcon.SetSprite(string.Format("mousetip_shuxing_{0}", monthConfig.FiveElementsType), false, null);
		bool flag3 = fiveElementsTips.RuntimeParam == null;
		if (flag3)
		{
			fiveElementsTips.RuntimeParam = new ArgumentBox();
		}
		fiveElementsTips.RuntimeParam.Set("BirthMonth", birthMonth);
		infoRefers.CGet<Refers>("CharacterCharm").CGet<TextMeshProUGUI>("InfoValue").text = displayData[201].Item2;
		infoRefers.CGet<Refers>("CharacterGender").CGet<TextMeshProUGUI>("InfoValue").text = displayData[4].Item2;
		CommonUtils.EDisplayGender displayGender = CommonUtils.GetDisplayGender(displayData.Gender, displayData.NameRelatedData.CharTemplateId);
		infoRefers.CGet<Refers>("CharacterGender").CGet<CImage>("Icon").SetSprite(CommonUtils.GetGenderIcon(displayGender), false, null);
		infoRefers.CGet<Refers>("CharacterIdentity").CGet<TextMeshProUGUI>("InfoValue").text = displayData[1].Item2;
		infoRefers.CGet<Refers>("CharacterIdentity").CGet<CImage>("Icon").SetSprite(CommonUtils.GetIdentityIcon(displayData.OrganizationInfo.Grade), false, null);
		infoRefers.CGet<Refers>("CharacterBehavior").CGet<TextMeshProUGUI>("InfoValue").text = displayData[5].Item2;
		BehaviorTypeItem behaviorConfig = Config.BehaviorType.Instance.GetItem((short)displayData.BehaviorType);
		infoRefers.CGet<Refers>("CharacterBehavior").CGet<CImage>("Icon").SetSprite(behaviorConfig.Icon, false, null);
		infoRefers.CGet<Refers>("CharacterFame").CGet<TextMeshProUGUI>("InfoValue").text = displayData[8].Item2;
		infoRefers.CGet<Refers>("CharacterFame").CGet<CImage>("Icon").SetSprite(CommonUtils.GetFameIconLegacy(displayData.FameType), false, null);
		infoRefers.CGet<Refers>("CharacterHappiness").CGet<TextMeshProUGUI>("InfoValue").text = displayData[6].Item2;
		infoRefers.CGet<Refers>("CharacterHappiness").CGet<CImage>("Icon").SetSprite(CommonUtils.GetHappinessIconLegacy(displayData.HappinessType), false, null);
		infoRefers.CGet<Refers>("CharacterOrganization").CGet<TextMeshProUGUI>("InfoValue").text = displayData[24].Item2;
		infoRefers.CGet<Refers>("CharacterFavorability").CGet<TextMeshProUGUI>("InfoValue").text = displayData[7].Item2;
		infoRefers.CGet<Refers>("CharacterFavorability").CGet<CImage>("Icon").SetSprite(CommonUtils.GetFavorIconLegacy(displayData.FavorabilityToTaiwu), false, null);
		string positionString = SingletonObject.getInstance<WorldMapModel>().GetFullBlockName(displayData.LocationNameData, true, true, true, true);
		infoRefers.CGet<TextMeshProUGUI>("PositionValue").text = positionString;
	}

	// Token: 0x06003940 RID: 14656 RVA: 0x001CFAF4 File Offset: 0x001CDCF4
	private void InitDetailViewColumnSortButtonListener()
	{
		int i = 0;
		int max = this._columnStyles.Length;
		while (i < max)
		{
			string columnStyle = this._columnStyles[i];
			UI_UltimateSelectCharacter.DetailColumnDisplayConfig[] configArray;
			bool flag = this._detailViewDisplayConfig.TryGetValue(columnStyle, out configArray);
			if (flag)
			{
				RectTransform columnStyleRoot = this.DetailViewTitleStyles[i];
				foreach (UI_UltimateSelectCharacter.DetailColumnDisplayConfig config in configArray)
				{
					bool flag2 = !string.IsNullOrEmpty(config.ButtonName);
					if (flag2)
					{
						Transform child = columnStyleRoot.Find(config.ButtonName);
						bool flag3 = null != child;
						if (flag3)
						{
							int type = config.ColumnType;
							child.GetComponent<CButtonObsolete>().ClearAndAddListener(delegate
							{
								this.OnDetailViewColumnSortButtonClick(type);
							});
							child.Find("IndexBg").gameObject.SetActive(false);
							child.Find("Arrow").gameObject.SetActive(false);
						}
					}
				}
			}
			i++;
		}
		this.UpdateCurrentColumnStyleView();
	}

	// Token: 0x06003941 RID: 14657 RVA: 0x001CFC20 File Offset: 0x001CDE20
	private void UpdateCurrentColumnStyleView()
	{
		int columnStyleIndex = Array.IndexOf<string>(this._columnStyles, this._currentShowingLineStyle);
		UI_UltimateSelectCharacter.DetailColumnDisplayConfig[] configArray;
		bool flag = this.DetailViewTitleStyles.CheckIndex(columnStyleIndex) && this._detailViewDisplayConfig.TryGetValue(this._currentShowingLineStyle, out configArray);
		if (flag)
		{
			RectTransform columnStyleRoot = this.DetailViewTitleStyles[columnStyleIndex];
			int type = -1;
			bool isDescending = false;
			bool flag2 = this._sortFilterSettings.SortOrder.Count > 0;
			if (flag2)
			{
				ValueTuple<int, bool> valueTuple = this._sortFilterSettings.SortOrder[0];
				type = valueTuple.Item1;
				isDescending = valueTuple.Item2;
			}
			foreach (UI_UltimateSelectCharacter.DetailColumnDisplayConfig config in configArray)
			{
				bool flag3 = !string.IsNullOrEmpty(config.ButtonName);
				if (flag3)
				{
					Transform child = columnStyleRoot.Find(config.ButtonName);
					bool flag4 = null != child;
					if (flag4)
					{
						bool arrowVisible = config.ColumnType == type;
						Transform arrowTransform = child.Find("Arrow");
						arrowTransform.gameObject.SetActive(arrowVisible);
						bool flag5 = arrowVisible;
						if (flag5)
						{
							RectTransform arrowRectTrans = arrowTransform as RectTransform;
							arrowRectTrans.localScale = arrowRectTrans.localScale.SetY((float)(isDescending ? 1 : -1));
							arrowRectTrans.localPosition = (isDescending ? Vector3.down : Vector3.up) * 25f;
						}
					}
				}
			}
		}
	}

	// Token: 0x06003942 RID: 14658 RVA: 0x001CFD90 File Offset: 0x001CDF90
	private void OnDetailViewColumnSortButtonClick(int sortType)
	{
		bool flag = this._sortFilterSettings.SortOrder.Count <= 0;
		if (flag)
		{
			this._sortFilterSettings.SortOrder.Add(new ValueTuple<int, bool>(sortType, true));
		}
		else
		{
			bool flag2 = this._sortFilterSettings.SortOrder[0].Item1 != sortType;
			if (flag2)
			{
				this._sortFilterSettings.SortOrder.Clear();
				this._sortFilterSettings.SortOrder.Add(new ValueTuple<int, bool>(sortType, true));
			}
			else
			{
				bool item = this._sortFilterSettings.SortOrder[0].Item2;
				if (item)
				{
					this._sortFilterSettings.SortOrder[0] = new ValueTuple<int, bool>(sortType, false);
				}
				else
				{
					this._sortFilterSettings.SortOrder.Clear();
				}
			}
		}
		CharacterDomainMethod.Call.UpdateSortFilterSettings(this.Element.GameDataListenerId, this._sortFilterSettings);
		this.UpdateCurrentColumnStyleView();
	}

	// Token: 0x04002957 RID: 10583
	public RectTransform[] DetailViewTitleStyles;

	// Token: 0x04002958 RID: 10584
	private const string DetailViewDefaultColumnBackground = "";

	// Token: 0x04002959 RID: 10585
	private const string DetailViewDefaultColumnBackgroundSelected = "charactermenu3_04_gn_shuju_0";

	// Token: 0x0400295A RID: 10586
	private const string LineStyle1 = "Information";

	// Token: 0x0400295B RID: 10587
	private const string LineStyle2 = "Attribute";

	// Token: 0x0400295C RID: 10588
	private const string LineStyle3 = "LifeSkill";

	// Token: 0x0400295D RID: 10589
	private const string LineStyle4 = "CombatSkill";

	// Token: 0x0400295E RID: 10590
	private const string LineStyle5 = "Personality";

	// Token: 0x0400295F RID: 10591
	private const string LineStyle6 = "Items";

	// Token: 0x04002960 RID: 10592
	private static readonly string[] PositiveFeatureIcon = new string[]
	{
		"sp_icon_renwutexing_10",
		"sp_icon_renwutexing_9",
		"sp_icon_renwutexing_11"
	};

	// Token: 0x04002961 RID: 10593
	private static readonly string[] NegativeFeatureIcon = new string[]
	{
		"sp_icon_renwutexing_4",
		"sp_icon_renwutexing_3",
		"sp_icon_renwutexing_5"
	};

	// Token: 0x04002962 RID: 10594
	private string _currentShowingLineStyle;

	// Token: 0x04002963 RID: 10595
	private readonly string[] _columnStyles = new string[]
	{
		"Information",
		"Attribute",
		"LifeSkill",
		"CombatSkill",
		"Personality",
		"Items"
	};

	// Token: 0x04002964 RID: 10596
	private readonly Dictionary<string, UI_UltimateSelectCharacter.DetailColumnDisplayConfig[]> _detailViewDisplayConfig = new Dictionary<string, UI_UltimateSelectCharacter.DetailColumnDisplayConfig[]>
	{
		{
			"Information",
			new UI_UltimateSelectCharacter.DetailColumnDisplayConfig[]
			{
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 2,
					ButtonName = "Age"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 3,
					ButtonName = "Health"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 19,
					ButtonName = "Injury"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 201,
					ButtonName = "Charm"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 5,
					ButtonName = "Behavior"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 6,
					ButtonName = "Happiness"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 7,
					ButtonName = "Favorability"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 21,
					ButtonName = "Samsara"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 8,
					ButtonName = "Fame"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 11,
					SpecifyItem = "Attack",
					ContentPrefix = "X",
					ButtonName = "Attack"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 12,
					SpecifyItem = "Defence",
					ContentPrefix = "X",
					ButtonName = "Defence"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 13,
					SpecifyItem = "Wisdom",
					ContentPrefix = "X",
					ButtonName = "Wisdom"
				}
			}
		},
		{
			"Attribute",
			new UI_UltimateSelectCharacter.DetailColumnDisplayConfig[]
			{
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.Strength),
					ButtonName = "Strength"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.Dexterity),
					ButtonName = "Dexterity"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.Concentration),
					ButtonName = "Concentration"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.Vitality),
					ButtonName = "Vitality"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.Energy),
					ButtonName = "Energy"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.Intelligence),
					ButtonName = "Intelligence"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PenetrateOfOuter),
					ButtonName = "PenetrateOuter"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PenetrateOfInner),
					ButtonName = "PenetrateInner"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PenetrateResistOfOuter),
					ButtonName = "PenetrateResistOuter"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PenetrateResistOfInner),
					ButtonName = "PenetrateResistInner"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.HitRateStrength),
					ButtonName = "Hit0"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.HitRateTechnique),
					ButtonName = "Hit1"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.HitRateSpeed),
					ButtonName = "Hit2"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.HitRateMind),
					ButtonName = "Hit3"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.AvoidRateStrength),
					ButtonName = "Avoid0"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.AvoidRateTechnique),
					ButtonName = "Avoid1"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.AvoidRateSpeed),
					ButtonName = "Avoid2"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.AvoidRateMind),
					ButtonName = "Avoid3"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 20,
					ButtonName = "QiDisorder"
				}
			}
		},
		{
			"LifeSkill",
			new UI_UltimateSelectCharacter.DetailColumnDisplayConfig[]
			{
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationMusic),
					ButtonName = "LifeSkill0"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationChess),
					ButtonName = "LifeSkill1"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationPoem),
					ButtonName = "LifeSkill2"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationPainting),
					ButtonName = "LifeSkill3"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationMath),
					ButtonName = "LifeSkill4"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationAppraisal),
					ButtonName = "LifeSkill5"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationForging),
					ButtonName = "LifeSkill6"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationWoodworking),
					ButtonName = "LifeSkill7"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationMedicine),
					ButtonName = "LifeSkill8"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationToxicology),
					ButtonName = "LifeSkill9"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationWeaving),
					ButtonName = "LifeSkill10"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationJade),
					ButtonName = "LifeSkill11"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationTaoism),
					ButtonName = "LifeSkill12"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationBuddhism),
					ButtonName = "LifeSkill13"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationCooking),
					ButtonName = "LifeSkill14"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationEclectic),
					ButtonName = "LifeSkill15"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 18,
					ButtonName = "LifeSkillGrowth"
				}
			}
		},
		{
			"CombatSkill",
			new UI_UltimateSelectCharacter.DetailColumnDisplayConfig[]
			{
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationNeigong),
					ButtonName = "CombatSkill0"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationPosing),
					ButtonName = "CombatSkill1"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationStunt),
					ButtonName = "CombatSkill2"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationFistAndPalm),
					ButtonName = "CombatSkill3"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationFinger),
					ButtonName = "CombatSkill4"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationLeg),
					ButtonName = "CombatSkill5"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationThrow),
					ButtonName = "CombatSkill6"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationSword),
					ButtonName = "CombatSkill7"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationBlade),
					ButtonName = "CombatSkill8"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationPolearm),
					ButtonName = "CombatSkill9"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationSpecial),
					ButtonName = "CombatSkill10"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationWhip),
					ButtonName = "CombatSkill11"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationControllableShot),
					ButtonName = "CombatSkill12"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationCombatMusic),
					ButtonName = "CombatSkill13"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 17,
					ButtonName = "CombatSkillGrowth"
				}
			}
		},
		{
			"Personality",
			new UI_UltimateSelectCharacter.DetailColumnDisplayConfig[]
			{
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PersonalityCalm),
					Background = "charactermenu3_08_gn_fuxing_0",
					ButtonName = "Calm"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PersonalityClever),
					Background = "charactermenu3_08_gn_fuxing_1",
					ButtonName = "Clever"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PersonalityEnthusiastic),
					Background = "charactermenu3_08_gn_fuxing_2",
					ButtonName = "Enthusiastic"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PersonalityBrave),
					Background = "charactermenu3_08_gn_fuxing_3",
					ButtonName = "Brave"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PersonalityFirm),
					Background = "charactermenu3_08_gn_fuxing_4",
					ButtonName = "Firm"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PersonalityLucky),
					ButtonName = "Lucky"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PersonalityPerceptive),
					ButtonName = "Perceptive"
				}
			}
		},
		{
			"Items",
			new UI_UltimateSelectCharacter.DetailColumnDisplayConfig[]
			{
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 26,
					ButtonName = "Resource0"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 27,
					ButtonName = "Resource1"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 28,
					ButtonName = "Resource2"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 29,
					ButtonName = "Resource3"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 30,
					ButtonName = "Resource4"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 31,
					ButtonName = "Resource5"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 32,
					ButtonName = "Resource6"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 33,
					ButtonName = "Resource7"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 22,
					ButtonName = "InventoryLoad"
				},
				new UI_UltimateSelectCharacter.DetailColumnDisplayConfig
				{
					ColumnType = 23,
					ButtonName = "Kidnap"
				}
			}
		}
	};

	// Token: 0x04002965 RID: 10597
	private List<int> _needGetMaxValuesOfSortingTypeList = new List<int>
	{
		2,
		3,
		19,
		21,
		11,
		12,
		13,
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.Strength),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.Dexterity),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.Concentration),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.Vitality),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.Energy),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.Intelligence),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PenetrateOfOuter),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PenetrateOfInner),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PenetrateResistOfOuter),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PenetrateResistOfInner),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.HitRateStrength),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.HitRateTechnique),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.HitRateSpeed),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.HitRateMind),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.AvoidRateStrength),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.AvoidRateTechnique),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.AvoidRateSpeed),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.AvoidRateMind),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationMusic),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationChess),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationPoem),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationPainting),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationMath),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationAppraisal),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationForging),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationWoodworking),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationMedicine),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationToxicology),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationWeaving),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationJade),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationTaoism),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationBuddhism),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationCooking),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationEclectic),
		18,
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationNeigong),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationPosing),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationStunt),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationFistAndPalm),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationFinger),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationLeg),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationThrow),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationSword),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationBlade),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationPolearm),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationSpecial),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationWhip),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationControllableShot),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.QualificationCombatMusic),
		17,
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PersonalityCalm),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PersonalityClever),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PersonalityEnthusiastic),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PersonalityBrave),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PersonalityFirm),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PersonalityLucky),
		CharacterSortingType.GetCharacterSortingType(ECharacterPropertyReferencedType.PersonalityPerceptive),
		26,
		27,
		28,
		29,
		30,
		31,
		32,
		33,
		22,
		23
	};

	// Token: 0x04002966 RID: 10598
	private Dictionary<int, CharacterDisplayDataForUltimateSelect> _displayDataCache;

	// Token: 0x04002967 RID: 10599
	private List<int> _mapStateCharacterCountList;

	// Token: 0x04002968 RID: 10600
	private List<int> _needGetDisplayDataCharIdList;

	// Token: 0x04002969 RID: 10601
	private int _curSelectCharId;

	// Token: 0x0400296A RID: 10602
	private Action<int> _onConfirmSelect;

	// Token: 0x0400296B RID: 10603
	private Action _onCancelSelect;

	// Token: 0x0400296C RID: 10604
	private Action _onQuickHideBySystem;

	// Token: 0x0400296D RID: 10605
	private Action<int> _onClickChar;

	// Token: 0x0400296E RID: 10606
	private Predicate<CharacterDisplayDataForUltimateSelect> _disableCondition;

	// Token: 0x0400296F RID: 10607
	private CharacterList _displayResultList;

	// Token: 0x04002970 RID: 10608
	private CharacterSortFilterSettings _sortFilterSettings;

	// Token: 0x04002971 RID: 10609
	private Dictionary<int, int> _sortingTypeMaxValueCache;

	// Token: 0x04002972 RID: 10610
	private List<int> _maxValueCharIdList;

	// Token: 0x04002973 RID: 10611
	private bool _canQuickHide;

	// Token: 0x04002974 RID: 10612
	private bool _allowQuickHide;

	// Token: 0x04002975 RID: 10613
	private string _disableTips;

	// Token: 0x04002976 RID: 10614
	private bool _isSelectInvite;

	// Token: 0x04002977 RID: 10615
	private bool _triggerQuickHideBySystem = true;

	// Token: 0x04002978 RID: 10616
	private byte _simpleViewStatType;

	// Token: 0x04002979 RID: 10617
	private bool _showFiveElementType;

	// Token: 0x0200182B RID: 6187
	private class DetailColumnDisplayConfig
	{
		// Token: 0x0400ADCF RID: 44495
		public int ColumnType;

		// Token: 0x0400ADD0 RID: 44496
		public string SpecifyItem;

		// Token: 0x0400ADD1 RID: 44497
		public string ContentPrefix;

		// Token: 0x0400ADD2 RID: 44498
		public string Background;

		// Token: 0x0400ADD3 RID: 44499
		public string ButtonName;
	}

	// Token: 0x0200182C RID: 6188
	public static class SimpleViewStatType
	{
		// Token: 0x0400ADD4 RID: 44500
		public const byte Attraction = 1;

		// Token: 0x0400ADD5 RID: 44501
		public const byte Happiness = 2;

		// Token: 0x0400ADD6 RID: 44502
		public const byte BehaviorType = 4;

		// Token: 0x0400ADD7 RID: 44503
		public const byte FameType = 8;

		// Token: 0x0400ADD8 RID: 44504
		public const byte Settlement = 16;

		// Token: 0x0400ADD9 RID: 44505
		public const byte Grade = 32;

		// Token: 0x0400ADDA RID: 44506
		public const byte ConsummateLevel = 64;

		// Token: 0x0400ADDB RID: 44507
		public const byte DefaultGroup = 63;

		// Token: 0x0400ADDC RID: 44508
		public const byte LifeLinkGroup = 119;
	}
}
