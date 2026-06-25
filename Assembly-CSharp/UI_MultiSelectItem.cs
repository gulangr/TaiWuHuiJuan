using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000391 RID: 913
public class UI_MultiSelectItem : UIBase
{
	// Token: 0x170005A9 RID: 1449
	// (get) Token: 0x06003632 RID: 13874 RVA: 0x001B47A3 File Offset: 0x001B29A3
	private ItemScrollView InventoryScroll
	{
		get
		{
			return base.CGet<ItemScrollView>("InventoryItemScroll");
		}
	}

	// Token: 0x170005AA RID: 1450
	// (get) Token: 0x06003633 RID: 13875 RVA: 0x001B47B0 File Offset: 0x001B29B0
	private ItemScrollView WarehouseScroll
	{
		get
		{
			return base.CGet<ItemScrollView>("TreasuryItemScroll");
		}
	}

	// Token: 0x170005AB RID: 1451
	// (get) Token: 0x06003634 RID: 13876 RVA: 0x001B47BD File Offset: 0x001B29BD
	private ItemScrollView TreasuryScroll
	{
		get
		{
			return base.CGet<ItemScrollView>("WarehouseItemScroll");
		}
	}

	// Token: 0x170005AC RID: 1452
	// (get) Token: 0x06003635 RID: 13877 RVA: 0x001B47CA File Offset: 0x001B29CA
	private GameObject InventoryIcon
	{
		get
		{
			return base.CGet<GameObject>("InventoryIcon");
		}
	}

	// Token: 0x170005AD RID: 1453
	// (get) Token: 0x06003636 RID: 13878 RVA: 0x001B47D7 File Offset: 0x001B29D7
	private GameObject WarehouseIcon
	{
		get
		{
			return base.CGet<GameObject>("WarehouseIcon");
		}
	}

	// Token: 0x170005AE RID: 1454
	// (get) Token: 0x06003637 RID: 13879 RVA: 0x001B47E4 File Offset: 0x001B29E4
	private GameObject TreasuryIcon
	{
		get
		{
			return base.CGet<GameObject>("TreasuryIcon");
		}
	}

	// Token: 0x170005AF RID: 1455
	// (get) Token: 0x06003638 RID: 13880 RVA: 0x001B47F1 File Offset: 0x001B29F1
	private Transform SoldItemListHolder
	{
		get
		{
			return base.CGet<Transform>("SoldItemListHolder");
		}
	}

	// Token: 0x170005B0 RID: 1456
	// (get) Token: 0x06003639 RID: 13881 RVA: 0x001B4800 File Offset: 0x001B2A00
	private List<ItemDisplayData> RawList
	{
		get
		{
			int currentTogGroupKey = this._currentTogGroupKey;
			if (!true)
			{
			}
			List<ItemDisplayData> result;
			switch (currentTogGroupKey)
			{
			case 0:
				result = this._inventoryList;
				break;
			case 1:
				result = this._warehouseList;
				break;
			case 2:
				result = this._treasuryList;
				break;
			default:
				throw new ArgumentOutOfRangeException("_currentTogGroupKey", this._currentTogGroupKey, null);
			}
			if (!true)
			{
			}
			return result;
		}
	}

	// Token: 0x170005B1 RID: 1457
	// (get) Token: 0x0600363A RID: 13882 RVA: 0x001B4864 File Offset: 0x001B2A64
	private ItemScrollView Scroll
	{
		get
		{
			int currentTogGroupKey = this._currentTogGroupKey;
			if (!true)
			{
			}
			ItemScrollView result;
			switch (currentTogGroupKey)
			{
			case 0:
				result = this.InventoryScroll;
				break;
			case 1:
				result = this.WarehouseScroll;
				break;
			case 2:
				result = this.TreasuryScroll;
				break;
			default:
				throw new ArgumentOutOfRangeException("_currentTogGroupKey", this._currentTogGroupKey, null);
			}
			if (!true)
			{
			}
			return result;
		}
	}

	// Token: 0x170005B2 RID: 1458
	// (get) Token: 0x0600363B RID: 13883 RVA: 0x001B48C7 File Offset: 0x001B2AC7
	private bool IsBookCollectionRoom
	{
		get
		{
			return this._buildingTemplateId == 105;
		}
	}

	// Token: 0x0600363C RID: 13884 RVA: 0x001B48D4 File Offset: 0x001B2AD4
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get("BuildingTemplateId", out this._buildingTemplateId);
		argsBox.Get<BuildingBlockKey>("buildingBlockKey", out this._buildingBlockKey);
		argsBox.Get<List<ItemDisplayData>>("warehouseList", out this._warehouseList);
		argsBox.Get<List<ItemDisplayData>>("inventoryList", out this._inventoryList);
		argsBox.Get<List<ItemDisplayData>>("treasuryList", out this._treasuryList);
		argsBox.Get("canTransfer", out this._canUseInventory);
		argsBox.Get<Action>("callback", out this._onConfirm);
		string title;
		bool flag = !argsBox.Get("title", out title);
		if (flag)
		{
			title = LocalStringManager.Get(LanguageKey.LK_UI_SelectItem);
		}
		PopupWindow popUpWindow = base.CGet<PopupWindow>("PopupWindowBase");
		popUpWindow.SetTitle(title);
		popUpWindow.CancelButton.interactable = true;
		popUpWindow.ConfirmButton.interactable = true;
		popUpWindow.OnConfirmClick = new Action(this.OnConfirmSelect);
		popUpWindow.OnCancelClick = new Action(this.OnCancel);
		this._canSoldCount = (int)GameData.Domains.Building.SharedMethods.GetBuildingSlotCount(this._buildingTemplateId);
		this.WarehouseScroll.Init();
		this.InventoryScroll.Init();
		this.TreasuryScroll.Init();
		bool isFeast = this.IsFeast;
		if (isFeast)
		{
			this.FeastOnInit(argsBox);
		}
		else
		{
			argsBox.Get<BuildingEarningsData>("buildingEarningsData", out this._buildingEarnData);
			this.RefreshScrollView(argsBox);
			this._operateItemRecord.Clear();
			this._operateTypeRecord.Clear();
			this._startSoldItemList.Clear();
			this._soldItemViewList.Clear();
			bool flag2 = this._buildingEarnData != null;
			if (flag2)
			{
				bool isBookCollectionRoom = this.IsBookCollectionRoom;
				if (isBookCollectionRoom)
				{
					this._startSoldItemList.AddRange(this._buildingEarnData.FixBookInfoList);
				}
				else
				{
					for (int i = 0; i < this._buildingEarnData.ShopSoldItemList.Count; i++)
					{
						bool flag3 = this._buildingEarnData.ShopSoldItemList[i].TemplateId != -1;
						if (flag3)
						{
							this._startSoldItemList.Add(this._buildingEarnData.ShopSoldItemList[i]);
						}
					}
					for (int j = 0; j < this._buildingEarnData.ShopSoldItemEarnList.Count; j++)
					{
						bool flag4 = this._buildingEarnData.ShopSoldItemEarnList[j].First != -1;
						if (flag4)
						{
							this._canSoldCount--;
						}
					}
				}
			}
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		}
	}

	// Token: 0x0600363D RID: 13885 RVA: 0x001B4B94 File Offset: 0x001B2D94
	private void Awake()
	{
		CToggleGroupObsolete togGroup = base.CGet<CToggleGroupObsolete>("PanelToggleGroup");
		togGroup.InitPreOnToggle(-1);
		togGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnToggleChange);
		this._dishPool = new PoolItem("ItemView", base.CGet<GameObject>("DishPrefab"));
		this._itemKeyPool = new PoolItem("ItemView", base.CGet<GameObject>("ItemViewPrefab"));
	}

	// Token: 0x0600363E RID: 13886 RVA: 0x001B4C00 File Offset: 0x001B2E00
	private void OnEnable()
	{
		ConchShipCursor.Instance.SetSelectCountActive(true);
		CToggleGroupObsolete togGroup = base.CGet<CToggleGroupObsolete>("PanelToggleGroup");
		bool canUseInventory = this._canUseInventory;
		if (canUseInventory)
		{
			togGroup.Get(0).interactable = true;
			togGroup.Set(0, true, true);
			this._currentTogGroupKey = 0;
		}
		else
		{
			togGroup.Get(0).interactable = false;
			togGroup.Set(1, true, true);
			this._currentTogGroupKey = 1;
		}
		bool isFeast = this.IsFeast;
		if (isFeast)
		{
			this.FeastOnEnable();
		}
	}

	// Token: 0x0600363F RID: 13887 RVA: 0x001B4C88 File Offset: 0x001B2E88
	private void OnDisable()
	{
		this.WarehouseScroll.SaveSortFilterSetting(true);
		this.InventoryScroll.SaveSortFilterSetting(true);
		this.TreasuryScroll.SaveSortFilterSetting(true);
		bool isFeast = this.IsFeast;
		if (isFeast)
		{
			this.FeastOnDisable();
		}
		else
		{
			foreach (ItemView item in this._soldItemViewList)
			{
				this._itemKeyPool.DestroyObject(item.gameObject);
			}
		}
	}

	// Token: 0x06003640 RID: 13888 RVA: 0x001B4D28 File Offset: 0x001B2F28
	private void OnDestroy()
	{
		PoolItem dishPool = this._dishPool;
		if (dishPool != null)
		{
			dishPool.Destroy();
		}
		this._dishPool = null;
		PoolItem itemKeyPool = this._itemKeyPool;
		if (itemKeyPool != null)
		{
			itemKeyPool.Destroy();
		}
		this._itemKeyPool = null;
	}

	// Token: 0x06003641 RID: 13889 RVA: 0x001B4D60 File Offset: 0x001B2F60
	private void OnConfirmSelect()
	{
		ConchShipCursor.Instance.SetSelectCountActive(false);
		bool isFeast = this.IsFeast;
		if (isFeast)
		{
			this.OnFeastConfirm();
		}
		else
		{
			bool flag = this._operateItemRecord.Count == 0;
			if (flag)
			{
				this.HideUI();
			}
			else
			{
				this.HideUI();
				BuildingDomainMethod.Call.ShopBuildingMultiChangeSoldItem(this._buildingBlockKey, this._operateItemRecord, this._operateTypeRecord);
				Action onConfirm = this._onConfirm;
				if (onConfirm != null)
				{
					onConfirm();
				}
			}
		}
	}

	// Token: 0x06003642 RID: 13890 RVA: 0x001B4DDC File Offset: 0x001B2FDC
	private void OnCancel()
	{
		ConchShipCursor.Instance.SetSelectCountActive(false);
		bool isFeast = this.IsFeast;
		if (isFeast)
		{
			this.OnFeastQuit();
		}
		else
		{
			this.HideUI();
			bool flag = this._operateItemRecord.Count != 0;
			if (flag)
			{
				Action onConfirm = this._onConfirm;
				if (onConfirm != null)
				{
					onConfirm();
				}
			}
		}
	}

	// Token: 0x06003643 RID: 13891 RVA: 0x001B4E38 File Offset: 0x001B3038
	private void OnToggleChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		bool flag = newTog == null;
		if (!flag)
		{
			this._currentTogGroupKey = newTog.Key;
			this.InventoryScroll.gameObject.SetActive(this._currentTogGroupKey == 0);
			this.WarehouseScroll.gameObject.SetActive(this._currentTogGroupKey == 1);
			this.TreasuryScroll.gameObject.SetActive(this._currentTogGroupKey == 2);
			this.InventoryIcon.SetActive(this._currentTogGroupKey == 0);
			this.WarehouseIcon.SetActive(this._currentTogGroupKey == 1);
			this.TreasuryIcon.SetActive(this._currentTogGroupKey == 2);
			bool isFeast = this.IsFeast;
			if (isFeast)
			{
				this.SetFoodList();
			}
			else
			{
				switch (newTog.Key)
				{
				case 0:
					this.SetInventoryItemList();
					break;
				case 1:
					this.SetWarehouseItemList();
					break;
				case 2:
					this.SetTreasureItemList();
					break;
				}
			}
		}
	}

	// Token: 0x06003644 RID: 13892 RVA: 0x001B4F3B File Offset: 0x001B313B
	private void HideUI()
	{
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x06003645 RID: 13893 RVA: 0x001B4F64 File Offset: 0x001B3164
	private void RefreshScrollView(ArgumentBox argsBox)
	{
		this.SetInventoryItemList();
		this.SetWarehouseItemList();
		this.SetTreasureItemList();
		bool isBookCollectionRoom = this.IsBookCollectionRoom;
		if (isBookCollectionRoom)
		{
			this._itemFilterType = ItemSortAndFilter.ItemFilterType.Book;
		}
		else
		{
			ItemSortAndFilter.ItemFilterType filterType;
			this._itemFilterType = (argsBox.Get<ItemSortAndFilter.ItemFilterType>("filterType", out filterType) ? filterType : ItemSortAndFilter.ItemFilterType.Invalid);
		}
		this.InventoryScroll.SortAndFilter.SwitchFilterGroupActive("Filter");
		this.WarehouseScroll.SortAndFilter.SwitchFilterGroupActive("Filter");
		this.TreasuryScroll.SortAndFilter.SwitchFilterGroupActive("Filter");
		this.InventoryScroll.SortAndFilter.LockFilterType(this._itemFilterType, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
		this.InventoryScroll.SortAndFilter.ShowFilterType(this._itemFilterType);
		this.WarehouseScroll.SortAndFilter.LockFilterType(this._itemFilterType, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
		this.WarehouseScroll.SortAndFilter.ShowFilterType(this._itemFilterType);
		this.TreasuryScroll.SortAndFilter.LockFilterType(this._itemFilterType, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
		this.TreasuryScroll.SortAndFilter.ShowFilterType(this._itemFilterType);
	}

	// Token: 0x06003646 RID: 13894 RVA: 0x001B5082 File Offset: 0x001B3282
	private void SetInventoryItemList()
	{
		this.InventoryScroll.SetItemList(ref this._inventoryList, true, "MultiSelectItem_Treasure_Other", this.InventoryScroll.SortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderInventoryItem));
	}

	// Token: 0x06003647 RID: 13895 RVA: 0x001B50B9 File Offset: 0x001B32B9
	private void SetWarehouseItemList()
	{
		this.WarehouseScroll.SetItemList(ref this._warehouseList, true, "MultiSelectItem_Treasure_Other", this.WarehouseScroll.SortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderWarehouseItem));
	}

	// Token: 0x06003648 RID: 13896 RVA: 0x001B50F0 File Offset: 0x001B32F0
	private void SetTreasureItemList()
	{
		this.TreasuryScroll.SetItemList(ref this._treasuryList, true, "MultiSelectItem_Treasure_Other", this.TreasuryScroll.SortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderTreasuryItem));
	}

	// Token: 0x06003649 RID: 13897 RVA: 0x001B5127 File Offset: 0x001B3327
	private void OnListenerIdReady()
	{
		ItemDomainMethod.AsyncCall.GetItemDisplayDataList(this, this._startSoldItemList, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._startSoldDatList);
			this.UpdateSoldItemList();
			bool canUseInventory = this._canUseInventory;
			if (canUseInventory)
			{
				this.SetInventoryItemList();
			}
			else
			{
				this.SetWarehouseItemList();
			}
			this.FreshSoldItemAll();
		});
	}

	// Token: 0x0600364A RID: 13898 RVA: 0x001B5150 File Offset: 0x001B3350
	private void UpdateSoldItemList()
	{
		this._soldItemList.Clear();
		bool flag = this._buildingEarnData == null;
		if (flag)
		{
			for (int i = 0; i < this._canSoldCount; i++)
			{
				this._soldItemList.Add(null);
			}
		}
		else
		{
			this.<UpdateSoldItemList>g__Handle|64_0(this.IsBookCollectionRoom ? this._buildingEarnData.FixBookInfoList : this._buildingEarnData.ShopSoldItemList);
		}
	}

	// Token: 0x0600364B RID: 13899 RVA: 0x001B51CC File Offset: 0x001B33CC
	private ItemView GetItemView()
	{
		GameObject go = this._itemKeyPool.GetObject();
		go.transform.SetParent(this.SoldItemListHolder, false);
		go.SetActive(true);
		ItemView itemView = go.GetComponent<ItemView>();
		this._soldItemViewList.Add(itemView);
		return itemView;
	}

	// Token: 0x0600364C RID: 13900 RVA: 0x001B521C File Offset: 0x001B341C
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

	// Token: 0x0600364D RID: 13901 RVA: 0x001B5264 File Offset: 0x001B3464
	private void FreshSoldItemAll()
	{
		int curCount = this.GetSoldItemCount();
		base.CGet<CScrollRectLegacy>("SoldItemScroll").SetScrollEnable(curCount > 10);
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
		ConchShipCursor.Instance.SetSelectCount(this.GetSoldItemCount(), this._canSoldCount);
		this.FreshInteractable(curCount < this._canSoldCount);
	}

	// Token: 0x0600364E RID: 13902 RVA: 0x001B5380 File Offset: 0x001B3580
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

	// Token: 0x0600364F RID: 13903 RVA: 0x001B53CC File Offset: 0x001B35CC
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
	}

	// Token: 0x06003650 RID: 13904 RVA: 0x001B54B0 File Offset: 0x001B36B0
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

	// Token: 0x06003651 RID: 13905 RVA: 0x001B5500 File Offset: 0x001B3700
	private void Update()
	{
		bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
		if (flag)
		{
			this.OnConfirmSelect();
		}
	}

	// Token: 0x06003652 RID: 13906 RVA: 0x001B5530 File Offset: 0x001B3730
	public override void QuickHide()
	{
		this.OnCancel();
	}

	// Token: 0x06003653 RID: 13907 RVA: 0x001B553C File Offset: 0x001B373C
	private void OnRenderWarehouseItem(ItemDisplayData itemData, ItemView itemView)
	{
		itemView.UserString = "warehouse";
		itemView.SetClickEvent(delegate
		{
			this.WarehouseItemToSold(itemView.Data, 1);
		});
		this.SetLockText(itemData, itemView);
	}

	// Token: 0x06003654 RID: 13908 RVA: 0x001B5594 File Offset: 0x001B3794
	private void OnRenderInventoryItem(ItemDisplayData itemData, ItemView itemView)
	{
		itemView.UserString = "inventory";
		itemView.SetClickEvent(delegate
		{
			this.InventoryItemToSold(itemView.Data, 1);
		});
		this.SetLockText(itemData, itemView);
	}

	// Token: 0x06003655 RID: 13909 RVA: 0x001B55EC File Offset: 0x001B37EC
	private void OnRenderTreasuryItem(ItemDisplayData itemData, ItemView itemView)
	{
		itemView.UserString = "treasury";
		itemView.SetClickEvent(delegate
		{
			this.TreasuryItemToSold(itemView.Data, 1);
		});
		this.SetLockText(itemData, itemView);
	}

	// Token: 0x06003656 RID: 13910 RVA: 0x001B5644 File Offset: 0x001B3844
	private void SetLockText(ItemDisplayData itemData, ItemView itemView)
	{
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

	// Token: 0x06003657 RID: 13911 RVA: 0x001B5698 File Offset: 0x001B3898
	private void WarehouseItemToSold(ItemDisplayData item, int amount)
	{
	}

	// Token: 0x06003658 RID: 13912 RVA: 0x001B569C File Offset: 0x001B389C
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

	// Token: 0x06003659 RID: 13913 RVA: 0x001B56F1 File Offset: 0x001B38F1
	private void InventoryItemToSold(ItemDisplayData item, int amount)
	{
	}

	// Token: 0x0600365A RID: 13914 RVA: 0x001B56F4 File Offset: 0x001B38F4
	private void TreasuryItemToSold(ItemDisplayData item, int amount)
	{
	}

	// Token: 0x0600365B RID: 13915 RVA: 0x001B56F7 File Offset: 0x001B38F7
	private void SoldItemToInventory(ItemDisplayData item, int index, int amount)
	{
	}

	// Token: 0x0600365C RID: 13916 RVA: 0x001B56FA File Offset: 0x001B38FA
	private void SoldItemToWarehouse(ItemDisplayData item, int index, int amount)
	{
	}

	// Token: 0x0600365D RID: 13917 RVA: 0x001B56FD File Offset: 0x001B38FD
	private void SoldItemToTreasury(ItemDisplayData item, int index, int amount)
	{
	}

	// Token: 0x170005B3 RID: 1459
	// (get) Token: 0x0600365E RID: 13918 RVA: 0x001B5700 File Offset: 0x001B3900
	private TMP_InputField SearchByName
	{
		get
		{
			return base.CGet<TMP_InputField>("SearchByName");
		}
	}

	// Token: 0x170005B4 RID: 1460
	// (get) Token: 0x0600365F RID: 13919 RVA: 0x001B570D File Offset: 0x001B390D
	private bool IsFeast
	{
		get
		{
			return this._buildingTemplateId == 47;
		}
	}

	// Token: 0x170005B5 RID: 1461
	// (get) Token: 0x06003660 RID: 13920 RVA: 0x001B5719 File Offset: 0x001B3919
	// (set) Token: 0x06003661 RID: 13921 RVA: 0x001B5721 File Offset: 0x001B3921
	public int SelectedDishIndex
	{
		get
		{
			return this._selectedDishIndex;
		}
		set
		{
			this._selectedDishIndex = value;
			ConchShipCursor.Instance.SetSelectCountDescOn(value >= 0);
		}
	}

	// Token: 0x06003662 RID: 13922 RVA: 0x001B5740 File Offset: 0x001B3940
	private void OnRenderFood(ItemDisplayData itemData, ItemView itemView)
	{
		itemView.SetClickEvent(delegate
		{
			this.OnClickFood(itemData);
		});
		this.SetLockText(itemData, itemView);
	}

	// Token: 0x06003663 RID: 13923 RVA: 0x001B5784 File Offset: 0x001B3984
	private void OnRenderDish(int index, bool isSelected)
	{
		UI_MultiSelectItem.FeastDishDisplayData data = this._dishes[index];
		this.DestroyDish(data);
		bool flag = data.CurrentDish == null;
		if (!flag)
		{
			GameObject obj = this._dishPool.GetObject();
			Refers refers = obj.GetComponent<Refers>();
			ItemView itemView = refers.CGet<ItemView>("ItemView");
			string durability = ((data.CurrentDish.Durability > 0) ? ((int)data.CurrentDish.Durability) : GlobalConfig.Instance.FeastDurability).ToString();
			string maxDurability = GlobalConfig.Instance.FeastDurability.ToString();
			TextMeshProUGUI durabilityText = refers.CGet<TextMeshProUGUI>("DurabilityText");
			ItemKey itemKey = data.CurrentDish.RealKey;
			string dishName = ItemTemplateHelper.GetName(itemKey.ItemType, itemKey.TemplateId).SetGradeColor((int)ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId));
			bool flag2 = this.IsDishEaten(data.CurrentDish);
			if (flag2)
			{
				durability.SetGradeColor(7);
			}
			itemView.SetData(data.CurrentDish, false, 1, false, true, null, false, true);
			itemView.SetSelectState(true);
			itemView.SetClickEvent(delegate
			{
				this.OnClickLaunchedDish(index);
			});
			if (isSelected)
			{
				durabilityText.transform.parent.gameObject.SetActive(false);
				obj.GetComponent<LayoutElement>().minWidth = 308f;
				obj.GetComponent<HorizontalLayoutGroup>().enabled = true;
				refers.CGet<GameObject>("Highlight").SetActive(true);
				refers.CGet<GameObject>("Normal").SetActive(false);
				refers.CGet<TextMeshProUGUI>("DurabilityDesc").text = LanguageKey.LK_Building_Entertain_SelectedDish_DurabilityDesc.TrFormat(durability, maxDurability);
				refers.CGet<TextMeshProUGUI>("NameDesc").text = LanguageKey.LK_Building_Entertain_SelectedDish_NameDesc.TrFormat(dishName);
			}
			else
			{
				durabilityText.text = durability + "/" + maxDurability;
				durabilityText.transform.parent.gameObject.SetActive(true);
				obj.GetComponent<LayoutElement>().minWidth = 114f;
				obj.GetComponent<HorizontalLayoutGroup>().enabled = false;
				refers.CGet<GameObject>("Highlight").SetActive(false);
				refers.CGet<GameObject>("Normal").SetActive(true);
				refers.CGet<TextMeshProUGUI>("DurabilityDesc").text = "";
				refers.CGet<TextMeshProUGUI>("NameDesc").text = "";
			}
			obj.transform.SetParent(this.SoldItemListHolder, false);
			obj.SetActive(true);
			data.DisplayObject = obj;
		}
	}

	// Token: 0x06003664 RID: 13924 RVA: 0x001B5A30 File Offset: 0x001B3C30
	private void OnFeastConfirm()
	{
		bool flag = this.ContainEatenDish();
		if (flag)
		{
			DialogCmd dialogCmd = new DialogCmd();
			dialogCmd.Title = LanguageKey.LK_Building_Entertain_Warn_RemoveDish_Title.Tr();
			dialogCmd.Content = LanguageKey.LK_Building_Entertain_Warn_RemoveDish_Content.Tr();
			dialogCmd.Type = 1;
			dialogCmd.Yes = new Action(this.ConfirmDishes);
			dialogCmd.No = delegate()
			{
				ConchShipCursor.Instance.SetSelectCountActive(true);
			};
			DialogCmd cmd = dialogCmd;
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}
		else
		{
			this.ConfirmDishes();
		}
	}

	// Token: 0x06003665 RID: 13925 RVA: 0x001B5AE7 File Offset: 0x001B3CE7
	private void OnFeastQuit()
	{
		this.HideUI();
		Action onConfirm = this._onConfirm;
		if (onConfirm != null)
		{
			onConfirm();
		}
	}

	// Token: 0x06003666 RID: 13926 RVA: 0x001B5B04 File Offset: 0x001B3D04
	private void OnClickLaunchedDish(int index)
	{
		this.SelectedDishIndex = -1;
		ItemDisplayData displayData = this._dishes[index].CurrentDish;
		int inventoryIndex = this.IsDishEaten(displayData) ? -1 : this.RawList.FindIndex((ItemDisplayData data) => data.CanMerge(displayData));
		bool flag = inventoryIndex >= 0;
		if (flag)
		{
			this.RawList[inventoryIndex].ChangeAmount(displayData.GetOperationInventoryFromPool(1, false), true);
		}
		else
		{
			this.RawList.Add(this._dishes[index].CurrentDish);
		}
		this._dishes[index].CurrentDish = null;
		this.SetDishList();
		this.SetFoodList();
		this.UpdateInitialDishLocation(displayData);
	}

	// Token: 0x06003667 RID: 13927 RVA: 0x001B5BD7 File Offset: 0x001B3DD7
	private void OnClickFood(ItemDisplayData displayData)
	{
	}

	// Token: 0x06003668 RID: 13928 RVA: 0x001B5BDA File Offset: 0x001B3DDA
	private void OnInputFieldChanged(string text)
	{
		this._searchInputText = text;
		this.SetFoodList();
	}

	// Token: 0x06003669 RID: 13929 RVA: 0x001B5BEC File Offset: 0x001B3DEC
	private void FeastOnInit(ArgumentBox argsBox)
	{
		argsBox.Get<Feast>("feast", out this._feast);
		argsBox.Get("currentIndex", out this._selectedDishIndex);
		List<ItemDisplayData> dishList;
		argsBox.Get<List<ItemDisplayData>>("dishList", out dishList);
		this.SelectedDishIndex = this._selectedDishIndex;
		for (int i = 0; i < GlobalConfig.Instance.FeastCount; i++)
		{
			bool flag = this._feast.GetDish(i).IsValid();
			if (flag)
			{
				dishList[i].Durability = (short)this._feast.DishDurability[i];
				this._dishes[i] = new UI_MultiSelectItem.FeastDishDisplayData
				{
					InitialDish = dishList[i],
					CurrentDish = dishList[i]
				};
			}
			else
			{
				this._dishes[i] = new UI_MultiSelectItem.FeastDishDisplayData();
			}
		}
	}

	// Token: 0x0600366A RID: 13930 RVA: 0x001B5CD4 File Offset: 0x001B3ED4
	private void FeastOnEnable()
	{
		this.InventoryScroll.SortAndFilter.SwitchFilterGroupActive("EntertainTypeFilter");
		this.WarehouseScroll.SortAndFilter.SwitchFilterGroupActive("EntertainTypeFilter");
		this.TreasuryScroll.SortAndFilter.SwitchFilterGroupActive("EntertainTypeFilter");
		this.SearchByName.gameObject.SetActive(this.IsFeast);
		this.SearchByName.onValueChanged.RemoveListener(new UnityAction<string>(this.OnInputFieldChanged));
		this.SearchByName.onValueChanged.AddListener(new UnityAction<string>(this.OnInputFieldChanged));
		bool flag = this.SelectedDishIndex >= 0 && this._dishes[this.SelectedDishIndex].InitialDish == null;
		if (flag)
		{
			this.SelectedDishIndex = -1;
		}
		this.SetDishList();
		this.SetFoodList();
	}

	// Token: 0x0600366B RID: 13931 RVA: 0x001B5DB4 File Offset: 0x001B3FB4
	private void FeastOnDisable()
	{
		foreach (KeyValuePair<int, UI_MultiSelectItem.FeastDishDisplayData> keyValuePair in this._dishes)
		{
			int num;
			UI_MultiSelectItem.FeastDishDisplayData feastDishDisplayData;
			keyValuePair.Deconstruct(out num, out feastDishDisplayData);
			UI_MultiSelectItem.FeastDishDisplayData data = feastDishDisplayData;
			this.DestroyDish(data);
		}
	}

	// Token: 0x0600366C RID: 13932 RVA: 0x001B5E20 File Offset: 0x001B4020
	private void SetDishList()
	{
		bool flag = this.SelectedDishIndex >= 0;
		if (flag)
		{
			this.OnRenderDish(this.SelectedDishIndex, true);
		}
		foreach (KeyValuePair<int, UI_MultiSelectItem.FeastDishDisplayData> keyValuePair in this._dishes)
		{
			int num;
			UI_MultiSelectItem.FeastDishDisplayData feastDishDisplayData;
			keyValuePair.Deconstruct(out num, out feastDishDisplayData);
			int index = num;
			bool flag2 = index != this.SelectedDishIndex;
			if (flag2)
			{
				this.OnRenderDish(index, false);
			}
		}
		ConchShipCursor.Instance.SetSelectCount(this.GetDishCount(), this._canSoldCount);
	}

	// Token: 0x0600366D RID: 13933 RVA: 0x001B5ED4 File Offset: 0x001B40D4
	private void SetFoodList()
	{
		this._filteredList.Clear();
		bool flag = this._searchInputText.IsNullOrEmpty();
		if (flag)
		{
			this._filteredList.AddRange(this.RawList);
		}
		else
		{
			foreach (ItemDisplayData item in this.RawList)
			{
				bool flag2 = ItemTemplateHelper.GetName(item.RealKey.ItemType, item.RealKey.TemplateId).Contains(this._searchInputText);
				if (flag2)
				{
					this._filteredList.Add(item);
				}
			}
		}
		this.Scroll.SetItemList(ref this._filteredList, true, "MultiSelectItem_Treasure_Entertain", this.Scroll.SortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderFood));
		this.FreshInteractable(this.IsFoodInteractable());
	}

	// Token: 0x0600366E RID: 13934 RVA: 0x001B5FCC File Offset: 0x001B41CC
	private void ConfirmDishes()
	{
		foreach (KeyValuePair<int, UI_MultiSelectItem.FeastDishDisplayData> keyValuePair in this._dishes)
		{
			int num;
			UI_MultiSelectItem.FeastDishDisplayData feastDishDisplayData;
			keyValuePair.Deconstruct(out num, out feastDishDisplayData);
			int index = num;
			UI_MultiSelectItem.FeastDishDisplayData data = feastDishDisplayData;
			bool flag = data.IsInitialDishRemoved();
			if (flag)
			{
				ExtraDomainMethod.Call.FeastRemoveDish(this._buildingBlockKey, index, UI_MultiSelectItem.TogKey2SourceType[data.InitialDishLocation]);
			}
		}
		foreach (KeyValuePair<int, UI_MultiSelectItem.FeastDishDisplayData> keyValuePair in this._dishes)
		{
			int num;
			UI_MultiSelectItem.FeastDishDisplayData feastDishDisplayData;
			keyValuePair.Deconstruct(out num, out feastDishDisplayData);
			int index2 = num;
			UI_MultiSelectItem.FeastDishDisplayData data2 = feastDishDisplayData;
			bool flag2 = data2.IsCurrentDishNeedAdd();
			if (flag2)
			{
				ExtraDomainMethod.Call.FeastAddDish(this._buildingBlockKey, index2, data2.CurrentDish.RealKey, data2.CurrentDish.ItemSourceTypeEnum);
			}
		}
		ExtraDomainMethod.AsyncCall.IsFeastException(this, this._buildingBlockKey, delegate(int offset, RawDataPool dataPool)
		{
			bool needShow = false;
			Serializer.Deserialize(dataPool, offset, ref needShow);
			ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
			argsBox.SetObject("BuildingBlockKey", this._buildingBlockKey);
			argsBox.Set("ComfortableHouseEntertainException", needShow);
			GEvent.OnEvent(UiEvents.RefreshExceptionInfo, argsBox);
		});
		this.OnFeastQuit();
	}

	// Token: 0x0600366F RID: 13935 RVA: 0x001B60FC File Offset: 0x001B42FC
	private void UpdateInitialDishLocation(ItemDisplayData data)
	{
		foreach (UI_MultiSelectItem.FeastDishDisplayData dish in this._dishes.Values)
		{
			bool flag = data.Equals(dish.InitialDish);
			if (flag)
			{
				foreach (int index in this._dishes.Keys)
				{
					bool flag2 = data.Equals(this._dishes[index].CurrentDish);
					if (flag2)
					{
						dish.InitialDishLocation = -index - 1;
						return;
					}
				}
				dish.InitialDishLocation = this._currentTogGroupKey;
				break;
			}
		}
	}

	// Token: 0x06003670 RID: 13936 RVA: 0x001B61E8 File Offset: 0x001B43E8
	private void DestroyDish(UI_MultiSelectItem.FeastDishDisplayData data)
	{
		bool flag = data.DisplayObject != null;
		if (flag)
		{
			this._dishPool.DestroyObject(data.DisplayObject);
			data.DisplayObject = null;
		}
	}

	// Token: 0x06003671 RID: 13937 RVA: 0x001B6224 File Offset: 0x001B4424
	private int GetAvailableDishSlot()
	{
		foreach (KeyValuePair<int, UI_MultiSelectItem.FeastDishDisplayData> keyValuePair in this._dishes)
		{
			int num;
			UI_MultiSelectItem.FeastDishDisplayData feastDishDisplayData;
			keyValuePair.Deconstruct(out num, out feastDishDisplayData);
			int index = num;
			UI_MultiSelectItem.FeastDishDisplayData data = feastDishDisplayData;
			bool flag = data.CurrentDish == null;
			if (flag)
			{
				return index;
			}
		}
		return -1;
	}

	// Token: 0x06003672 RID: 13938 RVA: 0x001B62A0 File Offset: 0x001B44A0
	private int GetDishCount()
	{
		int res = 0;
		foreach (KeyValuePair<int, UI_MultiSelectItem.FeastDishDisplayData> keyValuePair in this._dishes)
		{
			int num;
			UI_MultiSelectItem.FeastDishDisplayData feastDishDisplayData;
			keyValuePair.Deconstruct(out num, out feastDishDisplayData);
			UI_MultiSelectItem.FeastDishDisplayData data = feastDishDisplayData;
			bool flag = data.CurrentDish != null;
			if (flag)
			{
				res++;
			}
		}
		return res;
	}

	// Token: 0x06003673 RID: 13939 RVA: 0x001B6320 File Offset: 0x001B4520
	private bool ContainEatenDish()
	{
		foreach (KeyValuePair<int, UI_MultiSelectItem.FeastDishDisplayData> keyValuePair in this._dishes)
		{
			int num;
			UI_MultiSelectItem.FeastDishDisplayData feastDishDisplayData;
			keyValuePair.Deconstruct(out num, out feastDishDisplayData);
			int index = num;
			UI_MultiSelectItem.FeastDishDisplayData data = feastDishDisplayData;
			bool flag = data.IsInitialDishRemoved() && this.IsDishEaten(this._dishes[index].InitialDish);
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003674 RID: 13940 RVA: 0x001B63B8 File Offset: 0x001B45B8
	private bool IsDishEaten(ItemDisplayData displayData)
	{
		return displayData.Durability > 0 && (int)displayData.Durability != GlobalConfig.Instance.FeastDurability;
	}

	// Token: 0x06003675 RID: 13941 RVA: 0x001B63EC File Offset: 0x001B45EC
	private bool IsFoodInteractable()
	{
		return this._selectedDishIndex >= 0 || this.GetDishCount() < this._canSoldCount;
	}

	// Token: 0x06003676 RID: 13942 RVA: 0x001B6418 File Offset: 0x001B4618
	private bool IsInitialDish(ItemDisplayData displayData)
	{
		foreach (UI_MultiSelectItem.FeastDishDisplayData dish in this._dishes.Values)
		{
			bool flag = displayData.Equals(dish.InitialDish);
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600367A RID: 13946 RVA: 0x001B6560 File Offset: 0x001B4760
	[CompilerGenerated]
	private void <UpdateSoldItemList>g__Handle|64_0(List<ItemKey> items)
	{
		for (int i = 0; i < items.Count; i++)
		{
			bool flag = items[i].Equals(ItemKey.Invalid);
			if (flag)
			{
				this._soldItemList.Add(null);
			}
			else
			{
				ItemDisplayData itemDisplayData = this.GetItemDisplayData(this._startSoldDatList, items[i]);
				this._soldItemList.Add(itemDisplayData);
			}
		}
		bool flag2 = this._canSoldCount > items.Count;
		if (flag2)
		{
			for (int j = 0; j < this._canSoldCount - items.Count; j++)
			{
				this._soldItemList.Add(null);
			}
		}
	}

	// Token: 0x0400274D RID: 10061
	private const int InventoryTogKey = 0;

	// Token: 0x0400274E RID: 10062
	private const int WarehouseTogKey = 1;

	// Token: 0x0400274F RID: 10063
	private const int TreasuryTogKey = 2;

	// Token: 0x04002750 RID: 10064
	private Action _onConfirm;

	// Token: 0x04002751 RID: 10065
	private Action _onRemove;

	// Token: 0x04002752 RID: 10066
	private int _canSoldCount;

	// Token: 0x04002753 RID: 10067
	private int _currentTogGroupKey;

	// Token: 0x04002754 RID: 10068
	private short _buildingTemplateId;

	// Token: 0x04002755 RID: 10069
	private BuildingBlockKey _buildingBlockKey;

	// Token: 0x04002756 RID: 10070
	private bool _canUseInventory;

	// Token: 0x04002757 RID: 10071
	private List<ItemDisplayData> _warehouseList;

	// Token: 0x04002758 RID: 10072
	private List<ItemDisplayData> _inventoryList;

	// Token: 0x04002759 RID: 10073
	private List<ItemDisplayData> _treasuryList;

	// Token: 0x0400275A RID: 10074
	private ItemSortAndFilter.ItemFilterType _itemFilterType;

	// Token: 0x0400275B RID: 10075
	private BuildingEarningsData _buildingEarnData;

	// Token: 0x0400275C RID: 10076
	private readonly List<ItemDisplayData> _soldItemList = new List<ItemDisplayData>();

	// Token: 0x0400275D RID: 10077
	private List<ItemDisplayData> _startSoldDatList = new List<ItemDisplayData>();

	// Token: 0x0400275E RID: 10078
	private const int MaxShowCount = 10;

	// Token: 0x0400275F RID: 10079
	private readonly List<ItemView> _soldItemViewList = new List<ItemView>();

	// Token: 0x04002760 RID: 10080
	private readonly List<ItemKey> _startSoldItemList = new List<ItemKey>();

	// Token: 0x04002761 RID: 10081
	private readonly List<ItemKey> _operateItemRecord = new List<ItemKey>();

	// Token: 0x04002762 RID: 10082
	private readonly List<int> _operateTypeRecord = new List<int>();

	// Token: 0x04002763 RID: 10083
	private RectTransform _countHolder;

	// Token: 0x04002764 RID: 10084
	private RectTransform _countIcon;

	// Token: 0x04002765 RID: 10085
	private TextMeshProUGUI _countText;

	// Token: 0x04002766 RID: 10086
	private const string TagName = "MultiSelectItem_Treasure_Other";

	// Token: 0x04002767 RID: 10087
	private const string FilterName = "Filter";

	// Token: 0x04002768 RID: 10088
	private const string ItemViewKey = "ItemView";

	// Token: 0x04002769 RID: 10089
	private PoolItem _itemKeyPool;

	// Token: 0x0400276A RID: 10090
	private const string FeastSortingTagName = "MultiSelectItem_Treasure_Entertain";

	// Token: 0x0400276B RID: 10091
	private const string FeastFilterName = "EntertainTypeFilter";

	// Token: 0x0400276C RID: 10092
	private const int SelectedDishWidth = 308;

	// Token: 0x0400276D RID: 10093
	private const int NormalDishWidth = 114;

	// Token: 0x0400276E RID: 10094
	private static readonly Dictionary<int, ItemSourceType> TogKey2SourceType = new Dictionary<int, ItemSourceType>
	{
		{
			0,
			ItemSourceType.Inventory
		},
		{
			1,
			ItemSourceType.Warehouse
		},
		{
			2,
			ItemSourceType.Treasury
		}
	};

	// Token: 0x0400276F RID: 10095
	private string _searchInputText;

	// Token: 0x04002770 RID: 10096
	private PoolItem _dishPool;

	// Token: 0x04002771 RID: 10097
	private Feast _feast;

	// Token: 0x04002772 RID: 10098
	private Dictionary<int, UI_MultiSelectItem.FeastDishDisplayData> _dishes = new Dictionary<int, UI_MultiSelectItem.FeastDishDisplayData>();

	// Token: 0x04002773 RID: 10099
	private List<ItemDisplayData> _filteredList = new List<ItemDisplayData>();

	// Token: 0x04002774 RID: 10100
	private int _selectedDishIndex;

	// Token: 0x020017B5 RID: 6069
	public enum MultiSelectOperateType
	{
		// Token: 0x0400AC5B RID: 44123
		InventoryToSold = 1,
		// Token: 0x0400AC5C RID: 44124
		SoldToInventory,
		// Token: 0x0400AC5D RID: 44125
		WarehouseToSold,
		// Token: 0x0400AC5E RID: 44126
		SoldToWarehouse,
		// Token: 0x0400AC5F RID: 44127
		TreasuryToSold,
		// Token: 0x0400AC60 RID: 44128
		SoldToTreasury
	}

	// Token: 0x020017B6 RID: 6070
	private class FeastDishDisplayData
	{
		// Token: 0x0600D4CC RID: 54476 RVA: 0x005B7562 File Offset: 0x005B5762
		public bool IsInitialDishRemoved()
		{
			return this.InitialDishLocation >= 0;
		}

		// Token: 0x0600D4CD RID: 54477 RVA: 0x005B7570 File Offset: 0x005B5770
		public bool IsCurrentDishNeedAdd()
		{
			ItemDisplayData currentDish = this.CurrentDish;
			sbyte? b = (currentDish != null) ? new sbyte?(currentDish.ItemSourceType) : null;
			int? num = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
			int num2 = 0;
			return num.GetValueOrDefault() >= num2 & num != null;
		}

		// Token: 0x0400AC61 RID: 44129
		public int InitialDishLocation = -1;

		// Token: 0x0400AC62 RID: 44130
		public ItemDisplayData InitialDish = null;

		// Token: 0x0400AC63 RID: 44131
		public ItemDisplayData CurrentDish = null;

		// Token: 0x0400AC64 RID: 44132
		public GameObject DisplayObject = null;
	}
}
