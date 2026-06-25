using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Profession;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x02000392 RID: 914
public class UI_MultiSelectSkillBook : UIBase
{
	// Token: 0x0600367C RID: 13948 RVA: 0x001B6674 File Offset: 0x001B4874
	public override void OnInit(ArgumentBox argsBox)
	{
		bool flag = null == this._popupWindow;
		if (flag)
		{
			this._popupWindow = base.CGet<PopupWindow>("PopupWindowBase");
		}
		string title;
		bool flag2 = !argsBox.Get("title", out title);
		if (flag2)
		{
			title = LocalStringManager.Get(LanguageKey.LK_UI_SelectItem);
		}
		this._popupWindow.SetTitle(title);
		ItemSortAndFilter.ItemFilterType filterType;
		this._filterType = (argsBox.Get<ItemSortAndFilter.ItemFilterType>("filterType", out filterType) ? filterType : ItemSortAndFilter.ItemFilterType.Invalid);
		this._popupWindow.CancelButton.interactable = true;
		this._popupWindow.OnConfirmClick = new Action(this.OnConfirmSelect);
		this._popupWindow.ConfirmButton.interactable = true;
		this._popupWindow.OnCancelClick = new Action(this.OnCancel);
		List<ItemDisplayData> _treasuryItemList = new List<ItemDisplayData>();
		List<ItemDisplayData> _inventoryCanSoldItemList = new List<ItemDisplayData>();
		List<ItemDisplayData> _warehouseCanSoldItemList = new List<ItemDisplayData>();
		List<ItemDisplayData> _treasuryCanSoldItemList = new List<ItemDisplayData>();
		TaiwuDomainMethod.AsyncCall.GetAllWarehouseItemsExcludeValueZero(null, delegate(int offset, RawDataPool dataPool)
		{
			List<ItemDisplayData> itemDisplayDatas = new List<ItemDisplayData>();
			Serializer.Deserialize(dataPool, offset, ref itemDisplayDatas);
			this.FilterItemByList(itemDisplayDatas, _warehouseCanSoldItemList);
		});
		CharacterDomainMethod.AsyncCall.GetAllInventoryItemsExcludeValueZero(null, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
		{
			List<ItemDisplayData> itemDisplayDatas = new List<ItemDisplayData>();
			Serializer.Deserialize(dataPool, offset, ref itemDisplayDatas);
			this.FilterItemByList(itemDisplayDatas, _inventoryCanSoldItemList);
		});
		TaiwuDomainMethod.AsyncCall.GetAllTreasuryItems(null, delegate(int offset, RawDataPool dataPool)
		{
			List<ItemDisplayData> itemDisplayDatas = new List<ItemDisplayData>();
			Serializer.Deserialize(dataPool, offset, ref itemDisplayDatas);
			_treasuryItemList.AddRange(from data in itemDisplayDatas
			where data.Value > 0L
			select data);
			this.FilterItemByList(_treasuryItemList, _treasuryCanSoldItemList);
		});
		TaiwuDomainMethod.AsyncCall.GetTreasuryNeededItemList(null, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._villagerNeededItemSet);
		});
		this._warehouseList = _warehouseCanSoldItemList;
		this._inventoryList = _inventoryCanSoldItemList;
		this._treasuryList = _treasuryCanSoldItemList;
		argsBox.Get("level", out this._level);
		argsBox.Get("canTransfer", out this._canTransfer);
		argsBox.Get<Action<List<ItemKey>>>("callback", out this._onConfirm);
		argsBox.Get("isCombatSkillBook", out this._isCombatSkill);
		this._operateItemRecord.Clear();
		this._operateTypeRecord.Clear();
		this._startSoldItemList.Clear();
		this._soldItemViewList.Clear();
		this._canSoldCount = (int)this._level;
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		UIElement element2 = this.Element;
		element2.OnShowed = (Action)Delegate.Combine(element2.OnShowed, new Action(delegate()
		{
			ConchShipCursor.Instance.SetSelectCountActive(true);
		}));
	}

	// Token: 0x0600367D RID: 13949 RVA: 0x001B68D4 File Offset: 0x001B4AD4
	private void OpenMultiSelectCharWindow()
	{
		HashSet<int> charIdSet = new HashSet<int>();
		MapBlockData block = SingletonObject.getInstance<WorldMapModel>().CurrentBlockData;
		bool flag = block.CharacterSet != null;
		if (flag)
		{
			charIdSet.UnionWith(block.CharacterSet);
		}
		charIdSet.UnionWith(SingletonObject.getInstance<CharacterMonitorModel>().GetTaiwuTeamCharIds());
		List<int> selectionList = charIdSet.ToList<int>();
		selectionList.Remove(SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
		Dictionary<int, CharacterDisplayData> displayDataDict = new Dictionary<int, CharacterDisplayData>();
		List<CharacterDisplayData> displayData = new List<CharacterDisplayData>();
		Action<int[]> <>9__1;
		CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, selectionList, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref displayData);
			foreach (CharacterDisplayData data in displayData)
			{
				bool flag2 = AgeGroup.GetAgeGroup(data.PhysiologicalAge) >= 1;
				if (flag2)
				{
					displayDataDict.Add(data.CharacterId, data);
				}
				else
				{
					selectionList.Remove(data.CharacterId);
				}
			}
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			ArgumentBox argumentBox = argBox;
			string key = "onMultiSelect";
			Action<int[]> arg;
			if ((arg = <>9__1) == null)
			{
				arg = (<>9__1 = delegate(int[] charIds)
				{
					this._charIds = charIds.ToList<int>();
					this._charIds.Add(SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
					this.OpenProfessionSkillConfirm();
				});
			}
			argumentBox.SetObject(key, arg);
			argBox.SetObject("charIdList", selectionList);
			argBox.Set("selectCount", GlobalConfig.Instance.TeachSkillCharacterMaxCount);
			argBox.Set("enableMultiSelect", true);
			UIElement.SelectCharLegacy.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.SelectCharLegacy, true);
		});
	}

	// Token: 0x0600367E RID: 13950 RVA: 0x001B697C File Offset: 0x001B4B7C
	private void OpenProfessionSkillConfirm()
	{
		List<int> bookIds = new List<int>();
		foreach (ItemKey bookIdItemKey in this._bookItemKeys)
		{
			bookIds.Add(bookIdItemKey.Id);
		}
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
		argumentBox.Clear();
		ProfessionSkillArg professionSkillArg = new ProfessionSkillArg
		{
			ProfessionId = (this._isCombatSkill ? 7 : 16),
			SkillId = (this._isCombatSkill ? 31 : 67),
			IsSuccess = true,
			CharIds = this._charIds,
			BookIds = bookIds
		};
		argumentBox.SetObject("ProfessionSkillArg", professionSkillArg);
		argumentBox.SetObject("OnConfirm", new Action(UI_MultiSelectSkillBook.<OpenProfessionSkillConfirm>g__Action|46_0));
		UIElement.ProfessionSkillConfirm.SetOnInitArgs(argumentBox);
		UIManager.Instance.MaskUI(UIElement.ProfessionSkillConfirm);
	}

	// Token: 0x0600367F RID: 13951 RVA: 0x001B6A78 File Offset: 0x001B4C78
	public void OpenTeachSkillResultConfirm(bool isCombat, TasterUltimateResult tasterUltimateResult)
	{
		ArgumentBox args = EasyPool.Get<ArgumentBox>();
		args.SetObject("teachCombatSkillResult", tasterUltimateResult);
		args.SetObject("bookItemKeys", this._bookItemKeys);
		if (isCombat)
		{
			UIElement.TeachCombatSkillResultConfirm.SetOnInitArgs(args);
			UIManager.Instance.ShowUI(UIElement.TeachCombatSkillResultConfirm, true);
		}
		else
		{
			UIElement.TeachLifeSkillResultConfirm.SetOnInitArgs(args);
			UIManager.Instance.ShowUI(UIElement.TeachLifeSkillResultConfirm, true);
		}
	}

	// Token: 0x06003680 RID: 13952 RVA: 0x001B6AF0 File Offset: 0x001B4CF0
	private void OnListenerIdReady()
	{
		ItemDomainMethod.AsyncCall.GetItemDisplayDataList(this, this._startSoldItemList, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._startSoldDatList);
			this.UpdateSoldItemList();
			this.UpdateInventoryScroll();
			this.FreshSoldItemAll();
		});
	}

	// Token: 0x06003681 RID: 13953 RVA: 0x001B6B18 File Offset: 0x001B4D18
	private void UpdateSoldItemList()
	{
		this._soldItemList.Clear();
		bool flag = this._buildingEarnData == null;
		if (flag)
		{
			for (int i = 0; i < (int)this._level; i++)
			{
				this._soldItemList.Add(null);
			}
		}
		else
		{
			for (int j = 0; j < this._buildingEarnData.ShopSoldItemList.Count; j++)
			{
				bool flag2 = this._buildingEarnData.ShopSoldItemList[j].Equals(ItemKey.Invalid);
				if (flag2)
				{
					this._soldItemList.Add(null);
				}
				else
				{
					ItemDisplayData itemDisplayData = this.GetItemDisplayData(this._startSoldDatList, this._buildingEarnData.ShopSoldItemList[j]);
					this._soldItemList.Add(itemDisplayData);
				}
			}
			bool flag3 = (int)this._level > this._buildingEarnData.ShopSoldItemList.Count;
			if (flag3)
			{
				for (int k = 0; k < (int)this._level - this._buildingEarnData.ShopSoldItemList.Count; k++)
				{
					this._soldItemList.Add(null);
				}
			}
		}
	}

	// Token: 0x06003682 RID: 13954 RVA: 0x001B6C54 File Offset: 0x001B4E54
	private void UpdateInventoryScroll()
	{
		this._inventoryScroll.SortAndFilter.StaticAheadItemKeysList.Clear();
		this._togGroup.Get(0).interactable = true;
		bool canTransfer = this._canTransfer;
		if (canTransfer)
		{
			this._togGroup.Set(0, true, true);
			this._currentTogGroupKey = 0;
			this._togGroup.Get(0).interactable = true;
			this._togGroup.Get(0).GetComponent<DisableStyleRoot>().SetStyleEffect(false, false);
			this._inventoryScroll.SetItemList(ref this._inventoryList, true, null, this._inventoryScroll.SortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderInventoryItem));
		}
		else
		{
			this._togGroup.Set(1, true, true);
			this._togGroup.Get(0).interactable = false;
			this._togGroup.Get(0).GetComponent<DisableStyleRoot>().SetStyleEffect(true, false);
			this._currentTogGroupKey = 1;
			this._warehouseScroll.SetItemList(ref this._warehouseList, true, null, this._inventoryScroll.SortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderWarehouseItem));
		}
		this._inventoryScroll.SortAndFilter.LockFilterType(this._filterType, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
	}

	// Token: 0x06003683 RID: 13955 RVA: 0x001B6D98 File Offset: 0x001B4F98
	private ItemView GetItemView()
	{
		GameObject go = this._itemKeyPool.GetObject();
		go.transform.SetParent(this._soldItemListHolder, false);
		this.SetItemViewPos(go);
		go.SetActive(true);
		ItemView itemView = go.GetComponent<ItemView>();
		this._soldItemViewList.Add(itemView);
		return itemView;
	}

	// Token: 0x06003684 RID: 13956 RVA: 0x001B6DF0 File Offset: 0x001B4FF0
	private void SetItemViewPos(GameObject go)
	{
		int count = this.GetActiveChildCount();
		go.transform.localPosition = new Vector3((float)(count - 1) * 114f, 60f, 0f);
	}

	// Token: 0x06003685 RID: 13957 RVA: 0x001B6E2C File Offset: 0x001B502C
	private int GetActiveChildCount()
	{
		int activeCount = 0;
		for (int i = 0; i < this._soldItemListHolder.childCount; i++)
		{
			Transform child = this._soldItemListHolder.GetChild(i);
			bool activeInHierarchy = child.gameObject.activeInHierarchy;
			if (activeInHierarchy)
			{
				activeCount++;
			}
		}
		return activeCount;
	}

	// Token: 0x06003686 RID: 13958 RVA: 0x001B6E84 File Offset: 0x001B5084
	private ItemDisplayData GetItemDisplayData(List<ItemDisplayData> itemDisplayDatas, ItemKey itemKey)
	{
		for (int i = 0; i < itemDisplayDatas.Count; i++)
		{
			bool flag = itemDisplayDatas[i].ContainsItemKey(itemKey);
			if (flag)
			{
				return itemDisplayDatas[i];
			}
		}
		return null;
	}

	// Token: 0x06003687 RID: 13959 RVA: 0x001B6ECC File Offset: 0x001B50CC
	private void FreshSoldItemAll()
	{
		int curCount = this.GetSoldItemCount();
		this._soldItemHolderScrollRect.SetScrollEnable(curCount > 10);
		for (int i = 0; i < this._soldItemViewList.Count; i++)
		{
			this._itemKeyPool.DestroyObject(this._soldItemViewList[i].gameObject);
		}
		for (int j = 0; j < this._soldItemList.Count; j++)
		{
			bool flag = this._soldItemList[j] == null;
			if (!flag)
			{
				int index = j;
				ItemView itemView = this.GetItemView();
				itemView.SetData(this._soldItemList[j], false, 1, false, true, null, false, true);
				itemView.SetSelectState(true);
				itemView.SetClickEvent(delegate
				{
					this.OnClickSoldItem(this._soldItemList[index], index);
				});
			}
		}
		ConchShipCursor.Instance.SetSelectCount(curCount, this._canSoldCount);
		this.FreshInteractable(curCount < this._canSoldCount);
		this.UpdateBtnConfirm(curCount != 0);
	}

	// Token: 0x06003688 RID: 13960 RVA: 0x001B6FE8 File Offset: 0x001B51E8
	private bool CheckSameItemKey(ItemDisplayData itemDisplayData)
	{
		foreach (ItemDisplayData itemView in this._soldItemList)
		{
			bool flag = itemView != null && itemDisplayData.Key.TemplateId == itemView.Key.TemplateId;
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003689 RID: 13961 RVA: 0x001B7064 File Offset: 0x001B5264
	private void FreshInteractable(bool interactable)
	{
		foreach (ItemDisplayData itemData in this._inventoryList)
		{
			itemData.Interactable = interactable;
		}
		foreach (ItemDisplayData itemData2 in this._warehouseList)
		{
			itemData2.Interactable = interactable;
		}
		foreach (ItemDisplayData itemData3 in this._treasuryList)
		{
			itemData3.Interactable = interactable;
		}
		bool flag = !interactable;
		if (!flag)
		{
			foreach (ItemDisplayData itemData4 in this._inventoryList)
			{
				itemData4.Interactable = !this.CheckSameItemKey(itemData4);
			}
			foreach (ItemDisplayData itemData5 in this._warehouseList)
			{
				itemData5.Interactable = !this.CheckSameItemKey(itemData5);
			}
			foreach (ItemDisplayData itemData6 in this._treasuryList)
			{
				itemData6.Interactable = !this.CheckSameItemKey(itemData6);
			}
		}
	}

	// Token: 0x0600368A RID: 13962 RVA: 0x001B7248 File Offset: 0x001B5448
	private int GetSoldItemCount()
	{
		int count = 0;
		for (int i = 0; i < this._soldItemList.Count; i++)
		{
			bool flag = this._soldItemList[i] != null;
			if (flag)
			{
				count++;
			}
		}
		return count;
	}

	// Token: 0x0600368B RID: 13963 RVA: 0x001B7294 File Offset: 0x001B5494
	private void FilterItemByList(List<ItemDisplayData> itemDataList, List<ItemDisplayData> resultList)
	{
		resultList.Clear();
		for (int i = 0; i < itemDataList.Count; i++)
		{
			bool isCombatSkill = this._isCombatSkill;
			if (isCombatSkill)
			{
				bool flag = itemDataList[i].RealKey.ItemType == 10 && SkillBook.Instance.GetItem(itemDataList[i].RealKey.TemplateId).CombatSkillType != -1;
				if (flag)
				{
					resultList.Add(itemDataList[i]);
				}
			}
			else
			{
				bool flag2 = itemDataList[i].RealKey.ItemType == 10 && SkillBook.Instance.GetItem(itemDataList[i].RealKey.TemplateId).CombatSkillType == -1;
				if (flag2)
				{
					resultList.Add(itemDataList[i]);
				}
			}
		}
	}

	// Token: 0x0600368C RID: 13964 RVA: 0x001B7378 File Offset: 0x001B5578
	private void OnClickSoldItem(ItemDisplayData itemDisplayData, int index)
	{
		switch (this._currentTogGroupKey)
		{
		case 0:
			this.SoldItemToInventory(itemDisplayData, index, 1);
			break;
		case 1:
			this.SoldItemToWarehouse(itemDisplayData, index, 1);
			break;
		case 2:
			this.SoldItemToTreasury(itemDisplayData, index, 1);
			break;
		}
	}

	// Token: 0x0600368D RID: 13965 RVA: 0x001B73C8 File Offset: 0x001B55C8
	private void Awake()
	{
		this._itemKeyPool = new PoolItem("ItemView", base.CGet<GameObject>("ItemViewPrefab"));
		this._warehouseScroll = base.CGet<ItemScrollView>("WarehouseItemScroll");
		this._warehouseScroll.Init();
		this._warehouseScroll.SetItemList(ref this._warehouseList, true, "warehouse_warehouse", this._warehouseScroll.SortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderWarehouseItem));
		this._inventoryScroll = base.CGet<ItemScrollView>("InventoryItemScroll");
		this._inventoryScroll.Init();
		this._inventoryScroll.SetItemList(ref this._inventoryList, true, "warehouse_inventory", this._inventoryScroll.SortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderInventoryItem));
		this._treasuryScroll = base.CGet<ItemScrollView>("TreasuryItemScroll");
		this._treasuryScroll.Init();
		this._treasuryScroll.SetItemList(ref this._treasuryList, true, "warehouse_treasury", this._treasuryScroll.SortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderTreasuryItem));
		this._togGroup = base.CGet<CToggleGroupObsolete>("PanelToggleGroup");
		this._togGroup.InitPreOnToggle(-1);
		this._togGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnToggleChange);
		this._inventoryIcon = base.CGet<GameObject>("InventoryIcon");
		this._warehouseIcon = base.CGet<GameObject>("WarehouseIcon");
		this._treasuryIcon = base.CGet<GameObject>("TreasuryIcon");
		this._soldItemListHolder = base.CGet<GameObject>("SoldItemListHolder").transform;
		this._soldItemHolderScrollRect = base.CGet<CScrollRectLegacy>("SoldItemScroll");
	}

	// Token: 0x0600368E RID: 13966 RVA: 0x001B7574 File Offset: 0x001B5774
	private void OnToggleChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		bool flag = newTog == null;
		if (!flag)
		{
			this._currentTogGroupKey = newTog.Key;
			this._inventoryScroll.gameObject.SetActive(newTog.Key == 0);
			this._warehouseScroll.gameObject.SetActive(newTog.Key == 1);
			this._treasuryScroll.gameObject.SetActive(newTog.Key == 2);
			this._inventoryIcon.SetActive(newTog.Key == 0);
			this._warehouseIcon.SetActive(newTog.Key == 1);
			this._treasuryIcon.SetActive(newTog.Key == 2);
			switch (newTog.Key)
			{
			case 0:
				this._inventoryScroll.SetItemList(ref this._inventoryList, true, null, this._inventoryScroll.SortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderInventoryItem));
				break;
			case 1:
				this._warehouseScroll.SetItemList(ref this._warehouseList, true, null, this._inventoryScroll.SortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderWarehouseItem));
				break;
			case 2:
				this._treasuryScroll.SetItemList(ref this._treasuryList, true, null, this._treasuryScroll.SortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderTreasuryItem));
				break;
			}
		}
	}

	// Token: 0x0600368F RID: 13967 RVA: 0x001B76E4 File Offset: 0x001B58E4
	private void OnDisable()
	{
		this._warehouseScroll.SaveSortFilterSetting(false);
		this._inventoryScroll.SaveSortFilterSetting(true);
		this._treasuryScroll.SaveSortFilterSetting(true);
		foreach (ItemView item in this._soldItemViewList)
		{
			this._itemKeyPool.DestroyObject(item.gameObject);
		}
	}

	// Token: 0x06003690 RID: 13968 RVA: 0x001B7770 File Offset: 0x001B5970
	private void OnConfirmSelect()
	{
		ConchShipCursor.Instance.SetSelectCountActive(false);
		this.HideUI();
		this._bookItemKeys = (from item in this._soldItemList
		where item != null
		select item.RealKey).ToList<ItemKey>();
		this.OpenMultiSelectCharWindow();
	}

	// Token: 0x06003691 RID: 13969 RVA: 0x001B77F1 File Offset: 0x001B59F1
	private void OnCancel()
	{
		ConchShipCursor.Instance.SetSelectCountActive(false);
		this.HideUI();
	}

	// Token: 0x06003692 RID: 13970 RVA: 0x001B7807 File Offset: 0x001B5A07
	private void HideUI()
	{
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x06003693 RID: 13971 RVA: 0x001B7830 File Offset: 0x001B5A30
	[Obsolete]
	public void ShowDialogView(string title, string content, Action action)
	{
		DialogCmd cmd = new DialogCmd
		{
			Title = title,
			Content = content,
			Yes = delegate()
			{
				Action action2 = action;
				if (action2 != null)
				{
					action2();
				}
			},
			GroupYesText = LocalStringManager.Get(LanguageKey.LK_HotKeyGroup_Common_Confirm),
			GroupNoText = LocalStringManager.Get(LanguageKey.LK_HotKeyGroup_Common_Cancel)
		};
		UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
		UIManager.Instance.MaskUI(UIElement.Dialog);
	}

	// Token: 0x06003694 RID: 13972 RVA: 0x001B78BC File Offset: 0x001B5ABC
	public override void QuickHide()
	{
		this.OnCancel();
	}

	// Token: 0x06003695 RID: 13973 RVA: 0x001B78C8 File Offset: 0x001B5AC8
	private void OnRenderWarehouseItem(ItemDisplayData itemData, ItemView itemView)
	{
		itemView.UserString = "warehouse";
		itemView.SetClickEvent(delegate
		{
			this.WarehouseItemToSold(itemView.Data, 1);
		});
		bool flag = !itemData.Interactable;
		if (flag)
		{
			itemView.SetInteractionStateLockText(LocalStringManager.Get(LanguageKey.LK_Item_Operation_Locked));
			itemView.CGet<GameObject>("Mask").SetActive(true);
		}
		else
		{
			itemView.CGet<GameObject>("Mask").SetActive(false);
		}
	}

	// Token: 0x06003696 RID: 13974 RVA: 0x001B7968 File Offset: 0x001B5B68
	private void OnRenderInventoryItem(ItemDisplayData itemData, ItemView itemView)
	{
		itemView.UserString = "inventory";
		itemView.SetClickEvent(delegate
		{
			this.InventoryItemToSold(itemView.Data, 1);
		});
		bool flag = !itemData.Interactable;
		if (flag)
		{
			itemView.SetInteractionStateLockText(LocalStringManager.Get(LanguageKey.LK_Item_Operation_Locked));
			itemView.CGet<GameObject>("Mask").SetActive(true);
		}
		else
		{
			itemView.CGet<GameObject>("Mask").SetActive(false);
		}
	}

	// Token: 0x06003697 RID: 13975 RVA: 0x001B7A08 File Offset: 0x001B5C08
	private void OnRenderTreasuryItem(ItemDisplayData itemData, ItemView itemView)
	{
		itemView.UserString = "treasury";
		itemView.SetClickEvent(delegate
		{
			this.TreasuryItemToSold(itemView.Data, 1);
		});
		bool flag = !itemData.Interactable;
		if (flag)
		{
			itemView.SetInteractionStateLockText(LocalStringManager.Get(LanguageKey.LK_Item_Operation_Locked));
			itemView.CGet<GameObject>("Mask").SetActive(true);
		}
		else
		{
			itemView.CGet<GameObject>("Mask").SetActive(false);
		}
		this.SetVillagerNeedMark(itemView, itemData.ItemSourceTypeEnum);
	}

	// Token: 0x06003698 RID: 13976 RVA: 0x001B7ABC File Offset: 0x001B5CBC
	private int AddSoldItemList(ItemDisplayData item)
	{
		for (int i = 0; i < this._soldItemList.Count; i++)
		{
			bool flag = this._soldItemList[i] == null;
			if (flag)
			{
				this._soldItemList[i] = item;
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06003699 RID: 13977 RVA: 0x001B7B11 File Offset: 0x001B5D11
	private void WarehouseItemToSold(ItemDisplayData item, int amount)
	{
	}

	// Token: 0x0600369A RID: 13978 RVA: 0x001B7B14 File Offset: 0x001B5D14
	private void InventoryItemToSold(ItemDisplayData item, int amount)
	{
	}

	// Token: 0x0600369B RID: 13979 RVA: 0x001B7B17 File Offset: 0x001B5D17
	private void TreasuryItemToSold(ItemDisplayData item, int amount)
	{
	}

	// Token: 0x0600369C RID: 13980 RVA: 0x001B7B1A File Offset: 0x001B5D1A
	private void SoldItemToInventory(ItemDisplayData item, int index, int amount)
	{
	}

	// Token: 0x0600369D RID: 13981 RVA: 0x001B7B1D File Offset: 0x001B5D1D
	private void SoldItemToWarehouse(ItemDisplayData item, int index, int amount)
	{
	}

	// Token: 0x0600369E RID: 13982 RVA: 0x001B7B20 File Offset: 0x001B5D20
	private void SoldItemToTreasury(ItemDisplayData item, int index, int amount)
	{
	}

	// Token: 0x0600369F RID: 13983 RVA: 0x001B7B23 File Offset: 0x001B5D23
	private void UpdateBtnConfirm(bool interactable)
	{
		this._popupWindow.ConfirmButton.interactable = interactable;
	}

	// Token: 0x060036A0 RID: 13984 RVA: 0x001B7B38 File Offset: 0x001B5D38
	private void SetVillagerNeedMark(ItemView itemView, ItemSourceType sourceType)
	{
		bool sourceTypeIsMeet = sourceType == ItemSourceType.Treasury;
		bool flag = !sourceTypeIsMeet;
		if (flag)
		{
			itemView.SetVillagerNeedMark(false, true);
		}
		else
		{
			ItemKey tempKey = ItemKey.Invalid;
			tempKey.ItemType = itemView.Data.Key.ItemType;
			tempKey.TemplateId = itemView.Data.Key.TemplateId;
			bool isNeeded = this._villagerNeededItemSet.Contains(tempKey);
			itemView.SetVillagerNeedMark(isNeeded, true);
		}
	}

	// Token: 0x060036A2 RID: 13986 RVA: 0x001B7C22 File Offset: 0x001B5E22
	[CompilerGenerated]
	internal static void <OpenProfessionSkillConfirm>g__Action|46_0()
	{
	}

	// Token: 0x04002775 RID: 10101
	private Action<List<ItemKey>> _onConfirm;

	// Token: 0x04002776 RID: 10102
	private Action _onRemove;

	// Token: 0x04002777 RID: 10103
	private bool _isCombatSkill;

	// Token: 0x04002778 RID: 10104
	private ItemScrollView _inventoryScroll;

	// Token: 0x04002779 RID: 10105
	private ItemScrollView _warehouseScroll;

	// Token: 0x0400277A RID: 10106
	private ItemScrollView _treasuryScroll;

	// Token: 0x0400277B RID: 10107
	private ItemSortAndFilter.ItemFilterType _filterType;

	// Token: 0x0400277C RID: 10108
	private PopupWindow _popupWindow;

	// Token: 0x0400277D RID: 10109
	private BuildingEarningsData _buildingEarnData;

	// Token: 0x0400277E RID: 10110
	private readonly List<ItemDisplayData> _soldItemList = new List<ItemDisplayData>();

	// Token: 0x0400277F RID: 10111
	private List<ItemDisplayData> _warehouseList;

	// Token: 0x04002780 RID: 10112
	private List<ItemDisplayData> _inventoryList;

	// Token: 0x04002781 RID: 10113
	private List<ItemDisplayData> _treasuryList;

	// Token: 0x04002782 RID: 10114
	private List<ItemDisplayData> _startSoldDatList = new List<ItemDisplayData>();

	// Token: 0x04002783 RID: 10115
	private List<ItemKey> _villagerNeededItemSet = new List<ItemKey>();

	// Token: 0x04002784 RID: 10116
	private bool _canTransfer;

	// Token: 0x04002785 RID: 10117
	private sbyte _level;

	// Token: 0x04002786 RID: 10118
	private List<int> _charIds;

	// Token: 0x04002787 RID: 10119
	private List<ItemKey> _bookItemKeys;

	// Token: 0x04002788 RID: 10120
	private CToggleGroupObsolete _togGroup;

	// Token: 0x04002789 RID: 10121
	private const int InventoryTogKey = 0;

	// Token: 0x0400278A RID: 10122
	private const int WarehouseTogKey = 1;

	// Token: 0x0400278B RID: 10123
	private const int TreasuryTogKey = 2;

	// Token: 0x0400278C RID: 10124
	private const int MaxShowCount = 10;

	// Token: 0x0400278D RID: 10125
	private const float ItemViewWidth = 114f;

	// Token: 0x0400278E RID: 10126
	private const int ItemViewPosY = 60;

	// Token: 0x0400278F RID: 10127
	private readonly List<ItemView> _soldItemViewList = new List<ItemView>();

	// Token: 0x04002790 RID: 10128
	private readonly List<ItemKey> _startSoldItemList = new List<ItemKey>();

	// Token: 0x04002791 RID: 10129
	private int _currentTogGroupKey;

	// Token: 0x04002792 RID: 10130
	private int _canSoldCount;

	// Token: 0x04002793 RID: 10131
	private GameObject _inventoryIcon;

	// Token: 0x04002794 RID: 10132
	private GameObject _warehouseIcon;

	// Token: 0x04002795 RID: 10133
	private GameObject _treasuryIcon;

	// Token: 0x04002796 RID: 10134
	private readonly List<ItemKey> _operateItemRecord = new List<ItemKey>();

	// Token: 0x04002797 RID: 10135
	private readonly List<int> _operateTypeRecord = new List<int>();

	// Token: 0x04002798 RID: 10136
	private RectTransform _countHolder;

	// Token: 0x04002799 RID: 10137
	private RectTransform _countIcon;

	// Token: 0x0400279A RID: 10138
	private readonly Vector2 _offsetValue = new Vector2(90f, -30f);

	// Token: 0x0400279B RID: 10139
	private TextMeshProUGUI _countText;

	// Token: 0x0400279C RID: 10140
	private Transform _soldItemListHolder;

	// Token: 0x0400279D RID: 10141
	private CScrollRectLegacy _soldItemHolderScrollRect;

	// Token: 0x0400279E RID: 10142
	private const string ItemViewKey = "ItemView";

	// Token: 0x0400279F RID: 10143
	private PoolItem _itemKeyPool;

	// Token: 0x020017BF RID: 6079
	public enum MultiSelectOperateType
	{
		// Token: 0x0400AC75 RID: 44149
		InventoryToSold = 1,
		// Token: 0x0400AC76 RID: 44150
		SoldToInventory,
		// Token: 0x0400AC77 RID: 44151
		WarehouseToSold,
		// Token: 0x0400AC78 RID: 44152
		SoldToWarehouse,
		// Token: 0x0400AC79 RID: 44153
		TreasuryToSold,
		// Token: 0x0400AC7A RID: 44154
		SoldToTreasury
	}
}
