using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Components.Avatar;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.LegendaryBook;
using GameData.Domains.Map;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000383 RID: 899
public class UI_GiveUpLegendaryBook : UIBase
{
	// Token: 0x060034EB RID: 13547 RVA: 0x001A687C File Offset: 0x001A4A7C
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get("CorpseTemplateId", out this._corpseTemplateId);
		argsBox.Get("CorpseId", out this._corpseId);
		argsBox.Get("CorpseIndex", out this._corpseIndex);
		this.InitSort();
		this._selectedNotch = -1;
		this._selectedBookTarget = -1;
		bool inited = this._inited;
		if (inited)
		{
			this.RefreshView();
		}
	}

	// Token: 0x060034EC RID: 13548 RVA: 0x001A68E8 File Offset: 0x001A4AE8
	public void Awake()
	{
		this._scrollView = base.CGet<InfinityScrollLegacy>("VerticalScrollView");
		this._scrollView.OnItemRender = new Action<int, Refers>(this.OnRenderChar);
		this._notchItemList = base.CGetList<Refers>("GiveUpLegendaryBookBottomItem");
		this._filterController = base.CGet<HorizontalPageSwitchController>("PageSwitchController");
		this._filterController.PageItemRefreshHandler = new Action<int, Refers>(this.RefreshLegendaryBookFilterItem);
		this._filterController.SetItemSelectStateHandler = new Action<Refers, bool>(this.SetLegendaryBookFilterSelectState);
		this._filterController.InitPageCount(0, 0, false);
		this._filterController.RegisterOnSelectIndexChangeHandler(new Action<int>(this.OnSelectLegendaryBookFilterIndexChange));
		this._searchText = base.CGet<TMP_InputField>("SearchOwner");
		this._searchText.onValueChanged.RemoveAllListeners();
		this._searchText.onValueChanged.AddListener(delegate(string inputValue)
		{
			this.FilterOwnerListBySearchName();
		});
		this._searchText.onEndEdit.RemoveAllListeners();
		this._searchText.onEndEdit.AddListener(delegate(string inputValue)
		{
			this.FilterOwnerListBySearchName();
		});
	}

	// Token: 0x060034ED RID: 13549 RVA: 0x001A6A04 File Offset: 0x001A4C04
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(11, 1, ulong.MaxValue, null));
		int taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)taiwuId), new uint[]
		{
			34U,
			66U
		}));
	}

	// Token: 0x060034EE RID: 13550 RVA: 0x001A6A5C File Offset: 0x001A4C5C
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b != 0)
			{
				if (b != 1)
				{
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				int taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				bool flag = uid.DataId == 1;
				if (flag)
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._selectLegendaryBookOwnerData);
					this.RefreshView();
					this.Element.ShowAfterRefresh();
					this._inited = true;
				}
				else
				{
					bool flag2 = uid.DomainId == 4 && uid.DataId == 0 && (int)uid.SubId0 == taiwuId;
					if (flag2)
					{
						bool flag3 = uid.SubId1 == 34U;
						if (flag3)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._taiwuResources);
							this.RefreshNotchSelect();
						}
						else
						{
							bool flag4 = uid.SubId1 == 66U;
							if (flag4)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._taiwuExp);
								this.RefreshNotchSelect();
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060034EF RID: 13551 RVA: 0x001A6BCC File Offset: 0x001A4DCC
	private void ConvertOwnerDataToOwnerList()
	{
		bool flag = this._selectLegendaryBookOwnerData == null;
		if (!flag)
		{
			this._ownerList.Clear();
			int taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			foreach (KeyValuePair<sbyte, int> keyValuePair in this._selectLegendaryBookOwnerData.BookMap)
			{
				sbyte b;
				int num;
				keyValuePair.Deconstruct(out b, out num);
				sbyte bookType = b;
				int ownerId = num;
				bool flag2 = ownerId == taiwuId;
				if (!flag2)
				{
					CharacterDisplayData characterData;
					this._selectLegendaryBookOwnerData.CharacterDisplayDataMap.TryGetValue(ownerId, out characterData);
					sbyte happiness;
					this._selectLegendaryBookOwnerData.CharacterHappinessMap.TryGetValue(ownerId, out happiness);
					MainAttributes attributes;
					this._selectLegendaryBookOwnerData.CharacterAttributeMap.TryGetValue(ownerId, out attributes);
					short health;
					this._selectLegendaryBookOwnerData.CharacterHealthMap.TryGetValue(ownerId, out health);
					short leftMaxHealth;
					this._selectLegendaryBookOwnerData.CharacterLeftMaxHealthMap.TryGetValue(ownerId, out leftMaxHealth);
					UI_GiveUpLegendaryBook.OwnerDisplayData ownerDisplayDataData = new UI_GiveUpLegendaryBook.OwnerDisplayData
					{
						CharracterData = characterData,
						Happiness = happiness,
						Attributes = attributes,
						BookType = bookType,
						Health = health,
						LeftMaxHealth = leftMaxHealth
					};
					this._ownerList.Add(ownerDisplayDataData);
				}
			}
		}
	}

	// Token: 0x060034F0 RID: 13552 RVA: 0x001A6D24 File Offset: 0x001A4F24
	private void UpdateAllBookTypeList()
	{
		bool flag = this._selectLegendaryBookOwnerData == null;
		if (!flag)
		{
			this._allBookTypeList.Clear();
			int taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			foreach (KeyValuePair<sbyte, int> keyValuePair in this._selectLegendaryBookOwnerData.BookMap)
			{
				sbyte b;
				int num;
				keyValuePair.Deconstruct(out b, out num);
				sbyte bookType = b;
				int ownerId = num;
				bool flag2 = taiwuId == ownerId;
				if (!flag2)
				{
					this._allBookTypeList.Add(bookType);
				}
			}
		}
	}

	// Token: 0x060034F1 RID: 13553 RVA: 0x001A6DD0 File Offset: 0x001A4FD0
	private void RefreshView()
	{
		this._selectedNotch = 0;
		this.RefreshConfirmButton();
		this.RefreshScrollView();
		this.RefreshNotchSelect();
		this._filterController.InitPageCount(this._allBookTypeList.Count + 1, 0, false);
	}

	// Token: 0x060034F2 RID: 13554 RVA: 0x001A6E0C File Offset: 0x001A500C
	private void RefreshConfirmButton()
	{
		CButtonObsolete button = base.CGet<CButtonObsolete>("ButtonConfirm");
		button.interactable = (this._isMoneyEnough && this._selectedBookTarget != -1 && this._selectedNotch != -1);
	}

	// Token: 0x060034F3 RID: 13555 RVA: 0x001A6E50 File Offset: 0x001A5050
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "ButtonConfirm";
		if (flag)
		{
			ExtraDomainMethod.Call.SetRanshanThreeCorpsesCharacterTarget(this._corpseTemplateId, this._selectedBookTarget, this._selectedNotch);
			UIManager.Instance.HideUI(this.Element);
			ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
			argsBox.Clear();
			argsBox.Set("CorpseIndex", this._corpseIndex);
			GEvent.OnEvent(UiEvents.OnLegendaryBookGiveUpStart, argsBox);
		}
		else
		{
			bool flag2 = btnName == "ButtonCancel";
			if (flag2)
			{
				UIManager.Instance.HideUI(this.Element);
			}
		}
	}

	// Token: 0x060034F4 RID: 13556 RVA: 0x001A6EF3 File Offset: 0x001A50F3
	private void RefreshScrollView()
	{
		this.UpdateAllBookTypeList();
		this.ConvertOwnerDataToOwnerList();
		this._filteredOwnerList.Sort(new Comparison<UI_GiveUpLegendaryBook.OwnerDisplayData>(this.CompareOwner));
		this._scrollView.UpdateData(this._filteredOwnerList.Count);
	}

	// Token: 0x060034F5 RID: 13557 RVA: 0x001A6F34 File Offset: 0x001A5134
	private void OnRenderChar(int index, Refers charView)
	{
		UI_GiveUpLegendaryBook.OwnerDisplayData displayData = this._filteredOwnerList[index];
		charView.CGet<Game.Components.Avatar.Avatar>("Avatar").Refresh(displayData.CharracterData, true);
		charView.CGet<CImage>("BookTypeImage").SetSprite("mousetip_gongfa_" + displayData.BookType.ToString(), false, null);
		string bookName = LocalStringManager.Get(string.Format("LK_LegendaryBook_{0}", displayData.BookType));
		charView.CGet<TextMeshProUGUI>("BookName").text = bookName;
		charView.CGet<TextMeshProUGUI>("Happiness").text = CommonUtils.GetHappinessString(displayData.Happiness);
		WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
		string orgName = mapModel.GetSettlementName(displayData.CharracterData.OrgInfo);
		charView.CGet<TextMeshProUGUI>("Orgnanization").text = orgName;
		string identity = CommonUtils.GetIdentityString(displayData.CharracterData.OrgInfo, displayData.CharracterData.Gender, displayData.CharracterData.PhysiologicalAge, false);
		charView.CGet<TextMeshProUGUI>("Identity").text = identity;
		sbyte attributeType = UI_GiveUpLegendaryBook.ThreeCorpseTypeAttributeTypeMap[this._corpseTemplateId][0];
		charView.CGet<TextMeshProUGUI>("Attribute1").text = this.GetMainAttribute(displayData.Attributes, attributeType).ToString();
		CharacterPropertyDisplayItem propertyItem = CharacterPropertyDisplay.Instance.GetItem((short)attributeType);
		charView.CGet<TextMeshProUGUI>("AttributeName1").text = propertyItem.Name;
		sbyte attributeType2 = UI_GiveUpLegendaryBook.ThreeCorpseTypeAttributeTypeMap[this._corpseTemplateId][1];
		charView.CGet<TextMeshProUGUI>("Attribute2").text = this.GetMainAttribute(displayData.Attributes, attributeType2).ToString();
		propertyItem = CharacterPropertyDisplay.Instance.GetItem((short)attributeType2);
		charView.CGet<TextMeshProUGUI>("AttributeName2").text = propertyItem.Name;
		MapDomainMethod.AsyncCall.GetBlockFullName(null, displayData.CharracterData.Location, delegate(int offsetData, RawDataPool poolData)
		{
			FullBlockName fullBlockName = default(FullBlockName);
			Serializer.Deserialize(poolData, offsetData, ref fullBlockName);
			MapStateItem stateConfig = MapState.Instance[fullBlockName.stateTemplateId];
			MapAreaItem areaConfig = MapArea.Instance[fullBlockName.areaTemplateId];
			bool flag = stateConfig == null && areaConfig == null;
			if (flag)
			{
				charView.CGet<TextMeshProUGUI>("Location").text = LocalStringManager.Get(LanguageKey.LK_Character_Location_Format_Invalid);
			}
			else
			{
				charView.CGet<TextMeshProUGUI>("Location").text = stateConfig.Name + "-" + areaConfig.Name;
			}
		});
		short displayAge = displayData.CharracterData.PhysiologicalAge;
		charView.CGet<TextMeshProUGUI>("Age").text = ((displayAge < 0) ? string.Empty : LocalStringManager.GetFormat(LanguageKey.LK_Age, displayAge));
		string healthName = CommonUtils.GetCharacterHealthInfo(displayData.Health, displayData.LeftMaxHealth, displayData.CharracterData.CharacterId).Item1;
		charView.CGet<TextMeshProUGUI>("Health").text = healthName;
		int taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		charView.CGet<TextMeshProUGUI>("Name").text = NameCenter.GetCharMonasticTitleOrNameByDisplayData(displayData.CharracterData, displayData.CharracterData.CharacterId == taiwuId, false);
		Toggle toggle = charView.GetComponent<Toggle>();
		CButtonObsolete button = charView.transform.Find("Button").GetComponent<CButtonObsolete>();
		toggle.isOn = (this._selectedBookTarget == displayData.BookType);
		button.ClearAndAddListener(delegate
		{
			toggle.isOn = true;
			this._selectedBookTarget = displayData.BookType;
			this.UpdateMoneyEnough();
			this.RefreshConfirmButton();
		});
	}

	// Token: 0x060034F6 RID: 13558 RVA: 0x001A72D8 File Offset: 0x001A54D8
	private unsafe short GetMainAttribute(MainAttributes attributes, sbyte type)
	{
		return *(ref attributes.Items.FixedElementField + (IntPtr)type * 2);
	}

	// Token: 0x060034F7 RID: 13559 RVA: 0x001A72FC File Offset: 0x001A54FC
	private void InitSort()
	{
		this._sortFilterSetting = SingletonObject.getInstance<GameSort>().GetCharacterSortConfig(base.GetType().Name);
		this._characterSort = base.CGet<CharacterSort>("CharacterSort");
		this._characterSort.Init(this._sortFilterSetting, new Action<CharacterSortFilterSetting>(this.OnSortOrderChange));
	}

	// Token: 0x060034F8 RID: 13560 RVA: 0x001A7354 File Offset: 0x001A5554
	private void OnSortOrderChange(CharacterSortFilterSetting sortFilterSetting)
	{
		this._filteredOwnerList.Sort(new Comparison<UI_GiveUpLegendaryBook.OwnerDisplayData>(this.CompareOwner));
		this._scrollView.ReRender();
	}

	// Token: 0x060034F9 RID: 13561 RVA: 0x001A737C File Offset: 0x001A557C
	private int CompareOwner(UI_GiveUpLegendaryBook.OwnerDisplayData char1, UI_GiveUpLegendaryBook.OwnerDisplayData char2)
	{
		int taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		for (int i = 0; i < this._sortFilterSetting.SortOrders.Count; i++)
		{
			switch (this._sortFilterSetting.SortOrders[i].Item1)
			{
			case 0:
			{
				string itemNameL = NameCenter.GetCharMonasticTitleOrNameByDisplayData(char1.CharracterData, char1.CharracterData.CharacterId == taiwuId, false);
				string itemNameR = NameCenter.GetCharMonasticTitleOrNameByDisplayData(char2.CharracterData, char2.CharracterData.CharacterId == taiwuId, false);
				bool flag = itemNameL != itemNameR;
				if (flag)
				{
					return (!this._sortFilterSetting.SortOrders[i].Item2) ? Utils_Sorting.CompareByCurrentLangEncoding(itemNameL, itemNameR) : Utils_Sorting.CompareByCurrentLangEncoding(itemNameR, itemNameL);
				}
				break;
			}
			case 1:
			{
				sbyte itemGradeL = char1.CharracterData.OrgInfo.Grade;
				sbyte itemGradeR = char2.CharracterData.OrgInfo.Grade;
				bool flag2 = itemGradeL != itemGradeR;
				if (flag2)
				{
					return this._sortFilterSetting.SortOrders[i].Item2 ? itemGradeR.CompareTo(itemGradeL) : itemGradeL.CompareTo(itemGradeR);
				}
				break;
			}
			case 2:
			{
				short age = char1.CharracterData.PhysiologicalAge;
				short age2 = char2.CharracterData.PhysiologicalAge;
				bool flag3 = age != age2;
				if (flag3)
				{
					return this._sortFilterSetting.SortOrders[i].Item2 ? age2.CompareTo(age) : age.CompareTo(age2);
				}
				break;
			}
			case 3:
			{
				float healthPercent = (float)char1.Health / (float)char1.LeftMaxHealth;
				float healthPercent2 = (float)char2.Health / (float)char2.LeftMaxHealth;
				bool flag4 = !Mathf.Approximately(healthPercent, healthPercent2);
				if (flag4)
				{
					return this._sortFilterSetting.SortOrders[i].Item2 ? healthPercent2.CompareTo(healthPercent) : healthPercent.CompareTo(healthPercent2);
				}
				break;
			}
			case 4:
			{
				sbyte gender = char1.CharracterData.Gender;
				sbyte gender2 = char2.CharracterData.Gender;
				bool flag5 = gender != gender2;
				if (flag5)
				{
					return this._sortFilterSetting.SortOrders[i].Item2 ? gender.CompareTo(gender2) : gender2.CompareTo(gender);
				}
				break;
			}
			case 5:
			{
				sbyte behaviorType = char1.CharracterData.BehaviorType;
				sbyte behaviorType2 = char2.CharracterData.BehaviorType;
				bool flag6 = behaviorType != behaviorType2;
				if (flag6)
				{
					return this._sortFilterSetting.SortOrders[i].Item2 ? behaviorType.CompareTo(behaviorType2) : behaviorType2.CompareTo(behaviorType);
				}
				break;
			}
			case 6:
			{
				sbyte happiness = char1.Happiness;
				sbyte happiness2 = char2.Happiness;
				bool flag7 = happiness != happiness2;
				if (flag7)
				{
					return this._sortFilterSetting.SortOrders[i].Item2 ? happiness2.CompareTo(happiness) : happiness.CompareTo(happiness2);
				}
				break;
			}
			case 7:
			{
				short favorite = char1.CharracterData.FavorabilityToTaiwu;
				short favorite2 = char2.CharracterData.FavorabilityToTaiwu;
				bool flag8 = favorite != favorite2;
				if (flag8)
				{
					return this._sortFilterSetting.SortOrders[i].Item2 ? favorite2.CompareTo(favorite) : favorite.CompareTo(favorite2);
				}
				break;
			}
			}
		}
		return char1.CharracterData.CharacterId.CompareTo(char2.CharracterData.CharacterId);
	}

	// Token: 0x060034FA RID: 13562 RVA: 0x001A7748 File Offset: 0x001A5948
	private unsafe void RefreshNotchSelect()
	{
		byte[] configKeys = UI_GiveUpLegendaryBook.RanshanThreeCorpsesActionData[this._corpseTemplateId];
		ToggleGroup toggleGroup = base.CGet<ToggleGroup>("Bottom");
		toggleGroup.allowSwitchOff = true;
		for (int i = 0; i < this._notchItemList.Count; i++)
		{
			RanshanGiveupLegendaryBookItem config = RanshanGiveupLegendaryBook.Instance[configKeys[i]];
			Refers item = this._notchItemList[i];
			Toggle buttonToggle = item.CGet<Toggle>("ButtonToggle");
			buttonToggle.group = toggleGroup;
			buttonToggle.isOn = ((int)this._selectedNotch == i);
			TextMeshProUGUI buttonLabel = item.CGet<TextMeshProUGUI>("ButtonText");
			buttonLabel.text = LocalStringManager.Get(string.Format("LK_LegendaryBook_GiveUp_NotchButton_{0}", i + 1));
			TextMeshProUGUI costTimeLabel = item.CGet<TextMeshProUGUI>("CostTime");
			costTimeLabel.text = config.FollowDuration.ToString();
			this._costResource = 0;
			this._taiwuResource = 0;
			CImage resouceIcon = item.CGet<CImage>("ResourceIcon");
			TextMeshProUGUI resourceTypeLabel = item.CGet<TextMeshProUGUI>("ResourceTypeLabel");
			bool flag = config.ResourceType != -1;
			if (flag)
			{
				this._taiwuResource = *this._taiwuResources[(int)config.ResourceType];
				this._costResource = config.ResourceCost;
				bool flag2 = config.ResourceType == 6;
				if (flag2)
				{
					resouceIcon.SetSprite("sp_icon_resource_money", false, null);
					resourceTypeLabel.text = LocalStringManager.Get(LanguageKey.LK_Resource_Name_Money);
				}
				else
				{
					bool flag3 = config.ResourceType == 7;
					if (flag3)
					{
						resouceIcon.SetSprite("sp_icon_resource_authority", false, null);
						resourceTypeLabel.text = LocalStringManager.Get(LanguageKey.LK_Resource_Name_Authority);
					}
				}
			}
			else
			{
				this._taiwuResource = this._taiwuExp;
				this._costResource = config.ExpCost;
				resouceIcon.SetSprite("sp_icon_lilianyuan", false, null);
				resourceTypeLabel.text = LocalStringManager.Get(LanguageKey.LK_Exp);
			}
			TextMeshProUGUI haveResourceLabel = item.CGet<TextMeshProUGUI>("HaveResource");
			haveResourceLabel.text = ((this._taiwuResource >= this._costResource) ? ("<color=#brightblue>" + CommonUtils.GetDisplayStringForNum(this._taiwuResource, 100000) + "</color>").ColorReplace() : ("<color=#brightred>" + CommonUtils.GetDisplayStringForNum(this._taiwuResource, 100000) + "</color>").ColorReplace());
			TextMeshProUGUI costResourceLabel = item.CGet<TextMeshProUGUI>("CostResource");
			costResourceLabel.text = this._costResource.ToString();
			TextMeshProUGUI happinessChangeLabel = item.CGet<TextMeshProUGUI>("HappinessChange");
			happinessChangeLabel.text = this.GetHappinessChangeText(config.MoodChange);
			CButtonObsolete button = item.CGet<CButtonObsolete>("SelectLevelButton");
			int ii = i;
			button.ClearAndAddListener(delegate
			{
				buttonToggle.isOn = true;
				this._selectedNotch = (sbyte)ii;
				this.UpdateMoneyEnough();
				this.RefreshConfirmButton();
			});
		}
	}

	// Token: 0x060034FB RID: 13563 RVA: 0x001A7A35 File Offset: 0x001A5C35
	private void UpdateMoneyEnough()
	{
		this._isMoneyEnough = (this._taiwuResource >= this._costResource);
	}

	// Token: 0x060034FC RID: 13564 RVA: 0x001A7A50 File Offset: 0x001A5C50
	private string GetHappinessChangeText(int changeValue)
	{
		string result;
		if (changeValue != 5)
		{
			if (changeValue != 10)
			{
				if (changeValue != 15)
				{
					result = "";
				}
				else
				{
					result = LocalStringManager.Get(LanguageKey.LK_LegendaryBook_GiveUp_HappinessChange_3);
				}
			}
			else
			{
				result = LocalStringManager.Get(LanguageKey.LK_LegendaryBook_GiveUp_HappinessChange_2);
			}
		}
		else
		{
			result = LocalStringManager.Get(LanguageKey.LK_LegendaryBook_GiveUp_HappinessChange_1);
		}
		return result;
	}

	// Token: 0x060034FD RID: 13565 RVA: 0x001A7AA6 File Offset: 0x001A5CA6
	private void OnSelectLegendaryBookFilterIndexChange(int index)
	{
		this.FilterOwnerListByBookType(index);
	}

	// Token: 0x060034FE RID: 13566 RVA: 0x001A7AB1 File Offset: 0x001A5CB1
	private void SetLegendaryBookFilterSelectState(Refers refers, bool isSelected)
	{
		refers.CGet<CToggleObsolete>("Toggle").isOn = isSelected;
		refers.CGet<CToggleObsolete>("Toggle").interactable = !isSelected;
	}

	// Token: 0x060034FF RID: 13567 RVA: 0x001A7ADC File Offset: 0x001A5CDC
	private void RefreshLegendaryBookFilterItem(int index, Refers refers)
	{
		TextMeshProUGUI labelOff = refers.CGet<TextMeshProUGUI>("LabelOff");
		TextMeshProUGUI labelOn = refers.CGet<TextMeshProUGUI>("LabelOn");
		bool flag = index == 0;
		if (flag)
		{
			labelOff.text = LocalStringManager.Get(LanguageKey.LK_Filter_Type_All);
			labelOn.text = LocalStringManager.Get(LanguageKey.LK_Filter_Type_All);
		}
		else
		{
			labelOff.text = LocalStringManager.Get(string.Format("LK_LegendaryBook_{0}", this._allBookTypeList[index - 1]));
			labelOn.text = LocalStringManager.Get(string.Format("LK_LegendaryBook_{0}", this._allBookTypeList[index - 1]));
		}
		refers.CGet<CToggleObsolete>("Toggle").onValueChanged.RemoveAllListeners();
		refers.CGet<CToggleObsolete>("Toggle").onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				this.CGet<HorizontalPageSwitchController>("PageSwitchController").SetSelect(index, true);
			}
		});
	}

	// Token: 0x06003500 RID: 13568 RVA: 0x001A7BE0 File Offset: 0x001A5DE0
	private void FilterOwnerListByBookType(int filterIndex)
	{
		bool lockFilter = this._lockFilter;
		if (!lockFilter)
		{
			this._lockSearch = true;
			this._searchText.text = "";
			this._lockSearch = false;
			this._filteredOwnerList.Clear();
			foreach (UI_GiveUpLegendaryBook.OwnerDisplayData owner in this._ownerList)
			{
				bool flag = filterIndex == 0 || owner.BookType == this._allBookTypeList[filterIndex - 1];
				if (flag)
				{
					this._filteredOwnerList.Add(owner);
				}
			}
			this.RefreshScrollView();
		}
	}

	// Token: 0x06003501 RID: 13569 RVA: 0x001A7CA4 File Offset: 0x001A5EA4
	private void FilterOwnerListBySearchName()
	{
		bool lockSearch = this._lockSearch;
		if (!lockSearch)
		{
			this._lockFilter = true;
			this._filterController.SelectFirst();
			this._lockFilter = false;
			this._filteredOwnerList.Clear();
			int taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			foreach (UI_GiveUpLegendaryBook.OwnerDisplayData owner in this._ownerList)
			{
				string ownerName = NameCenter.GetCharMonasticTitleOrNameByDisplayData(owner.CharracterData, owner.CharracterData.CharacterId == taiwuId, false);
				bool flag = ownerName.Contains(this._searchText.text);
				if (flag)
				{
					this._filteredOwnerList.Add(owner);
				}
			}
			this.RefreshScrollView();
		}
	}

	// Token: 0x04002665 RID: 9829
	private CharacterSort _characterSort;

	// Token: 0x04002666 RID: 9830
	private CharacterSortFilterSetting _sortFilterSetting;

	// Token: 0x04002667 RID: 9831
	private InfinityScrollLegacy _scrollView;

	// Token: 0x04002668 RID: 9832
	private List<Refers> _notchItemList;

	// Token: 0x04002669 RID: 9833
	private HorizontalPageSwitchController _filterController;

	// Token: 0x0400266A RID: 9834
	private TMP_InputField _searchText;

	// Token: 0x0400266B RID: 9835
	private static readonly Dictionary<short, sbyte[]> ThreeCorpseTypeAttributeTypeMap = new Dictionary<short, sbyte[]>
	{
		{
			700,
			new sbyte[]
			{
				2,
				4
			}
		},
		{
			698,
			new sbyte[]
			{
				0,
				3
			}
		},
		{
			699,
			new sbyte[]
			{
				1,
				5
			}
		}
	};

	// Token: 0x0400266C RID: 9836
	private static readonly Dictionary<short, byte[]> RanshanThreeCorpsesActionData = new Dictionary<short, byte[]>
	{
		{
			698,
			new byte[]
			{
				0,
				1,
				2
			}
		},
		{
			699,
			new byte[]
			{
				3,
				4,
				5
			}
		},
		{
			700,
			new byte[]
			{
				6,
				7,
				8
			}
		}
	};

	// Token: 0x0400266D RID: 9837
	private ResourceInts _taiwuResources;

	// Token: 0x0400266E RID: 9838
	private int _taiwuExp;

	// Token: 0x0400266F RID: 9839
	private LegendaryBookOwnerData _selectLegendaryBookOwnerData;

	// Token: 0x04002670 RID: 9840
	private readonly List<sbyte> _allBookTypeList = new List<sbyte>();

	// Token: 0x04002671 RID: 9841
	private readonly List<UI_GiveUpLegendaryBook.OwnerDisplayData> _ownerList = new List<UI_GiveUpLegendaryBook.OwnerDisplayData>();

	// Token: 0x04002672 RID: 9842
	private readonly List<UI_GiveUpLegendaryBook.OwnerDisplayData> _filteredOwnerList = new List<UI_GiveUpLegendaryBook.OwnerDisplayData>();

	// Token: 0x04002673 RID: 9843
	private short _corpseTemplateId;

	// Token: 0x04002674 RID: 9844
	private int _corpseId;

	// Token: 0x04002675 RID: 9845
	private int _corpseIndex;

	// Token: 0x04002676 RID: 9846
	private sbyte _selectedNotch = -1;

	// Token: 0x04002677 RID: 9847
	private sbyte _selectedBookTarget = -1;

	// Token: 0x04002678 RID: 9848
	private bool _isMoneyEnough = false;

	// Token: 0x04002679 RID: 9849
	private bool _lockFilter = false;

	// Token: 0x0400267A RID: 9850
	private bool _lockSearch = false;

	// Token: 0x0400267B RID: 9851
	private int _taiwuResource;

	// Token: 0x0400267C RID: 9852
	private int _costResource;

	// Token: 0x0400267D RID: 9853
	private bool _inited = false;

	// Token: 0x02001786 RID: 6022
	private class OwnerDisplayData
	{
		// Token: 0x0400ABE5 RID: 44005
		public CharacterDisplayData CharracterData;

		// Token: 0x0400ABE6 RID: 44006
		public sbyte Happiness;

		// Token: 0x0400ABE7 RID: 44007
		public MainAttributes Attributes;

		// Token: 0x0400ABE8 RID: 44008
		public sbyte BookType;

		// Token: 0x0400ABE9 RID: 44009
		public short Health;

		// Token: 0x0400ABEA RID: 44010
		public short LeftMaxHealth;
	}
}
