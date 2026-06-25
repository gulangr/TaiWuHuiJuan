using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using Config.ConfigCells;
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

// Token: 0x02000368 RID: 872
public class UI_AddCraftResource : UIBase
{
	// Token: 0x17000578 RID: 1400
	// (get) Token: 0x06003267 RID: 12903 RVA: 0x0018D903 File Offset: 0x0018BB03
	private Refers _inventory
	{
		get
		{
			return base.CGet<Refers>("Inventory");
		}
	}

	// Token: 0x17000579 RID: 1401
	// (get) Token: 0x06003268 RID: 12904 RVA: 0x0018D910 File Offset: 0x0018BB10
	private MultiplyItemScrollView _multiplyItemScrollView
	{
		get
		{
			return base.CGet<MultiplyItemScrollView>("MultiplyItemScrollView");
		}
	}

	// Token: 0x1700057A RID: 1402
	// (get) Token: 0x06003269 RID: 12905 RVA: 0x0018D91D File Offset: 0x0018BB1D
	private SubTogGroup _subTogGroup
	{
		get
		{
			return base.CGet<SubTogGroup>("SubTitleGroup");
		}
	}

	// Token: 0x1700057B RID: 1403
	// (get) Token: 0x0600326A RID: 12906 RVA: 0x0018D92A File Offset: 0x0018BB2A
	private TextMeshProUGUI _inventoryLoadTips
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("InventoryLoadTips");
		}
	}

	// Token: 0x1700057C RID: 1404
	// (get) Token: 0x0600326B RID: 12907 RVA: 0x0018D937 File Offset: 0x0018BB37
	private ItemScrollView _inventoryScroll
	{
		get
		{
			return base.CGet<ItemScrollView>("InventoryItemScroll");
		}
	}

	// Token: 0x1700057D RID: 1405
	// (get) Token: 0x0600326C RID: 12908 RVA: 0x0018D944 File Offset: 0x0018BB44
	private CToggleGroupObsolete _inventoryTogGroup
	{
		get
		{
			return base.CGet<CToggleGroupObsolete>("InventoryTogGroup");
		}
	}

	// Token: 0x1700057E RID: 1406
	// (get) Token: 0x0600326D RID: 12909 RVA: 0x0018D951 File Offset: 0x0018BB51
	private RectTransform _scrollBack
	{
		get
		{
			return base.CGet<RectTransform>("ScrollBack");
		}
	}

	// Token: 0x1700057F RID: 1407
	// (get) Token: 0x0600326E RID: 12910 RVA: 0x0018D95E File Offset: 0x0018BB5E
	private RectTransform _space
	{
		get
		{
			return base.CGet<RectTransform>("Space");
		}
	}

	// Token: 0x17000580 RID: 1408
	// (get) Token: 0x0600326F RID: 12911 RVA: 0x0018D96B File Offset: 0x0018BB6B
	private TextMeshProUGUI _inventoryLoad
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("InventoryLoad");
		}
	}

	// Token: 0x17000581 RID: 1409
	// (get) Token: 0x06003270 RID: 12912 RVA: 0x0018D978 File Offset: 0x0018BB78
	private TooltipInvoker _inventoryOverflowTIps
	{
		get
		{
			return base.CGet<TooltipInvoker>("InventoryOverflowTIps");
		}
	}

	// Token: 0x17000582 RID: 1410
	// (get) Token: 0x06003271 RID: 12913 RVA: 0x0018D985 File Offset: 0x0018BB85
	private GameObject _loadBackRight
	{
		get
		{
			return base.CGet<GameObject>("LoadBackRight");
		}
	}

	// Token: 0x17000583 RID: 1411
	// (get) Token: 0x06003272 RID: 12914 RVA: 0x0018D992 File Offset: 0x0018BB92
	private TextMeshProUGUI _inventoryTitle
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("InventoryTitle");
		}
	}

	// Token: 0x17000584 RID: 1412
	// (get) Token: 0x06003273 RID: 12915 RVA: 0x0018D99F File Offset: 0x0018BB9F
	private RectTransform _tempHolder
	{
		get
		{
			return base.CGet<RectTransform>("TempHolder");
		}
	}

	// Token: 0x17000585 RID: 1413
	// (get) Token: 0x06003274 RID: 12916 RVA: 0x0018D9AC File Offset: 0x0018BBAC
	private CToggleObsolete _inventoryToggle
	{
		get
		{
			return base.CGet<CToggleObsolete>("InventoryToggle");
		}
	}

	// Token: 0x17000586 RID: 1414
	// (get) Token: 0x06003275 RID: 12917 RVA: 0x0018D9B9 File Offset: 0x0018BBB9
	private CToggleObsolete _warehouseToggle
	{
		get
		{
			return base.CGet<CToggleObsolete>("WarehouseToggle");
		}
	}

	// Token: 0x17000587 RID: 1415
	// (get) Token: 0x06003276 RID: 12918 RVA: 0x0018D9C6 File Offset: 0x0018BBC6
	private CToggleGroupObsolete _inventoryFilterGroup
	{
		get
		{
			return this._inventoryScroll.CGet<ItemSortAndFilter>("ItemSortAndFilter").CGet<CToggleGroupObsolete>("Filter");
		}
	}

	// Token: 0x17000588 RID: 1416
	// (get) Token: 0x06003277 RID: 12919 RVA: 0x0018D9E2 File Offset: 0x0018BBE2
	private List<ItemSourceType> CurNeedItemSourceTypeList
	{
		get
		{
			return this._warehouseNeedItemSourceTypeList;
		}
	}

	// Token: 0x17000589 RID: 1417
	// (get) Token: 0x06003278 RID: 12920 RVA: 0x0018D9EA File Offset: 0x0018BBEA
	private List<ItemDisplayData> CurInventoryItems
	{
		get
		{
			return this._itemDict[this._itemSourceType];
		}
	}

	// Token: 0x1700058A RID: 1418
	// (get) Token: 0x06003279 RID: 12921 RVA: 0x0018D9FD File Offset: 0x0018BBFD
	private bool dataReady
	{
		get
		{
			return this._canTransferReady;
		}
	}

	// Token: 0x0600327A RID: 12922 RVA: 0x0018DA08 File Offset: 0x0018BC08
	public override void OnInit(ArgumentBox argsBox)
	{
		this._canTransferReady = false;
		this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		this._resourceTypeData = null;
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		this.<OnInit>g__InitButton|70_0(this._inventory, true);
		this._multiplyItemScrollView.Init(this._taiwuCharId, this._itemDict, new Action(UI_AddCraftResource.<OnInit>g__OnEnter|70_1), new Action(this.<OnInit>g__OnExit|70_2), null, true);
		this._multiplyItemScrollView.OpenMultiOperation = false;
		this._multiplyItemScrollView.ReSelectAmountWhenSelected = true;
		bool flag = argsBox == null || !argsBox.Get("CallTriggerListner", out this._callTriggerListner);
		if (flag)
		{
			this._callTriggerListner = true;
		}
		this._subTogGroup.gameObject.SetActive(false);
		foreach (ItemSourceType itemSourceType in this.CurNeedItemSourceTypeList)
		{
			List<ItemDisplayData> list;
			bool flag2 = this._itemDict.TryGetValue(itemSourceType, out list);
			if (flag2)
			{
				list.Clear();
			}
			else
			{
				this._itemDict.Add(itemSourceType, new List<ItemDisplayData>());
			}
		}
		argsBox.Get("BuildingCraftPanel", out this._buildingCraftPanel);
		argsBox.Get<Transform>("FocusTransform", out this._focusTransform);
		argsBox.Get<ArtisanOrder>("ArtisanOrder", out this._artisanOrder);
		argsBox.Get("ResourceType", out this._resourceType);
		argsBox.Get<Action>("OnClose", out this._onClose);
		int maxProgress = GameData.Domains.Building.SharedMethods.MaxProductionProgress(this._buildingCraftPanel);
		this._need = maxProgress - this._artisanOrder.Progress;
		this.GetArtisanOrderProductionSet(this._artisanOrder);
		bool flag3 = this._focusTransform != null;
		if (flag3)
		{
			this._focusTransformOriginalParent = this._focusTransform.parent;
			this._focusTransform.parent = this._tempHolder;
		}
	}

	// Token: 0x0600327B RID: 12923 RVA: 0x0018DC1C File Offset: 0x0018BE1C
	private void RefreshInventoryToggles()
	{
		bool flag = !this.dataReady;
		if (!flag)
		{
			bool activeInventory = true;
			bool activeWarehouse = true;
			bool buildingCraftPanel = this._buildingCraftPanel;
			if (buildingCraftPanel)
			{
				activeInventory = this._canTransfer;
			}
			else
			{
				activeWarehouse = this._canTransfer;
			}
			this._inventoryTogGroup.SetInteractable(activeInventory, this._inventoryToggle);
			this._inventoryToggle.GetComponent<TooltipInvoker>().enabled = !activeInventory;
			this._inventoryToggle.GetComponent<DisableStyleRoot>().SetStyleEffect(!activeInventory, false);
			bool flag2 = !activeInventory;
			if (flag2)
			{
				this._inventoryTogGroup.Set(this._warehouseToggle, true);
			}
			this._inventoryTogGroup.SetInteractable(activeWarehouse, this._warehouseToggle);
			this._warehouseToggle.GetComponent<TooltipInvoker>().enabled = !activeWarehouse;
			this._warehouseToggle.GetComponent<DisableStyleRoot>().SetStyleEffect(!activeWarehouse, false);
			bool flag3 = !activeWarehouse;
			if (flag3)
			{
				this._inventoryTogGroup.Set(this._inventoryToggle, true);
			}
		}
	}

	// Token: 0x0600327C RID: 12924 RVA: 0x0018DD10 File Offset: 0x0018BF10
	private void Awake()
	{
		string loadTips = LocalStringManager.Get(LanguageKey.LK_Weight) + LocalStringManager.Get(LanguageKey.LK_Colon_Symbol);
		this._inventoryLoadTips.text = loadTips;
		this._inventoryScroll.Init();
		this._inventoryScroll.MySortAndFilter.AllTypeIncludeInactive = true;
		List<ItemDisplayData> curInventoryItems = this.CurInventoryItems;
		this._inventoryScroll.SetItemList(ref curInventoryItems, true, "addresource_warehouse_inventory", this._inventoryScroll.MySortAndFilter.IsDetailView, new Action<ItemDisplayData, ItemView>(this.OnRenderInventoryItem));
		this._inventoryScroll.SortAndFilter.SetItemInteraction = ((ItemDisplayData data) => !this.CheckItemIsLocked(data, true));
		this._inventoryScroll.ItemListChangedAction = new Action(this.RefreshBtnInteractable);
		this._inventoryScroll.InfinityScroll.AddOnScrollEvent(new Action(this.OnScroll));
		this._inventoryTogGroup.InitPreOnToggle(-1);
		this._inventoryTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnInventoryItemTogChange);
		this.RefreshBtnInteractable();
	}

	// Token: 0x0600327D RID: 12925 RVA: 0x0018DE14 File Offset: 0x0018C014
	private void OnDestroy()
	{
		this._inventoryScroll.InfinityScroll.RemoveOnScrollEvent(new Action(this.OnScroll));
	}

	// Token: 0x0600327E RID: 12926 RVA: 0x0018DE34 File Offset: 0x0018C034
	private void OnScroll()
	{
		this._multiplyItemScrollView.HideGradeLimitTip();
	}

	// Token: 0x0600327F RID: 12927 RVA: 0x0018DE44 File Offset: 0x0018C044
	private void OnEnable()
	{
		GEvent.Add(UiEvents.OnAddCraftsmanResource, new GEvent.Callback(this.OnExitCraftsmanFocusMode));
		GEvent.Add(UiEvents.ExitMultiplyOperation, new GEvent.Callback(this._multiplyItemScrollView.OnExitMultiplyOperation));
		GEvent.Add(UiEvents.ItemMultiplyOperationTypeChange, new GEvent.Callback(this._multiplyItemScrollView.OnItemMultiplyOperationTypeChange));
		GEvent.Add(UiEvents.ItemMultiplyOperationConfirm, new GEvent.Callback(this._multiplyItemScrollView.OnItemMultiplyOperationConfirm));
		GEvent.Add(UiEvents.ItemMultiplyOperationCancelSelection, new GEvent.Callback(this._multiplyItemScrollView.OnItemMultiplyOperationCancelSelection));
		GEvent.Add(UiEvents.ItemGradeFilterSettingChange, new GEvent.Callback(this.OnItemGradeFilterSettingChange));
		bool showMoreTog = SingletonObject.getInstance<BasicGameData>().CanShowMoreTogOnWarehouse();
		foreach (CToggleObsolete tog in this._inventoryTogGroup.GetAll())
		{
			bool flag = tog.Key > 1;
			if (flag)
			{
				tog.gameObject.SetActive(showMoreTog);
			}
		}
		bool isLongScroll = true;
		this._scrollBack.sizeDelta = this._scrollBack.sizeDelta.SetY(isLongScroll ? this._longScrollBackHeight : this._shortScrollBackHeight);
		this._space.sizeDelta = this._space.sizeDelta.SetY(isLongScroll ? this._shortSpaceHeight : this._longBackHeight);
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(this._space.parent as RectTransform);
		});
	}

	// Token: 0x06003280 RID: 12928 RVA: 0x0018DFE4 File Offset: 0x0018C1E4
	private void OnExitCraftsmanFocusMode(ArgumentBox argBox)
	{
		this.QuickHide();
	}

	// Token: 0x06003281 RID: 12929 RVA: 0x0018DFF0 File Offset: 0x0018C1F0
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.OnAddCraftsmanResource, new GEvent.Callback(this.OnExitCraftsmanFocusMode));
		GEvent.Remove(UiEvents.ExitMultiplyOperation, new GEvent.Callback(this._multiplyItemScrollView.OnExitMultiplyOperation));
		GEvent.Remove(UiEvents.ItemMultiplyOperationTypeChange, new GEvent.Callback(this._multiplyItemScrollView.OnItemMultiplyOperationTypeChange));
		GEvent.Remove(UiEvents.ItemMultiplyOperationConfirm, new GEvent.Callback(this._multiplyItemScrollView.OnItemMultiplyOperationConfirm));
		GEvent.Remove(UiEvents.ItemMultiplyOperationCancelSelection, new GEvent.Callback(this._multiplyItemScrollView.OnItemMultiplyOperationCancelSelection));
		GEvent.Remove(UiEvents.ItemGradeFilterSettingChange, new GEvent.Callback(this.OnItemGradeFilterSettingChange));
		GEvent.OnEvent(UiEvents.WareHouseOnDisable, null);
		this._inventoryScroll.SaveSortFilterSetting(true);
		bool callTriggerListner = this._callTriggerListner;
		if (callTriggerListner)
		{
			TaiwuEventDomainMethod.Call.TriggerListener("WarehouseShowed", true);
			SingletonObject.getInstance<WorldMapModel>().ChangeTaiwuMoveState(WorldMapModel.MoveState.WaitEventShow);
		}
	}

	// Token: 0x06003282 RID: 12930 RVA: 0x0018E0F0 File Offset: 0x0018C2F0
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

	// Token: 0x06003283 RID: 12931 RVA: 0x0018E19C File Offset: 0x0018C39C
	private void OnListenerIdReady()
	{
		CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, new List<int>
		{
			SingletonObject.getInstance<BasicGameData>().TaiwuCharId
		});
		TaiwuDomainMethod.Call.CanTransferItemToWarehouse(this.Element.GameDataListenerId);
		TaiwuDomainMethod.Call.GetTreasuryNeededItemList(this.Element.GameDataListenerId);
		this._inventoryScroll.SetCharId(this._taiwuCharId);
		this.Refresh();
	}

	// Token: 0x06003284 RID: 12932 RVA: 0x0018E20C File Offset: 0x0018C40C
	private void Refresh()
	{
		foreach (ItemSourceType itemSourceType in this.CurNeedItemSourceTypeList)
		{
			TaiwuDomainMethod.Call.GetAllItems(this.Element.GameDataListenerId, itemSourceType, true);
		}
	}

	// Token: 0x06003285 RID: 12933 RVA: 0x0018E270 File Offset: 0x0018C470
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
				this._multiplyItemScrollView.ExitMultiplyMode();
			}
			else
			{
				this.CloseInternal();
			}
		}
	}

	// Token: 0x06003286 RID: 12934 RVA: 0x0018E2C4 File Offset: 0x0018C4C4
	private void CloseInternal()
	{
		Action onClose = this._onClose;
		if (onClose != null)
		{
			onClose();
		}
		bool flag = this._focusTransform != null;
		if (flag)
		{
			this._focusTransform.parent = this._focusTransformOriginalParent;
		}
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x06003287 RID: 12935 RVA: 0x0018E32C File Offset: 0x0018C52C
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
							this._canTransferReady = true;
							this.RefreshInventoryToggles();
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
												bool flag9 = resourceType == this._resourceType;
												if (flag9)
												{
													this._resourceTypeData = itemData;
												}
											}
										}
										this.RefreshInventoryItems();
										this.Element.ShowAfterRefresh();
									}
								}
							}
						}
					}
					else
					{
						bool flag10 = notification.DomainId == 4;
						if (flag10)
						{
							bool flag11 = notification.MethodId == 48;
							if (flag11)
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
				bool flag12 = uid.DomainId == 4;
				if (flag12)
				{
					bool flag13 = uid.SubId1 == 104U;
					if (flag13)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._inventoryCurrLoad);
					}
					else
					{
						bool flag14 = uid.SubId1 == 103U;
						if (flag14)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._inventoryMaxLoad);
						}
					}
					string currLoad = ((float)this._inventoryCurrLoad / 100f).ToString("f1").SetColor(CommonUtils.GetLoadWeightValueColor(this._inventoryCurrLoad, this._inventoryMaxLoad));
					this._inventoryLoad.text = string.Format("{0}/{1:f1}", currLoad, (float)this._inventoryMaxLoad / 100f);
					this._inventoryOverflowTIps.enabled = (this._inventoryCurrLoad > this._inventoryMaxLoad);
				}
				else
				{
					bool flag15 = uid.DomainId == 5;
					if (flag15)
					{
						ushort dataId = uid.DataId;
						bool flag16 = dataId == 8 || dataId == 7 || dataId == 69 || dataId == 70;
						if (flag16)
						{
							bool flag17 = uid.DataId == 8;
							if (flag17)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._wareHouseCurrLoad);
							}
							else
							{
								bool flag18 = uid.DataId == 7;
								if (flag18)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._wareHouseMaxLoad);
								}
								else
								{
									bool flag19 = uid.DataId == 69;
									if (flag19)
									{
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._troughMaxLoad);
									}
									else
									{
										bool flag20 = uid.DataId == 70;
										if (flag20)
										{
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._troughCurrLoad);
										}
									}
								}
							}
						}
						else
						{
							bool flag21 = uid.DataId == 22;
							if (flag21)
							{
								int moveTimeCostPercent = 0;
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref moveTimeCostPercent);
								this._inventoryOverflowTIps.PresetParam[1] = LocalStringManager.GetFormat(LanguageKey.LK_Inventory_Overflow_Tips, moveTimeCostPercent - 100).ColorReplace();
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06003288 RID: 12936 RVA: 0x0018E818 File Offset: 0x0018CA18
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "Close";
		if (flag)
		{
			this.QuickHide();
		}
	}

	// Token: 0x06003289 RID: 12937 RVA: 0x0018E848 File Offset: 0x0018CA48
	private void OnInventoryItemTogChange(CToggleObsolete togNew, CToggleObsolete togOld)
	{
		bool flag = !togNew;
		if (!flag)
		{
			UI_AddCraftResource.InventoryTogKey newTogKey = (UI_AddCraftResource.InventoryTogKey)togNew.Key;
			this._loadBackRight.SetActive(newTogKey == UI_AddCraftResource.InventoryTogKey.Inventory);
			if (!true)
			{
			}
			ItemSourceType itemSourceType;
			if (newTogKey != UI_AddCraftResource.InventoryTogKey.Inventory)
			{
				if (newTogKey != UI_AddCraftResource.InventoryTogKey.Warehouse)
				{
					throw new ArgumentOutOfRangeException();
				}
				itemSourceType = ItemSourceType.Warehouse;
			}
			else
			{
				itemSourceType = ItemSourceType.Inventory;
			}
			if (!true)
			{
			}
			this._itemSourceType = itemSourceType;
			this._multiplyItemScrollView.InventoryItemSourceType = this._itemSourceType;
			MonoJoint componentInChildren = togNew.GetComponentInChildren<MonoJoint>(true);
			if (componentInChildren != null)
			{
				componentInChildren.JointSync();
			}
			this._inventoryTitle.text = UI_AddCraftResource.GetTitle(this._itemSourceType, true);
			this.RefreshInventoryItems();
		}
	}

	// Token: 0x0600328A RID: 12938 RVA: 0x0018E8EC File Offset: 0x0018CAEC
	private void RefreshInventoryItems()
	{
		HashSet<short> idSet;
		List<ItemDisplayData> items = (from t in this.CurInventoryItems
		where this._productionPoolDic.TryGetValue(t.Key.ItemType, out idSet) && idSet.Contains(t.Key.TemplateId)
		select t).ToList<ItemDisplayData>();
		bool flag = this._resourceTypeData != null;
		if (flag)
		{
			bool flag2 = this._inventoryToggle.interactable && this._itemSourceType == ItemSourceType.Inventory;
			if (flag2)
			{
				items.Add(this._resourceTypeData);
			}
			else
			{
				bool flag3 = !this._inventoryToggle.interactable && this._itemSourceType == ItemSourceType.Warehouse;
				if (flag3)
				{
					items.Add(this._resourceTypeData);
				}
			}
		}
		this._multiplyItemScrollView.SetCustomItemList(items);
		this._multiplyItemScrollView.EnterPutCraftResourceMode(this._need);
		this._inventoryScroll.SetItemList(ref items, false, null, false, null);
		this.RefreshBtnInteractable();
	}

	// Token: 0x0600328B RID: 12939 RVA: 0x0018E9C4 File Offset: 0x0018CBC4
	private void GroupItemsFoodStorageWarehouse(List<GroupedItemScrollView.ItemGroup> groups, List<ItemDisplayData> items)
	{
		HashSet<ItemDisplayData> sourceSet = new HashSet<ItemDisplayData>(items);
		GroupedItemScrollView.ItemGroup resourceGroup = UI_AddCraftResource.NewResourceGroup();
		this.SelectResourceToGroup(sourceSet, resourceGroup.ItemList);
		groups.Add(resourceGroup);
		GroupedItemScrollView.ItemGroup toolGroup = UI_AddCraftResource.NewToolGroup();
		this.SelectToolToGroup(sourceSet, toolGroup.ItemList);
		groups.Add(toolGroup);
		GroupedItemScrollView.ItemGroup otherGroup = UI_AddCraftResource.NewOtherGroup();
		otherGroup.ItemList = sourceSet.ToList<ItemDisplayData>();
		groups.Add(otherGroup);
	}

	// Token: 0x0600328C RID: 12940 RVA: 0x0018EA2C File Offset: 0x0018CC2C
	private void GroupItemsMedicineStorageWarehouse(List<GroupedItemScrollView.ItemGroup> groups, List<ItemDisplayData> items)
	{
		HashSet<ItemDisplayData> sourceSet = new HashSet<ItemDisplayData>(items);
		GroupedItemScrollView.ItemGroup resourceGroup = UI_AddCraftResource.NewResourceGroup();
		this.SelectResourceToGroup(sourceSet, resourceGroup.ItemList);
		groups.Add(resourceGroup);
		GroupedItemScrollView.ItemGroup toolGroup = UI_AddCraftResource.NewToolGroup();
		this.SelectToolToGroup(sourceSet, toolGroup.ItemList);
		groups.Add(toolGroup);
		GroupedItemScrollView.ItemGroup medicineGroup = new GroupedItemScrollView.ItemGroup(800, LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + LocalStringManager.Get(LanguageKey.LK_ItemSubType_800));
		GroupedItemScrollView.ItemGroup poisonGroup = new GroupedItemScrollView.ItemGroup(801, LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + LocalStringManager.Get(LanguageKey.LK_ItemSubType_801));
		this.SelectItemSubTypeToGroup(sourceSet, medicineGroup.ItemList, 800);
		this.SelectItemSubTypeToGroup(sourceSet, poisonGroup.ItemList, 801);
		groups.Add(medicineGroup);
		groups.Add(poisonGroup);
		GroupedItemScrollView.ItemGroup otherGroup = UI_AddCraftResource.NewOtherGroup();
		otherGroup.ItemList = sourceSet.ToList<ItemDisplayData>();
		groups.Add(otherGroup);
	}

	// Token: 0x0600328D RID: 12941 RVA: 0x0018EB18 File Offset: 0x0018CD18
	private void GroupItemsCraftStorageWarehouse(List<GroupedItemScrollView.ItemGroup> groups, List<ItemDisplayData> items)
	{
		HashSet<ItemDisplayData> sourceSet = new HashSet<ItemDisplayData>(items);
		GroupedItemScrollView.ItemGroup resourceGroup = UI_AddCraftResource.NewResourceGroup();
		this.SelectResourceToGroup(sourceSet, resourceGroup.ItemList);
		groups.Add(resourceGroup);
		GroupedItemScrollView.ItemGroup toolGroup = UI_AddCraftResource.NewToolGroup();
		this.SelectToolToGroup(sourceSet, toolGroup.ItemList);
		groups.Add(toolGroup);
		GroupedItemScrollView.ItemGroup otherGroup = UI_AddCraftResource.NewOtherGroup();
		otherGroup.ItemList = sourceSet.ToList<ItemDisplayData>();
		groups.Add(otherGroup);
	}

	// Token: 0x0600328E RID: 12942 RVA: 0x0018EB80 File Offset: 0x0018CD80
	private void GroupItemsStockStorageWarehouse(List<GroupedItemScrollView.ItemGroup> groups, List<ItemDisplayData> items)
	{
		HashSet<ItemDisplayData> sourceSet = new HashSet<ItemDisplayData>(items);
		GroupedItemScrollView.ItemGroup resourceGroup = UI_AddCraftResource.NewResourceGroup();
		this.SelectResourceToGroup(sourceSet, resourceGroup.ItemList);
		groups.Add(resourceGroup);
		GroupedItemScrollView.ItemGroup otherGroup = UI_AddCraftResource.NewOtherGroup();
		otherGroup.ItemList = sourceSet.ToList<ItemDisplayData>();
		groups.Add(otherGroup);
	}

	// Token: 0x0600328F RID: 12943 RVA: 0x0018EBCC File Offset: 0x0018CDCC
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

	// Token: 0x06003290 RID: 12944 RVA: 0x0018ED28 File Offset: 0x0018CF28
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

	// Token: 0x06003291 RID: 12945 RVA: 0x0018EDF8 File Offset: 0x0018CFF8
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

	// Token: 0x06003292 RID: 12946 RVA: 0x0018EEC0 File Offset: 0x0018D0C0
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

	// Token: 0x06003293 RID: 12947 RVA: 0x0018EF44 File Offset: 0x0018D144
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

	// Token: 0x06003294 RID: 12948 RVA: 0x0018EFC8 File Offset: 0x0018D1C8
	private static GroupedItemScrollView.ItemGroup NewOtherGroup()
	{
		return new GroupedItemScrollView.ItemGroup(4, LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + LocalStringManager.Get(LanguageKey.LK_WarehouseItemGroup_Other));
	}

	// Token: 0x06003295 RID: 12949 RVA: 0x0018EFFC File Offset: 0x0018D1FC
	private static GroupedItemScrollView.ItemGroup NewToolGroup()
	{
		return new GroupedItemScrollView.ItemGroup(1, LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + LocalStringManager.Get(LanguageKey.LK_WarehouseItemGroup_Tool));
	}

	// Token: 0x06003296 RID: 12950 RVA: 0x0018F030 File Offset: 0x0018D230
	private static GroupedItemScrollView.ItemGroup NewResourceGroup()
	{
		return new GroupedItemScrollView.ItemGroup(0, LocalStringManager.Get(LanguageKey.LK_Dot_Symbol) + LocalStringManager.Get(LanguageKey.LK_WarehouseItemGroup_Resource));
	}

	// Token: 0x06003297 RID: 12951 RVA: 0x0018F064 File Offset: 0x0018D264
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

	// Token: 0x06003298 RID: 12952 RVA: 0x0018F0EC File Offset: 0x0018D2EC
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

	// Token: 0x06003299 RID: 12953 RVA: 0x0018F174 File Offset: 0x0018D374
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

	// Token: 0x0600329A RID: 12954 RVA: 0x0018F1F8 File Offset: 0x0018D3F8
	private void OnRenderInventoryItem(ItemDisplayData itemData, ItemView itemView)
	{
		DragDrop itemDrag = itemView.GetComponent<DragDrop>();
		itemView.UserString = "inventory";
		itemView.SetLocked(!itemData.Interactable);
		bool flag = !itemData.Interactable;
		if (flag)
		{
			bool isResource = ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
			bool lockInnerResult = true;
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
		}
		else
		{
			itemView.SetSelectedOrder(false, 0);
			itemView.HideInteractionState();
			itemView.SetClickEvent(delegate
			{
				this._inventoryScroll.HandleClickItem(itemData, itemView, new Action<ItemView>(base.<OnRenderInventoryItem>g__Action|1));
			});
		}
	}

	// Token: 0x0600329B RID: 12955 RVA: 0x0018F3DC File Offset: 0x0018D5DC
	private void SetResourceItemTip(ItemView itemView)
	{
		bool flag = this._taiwuDisplayData == null || !itemView.Data.IsResource;
		if (!flag)
		{
			bool isTaiwu = itemView.Data.ItemSourceTypeEnum == ItemSourceType.Resources;
			string charName = isTaiwu ? NameCenter.GetMonasticTitleOrDisplayName(this._taiwuDisplayData, true) : UI_AddCraftResource.GetTitle(itemView.Data.ItemSourceTypeEnum, false);
			itemView.SetResourceTip(charName, isTaiwu);
		}
	}

	// Token: 0x0600329C RID: 12956 RVA: 0x0018F448 File Offset: 0x0018D648
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

	// Token: 0x0600329D RID: 12957 RVA: 0x0018F4F0 File Offset: 0x0018D6F0
	public void ExitHighLight()
	{
		this._focusingTuple.Item1.transform.SetParent(this._focusingTuple.Item2, true);
		this._focusingTuple.Item1.transform.SetSiblingIndex(this._focusingTuple.Item3);
		this._focusingTuple.Item1 = null;
		this._focusingTuple.Item4.gameObject.SetActive(false);
	}

	// Token: 0x0600329E RID: 12958 RVA: 0x0018F564 File Offset: 0x0018D764
	private void TryPutAllItem()
	{
		this._multiplyItemScrollView.TrySelectItemInValue((ItemDisplayData d) => d.UsingType == ItemDisplayData.ItemUsingType.Invalid && this._multiplyItemScrollView.CheckItemGradeLower(d.Key, true));
	}

	// Token: 0x0600329F RID: 12959 RVA: 0x0018F57F File Offset: 0x0018D77F
	private void RefreshBtnInteractable()
	{
		this.<RefreshBtnInteractable>g__RefreshBtn|107_0(true);
	}

	// Token: 0x060032A0 RID: 12960 RVA: 0x0018F58C File Offset: 0x0018D78C
	private bool CheckCanTransfer(ItemDisplayData d, bool isInventory)
	{
		return this._multiplyItemScrollView.CheckItemGradeLower(d.Key, true);
	}

	// Token: 0x060032A1 RID: 12961 RVA: 0x0018F5B4 File Offset: 0x0018D7B4
	private bool CheckItemIsLocked(ItemDisplayData itemData, bool isInventory)
	{
		bool flag3 = ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
		bool result;
		if (flag3)
		{
			result = false;
		}
		else
		{
			bool flag2 = ItemTemplateHelper.IsTransferable(itemData.Key.ItemType, itemData.Key.TemplateId);
			ItemSourceType itemSourceType = this._itemSourceType;
			if (!true)
			{
			}
			bool flag4 = itemSourceType != ItemSourceType.Inventory && itemSourceType == ItemSourceType.Warehouse && !flag2;
			if (!true)
			{
			}
			result = flag4;
		}
		return result;
	}

	// Token: 0x060032A2 RID: 12962 RVA: 0x0018F634 File Offset: 0x0018D834
	private void OnItemGradeFilterSettingChange(ArgumentBox argumentBox)
	{
		this.RefreshBtnInteractable();
	}

	// Token: 0x060032A3 RID: 12963 RVA: 0x0018F640 File Offset: 0x0018D840
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

	// Token: 0x060032A4 RID: 12964 RVA: 0x0018F704 File Offset: 0x0018D904
	public void GetArtisanOrderProductionSet(ArtisanOrder order)
	{
		this._activeFilterTypes.Clear();
		sbyte lifeSkillType = order.LifeSkillType;
		this._productionPoolDic.Clear();
		sbyte b = lifeSkillType;
		sbyte b2 = b;
		if (b2 != 5)
		{
			if (b2 != 14)
			{
				foreach (MaterialItem materialConfig in ((IEnumerable<MaterialItem>)Config.Material.Instance))
				{
					bool flag = materialConfig.RequiredLifeSkillType != lifeSkillType || materialConfig.IsSpecial;
					if (!flag)
					{
						for (int index = 0; index < materialConfig.CraftableItemTypes.Count; index++)
						{
							short makeItemType = materialConfig.CraftableItemTypes[index];
							foreach (short makeItemSubType in MakeItemType.Instance[makeItemType].MakeItemSubTypes)
							{
								MakeItemResult result = MakeItemSubType.Instance[makeItemSubType].Result;
								bool flag2 = order.ItemSubType < 0 || ItemTemplateHelper.GetItemSubType(result.ItemType, result.TemplateId) == order.ItemSubType;
								if (flag2)
								{
									this.<GetArtisanOrderProductionSet>g__AddToDic|112_0(result.ItemType, this.GetProductionSet(result.ItemType, result.TemplateId, materialConfig.Grade, 0));
								}
							}
						}
					}
				}
			}
			else
			{
				foreach (MaterialItem materialConfig2 in ((IEnumerable<MaterialItem>)Config.Material.Instance))
				{
					bool flag3 = materialConfig2.RequiredLifeSkillType != lifeSkillType || materialConfig2.IsSpecial;
					if (!flag3)
					{
						foreach (short makeItemType2 in materialConfig2.CraftableItemTypes)
						{
							foreach (short makeItemSubType2 in MakeItemType.Instance[makeItemType2].MakeItemSubTypes)
							{
								MakeItemResult result2 = MakeItemSubType.Instance[makeItemSubType2].Result;
								bool flag4 = order.ItemSubType < 0 || ItemTemplateHelper.GetItemSubType(result2.ItemType, result2.TemplateId) == order.ItemSubType;
								if (flag4)
								{
									this.<GetArtisanOrderProductionSet>g__AddToDic|112_0(result2.ItemType, this.GetProductionSet(result2.ItemType, result2.TemplateId, materialConfig2.Grade, 8));
								}
							}
						}
					}
				}
			}
		}
		else
		{
			foreach (TeaWineItem teaWineConfig in ((IEnumerable<TeaWineItem>)TeaWine.Instance))
			{
				bool flag5 = teaWineConfig.ItemSubType == order.ItemSubType || (order.IsArtisanOrder() && order.ItemSubType < 0);
				if (flag5)
				{
					this.<GetArtisanOrderProductionSet>g__AddToDic|112_0(teaWineConfig.ItemType, this.GetProductionSet(teaWineConfig.ItemType, teaWineConfig.TemplateId, teaWineConfig.Grade, 0));
				}
			}
		}
		foreach (CToggleObsolete item in this._inventoryFilterGroup.GetAll())
		{
			item.gameObject.SetActive(item.Key == 0 || this._activeFilterTypes.Contains(item.Key));
		}
	}

	// Token: 0x060032A5 RID: 12965 RVA: 0x0018FB68 File Offset: 0x0018DD68
	private HashSet<short> GetProductionSet(sbyte itemType, short baseTemplateId, sbyte baseGrade, int addOn = 8)
	{
		HashSet<short> result = new HashSet<short>();
		int upperGrade = 2 + addOn;
		for (int i = -1; i < upperGrade; i++)
		{
			short templateId;
			bool flag = !ProductionPool.TryGetProductionTemplateId((sbyte)((int)baseGrade + i), itemType, baseTemplateId, out templateId);
			if (!flag)
			{
				result.Add(templateId);
			}
		}
		return result;
	}

	// Token: 0x060032A7 RID: 12967 RVA: 0x0018FC30 File Offset: 0x0018DE30
	[CompilerGenerated]
	private void <OnInit>g__InitButton|70_0(Refers refers, bool isInventory)
	{
		CButtonObsolete btnTransferAll = refers.CGet<CButtonObsolete>("BtnTransferAll");
		btnTransferAll.gameObject.SetActive(true);
		btnTransferAll.ClearAndAddListener(delegate
		{
			bool isInventory2 = isInventory;
			if (isInventory2)
			{
				this.TryPutAllItem();
			}
		});
	}

	// Token: 0x060032A8 RID: 12968 RVA: 0x0018FC7E File Offset: 0x0018DE7E
	[CompilerGenerated]
	internal static void <OnInit>g__OnEnter|70_1()
	{
	}

	// Token: 0x060032A9 RID: 12969 RVA: 0x0018FC81 File Offset: 0x0018DE81
	[CompilerGenerated]
	private void <OnInit>g__OnExit|70_2()
	{
		this.CloseInternal();
	}

	// Token: 0x060032AD RID: 12973 RVA: 0x0018FCD4 File Offset: 0x0018DED4
	[CompilerGenerated]
	private void <RefreshBtnInteractable>g__RefreshBtn|107_0(bool isInventory)
	{
		Refers refer = this._inventory;
		ItemSortAndFilter sortAndFilter = this._inventoryScroll.MySortAndFilter;
		int lockedCount = sortAndFilter.OutputItemList.Count((ItemDisplayData d) => !this.CheckCanTransfer(d, isInventory) || d.IsResource);
		CButtonObsolete btnTransferAll = refer.CGet<CButtonObsolete>("BtnTransferAll");
		btnTransferAll.interactable = (sortAndFilter.OutputItemList.Count - lockedCount > 0);
		MonoJoint componentInChildren = btnTransferAll.GetComponentInChildren<MonoJoint>(true);
		if (componentInChildren != null)
		{
			componentInChildren.JointSync();
		}
		TooltipInvoker tip = btnTransferAll.GetComponent<TooltipInvoker>();
		StringBuilder sb = EasyPool.Get<StringBuilder>();
		sb.AppendLine(LocalStringManager.Get(LanguageKey.LK_Building_AddCraftResource_Hint));
		ItemGradeFilterSetting setting = SingletonObject.getInstance<GameSort>().GetItemGradeFilterSetting();
		ItemGradeFilterSetting.ItemGradeFilterSourceType type = ItemGradeFilterSetting.GetItemGradeFilterSourceType(this._itemSourceType, this._itemSourceType == ItemSourceType.Inventory, false);
		sbyte grade = setting.GetGrade(type);
		string gradeName = CommonUtils.GetItemGradeShortNameWithMoreThan((int)grade);
		sb.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_Warehouse_Current_Grade_Limit, gradeName));
		bool flag = tip.PresetParam == null || tip.PresetParam.Length < 1;
		if (flag)
		{
			tip.PresetParam = new string[1];
		}
		tip.PresetParam[0] = sb.ToString();
		EasyPool.Free<StringBuilder>(sb);
	}

	// Token: 0x060032AE RID: 12974 RVA: 0x0018FE0C File Offset: 0x0018E00C
	[CompilerGenerated]
	private void <GetArtisanOrderProductionSet>g__AddToDic|112_0(sbyte itemType, HashSet<short> hashSet)
	{
		bool flag = !this._productionPoolDic.ContainsKey(itemType);
		if (flag)
		{
			this._productionPoolDic[itemType] = new HashSet<short>();
		}
		foreach (short item in hashSet)
		{
			this._productionPoolDic[itemType].Add(item);
		}
		bool success = this._activeFilterTypes.Add((int)ItemSortAndFilter.GetFilterType(itemType));
	}

	// Token: 0x040024FB RID: 9467
	private readonly Dictionary<ItemSourceType, List<ItemDisplayData>> _itemDict = new Dictionary<ItemSourceType, List<ItemDisplayData>>();

	// Token: 0x040024FC RID: 9468
	private Dictionary<sbyte, HashSet<short>> _productionPoolDic = new Dictionary<sbyte, HashSet<short>>();

	// Token: 0x040024FD RID: 9469
	private HashSet<int> _activeFilterTypes = new HashSet<int>();

	// Token: 0x040024FE RID: 9470
	private bool _canTransfer;

	// Token: 0x040024FF RID: 9471
	private CharacterDisplayData _taiwuDisplayData;

	// Token: 0x04002500 RID: 9472
	private int _wareHouseCurrLoad;

	// Token: 0x04002501 RID: 9473
	private int _wareHouseMaxLoad;

	// Token: 0x04002502 RID: 9474
	private int _troughCurrLoad;

	// Token: 0x04002503 RID: 9475
	private int _troughMaxLoad;

	// Token: 0x04002504 RID: 9476
	private int _inventoryCurrLoad;

	// Token: 0x04002505 RID: 9477
	private int _inventoryMaxLoad;

	// Token: 0x04002506 RID: 9478
	private ArtisanOrder _artisanOrder;

	// Token: 0x04002507 RID: 9479
	private sbyte _resourceType;

	// Token: 0x04002508 RID: 9480
	private Action _onClose;

	// Token: 0x04002509 RID: 9481
	private int _need;

	// Token: 0x0400250A RID: 9482
	[TupleElementNames(new string[]
	{
		"focusingItemView",
		"parent",
		"sibling",
		"mask"
	})]
	private ValueTuple<ItemView, Transform, int, RectTransform> _focusingTuple;

	// Token: 0x0400250B RID: 9483
	private List<ItemKey> _villagerNeededItemSet = new List<ItemKey>();

	// Token: 0x0400250C RID: 9484
	private ItemSourceType _itemSourceType = ItemSourceType.Inventory;

	// Token: 0x0400250D RID: 9485
	private readonly List<ItemSourceType> _warehouseNeedItemSourceTypeList = new List<ItemSourceType>
	{
		ItemSourceType.Resources,
		ItemSourceType.Inventory,
		ItemSourceType.Warehouse
	};

	// Token: 0x0400250E RID: 9486
	private int _taiwuCharId;

	// Token: 0x0400250F RID: 9487
	[SerializeField]
	private float _shortScrollBackHeight;

	// Token: 0x04002510 RID: 9488
	[SerializeField]
	private float _longScrollBackHeight;

	// Token: 0x04002511 RID: 9489
	[SerializeField]
	private float _shortSpaceHeight;

	// Token: 0x04002512 RID: 9490
	[SerializeField]
	private float _longBackHeight;

	// Token: 0x04002513 RID: 9491
	private bool _callTriggerListner;

	// Token: 0x04002514 RID: 9492
	private Transform _focusTransform;

	// Token: 0x04002515 RID: 9493
	private Transform _focusTransformOriginalParent;

	// Token: 0x04002516 RID: 9494
	private bool _buildingCraftPanel;

	// Token: 0x04002517 RID: 9495
	private ItemDisplayData _resourceTypeData;

	// Token: 0x04002518 RID: 9496
	private bool _canTransferReady = false;

	// Token: 0x02001727 RID: 5927
	private enum InventoryTogKey
	{
		// Token: 0x0400AA95 RID: 43669
		Inventory,
		// Token: 0x0400AA96 RID: 43670
		Warehouse
	}

	// Token: 0x02001728 RID: 5928
	private enum EWarehouseItemGroup
	{
		// Token: 0x0400AA98 RID: 43672
		Resource,
		// Token: 0x0400AA99 RID: 43673
		Tool,
		// Token: 0x0400AA9A RID: 43674
		Medicine,
		// Token: 0x0400AA9B RID: 43675
		Poison,
		// Token: 0x0400AA9C RID: 43676
		Other
	}
}
