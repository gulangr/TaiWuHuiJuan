using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using GameData.Common;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.VillagerRole;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003B8 RID: 952
public class UI_Warehouse : UIBase
{
	// Token: 0x170005CF RID: 1487
	// (get) Token: 0x0600395F RID: 14687 RVA: 0x001D1B81 File Offset: 0x001CFD81
	private List<ItemSourceType> CurNeedItemSourceTypeList
	{
		get
		{
			return (this._warehouseItemSourceType == ItemSourceType.Trough) ? this._troughNeedItemSourceTypeList : this._warehouseNeedItemSourceTypeList;
		}
	}

	// Token: 0x170005D0 RID: 1488
	// (get) Token: 0x06003960 RID: 14688 RVA: 0x001D1B9A File Offset: 0x001CFD9A
	private List<ItemDisplayData> CurInventoryItems
	{
		get
		{
			return this._itemDict[this._inventoryItemSourceType];
		}
	}

	// Token: 0x170005D1 RID: 1489
	// (get) Token: 0x06003961 RID: 14689 RVA: 0x001D1BAD File Offset: 0x001CFDAD
	private List<ItemDisplayData> CurWarehouseItems
	{
		get
		{
			return this._itemDict[this._warehouseItemSourceType];
		}
	}

	// Token: 0x06003962 RID: 14690 RVA: 0x001D1BC0 File Offset: 0x001CFDC0
	public override void OnInit(ArgumentBox argsBox)
	{
		this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		this.<OnInit>g__InitButton|38_1(base.CGet<Refers>("Inventory"), true);
		this.<OnInit>g__InitButton|38_1(base.CGet<Refers>("Warehouse"), false);
		this._multiplyItemScrollView = base.CGet<MultiplyItemScrollView>("MultiplyItemScrollView");
		this._multiplyItemScrollView.Init(this._taiwuCharId, this._itemDict, new Action(this.<OnInit>g__OnEnter|38_2), new Action(this.<OnInit>g__OnExit|38_3), null, true);
		this._warehouseItemSourceType = ItemSourceType.Warehouse;
		ItemSourceType type;
		bool flag = argsBox != null && argsBox.Get<ItemSourceType>("WarehouseItemSourceType", out type);
		if (flag)
		{
			this._warehouseItemSourceType = type;
		}
		bool flag2 = argsBox == null || !argsBox.Get("CallTriggerListner", out this._callTriggerListner);
		if (flag2)
		{
			this._callTriggerListner = true;
		}
		this._subTogGroup = base.CGet<SubTogGroup>("SubTitleGroup");
		this._subTogGroup.gameObject.SetActive(false);
		foreach (ItemSourceType itemSourceType in this.CurNeedItemSourceTypeList)
		{
			List<ItemDisplayData> list;
			bool flag3 = this._itemDict.TryGetValue(itemSourceType, out list);
			if (flag3)
			{
				list.Clear();
			}
			else
			{
				this._itemDict.Add(itemSourceType, new List<ItemDisplayData>());
			}
		}
		CButtonObsolete btnRecord = base.CGet<CButtonObsolete>("BtnRecord");
		bool showMoreTog = SingletonObject.getInstance<BasicGameData>().CanShowMoreTogOnWarehouse();
		bool isTrough = this._warehouseItemSourceType == ItemSourceType.Trough;
		btnRecord.gameObject.SetActive(!isTrough && showMoreTog);
		btnRecord.ClearAndAddListener(delegate
		{
			TaiwuVillageStorageType storageType = UI_TaiwuVillageStoragesRecord.GetStorageType(this._warehouseItemSourceType);
			ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("StorageType", storageType);
			UIElement.TaiwuVillageStoragesRecord.SetOnInitArgs(args);
			UIManager.Instance.ShowUI(UIElement.TaiwuVillageStoragesRecord, true);
		});
	}

	// Token: 0x06003963 RID: 14691 RVA: 0x001D1DA0 File Offset: 0x001CFFA0
	private void Awake()
	{
		string loadTips = LocalStringManager.Get(LanguageKey.LK_Weight) + LocalStringManager.Get(LanguageKey.LK_Colon_Symbol);
		base.CGet<TextMeshProUGUI>("WarehouseLoadTips").text = loadTips;
		base.CGet<TextMeshProUGUI>("InventoryLoadTips").text = loadTips;
		this._warehouseScroll = base.CGet<GroupedItemScrollView>("WarehouseItemGroupedScroll");
		this._warehouseScroll.Init();
		this._warehouseScroll.ItemListChangedAction = new Action(this.RefreshBtnInteractable);
		this._warehouseScroll.ItemScrollView.AddOnScrollEvent(new Action(this.OnScroll));
		this._warehouseScroll.GrayEmptyGroupTitle = true;
		this._inventoryScroll = base.CGet<ItemScrollView>("InventoryItemScroll");
		this._inventoryScroll.Init();
		List<ItemDisplayData> curInventoryItems = this.CurInventoryItems;
		this._inventoryScroll.SetItemList(ref curInventoryItems, true, "warehouse_inventory", this._inventoryScroll.MySortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderInventoryItem));
		this._inventoryScroll.SortAndFilter.SetItemInteraction = ((ItemDisplayData data) => !this.CheckItemIsLocked(data, true));
		this._inventoryScroll.ItemListChangedAction = new Action(this.RefreshBtnInteractable);
		this._inventoryScroll.InfinityScroll.AddOnScrollEvent(new Action(this.OnScroll));
		DragDropGroup dragDropGroup = base.GetComponent<DragDropGroup>();
		dragDropGroup.AddOnDragStartHandler(new DragDropGroup.OnDragStartHandler(this.OnDragStart));
		dragDropGroup.AddOnDropHandler(new DragDropGroup.OnDropHandler(this.OnDrop));
		this._dropToWarehouse = base.CGet<DragDrop>("DropItemToWarehouse");
		this._dropToWarehouse.Identify = "warehouse";
		this._dropToWarehouse.CanDrop = ((object obj) => true);
		this._dropToWarehouse.OnEnterNotice = delegate()
		{
			this._dropToWarehouse.transform.GetChild(0).gameObject.SetActive(true);
		};
		this._dropToWarehouse.OnExitNotice = delegate()
		{
			this._dropToWarehouse.transform.GetChild(0).gameObject.SetActive(false);
		};
		this._dropToInventory = base.CGet<DragDrop>("DropItemToInventory");
		this._dropToInventory.Identify = "inventory";
		this._dropToInventory.CanDrop = ((object obj) => true);
		this._dropToInventory.OnEnterNotice = delegate()
		{
			this._dropToInventory.transform.GetChild(0).gameObject.SetActive(true);
		};
		this._dropToInventory.OnExitNotice = delegate()
		{
			this._dropToInventory.transform.GetChild(0).gameObject.SetActive(false);
		};
		this._inventoryTogGroup = base.CGet<CToggleGroupObsolete>("InventoryTogGroup");
		this._inventoryTogGroup.InitPreOnToggle(-1);
		this._inventoryTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnInventoryItemTogChange);
		this._warehouseTogGroup = base.CGet<CToggleGroupObsolete>("WarehouseTogGroup");
		this._warehouseTogGroup.InitPreOnToggle(-1);
		this._warehouseTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnWarehouseItemTogChange);
		this.RefreshBtnInteractable();
	}

	// Token: 0x06003964 RID: 14692 RVA: 0x001D207F File Offset: 0x001D027F
	private void OnDestroy()
	{
		this._warehouseScroll.ItemScrollView.RemoveOnScrollEvent(new Action(this.OnScroll));
		this._inventoryScroll.InfinityScroll.RemoveOnScrollEvent(new Action(this.OnScroll));
	}

	// Token: 0x06003965 RID: 14693 RVA: 0x001D20BC File Offset: 0x001D02BC
	private void OnScroll()
	{
		this._multiplyItemScrollView.HideGradeLimitTip();
	}

	// Token: 0x06003966 RID: 14694 RVA: 0x001D20CC File Offset: 0x001D02CC
	private void OnEnable()
	{
		GEvent.Add(UiEvents.ExitMultiplyOperation, new GEvent.Callback(this._multiplyItemScrollView.OnExitMultiplyOperation));
		GEvent.Add(UiEvents.ItemMultiplyOperationTypeChange, new GEvent.Callback(this._multiplyItemScrollView.OnItemMultiplyOperationTypeChange));
		GEvent.Add(UiEvents.ItemMultiplyOperationConfirm, new GEvent.Callback(this._multiplyItemScrollView.OnItemMultiplyOperationConfirm));
		GEvent.Add(UiEvents.ItemMultiplyOperationCancelSelection, new GEvent.Callback(this._multiplyItemScrollView.OnItemMultiplyOperationCancelSelection));
		GEvent.Add(UiEvents.ItemGradeFilterSettingChange, new GEvent.Callback(this.OnItemGradeFilterSettingChange));
		ItemSourceType warehouseItemSourceType = this._warehouseItemSourceType;
		if (!true)
		{
		}
		int num;
		switch (warehouseItemSourceType)
		{
		case ItemSourceType.Warehouse:
			num = UI_Warehouse.WarehouseTogKey.Warehouse.ToInt();
			break;
		case ItemSourceType.Treasury:
			num = UI_Warehouse.WarehouseTogKey.Treasury.ToInt();
			break;
		case ItemSourceType.Trough:
			num = UI_Warehouse.WarehouseTogKey.Trough.ToInt();
			break;
		case ItemSourceType.Stock:
			num = UI_Warehouse.WarehouseTogKey.StockStorage.ToInt();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (!true)
		{
		}
		int warehouseTogKey = num;
		this._warehouseTogGroup.SetInteractable(true, warehouseTogKey);
		this._warehouseTogGroup.Set(warehouseTogKey, true, true);
		bool showMoreTog = SingletonObject.getInstance<BasicGameData>().CanShowMoreTogOnWarehouse();
		foreach (CToggleObsolete tog in this._inventoryTogGroup.GetAll())
		{
			bool flag = tog.Key > 1;
			if (flag)
			{
				tog.gameObject.SetActive(showMoreTog);
			}
		}
		foreach (CToggleObsolete tog2 in this._warehouseTogGroup.GetAll())
		{
			bool modeIsTrough = this._warehouseItemSourceType == ItemSourceType.Trough;
			bool togIsTrough = tog2.Key == UI_Warehouse.WarehouseTogKey.Trough.ToInt();
			bool togIsMore = tog2.Key > 0;
			bool showThisMoreTog = !togIsMore || showMoreTog;
			bool show = (modeIsTrough && togIsTrough) || (!modeIsTrough && !togIsTrough && showThisMoreTog);
			tog2.gameObject.SetActive(show);
		}
		this._inventoryTogGroup.SetInteractable(true, UI_Warehouse.InventoryTogKey.Inventory.ToInt());
		this._inventoryTogGroup.Set(UI_Warehouse.InventoryTogKey.Inventory.ToInt(), true, true);
		RectTransform scrollBack = base.CGet<RectTransform>("ScrollBack");
		bool isLongScroll = true;
		scrollBack.sizeDelta = scrollBack.sizeDelta.SetY(isLongScroll ? this._longScrollBackHeight : this._shortScrollBackHeight);
		RectTransform space = base.CGet<RectTransform>("Space");
		space.sizeDelta = space.sizeDelta.SetY(isLongScroll ? this._shortSpaceHeight : this._longBackHeight);
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(space.parent as RectTransform);
		});
	}

	// Token: 0x06003967 RID: 14695 RVA: 0x001D23D4 File Offset: 0x001D05D4
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.ExitMultiplyOperation, new GEvent.Callback(this._multiplyItemScrollView.OnExitMultiplyOperation));
		GEvent.Remove(UiEvents.ItemMultiplyOperationTypeChange, new GEvent.Callback(this._multiplyItemScrollView.OnItemMultiplyOperationTypeChange));
		GEvent.Remove(UiEvents.ItemMultiplyOperationConfirm, new GEvent.Callback(this._multiplyItemScrollView.OnItemMultiplyOperationConfirm));
		GEvent.Remove(UiEvents.ItemMultiplyOperationCancelSelection, new GEvent.Callback(this._multiplyItemScrollView.OnItemMultiplyOperationCancelSelection));
		GEvent.Remove(UiEvents.ItemGradeFilterSettingChange, new GEvent.Callback(this.OnItemGradeFilterSettingChange));
		GEvent.OnEvent(UiEvents.WareHouseOnDisable, null);
		this._warehouseScroll.SaveSortFilterSetting(false);
		this._inventoryScroll.SaveSortFilterSetting(true);
		bool callTriggerListner = this._callTriggerListner;
		if (callTriggerListner)
		{
			TaiwuEventDomainMethod.Call.TriggerListener("WarehouseShowed", true);
			SingletonObject.getInstance<WorldMapModel>().ChangeTaiwuMoveState(WorldMapModel.MoveState.WaitEventShow);
		}
	}

	// Token: 0x06003968 RID: 14696 RVA: 0x001D24C4 File Offset: 0x001D06C4
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)this._taiwuCharId, new uint[]
		{
			104U,
			103U
		}));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 8, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 7, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 69, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 70, ulong.MaxValue, null));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 22, ulong.MaxValue, null));
	}

	// Token: 0x06003969 RID: 14697 RVA: 0x001D2570 File Offset: 0x001D0770
	private void OnListenerIdReady()
	{
		CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, new List<int>
		{
			SingletonObject.getInstance<BasicGameData>().TaiwuCharId
		});
		TaiwuDomainMethod.Call.CanTransferItemToWarehouse(this.Element.GameDataListenerId);
		TaiwuDomainMethod.Call.GetTreasuryNeededItemList(this.Element.GameDataListenerId);
		this._warehouseScroll.SetCharId(this._taiwuCharId);
		this._inventoryScroll.SetCharId(this._taiwuCharId);
		this.Refresh();
	}

	// Token: 0x0600396A RID: 14698 RVA: 0x001D25F4 File Offset: 0x001D07F4
	private void Refresh()
	{
		foreach (ItemSourceType itemSourceType in this.CurNeedItemSourceTypeList)
		{
			TaiwuDomainMethod.Call.GetAllItems(this.Element.GameDataListenerId, itemSourceType, true);
		}
	}

	// Token: 0x0600396B RID: 14699 RVA: 0x001D2658 File Offset: 0x001D0858
	public override void QuickHide()
	{
		bool flag = base.GetComponent<DragDropGroup>().DraggingIdentify != null;
		if (flag)
		{
			base.GetComponent<DragDropGroup>().EndDrag();
		}
		else
		{
			bool isMultiItemSelect = this._multiplyItemScrollView.IsMultiItemSelect;
			if (isMultiItemSelect)
			{
				this._multiplyItemScrollView.TryExitMultiplyMode();
			}
			else
			{
				AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
				UIManager.Instance.HideUI(this.Element);
			}
		}
	}

	// Token: 0x0600396C RID: 14700 RVA: 0x001D26C8 File Offset: 0x001D08C8
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b != 0)
			{
				if (b == 1)
				{
					bool flag = notification.DomainId == 5;
					if (flag)
					{
						bool flag2 = notification.MethodId == 42;
						if (flag2)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._canTransfer);
						}
						else
						{
							bool flag3 = notification.MethodId == 139;
							if (flag3)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._villagerNeededItemSet);
							}
							else
							{
								bool flag4 = notification.MethodId == 118;
								if (flag4)
								{
									ValueTuple<ItemSourceType, List<ItemDisplayData>> tuple = default(ValueTuple<ItemSourceType, List<ItemDisplayData>>);
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref tuple);
									this._itemDict[tuple.Item1].Clear();
									bool flag5 = tuple.Item2 != null;
									if (flag5)
									{
										this._itemDict[tuple.Item1].AddRange(tuple.Item2);
									}
									ItemSourceType item = tuple.Item1;
									List<ItemSourceType> curNeedItemSourceTypeList = this.CurNeedItemSourceTypeList;
									bool flag6 = item == curNeedItemSourceTypeList[curNeedItemSourceTypeList.Count - 1];
									if (flag6)
									{
										this._itemDict[ItemSourceType.Inventory].AddRange(this._itemDict[ItemSourceType.Equipment]);
										List<ItemDisplayData> list;
										bool flag7 = this._itemDict.TryGetValue(ItemSourceType.Resources, out list);
										if (flag7)
										{
											foreach (ItemDisplayData itemData in list)
											{
												sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(itemData.Key.ItemType, itemData.Key.TemplateId);
												bool flag8 = resourceType < 7;
												if (flag8)
												{
													this._itemDict[ItemSourceType.Inventory].Add(itemData);
												}
											}
										}
										this.RefreshWarehouseItems(false);
										this.RefreshInventoryItems();
										this.Element.ShowAfterRefresh();
									}
								}
							}
						}
					}
					else
					{
						bool flag9 = notification.DomainId == 4;
						if (flag9)
						{
							bool flag10 = notification.MethodId == 48;
							if (flag10)
							{
								List<CharacterDisplayData> displayDataList = null;
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref displayDataList);
								this._taiwuDisplayData = displayDataList[0];
							}
						}
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				bool flag11 = uid.DomainId == 4;
				if (flag11)
				{
					bool flag12 = uid.SubId1 == 104U;
					if (flag12)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._inventoryCurrLoad);
					}
					else
					{
						bool flag13 = uid.SubId1 == 103U;
						if (flag13)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._inventoryMaxLoad);
						}
					}
					string currLoad = ((float)this._inventoryCurrLoad / 100f).ToString("f1").SetColor(CommonUtils.GetLoadWeightValueColor(this._inventoryCurrLoad, this._inventoryMaxLoad));
					base.CGet<TextMeshProUGUI>("InventoryLoad").text = string.Format("{0}/{1:f1}", currLoad, (float)this._inventoryMaxLoad / 100f);
					base.CGet<TooltipInvoker>("InventoryOverflowTIps").enabled = (this._inventoryCurrLoad > this._inventoryMaxLoad);
				}
				else
				{
					bool flag14 = uid.DomainId == 5;
					if (flag14)
					{
						ushort dataId = uid.DataId;
						bool flag15 = dataId == 8 || dataId == 7 || dataId == 69 || dataId == 70;
						if (flag15)
						{
							bool flag16 = uid.DataId == 8;
							if (flag16)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._wareHouseCurrLoad);
							}
							else
							{
								bool flag17 = uid.DataId == 7;
								if (flag17)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._wareHouseMaxLoad);
								}
								else
								{
									bool flag18 = uid.DataId == 69;
									if (flag18)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._troughMaxLoad);
									}
									else
									{
										bool flag19 = uid.DataId == 70;
										if (flag19)
										{
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._troughCurrLoad);
										}
									}
								}
							}
							bool isTrough = this._warehouseItemSourceType == ItemSourceType.Trough;
							int currentLoad = isTrough ? this._troughCurrLoad : this._wareHouseCurrLoad;
							int maxLoad = isTrough ? this._troughMaxLoad : this._wareHouseMaxLoad;
							string currLoadText = ((float)currentLoad / 100f).ToString("f1").SetColor(CommonUtils.GetLoadWeightValueColor(currentLoad, maxLoad));
							base.CGet<TextMeshProUGUI>("WarehouseLoad").text = string.Format("{0}/{1:f1}", currLoadText, (float)maxLoad / 100f);
							base.CGet<TooltipInvoker>("WarehouseOverflowTips").enabled = (currentLoad > maxLoad);
						}
						else
						{
							bool flag20 = uid.DataId == 22;
							if (flag20)
							{
								int moveTimeCostPercent = 0;
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref moveTimeCostPercent);
								base.CGet<TooltipInvoker>("InventoryOverflowTIps").PresetParam[1] = LocalStringManager.GetFormat(LanguageKey.LK_Inventory_Overflow_Tips, moveTimeCostPercent - 100).ColorReplace();
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600396D RID: 14701 RVA: 0x001D2C5C File Offset: 0x001D0E5C
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "Close";
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x0600396E RID: 14702 RVA: 0x001D2C8C File Offset: 0x001D0E8C
	private void OnInventoryItemTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		UI_Warehouse.<>c__DisplayClass50_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.togNew = togNew;
		bool flag = !CS$<>8__locals1.togNew;
		if (!flag)
		{
			UI_Warehouse.InventoryTogKey newTogKey = (UI_Warehouse.InventoryTogKey)CS$<>8__locals1.togNew.Key;
			base.CGet<GameObject>("LoadBackRight").SetActive(newTogKey == UI_Warehouse.InventoryTogKey.Inventory);
			if (!true)
			{
			}
			ItemSourceType inventoryItemSourceType;
			switch (newTogKey)
			{
			case UI_Warehouse.InventoryTogKey.Inventory:
				inventoryItemSourceType = ItemSourceType.Inventory;
				break;
			case UI_Warehouse.InventoryTogKey.Warehouse:
				inventoryItemSourceType = ItemSourceType.Warehouse;
				break;
			case UI_Warehouse.InventoryTogKey.Treasury:
				inventoryItemSourceType = ItemSourceType.Treasury;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			this._inventoryItemSourceType = inventoryItemSourceType;
			this._multiplyItemScrollView.InventoryItemSourceType = this._inventoryItemSourceType;
			MonoJoint componentInChildren = CS$<>8__locals1.togNew.GetComponentInChildren<MonoJoint>(true);
			if (componentInChildren != null)
			{
				componentInChildren.JointSync();
			}
			this.<OnInventoryItemTogChange>g__HandleConflict|50_0(UI_Warehouse.WarehouseTogKey.Warehouse, UI_Warehouse.InventoryTogKey.Warehouse, ref CS$<>8__locals1);
			this.<OnInventoryItemTogChange>g__HandleConflict|50_0(UI_Warehouse.WarehouseTogKey.Treasury, UI_Warehouse.InventoryTogKey.Treasury, ref CS$<>8__locals1);
			base.CGet<TextMeshProUGUI>("InventoryTitle").text = UI_Warehouse.GetTitle(this._inventoryItemSourceType, true);
			this.RefreshInventoryItems();
			this.RefreshWarehouseItems(false);
		}
	}

	// Token: 0x0600396F RID: 14703 RVA: 0x001D2D84 File Offset: 0x001D0F84
	private void OnWarehouseItemTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		UI_Warehouse.<>c__DisplayClass51_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.togNew = togNew;
		bool flag = !CS$<>8__locals1.togNew;
		if (!flag)
		{
			List<SubTogGroup.TogContent> togContents = new List<SubTogGroup.TogContent>();
			CS$<>8__locals1.newTogKey = (UI_Warehouse.WarehouseTogKey)CS$<>8__locals1.togNew.Key;
			switch (CS$<>8__locals1.newTogKey)
			{
			case UI_Warehouse.WarehouseTogKey.Warehouse:
				this._warehouseItemSourceType = ItemSourceType.Warehouse;
				break;
			case UI_Warehouse.WarehouseTogKey.Treasury:
				this._warehouseItemSourceType = ItemSourceType.Treasury;
				break;
			case UI_Warehouse.WarehouseTogKey.StockStorage:
				this._warehouseItemSourceType = ItemSourceType.Stock;
				break;
			case UI_Warehouse.WarehouseTogKey.Trough:
				this._warehouseItemSourceType = ItemSourceType.Trough;
				break;
			default:
				throw new ArgumentOutOfRangeException("newTogKey");
			}
			bool showSubTog = togContents.Count > 0;
			this._subTogGroup.gameObject.SetActive(showSubTog);
			bool flag2 = showSubTog;
			if (!flag2)
			{
				this.<OnWarehouseItemTogChange>g__RefreshScroll|51_1(ref CS$<>8__locals1);
			}
			MonoJoint componentInChildren = CS$<>8__locals1.togNew.GetComponentInChildren<MonoJoint>(true);
			if (componentInChildren != null)
			{
				componentInChildren.JointSync();
			}
			this.<OnWarehouseItemTogChange>g__HandleConflict|51_2(UI_Warehouse.WarehouseTogKey.Warehouse, UI_Warehouse.InventoryTogKey.Warehouse, ref CS$<>8__locals1);
			this.<OnWarehouseItemTogChange>g__HandleConflict|51_2(UI_Warehouse.WarehouseTogKey.Treasury, UI_Warehouse.InventoryTogKey.Treasury, ref CS$<>8__locals1);
		}
	}

	// Token: 0x06003970 RID: 14704 RVA: 0x001D2E88 File Offset: 0x001D1088
	private SubTogGroup.TogContent GetTogContent(ItemSourceType sourceType)
	{
		SubTogGroup.TogContent togContent = default(SubTogGroup.TogContent);
		if (!true)
		{
		}
		if (sourceType != ItemSourceType.Stock)
		{
			throw new ArgumentOutOfRangeException("sourceType", sourceType, null);
		}
		string text = LocalStringManager.Get(LanguageKey.LK_StockStorageGoodsShelf);
		if (!true)
		{
		}
		string content = text;
		if (!true)
		{
		}
		if (sourceType != ItemSourceType.Stock)
		{
			throw new ArgumentOutOfRangeException("sourceType", sourceType, null);
		}
		text = LocalStringManager.Get(LanguageKey.LK_StockStorageGoodsShelf_Tip);
		if (!true)
		{
		}
		string tipContent = text;
		togContent.Content = content;
		togContent.TipTitle = content;
		togContent.TipContent = tipContent;
		return togContent;
	}

	// Token: 0x06003971 RID: 14705 RVA: 0x001D2F1C File Offset: 0x001D111C
	private void RefreshInventoryItems()
	{
		List<ItemDisplayData> items = this.CurInventoryItems;
		this._inventoryScroll.SetItemList(ref items, false, null, false, null);
		this.RefreshBtnInteractable();
	}

	// Token: 0x06003972 RID: 14706 RVA: 0x001D2F4C File Offset: 0x001D114C
	private void RefreshWarehouseItems(bool reset = false)
	{
		this._warehouseScroll.SetItemList(this.CurWarehouseItems, new GroupedItemScrollView.ItemGroupGetter(this.GroupItems), this._warehouseScroll.MySortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderWarehouseItem), false, null);
		if (reset)
		{
			this._warehouseScroll.ResetScroll();
		}
		this.RefreshBtnInteractable();
	}

	// Token: 0x06003973 RID: 14707 RVA: 0x001D2FB0 File Offset: 0x001D11B0
	private List<GroupedItemScrollView.ItemGroup> GroupItems(List<ItemDisplayData> items)
	{
		bool isSorting = this._warehouseScroll.SortAndFilter.SortFilterSetting.SortTypes.Count > 0;
		bool isFilteringAll = this._warehouseScroll.SortAndFilter.ActiveFilterType == ItemSortAndFilter.ItemFilterType.Invalid;
		List<GroupedItemScrollView.ItemGroup> groups = new List<GroupedItemScrollView.ItemGroup>();
		bool noGroup = isSorting || !isFilteringAll;
		bool flag = noGroup;
		List<GroupedItemScrollView.ItemGroup> result;
		if (flag)
		{
			groups.Add(new GroupedItemScrollView.ItemGroup
			{
				GroupId = -1,
				ItemList = items
			});
			result = groups;
		}
		else
		{
			ItemSourceType warehouseItemSourceType = this._warehouseItemSourceType;
			groups.Add(new GroupedItemScrollView.ItemGroup
			{
				GroupId = -1,
				ItemList = items
			});
			result = groups;
		}
		return result;
	}

	// Token: 0x06003974 RID: 14708 RVA: 0x001D3068 File Offset: 0x001D1268
	private void GroupItemsFoodStorageWarehouse(List<GroupedItemScrollView.ItemGroup> groups, List<ItemDisplayData> items)
	{
		HashSet<ItemDisplayData> sourceSet = new HashSet<ItemDisplayData>(items);
		GroupedItemScrollView.ItemGroup resourceGroup = UI_Warehouse.NewResourceGroup();
		this.SelectResourceToGroup(sourceSet, resourceGroup.ItemList);
		groups.Add(resourceGroup);
		GroupedItemScrollView.ItemGroup toolGroup = UI_Warehouse.NewToolGroup();
		this.SelectToolToGroup(sourceSet, toolGroup.ItemList);
		groups.Add(toolGroup);
		GroupedItemScrollView.ItemGroup otherGroup = UI_Warehouse.NewOtherGroup();
		otherGroup.ItemList = sourceSet.ToList<ItemDisplayData>();
		groups.Add(otherGroup);
	}

	// Token: 0x06003975 RID: 14709 RVA: 0x001D30D0 File Offset: 0x001D12D0
	private void GroupItemsMedicineStorageWarehouse(List<GroupedItemScrollView.ItemGroup> groups, List<ItemDisplayData> items)
	{
		HashSet<ItemDisplayData> sourceSet = new HashSet<ItemDisplayData>(items);
		GroupedItemScrollView.ItemGroup resourceGroup = UI_Warehouse.NewResourceGroup();
		this.SelectResourceToGroup(sourceSet, resourceGroup.ItemList);
		groups.Add(resourceGroup);
		GroupedItemScrollView.ItemGroup toolGroup = UI_Warehouse.NewToolGroup();
		this.SelectToolToGroup(sourceSet, toolGroup.ItemList);
		groups.Add(toolGroup);
		GroupedItemScrollView.ItemGroup medicineGroup = new GroupedItemScrollView.ItemGroup(800, LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + LocalStringManager.Get(LanguageKey.LK_ItemSubType_800));
		GroupedItemScrollView.ItemGroup poisonGroup = new GroupedItemScrollView.ItemGroup(801, LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + LocalStringManager.Get(LanguageKey.LK_ItemSubType_801));
		this.SelectItemSubTypeToGroup(sourceSet, medicineGroup.ItemList, 800);
		this.SelectItemSubTypeToGroup(sourceSet, poisonGroup.ItemList, 801);
		groups.Add(medicineGroup);
		groups.Add(poisonGroup);
		GroupedItemScrollView.ItemGroup otherGroup = UI_Warehouse.NewOtherGroup();
		otherGroup.ItemList = sourceSet.ToList<ItemDisplayData>();
		groups.Add(otherGroup);
	}

	// Token: 0x06003976 RID: 14710 RVA: 0x001D31BC File Offset: 0x001D13BC
	private void GroupItemsCraftStorageWarehouse(List<GroupedItemScrollView.ItemGroup> groups, List<ItemDisplayData> items)
	{
		HashSet<ItemDisplayData> sourceSet = new HashSet<ItemDisplayData>(items);
		GroupedItemScrollView.ItemGroup resourceGroup = UI_Warehouse.NewResourceGroup();
		this.SelectResourceToGroup(sourceSet, resourceGroup.ItemList);
		groups.Add(resourceGroup);
		GroupedItemScrollView.ItemGroup toolGroup = UI_Warehouse.NewToolGroup();
		this.SelectToolToGroup(sourceSet, toolGroup.ItemList);
		groups.Add(toolGroup);
		GroupedItemScrollView.ItemGroup otherGroup = UI_Warehouse.NewOtherGroup();
		otherGroup.ItemList = sourceSet.ToList<ItemDisplayData>();
		groups.Add(otherGroup);
	}

	// Token: 0x06003977 RID: 14711 RVA: 0x001D3224 File Offset: 0x001D1424
	private void GroupItemsStockStorageWarehouse(List<GroupedItemScrollView.ItemGroup> groups, List<ItemDisplayData> items)
	{
		HashSet<ItemDisplayData> sourceSet = new HashSet<ItemDisplayData>(items);
		GroupedItemScrollView.ItemGroup resourceGroup = UI_Warehouse.NewResourceGroup();
		this.SelectResourceToGroup(sourceSet, resourceGroup.ItemList);
		groups.Add(resourceGroup);
		GroupedItemScrollView.ItemGroup otherGroup = UI_Warehouse.NewOtherGroup();
		otherGroup.ItemList = sourceSet.ToList<ItemDisplayData>();
		groups.Add(otherGroup);
	}

	// Token: 0x06003978 RID: 14712 RVA: 0x001D3270 File Offset: 0x001D1470
	private void GroupItemsCraftStorageMaterial(List<GroupedItemScrollView.ItemGroup> groups, List<ItemDisplayData> items)
	{
		HashSet<ItemDisplayData> sourceSet = new HashSet<ItemDisplayData>(items);
		GroupedItemScrollView.ItemGroup woodGroup = new GroupedItemScrollView.ItemGroup(1, LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + LocalStringManager.Get(LanguageKey.LK_ItemSubType_501));
		this.SelectResourceTypeToGroup(sourceSet, woodGroup.ItemList, 1);
		GroupedItemScrollView.ItemGroup metalGroup = new GroupedItemScrollView.ItemGroup(2, LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + LocalStringManager.Get(LanguageKey.LK_ItemSubType_502));
		this.SelectResourceTypeToGroup(sourceSet, metalGroup.ItemList, 2);
		GroupedItemScrollView.ItemGroup jadeGroup = new GroupedItemScrollView.ItemGroup(3, LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + LocalStringManager.Get(LanguageKey.LK_ItemSubType_503));
		this.SelectResourceTypeToGroup(sourceSet, jadeGroup.ItemList, 3);
		GroupedItemScrollView.ItemGroup fabricGroup = new GroupedItemScrollView.ItemGroup(4, LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + LocalStringManager.Get(LanguageKey.LK_ItemSubType_504));
		this.SelectResourceTypeToGroup(sourceSet, fabricGroup.ItemList, 4);
		bool flag = fabricGroup.ItemList.Count > 0;
		if (flag)
		{
			groups.Add(fabricGroup);
		}
		bool flag2 = woodGroup.ItemList.Count > 0;
		if (flag2)
		{
			groups.Add(woodGroup);
		}
		bool flag3 = jadeGroup.ItemList.Count > 0;
		if (flag3)
		{
			groups.Add(jadeGroup);
		}
		bool flag4 = metalGroup.ItemList.Count > 0;
		if (flag4)
		{
			groups.Add(metalGroup);
		}
		Tester.Assert(sourceSet.Count == 0, "");
	}

	// Token: 0x06003979 RID: 14713 RVA: 0x001D33CC File Offset: 0x001D15CC
	private void GroupItemsMedicineStorageMaterial(List<GroupedItemScrollView.ItemGroup> groups, List<ItemDisplayData> items)
	{
		HashSet<ItemDisplayData> sourceSet = new HashSet<ItemDisplayData>(items);
		GroupedItemScrollView.ItemGroup medicineGroup = new GroupedItemScrollView.ItemGroup(800, LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + LocalStringManager.Get(LanguageKey.LK_ItemSubType_505));
		GroupedItemScrollView.ItemGroup poisonGroup = new GroupedItemScrollView.ItemGroup(801, LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + LocalStringManager.Get(LanguageKey.LK_ItemSubType_506));
		this.SelectItemSubTypeToGroup(sourceSet, medicineGroup.ItemList, 505);
		this.SelectItemSubTypeToGroup(sourceSet, poisonGroup.ItemList, 506);
		bool flag = medicineGroup.ItemList.Count > 0;
		if (flag)
		{
			groups.Add(medicineGroup);
		}
		bool flag2 = poisonGroup.ItemList.Count > 0;
		if (flag2)
		{
			groups.Add(poisonGroup);
		}
		Tester.Assert(sourceSet.Count == 0, "");
	}

	// Token: 0x0600397A RID: 14714 RVA: 0x001D349C File Offset: 0x001D169C
	private void GroupItemsFoodStorageMaterial(List<GroupedItemScrollView.ItemGroup> groups, List<ItemDisplayData> items)
	{
		HashSet<ItemDisplayData> sourceSet = new HashSet<ItemDisplayData>(items);
		GroupedItemScrollView.ItemGroup meatGroup = new GroupedItemScrollView.ItemGroup(701, LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + LocalStringManager.Get(LanguageKey.LK_ItemSubType_701));
		GroupedItemScrollView.ItemGroup vegetarianGroup = new GroupedItemScrollView.ItemGroup(700, LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + LocalStringManager.Get(LanguageKey.LK_ItemSubType_700));
		this.SelectItemFoodMaterialToGroup(sourceSet, meatGroup.ItemList, true);
		this.SelectItemFoodMaterialToGroup(sourceSet, vegetarianGroup.ItemList, false);
		bool flag = meatGroup.ItemList.Count > 0;
		if (flag)
		{
			groups.Add(meatGroup);
		}
		bool flag2 = vegetarianGroup.ItemList.Count > 0;
		if (flag2)
		{
			groups.Add(vegetarianGroup);
		}
		Tester.Assert(sourceSet.Count == 0, "");
	}

	// Token: 0x0600397B RID: 14715 RVA: 0x001D3564 File Offset: 0x001D1764
	private void SelectItemFoodMaterialToGroup(HashSet<ItemDisplayData> leftItemSet, List<ItemDisplayData> resultList, bool isMeat)
	{
		List<ItemDisplayData> itemsToRemove = leftItemSet.Where(delegate(ItemDisplayData item)
		{
			bool flag = item.Key.ItemType == 5;
			bool result;
			if (flag)
			{
				MaterialItem config = Config.Material.Instance[item.Key.TemplateId];
				result = (isMeat ^ config.GroupId == 70);
			}
			else
			{
				result = false;
			}
			return result;
		}).ToList<ItemDisplayData>();
		foreach (ItemDisplayData item2 in itemsToRemove)
		{
			resultList.Add(item2);
			leftItemSet.Remove(item2);
		}
	}

	// Token: 0x0600397C RID: 14716 RVA: 0x001D35E8 File Offset: 0x001D17E8
	private void SelectResourceTypeToGroup(HashSet<ItemDisplayData> leftItemSet, List<ItemDisplayData> resultList, sbyte resourceType)
	{
		List<ItemDisplayData> itemsToRemove = leftItemSet.Where(delegate(ItemDisplayData item)
		{
			sbyte itemResourceType = GameData.Domains.Taiwu.VillagerRole.SharedMethods.GetItemResourceType(item);
			return itemResourceType == resourceType;
		}).ToList<ItemDisplayData>();
		foreach (ItemDisplayData item2 in itemsToRemove)
		{
			resultList.Add(item2);
			leftItemSet.Remove(item2);
		}
	}

	// Token: 0x0600397D RID: 14717 RVA: 0x001D366C File Offset: 0x001D186C
	private static GroupedItemScrollView.ItemGroup NewOtherGroup()
	{
		return new GroupedItemScrollView.ItemGroup(4, LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + LocalStringManager.Get(LanguageKey.LK_WarehouseItemGroup_Other));
	}

	// Token: 0x0600397E RID: 14718 RVA: 0x001D36A0 File Offset: 0x001D18A0
	private static GroupedItemScrollView.ItemGroup NewToolGroup()
	{
		return new GroupedItemScrollView.ItemGroup(1, LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + LocalStringManager.Get(LanguageKey.LK_WarehouseItemGroup_Tool));
	}

	// Token: 0x0600397F RID: 14719 RVA: 0x001D36D4 File Offset: 0x001D18D4
	private static GroupedItemScrollView.ItemGroup NewResourceGroup()
	{
		return new GroupedItemScrollView.ItemGroup(0, LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + LocalStringManager.Get(LanguageKey.LK_WarehouseItemGroup_Resource));
	}

	// Token: 0x06003980 RID: 14720 RVA: 0x001D3708 File Offset: 0x001D1908
	private void SelectResourceToGroup(HashSet<ItemDisplayData> leftItemSet, List<ItemDisplayData> resultList)
	{
		List<ItemDisplayData> itemsToRemove = (from item in leftItemSet
		where item.IsResource
		select item).ToList<ItemDisplayData>();
		foreach (ItemDisplayData item2 in itemsToRemove)
		{
			resultList.Add(item2);
			leftItemSet.Remove(item2);
		}
	}

	// Token: 0x06003981 RID: 14721 RVA: 0x001D3790 File Offset: 0x001D1990
	private void SelectToolToGroup(HashSet<ItemDisplayData> leftItemSet, List<ItemDisplayData> resultList)
	{
		List<ItemDisplayData> itemsToRemove = (from item in leftItemSet
		where item.Key.ItemType == 6
		select item).ToList<ItemDisplayData>();
		foreach (ItemDisplayData item2 in itemsToRemove)
		{
			resultList.Add(item2);
			leftItemSet.Remove(item2);
		}
	}

	// Token: 0x06003982 RID: 14722 RVA: 0x001D3818 File Offset: 0x001D1A18
	private void SelectItemSubTypeToGroup(HashSet<ItemDisplayData> leftItemSet, List<ItemDisplayData> resultList, int subType)
	{
		List<ItemDisplayData> itemsToRemove = leftItemSet.Where(delegate(ItemDisplayData item)
		{
			short itemSubType = ItemTemplateHelper.GetItemSubType(item.Key.ItemType, item.Key.TemplateId);
			return (int)itemSubType == subType;
		}).ToList<ItemDisplayData>();
		foreach (ItemDisplayData item2 in itemsToRemove)
		{
			resultList.Add(item2);
			leftItemSet.Remove(item2);
		}
	}

	// Token: 0x06003983 RID: 14723 RVA: 0x001D389C File Offset: 0x001D1A9C
	private void OnRenderWarehouseItem(ItemDisplayData itemData, ItemView itemView)
	{
		DragDrop itemDrag = itemView.GetComponent<DragDrop>();
		itemView.UserString = "warehouse";
		itemDrag.Identify = itemView;
		itemDrag.DragOn = this._canTransfer;
		itemView.SetLocked(this.CheckItemIsLocked(itemData, false));
		this.SetResourceItemTip(itemView);
		bool flag = this._multiplyItemScrollView.IsMultiItemSelect && !this._multiplyItemScrollView.IsInventoryMultiply;
		if (flag)
		{
			this._multiplyItemScrollView.OnRenderItemMultiply(itemData, itemView);
			bool isSelected = this._multiplyItemScrollView.IsSelected(itemData);
			this.SetVillagerNeedMark(itemView, false, !isSelected);
		}
		else
		{
			itemView.SetSelectedOrder(false, 0);
			itemView.HideInteractionState();
			itemView.SetClickEvent(delegate
			{
				this._warehouseScroll.HandleClickItem(itemData, itemView, new Action<ItemView>(base.<OnRenderWarehouseItem>g__Action|1));
			});
			this.SetVillagerNeedMark(itemView, false, true);
		}
	}

	// Token: 0x06003984 RID: 14724 RVA: 0x001D39C8 File Offset: 0x001D1BC8
	private void OnRenderInventoryItem(ItemDisplayData itemData, ItemView itemView)
	{
		DragDrop itemDrag = itemView.GetComponent<DragDrop>();
		itemView.UserString = "inventory";
		itemView.SetLocked(!itemData.Interactable);
		bool flag = !itemData.Interactable;
		if (flag)
		{
			bool isResource = ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool lockInnerResult = !GameData.Domains.Taiwu.VillagerRole.SharedMethods.CheckCanStoreItem(this._warehouseItemSourceType, itemData, (int)this._taiwuDisplayData.AvatarRelatedData.DisplayAge);
			bool flag2 = !ItemTemplateHelper.IsTransferable(itemData.Key.ItemType, itemData.Key.TemplateId) && !isResource;
			if (flag2)
			{
				itemView.SetInteractionStateLockText(LocalStringManager.Get(LanguageKey.LK_Item_Operation_Locked));
			}
			else
			{
				bool flag3 = lockInnerResult;
				if (flag3)
				{
					itemView.SetInteractionStateLockText(LocalStringManager.Get(LanguageKey.LK_Item_Operation_TypeNotMatch));
				}
				else
				{
					itemView.SetInteractionStateLockText(LocalStringManager.Get(LanguageKey.LK_Item_Operation_FarAway));
				}
			}
		}
		itemDrag.Identify = itemView;
		itemDrag.DragOn = itemData.Interactable;
		this.SetResourceItemTip(itemView);
		bool flag4 = this._multiplyItemScrollView.IsMultiItemSelect && this._multiplyItemScrollView.IsInventoryMultiply;
		if (flag4)
		{
			this._multiplyItemScrollView.OnRenderItemMultiply(itemData, itemView);
			bool isSelected = this._multiplyItemScrollView.IsSelected(itemData);
			this.SetVillagerNeedMark(itemView, true, !isSelected);
		}
		else
		{
			itemView.SetSelectedOrder(false, 0);
			itemView.HideInteractionState();
			itemView.SetClickEvent(delegate
			{
				this._inventoryScroll.HandleClickItem(itemData, itemView, new Action<ItemView>(base.<OnRenderInventoryItem>g__Action|1));
			});
			this.SetVillagerNeedMark(itemView, true, true);
		}
	}

	// Token: 0x06003985 RID: 14725 RVA: 0x001D3BF0 File Offset: 0x001D1DF0
	private void SetVillagerNeedMark(ItemView itemView, bool isInventory, bool showIcon)
	{
		bool sourceTypeIsMeet = (isInventory && this._inventoryItemSourceType == ItemSourceType.Treasury) || (!isInventory && this._warehouseItemSourceType == ItemSourceType.Treasury);
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
			itemView.SetVillagerNeedMark(isNeeded, showIcon);
		}
	}

	// Token: 0x06003986 RID: 14726 RVA: 0x001D3C7C File Offset: 0x001D1E7C
	private void SetResourceItemTip(ItemView itemView)
	{
		bool flag = this._taiwuDisplayData == null || !itemView.Data.IsResource;
		if (!flag)
		{
			bool isTaiwu = itemView.Data.ItemSourceTypeEnum == ItemSourceType.Resources;
			string charName = isTaiwu ? NameCenter.GetMonasticTitleOrDisplayName(this._taiwuDisplayData, true) : UI_Warehouse.GetTitle(itemView.Data.ItemSourceTypeEnum, false);
			itemView.SetResourceTip(charName, isTaiwu);
		}
	}

	// Token: 0x06003987 RID: 14727 RVA: 0x001D3CE8 File Offset: 0x001D1EE8
	private void OnDragStart(object dragObj)
	{
		ItemView itemView = dragObj as ItemView;
		DragDrop dropTarget = (itemView.UserString == "warehouse") ? this._dropToInventory : this._dropToWarehouse;
		dropTarget.transform.GetChild(0).gameObject.SetActive(false);
		dropTarget.gameObject.SetActive(true);
		bool flag = itemView.Data.Key.ItemType == 11;
		if (flag)
		{
			UIElement.DragShow.OnShowed = delegate()
			{
				CricketView cricket = UIElement.DragShow.UiBaseAs<UI_DragShow>().DragItem.GetComponentInChildren<CricketView>();
				cricket.Inited = false;
				cricket.SetCricketData(itemView.Data.CricketColorId, itemView.Data.CricketPartId, false, null, false);
			};
		}
	}

	// Token: 0x06003988 RID: 14728 RVA: 0x001D3D88 File Offset: 0x001D1F88
	private void OnDrop(object dragObj, object dropToObj)
	{
		ItemView itemView = dragObj as ItemView;
		string dropTarget = dropToObj as string;
		bool flag = dropTarget != null;
		if (flag)
		{
			bool flag2 = dropTarget == "inventory";
			if (flag2)
			{
				this.TakeFromWarehouse(itemView);
			}
			else
			{
				this.PutIntoWarehouse(itemView);
			}
		}
		this._dropToWarehouse.gameObject.SetActive(false);
		this._dropToInventory.gameObject.SetActive(false);
	}

	// Token: 0x06003989 RID: 14729 RVA: 0x001D3DF4 File Offset: 0x001D1FF4
	private void TakeFromWarehouse(ItemView itemView)
	{
		ItemDisplayData itemData = itemView.Data;
		bool flag = itemData.Amount > 1;
		if (flag)
		{
			ValueTuple<Transform, int> valueTuple = this.HighLightItemView(itemView, base.CGet<RectTransform>("WarehouseFocusItemMask"));
			Transform itemViewOriginParent = valueTuple.Item1;
			int itemViewOriginSibling = valueTuple.Item2;
			this._warehouseScroll.SetItemToSelectCountMode(this._warehouseScroll.SortAndFilter.OutputItemList.IndexOf(itemData), itemView, delegate(int count)
			{
				this.ExitHighLight();
				bool flag2 = count <= 0;
				if (!flag2)
				{
					this.TakeFromWarehouse(itemData, count);
				}
			}, new Action(this.ExitHighLight), 0, 0, 1, null, false, null, itemViewOriginParent, itemViewOriginSibling);
		}
		else
		{
			this.TakeFromWarehouse(itemData, 1);
		}
	}

	// Token: 0x0600398A RID: 14730 RVA: 0x001D3EA8 File Offset: 0x001D20A8
	private void TakeFromWarehouse(ItemDisplayData item, int amount)
	{
		AudioManager.Instance.PlaySound("ui_caravan_move", false, false);
		List<ItemDisplayData> fromItems = this.CurWarehouseItems;
		List<ItemDisplayData> toItems = this.CurInventoryItems;
		int inventoryIndex = toItems.FindIndex((ItemDisplayData data) => data.CanMerge(item));
		bool isMiscResource = ItemTemplateHelper.IsMiscResource(item.Key.ItemType, item.Key.TemplateId);
		bool flag = isMiscResource;
		if (flag)
		{
			sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(item.Key.ItemType, item.Key.TemplateId);
			bool flag2 = item.Amount > amount;
			if (flag2)
			{
				item.Amount -= amount;
			}
			else
			{
				fromItems.Remove(item);
			}
			bool flag3 = inventoryIndex >= 0;
			if (flag3)
			{
				toItems[inventoryIndex].Amount += amount;
			}
			else
			{
				ItemDisplayData newItem = item.Clone(amount);
				newItem.ItemSourceType = this._inventoryItemSourceType.ToSbyte();
				toItems.Add(newItem);
			}
			TaiwuDomainMethod.Call.TransferResource(this.Element.GameDataListenerId, this._warehouseItemSourceType, this._inventoryItemSourceType, resourceType, amount);
		}
		this.RefreshWarehouseItems(false);
		this.RefreshInventoryItems();
	}

	// Token: 0x0600398B RID: 14731 RVA: 0x001D4010 File Offset: 0x001D2210
	private void PutIntoWarehouse(ItemView itemView)
	{
		ItemDisplayData itemData = itemView.Data;
		bool flag = itemData.Amount > 1;
		if (flag)
		{
			ValueTuple<Transform, int> valueTuple = this.HighLightItemView(itemView, base.CGet<RectTransform>("InventoryFocusItemMask"));
			Transform itemViewOriginParent = valueTuple.Item1;
			int itemViewOriginSibling = valueTuple.Item2;
			this._inventoryScroll.SetItemToSelectCountMode(this._inventoryScroll.MySortAndFilter.OutputItemList.IndexOf(itemData), itemView, delegate(int count)
			{
				this.ExitHighLight();
				bool flag2 = count <= 0;
				if (!flag2)
				{
					this.PutIntoWarehouse(itemData, count);
				}
			}, new Action(this.ExitHighLight), 0, 0, 1, null, false, null, itemViewOriginParent, itemViewOriginSibling);
		}
		else
		{
			this.PutIntoWarehouse(itemData, 1);
		}
	}

	// Token: 0x0600398C RID: 14732 RVA: 0x001D40C4 File Offset: 0x001D22C4
	private void PutIntoWarehouse(ItemDisplayData item, int amount)
	{
		AudioManager.Instance.PlaySound("ui_caravan_move", false, false);
		List<ItemDisplayData> fromItems = this.CurInventoryItems;
		List<ItemDisplayData> toItems = this.CurWarehouseItems;
		int warehouseIndex = toItems.FindIndex((ItemDisplayData data) => data.CanMerge(item));
		bool isMiscResource = ItemTemplateHelper.IsMiscResource(item.Key.ItemType, item.Key.TemplateId);
		bool flag = isMiscResource;
		if (flag)
		{
			sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(item.Key.ItemType, item.Key.TemplateId);
			bool flag2 = item.Amount > amount;
			if (flag2)
			{
				item.Amount -= amount;
			}
			else
			{
				fromItems.Remove(item);
			}
			bool flag3 = warehouseIndex >= 0;
			if (flag3)
			{
				toItems[warehouseIndex].Amount += amount;
			}
			else
			{
				ItemDisplayData newItem = item.Clone(amount);
				newItem.ItemSourceType = this._warehouseItemSourceType.ToSbyte();
				toItems.Add(newItem);
			}
			TaiwuDomainMethod.Call.TransferResource(this.Element.GameDataListenerId, this._inventoryItemSourceType, this._warehouseItemSourceType, resourceType, amount);
		}
		this.RefreshWarehouseItems(false);
		this.RefreshInventoryItems();
	}

	// Token: 0x0600398D RID: 14733 RVA: 0x001D422C File Offset: 0x001D242C
	[return: TupleElementNames(new string[]
	{
		"originParent",
		"originSibling"
	})]
	private ValueTuple<Transform, int> HighLightItemView(ItemView itemView, RectTransform mask)
	{
		bool flag = null == itemView;
		ValueTuple<Transform, int> result;
		if (flag)
		{
			result = new ValueTuple<Transform, int>(null, -1);
		}
		else
		{
			this._focusingTuple.Item1 = itemView;
			this._focusingTuple.Item2 = itemView.transform.parent;
			this._focusingTuple.Item3 = itemView.transform.GetSiblingIndex();
			this._focusingTuple.Item4 = mask;
			Transform originParent = itemView.transform.parent;
			int originSibling = itemView.transform.GetSiblingIndex();
			itemView.transform.SetParent(mask, true);
			mask.gameObject.SetActive(true);
			result = new ValueTuple<Transform, int>(originParent, originSibling);
		}
		return result;
	}

	// Token: 0x0600398E RID: 14734 RVA: 0x001D42D4 File Offset: 0x001D24D4
	public void ExitHighLight()
	{
		this._focusingTuple.Item1.transform.SetParent(this._focusingTuple.Item2, true);
		this._focusingTuple.Item1.transform.SetSiblingIndex(this._focusingTuple.Item3);
		this._focusingTuple.Item1 = null;
		this._focusingTuple.Item4.gameObject.SetActive(false);
	}

	// Token: 0x0600398F RID: 14735 RVA: 0x001D4348 File Offset: 0x001D2548
	private void TransferAllToWarehouse()
	{
	}

	// Token: 0x06003990 RID: 14736 RVA: 0x001D434B File Offset: 0x001D254B
	private void TransferAllToInventory()
	{
	}

	// Token: 0x06003991 RID: 14737 RVA: 0x001D434E File Offset: 0x001D254E
	private void RefreshBtnInteractable()
	{
		this.<RefreshBtnInteractable>g__RefreshBtn|85_0(true);
		this.<RefreshBtnInteractable>g__RefreshBtn|85_0(false);
	}

	// Token: 0x06003992 RID: 14738 RVA: 0x001D4364 File Offset: 0x001D2564
	private bool CheckCanTransfer(ItemDisplayData d, bool isInventory)
	{
		bool flag = !isInventory;
		bool result;
		if (flag)
		{
			result = (this._canTransfer && !this._multiplyItemScrollView.CheckItemHasGradeLimit(d.Key, false));
		}
		else
		{
			bool canFeed = true;
			bool flag2 = this._warehouseItemSourceType == ItemSourceType.Trough;
			if (flag2)
			{
				canFeed = GameData.Domains.Building.SharedMethods.CheckItemCanFeedChicken(d.Key);
			}
			bool isUsed = (ItemType.IsEquipmentItemType(d.Key.ItemType) || d.Key.ItemType == 10) && d.UsingType != ItemDisplayData.ItemUsingType.Invalid;
			bool isLocked = this.CheckItemIsLocked(d, true) || isUsed;
			bool isGradeLimited = this._multiplyItemScrollView.CheckItemHasGradeLimit(d.Key, true);
			result = (canFeed && !isLocked && this._canTransfer && !isGradeLimited);
		}
		return result;
	}

	// Token: 0x06003993 RID: 14739 RVA: 0x001D442C File Offset: 0x001D262C
	private bool CheckItemIsLocked(ItemDisplayData itemData, bool isInventory)
	{
		bool flag = !this._canTransfer;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			ItemSourceType type = isInventory ? this._warehouseItemSourceType : this._inventoryItemSourceType;
			short taiwuAge = this._taiwuDisplayData.AvatarRelatedData.DisplayAge;
			result = !GameData.Domains.Taiwu.VillagerRole.SharedMethods.CheckCanStoreItem(type, itemData, (int)taiwuAge);
		}
		return result;
	}

	// Token: 0x06003994 RID: 14740 RVA: 0x001D447D File Offset: 0x001D267D
	private void OnItemGradeFilterSettingChange(ArgumentBox argumentBox)
	{
		this.RefreshBtnInteractable();
	}

	// Token: 0x06003995 RID: 14741 RVA: 0x001D4488 File Offset: 0x001D2688
	public static string GetTitle(ItemSourceType type, bool full = true)
	{
		if (!true)
		{
		}
		string result;
		switch (type)
		{
		case ItemSourceType.Inventory:
			result = LocalStringManager.Get(LanguageKey.LK_Inventory);
			goto IL_AC;
		case ItemSourceType.Warehouse:
			result = LocalStringManager.Get(LanguageKey.LK_Warehouse);
			goto IL_AC;
		case ItemSourceType.Treasury:
			result = LocalStringManager.Get(LanguageKey.LK_Treasury);
			goto IL_AC;
		case ItemSourceType.Trough:
			result = LocalStringManager.Get(LanguageKey.LK_Trough);
			goto IL_AC;
		case ItemSourceType.Stock:
			result = (full ? (LocalStringManager.Get(LanguageKey.LK_StockStorage) + " - " + LocalStringManager.Get(LanguageKey.LK_StockStorageGoodsShelf)) : LocalStringManager.Get(LanguageKey.LK_StockStorage));
			goto IL_AC;
		case ItemSourceType.Resources:
			result = LocalStringManager.Get(LanguageKey.LK_Inventory);
			goto IL_AC;
		}
		result = string.Empty;
		IL_AC:
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06003996 RID: 14742 RVA: 0x001D454C File Offset: 0x001D274C
	private ItemSourceType GetSubTogItemSourceType(UI_Warehouse.WarehouseTogKey togKey, int index)
	{
		if (!true)
		{
		}
		if (togKey != UI_Warehouse.WarehouseTogKey.StockStorage)
		{
			throw new ArgumentOutOfRangeException("togKey", togKey, null);
		}
		ItemSourceType result = ItemSourceType.Stock;
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06003999 RID: 14745 RVA: 0x001D4678 File Offset: 0x001D2878
	[CompilerGenerated]
	private void <OnInit>g__InitButton|38_1(Refers refers, bool isInventory)
	{
		CButtonObsolete btnTransferAll = refers.CGet<CButtonObsolete>("BtnTransferAll");
		btnTransferAll.gameObject.SetActive(true);
		btnTransferAll.ClearAndAddListener(delegate
		{
			btnTransferAll.interactable = false;
			bool isInventory2 = isInventory;
			if (isInventory2)
			{
				this.TransferAllToWarehouse();
			}
			else
			{
				this.TransferAllToInventory();
			}
		});
	}

	// Token: 0x0600399A RID: 14746 RVA: 0x001D46D8 File Offset: 0x001D28D8
	[CompilerGenerated]
	private void <OnInit>g__OnEnter|38_2()
	{
		Refers refers = this._multiplyItemScrollView.SourceRootRefer;
		refers.CGet<GameObject>("UIMask").SetActive(true);
		refers.CGet<CButtonObsolete>("BtnTransferAll").gameObject.SetActive(false);
		refers.transform.SetSiblingIndex(1);
	}

	// Token: 0x0600399B RID: 14747 RVA: 0x001D4728 File Offset: 0x001D2928
	[CompilerGenerated]
	private void <OnInit>g__OnExit|38_3()
	{
		Refers refers = this._multiplyItemScrollView.SourceRootRefer;
		refers.CGet<GameObject>("UIMask").SetActive(false);
		refers.CGet<CButtonObsolete>("BtnTransferAll").gameObject.SetActive(true);
	}

	// Token: 0x060039A1 RID: 14753 RVA: 0x001D47F4 File Offset: 0x001D29F4
	[CompilerGenerated]
	private void <OnInventoryItemTogChange>g__HandleConflict|50_0(UI_Warehouse.WarehouseTogKey warehouseTogKey, UI_Warehouse.InventoryTogKey inventoryTogKey, ref UI_Warehouse.<>c__DisplayClass50_0 A_3)
	{
		CToggleObsolete warehouseTog = this._warehouseTogGroup.Get(warehouseTogKey.ToInt());
		warehouseTog.interactable = (A_3.togNew.Key != inventoryTogKey.ToInt());
		warehouseTog.GetComponentsInChildren<MonoJoint>(true).ForEach(delegate(int i, MonoJoint joint)
		{
			joint.JointSync();
			return false;
		});
	}

	// Token: 0x060039A2 RID: 14754 RVA: 0x001D4868 File Offset: 0x001D2A68
	[CompilerGenerated]
	private void <OnWarehouseItemTogChange>g__SubToggleEvent|51_0(bool isOn, int toggleIndex, ref UI_Warehouse.<>c__DisplayClass51_0 A_3)
	{
		bool flag = !isOn;
		if (!flag)
		{
			this._warehouseItemSourceType = this.GetSubTogItemSourceType(A_3.newTogKey, toggleIndex);
			this.<OnWarehouseItemTogChange>g__RefreshScroll|51_1(ref A_3);
		}
	}

	// Token: 0x060039A3 RID: 14755 RVA: 0x001D489B File Offset: 0x001D2A9B
	[CompilerGenerated]
	private void <OnWarehouseItemTogChange>g__RefreshScroll|51_1(ref UI_Warehouse.<>c__DisplayClass51_0 A_1)
	{
		this._multiplyItemScrollView.WarehouseItemSourceType = this._warehouseItemSourceType;
		base.CGet<TextMeshProUGUI>("WarehouseTitle").text = UI_Warehouse.GetTitle(this._warehouseItemSourceType, true);
		this.RefreshInventoryItems();
		this.RefreshWarehouseItems(true);
	}

	// Token: 0x060039A4 RID: 14756 RVA: 0x001D48DC File Offset: 0x001D2ADC
	[CompilerGenerated]
	private void <OnWarehouseItemTogChange>g__HandleConflict|51_2(UI_Warehouse.WarehouseTogKey warehouseTogKey, UI_Warehouse.InventoryTogKey inventoryTogKey, ref UI_Warehouse.<>c__DisplayClass51_0 A_3)
	{
		CToggleObsolete inventoryTog = this._inventoryTogGroup.Get(inventoryTogKey.ToInt());
		inventoryTog.interactable = (A_3.togNew.Key != warehouseTogKey.ToInt());
		inventoryTog.GetComponentsInChildren<MonoJoint>(true).ForEach(delegate(int i, MonoJoint joint)
		{
			joint.JointSync();
			return false;
		});
	}

	// Token: 0x060039A5 RID: 14757 RVA: 0x001D4950 File Offset: 0x001D2B50
	[CompilerGenerated]
	private void <RefreshBtnInteractable>g__RefreshBtn|85_0(bool isInventory)
	{
		Refers refer = base.CGet<Refers>(isInventory ? "Inventory" : "Warehouse");
		ItemSortAndFilter sortAndFilter = isInventory ? this._inventoryScroll.MySortAndFilter : this._warehouseScroll.MySortAndFilter;
		int lockedCount = sortAndFilter.OutputItemList.Count((ItemDisplayData d) => !this.CheckCanTransfer(d, isInventory) || d.IsResource);
		CButtonObsolete btnTransferAll = refer.CGet<CButtonObsolete>("BtnTransferAll");
		btnTransferAll.interactable = (sortAndFilter.OutputItemList.Count - lockedCount > 0 && this._canTransfer);
		MonoJoint componentInChildren = btnTransferAll.GetComponentInChildren<MonoJoint>(true);
		if (componentInChildren != null)
		{
			componentInChildren.JointSync();
		}
		TooltipInvoker tip = btnTransferAll.GetComponent<TooltipInvoker>();
		StringBuilder sb = EasyPool.Get<StringBuilder>();
		sb.AppendLine(LocalStringManager.Get(isInventory ? LanguageKey.LK_Warehouse_Add_All_Tip : LanguageKey.LK_Warehouse_Remove_All_Tip));
		bool flag = !btnTransferAll.interactable;
		if (flag)
		{
			sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_Warehouse_Cannot_Transfer_All).ColorReplace());
		}
		ItemGradeFilterSetting setting = SingletonObject.getInstance<GameSort>().GetItemGradeFilterSetting();
		ItemSourceType itemSourceType = isInventory ? this._inventoryItemSourceType : this._warehouseItemSourceType;
		ItemGradeFilterSetting.ItemGradeFilterSourceType type = ItemGradeFilterSetting.GetItemGradeFilterSourceType(itemSourceType, itemSourceType == ItemSourceType.Inventory, false);
		sbyte grade = setting.GetGrade(type);
		string gradeName = CommonUtils.GetItemGradeShortNameWithMoreThan((int)grade);
		sb.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_Warehouse_Current_Grade_Limit, gradeName));
		bool flag2 = tip.PresetParam == null || tip.PresetParam.Length < 1;
		if (flag2)
		{
			tip.PresetParam = new string[1];
		}
		tip.PresetParam[0] = sb.ToString();
		EasyPool.Free<StringBuilder>(sb);
		bool flag3 = !this._multiplyItemScrollView.IsMultiItemSelect;
		if (flag3)
		{
			CButtonObsolete btnMultiplySelect = refer.CGet<CButtonObsolete>("BtnMultiplySelect");
			this._multiplyItemScrollView.ShowMultiplySelectButton(btnMultiplySelect, this._canTransfer);
		}
	}

	// Token: 0x04002982 RID: 10626
	private readonly Dictionary<ItemSourceType, List<ItemDisplayData>> _itemDict = new Dictionary<ItemSourceType, List<ItemDisplayData>>();

	// Token: 0x04002983 RID: 10627
	private bool _canTransfer;

	// Token: 0x04002984 RID: 10628
	private CharacterDisplayData _taiwuDisplayData;

	// Token: 0x04002985 RID: 10629
	private int _wareHouseCurrLoad;

	// Token: 0x04002986 RID: 10630
	private int _wareHouseMaxLoad;

	// Token: 0x04002987 RID: 10631
	private int _troughCurrLoad;

	// Token: 0x04002988 RID: 10632
	private int _troughMaxLoad;

	// Token: 0x04002989 RID: 10633
	private int _inventoryCurrLoad;

	// Token: 0x0400298A RID: 10634
	private int _inventoryMaxLoad;

	// Token: 0x0400298B RID: 10635
	private GroupedItemScrollView _warehouseScroll;

	// Token: 0x0400298C RID: 10636
	private ItemScrollView _inventoryScroll;

	// Token: 0x0400298D RID: 10637
	private DragDrop _dropToWarehouse;

	// Token: 0x0400298E RID: 10638
	private DragDrop _dropToInventory;

	// Token: 0x0400298F RID: 10639
	[TupleElementNames(new string[]
	{
		"focusingItemView",
		"parent",
		"sibling",
		"mask"
	})]
	private ValueTuple<ItemView, Transform, int, RectTransform> _focusingTuple;

	// Token: 0x04002990 RID: 10640
	private List<ItemKey> _villagerNeededItemSet = new List<ItemKey>();

	// Token: 0x04002991 RID: 10641
	private CToggleGroupObsolete _inventoryTogGroup;

	// Token: 0x04002992 RID: 10642
	private CToggleGroupObsolete _warehouseTogGroup;

	// Token: 0x04002993 RID: 10643
	private SubTogGroup _subTogGroup;

	// Token: 0x04002994 RID: 10644
	private ItemSourceType _inventoryItemSourceType = ItemSourceType.Inventory;

	// Token: 0x04002995 RID: 10645
	private ItemSourceType _warehouseItemSourceType = ItemSourceType.Warehouse;

	// Token: 0x04002996 RID: 10646
	private readonly List<ItemSourceType> _warehouseNeedItemSourceTypeList = new List<ItemSourceType>
	{
		ItemSourceType.Resources,
		ItemSourceType.Equipment,
		ItemSourceType.Inventory,
		ItemSourceType.Warehouse,
		ItemSourceType.Treasury,
		ItemSourceType.Stock
	};

	// Token: 0x04002997 RID: 10647
	private readonly List<ItemSourceType> _troughNeedItemSourceTypeList = new List<ItemSourceType>
	{
		ItemSourceType.Equipment,
		ItemSourceType.Inventory,
		ItemSourceType.Warehouse,
		ItemSourceType.Treasury,
		ItemSourceType.Trough
	};

	// Token: 0x04002998 RID: 10648
	private int _taiwuCharId;

	// Token: 0x04002999 RID: 10649
	private MultiplyItemScrollView _multiplyItemScrollView;

	// Token: 0x0400299A RID: 10650
	[SerializeField]
	private float _shortScrollBackHeight;

	// Token: 0x0400299B RID: 10651
	[SerializeField]
	private float _longScrollBackHeight;

	// Token: 0x0400299C RID: 10652
	[SerializeField]
	private float _shortSpaceHeight;

	// Token: 0x0400299D RID: 10653
	[SerializeField]
	private float _longBackHeight;

	// Token: 0x0400299E RID: 10654
	private bool _callTriggerListner;

	// Token: 0x02001837 RID: 6199
	private enum InventoryTogKey
	{
		// Token: 0x0400ADFD RID: 44541
		Inventory,
		// Token: 0x0400ADFE RID: 44542
		Warehouse,
		// Token: 0x0400ADFF RID: 44543
		Treasury
	}

	// Token: 0x02001838 RID: 6200
	private enum WarehouseTogKey
	{
		// Token: 0x0400AE01 RID: 44545
		Warehouse,
		// Token: 0x0400AE02 RID: 44546
		Treasury,
		// Token: 0x0400AE03 RID: 44547
		StockStorage,
		// Token: 0x0400AE04 RID: 44548
		Trough,
		// Token: 0x0400AE05 RID: 44549
		[Obsolete]
		CraftStorage,
		// Token: 0x0400AE06 RID: 44550
		[Obsolete]
		MedicineStorage,
		// Token: 0x0400AE07 RID: 44551
		[Obsolete]
		FoodStorage
	}

	// Token: 0x02001839 RID: 6201
	private enum EWarehouseItemGroup
	{
		// Token: 0x0400AE09 RID: 44553
		Resource,
		// Token: 0x0400AE0A RID: 44554
		Tool,
		// Token: 0x0400AE0B RID: 44555
		Medicine,
		// Token: 0x0400AE0C RID: 44556
		Poison,
		// Token: 0x0400AE0D RID: 44557
		Other
	}
}
