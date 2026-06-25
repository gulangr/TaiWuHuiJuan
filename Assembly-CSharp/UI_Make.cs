using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CharacterDataMonitor;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using Game.Components.Avatar;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Organization;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UICommon.Character.Elements;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001BA RID: 442
public class UI_Make : UIBase
{
	// Token: 0x170002BF RID: 703
	// (get) Token: 0x06001A6B RID: 6763 RVA: 0x000AE9F9 File Offset: 0x000ACBF9
	private GameObject ResultPreviewImage
	{
		get
		{
			return base.CGet<GameObject>("Preview");
		}
	}

	// Token: 0x170002C0 RID: 704
	// (get) Token: 0x06001A6C RID: 6764 RVA: 0x000AEA06 File Offset: 0x000ACC06
	private short CurMakeResourceTotalCount
	{
		get
		{
			return (short)Mathf.Clamp(this._curMakeResourceCountInts.GetSum(), 0, (int)this._maxMakeResourceTotalCount);
		}
	}

	// Token: 0x170002C1 RID: 705
	// (get) Token: 0x06001A6D RID: 6765 RVA: 0x000AEA20 File Offset: 0x000ACC20
	private bool NeedLockOnMaking
	{
		get
		{
			return this._workingMakeItemData != null && this._curTab == UI_Make.UIMakeTab.Make;
		}
	}

	// Token: 0x170002C2 RID: 706
	// (get) Token: 0x06001A6E RID: 6766 RVA: 0x000AEA38 File Offset: 0x000ACC38
	private MakeResult CurMakeResult
	{
		get
		{
			MakeResult makeResult;
			this._makeResultDict.TryGetValue((int)(this._makeIsManual ? this._makeItemSubTypeId : -1), out makeResult);
			return makeResult;
		}
	}

	// Token: 0x170002C3 RID: 707
	// (get) Token: 0x06001A6F RID: 6767 RVA: 0x000AEA6A File Offset: 0x000ACC6A
	private List<ItemDisplayData> SlotCacheItemList
	{
		get
		{
			return this._slotItemListDict[this._curSlotItemSourceType];
		}
	}

	// Token: 0x170002C4 RID: 708
	// (get) Token: 0x06001A70 RID: 6768 RVA: 0x000AEA80 File Offset: 0x000ACC80
	private int SlotCount
	{
		get
		{
			UI_Make.UIMakeTab curTab = this._curTab;
			if (!true)
			{
			}
			int result;
			switch (curTab)
			{
			case UI_Make.UIMakeTab.Poison:
				result = 3;
				break;
			case UI_Make.UIMakeTab.RemovePoison:
				result = 3;
				break;
			case UI_Make.UIMakeTab.Refine:
				result = 5;
				break;
			case UI_Make.UIMakeTab.Weave:
				result = 1;
				break;
			default:
				result = 0;
				break;
			}
			if (!true)
			{
			}
			return result;
		}
	}

	// Token: 0x170002C5 RID: 709
	// (get) Token: 0x06001A71 RID: 6769 RVA: 0x000AEACB File Offset: 0x000ACCCB
	private static int MaxSlotCount
	{
		get
		{
			return Math.Max(5, 3);
		}
	}

	// Token: 0x170002C6 RID: 710
	// (get) Token: 0x06001A72 RID: 6770 RVA: 0x000AEAD4 File Offset: 0x000ACCD4
	private ItemSourceChange CurSlotItemChange
	{
		get
		{
			return this._slotItemChangeDict[this._curSlotItemSourceType];
		}
	}

	// Token: 0x170002C7 RID: 711
	// (get) Token: 0x06001A73 RID: 6771 RVA: 0x000AEAE7 File Offset: 0x000ACCE7
	private bool _hasMakeFunction
	{
		get
		{
			return BuildingBlock.Instance[this._blockData.TemplateId].CanMakeItem;
		}
	}

	// Token: 0x170002C8 RID: 712
	// (get) Token: 0x06001A74 RID: 6772 RVA: 0x000AEB03 File Offset: 0x000ACD03
	private bool IsInDoor
	{
		get
		{
			return SingletonObject.getInstance<WorldMapModel>().GetTaiwuCharOnSettlement() > 0;
		}
	}

	// Token: 0x170002C9 RID: 713
	// (get) Token: 0x06001A75 RID: 6773 RVA: 0x000AEB12 File Offset: 0x000ACD12
	private bool CurTabIsAboutPoison
	{
		get
		{
			return this._curTab == UI_Make.UIMakeTab.Poison || this._curTab == UI_Make.UIMakeTab.RemovePoison;
		}
	}

	// Token: 0x06001A76 RID: 6774 RVA: 0x000AEB2C File Offset: 0x000ACD2C
	public override void OnInit(ArgumentBox argsBox)
	{
		this._buildingModel = SingletonObject.getInstance<BuildingModel>();
		this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		argsBox.Get<UI_Make.UIMakeTab>("Tab", out this._curTab);
		argsBox.Get<HashSet<UI_Make.UIMakeTab>>("AllTab", out this._allTabSet);
		argsBox.Get<BuildingBlockKey>("BuildingBlockKey", out this._buildingBlockKey);
		argsBox.Get<BuildingBlockData>("BuildingBlockData", out this._blockData);
		argsBox.Get("LifeSkillType", out this._curLifeSkillType);
		argsBox.Get<ItemKey>("RepairTool", out this._repairToolKey);
		argsBox.Get<ItemKey>("RepairedItem", out this._repairedItemkey);
		short settlementId = SingletonObject.getInstance<WorldMapModel>().GetTaiwuCharOnSettlement();
		this._isSettlement = (settlementId >= 0);
		this._itemViewToolSelected = base.CGet<ItemView>("ItemViewToolSelected");
		this._itemViewTargetSelected = base.CGet<ItemView>("ItemViewTargetSelected");
		this._buttonConfirm = base.CGet<CButtonObsolete>("ButtonConfirm");
		this._confirmNormalLabel = base.CGet<TextMeshProUGUI>("ConfirmNormalLabel");
		this._confirmDisableLabel = base.CGet<TextMeshProUGUI>("ConfirmDisableLabel");
		this._itemViewToolSelected.GetComponent<CButtonObsolete>().ClearAndAddListener(new Action(this.OnClickToolItemView));
		this._itemViewTargetSelected.GetComponent<CButtonObsolete>().ClearAndAddListener(new Action(this.OnClickTargetItemView));
		this._makeRequireResourceList = base.CGet<Refers>("RequireResourceList");
		this.ShowMakeRequireResourceList(false);
		this._makeEffectList = base.CGet<Refers>("EffectList");
		this.ShowMakeEffectList(false);
		this._makeTotalCountLabel = base.CGet<TextMeshProUGUI>("TotalCountLabel");
		this.ShowMakeTotalCount(false);
		for (int i = 0; i < this._makeRequireResourceList.transform.childCount; i++)
		{
			Refers refers = this._makeRequireResourceList.transform.GetChild(i).GetComponent<Refers>();
			CButtonObsolete buttonMore = refers.CGet<CButtonObsolete>("ButtonMore");
			buttonMore.ClearAndAddListener(delegate
			{
				this.ChangeMakeResourceCount(buttonMore, refers.name, true, false, false);
			});
			CButtonObsolete buttonLess = refers.CGet<CButtonObsolete>("ButtonLess");
			buttonLess.ClearAndAddListener(delegate
			{
				this.ChangeMakeResourceCount(buttonLess, refers.name, false, false, false);
			});
			CButtonObsolete buttonMax = refers.CGet<CButtonObsolete>("ButtonMax");
			buttonMax.ClearAndAddListener(delegate
			{
				this.ChangeMakeResourceCount(buttonMax, refers.name, true, true, false);
			});
			CButtonObsolete buttonMin = refers.CGet<CButtonObsolete>("ButtonMin");
			buttonMin.ClearAndAddListener(delegate
			{
				this.ChangeMakeResourceCount(buttonMin, refers.name, false, true, false);
			});
		}
		this._poisonTip = base.CGet<Refers>("PoisonTip");
		this._poisonTip.gameObject.SetActive(false);
		this._onClickConfirm = null;
		this.PlayCenterAnim(false, false, false);
		this._toolDurabilityText = base.CGet<TextMeshProUGUI>("ToolDurabilityText");
		this._requireLifeSkillList = base.CGet<RectTransform>("RequireLifeSkillList");
		this._colorBack = base.CGet<CImage>("ColorBack");
		this._colorLeft = base.CGet<CImage>("ColorLeft");
		this._colorRight = base.CGet<CImage>("ColorRight");
		this._makeTimeRoot = base.CGet<Refers>("TimeLabel");
		this._makeTimeLabel = this._makeTimeRoot.CGet<TextMeshProUGUI>("Label");
		this._makePanel = base.CGet<Refers>("Make");
		this._previewTip = base.CGet<TooltipInvoker>("PreviewTip");
		this._previewTip.gameObject.SetActive(true);
		this.PlayEffect();
		this.InitItemView();
		this.InitSlot();
		this.InitRefine();
		this.InitMake();
		this.InitPoison();
		this.InitMakeCraftsManPanel();
		this.NeedDataListenerId = true;
		this._itemViewToolSelected.gameObject.SetActive(false);
		this._itemViewTargetSelected.gameObject.SetActive(false);
		this._itemCacheTools.Clear();
		this._itemCacheTargets.Clear();
		this.SlotCacheItemList.Clear();
		this._inventoryItems.Clear();
		this._buttonConfirm.interactable = false;
		this._currentTarget = null;
		this._currentTool = null;
		this._workingMakeItemData = null;
		this._finishedMakeItemData = null;
		this.InitTab();
		this.InitButtonIdentity();
		this.HidePreviewPoison();
		this.ChangeTab(this._curTab, UI_Make.UIMakeTab.None);
	}

	// Token: 0x06001A77 RID: 6775 RVA: 0x000AEF8C File Offset: 0x000AD18C
	private void Awake()
	{
		this.InitWeaveSprite();
		this._mixedPoisonMedicineIdList.AddRange(Medicine.Instance.GetAllKeys().Where(delegate(short id)
		{
			MedicineItem medicineItem = Medicine.Instance[id];
			return medicineItem.ItemSubType == 803 && medicineItem.SpecialEffectId > -1;
		}));
	}

	// Token: 0x06001A78 RID: 6776 RVA: 0x000AEFDC File Offset: 0x000AD1DC
	private void InitItemView()
	{
		this._itemViewMaterial = base.CGet<ItemScrollView>("ItemViewMaterial");
		this._itemViewMaterial.Init();
		this._itemViewMaterial.SetCharId(this._taiwuCharId);
		this._itemViewMaterial.SetItemList(ref this._inventoryItems, true, "make_item_view_material", false, new Action<ItemDisplayData, ItemView>(this.OnRenderItemMaterial));
		this._itemViewEquip = base.CGet<ItemScrollView>("ItemViewEquip");
		this._itemViewEquip.Init();
		this._itemViewEquip.SetCharId(this._taiwuCharId);
		this._itemViewEquip.SetItemList(ref this._inventoryItems, true, "make_item_view_equip", false, new Action<ItemDisplayData, ItemView>(this.OnRenderItemEquip));
		this._itemViewEquip.SortAndFilter.ShowFilterType(ItemSortAndFilter.ItemFilterType.Equip);
		this._itemViewEquip.SortAndFilter.LockFilterType(ItemSortAndFilter.ItemFilterType.Equip, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
		this._itemViewTools = base.CGet<ItemScrollView>("ItemViewTools");
		this._itemViewTools.Init();
		this._itemViewTools.SetCharId(this._taiwuCharId);
		this._itemViewTools.SetItemList(ref this._inventoryItems, true, "make_item_view_tools", false, new Action<ItemDisplayData, ItemView>(this.OnRenderItemTool));
		this._itemViewTools.SortAndFilter.ShowFilterType(ItemSortAndFilter.ItemFilterType.Make);
		this._itemViewTools.SortAndFilter.LockFilterType(ItemSortAndFilter.ItemFilterType.Make, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
		this._itemViewSlot = base.CGet<ItemScrollView>("ItemViewSlot");
		this._itemViewSlot.Init();
		this._itemViewSlot.SetCharId(this._taiwuCharId);
		this._itemViewSlot.SetItemList(ref this._inventoryItems, true, "make_item_view_slot", false, new Action<ItemDisplayData, ItemView>(this.OnRenderItemSlot));
		this._toolTogGroup = base.CGet<CToggleGroupObsolete>("ToolTogGroup");
		this._toolTogGroup.InitPreOnToggle(-1);
		this._toolTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnToolTogChange);
		this._slotTogGroup = base.CGet<CToggleGroupObsolete>("SlotTogGroup");
		this._slotTogGroup.InitPreOnToggle(-1);
		this._slotTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnSlotTogChange);
		this._equipTogGroup = base.CGet<CToggleGroupObsolete>("EquipTogGroup");
		this._equipTogGroup.InitPreOnToggle(-1);
		this._equipTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnEquipTogChange);
		this._materialTogGroup = base.CGet<CToggleGroupObsolete>("MaterialTogGroup");
		this._materialTogGroup.InitPreOnToggle(-1);
		this._materialTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnMaterialTogChange);
	}

	// Token: 0x06001A79 RID: 6777 RVA: 0x000AF258 File Offset: 0x000AD458
	private void OnMaterialTogChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		this.ChangeCurrentTarget(this._currentTarget, false);
		CToggleObsolete slotActiveTog = this._slotTogGroup.GetActive();
		bool flag = slotActiveTog;
		if (flag)
		{
			this._slotTogGroup.Set(slotActiveTog.Key, true, true);
		}
	}

	// Token: 0x06001A7A RID: 6778 RVA: 0x000AF2A0 File Offset: 0x000AD4A0
	private void OnEquipTogChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		this.ChangeCurrentTarget(this._currentTarget, false);
		CToggleObsolete slotActiveTog = this._slotTogGroup.GetActive();
		bool flag = slotActiveTog;
		if (flag)
		{
			this._slotTogGroup.Set(slotActiveTog.Key, true, true);
		}
	}

	// Token: 0x06001A7B RID: 6779 RVA: 0x000AF2E8 File Offset: 0x000AD4E8
	private void OnSlotTogChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		bool flag = !newTog;
		if (!flag)
		{
			switch (this._slotTogGroup.GetActive().Key)
			{
			case 0:
				this._curSlotItemSourceType = ItemSourceType.Inventory;
				break;
			case 1:
				this._curSlotItemSourceType = ItemSourceType.Warehouse;
				break;
			case 2:
				this._curSlotItemSourceType = ItemSourceType.Treasury;
				break;
			}
			this.RefreshSlotView();
		}
	}

	// Token: 0x06001A7C RID: 6780 RVA: 0x000AF350 File Offset: 0x000AD550
	private void OnToolTogChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		this.ChangeCurrentTool(this._currentTool);
	}

	// Token: 0x06001A7D RID: 6781 RVA: 0x000AF360 File Offset: 0x000AD560
	private void OnEnable()
	{
		UI_Make.<>c__DisplayClass162_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		this._isShowingGetItem = false;
		GEvent.Add(UiEvents.SwitchBuildingMake, new GEvent.Callback(this.OnSwitchBuildingMake));
		GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		GEvent.Add(EEvents.OnTaiwuResourceChange, new GEvent.Callback(this.OnTaiwuResourceChange));
		GEvent.Add(UiEvents.OnConfirmVillagerCraftInputMaterial, new GEvent.Callback(this.OnConfirmVillagerCraftInputMaterial));
		GEvent.Add(UiEvents.BuildingShopManagerChange, new GEvent.Callback(this.OnBuildingManagerUpdate));
		GEvent.Add(UiEvents.OnSetVillagerRole, new GEvent.Callback(this.OnSetVillagerRole));
		MonoJoint[] allJoints = this._pageTogGroup.GetComponentsInChildren<MonoJoint>(true);
		Array.ForEach<MonoJoint>(allJoints, delegate(MonoJoint e)
		{
			e.JointSync();
		});
		CS$<>8__locals1.curKey = (this._isSettlement ? 0 : 1);
		this.<OnEnable>g__InitTogGroup|162_1(this._toolTogGroup, ref CS$<>8__locals1);
		this.<OnEnable>g__InitTogGroup|162_1(this._equipTogGroup, ref CS$<>8__locals1);
		this.<OnEnable>g__InitTogGroup|162_1(this._materialTogGroup, ref CS$<>8__locals1);
		this.<OnEnable>g__InitTogGroup|162_1(this._slotTogGroup, ref CS$<>8__locals1);
		bool flag = SingletonObject.getInstance<TutorialChapterModel>().IsInTutorialChapter(2);
		if (flag)
		{
			this._toolTogGroup.Set(1, true, false);
			this._materialTogGroup.Set(1, true, false);
			GEvent.OnEvent(UiEvents.RefreshVideo, EasyPool.Get<ArgumentBox>().Set("TutorialVideoPathName", "Tutorial_Chapter_3_4"));
		}
	}

	// Token: 0x06001A7E RID: 6782 RVA: 0x000AF4EC File Offset: 0x000AD6EC
	private void OnDisable()
	{
		this.PlayCenterAnim(false, false, false);
		this.Clear();
		GEvent.Remove(UiEvents.SwitchBuildingMake, new GEvent.Callback(this.OnSwitchBuildingMake));
		GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
		GEvent.Remove(EEvents.OnTaiwuResourceChange, new GEvent.Callback(this.OnTaiwuResourceChange));
		GEvent.Remove(UiEvents.OnConfirmVillagerCraftInputMaterial, new GEvent.Callback(this.OnConfirmVillagerCraftInputMaterial));
		GEvent.Remove(UiEvents.BuildingShopManagerChange, new GEvent.Callback(this.OnBuildingManagerUpdate));
		GEvent.Remove(UiEvents.OnSetVillagerRole, new GEvent.Callback(this.OnSetVillagerRole));
		this.DestroyResourceSprite();
	}

	// Token: 0x06001A7F RID: 6783 RVA: 0x000AF5B0 File Offset: 0x000AD7B0
	private void OnClickToolItemView()
	{
		bool flag = this._currentTool != null && !this.NeedLockOnMaking;
		if (flag)
		{
			this.ChangeCurrentTool(null);
		}
	}

	// Token: 0x06001A80 RID: 6784 RVA: 0x000AF5E0 File Offset: 0x000AD7E0
	private void OnClickTargetItemView()
	{
		bool flag = this._currentTarget != null && !this.NeedLockOnMaking;
		if (flag)
		{
			this.ChangeCurrentTarget(null, false);
		}
	}

	// Token: 0x06001A81 RID: 6785 RVA: 0x000AF610 File Offset: 0x000AD810
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)this._taiwuCharId), new uint[]
		{
			97U
		}));
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 77, ulong.MaxValue, null));
		bool flag = this._curLifeSkillType == 10;
		if (flag)
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 75, ulong.MaxValue, null));
		}
		bool flag2 = this._curTab == UI_Make.UIMakeTab.CraftsmanPanel;
		if (flag2)
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(3, 1, (ulong)this.SettlementId, new uint[]
			{
				10U
			}));
		}
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
		{
			GameDataBridge.AddMethodCall(this.Element.GameDataListenerId, 6, 29);
			this.RefreshAllItems();
			BuildingDomainMethod.Call.GetMakingItemData(this.Element.GameDataListenerId, this._buildingBlockKey);
			bool isInDoor = this.IsInDoor;
			if (isInDoor)
			{
				BuildingDomainMethod.Call.GetBuildingEffectForMake(this.Element.GameDataListenerId, this._buildingBlockKey, this._curLifeSkillType);
			}
			CharacterDomainMethod.Call.GetCharacterDisplayData(this.Element.GameDataListenerId, this._taiwuCharId);
		}));
	}

	// Token: 0x06001A82 RID: 6786 RVA: 0x000AF6D8 File Offset: 0x000AD8D8
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
					bool flag = notification.DomainId == 4;
					if (flag)
					{
						bool flag2 = notification.MethodId == 27;
						if (flag2)
						{
							this._inventoryItems.Clear();
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._inventoryItems);
						}
						else
						{
							bool flag3 = notification.MethodId == 29;
							if (flag3)
							{
								this._equipmentItems.Clear();
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._equipmentItems);
								sbyte slotIndex = 0;
								while ((int)slotIndex < this._equipmentItems.Count)
								{
									ItemDisplayData itemData = this._equipmentItems[(int)slotIndex];
									bool flag4 = itemData.Key.IsValid();
									if (flag4)
									{
										itemData.UsingType = ItemDisplayData.ItemUsingType.Equiped;
									}
									slotIndex += 1;
								}
							}
							else
							{
								bool flag5 = notification.MethodId == 131;
								if (flag5)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._characterDisplayData);
								}
								else
								{
									bool flag6 = notification.MethodId == 48;
									if (flag6)
									{
										List<CharacterDisplayData> displayDataList = null;
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref displayDataList);
										this._charDisplayDataDict.Clear();
										foreach (CharacterDisplayData data in displayDataList)
										{
											this._charDisplayDataDict.Add(data.CharacterId, data);
										}
										this.RefreshAllQuickSelectButtons();
										this.Element.ShowAfterRefresh();
									}
								}
							}
						}
					}
					else
					{
						bool flag7 = notification.DomainId == 5;
						if (flag7)
						{
							bool flag8 = notification.MethodId == 15;
							if (flag8)
							{
								this._warehouseItems.Clear();
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._warehouseItems);
							}
							else
							{
								bool flag9 = notification.MethodId == 139;
								if (flag9)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._villagerNeededItemSet);
								}
								else
								{
									bool flag10 = notification.MethodId == 64;
									if (flag10)
									{
										this._treasuryItems.Clear();
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._treasuryItems);
										this._allItems.Clear();
										this._allItems.AddRange(this._inventoryItems);
										this._allItems.AddRange(this._warehouseItems);
										this._allItems.AddRange(this._equipmentItems);
										this._allItems.AddRange(this._treasuryItems);
										this.Element.ShowAfterRefresh();
										this.OnItemsDataRefresh();
									}
									else
									{
										bool flag11 = notification.MethodId == 11;
										if (flag11)
										{
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._availableWorker);
											int i = 0;
											while (i < 7)
											{
												bool flag12 = i == 0;
												ResidentView residentView;
												if (flag12)
												{
													residentView = this._managerLeaderViewRefer;
													goto IL_570;
												}
												Transform child = this._buildingCraftsmanPanel.CGet<GameObject>("MemberHolder").transform.GetChild(i - 1);
												residentView = child.GetComponent<ResidentView>();
												bool flag13 = residentView == null;
												if (!flag13)
												{
													goto IL_570;
												}
												IL_58C:
												i++;
												continue;
												IL_570:
												Refers childRefer = residentView.GetComponent<Refers>();
												this.RefreshSelectCharacterBtn(i, childRefer, 1, this.LiftSkillType);
												goto IL_58C;
											}
										}
										else
										{
											bool flag14 = notification.MethodId == 181;
											if (flag14)
											{
												Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._availableChildren);
											}
											else
											{
												bool flag15 = notification.MethodId == 176;
												if (flag15)
												{
													List<VillagerRoleCharacterDisplayData> villagerRoleCharacterDisplayDataList = new List<VillagerRoleCharacterDisplayData>();
													Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref villagerRoleCharacterDisplayDataList);
													this.HandleVillagerDisplayData(villagerRoleCharacterDisplayDataList);
													this.Element.ShowAfterRefresh();
												}
												else
												{
													bool flag16 = notification.MethodId == 121;
													if (flag16)
													{
														this.RefreshMakeCraftsManPanelData();
													}
												}
											}
										}
									}
								}
							}
						}
						else
						{
							bool flag17 = notification.DomainId == 9;
							if (flag17)
							{
								bool flag18 = notification.MethodId == 165;
								if (flag18)
								{
									ValueTuple<int, bool> result = new ValueTuple<int, bool>(0, false);
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref result);
									this._buildingAttainmentEffect = result.Item1;
									this._haveBuildingUpgradeMakeItem = result.Item2;
								}
								else
								{
									bool flag19 = notification.MethodId == 41;
									if (flag19)
									{
										this._workingMakeItemData = null;
										Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._workingMakeItemData);
										this._lastMakeResourceCount.Initialize();
										bool flag20 = this._workingMakeItemData != null;
										if (flag20)
										{
											this._lastMakeResourceCount.Add(ref this._workingMakeItemData.MaterialResources);
										}
										this.UpdateMakeState(true);
									}
									else
									{
										bool flag21 = notification.MethodId == 38;
										if (flag21)
										{
											this._finishedMakeItemData = null;
											Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._finishedMakeItemData);
											this._lastMakeResourceCount.Initialize();
											bool flag22 = this._finishedMakeItemData != null;
											if (flag22)
											{
												this._lastMakeResourceCount.Add(ref this._finishedMakeItemData.MaterialResources);
											}
											this.UpdateMakeState(false);
										}
										else
										{
											bool flag23 = notification.MethodId == 40;
											if (flag23)
											{
												List<ItemDisplayData> itemDataList = null;
												Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref itemDataList);
												TaiwuEventDomainMethod.Call.OnCollectedMakingSystemItem(this._buildingBlockKey, this._blockData.TemplateId, true);
												ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
												argBox.SetObject("ItemList", itemDataList);
												argBox.Set("ObtainType", 6);
												argBox.Set("InWareHouse", !this._isSettlement);
												Action closeAction = delegate()
												{
													this.CheckCondition(false);
													this.Tutorial(itemDataList);
												};
												argBox.SetObject("CloseAction", closeAction);
												UIElement.GetItem.SetOnInitArgs(argBox);
												UIManager.Instance.MaskUI(UIElement.GetItem);
												this._workingMakeItemData = null;
												this.UpdateMakeState(false);
											}
										}
									}
								}
							}
							else
							{
								bool flag24 = notification.DomainId == 6;
								if (flag24)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._emptyToolKey);
								}
							}
						}
					}
				}
			}
			else
			{
				bool flag25 = notification.Uid.DomainId == 4 && notification.Uid.DataId == 0;
				if (flag25)
				{
					uint subId = notification.Uid.SubId1;
					uint num = subId;
					if (num == 97U)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._lifeSkillAttainments);
					}
				}
				else
				{
					bool flag26 = notification.Uid.DomainId == 5;
					if (flag26)
					{
						bool flag27 = notification.Uid.DataId == 75;
						if (flag27)
						{
							Serializer.DeserializeModifications<short>(wrapper.DataPool, notification.ValueOffset, this._ownedClothingSet);
							bool flag28 = this._curTab == UI_Make.UIMakeTab.Weave;
							if (flag28)
							{
								this.RefreshWeaveSlotView();
							}
							this.InitWeave();
							this.RefreshWeaveClothingView();
						}
						else
						{
							bool flag29 = notification.Uid.DataId == 77;
							if (flag29)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._weaveClothingDisplaySetting);
							}
						}
					}
					else
					{
						bool flag30 = notification.Uid.DomainId == 3 && notification.Uid.DataId == 1 && (short)notification.Uid.SubId0 == this.SettlementId && notification.Uid.SubId1 == 10U;
						if (flag30)
						{
							OrgMemberCollection collection = null;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref collection);
							collection.GetAllMembers(this._villagerList);
							bool flag31 = this._villagerList.Count > 0;
							if (flag31)
							{
								TaiwuDomainMethod.Call.GetAllVillagersAvailableForWork(this.Element.GameDataListenerId);
								TaiwuDomainMethod.Call.GetAllChildAvailableForWork(this.Element.GameDataListenerId);
								CharacterDomainMethod.Call.GetCharacterDisplayDataList(this.Element.GameDataListenerId, this._villagerList);
							}
							else
							{
								this._charDisplayDataDict.Clear();
								this.Element.ShowAfterRefresh();
							}
							this.UpdatePropertyValueData(null);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001A83 RID: 6787 RVA: 0x000AFFD0 File Offset: 0x000AE1D0
	private void Tutorial(List<ItemDisplayData> itemDataList)
	{
		bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
		if (inGuiding)
		{
			bool flag = SingletonObject.getInstance<TutorialChapterModel>().TutorialChapterIndex == 2;
			if (flag)
			{
				bool flag2 = itemDataList == null;
				if (!flag2)
				{
					foreach (ItemDisplayData itemDisplayData in itemDataList)
					{
						bool flag3 = itemDisplayData.Key.ItemType == 12 && itemDisplayData.Key.TemplateId == 270;
						if (flag3)
						{
							TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("MakeSystemShowed", "MadeBambooThorn", true);
							TaiwuEventDomainMethod.Call.TriggerListener("MakeSystemShowed", true);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001A84 RID: 6788 RVA: 0x000B009C File Offset: 0x000AE29C
	private void OnItemsDataRefresh()
	{
		UIElement.FullScreenMask.Hide(false);
		bool flag = this._currentTarget == null && this._currentTool == null;
		if (flag)
		{
			bool flag2 = this._curTab == UI_Make.UIMakeTab.Repair && this._repairToolKey.ItemType == 6;
			if (flag2)
			{
				ItemDisplayData tool = this._allItems.Find((ItemDisplayData d) => d.Key.Id == this._repairToolKey.Id);
				this.ChangeCurrentTarget(null, false);
				this.ChangeCurrentTool(tool);
			}
		}
		else
		{
			bool flag3 = this._currentTool != null;
			if (flag3)
			{
				bool flag4 = this.IsEmptyTool(this._currentTool);
				ItemDisplayData tool2;
				if (flag4)
				{
					tool2 = this._currentTool;
				}
				else
				{
					ItemDisplayData lastTool = this._allItems.Find((ItemDisplayData d) => d.Key.Id == this._currentTool.Key.Id);
					bool flag5 = lastTool != null;
					if (flag5)
					{
						tool2 = lastTool;
					}
					else
					{
						sbyte grade = ItemTemplateHelper.GetGrade(this._currentTool.Key.ItemType, this._currentTool.Key.TemplateId);
						short groupId = ItemTemplateHelper.GetGroupId(this._currentTool.Key.ItemType, this._currentTool.Key.TemplateId);
						ItemDisplayData nextTool = this._allItems.Find((ItemDisplayData d) => d.Key.ItemType == this._currentTool.Key.ItemType && grade == ItemTemplateHelper.GetGrade(d.Key.ItemType, d.Key.TemplateId) && groupId == ItemTemplateHelper.GetGroupId(d.Key.ItemType, d.Key.TemplateId));
						tool2 = nextTool;
					}
				}
				this.ChangeCurrentTool(tool2);
			}
			bool flag6 = this._currentTarget != null;
			if (flag6)
			{
				List<ItemDisplayData> source = this.GetItemsSource(this._materialTogGroup);
				bool isStackable = ItemTemplateHelper.IsStackable(this._currentTarget.Key.ItemType, this._currentTarget.Key.TemplateId);
				UI_Make.UIMakeTab curTab = this._curTab;
				bool needSelectResult = curTab == UI_Make.UIMakeTab.Refine || curTab == UI_Make.UIMakeTab.Poison || curTab == UI_Make.UIMakeTab.RemovePoison || curTab == UI_Make.UIMakeTab.Weave;
				ItemDisplayData target = (this._resultItemDisplayData != null && this._resultItemDisplayData.Key.IsValid() && needSelectResult) ? (source.Find((ItemDisplayData d) => d.Key.Id == this._resultItemDisplayData.Key.Id) ?? this._allItems.Find((ItemDisplayData d) => d.Key.Id == this._resultItemDisplayData.Key.Id)) : (source.Find((ItemDisplayData d) => d.Key.Id == this._currentTarget.Key.Id) ?? this._allItems.Find((ItemDisplayData d) => d.Key.Id == this._currentTarget.Key.Id));
				bool flag7 = target == null && isStackable;
				if (flag7)
				{
					ItemDisplayData searchData = needSelectResult ? this._resultItemDisplayData : this._currentTarget;
					ItemDisplayData itemDisplayData;
					if ((itemDisplayData = source.Find((ItemDisplayData d) => d.Key.TemplateEquals(searchData.Key) && d.PoisonEffects == searchData.PoisonEffects)) == null && (itemDisplayData = source.Find((ItemDisplayData d) => d.Key.TemplateEquals(searchData.Key))) == null)
					{
						itemDisplayData = (this._allItems.Find((ItemDisplayData d) => d.Key.TemplateEquals(searchData.Key) && d.PoisonEffects == searchData.PoisonEffects) ?? this._allItems.Find((ItemDisplayData d) => d.Key.TemplateEquals(searchData.Key)));
					}
					target = itemDisplayData;
				}
				this.ChangeCurrentTarget(target, true);
			}
			this.RefreshSlotView();
		}
		this._pageTogGroup.Set((int)this._curTab, true, false);
		CToggleGroupObsolete toolTogGroup = this._toolTogGroup;
		CToggleObsolete active = this._toolTogGroup.GetActive();
		toolTogGroup.Set((active != null) ? active.Key : 0, true, true);
		CToggleGroupObsolete materialTogGroup = this._materialTogGroup;
		CToggleObsolete active2 = this._materialTogGroup.GetActive();
		materialTogGroup.Set((active2 != null) ? active2.Key : 0, true, true);
	}

	// Token: 0x06001A85 RID: 6789 RVA: 0x000B040C File Offset: 0x000AE60C
	private void RefreshSlotView()
	{
		switch (this._curTab)
		{
		case UI_Make.UIMakeTab.Poison:
			this.RefreshPoisonSlotView(true);
			break;
		case UI_Make.UIMakeTab.RemovePoison:
			this.RefreshPoisonSlotView(false);
			break;
		case UI_Make.UIMakeTab.Refine:
			this.RefreshRefineSlotView(false);
			break;
		case UI_Make.UIMakeTab.Weave:
			this.RefreshWeaveSlotView();
			break;
		}
	}

	// Token: 0x06001A86 RID: 6790 RVA: 0x000B0464 File Offset: 0x000AE664
	private void RefreshAllItems()
	{
		CharacterDomainMethod.Call.GetAllInventoryItems(this.Element.GameDataListenerId, this._taiwuCharId);
		CharacterDomainMethod.Call.GetAllEquipmentItems(this.Element.GameDataListenerId, this._taiwuCharId);
		TaiwuDomainMethod.Call.GetTreasuryNeededItemList(this.Element.GameDataListenerId);
		TaiwuDomainMethod.Call.GetAllWarehouseItems(this.Element.GameDataListenerId);
		TaiwuDomainMethod.Call.GetAllTreasuryItems(this.Element.GameDataListenerId);
		this.ClearSlotItemChange();
	}

	// Token: 0x06001A87 RID: 6791 RVA: 0x000B04DC File Offset: 0x000AE6DC
	private List<ItemDisplayData> GetItemsSource(CToggleGroupObsolete toggleGroup)
	{
		List<ItemDisplayData> result;
		switch (this.GetTogType(toggleGroup))
		{
		case UI_Make.TogType.Inventory:
			result = this._inventoryItems;
			break;
		case UI_Make.TogType.Warehouse:
			result = this._warehouseItems;
			break;
		case UI_Make.TogType.Treasury:
			result = this._treasuryItems;
			break;
		default:
			result = null;
			break;
		}
		return result;
	}

	// Token: 0x06001A88 RID: 6792 RVA: 0x000B052C File Offset: 0x000AE72C
	private List<ItemDisplayData> GetItemsSource(ItemSourceType itemSourceType)
	{
		List<ItemDisplayData> result;
		switch (itemSourceType)
		{
		case ItemSourceType.Equipment:
			result = this._equipmentItems;
			break;
		case ItemSourceType.Inventory:
			result = this._inventoryItems;
			break;
		case ItemSourceType.Warehouse:
			result = this._warehouseItems;
			break;
		case ItemSourceType.Treasury:
			result = this._treasuryItems;
			break;
		default:
			result = null;
			break;
		}
		return result;
	}

	// Token: 0x06001A89 RID: 6793 RVA: 0x000B0580 File Offset: 0x000AE780
	private UI_Make.TogType GetTogType(CToggleGroupObsolete toggleGroup)
	{
		return (UI_Make.TogType)toggleGroup.GetActive().Key;
	}

	// Token: 0x06001A8A RID: 6794 RVA: 0x000B05A0 File Offset: 0x000AE7A0
	private void OnSwitchBuildingMake(ArgumentBox argsBox)
	{
		this._workingMakeItemData = null;
		argsBox.Get<HashSet<UI_Make.UIMakeTab>>("AllTab", out this._allTabSet);
		argsBox.Get<BuildingBlockKey>("BuildingBlockKey", out this._buildingBlockKey);
		argsBox.Get<BuildingBlockData>("BuildingBlockData", out this._blockData);
		argsBox.Get("LifeSkillType", out this._curLifeSkillType);
		bool flag = !this._allTabSet.Contains(this._curTab);
		if (flag)
		{
			this.Clear();
			this._curTab = this._allTabSet.First<UI_Make.UIMakeTab>();
		}
		this.InitTab();
		this._pageTogGroup.Set((int)this._curTab, true, true);
		BuildingDomainMethod.Call.GetBuildingEffectForMake(this.Element.GameDataListenerId, this._buildingBlockKey, this._curLifeSkillType);
		BuildingDomainMethod.Call.GetMakingItemData(this.Element.GameDataListenerId, this._buildingBlockKey);
	}

	// Token: 0x06001A8B RID: 6795 RVA: 0x000B0680 File Offset: 0x000AE880
	private void OnTopUiChanged(ArgumentBox argsBox)
	{
		bool lastShow = this._isShowingGetItem;
		this._isShowingGetItem = UIManager.Instance.IsFocusElement(UIElement.GetItem);
		bool flag = this._needAutoFillResourceOnMakeEnd && lastShow && !this._isShowingGetItem;
		if (flag)
		{
			this._needAutoFillResourceOnMakeEnd = false;
			this.AutoFillResourceOnMakeEnd();
			this._lastMakeResourceCount.Initialize();
		}
		bool flag2 = UIManager.Instance.IsFocusElement(UIElement.EventWindow);
		if (flag2)
		{
			this._needRefreshAfterEvent = true;
		}
		bool isFocusElement = UIManager.Instance.IsFocusElement(this.Element);
		bool flag3 = this._needRefreshAfterEvent && isFocusElement;
		if (flag3)
		{
			this._needRefreshAfterEvent = false;
			this.RefreshAllItems();
		}
		base.CGet<GameObject>("FoodImage").SetActive(isFocusElement && this._curLifeSkillType == 14);
	}

	// Token: 0x06001A8C RID: 6796 RVA: 0x000B074A File Offset: 0x000AE94A
	private void OnTaiwuResourceChange(ArgumentBox argsBox)
	{
		this.CheckCondition(false);
	}

	// Token: 0x06001A8D RID: 6797 RVA: 0x000B0758 File Offset: 0x000AE958
	private void InitTab()
	{
		this._pageTogGroup = base.CGet<CToggleGroupObsolete>("TabPanel");
		this._pageTogGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnActiveToggleChange);
		this._pageTogGroup.InitPreOnToggle(this._curTab.ToInt());
		Refers refer = this._pageTogGroup.GetComponent<Refers>();
		List<CToggleObsolete> togList = this._pageTogGroup.GetAll();
		List<Transform> activeTogList = new List<Transform>();
		for (int i = 0; i < togList.Count; i++)
		{
			CToggleObsolete tog = togList[i];
			bool togActive = this._allTabSet.Contains((UI_Make.UIMakeTab)tog.Key);
			tog.gameObject.SetActive(togActive);
			bool flag = togActive;
			if (flag)
			{
				activeTogList.Add(tog.transform);
			}
		}
		for (int j = 0; j < togList.Count - 1; j++)
		{
			bool active = j < activeTogList.Count - 1;
			GameObject imgGo = refer.CGet<GameObject>((j + 1).ToString());
			imgGo.SetActive(active);
			bool flag2 = active;
			if (flag2)
			{
				int index = activeTogList[j].GetSiblingIndex();
				imgGo.transform.SetSiblingIndex(index + 1);
			}
		}
	}

	// Token: 0x06001A8E RID: 6798 RVA: 0x000B0899 File Offset: 0x000AEA99
	private void Clear()
	{
		this.ClearSelectedPoisonPage();
		this.HidePreviewPoison();
		this.ChangeCurrentTarget(null, false);
		this.ChangeCurrentTool(null);
		this.ClearSlot();
		this.ShowMakeDropdown(false, 0);
	}

	// Token: 0x06001A8F RID: 6799 RVA: 0x000B08CC File Offset: 0x000AEACC
	private void ChangeTab(UI_Make.UIMakeTab newTab, UI_Make.UIMakeTab oldTab = UI_Make.UIMakeTab.None)
	{
		bool flag = oldTab > UI_Make.UIMakeTab.None;
		if (flag)
		{
			this.Clear();
		}
		this._curTab = newTab;
		bool isCraftsmanPanel = this._curTab == UI_Make.UIMakeTab.CraftsmanPanel;
		base.CGet<GameObject>("MakeRoot").SetActive(!isCraftsmanPanel);
		this.SetShowCraftsManPanel(isCraftsmanPanel);
		bool flag2 = isCraftsmanPanel;
		if (flag2)
		{
			this.RefreshMakeCraftsManPanelData();
		}
		else
		{
			this.ChangeCurrentTool(null);
			this.ChangeCurrentTarget(null, false);
			this.ShowMakeRequireResourceList(false);
			this._refineRequireResourceList.gameObject.SetActive(false);
			this.ShowMakeEffectList(false);
			this._itemCacheTargets.Clear();
			this.SlotCacheItemList.Clear();
			this._itemCacheTools.Clear();
			this._makeItemTypeId = -1;
			this._makeItemSubTypeId = -1;
			this._makeTypeDict.Clear();
			this._makeTypeList.Clear();
			this.ClearSlotItemChange();
			this.PlayCenterAnim(false, false, false);
			this.ShowResultPreviewImage(false);
			this.ShowUnidentifiedPoisonTip(false, false);
			this.PlayEffect();
			this.InitResourceSprite();
			this.InitPoisonSpecialInteraction();
			TooltipInvoker targetTip = base.CGet<TooltipInvoker>("TargetTip");
			targetTip.PresetParam = new string[1];
			this._itemViewMaterial.SortAndFilter.IsOnRemovePoison = (this._curTab == UI_Make.UIMakeTab.RemovePoison);
			bool isWave = this._curTab == UI_Make.UIMakeTab.Weave;
			this._itemViewSlot.SortAndFilter.ShowClothingWaveFilter(isWave);
			this._slotTogGroup.gameObject.SetActive(!isWave);
			UI_Make.UIMakeTab curTab = this._curTab;
			if (!true)
			{
			}
			string text;
			if (curTab - UI_Make.UIMakeTab.Poison > 1)
			{
				if (curTab != UI_Make.UIMakeTab.Weave)
				{
					text = CommonUtils.GetItemTypeName(5);
				}
				else
				{
					text = LanguageKey.LK_Making_Cloth_Appearance.Tr();
				}
			}
			else
			{
				text = CommonUtils.GetItemTypeName(8);
			}
			if (!true)
			{
			}
			string slotPanelTitle = text;
			base.CGet<TextMeshProUGUI>("SlotPanelTitle").text = slotPanelTitle;
			string slotPanelIcon = isWave ? "building_icon_appearance" : "building_icon_cailiao";
			base.CGet<CImage>("SlotPanelIcon").SetSprite(slotPanelIcon, true, null);
			base.CGet<TooltipInvoker>("SlotPanelTitleBG").enabled = isWave;
			LanguageKey equipPanelTitle = isWave ? LanguageKey.LK_ItemType_3 : LanguageKey.LK_Refine_Equipment;
			base.CGet<TextMeshProUGUI>("EquipPanelTitle").text = LocalStringManager.Get(equipPanelTitle);
			this.ShowMakeTime();
			base.CGet<Refers>("Make").gameObject.SetActive(this._curTab == UI_Make.UIMakeTab.Make);
			base.CGet<Refers>("Repair").gameObject.SetActive(this._curTab == UI_Make.UIMakeTab.Repair);
			base.CGet<Refers>("Refine").gameObject.SetActive(this._curTab == UI_Make.UIMakeTab.Refine);
			base.CGet<Refers>("Poison").gameObject.SetActive(this._curTab == UI_Make.UIMakeTab.Poison);
			base.CGet<Refers>("RemovePoison").gameObject.SetActive(this._curTab == UI_Make.UIMakeTab.RemovePoison);
			switch (this._curTab)
			{
			case UI_Make.UIMakeTab.Make:
			{
				this.SetColorMaskColor("ffd5bd");
				targetTip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Select_Making_Material_Tips);
				this.UpdateMakeState(true);
				this.ShowScrollView(true, false, false);
				this._itemViewMaterial.SortAndFilter.ShowFilterType(ItemSortAndFilter.ItemFilterType.Material);
				this._itemViewMaterial.SortAndFilter.LockFilterType(ItemSortAndFilter.ItemFilterType.Material, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
				base.CGet<TextMeshProUGUI>("MaterialTitle").text = CommonUtils.GetItemTypeName(5);
				MakeItemData workingMakeItemData = this._workingMakeItemData;
				bool flag3 = workingMakeItemData != null && workingMakeItemData.LeftTime > 0;
				if (flag3)
				{
					this.PlayCenterAnim(true, true, false);
				}
				break;
			}
			case UI_Make.UIMakeTab.Repair:
				this.SetColorMaskColor("c2ffef");
				targetTip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Select_Repair_Item);
				this._confirmNormalLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Repair_Start_Tip_Title, Array.Empty<object>());
				this._confirmDisableLabel.text = this._confirmNormalLabel.text;
				this.ShowScrollView(false, false, true);
				break;
			case UI_Make.UIMakeTab.Poison:
				this.SetColorMaskColor("d1bcff");
				targetTip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Select_Add_Poison_Item);
				this.ChangeTabAboutPoison(true);
				break;
			case UI_Make.UIMakeTab.RemovePoison:
				this.SetColorMaskColor("b4d7ff");
				targetTip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Select_Remove_Poison_Item);
				this.ChangeTabAboutPoison(false);
				break;
			case UI_Make.UIMakeTab.Refine:
				this.SetColorMaskColor("ffffff");
				targetTip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Select_Strengthen_Item);
				this._confirmNormalLabel.text = LocalStringManager.Get(LanguageKey.LK_Strengthen_Start_Tip_Title);
				this._confirmDisableLabel.text = this._confirmNormalLabel.text;
				this.ShowScrollView(false, true, true);
				break;
			case UI_Make.UIMakeTab.Weave:
				this._makeTimeLabel.text = 0.ToString();
				this.SetColorMaskColor("ffd5bd");
				targetTip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_Select_Weave_Clothing);
				this._confirmNormalLabel.text = LocalStringManager.Get(LanguageKey.LK_Weave_Item_Start_Tip_Title);
				this._confirmDisableLabel.text = this._confirmNormalLabel.text;
				this.ShowScrollView(false, true, true);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			this.RefreshSlotCount();
			this._itemCacheTools.Clear();
			List<ItemDisplayData> sourceItems = this.GetItemsSource(this._toolTogGroup);
			bool flag4 = this._currentTool == null || !this.IsEmptyTool(this._currentTool);
			if (flag4)
			{
				this._itemCacheTools.Add(new ItemDisplayData
				{
					Key = this._emptyToolKey
				});
			}
			this._itemViewTools.SortAndFilter.StaticAheadItemKeysList.Clear();
			this._itemViewTools.SortAndFilter.StaticAheadItemKeysList.Add(this._emptyToolKey);
			this._itemCacheTools.AddRange(from d in sourceItems
			where d != this._currentTool && d.Key.ItemType == 6 && CraftTool.Instance[d.Key.TemplateId].RequiredLifeSkillTypes.Contains(this._curLifeSkillType)
			select d);
			this._itemViewTools.SetItemList(ref this._itemCacheTools, false, null, false, null);
			this.RefreshButtonIdentity();
			this.RefreshButtonPerfect(true);
		}
	}

	// Token: 0x06001A90 RID: 6800 RVA: 0x000B0EC0 File Offset: 0x000AF0C0
	private void SetColorMaskColor(string hex)
	{
		Color color = hex.HexStringToColor();
		this._colorBack.SetColor(color);
		this._colorLeft.SetColor(color);
		this._colorRight.SetColor(color);
	}

	// Token: 0x06001A91 RID: 6801 RVA: 0x000B0EFC File Offset: 0x000AF0FC
	private void ShowScrollView(bool showMaterial, bool showSlot, bool showEquip)
	{
		base.CGet<GameObject>("SlotPanel").SetActive(showSlot);
		base.CGet<GameObject>("MaterialPanel").SetActive(showMaterial);
		base.CGet<GameObject>("EquipPanel").SetActive(showEquip);
		if (showMaterial)
		{
			CToggleGroupObsolete materialTogGroup = this._materialTogGroup;
			CToggleObsolete active = this._materialTogGroup.GetActive();
			materialTogGroup.Set((active != null) ? active.Key : 0, true, true);
		}
		if (showSlot)
		{
			CToggleGroupObsolete slotTogGroup = this._slotTogGroup;
			CToggleObsolete active2 = this._slotTogGroup.GetActive();
			slotTogGroup.Set((active2 != null) ? active2.Key : 0, true, true);
		}
		if (showEquip)
		{
			CToggleGroupObsolete equipTogGroup = this._equipTogGroup;
			CToggleObsolete active3 = this._equipTogGroup.GetActive();
			equipTogGroup.Set((active3 != null) ? active3.Key : 0, true, true);
		}
	}

	// Token: 0x06001A92 RID: 6802 RVA: 0x000B0FC0 File Offset: 0x000AF1C0
	private void OnActiveToggleChange(CToggleObsolete newToggle, CToggleObsolete oldToggle)
	{
		bool flag = null == newToggle;
		if (!flag)
		{
			this.ChangeTab((UI_Make.UIMakeTab)newToggle.Key, (UI_Make.UIMakeTab)((null == oldToggle) ? 0 : oldToggle.Key));
		}
	}

	// Token: 0x06001A93 RID: 6803 RVA: 0x000B0FFC File Offset: 0x000AF1FC
	private void ChangeTabAboutPoison(bool isAdd)
	{
		this._confirmNormalLabel.text = LocalStringManager.GetFormat(isAdd ? LanguageKey.LK_Add_Item_Poison_Start_Tip_Title : LanguageKey.LK_Remove_Item_Poison_Start_Tip_Title, Array.Empty<object>());
		this._confirmDisableLabel.text = this._confirmNormalLabel.text;
		this.RefreshPoisonSlotView(isAdd);
		this.ShowScrollView(true, true, false);
		this._itemViewMaterial.SortAndFilter.ShowFilterType(this._poisonItemFilterTypeList);
		this._itemViewMaterial.SortAndFilter.LockFilterType(this._poisonItemFilterTypeList, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
		base.CGet<TextMeshProUGUI>("MaterialTitle").text = CommonUtils.GetItemTypeName(8);
	}

	// Token: 0x06001A94 RID: 6804 RVA: 0x000B10A0 File Offset: 0x000AF2A0
	private void OnRenderItemTool(ItemDisplayData itemData, ItemView itemView)
	{
		bool interactable = !this.NeedLockOnMaking && !itemData.IsLocked;
		itemView.SetLocked(!interactable);
		itemView.SetInteractable(interactable);
		itemView.SetClickEvent(delegate
		{
			bool flag2 = this._currentTool == null || !this._currentTool.ContainsItemKey(itemData.Key);
			if (flag2)
			{
				this.ChangeCurrentTool((itemData == this._invalidItemDisplayData) ? itemData : itemData.Clone(-1));
			}
		});
		bool flag = itemData.Key.IsValid() && !this.IsEmptyTool(itemData);
		if (flag)
		{
			itemView.ShowDurability();
		}
		this.SetVillagerNeedMark(itemView, itemData.ItemSourceTypeEnum);
	}

	// Token: 0x06001A95 RID: 6805 RVA: 0x000B114C File Offset: 0x000AF34C
	private void OnRenderItemMaterial(ItemDisplayData itemData, ItemView itemView)
	{
		bool interactable = !this.NeedLockOnMaking && !itemData.IsLocked;
		itemView.SetLocked(!interactable);
		itemView.SetInteractable(interactable);
		itemView.SetClickEvent(delegate
		{
			bool flag = this._currentTarget == null || !this._currentTarget.RealKey.TemplateEquals(itemData.RealKey) || this._currentTarget.ItemSourceType != itemData.ItemSourceType;
			if (flag)
			{
				this.ChangeCurrentTarget(itemData, false);
			}
		});
		this.SetVillagerNeedMark(itemView, itemData.ItemSourceTypeEnum);
	}

	// Token: 0x06001A96 RID: 6806 RVA: 0x000B11C4 File Offset: 0x000AF3C4
	private void OnRenderItemEquip(ItemDisplayData itemData, ItemView itemView)
	{
		itemView.SetLocked(this.NeedLockOnMaking);
		itemView.SetClickEvent(delegate
		{
			bool flag = this._currentTarget == null || !this._currentTarget.RealKey.TemplateEquals(itemData.RealKey) || this._currentTarget.ItemSourceType != itemData.ItemSourceType;
			if (flag)
			{
				this.ChangeCurrentTarget(itemData, false);
			}
		});
		this.SetVillagerNeedMark(itemView, itemData.ItemSourceTypeEnum);
	}

	// Token: 0x06001A97 RID: 6807 RVA: 0x000B121C File Offset: 0x000AF41C
	private void ChangeCurrentTool(ItemDisplayData target)
	{
		this._toolDurabilityCost = 0;
		this._itemViewToolSelected.SetUsedDurability(0, false);
		bool hasChanged = (target != this._currentTool && (target == null || this._currentTool == null)) || (target != null && this._currentTool != null && !target.Key.Equals(this._currentTool.Key));
		bool dontStopAnim = false;
		this._currentTool = target;
		bool flag = this._currentTool != null;
		if (flag)
		{
			dontStopAnim = (target != null && target.Key.Id == this._currentTool.Key.Id);
			this._itemViewToolSelected.gameObject.SetActive(true);
			this._itemViewToolSelected.SetData(this._currentTool, false, 1, false, true, null, false, true);
			bool isEmptyTool = this.IsEmptyTool(this._currentTool);
			bool flag2 = !isEmptyTool;
			if (flag2)
			{
				this._itemViewToolSelected.ShowDurability();
			}
			bool flag3 = !this.NeedLockOnMaking;
			if (flag3)
			{
				bool flag4 = isEmptyTool;
				if (flag4)
				{
					LanguageKey key = ((float)this._currentTool.Durability > (float)this._currentTool.MaxDurability * 0.5f) ? LanguageKey.LK_Tool_Durability_Sufficient : LanguageKey.LK_Tool_Durability_Insufficient;
					this._toolDurabilityText.text = LocalStringManager.GetFormat(key, this._currentTool.Durability, this._currentTool.MaxDurability);
				}
				else
				{
					this._toolDurabilityText.text = LocalStringManager.Get(LanguageKey.LK_Tool_Durability_Infinity);
				}
				this.RefreshToolAttainment(true);
			}
			this.PlayConditionAnim(false, true);
		}
		else
		{
			this._itemViewToolSelected.gameObject.SetActive(false);
			this._toolDurabilityText.transform.parent.gameObject.SetActive(false);
			this.RefreshToolAttainment(false);
			this.PlayConditionAnim(false, false);
			bool flag5 = target == null;
			if (flag5)
			{
				this.RefreshButtonPerfect(true);
			}
		}
		this._itemCacheTools.Clear();
		List<ItemDisplayData> sourceItems = this.GetItemsSource(this._toolTogGroup);
		bool flag6 = this._currentTool == null || !this.IsEmptyTool(this._currentTool);
		if (flag6)
		{
			this._itemCacheTools.Add(new ItemDisplayData
			{
				Key = this._emptyToolKey
			});
		}
		this._itemViewTools.SortAndFilter.StaticAheadItemKeysList.Clear();
		this._itemViewTools.SortAndFilter.StaticAheadItemKeysList.Add(this._emptyToolKey);
		this._itemCacheTools.AddRange(from d in sourceItems
		where (target == null || !d.ContainsItemKey(target.Key)) && d.Key.ItemType == 6 && CraftTool.Instance[d.Key.TemplateId].RequiredLifeSkillTypes.Contains(this._curLifeSkillType)
		select d);
		this._itemViewTools.SetItemList(ref this._itemCacheTools, false, null, false, null);
		bool flag7 = this._currentTarget != null;
		if (flag7)
		{
			this.CheckCondition(hasChanged);
		}
		bool flag8 = !dontStopAnim;
		if (flag8)
		{
			this.PlayCenterAnim(false, false, false);
		}
	}

	// Token: 0x06001A98 RID: 6808 RVA: 0x000B1540 File Offset: 0x000AF740
	private bool CheckTargetHasChanged(ItemDisplayData target)
	{
		bool flag = target != this._currentTarget && (target == null || this._currentTarget == null);
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			bool flag2 = target != null && this._currentTarget != null;
			if (flag2)
			{
				bool flag3 = !target.ContainsItemKey(this._currentTarget.Key);
				if (flag3)
				{
					return true;
				}
				bool flag4 = target.PoisonEffects != this._currentTarget.PoisonEffects;
				if (flag4)
				{
					return true;
				}
				bool flag5 = !target.RefiningEffects.Equals(this._currentTarget.RefiningEffects);
				if (flag5)
				{
					return true;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x06001A99 RID: 6809 RVA: 0x000B15F8 File Offset: 0x000AF7F8
	private void ChangeCurrentTarget(ItemDisplayData target, bool forceRefresh = false)
	{
		bool hasChanged = this.CheckTargetHasChanged(target);
		this.ShowUnidentifiedPoisonTip(false, false);
		bool flag = hasChanged || forceRefresh || target == null;
		if (flag)
		{
			this.ShowIdentifiedPoisonTip(false, null);
		}
		switch (this._curTab)
		{
		case UI_Make.UIMakeTab.None:
		case UI_Make.UIMakeTab.CraftsmanPanel:
			break;
		case UI_Make.UIMakeTab.Make:
			this.ChangeCurrentTargetOnMake(target, hasChanged, forceRefresh);
			break;
		case UI_Make.UIMakeTab.Repair:
			this.ChangeCurrentTargetOnRepair(target);
			break;
		case UI_Make.UIMakeTab.Poison:
			this.ChangeCurrentTargetOnAddPoison(target);
			break;
		case UI_Make.UIMakeTab.RemovePoison:
			this.ChangeCurrentTargetOnRemovePoison(target);
			break;
		case UI_Make.UIMakeTab.Refine:
			this.ChangeCurrentTargetOnRefine(target);
			break;
		case UI_Make.UIMakeTab.Weave:
			this.ChangeCurrentTargetOnWeave(target);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		bool flag2 = this._currentTarget != null;
		if (flag2)
		{
			this._itemViewTargetSelected.gameObject.SetActive(true);
			this._itemViewTargetSelected.SetData(this._currentTarget, false, this._currentTarget.Amount, false, true, null, false, true);
			this.PlayConditionAnim(true, true);
		}
		else
		{
			this._itemViewTargetSelected.gameObject.SetActive(false);
			this.PlayConditionAnim(true, false);
			this.HideLifeSkill();
		}
		bool flag3 = this._curTab == UI_Make.UIMakeTab.Refine || this.CurTabIsAboutPoison || this._curTab == UI_Make.UIMakeTab.Weave;
		if (flag3)
		{
			this.RefreshSlots();
		}
		else
		{
			this.CheckCondition(hasChanged);
		}
		this.ShowMakeTime();
	}

	// Token: 0x06001A9A RID: 6810 RVA: 0x000B1758 File Offset: 0x000AF958
	private void RefreshTargetList(CToggleGroupObsolete toggleGroup, ItemScrollView itemScrollView, Func<ItemDisplayData, bool> filter)
	{
		this._itemCacheTargets.Clear();
		switch (this.GetTogType(toggleGroup))
		{
		case UI_Make.TogType.Inventory:
			this._itemCacheTargets.AddRange(this._inventoryItems.Where(filter));
			this._itemCacheTargets.AddRange(this._equipmentItems.Where(filter));
			break;
		case UI_Make.TogType.Warehouse:
			this._itemCacheTargets.AddRange(this._warehouseItems.Where(filter));
			break;
		case UI_Make.TogType.Treasury:
			this._itemCacheTargets.AddRange(this._treasuryItems.Where(filter));
			break;
		}
		itemScrollView.SetItemList(ref this._itemCacheTargets, false, null, false, null);
	}

	// Token: 0x06001A9B RID: 6811 RVA: 0x000B180C File Offset: 0x000AFA0C
	private void ChangeCurrentTargetOnRefine(ItemDisplayData target)
	{
		UI_Make.<>c__DisplayClass192_0 CS$<>8__locals1 = new UI_Make.<>c__DisplayClass192_0();
		CS$<>8__locals1.target = target;
		CS$<>8__locals1.<>4__this = this;
		this.RefreshTargetList(this._equipTogGroup, this._itemViewEquip, new Func<ItemDisplayData, bool>(CS$<>8__locals1.<ChangeCurrentTargetOnRefine>g__Filter|0));
		bool flag = this._currentTarget != CS$<>8__locals1.target;
		if (flag)
		{
			for (int index = 0; index < this.SlotCount; index++)
			{
				ItemDisplayData itemData = this._slotItemArray[index];
				ItemDisplayData originItemData = this._originSlotItemArray[index];
				bool hasCur = itemData != null && itemData.Key.IsValid();
				bool hasOrigin = originItemData != null && originItemData.Key.HasTemplate;
				bool isSameTemplate = itemData != null && originItemData != null && itemData.Key.TemplateId == originItemData.Key.TemplateId;
				bool flag2 = hasCur && (!hasOrigin || !isSameTemplate);
				if (flag2)
				{
					this.RemoveSlotItem(itemData, index, false);
				}
				bool flag3 = !hasCur && hasOrigin && !isSameTemplate;
				if (flag3)
				{
					Func<ItemKeyAndCount, bool> <>9__1;
					foreach (KeyValuePair<ItemSourceType, ItemSourceChange> pair in this._slotItemChangeDict)
					{
						IEnumerable<ItemKeyAndCount> items = pair.Value.Items;
						Func<ItemKeyAndCount, bool> predicate;
						if ((predicate = <>9__1) == null)
						{
							predicate = (<>9__1 = ((ItemKeyAndCount i) => i.ItemKey.Equals(originItemData.Key)));
						}
						bool flag4 = items.Any(predicate);
						if (flag4)
						{
							this.AddSlotItem(originItemData, index, pair.Key);
							break;
						}
					}
				}
				short templateId = (CS$<>8__locals1.target == null) ? -1 : CS$<>8__locals1.target.RefiningEffects.GetMaterialTemplateIdAt(index);
				bool flag5 = templateId != -1;
				if (flag5)
				{
					int itemId = (itemData == null) ? -1 : itemData.Key.Id;
					ItemKey itemKey = new ItemKey(5, 0, templateId, itemId);
					itemData = new ItemDisplayData
					{
						Key = itemKey,
						ItemSourceType = 0,
						Amount = 1
					};
				}
				else
				{
					itemData = this._invalidItemDisplayData;
				}
				this._slotItemArray[index] = (this._originSlotItemArray[index] = itemData);
			}
		}
		bool flag6 = CS$<>8__locals1.target == null;
		if (flag6)
		{
			this._refineRequireResourceList.gameObject.SetActive(false);
			bool flag7 = this._currentTarget != null;
			if (flag7)
			{
				this.RefreshAllItems();
			}
		}
		else
		{
			this.RefreshRefineSlotView(true);
		}
		this._currentTarget = CS$<>8__locals1.target;
	}

	// Token: 0x06001A9C RID: 6812 RVA: 0x000B1AC8 File Offset: 0x000AFCC8
	private void ChangeCurrentTargetOnWeave(ItemDisplayData target)
	{
		UI_Make.<>c__DisplayClass193_0 CS$<>8__locals1 = new UI_Make.<>c__DisplayClass193_0();
		CS$<>8__locals1.target = target;
		this.RefreshTargetList(this._equipTogGroup, this._itemViewEquip, new Func<ItemDisplayData, bool>(CS$<>8__locals1.<ChangeCurrentTargetOnWeave>g__Filter|0));
		this._slotItemArray[0] = (this._originSlotItemArray[0] = this._invalidItemDisplayData);
		this._currentTarget = CS$<>8__locals1.target;
		this.RefreshWeaveSlotView();
	}

	// Token: 0x06001A9D RID: 6813 RVA: 0x000B1B30 File Offset: 0x000AFD30
	private void ChangeCurrentTargetOnMake(ItemDisplayData target, bool hasChanged, bool forceRefresh)
	{
		ItemDisplayData currentTarget = this._currentTarget;
		bool flag;
		if (!(((currentTarget != null) ? new sbyte?(currentTarget.Key.ItemType) : null) != 5))
		{
			List<short> craftableItemTypes2 = Config.Material.Instance[this._currentTarget.Key.TemplateId].CraftableItemTypes;
			flag = (craftableItemTypes2 != null && craftableItemTypes2.Count > 0);
		}
		else
		{
			flag = false;
		}
		bool lastTargetIsMedicineMaterial = flag;
		this._needKeepSubtypeSelectionOnNoneTarget = false;
		List<ItemDisplayData> itemSource = this.GetItemsSource(this._materialTogGroup);
		bool flag2 = target == null || (this._currentTarget != null && target.ContainsItemKey(this._currentTarget.Key));
		if (flag2)
		{
			this.HideResourceSprite();
		}
		this._currentTarget = target;
		sbyte targetItemSourceType = (target != null) ? target.ItemSourceType : -1;
		ItemKey targetIemKey = (target != null) ? target.RealKey : ItemKey.Invalid;
		this._itemCacheTargets.Clear();
		this._itemCacheTargets.AddRange(from d in itemSource
		where d.Key.ItemType == 5 && (targetItemSourceType != d.ItemSourceType || !targetIemKey.TemplateEquals(d.RealKey)) && Config.Material.Instance[d.Key.TemplateId].CraftableItemTypes.Count > 0 && ItemTemplateHelper.GetCraftRequiredLifeSkillType(d.Key.ItemType, d.Key.TemplateId) == this._curLifeSkillType
		select d);
		this._itemViewMaterial.SetItemList(ref this._itemCacheTargets, false, null, false, null);
		if (hasChanged)
		{
			this._lastMakeResourceCount.Initialize();
			this._makeCount = 1;
			this._curMakeResourceCountInts.Initialize();
		}
		bool needRefresh = hasChanged || forceRefresh;
		bool flag3 = target == null || this.NeedLockOnMaking;
		if (flag3)
		{
			this._makeItemTypeId = -1;
			this._makeItemSubTypeId = -1;
			this._makeDropdownOptionList.Clear();
			this._lastMakeDropdownDataList.Clear();
			this._curMakeDropdownDataList.Clear();
			this._makeDropdown.value = 0;
			bool flag4 = lastTargetIsMedicineMaterial;
			if (flag4)
			{
				this._needKeepSubtypeSelectionOnNoneTarget = true;
			}
			else
			{
				this._curMakeItemSubToggleIndex = -1;
			}
			this._makePageSwitchController.gameObject.SetActive(false);
			this.ShowMakeRequireResourceList(false);
			this.ShowMakeEffectList(false);
			this.ShowMakeTotalCount(false);
			this._makePanel.gameObject.SetActive(false);
			this.ShowResultPreviewImage(false);
			this._previewTip.enabled = false;
			this.PlayCenterAnim(false, false, false);
			this.RefreshButtonPerfect(target == null);
		}
		else
		{
			bool flag5 = needRefresh;
			if (flag5)
			{
				this._makeTypeList.Clear();
				this._makeTypeDict.Clear();
				List<short> craftableItemTypes = Config.Material.Instance[target.Key.TemplateId].CraftableItemTypes;
				craftableItemTypes.ForEach(delegate(short i)
				{
					this._makeTypeList.Add(i);
					this._makeTypeDict[i] = MakeItemType.Instance[i].MakeItemSubTypes;
				});
				int lastIndex = -1;
				bool flag6 = this._makeItemTypeId > -1;
				if (flag6)
				{
					string lastName = MakeItemType.Instance[this._makeItemTypeId].TypeName;
					lastIndex = this._makeTypeList.FindIndex((short d) => MakeItemType.Instance[d].TypeName == lastName);
				}
				int index = (lastIndex > -1) ? lastIndex : 0;
				this._makePageSwitchController.InitPageCount(this._makeTypeList.Count, index, false);
			}
			bool flag7 = this._finishedMakeItemData != null;
			if (flag7)
			{
				this.OnSelectMakePageIndexChange(this._makePageSwitchController.CurPageIndex);
				this._finishedMakeItemData = null;
				this._needAutoFillResourceOnMakeEnd = true;
			}
			this._makePageSwitchController.gameObject.SetActive(true);
			this.ShowMakeRequireResourceList(true);
			this.ShowMakeEffectList(true);
			this.ShowMakeTotalCount(true);
			this._makePanel.gameObject.SetActive(true);
			this.PlayCenterAnim(false, false, false);
			this.RefreshButtonPerfect(false);
		}
	}

	// Token: 0x06001A9E RID: 6814 RVA: 0x000B1EC4 File Offset: 0x000B00C4
	private void ChangeCurrentTargetOnRepair(ItemDisplayData target)
	{
		UI_Make.<>c__DisplayClass195_0 CS$<>8__locals1 = new UI_Make.<>c__DisplayClass195_0();
		CS$<>8__locals1.target = target;
		CS$<>8__locals1.<>4__this = this;
		this._currentTarget = CS$<>8__locals1.target;
		this.RefreshTargetList(this._equipTogGroup, this._itemViewEquip, new Func<ItemDisplayData, bool>(CS$<>8__locals1.<ChangeCurrentTargetOnRepair>g__Filter|0));
		TextMeshProUGUI curDurabilityLabel = base.CGet<Refers>("Repair").CGet<TextMeshProUGUI>("Cur");
		TextMeshProUGUI maxDurabilityLabel = base.CGet<Refers>("Repair").CGet<TextMeshProUGUI>("Max");
		this._maxMakeResourceTotalCount = 0;
		this._maxMakeResourceCountInts.Initialize();
		this._curMakeResourceCountInts.Initialize();
		bool flag = CS$<>8__locals1.target == null;
		if (flag)
		{
			this.ShowMakeRequireResourceList(false);
			this.ShowMakeEffectList(false);
			curDurabilityLabel.transform.parent.parent.gameObject.SetActive(false);
		}
		else
		{
			bool isSpecial = this._currentTarget.Key.ItemType == 2 && Accessory.Instance[this._currentTarget.Key.TemplateId].BonusCombatSkillSect > -1;
			short makeItemSubType = ItemTemplateHelper.GetEquipmentMakeItemSubType(this._currentTarget.Key.ItemType, this.MakeOriginTemplateId);
			MakeItemSubTypeItem makeItemSubTypeConfig = MakeItemSubType.Instance[makeItemSubType];
			this._maxMakeResourceTotalCount = makeItemSubTypeConfig.ResourceTotalCount;
			for (sbyte resourceType = 0; resourceType < 6; resourceType += 1)
			{
				this._maxMakeResourceCountInts.Set((int)resourceType, (int)makeItemSubTypeConfig.MaxMaterialResources.Get((int)resourceType));
			}
			this.ShowMakeRequireResourceList(true);
			this._curMakeResourceCountInts.Initialize();
			for (int i = 0; i < 6; i++)
			{
				this._curMakeResourceCountInts.Set(i, (int)(isSpecial ? makeItemSubTypeConfig.MaxMaterialResources.Get(i) : this._currentTarget.MaterialResources.Get(i)));
			}
			curDurabilityLabel.transform.parent.parent.gameObject.SetActive(true);
			LanguageKey key = ((float)this._currentTarget.Durability > (float)this._currentTarget.MaxDurability * 0.5f) ? LanguageKey.LK_Make_Resource_Require_Meet : LanguageKey.LK_Make_Resource_Require_Not_Meet;
			curDurabilityLabel.text = LocalStringManager.GetFormat(key, this._currentTarget.Durability, this._currentTarget.MaxDurability);
			maxDurabilityLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Require_Meet, this._currentTarget.MaxDurability, this._currentTarget.MaxDurability);
		}
	}

	// Token: 0x06001A9F RID: 6815 RVA: 0x000B215C File Offset: 0x000B035C
	private void ChangeCurrentTargetOnAddPoison(ItemDisplayData target)
	{
		UI_Make.<>c__DisplayClass196_0 CS$<>8__locals1 = new UI_Make.<>c__DisplayClass196_0();
		CS$<>8__locals1.target = target;
		this.ChangeCurrentTargetAboutPoison(CS$<>8__locals1.target);
		bool flag = CS$<>8__locals1.target != null;
		if (flag)
		{
			this._poisonPageSwitchController.gameObject.SetActive(true);
			this._poisonPageSwitchController.InitPageCount(this._mixedPoisonMedicineIdList.Count, -1, false);
			bool flag2 = CS$<>8__locals1.target.HasAnyPoison && !CS$<>8__locals1.target.PoisonIsIdentified;
			if (flag2)
			{
				this.ShowUnidentifiedPoisonTip(true, false);
			}
		}
		else
		{
			this.ClearSelectedPoisonPage();
		}
		this.RefreshTargetList(this._materialTogGroup, this._itemViewMaterial, new Func<ItemDisplayData, bool>(CS$<>8__locals1.<ChangeCurrentTargetOnAddPoison>g__Filter|0));
		this._itemViewSlot.ReRender();
	}

	// Token: 0x06001AA0 RID: 6816 RVA: 0x000B2220 File Offset: 0x000B0420
	private void ChangeCurrentTargetOnRemovePoison(ItemDisplayData target)
	{
		UI_Make.<>c__DisplayClass197_0 CS$<>8__locals1 = new UI_Make.<>c__DisplayClass197_0();
		CS$<>8__locals1.target = target;
		this.ChangeCurrentTargetAboutPoison(CS$<>8__locals1.target);
		this.RefreshPoisonSlotView(false);
		this.RefreshTargetList(this._materialTogGroup, this._itemViewMaterial, new Func<ItemDisplayData, bool>(CS$<>8__locals1.<ChangeCurrentTargetOnRemovePoison>g__Filter|0));
		this._itemViewSlot.ReRender();
	}

	// Token: 0x06001AA1 RID: 6817 RVA: 0x000B227C File Offset: 0x000B047C
	private static bool CheckIsIdentified(ItemDisplayData d)
	{
		bool? flag;
		if (d == null)
		{
			flag = null;
		}
		else
		{
			FullPoisonEffects poisonEffects = d.PoisonEffects;
			flag = ((poisonEffects != null) ? new bool?(poisonEffects.IsIdentified) : null);
		}
		bool? flag2 = flag;
		return flag2.GetValueOrDefault();
	}

	// Token: 0x06001AA2 RID: 6818 RVA: 0x000B22C0 File Offset: 0x000B04C0
	private void ChangeCurrentTargetAboutPoison(ItemDisplayData target)
	{
		bool flag = this._currentTarget == target;
		if (!flag)
		{
			bool hasChanged = this.CheckTargetHasChanged(target);
			this._currentTarget = target;
			bool flag2 = hasChanged || (this._hasIdentifiedResult && this._curIdentifiedResultHasPoison);
			if (flag2)
			{
				this.ResetPoisonSlots(this._currentTarget);
				this.RefreshPoisonEffects(this._currentTarget);
			}
			this.RefreshButtonIdentity();
		}
	}

	// Token: 0x06001AA3 RID: 6819 RVA: 0x000B232C File Offset: 0x000B052C
	private void RefreshPoisonEffects(ItemDisplayData target)
	{
		bool flag = target != null && target.HasAnyPoison && target.PoisonEffects != null;
		if (flag)
		{
			this._tempPoisonEffects.Assign(target.PoisonEffects);
		}
		else
		{
			this._tempPoisonEffects.Clear();
		}
		this._originPoisonEffects.Assign(this._tempPoisonEffects);
	}

	// Token: 0x06001AA4 RID: 6820 RVA: 0x000B238C File Offset: 0x000B058C
	private void ResetPoisonSlots(ItemDisplayData target)
	{
		this.ClearCondense();
		for (int index = 0; index < this.SlotCount; index++)
		{
			short templateId = UI_Make.CheckIsIdentified(target) ? target.PoisonEffects.GetMedicineTemplateIdAt(index) : -1;
			ItemDisplayData itemData = this._slotItemArray[index];
			ItemDisplayData originItemData = this._originSlotItemArray[index];
			bool flag = itemData != null && itemData.Key.IsValid() && (!originItemData.Key.IsValid() || itemData.Key.TemplateId != originItemData.Key.TemplateId);
			if (flag)
			{
				this.RemoveSlotItem(itemData, index, false);
				itemData = null;
			}
			bool flag2 = templateId != -1;
			if (flag2)
			{
				int id = (itemData != null) ? itemData.Key.Id : -1;
				ItemKey itemKey = new ItemKey(8, 0, templateId, id);
				itemData = new ItemDisplayData
				{
					Key = itemKey,
					ItemSourceType = 0,
					Amount = 1
				};
			}
			else
			{
				itemData = this._invalidItemDisplayData;
			}
			this._slotItemArray[index] = (this._originSlotItemArray[index] = itemData);
		}
	}

	// Token: 0x06001AA5 RID: 6821 RVA: 0x000B24B4 File Offset: 0x000B06B4
	private void RefreshToolAttainment(bool show)
	{
		RectTransform layout = base.CGet<RectTransform>("ToolLifeSkillList");
		layout.gameObject.SetActive(show);
		bool flag = !show;
		if (!flag)
		{
			CraftToolItem toolConfig = CraftTool.Instance[this._currentTool.Key.TemplateId];
			for (int i = 0; i < GameData.Domains.Character.LifeSkillType.CraftingTypes.Length; i++)
			{
				sbyte skillType = GameData.Domains.Character.LifeSkillType.CraftingTypes[i];
				bool showSkill = toolConfig.RequiredLifeSkillTypes.Contains(skillType);
				bool showRefineSkill = false;
				bool flag2 = showSkill;
				if (flag2)
				{
					bool flag3 = this._curTab == UI_Make.UIMakeTab.Refine;
					if (flag3)
					{
						bool flag4 = this._currentTarget != null;
						if (flag4)
						{
							for (int j = 0; j < this.SlotCount; j++)
							{
								short oldId = this._originSlotItemArray[j].Key.TemplateId;
								short curId = this._slotItemArray[j].Key.TemplateId;
								bool flag5 = curId != -1 || oldId != -1;
								if (flag5)
								{
									short id = curId;
									bool flag6 = curId == -1;
									if (flag6)
									{
										id = oldId;
									}
									MaterialItem materialConfig = Config.Material.Instance[id];
									bool flag7 = skillType == materialConfig.RequiredLifeSkillType;
									if (flag7)
									{
										showRefineSkill = true;
										break;
									}
								}
							}
						}
						showSkill = showRefineSkill;
					}
					else
					{
						showSkill = (skillType == this._curLifeSkillType);
					}
				}
				Refers refers = layout.transform.GetChild(i).GetComponent<Refers>();
				refers.gameObject.SetActive(showSkill);
				bool flag8 = showSkill;
				if (flag8)
				{
					short skillAttainment = this._lifeSkillAttainments.Get((int)skillType);
					short toolAttainment = UI_Make.GetToolAttainment(this._currentTool.Key.TemplateId, skillAttainment, this._curLifeSkillType);
					string toolAttainmentText = (toolAttainment >= 0) ? string.Format("+{0}", toolAttainment) : toolAttainment.ToString().SetColor("brightred");
					LifeSkillTypeItem skillConfig = Config.LifeSkillType.Instance[skillType];
					refers.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.GetFormat(LanguageKey.LK_Tool_Add_Attainment, skillConfig.Name);
					refers.CGet<TextMeshProUGUI>("Value").text = toolAttainmentText;
					refers.CGet<CImage>("Icon").SetSprite(skillConfig.DisplayIcon, false, null);
				}
			}
		}
	}

	// Token: 0x06001AA6 RID: 6822 RVA: 0x000B2710 File Offset: 0x000B0910
	public override void QuickHide()
	{
		bool focusMode = this._craftsManExtraOperatePanel.CraftsManAddResourcePanel.FocusMode;
		if (focusMode)
		{
			this._craftsManExtraOperatePanel.CraftsManAddResourcePanel.SetAddResourceFocusMode(false);
		}
		else
		{
			bool activeSelf = this._makeDropdownMask.gameObject.activeSelf;
			if (activeSelf)
			{
				this._makeDropdown.Hide();
			}
			else
			{
				AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
				base.QuickHide();
				GEvent.OnEvent(UiEvents.CloseMakePage, null);
			}
		}
	}

	// Token: 0x06001AA7 RID: 6823 RVA: 0x000B2794 File Offset: 0x000B0994
	protected override void OnClick(Transform btn)
	{
		string name = btn.name;
		string a = name;
		if (!(a == "ButtonCancel"))
		{
			if (!(a == "ButtonConfirm"))
			{
				if (a == "MakeDropdownMask")
				{
					this.ShowMakeDropdownMask(false);
				}
			}
			else
			{
				UIElement.FullScreenMask.Show();
				this._onClickConfirm = null;
				this._buttonConfirm.interactable = false;
				switch (this._curTab)
				{
				case UI_Make.UIMakeTab.Make:
				{
					bool flag = this.NeedLockOnMaking && this._workingMakeItemData.LeftTime == 0;
					if (flag)
					{
						this.ConfirmGetMakeItems();
					}
					else
					{
						this.ConfirmMake();
					}
					break;
				}
				case UI_Make.UIMakeTab.Repair:
					this._onClickConfirm = new Action(this.ConfirmRepair);
					this.PlayCenterAnim(true, false, false);
					break;
				case UI_Make.UIMakeTab.Poison:
					this._onClickConfirm = new Action(this.ConfirmPoison);
					this.PlayCenterAnim(true, false, false);
					break;
				case UI_Make.UIMakeTab.RemovePoison:
					this._onClickConfirm = new Action(this.ConfirmRemovePoison);
					this.PlayCenterAnim(true, false, false);
					break;
				case UI_Make.UIMakeTab.Refine:
					this._onClickConfirm = new Action(this.ConfirmRefine);
					this.PlayCenterAnim(true, false, false);
					break;
				case UI_Make.UIMakeTab.Weave:
					this._onClickConfirm = new Action(this.ConfirmWeave);
					this.ShowResultPreviewImage(false);
					this.PlayCenterAnim(true, false, false);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}
		else
		{
			base.QuickHide();
		}
	}

	// Token: 0x06001AA8 RID: 6824 RVA: 0x000B291C File Offset: 0x000B0B1C
	private void CheckCondition(bool targetOrToolHasChanged = false)
	{
		this._buttonConfirm.GetComponent<Refers>().CGet<GameObject>("Start").SetActive(true);
		this._buttonConfirm.GetComponent<Refers>().CGet<GameObject>("Get").SetActive(false);
		switch (this._curTab)
		{
		case UI_Make.UIMakeTab.None:
		case UI_Make.UIMakeTab.CraftsmanPanel:
			break;
		case UI_Make.UIMakeTab.Make:
		{
			bool needRefreshMakeResult = targetOrToolHasChanged && this._currentTarget != null && !this.NeedLockOnMaking;
			this.CheckMakeCondition(needRefreshMakeResult, null);
			break;
		}
		case UI_Make.UIMakeTab.Repair:
			this.CheckRepairCondition();
			break;
		case UI_Make.UIMakeTab.Poison:
			this.CheckPoisonCondition();
			break;
		case UI_Make.UIMakeTab.RemovePoison:
			this.CheckRemovePoisonCondition();
			break;
		case UI_Make.UIMakeTab.Refine:
			this.CheckRefineCondition();
			break;
		case UI_Make.UIMakeTab.Weave:
			this.CheckWeaveCondition();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		this.RefreshWeaveClothingView();
		this.ShowMakeTime();
	}

	// Token: 0x06001AA9 RID: 6825 RVA: 0x000B29FC File Offset: 0x000B0BFC
	private short GetLifeSkillTotalAttainment(sbyte type)
	{
		short attainment = this._lifeSkillAttainments.Get((int)type);
		bool flag = this._currentTool == null;
		short result;
		if (flag)
		{
			result = attainment;
		}
		else
		{
			bool flag2 = this.IsEmptyTool(this._currentTool);
			if (flag2)
			{
				short finalAttainment = UI_Make.GetFinalAttainment(this._currentTool.Key.TemplateId, attainment, this._curLifeSkillType);
				result = finalAttainment;
			}
			else
			{
				CraftToolItem toolConfig = CraftTool.Instance[this._currentTool.Key.TemplateId];
				bool flag3 = toolConfig != null && toolConfig.RequiredLifeSkillTypes.Contains(type);
				if (flag3)
				{
					short finalAttainment2 = UI_Make.GetFinalAttainment(this._currentTool.Key.TemplateId, attainment, this._curLifeSkillType);
					result = finalAttainment2;
				}
				else
				{
					result = attainment;
				}
			}
		}
		return result;
	}

	// Token: 0x06001AAA RID: 6826 RVA: 0x000B2ABD File Offset: 0x000B0CBD
	private bool IsEmptyTool(ItemDisplayData data)
	{
		return this.IsEmptyTool(data.Key);
	}

	// Token: 0x06001AAB RID: 6827 RVA: 0x000B2ACB File Offset: 0x000B0CCB
	private bool IsEmptyTool(ItemKey itemKey)
	{
		return ItemTemplateHelper.IsEmptyTool(itemKey.ItemType, itemKey.TemplateId);
	}

	// Token: 0x06001AAC RID: 6828 RVA: 0x000B2AE0 File Offset: 0x000B0CE0
	private short GetToolDurabilityCost(ItemDisplayData tool, sbyte materialGrade)
	{
		bool flag = tool == null || !tool.Key.IsValid() || this.IsEmptyTool(tool);
		short result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			result = CraftTool.Instance[tool.Key.TemplateId].DurabilityCost[(int)materialGrade];
		}
		return result;
	}

	// Token: 0x06001AAD RID: 6829 RVA: 0x000B2B34 File Offset: 0x000B0D34
	public static int GetEmptyToolAttainmentBonus(sbyte lifeSkillType)
	{
		ProfessionModel professionModel = SingletonObject.getInstance<ProfessionModel>();
		bool skillTypeIsMeet = lifeSkillType == 7 || lifeSkillType == 6 || lifeSkillType == 11 || lifeSkillType == 10;
		bool flag = skillTypeIsMeet && professionModel.IsProfessionalSkillUnlockedAndEquipped(8);
		int result;
		if (flag)
		{
			ProfessionData professionData = professionModel.GetProfessionData(2);
			int bonus = professionData.GetSeniorityEmptyToolAttainmentBonus();
			result = bonus;
		}
		else
		{
			result = ProfessionData.SeniorityToEmptyToolAttainmentBonus(0);
		}
		return result;
	}

	// Token: 0x06001AAE RID: 6830 RVA: 0x000B2B94 File Offset: 0x000B0D94
	public static short GetToolAttainment(short templateId, sbyte lifeSkillType)
	{
		bool flag = templateId < 0;
		short result2;
		if (flag)
		{
			result2 = 0;
		}
		else
		{
			CraftToolItem toolConfig = CraftTool.Instance[templateId];
			bool skillTypeIsMeet = lifeSkillType == 7 || lifeSkillType == 6 || lifeSkillType == 11 || lifeSkillType == 10;
			bool flag2 = !skillTypeIsMeet;
			if (flag2)
			{
				result2 = toolConfig.AttainmentBonus;
			}
			else
			{
				ProfessionModel professionModel = SingletonObject.getInstance<ProfessionModel>();
				bool equipped = professionModel.IsProfessionalSkillUnlockedAndEquipped(9);
				short attainment = toolConfig.AttainmentBonus;
				bool flag3 = equipped;
				if (flag3)
				{
					ProfessionData professionData = professionModel.GetProfessionData(2);
					int result = (int)attainment * (100 + professionData.GetSeniorityAttainmentBonus()) / 100;
					attainment = (short)result;
				}
				result2 = attainment;
			}
		}
		return result2;
	}

	// Token: 0x06001AAF RID: 6831 RVA: 0x000B2C30 File Offset: 0x000B0E30
	public static short GetFinalAttainment(short toolTemplateId, short skillAttainment, sbyte lifeSkillType)
	{
		short toolAttainment = UI_Make.GetToolAttainment(toolTemplateId, skillAttainment, lifeSkillType);
		return Convert.ToInt16((int)(skillAttainment + toolAttainment));
	}

	// Token: 0x06001AB0 RID: 6832 RVA: 0x000B2C58 File Offset: 0x000B0E58
	public static short GetToolAttainment(short toolTemplateId, short skillAttainment, sbyte lifeSkillType)
	{
		bool flag = ItemTemplateHelper.IsEmptyTool(6, toolTemplateId);
		short result;
		if (flag)
		{
			int bonus = UI_Make.GetEmptyToolAttainmentBonus(lifeSkillType);
			int toolAttainment = (int)skillAttainment * bonus / 100;
			result = (short)toolAttainment;
		}
		else
		{
			short toolAttainment2 = UI_Make.GetToolAttainment(toolTemplateId, lifeSkillType);
			result = toolAttainment2;
		}
		return result;
	}

	// Token: 0x06001AB1 RID: 6833 RVA: 0x000B2C98 File Offset: 0x000B0E98
	private bool CheckAndShowLifeSkill(LifeSkillShorts needLifeSkill, LifeSkillShorts showLifeSkill)
	{
		bool lifeSkillMeet = true;
		this._requireLifeSkillList.gameObject.SetActive(true);
		for (int i = 0; i < GameData.Domains.Character.LifeSkillType.CraftingTypes.Length; i++)
		{
			sbyte skillType = GameData.Domains.Character.LifeSkillType.CraftingTypes[i];
			short finalLifeSkillAttainment = this.GetLifeSkillTotalAttainment(skillType);
			bool curSkillMeet = true;
			int needAttainment = Mathf.Max(0, (int)needLifeSkill.Get((int)skillType));
			bool flag = (int)finalLifeSkillAttainment < needAttainment;
			if (flag)
			{
				lifeSkillMeet = false;
				curSkillMeet = false;
			}
			LifeSkillTypeItem skillConfig = Config.LifeSkillType.Instance[skillType];
			Refers refers = this._requireLifeSkillList.transform.GetChild(i).GetComponent<Refers>();
			refers.CGet<CImage>("Icon").SetSprite(skillConfig.DisplayIcon, false, null);
			string color = curSkillMeet ? "brightblue" : "brightred";
			refers.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.GetFormat(LanguageKey.LK_Make_LifeSkill_Require_Name, skillConfig.Name);
			refers.CGet<TextMeshProUGUI>("Value").text = LocalStringManager.GetFormat(LanguageKey.LK_Make_LifeSkill_Require_Value, finalLifeSkillAttainment.ToString().SetColor(color), needAttainment);
			refers.gameObject.SetActive(showLifeSkill.Get((int)skillType) > 0);
		}
		for (int j = 0; j < GameData.Domains.Character.LifeSkillType.CraftingTypes.Length; j++)
		{
			Refers refers2 = this._requireLifeSkillList.transform.GetChild(j).GetComponent<Refers>();
			this.RefreshAttainmentTip(refers2.CGet<TooltipInvoker>("Tip"));
		}
		return lifeSkillMeet;
	}

	// Token: 0x06001AB2 RID: 6834 RVA: 0x000B2E1E File Offset: 0x000B101E
	private void HideLifeSkill()
	{
		this._requireLifeSkillList.gameObject.SetActive(false);
	}

	// Token: 0x06001AB3 RID: 6835 RVA: 0x000B2E34 File Offset: 0x000B1034
	private void SetConfirmButtonTip(int contentKey = -1, bool enabled = true, TipType type = TipType.SingleDesc, ItemDisplayData itemData = null)
	{
		this._confirmButtonTipDisplayer.enabled = enabled;
		this._confirmButtonTipDisplayer.Type = type;
		StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
		stringBuilder.Clear();
		bool flag = -1 != contentKey;
		if (flag)
		{
			stringBuilder.AppendLine(LocalStringManager.Get((LanguageKey)contentKey));
		}
		bool flag2 = this._curTab == UI_Make.UIMakeTab.Refine;
		if (flag2)
		{
			stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Make_Refine_Confirm_Tip));
		}
		else
		{
			bool flag3 = this._curTab == UI_Make.UIMakeTab.Poison;
			if (flag3)
			{
				stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Make_Poison_Confirm_Tip));
			}
			else
			{
				bool flag4 = this._curTab == UI_Make.UIMakeTab.Weave;
				if (flag4)
				{
					stringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_Make_Weave_Confirm_Tip));
				}
			}
		}
		string content = stringBuilder.ToString();
		this._confirmButtonTipDisplayer.PresetParam[0] = content;
		bool flag5 = content.IsNullOrEmpty();
		if (flag5)
		{
			this._confirmButtonTipDisplayer.enabled = false;
		}
		EasyPool.Free<StringBuilder>(stringBuilder);
		this._confirmButtonTipDisplayer.RuntimeParam = ((itemData == null) ? null : new ArgumentBox().SetObject("ItemData", itemData));
		bool flag6 = this._confirmButtonTipDisplayer.Showing && this._confirmButtonTipDisplayer.enabled;
		if (flag6)
		{
			this._confirmButtonTipDisplayer.ShowTips();
		}
		else
		{
			this._confirmButtonTipDisplayer.HideTips();
		}
	}

	// Token: 0x06001AB4 RID: 6836 RVA: 0x000B2F78 File Offset: 0x000B1178
	private int GetBuildingAttainmentEffect(sbyte lifeSkillType)
	{
		return (lifeSkillType == this._curLifeSkillType) ? this._buildingAttainmentEffect : 0;
	}

	// Token: 0x06001AB5 RID: 6837 RVA: 0x000B2F9C File Offset: 0x000B119C
	private short GetAttainmentByBuildingEffect(sbyte lifeSkillType, short requiredAttainment)
	{
		int attainmentEffect = this.GetBuildingAttainmentEffect(lifeSkillType);
		requiredAttainment = GameData.Domains.Building.SharedMethods.GetRequiredLifeSkillAttainmentByBuildingEffect((int)requiredAttainment, attainmentEffect);
		return requiredAttainment;
	}

	// Token: 0x06001AB6 RID: 6838 RVA: 0x000B2FC0 File Offset: 0x000B11C0
	private void ShowResultPreviewImage(bool show)
	{
		this.ResultPreviewImage.SetActive(show);
	}

	// Token: 0x06001AB7 RID: 6839 RVA: 0x000B2FD0 File Offset: 0x000B11D0
	private void ShowMakeTime()
	{
		bool showTime = false;
		bool flag = this._currentTarget != null;
		if (flag)
		{
			bool flag2 = this._curTab == UI_Make.UIMakeTab.Make;
			if (flag2)
			{
				showTime = true;
			}
			else
			{
				bool flag3 = this._curTab == UI_Make.UIMakeTab.Weave;
				if (flag3)
				{
					showTime = this._previewTip.enabled;
				}
			}
		}
		this._makeTimeRoot.gameObject.SetActive(showTime);
	}

	// Token: 0x06001AB8 RID: 6840 RVA: 0x000B3030 File Offset: 0x000B1230
	private void RefreshAttainmentTip(TooltipInvoker tipDisplayer)
	{
		bool flag = this._currentTool == null;
		if (flag)
		{
			tipDisplayer.enabled = false;
		}
		else
		{
			bool isEmptyTool = this._currentTool.Key == this._emptyToolKey;
			CraftToolItem toolConfig = CraftTool.Instance[this._currentTool.Key.TemplateId];
			tipDisplayer.enabled = true;
			List<HealAttainmentTipsHelper.AttainmentItem> attainmentItems = EasyPool.Get<List<HealAttainmentTipsHelper.AttainmentItem>>();
			attainmentItems.Clear();
			for (int index = 0; index < GameData.Domains.Character.LifeSkillType.CraftingTypes.Length; index++)
			{
				sbyte tipSkillType = GameData.Domains.Character.LifeSkillType.CraftingTypes[index];
				bool flag2 = !this._requireLifeSkillList.GetChild(index).gameObject.activeSelf || !toolConfig.RequiredLifeSkillTypes.Contains(tipSkillType);
				if (!flag2)
				{
					short toolAttainment = UI_Make.GetToolAttainment(this._currentTool.Key.TemplateId, this._lifeSkillAttainments.Get((int)tipSkillType), tipSkillType);
					int delta = (int)(toolConfig.RequiredLifeSkillTypes.Contains(tipSkillType) ? toolAttainment : 0);
					attainmentItems.Add(new HealAttainmentTipsHelper.AttainmentItem
					{
						SkillType = tipSkillType,
						DeltaAttainment = delta,
						Attainment = (int)this._lifeSkillAttainments.Get((int)tipSkillType)
					});
				}
			}
			bool flag3 = attainmentItems.Count > 0;
			if (flag3)
			{
				HealAttainmentTipsHelper.RefreshTips(tipDisplayer, this._currentTool.Key, isEmptyTool, attainmentItems, false);
			}
			EasyPool.Free<List<HealAttainmentTipsHelper.AttainmentItem>>(attainmentItems);
		}
	}

	// Token: 0x06001AB9 RID: 6841 RVA: 0x000B31A0 File Offset: 0x000B13A0
	private void InitSlot()
	{
		bool flag = this._slots != null;
		if (!flag)
		{
			this._slotTemplate = base.CGet<Refers>("SlotTemplate");
			this._slots = new Refers[UI_Make.MaxSlotCount];
			for (int index = 0; index < this._slots.Length; index++)
			{
				GameObject slot = Object.Instantiate<GameObject>(this._slotTemplate.gameObject, this._slotTemplate.transform.parent);
				slot.name = index.ToString();
				slot.SetActive(true);
				this._slots[index] = slot.GetComponent<Refers>();
			}
			this._slotTemplate.gameObject.SetActive(false);
			bool flag2 = this._itemViewSlot;
			if (flag2)
			{
				base.CGet<GameObject>("SlotPanel").SetActive(false);
			}
		}
	}

	// Token: 0x06001ABA RID: 6842 RVA: 0x000B3274 File Offset: 0x000B1474
	private void RefreshSlotCount()
	{
		bool flag = this._slots == null;
		if (!flag)
		{
			bool flag2 = this.SlotCount == 0;
			if (flag2)
			{
				this._slotTemplate.transform.parent.gameObject.SetActive(false);
			}
			else
			{
				this._slotTemplate.transform.parent.gameObject.SetActive(true);
				for (int i = 0; i < this._slots.Length; i++)
				{
					this._slots[i].gameObject.SetActive(i < this.SlotCount);
				}
			}
		}
	}

	// Token: 0x06001ABB RID: 6843 RVA: 0x000B3310 File Offset: 0x000B1510
	private void RemoveSlotItem(ItemDisplayData itemData, int index, bool sort = false)
	{
		bool flag = !itemData.Key.HasTemplate;
		if (flag)
		{
			this.RefreshSlots();
		}
		else
		{
			ItemSourceType itemSourceTypeEnum = itemData.ItemSourceTypeEnum;
			bool flag2 = itemSourceTypeEnum == ItemSourceType.Equipment;
			if (flag2)
			{
				itemSourceTypeEnum = this._curSlotItemSourceType;
			}
			List<ItemDisplayData> slotCacheItemList = this._slotItemListDict[itemSourceTypeEnum];
			ItemDisplayData cacheData = slotCacheItemList.Find((ItemDisplayData d) => d.Key.ItemType == itemData.Key.ItemType && d.Key.TemplateId == itemData.Key.TemplateId);
			bool flag3 = cacheData == null;
			if (flag3)
			{
				slotCacheItemList.Add(itemData);
				List<ItemDisplayData> itemsSource = this.GetItemsSource(itemSourceTypeEnum);
				itemsSource.Add(itemData);
			}
			else
			{
				cacheData.Amount += itemData.Amount;
			}
			bool flag4 = this._curTab == UI_Make.UIMakeTab.Weave;
			if (flag4)
			{
				this.RefreshWeaveSlotView();
			}
			else
			{
				List<ItemDisplayData> itemList = this.SlotCacheItemList;
				this._itemViewSlot.SetItemList(ref itemList, false, null, false, null);
			}
			bool flag5 = sort && this._curTab == UI_Make.UIMakeTab.Poison;
			if (flag5)
			{
				int curIndex = index;
				bool hasMoved = false;
				while (curIndex < this.SlotCount)
				{
					int nextIndex = curIndex + 1;
					bool canMove = (!hasMoved && nextIndex - index == 1) || hasMoved;
					bool flag6 = !this._originSlotItemArray[curIndex].Key.HasTemplate && canMove;
					if (flag6)
					{
						bool flag7 = nextIndex < this.SlotCount && !this._originSlotItemArray[curIndex].Key.HasTemplate && !this._originSlotItemArray[nextIndex].Key.HasTemplate && !this._slotItemArray[curIndex].Key.TemplateEquals(this._slotItemArray[nextIndex].Key);
						if (flag7)
						{
							this._slotItemArray[curIndex] = this._slotItemArray[nextIndex];
							hasMoved = true;
						}
						else
						{
							this._slotItemArray[curIndex] = this._originSlotItemArray[curIndex];
						}
					}
					curIndex = nextIndex;
				}
				bool flag8 = !hasMoved;
				if (flag8)
				{
					this._slotItemArray[index] = this._originSlotItemArray[index];
				}
			}
			else
			{
				bool curTabIsAboutPoison = this.CurTabIsAboutPoison;
				if (curTabIsAboutPoison)
				{
					this._slotItemArray[index] = this._originSlotItemArray[index];
				}
				else
				{
					this._slotItemArray[index] = this._invalidItemDisplayData;
				}
			}
			this._slotItemChangeDict[itemSourceTypeEnum].AddItem(itemData.Key, 1, 0);
			bool curTabIsAboutPoison2 = this.CurTabIsAboutPoison;
			if (curTabIsAboutPoison2)
			{
				bool flag9 = this._originPoisonEffects.PoisonSlotList.CheckIndex(index);
				if (flag9)
				{
					PoisonSlot slot = new PoisonSlot(this._originPoisonEffects.PoisonSlotList[index]);
					bool flag10 = !this._tempPoisonEffects.PoisonSlotList.CheckIndex(index);
					if (flag10)
					{
						this._tempPoisonEffects.PoisonSlotList.Insert(index, slot);
					}
					else
					{
						this._tempPoisonEffects.PoisonSlotList[index] = slot;
					}
				}
				else
				{
					bool flag11 = this._tempPoisonEffects.PoisonSlotList.CheckIndex(index);
					if (flag11)
					{
						if (sort)
						{
							this._tempPoisonEffects.PoisonSlotList.RemoveAt(index);
						}
						else
						{
							this._tempPoisonEffects.PoisonSlotList[index].Clear();
						}
					}
				}
			}
			this.RefreshSlots();
		}
	}

	// Token: 0x06001ABC RID: 6844 RVA: 0x000B3678 File Offset: 0x000B1878
	private void AddSlotItem(ItemDisplayData itemData, int index, ItemSourceType slotItemSourceType)
	{
		List<ItemDisplayData> itemList = this._slotItemListDict[slotItemSourceType];
		ItemDisplayData data = itemList.Find((ItemDisplayData d) => d.Key.TemplateId == itemData.Key.TemplateId);
		data.Amount--;
		bool flag = data.Amount <= 0;
		if (flag)
		{
			itemList.Remove(data);
			List<ItemDisplayData> itemsSource = this.GetItemsSource(slotItemSourceType);
			itemsSource.Remove(data);
		}
		this._itemViewSlot.SetItemList(ref itemList, false, null, false, null);
		ItemDisplayData slotData = itemData.Clone(-1);
		slotData.Amount = 1;
		this._slotItemArray[index] = slotData;
		this._slotItemChangeDict[slotItemSourceType].RemoveItem(itemData.Key, 1, 0);
		bool curTabIsAboutPoison = this.CurTabIsAboutPoison;
		if (curTabIsAboutPoison)
		{
			bool flag2 = !this._tempPoisonEffects.PoisonSlotList.CheckIndex(index);
			if (flag2)
			{
				this._tempPoisonEffects.PoisonSlotList.Add(new PoisonSlot());
			}
			this._tempPoisonEffects.PoisonSlotList[index].MedicineTemplateId = itemData.Key.TemplateId;
		}
		this.RefreshSlots();
	}

	// Token: 0x06001ABD RID: 6845 RVA: 0x000B37A8 File Offset: 0x000B19A8
	private void ShowUnidentifiedPoisonTip(bool show, bool materialHasPoison = false)
	{
		GameObject tip = base.CGet<GameObject>("UnidentifiedPoisonTip");
		tip.SetActive(show);
		bool flag = !show;
		if (!flag)
		{
			TextMeshProUGUI text = tip.GetComponentInChildren<TextMeshProUGUI>();
			LanguageKey key = materialHasPoison ? LanguageKey.LK_Remove_Poison_Material_Unidentified : ((this._curTab == UI_Make.UIMakeTab.Poison) ? LanguageKey.LK_Add_Poison_Has_Unidentified : LanguageKey.LK_Remove_Poison_Has_Unidentified);
			text.SetText(LocalStringManager.Get(key), true);
		}
	}

	// Token: 0x06001ABE RID: 6846 RVA: 0x000B380C File Offset: 0x000B1A0C
	private void ShowIdentifiedPoisonTip(bool show, Action action)
	{
		this._curIdentifiedResultAction = action;
		GameObject tip = base.CGet<GameObject>("IdentifiedPoisonTip");
		tip.SetActive(show);
		bool flag = !show;
		if (!flag)
		{
			TextMeshProUGUI text = tip.GetComponentInChildren<TextMeshProUGUI>();
			text.SetText(LocalStringManager.Get(LanguageKey.LK_Poison_Identifying), true);
			SkeletonGraphic spine = tip.GetComponentInChildren<SkeletonGraphic>();
			string animName = this._curIdentifiedResultHasPoison ? "bad" : "good";
			spine.timeScale = 2f;
			spine.AnimationState.SetAnimation(0, animName, false);
			spine.AnimationState.Complete -= this.AnimationStateOnEnd;
			spine.AnimationState.Complete += this.AnimationStateOnEnd;
		}
	}

	// Token: 0x06001ABF RID: 6847 RVA: 0x000B38C0 File Offset: 0x000B1AC0
	private void AnimationStateOnEnd(TrackEntry trackEntry)
	{
		GameObject tip = base.CGet<GameObject>("IdentifiedPoisonTip");
		TextMeshProUGUI text = tip.GetComponentInChildren<TextMeshProUGUI>();
		LanguageKey key = (!this._curIdentifySuccess) ? LanguageKey.LK_Poison_Identify_LifeSkill_NotMeet : (this._curIdentifiedResultHasPoison ? LanguageKey.LK_Poison_Identify_HasPoison : LanguageKey.LK_Poison_Identify_NoPoison);
		text.SetText(LocalStringManager.Get(key).ColorReplace(), true);
		Action curIdentifiedResultAction = this._curIdentifiedResultAction;
		if (curIdentifiedResultAction != null)
		{
			curIdentifiedResultAction();
		}
	}

	// Token: 0x06001AC0 RID: 6848 RVA: 0x000B392C File Offset: 0x000B1B2C
	private void OnRenderItemSlot(ItemDisplayData itemData, ItemView itemView)
	{
		UI_Make.<>c__DisplayClass230_0 CS$<>8__locals1 = new UI_Make.<>c__DisplayClass230_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.itemView = itemView;
		CS$<>8__locals1.itemData = itemData;
		CS$<>8__locals1.itemView.SetSelectState(false);
		CS$<>8__locals1.itemView.SetHighLight(false);
		CS$<>8__locals1.itemView.SetClickEvent(new UnityAction(CS$<>8__locals1.<OnRenderItemSlot>g__OnClick|0));
		this.SetVillagerNeedMark(CS$<>8__locals1.itemView, CS$<>8__locals1.itemData.ItemSourceTypeEnum);
		bool interactable = !CS$<>8__locals1.itemData.IsLocked;
		CS$<>8__locals1.itemView.SetLocked(!interactable);
		CS$<>8__locals1.itemView.SetInteractable(interactable);
		bool flag = !interactable;
		if (!flag)
		{
			bool needLock = false;
			switch (this._curTab)
			{
			case UI_Make.UIMakeTab.Poison:
			{
				needLock = !this.CheckPoisonItemCondition(CS$<>8__locals1.itemView);
				bool flag2 = !needLock && this._targetMixedPoisonMedicineId > -1;
				if (flag2)
				{
					MedicineItem medicineConfig = Medicine.Instance[CS$<>8__locals1.itemData.Key.TemplateId];
					sbyte poisonType = medicineConfig.PoisonType;
					sbyte mixedPoisonType = MixedPoisonType.FromMedicineTemplateId(this._targetMixedPoisonMedicineId);
					sbyte[] poisonTypes = MixedPoisonType.ToPoisonTypes[(int)mixedPoisonType];
					needLock = !poisonTypes.Contains(poisonType);
					bool flag3 = needLock;
					if (flag3)
					{
						CS$<>8__locals1.itemView.SetItemNotCanSelectReason(LocalStringManager.Get(LanguageKey.LK_Add_Poison_Type_Different).ColorReplace());
					}
				}
				break;
			}
			case UI_Make.UIMakeTab.RemovePoison:
			{
				needLock = !this.CheckPoisonItemCondition(CS$<>8__locals1.itemView);
				bool poisonIsIdentified = CS$<>8__locals1.itemView.Data.PoisonIsIdentified;
				if (poisonIsIdentified)
				{
					CS$<>8__locals1.itemView.SetItemNotCanSelectReason(LocalStringManager.Get(LanguageKey.LK_Remove_Poison_Has_Poison).ColorReplace());
					needLock = true;
				}
				break;
			}
			case UI_Make.UIMakeTab.Refine:
				needLock = (this._currentTarget == null);
				break;
			case UI_Make.UIMakeTab.Weave:
			{
				bool flag4 = this._currentTarget == null;
				if (flag4)
				{
					needLock = true;
				}
				else
				{
					short curId = CS$<>8__locals1.itemData.Key.TemplateId;
					bool flag5 = this._currentTarget.IsWeaved && curId == this._currentTarget.WeavedClothingTemplateId;
					if (flag5)
					{
						needLock = true;
					}
					else
					{
						bool flag6 = !this._currentTarget.IsWeaved && curId == this._currentTarget.Key.TemplateId;
						if (flag6)
						{
							needLock = true;
						}
					}
					ItemDisplayData slotData = this._slotItemArray.First<ItemDisplayData>();
					bool flag7 = slotData.Key.ItemType == 3 && slotData.Key.TemplateId == curId;
					if (flag7)
					{
						CS$<>8__locals1.itemView.SetSelectState(true);
						CS$<>8__locals1.itemView.SetHighLight(true);
					}
				}
				break;
			}
			}
			bool flag8 = this.CurTabIsAboutPoison && needLock;
			if (flag8)
			{
				CS$<>8__locals1.itemView.SetInteractable(false);
			}
			else
			{
				CS$<>8__locals1.itemView.SetLocked(needLock);
			}
		}
	}

	// Token: 0x06001AC1 RID: 6849 RVA: 0x000B3C10 File Offset: 0x000B1E10
	private bool CheckPoisonItemCondition(ItemView itemView)
	{
		bool flag = !itemView.Data.Key.IsValid();
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = this._currentTarget == null;
			if (flag2)
			{
				itemView.SetItemNotCanSelectReason(LocalStringManager.Get(LanguageKey.LK_Add_Poison_No_Target).ColorReplace());
				result = false;
			}
			else
			{
				bool flag3 = this._currentTarget.HasAnyPoison && !this._currentTarget.PoisonIsIdentified;
				if (flag3)
				{
					itemView.SetLocked(true);
					result = false;
				}
				else
				{
					LanguageKey contentKey = LanguageKey.Invalid;
					MedicineItem itemConfig = Medicine.Instance[itemView.Data.Key.TemplateId];
					sbyte poisonType = itemConfig.PoisonType;
					short poisonLevel = itemConfig.EffectThresholdValue;
					sbyte poisonGrade = itemConfig.Grade;
					bool flag4 = this._tempPoisonEffects.PoisonSlotList.Exists((PoisonSlot s) => s.IsValid && s.MedicineTemplateId == itemConfig.TemplateId) || this._originPoisonEffects.PoisonSlotList.Exists((PoisonSlot s) => s.IsValid && s.MedicineTemplateId == itemConfig.TemplateId);
					if (flag4)
					{
						contentKey = LanguageKey.LK_Add_Poison_Same;
					}
					else
					{
						bool flag5 = !this._tempPoisonEffects.PoisonSlotList.Exists((PoisonSlot s) => s.IsValid && s.MedicineConfig.PoisonType == poisonType);
						if (flag5)
						{
							bool flag6 = this._curTab == UI_Make.UIMakeTab.Poison;
							if (flag6)
							{
								bool flag7 = this._tempPoisonEffects.CurrentValidSlotCount == FullPoisonEffects.MaxSlotCount;
								if (flag7)
								{
									contentKey = LanguageKey.LK_Add_Poison_Type_Different;
								}
							}
							else
							{
								contentKey = LanguageKey.LK_Remove_Poison_Not_Meet;
							}
						}
						else
						{
							bool flag8 = this._tempPoisonEffects.PoisonSlotList.Exists((PoisonSlot s) => s.IsValid && s.MedicineConfig.PoisonType == poisonType && poisonGrade < s.MedicineConfig.Grade);
							if (flag8)
							{
								contentKey = ((this._curTab == UI_Make.UIMakeTab.Poison) ? LanguageKey.LK_Add_Poison_Value_Not_Enough : LanguageKey.LK_Remove_Poison_Not_Meet);
							}
						}
					}
					bool canUse = contentKey == LanguageKey.Invalid;
					bool flag9 = canUse;
					if (flag9)
					{
						itemView.HideInteractionState();
					}
					else
					{
						itemView.SetItemNotCanSelectReason(LocalStringManager.Get(contentKey).ColorReplace());
					}
					bool canAddCondense = this._isAddPoisonCondense && this.CheckCondenseItemCondition(itemView, poisonType, poisonLevel);
					result = (canUse || canAddCondense);
				}
			}
		}
		return result;
	}

	// Token: 0x06001AC2 RID: 6850 RVA: 0x000B3E38 File Offset: 0x000B2038
	private bool CheckCondenseItemCondition(ItemView itemView, sbyte poisonType, short poisonLevel)
	{
		bool needLock = false;
		LanguageKey contentKey = LanguageKey.Invalid;
		bool flag = this._tempPoisonEffects.PoisonSlotList.Exists((PoisonSlot s) => s.IsValid && s.MedicineConfig.PoisonType == poisonType && poisonLevel != s.MedicineConfig.EffectThresholdValue);
		if (flag)
		{
			needLock = true;
			contentKey = LanguageKey.LK_Add_Poison_Value_Not_Enough;
		}
		else
		{
			bool flag2 = this._tempPoisonEffects.PoisonSlotList.Exists((PoisonSlot s) => s.IsValid && s.MedicineConfig.PoisonType == poisonType && s.MedicineCountIsMax);
			if (flag2)
			{
				needLock = true;
				contentKey = LanguageKey.LK_Add_Poison_Same;
			}
			else
			{
				bool flag3 = !this._tempPoisonEffects.PoisonSlotList.Exists((PoisonSlot s) => s.IsValid && s.MedicineConfig.PoisonType == poisonType);
				if (flag3)
				{
					needLock = true;
					contentKey = LanguageKey.LK_Add_Poison_Type_Different;
				}
			}
		}
		itemView.SetItemNotCanSelectReason(LocalStringManager.Get(contentKey).ColorReplace());
		return !needLock;
	}

	// Token: 0x06001AC3 RID: 6851 RVA: 0x000B3F04 File Offset: 0x000B2104
	private void RefreshSlots()
	{
		for (int i = 0; i < this.SlotCount; i++)
		{
			UI_Make.<>c__DisplayClass233_0 CS$<>8__locals1 = new UI_Make.<>c__DisplayClass233_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.index = i;
			ref Refers refers = ref this._slots[CS$<>8__locals1.index];
			ItemView itemView = refers.CGet<ItemView>("ItemView");
			ItemDisplayData origin = this._originSlotItemArray[CS$<>8__locals1.index];
			ItemDisplayData cur = this._slotItemArray[CS$<>8__locals1.index];
			CS$<>8__locals1.itemData = cur;
			bool curTabIsAboutPoison = this.CurTabIsAboutPoison;
			if (curTabIsAboutPoison)
			{
				bool flag = CS$<>8__locals1.itemData == null || !CS$<>8__locals1.itemData.Key.HasTemplate;
				if (flag)
				{
					CS$<>8__locals1.itemData = origin;
				}
			}
			bool flag2 = CS$<>8__locals1.itemData == null;
			if (flag2)
			{
				CS$<>8__locals1.itemData = this._invalidItemDisplayData;
			}
			itemView.ShowInvalidIcon = false;
			itemView.EnbaleTip = (CS$<>8__locals1.itemData != this._invalidItemDisplayData);
			itemView.SetData(CS$<>8__locals1.itemData, false, -1, false, true, null, false, true);
			TooltipInvoker tip = itemView.GetMouseTip();
			List<PoisonSlot> poisonSlotList = this._originPoisonEffects.PoisonSlotList;
			PoisonSlot originSlot = ((poisonSlotList != null) ? poisonSlotList.GetOrDefault(i) : null) ?? null;
			List<PoisonSlot> poisonSlotList2 = this._tempPoisonEffects.PoisonSlotList;
			PoisonSlot tempSlot = ((poisonSlotList2 != null) ? poisonSlotList2.GetOrDefault(i) : null) ?? null;
			bool slotChanged = this.CurTabIsAboutPoison ? ((originSlot != null && !originSlot.Equals(tempSlot)) || (tempSlot != null && !tempSlot.Equals(originSlot))) : (cur != null && origin != null && !cur.Key.TemplateEquals(origin.Key));
			itemView.SetHighLight(slotChanged);
			bool isEmpty = !CS$<>8__locals1.itemData.Key.HasTemplate;
			bool isClothing = this._curTab == UI_Make.UIMakeTab.Weave;
			UI_Make.UIMakeTab curTab = this._curTab;
			bool isMedicine = curTab == UI_Make.UIMakeTab.Poison || curTab == UI_Make.UIMakeTab.RemovePoison;
			bool isMaterial = !isClothing && !isMedicine;
			refers.CGet<GameObject>("Clothing").SetActive(isEmpty && isClothing);
			refers.CGet<GameObject>("Medicine").SetActive(isEmpty && isMedicine);
			refers.CGet<GameObject>("Material").SetActive(isEmpty && isMaterial);
			bool interactable = this._currentTarget != null && CS$<>8__locals1.itemData.Key.HasTemplate;
			bool flag3 = interactable && this.CurTabIsAboutPoison;
			if (flag3)
			{
				bool flag4 = origin != null && CS$<>8__locals1.itemData.ContainsItemKey(origin.Key);
				if (flag4)
				{
					interactable = false;
				}
				else
				{
					bool flag5 = slotChanged && this._isAddPoisonCondense && this._tempPoisonEffects.PoisonSlotList.CheckIndex(i) && this._tempPoisonEffects.PoisonSlotList[i].IsCondensed;
					if (flag5)
					{
						interactable = true;
					}
				}
			}
			itemView.SetInteractable(interactable);
			itemView.SetMask(false);
			bool curTabIsAboutPoison2 = this.CurTabIsAboutPoison;
			if (curTabIsAboutPoison2)
			{
				bool flag6 = this._curTab == UI_Make.UIMakeTab.Poison && (cur == null || !cur.Key.HasTemplate);
				if (flag6)
				{
					tip.enabled = true;
					tip.Type = TipType.SingleDesc;
					string content = LocalStringManager.Get(LanguageKey.LK_Add_Poison_EmptySlot);
					tip.PresetParam = new string[]
					{
						content
					};
					tip.RuntimeParam = null;
				}
				else
				{
					bool flag7 = origin != null && origin.Key.HasTemplate && cur != null && cur.Key.TemplateEquals(origin.Key);
					if (flag7)
					{
						this.ItemShowPoisonIcon(itemView, CS$<>8__locals1.index);
					}
				}
				bool flag8 = this._isAddPoisonCondense && this._currentTarget != null;
				if (flag8)
				{
					this.ItemShowPoisonIcon(itemView, CS$<>8__locals1.index);
					int initCount = (this._currentTarget.HasAnyPoison && this._currentTarget.PoisonEffects.PoisonSlotList.CheckIndex(i)) ? this._currentTarget.PoisonEffects.PoisonSlotList[i].CurrentMedicineCount : 0;
					int curCount = this._tempPoisonEffects.PoisonSlotList.CheckIndex(i) ? this._tempPoisonEffects.PoisonSlotList[i].CurrentMedicineCount : 0;
					itemView.SetPoisonCondenseCount(initCount, curCount, PoisonSlot.MaxMedicineCount);
				}
			}
			itemView.SetClickEvent(new UnityAction(CS$<>8__locals1.<RefreshSlots>g__OnClick|0));
			bool enabled = tip.enabled;
			if (enabled)
			{
				tip.Refresh(true, -1);
			}
			else
			{
				tip.HideTips();
			}
		}
		this.CheckCondition(false);
		this._itemViewSlot.ReRender();
	}

	// Token: 0x06001AC4 RID: 6852 RVA: 0x000B43C0 File Offset: 0x000B25C0
	private void AddCondense(int index, ItemDisplayData itemData)
	{
		PoisonSlot poisonSlot = this._tempPoisonEffects.PoisonSlotList[index];
		if (poisonSlot.CondensedMedicineTemplateIdList == null)
		{
			poisonSlot.CondensedMedicineTemplateIdList = new List<short>();
		}
		this._tempPoisonEffects.PoisonSlotList[index].CondensedMedicineTemplateIdList.Add(itemData.Key.TemplateId);
		int findIndex = this._condensePoisonItemList.FindIndex((ItemDisplayData d) => d.Key.TemplateEquals(itemData.Key) && d.ItemSourceType == itemData.ItemSourceType);
		bool flag = findIndex >= 0;
		if (flag)
		{
			ItemDisplayData targetItem = this._condensePoisonItemList[findIndex];
			targetItem.Amount++;
		}
		else
		{
			ItemDisplayData targetItem = itemData.Clone(1);
			this._condensePoisonItemList.Add(targetItem);
		}
		List<ItemDisplayData> itemList = this._slotItemListDict[itemData.ItemSourceTypeEnum];
		ItemDisplayData data = itemList.Find((ItemDisplayData d) => d.Key.TemplateId == itemData.Key.TemplateId);
		data.Amount--;
		bool flag2 = data.Amount <= 0;
		if (flag2)
		{
			itemList.Remove(data);
			List<ItemDisplayData> itemsSource = this.GetItemsSource(itemData.ItemSourceTypeEnum);
			itemsSource.Remove(data);
		}
		this._itemViewSlot.SetItemList(ref itemList, false, null, false, null);
		this.RefreshSlots();
	}

	// Token: 0x06001AC5 RID: 6853 RVA: 0x000B4520 File Offset: 0x000B2720
	private bool RemoveCondense(int index)
	{
		bool flag = !this._isAddPoisonCondense || !this._tempPoisonEffects.PoisonSlotList.CheckIndex(index) || !this._tempPoisonEffects.PoisonSlotList[index].IsCondensed;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			PoisonSlot tempSlot = this._tempPoisonEffects.PoisonSlotList[index];
			PoisonSlot originSlot = this._originPoisonEffects.PoisonSlotList.CheckIndex(index) ? this._originPoisonEffects.PoisonSlotList[index] : null;
			int templateIdIndex = tempSlot.CondensedMedicineTemplateIdList.Count - 1;
			short templateId = tempSlot.CondensedMedicineTemplateIdList[templateIdIndex];
			bool? flag2;
			if (originSlot == null)
			{
				flag2 = null;
			}
			else
			{
				List<short> condensedMedicineTemplateIdList = originSlot.CondensedMedicineTemplateIdList;
				flag2 = ((condensedMedicineTemplateIdList != null) ? new bool?(condensedMedicineTemplateIdList.CheckIndex(templateIdIndex)) : null);
			}
			bool? flag3 = flag2;
			int originTemplateId = (int)(flag3.GetValueOrDefault() ? originSlot.CondensedMedicineTemplateIdList[templateIdIndex] : -1);
			bool flag4 = (int)templateId == originTemplateId;
			if (flag4)
			{
				result = false;
			}
			else
			{
				ItemKey tempKey = new ItemKey(8, 0, templateId, 0);
				tempSlot.CondensedMedicineTemplateIdList.RemoveAt(tempSlot.CondensedMedicineTemplateIdList.Count - 1);
				ItemDisplayData targetItem = this._condensePoisonItemList.Find((ItemDisplayData d) => d.Key.TemplateEquals(tempKey));
				targetItem.Amount--;
				bool flag5 = targetItem.Amount <= 0;
				if (flag5)
				{
					this._condensePoisonItemList.Remove(targetItem);
				}
				List<ItemDisplayData> slotCacheItemList = this._slotItemListDict[targetItem.ItemSourceTypeEnum];
				ItemDisplayData cacheData = slotCacheItemList.Find((ItemDisplayData d) => d.Key.TemplateEquals(targetItem.Key));
				bool flag6 = cacheData == null;
				if (flag6)
				{
					ItemDisplayData newItem = targetItem.Clone(1);
					slotCacheItemList.Add(newItem);
					List<ItemDisplayData> itemsSource = this.GetItemsSource(newItem.ItemSourceTypeEnum);
					itemsSource.Add(newItem);
				}
				else
				{
					cacheData.Amount++;
				}
				bool flag7 = this._curTab == UI_Make.UIMakeTab.Weave;
				if (flag7)
				{
					this.RefreshWeaveSlotView();
				}
				else
				{
					List<ItemDisplayData> itemList = this.SlotCacheItemList;
					this._itemViewSlot.SetItemList(ref itemList, false, null, false, null);
				}
				this.RefreshSlots();
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06001AC6 RID: 6854 RVA: 0x000B4778 File Offset: 0x000B2978
	private void ItemShowPoisonIcon(ItemView itemView, int index)
	{
		ItemDisplayData data = itemView.Data;
		bool flag = data == null || !data.Key.HasTemplate;
		if (!flag)
		{
			MedicineItem medicineConfig = Medicine.Instance[itemView.Data.Key.TemplateId];
			PoisonItem poisonConfig = Poison.Instance[medicineConfig.PoisonType];
			itemView.SetIcon(poisonConfig.Icon);
			bool flag2 = this._tempPoisonEffects.PoisonSlotList.CheckIndex(index);
			if (flag2)
			{
				TooltipInvoker tip = itemView.GetMouseTip();
				tip.enabled = true;
				tip.Type = TipType.AttachedPoison;
				tip.PresetParam = null;
				tip.RuntimeParam = EasyPool.Get<ArgumentBox>().SetObject("PoisonSlot", this._tempPoisonEffects.PoisonSlotList[index]);
			}
		}
	}

	// Token: 0x06001AC7 RID: 6855 RVA: 0x000B4848 File Offset: 0x000B2A48
	private void ClearSlot()
	{
		this._condensePoisonItemList.Clear();
		for (int i = 0; i < this.SlotCount; i++)
		{
			this._originSlotItemArray[i] = (this._slotItemArray[i] = this._invalidItemDisplayData);
		}
		this.RefreshSlots();
	}

	// Token: 0x06001AC8 RID: 6856 RVA: 0x000B489C File Offset: 0x000B2A9C
	private void ClearSlotItemChange()
	{
		foreach (ItemSourceChange itemSourceChange in this._slotItemChangeDict.Values)
		{
			itemSourceChange.Items.Clear();
		}
	}

	// Token: 0x06001AC9 RID: 6857 RVA: 0x000B4900 File Offset: 0x000B2B00
	private void PlayConditionAnim(bool isTarget, bool isIn)
	{
		string name = isTarget ? "TargetConditionMeet" : "ToolConditionMeet";
		base.CGet<CImage>(name).DOFillAmount((float)(isIn ? 1 : 0), 0.2f);
	}

	// Token: 0x06001ACA RID: 6858 RVA: 0x000B4938 File Offset: 0x000B2B38
	private void PlayCenterAnim(bool play, bool loop = false, bool isMakePerfect = false)
	{
		UI_Make.<>c__DisplayClass240_0 CS$<>8__locals1 = new UI_Make.<>c__DisplayClass240_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.play = play;
		CS$<>8__locals1.loop = loop;
		CS$<>8__locals1.craftAnim = base.CGet<SkeletonGraphic>("CraftAnim");
		bool flag = this._makeItemSubTypeId != -1 && this._curTab == UI_Make.UIMakeTab.Make && !this.NeedLockOnMaking && !isMakePerfect;
		if (flag)
		{
			base.CGet<GameObject>("Effects").gameObject.SetActive(false);
			CS$<>8__locals1.craftAnim.gameObject.SetActive(false);
		}
		else
		{
			bool flag2 = this._curTab == UI_Make.UIMakeTab.Weave && this.ResultPreviewImage && this.ResultPreviewImage.gameObject.activeSelf;
			if (flag2)
			{
				base.CGet<GameObject>("Effects").gameObject.SetActive(false);
				CS$<>8__locals1.craftAnim.gameObject.SetActive(false);
			}
			else
			{
				bool ready = this.Element.Ready;
				if (ready)
				{
					CS$<>8__locals1.<PlayCenterAnim>g__Play|0();
				}
				else
				{
					base.DelayCall(new Action(CS$<>8__locals1.<PlayCenterAnim>g__Play|0), this.AnimIn.duration);
				}
			}
		}
	}

	// Token: 0x06001ACB RID: 6859 RVA: 0x000B4A5A File Offset: 0x000B2C5A
	private void AnimationStateOnComplete(TrackEntry trackentry)
	{
		Action onClickConfirm = this._onClickConfirm;
		if (onClickConfirm != null)
		{
			onClickConfirm();
		}
		this.PlayCenterAnim(false, false, false);
	}

	// Token: 0x06001ACC RID: 6860 RVA: 0x000B4A7C File Offset: 0x000B2C7C
	private void PlayEffect()
	{
		bool flag = !this._hasMakeFunction;
		if (!flag)
		{
			string effectName = this.GetEffectName();
			bool flag2 = this._curEffectName == effectName;
			if (!flag2)
			{
				RectTransform animList = base.CGet<RectTransform>("AnimList");
				bool hasEffect = false;
				for (int i = 0; i < animList.transform.childCount; i++)
				{
					GameObject obj = animList.transform.GetChild(i).gameObject;
					bool isShow = effectName == obj.name;
					obj.SetActive(isShow);
					bool flag3 = obj.name == effectName;
					if (flag3)
					{
						hasEffect = true;
					}
				}
				bool flag4 = !hasEffect;
				if (flag4)
				{
					GameObject template = ResLoader.SyncLoad<GameObject>("RemakeResources/Prefab/Views/Legacy/Building/Make/" + effectName);
					GameObject obj2 = Object.Instantiate<GameObject>(template, animList);
					obj2.name = template.name;
					obj2.transform.localPosition = Vector3.zero;
					obj2.gameObject.SetActive(true);
				}
				this._curEffectName = effectName;
			}
		}
	}

	// Token: 0x06001ACD RID: 6861 RVA: 0x000B4B8C File Offset: 0x000B2D8C
	private string GetEffectName()
	{
		string name = string.Empty;
		bool flag = this._curTab == UI_Make.UIMakeTab.Repair;
		if (flag)
		{
			bool isInDoor = this.IsInDoor;
			if (isInDoor)
			{
				name = "IndoorRepairEffect";
			}
			else
			{
				name = "OutdoorRepairEffect";
			}
		}
		else
		{
			name = this._makeBuildingEffectDict[this._curLifeSkillType];
		}
		return name;
	}

	// Token: 0x06001ACE RID: 6862 RVA: 0x000B4BE0 File Offset: 0x000B2DE0
	private void InitRefine()
	{
		this._refineRequireResourceList = base.CGet<Refers>("Refine").CGet<Refers>("RequireResourceList");
	}

	// Token: 0x06001ACF RID: 6863 RVA: 0x000B4C00 File Offset: 0x000B2E00
	private void RefreshRefineSlotView(bool isInit)
	{
		if (isInit)
		{
			foreach (KeyValuePair<ItemSourceType, List<ItemDisplayData>> keyValuePair in this._slotItemListDict)
			{
				ItemSourceType itemSourceType;
				List<ItemDisplayData> list2;
				keyValuePair.Deconstruct(out itemSourceType, out list2);
				ItemSourceType type = itemSourceType;
				List<ItemDisplayData> list = list2;
				list.Clear();
				List<ItemDisplayData> sourceItems = this.GetItemsSource(type);
				list.AddRange(from d in sourceItems
				where d.Key.ItemType == 5 && Config.Material.Instance[d.Key.TemplateId].RefiningEffect >= 0
				select d);
			}
		}
		List<ItemDisplayData> itemList = this.SlotCacheItemList;
		this._itemViewSlot.SetItemList(ref itemList, false, null, false, null);
		this._itemViewSlot.SortAndFilter.ShowFilterType(ItemSortAndFilter.ItemFilterType.Material);
		this._itemViewSlot.SortAndFilter.LockFilterType(ItemSortAndFilter.ItemFilterType.Material, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
	}

	// Token: 0x06001AD0 RID: 6864 RVA: 0x000B4CEC File Offset: 0x000B2EEC
	private void CheckRefineCondition()
	{
		this._buttonConfirm.interactable = false;
		bool flag = this._currentTarget == null;
		if (flag)
		{
			this.SetConfirmButtonTip(10547, true, TipType.SingleDesc, null);
		}
		else
		{
			this.RefreshToolAttainment(this._currentTool != null);
			ResourceInts needResources = default(ResourceInts);
			LifeSkillShorts needLifeSkill = default(LifeSkillShorts);
			LifeSkillShorts showLifeSkill = default(LifeSkillShorts);
			bool haveNotChange = true;
			bool resourceMeet = true;
			short durabilityCost = 0;
			for (int i = 0; i < this.SlotCount; i++)
			{
				short oldId = this._originSlotItemArray[i].Key.TemplateId;
				short curId = this._slotItemArray[i].Key.TemplateId;
				bool isSame = oldId == curId;
				bool flag2 = !isSame;
				if (flag2)
				{
					haveNotChange = false;
				}
				bool flag3 = curId > -1 || oldId > -1;
				if (flag3)
				{
					short id = curId;
					bool flag4 = curId == -1;
					if (flag4)
					{
						id = oldId;
					}
					MaterialItem materialConfig = Config.Material.Instance[id];
					bool flag5 = oldId != curId && curId > 0;
					if (flag5)
					{
						needResources.Change((int)materialConfig.ResourceType, (int)materialConfig.RequiredResourceAmount);
					}
					short lifeSkillAttainment = needLifeSkill.Get((int)materialConfig.RequiredLifeSkillType);
					short attainmentRequirement = this.GetAttainmentByBuildingEffect(materialConfig.RequiredLifeSkillType, materialConfig.RequiredAttainment);
					needLifeSkill.Set((int)materialConfig.RequiredLifeSkillType, Math.Max(lifeSkillAttainment, attainmentRequirement));
					showLifeSkill.Set((int)materialConfig.RequiredLifeSkillType, 1);
					bool flag6 = !isSame;
					if (flag6)
					{
						short cost = this.GetToolDurabilityCost(this._currentTool, materialConfig.Grade);
						bool flag7 = oldId > -1;
						if (flag7)
						{
							MaterialItem oldMaterialConfig = Config.Material.Instance[oldId];
							short oldCost = this.GetToolDurabilityCost(this._currentTool, oldMaterialConfig.Grade);
							cost = Math.Max(cost, oldCost);
						}
						durabilityCost = Math.Max(durabilityCost, cost);
					}
				}
			}
			bool lifeSkillMeet = this.CheckAndShowLifeSkill(needLifeSkill, showLifeSkill);
			bool flag8 = haveNotChange;
			if (flag8)
			{
				this.SetConfirmButtonTip(10549, true, TipType.SingleDesc, null);
				this._refineRequireResourceList.gameObject.SetActive(false);
				this.HideLifeSkill();
			}
			else
			{
				for (sbyte j = 0; j < 8; j += 1)
				{
					bool curResourceMeet = true;
					int resource = this._buildingModel.GetResourceCount(j);
					bool flag9 = resource < needResources.Get((int)j);
					if (flag9)
					{
						resourceMeet = false;
						curResourceMeet = false;
					}
					Refers refers = null;
					switch (j)
					{
					case 1:
						refers = this._refineRequireResourceList.CGet<Refers>("Wood");
						break;
					case 2:
						refers = this._refineRequireResourceList.CGet<Refers>("Metal");
						break;
					case 3:
						refers = this._refineRequireResourceList.CGet<Refers>("Jade");
						break;
					case 4:
						refers = this._refineRequireResourceList.CGet<Refers>("Fabric");
						break;
					}
					bool flag10 = refers;
					if (flag10)
					{
						LanguageKey key = curResourceMeet ? LanguageKey.LK_Refine_Resource_Require_Meet : LanguageKey.LK_Refine_Resource_Require_Not_Meet;
						string color = curResourceMeet ? "brightblue" : "brightred";
						refers.CGet<TextMeshProUGUI>("Label").text = LocalStringManager.GetFormat(key, Config.ResourceType.Instance[j].Name, resource.ToString().SetColor(color), needResources.Get((int)j));
						bool need = needResources.Get((int)j) > 0;
						refers.gameObject.SetActive(need);
						bool flag11 = need;
						if (flag11)
						{
							this._refineRequireResourceList.gameObject.SetActive(true);
						}
					}
				}
				bool toolMeet = this.CheckTool(durabilityCost, durabilityCost);
				bool flag12 = !toolMeet;
				if (!flag12)
				{
					bool flag13 = !lifeSkillMeet;
					if (flag13)
					{
						this.SetConfirmButtonTip(7667, true, TipType.SingleDesc, null);
					}
					else
					{
						bool flag14 = !resourceMeet;
						if (flag14)
						{
							this.SetConfirmButtonTip(7681, true, TipType.SingleDesc, null);
						}
						else
						{
							this._buttonConfirm.interactable = true;
							this.SetConfirmButtonTip(-1, !this._buttonConfirm.interactable, TipType.SingleDesc, null);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001AD1 RID: 6865 RVA: 0x000B5108 File Offset: 0x000B3308
	private void ConfirmRefine()
	{
		this._buttonConfirm.interactable = false;
		ItemKey[] toolKeys = new ItemKey[]
		{
			this._currentTool.Key
		};
		ItemDisplayData[] tools = new ItemDisplayData[]
		{
			this._currentTool
		};
		AsyncMethodCallbackDelegate <>9__1;
		BuildingDomainMethod.AsyncCall.CheckRefineCondition(this, this._taiwuCharId, toolKeys, this._currentTarget.Key, this._slotItemArray, this._buildingBlockKey, delegate(int offset, RawDataPool dataPool)
		{
			bool isMeet = false;
			Serializer.Deserialize(dataPool, offset, ref isMeet);
			bool flag = !isMeet;
			if (flag)
			{
				UIElement.FullScreenMask.Hide(false);
			}
			else
			{
				IAsyncMethodRequestHandler <>4__this = this;
				int taiwuCharId = this._taiwuCharId;
				ItemDisplayData[] tools = tools;
				ItemDisplayData currentTarget = this._currentTarget;
				ItemDisplayData[] slotItemArray = this._slotItemArray;
				List<ItemSourceChange> changeList = this._slotItemChangeDict.Values.ToList<ItemSourceChange>();
				AsyncMethodCallbackDelegate callback;
				if ((callback = <>9__1) == null)
				{
					callback = (<>9__1 = delegate(int offset2, RawDataPool dataPool2)
					{
						Serializer.Deserialize(dataPool2, offset2, ref this._resultItemDisplayData);
						ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
						argBox.SetObject("ItemList", new List<ItemDisplayData>
						{
							this._resultItemDisplayData
						});
						argBox.Set("ObtainType", 9);
						UIElement.GetItem.SetOnInitArgs(argBox);
						UIManager.Instance.MaskUI(UIElement.GetItem);
						for (int i = 0; i < this.SlotCount; i++)
						{
							this._originSlotItemArray[i] = this._slotItemArray[i];
						}
						this.RefreshAllItems();
					});
				}
				BuildingDomainMethod.AsyncCall.RefineItem(<>4__this, taiwuCharId, tools, currentTarget, slotItemArray, changeList, callback);
			}
		});
	}

	// Token: 0x06001AD2 RID: 6866 RVA: 0x000B5190 File Offset: 0x000B3390
	private void InitMake()
	{
		this._isWaitingMakeResultRequest = false;
		this._needAutoFillResourceOnMakeEnd = false;
		this._makePageSwitchController = base.CGet<Refers>("Make").CGet<HorizontalPageSwitchController>("PageSwitchController");
		this._makeSubTogGroup = base.CGet<Refers>("Make").CGet<SubTogGroup>("SubTitleGroup");
		this._makePageSwitchController.PageItemRefreshHandler = new Action<int, Refers>(this.RefreshMakePageItem);
		this._makePageSwitchController.SetItemSelectStateHandler = new Action<Refers, bool>(this.SetMakePageItemSelectState);
		this._makePageSwitchController.InitPageCount(0, 0, false);
		this._makePageSwitchController.RegisterOnSelectIndexChangeHandler(new Action<int>(this.OnSelectMakePageIndexChange));
		this._makeCountButtonLess = base.CGet<Refers>("Make").CGet<CButtonObsolete>("ButtonLess");
		this._makeCountButtonLess.ClearAndAddListener(delegate
		{
			this.ChangeMakeCount(false);
		});
		this._makeCountButtonMore = base.CGet<Refers>("Make").CGet<CButtonObsolete>("ButtonMore");
		this._makeCountButtonMore.ClearAndAddListener(delegate
		{
			this.ChangeMakeCount(true);
		});
		this._makeCountButtonMin = base.CGet<Refers>("Make").CGet<CButtonObsolete>("ButtonMin");
		this._makeCountButtonMin.ClearAndAddListener(delegate
		{
			this.ChangeMakeCountRange(false);
		});
		this._makeCountButtonMax = base.CGet<Refers>("Make").CGet<CButtonObsolete>("ButtonMax");
		this._makeCountButtonMax.ClearAndAddListener(delegate
		{
			this.ChangeMakeCountRange(true);
		});
		this._makePageSwitchController.gameObject.SetActive(false);
		this._makeSubTogGroup.gameObject.SetActive(false);
		this._makePanel.gameObject.SetActive(false);
		this._makeCountButtonLess.interactable = false;
		this._makeCountButtonMore.interactable = false;
		this._makeCountButtonMin.interactable = false;
		this._makeCountButtonMax.interactable = false;
		this._confirmButtonTipDisplayer = this._buttonConfirm.GetComponent<TooltipInvoker>();
		this._confirmButtonTipDisplayer.PresetParam = new string[1];
		this.InitButtonPerfect();
		this._makeDropdown = base.CGet<CDropdownLegacy>("MakeDropdown");
		this._makeDropdown.onValueChanged.RemoveAllListeners();
		this._makeDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnMakeDropdownValueChanged));
		this._makeDropdown.SetOnShow(new Action<GameObject>(this.OnMakeDropdownShow));
		this._makeDropdownMask = base.CGet<GameObject>("MakeDropdownMask");
		this._buildingStoreToDropdown = base.CGet<BuildingStoreToDropdown>("StoreToDropDown");
		this._buildingStoreToDropdown.Init(this, (int)(-16 + this._blockData.ConfigData.RequireLifeSkillType), new Action<int>(this.ChangeStock));
		this.ShowMakeDropdown(false, 0);
	}

	// Token: 0x06001AD3 RID: 6867 RVA: 0x000B5444 File Offset: 0x000B3644
	private void ChangeStock(int param)
	{
		this.CheckMakeCondition(false, null);
		BuildingDomainMethod.Call.SetStoreLocation(-1, param);
	}

	// Token: 0x06001AD4 RID: 6868 RVA: 0x000B5458 File Offset: 0x000B3658
	private void RefreshMakeCount()
	{
		ItemDisplayData currentTarget = this._currentTarget;
		int targetAmount = (currentTarget != null) ? currentTarget.Amount : 1;
		bool flag = this._currentTool != null && !this.IsEmptyTool(this._currentTool) && this._makeToolDurabilityCost > 0;
		if (flag)
		{
			int count = ((int)this._currentTool.Durability % this._makeToolDurabilityCost == 0) ? ((int)this._currentTool.Durability / this._makeToolDurabilityCost) : ((int)this._currentTool.Durability / this._makeToolDurabilityCost + 1);
			this._maxMakeCount = Mathf.Max(1, count);
			this._maxMakeCount = Mathf.Min(targetAmount, this._maxMakeCount);
		}
		else
		{
			this._maxMakeCount = Mathf.Min(targetAmount, int.MaxValue);
		}
		this._makeCount = (short)Mathf.Clamp((int)this._makeCount, 1, this._maxMakeCount);
		this._confirmNormalLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Making_Times, this._makeCount);
		this._confirmDisableLabel.text = this._confirmNormalLabel.text;
	}

	// Token: 0x06001AD5 RID: 6869 RVA: 0x000B5564 File Offset: 0x000B3764
	private void InitButtonPerfect()
	{
		CButtonObsolete buttonPerfect = base.CGet<CButtonObsolete>("ButtonPerfect");
		TweenCallback <>9__1;
		buttonPerfect.ClearAndAddListener(delegate
		{
			this._makePerfect = !this._makePerfect;
			buttonPerfect.transform.Find("Select").gameObject.SetActive(this._makePerfect);
			this.CheckMakeCondition(false, null);
			GameObject perfectEffectSelect = this.CGet<GameObject>("PerfectEffectSelect");
			ParticleSystem particle = perfectEffectSelect.GetComponentInChildren<ParticleSystem>();
			bool makePerfect = this._makePerfect;
			if (makePerfect)
			{
				perfectEffectSelect.SetActive(true);
				this.StopInverseParticleCoroutine();
				particle.Play();
				this._perfectEffectSelectSequence = DOTween.Sequence();
				this._perfectEffectSelectSequence.AppendInterval(1f);
				DG.Tweening.Sequence perfectEffectSelectSequence = this._perfectEffectSelectSequence;
				TweenCallback callback;
				if ((callback = <>9__1) == null)
				{
					callback = (<>9__1 = delegate()
					{
						this.ShowPerfectEffectLoop(true, true);
					});
				}
				perfectEffectSelectSequence.AppendCallback(callback);
				this._perfectEffectSelectSequence.Play<DG.Tweening.Sequence>();
			}
			else
			{
				DG.Tweening.Sequence perfectEffectSelectSequence2 = this._perfectEffectSelectSequence;
				if (perfectEffectSelectSequence2 != null)
				{
					perfectEffectSelectSequence2.Pause<DG.Tweening.Sequence>();
				}
				this.StartInverseParticleCoroutine(particle);
				DG.Tweening.Sequence perfectEffectSelectSequence3 = this._perfectEffectSelectSequence;
				bool hasShow = perfectEffectSelectSequence3 != null && perfectEffectSelectSequence3.IsComplete();
				this.ShowPerfectEffectLoop(false, hasShow);
			}
		});
		this.RefreshButtonPerfect(true);
	}

	// Token: 0x06001AD6 RID: 6870 RVA: 0x000B55B0 File Offset: 0x000B37B0
	private void RefreshButtonPerfect(bool reset = true)
	{
		bool show = this._curTab == UI_Make.UIMakeTab.Make;
		bool flag = this._makeItemSubTypeId > -1;
		if (flag)
		{
			MakeItemSubTypeItem makeItemSubTypeConfig = MakeItemSubType.Instance[this._makeItemSubTypeId];
			bool flag2 = !ItemType.IsEquipmentEffectType(makeItemSubTypeConfig.Result.ItemType);
			if (flag2)
			{
				show = false;
			}
		}
		else
		{
			show = false;
		}
		CButtonObsolete buttonPerfect = base.CGet<CButtonObsolete>("ButtonPerfect");
		buttonPerfect.gameObject.SetActive(show);
		if (reset)
		{
			this._makePerfect = false;
		}
		buttonPerfect.transform.Find("Select").gameObject.SetActive(this._makePerfect);
		base.CGet<GameObject>("PerfectEffectSelect").SetActive(this._makePerfect);
		base.CGet<GameObject>("PerfectEffectConfirm").SetActive(this._makePerfect);
		bool flag3 = !this._makePerfect;
		if (flag3)
		{
			this.ShowPerfectEffectLoop(false, false);
			DG.Tweening.Sequence perfectEffectSelectSequence = this._perfectEffectSelectSequence;
			if (perfectEffectSelectSequence != null)
			{
				perfectEffectSelectSequence.Pause<DG.Tweening.Sequence>();
			}
			this.StopInverseParticleCoroutine();
		}
	}

	// Token: 0x06001AD7 RID: 6871 RVA: 0x000B56B0 File Offset: 0x000B38B0
	private void InitResourceSprite()
	{
		this.HideResourceSprite();
		bool flag = this._curTab != UI_Make.UIMakeTab.Make;
		if (!flag)
		{
			Refers template = base.CGet<Refers>("Make").CGet<Refers>("Template");
			for (int i = 0; i < template.transform.childCount; i++)
			{
				Transform child = template.transform.GetChild(i);
				child.GetComponent<SpriteRenderer>().sortingOrder = 603;
				child.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06001AD8 RID: 6872 RVA: 0x000B5738 File Offset: 0x000B3938
	private void HideResourceSprite()
	{
		GameObject pool = base.CGet<Refers>("Make").CGet<GameObject>("Pool");
		for (int i = 0; i < pool.transform.childCount; i++)
		{
			Transform child = pool.transform.GetChild(i);
			bool activeSelf = child.gameObject.activeSelf;
			if (activeSelf)
			{
				child.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06001AD9 RID: 6873 RVA: 0x000B57A4 File Offset: 0x000B39A4
	private void DestroyResourceSprite()
	{
		GameObject pool = base.CGet<Refers>("Make").CGet<GameObject>("Pool");
		for (int i = 0; i < pool.transform.childCount; i++)
		{
			Transform child = pool.transform.GetChild(i);
			Object.Destroy(child.gameObject);
		}
		this._makeResourceSpriteDict.Clear();
	}

	// Token: 0x06001ADA RID: 6874 RVA: 0x000B580C File Offset: 0x000B3A0C
	private void ShowResourceSprite(string resourceName, sbyte type, int curCount, bool isMore)
	{
		GameObject pool = base.CGet<Refers>("Make").CGet<GameObject>("Pool");
		Refers template = base.CGet<Refers>("Make").CGet<Refers>("Template");
		GameObject birthPointStart = base.CGet<Refers>("Make").CGet<GameObject>("BirthPointStart");
		GameObject birthPointEnd = base.CGet<Refers>("Make").CGet<GameObject>("BirthPointEnd");
		float randomX = Mathf.Lerp(birthPointStart.transform.position.x, birthPointEnd.transform.position.x, Random.value);
		Vector3 targetPos = birthPointStart.transform.position.SetX(randomX);
		List<GameObject> list;
		bool flag = !this._makeResourceSpriteDict.TryGetValue(resourceName, out list);
		if (flag)
		{
			list = new List<GameObject>();
			this._makeResourceSpriteDict[resourceName] = list;
		}
		if (isMore)
		{
			GameObject templateGo = template.CGet<GameObject>(resourceName);
			bool flag2 = curCount > list.Count;
			if (flag2)
			{
				GameObject go = Object.Instantiate<GameObject>(templateGo, targetPos, Quaternion.identity, pool.transform);
				string spriteName = CommonUtils.GetResourceSpriteName(type, true);
				AtlasInfo.Instance.GetSprite(spriteName, delegate(Sprite sprite)
				{
					go.GetComponent<SpriteRenderer>().sprite = sprite;
					go.SetActive(true);
				});
				list.Add(go);
			}
			else
			{
				GameObject go3 = list[curCount - 1];
				go3.transform.SetPositionAndRotation(targetPos, Quaternion.identity);
				go3.SetActive(true);
			}
		}
		else
		{
			GameObject go2 = list[curCount];
			go2.SetActive(false);
		}
	}

	// Token: 0x06001ADB RID: 6875 RVA: 0x000B59A4 File Offset: 0x000B3BA4
	private void ChangeMakeResourceCount(CButtonObsolete button, string resourceName, bool isMore, bool isToEdge = false, bool useLast = false)
	{
		UI_Make.<>c__DisplayClass258_0 CS$<>8__locals1 = new UI_Make.<>c__DisplayClass258_0();
		CS$<>8__locals1.isMore = isMore;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.resourceName = resourceName;
		CS$<>8__locals1.button = button;
		CS$<>8__locals1.type = GameData.Domains.Character.ResourceType.GetType(CS$<>8__locals1.resourceName);
		CS$<>8__locals1.curCount = this._curMakeResourceCountInts.Get((int)CS$<>8__locals1.type);
		CS$<>8__locals1.maxCount = (useLast ? ((int)this._lastMakeResourceCount.Get((int)CS$<>8__locals1.type)) : this._maxMakeResourceCountInts.Get((int)CS$<>8__locals1.type));
		CS$<>8__locals1.maxCount = Mathf.Min(CS$<>8__locals1.maxCount, this.GetMaxMeetResourceCount((int)this._makeCount, CS$<>8__locals1.type));
		int diff = Math.Min((int)(this._maxMakeResourceTotalCount - this.CurMakeResourceTotalCount), CS$<>8__locals1.maxCount - CS$<>8__locals1.curCount);
		CS$<>8__locals1.offset = (isToEdge ? (CS$<>8__locals1.isMore ? diff : CS$<>8__locals1.curCount) : 1);
		bool flag = this._corChangeMakeResourceCount[(int)CS$<>8__locals1.type] != null;
		if (flag)
		{
			SingletonObject.getInstance<YieldHelper>().StopYield(this._corChangeMakeResourceCount[(int)CS$<>8__locals1.type]);
			this._corChangeMakeResourceCount[(int)CS$<>8__locals1.type] = null;
		}
		bool flag2 = CS$<>8__locals1.offset > 1;
		if (flag2)
		{
			this._corChangeMakeResourceCount[(int)CS$<>8__locals1.type] = SingletonObject.getInstance<YieldHelper>().StartYield(CS$<>8__locals1.<ChangeMakeResourceCount>g__ChangeOnce|1());
		}
		else
		{
			CS$<>8__locals1.<ChangeMakeResourceCount>g__Tick|0();
		}
	}

	// Token: 0x06001ADC RID: 6876 RVA: 0x000B5B04 File Offset: 0x000B3D04
	private void ChangeMakeCount(bool isMore)
	{
		if (isMore)
		{
			this._makeCount += 1;
		}
		else
		{
			this._makeCount -= 1;
		}
		this.CheckMakeCondition(false, null);
	}

	// Token: 0x06001ADD RID: 6877 RVA: 0x000B5B44 File Offset: 0x000B3D44
	private void ChangeMakeCountRange(bool isMax)
	{
		if (isMax)
		{
			while (this._makeCountButtonMax.interactable)
			{
				this.ChangeMakeCount(true);
			}
		}
		else
		{
			while (this._makeCountButtonMin.interactable)
			{
				this.ChangeMakeCount(false);
			}
		}
	}

	// Token: 0x06001ADE RID: 6878 RVA: 0x000B5B94 File Offset: 0x000B3D94
	private void RefreshMakePageItem(int index, Refers refers)
	{
		bool flag = !this._makeTypeList.CheckIndex(index);
		if (!flag)
		{
			TextMeshProUGUI textComponent = refers.CGet<TextMeshProUGUI>("Label");
			textComponent.text = MakeItemType.Instance[this._makeTypeList[index]].TypeName;
			refers.CGet<CToggleObsolete>("Toggle").onValueChanged.RemoveAllListeners();
			refers.CGet<CToggleObsolete>("Toggle").onValueChanged.AddListener(delegate(bool isOn)
			{
				if (isOn)
				{
					this._makePageSwitchController.SetSelect(index, true);
				}
			});
		}
	}

	// Token: 0x06001ADF RID: 6879 RVA: 0x000B5C3B File Offset: 0x000B3E3B
	private void SetMakePageItemSelectState(Refers refers, bool selectState)
	{
		refers.CGet<CToggleObsolete>("Toggle").isOn = selectState;
		refers.CGet<CToggleObsolete>("Toggle").interactable = !selectState;
		MonoJoint componentInChildren = refers.GetComponentInChildren<MonoJoint>(true);
		if (componentInChildren != null)
		{
			componentInChildren.JointSync();
		}
	}

	// Token: 0x06001AE0 RID: 6880 RVA: 0x000B5C78 File Offset: 0x000B3E78
	private void OnSelectMakePageIndexChange(int index)
	{
		bool flag = !this._makeTypeList.CheckIndex(index);
		if (!flag)
		{
			short newMakeItemTypeId = this._makeTypeList[index];
			bool hasChanged = this._makeItemTypeId != newMakeItemTypeId;
			this._makeItemTypeId = newMakeItemTypeId;
			this._makeItemSubtypeIdList = this._makeTypeDict[this._makeItemTypeId];
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			string tip = LocalStringManager.Get(LanguageKey.LK_Make_SubType_Manual_Tip).SetColor("darkred");
			List<SubTogGroup.TogContent> togContents = this._makeItemSubtypeIdList.Select(delegate(short id)
			{
				SubTogGroup.TogContent togContent = default(SubTogGroup.TogContent);
				togContent.Content = MakeItemSubType.Instance[id].Name;
				togContent.TipTitle = LocalStringManager.Get(LanguageKey.LK_Make_SubType_Manual);
				sb.Clear();
				sb.Append(MakeItemSubType.Instance[id].Desc);
				sb.Append("\n");
				sb.Append(tip);
				togContent.TipContent = sb.ToString();
				return togContent;
			}).ToList<SubTogGroup.TogContent>();
			EasyPool.Free<StringBuilder>(sb);
			int lastMakeDropdownValue = this._makeDropdown.value;
			this._makeSubTogGroup.gameObject.SetActive(this._makeItemSubtypeIdList.Count > 1);
			bool flag2 = this._makeItemSubtypeIdList.Count > 1;
			if (flag2)
			{
				this._makeSubTogGroup.UpdateGroup(togContents, delegate(bool isOn, int toggleIndex)
				{
					bool flag9 = !isOn;
					if (flag9)
					{
						bool flag10 = this._curMakeItemSubToggleIndex == toggleIndex;
						if (!flag10)
						{
							return;
						}
						this._makeDropdown.value = 0;
					}
					this.SelectMakeItemSubType(toggleIndex, isOn);
				}, -1);
			}
			else
			{
				CToggleGroupObsolete toggleGroup = this._makeSubTogGroup.ToggleGroup;
				if (toggleGroup != null)
				{
					toggleGroup.Clear();
				}
			}
			bool targetIsMedicine = MakeItemSubType.Instance[this._makeItemSubtypeIdList.First<short>()].Result.ItemType == 8;
			bool flag3 = this._finishedMakeItemData != null || (!hasChanged && this._selectedMakeDropDownValue == 0) || (hasChanged && targetIsMedicine) || this._needKeepSubtypeSelectionOnNoneTarget;
			if (flag3)
			{
				this._needKeepSubtypeSelectionOnNoneTarget = false;
				bool hasSelectedSubtype = this._makeItemSubtypeIdList.Count > 1 && this._curMakeItemSubToggleIndex > -1;
				bool hasSelectedDropdown = lastMakeDropdownValue > 0;
				bool flag4 = hasSelectedSubtype || hasSelectedDropdown;
				if (flag4)
				{
					bool flag5 = hasChanged && targetIsMedicine;
					if (flag5)
					{
						hasSelectedDropdown = false;
						this._makeDropdown.value = 0;
					}
					bool flag6 = hasSelectedDropdown;
					if (flag6)
					{
						bool flag7 = this._makeDropdown.value == lastMakeDropdownValue;
						if (flag7)
						{
							this.OnMakeDropdownValueChanged(lastMakeDropdownValue);
						}
						else
						{
							this._makeDropdown.value = lastMakeDropdownValue;
						}
					}
					else
					{
						CToggleGroupObsolete toggleGroup2 = this._makeSubTogGroup.ToggleGroup;
						if (toggleGroup2 != null)
						{
							toggleGroup2.Set(this._curMakeItemSubToggleIndex, true, false);
						}
					}
				}
				else
				{
					this.SelectMakeItemSubType(-1, false);
				}
			}
			else
			{
				bool flag8 = !hasChanged && this._selectedMakeDropDownValue > 0 && this._currentTarget != null;
				if (flag8)
				{
					this._needKeepTypeSelectionOnTargetChanged = true;
				}
				else
				{
					this._selectedMakeDropDownValue = 0;
					this.SelectMakeItemSubType(-1, false);
				}
			}
		}
	}

	// Token: 0x06001AE1 RID: 6881 RVA: 0x000B5EF8 File Offset: 0x000B40F8
	private void SelectMakeItemSubType(int index, bool isManual)
	{
		bool needResetMakeDropDownValue = this._needResetMakeDropDownValue;
		if (needResetMakeDropDownValue)
		{
			this._makeDropdown.value = 0;
		}
		this.HideResourceSprite();
		this._curMakeItemSubToggleIndex = index;
		this._makeIsManual = isManual;
		bool flag = !isManual;
		if (flag)
		{
			index = Random.Range(0, this._makeItemSubtypeIdList.Count);
			this._curMakeItemSubToggleIndex = -1;
		}
		this._makeItemSubTypeId = this._makeItemSubtypeIdList[index];
		MakeItemSubTypeItem makeItemSubTypeConfig = MakeItemSubType.Instance[this._makeItemSubTypeId];
		bool needAverage = !isManual && makeItemSubTypeConfig.Result.ItemType == 8 && this._makeItemSubtypeIdList.Count > 1;
		short maxMakeResourceTotalCount;
		if (!needAverage)
		{
			maxMakeResourceTotalCount = makeItemSubTypeConfig.ResourceTotalCount;
		}
		else
		{
			maxMakeResourceTotalCount = (short)this._makeItemSubtypeIdList.Average((short id) => (int)MakeItemSubType.Instance[id].ResourceTotalCount);
		}
		this._maxMakeResourceTotalCount = maxMakeResourceTotalCount;
		this._maxMakeResourceCountInts.Initialize();
		sbyte resourceType;
		sbyte resourceType2;
		for (resourceType = 0; resourceType < 6; resourceType = resourceType2 + 1)
		{
			this._maxMakeResourceCountInts.Set((int)resourceType, (int)(needAverage ? ((short)this._makeItemSubtypeIdList.Average((short id) => (int)MakeItemSubType.Instance[id].MaxMaterialResources.Get((int)resourceType))) : makeItemSubTypeConfig.MaxMaterialResources.Get((int)resourceType)));
			resourceType2 = resourceType;
		}
		this._curMakeResourceCountInts.Initialize();
		this.CheckMakeCondition(true, delegate
		{
			this.AutoFillResource();
			this.RefreshButtonPerfect(true);
		});
	}

	// Token: 0x06001AE2 RID: 6882 RVA: 0x000B6084 File Offset: 0x000B4284
	private void RefreshMakeTip()
	{
		bool flag = this._currentTarget == null || this._curTab != UI_Make.UIMakeTab.Make || this.NeedLockOnMaking || this.CurMakeResult.MakeResultItemArray == null;
		if (flag)
		{
			this._previewTip.enabled = false;
		}
		else
		{
			this.ShowResultPreviewImage(true);
			bool flag2 = !this._previewTip.gameObject.activeSelf;
			if (flag2)
			{
				this._previewTip.gameObject.SetActive(true);
			}
			this._previewTip.enabled = true;
			this._previewTip.Type = TipType.MakeItem;
			string title = ItemTemplateHelper.GetName(this._currentTarget.Key.ItemType, this._currentTarget.Key.TemplateId);
			this._previewTip.RuntimeParam = new ArgumentBox().SetObject("MakeResult", this.CurMakeResult).Set("Title", title);
		}
	}

	// Token: 0x06001AE3 RID: 6883 RVA: 0x000B6178 File Offset: 0x000B4378
	private void CheckMakeCondition(bool needRefreshMakeResult, Action onCheckEnd = null)
	{
		this._buttonConfirm.interactable = false;
		if (needRefreshMakeResult)
		{
			this.RefreshMakeResult(delegate
			{
				this.OnCheckMakeCondition();
				Action onCheckEnd3 = onCheckEnd;
				if (onCheckEnd3 != null)
				{
					onCheckEnd3();
				}
			});
		}
		else
		{
			this.OnCheckMakeCondition();
			Action onCheckEnd2 = onCheckEnd;
			if (onCheckEnd2 != null)
			{
				onCheckEnd2();
			}
			this.RefreshMakeTip();
		}
	}

	// Token: 0x06001AE4 RID: 6884 RVA: 0x000B61E4 File Offset: 0x000B43E4
	private void OnCheckMakeCondition()
	{
		UI_Make.<>c__DisplayClass267_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		this._buttonConfirm.interactable = false;
		bool needLockOnMaking = this.NeedLockOnMaking;
		if (needLockOnMaking)
		{
			bool flag = this._workingMakeItemData.LeftTime == 0;
			if (flag)
			{
				this._confirmNormalLabel.text = LocalStringManager.Get(LanguageKey.LK_Making_State_Finished);
				this._buttonConfirm.interactable = true;
				this._buttonConfirm.GetComponent<Refers>().CGet<GameObject>("Start").SetActive(false);
				this._buttonConfirm.GetComponent<Refers>().CGet<GameObject>("Get").SetActive(true);
				this._makePanel.gameObject.SetActive(false);
			}
			else
			{
				this._confirmDisableLabel.text = LocalStringManager.Get(LanguageKey.LK_Making_State_Making);
				this._buttonConfirm.GetComponent<Refers>().CGet<GameObject>("Start").SetActive(true);
				this._buttonConfirm.GetComponent<Refers>().CGet<GameObject>("Get").SetActive(false);
				this._makeTimeLabel.text = this._workingMakeItemData.LeftTime.ToString();
				this._makePanel.gameObject.SetActive(true);
			}
			this.ShowMakeDropdown(false, 0);
		}
		else
		{
			this._makeCountButtonLess.gameObject.SetActive(!this.NeedLockOnMaking);
			this._makeCountButtonMore.gameObject.SetActive(!this.NeedLockOnMaking);
			this._makeCountButtonMin.gameObject.SetActive(!this.NeedLockOnMaking);
			this._makeCountButtonMax.gameObject.SetActive(!this.NeedLockOnMaking);
			this.RefreshMakeCount();
			bool flag2 = this._currentTarget == null;
			if (flag2)
			{
				this._makeToolDurabilityCost = 0;
				this._makeCountButtonLess.interactable = false;
				this._makeCountButtonMore.interactable = false;
				this._makeCountButtonMin.interactable = false;
				this._makeCountButtonMax.interactable = false;
				this._makePanel.gameObject.SetActive(false);
				this.ShowMakeTotalCount(false);
				this.CheckTool(0, 0);
				this.SetConfirmButtonTip(7603, true, TipType.SingleDesc, null);
				this.ShowMakeDropdown(false, 0);
			}
			else
			{
				bool flag3 = this._makeItemSubTypeId == -1;
				if (!flag3)
				{
					MakeItemSubTypeItem makeItemSubTypeConfig = MakeItemSubType.Instance[this._makeItemSubTypeId];
					this._makeTime = makeItemSubTypeConfig.Time * (this._makeCount / 3 + 1);
					this._makeTimeLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Making_Cost_Time, this._makeTime);
					this._makePanel.gameObject.SetActive(true);
					this.ShowMakeTotalCount(true);
					for (int i = 0; i < this._makeEffectList.transform.childCount; i++)
					{
						this._makeEffectList.transform.GetChild(i).gameObject.SetActive(false);
					}
					this._makeRequiredResourceInts = default(ResourceInts);
					bool resourceMeet = true;
					for (sbyte j = 0; j < 6; j += 1)
					{
						string resourceName = GameData.Domains.Character.ResourceType.GetName(j);
						Refers resourceRefers = this._makeRequireResourceList.CGet<Refers>(resourceName);
						Refers effectRefers = null;
						sbyte equipmentBonusType = -1;
						switch (j)
						{
						case 1:
							effectRefers = this._makeEffectList.CGet<Refers>(resourceName);
							equipmentBonusType = makeItemSubTypeConfig.WoodEffect;
							break;
						case 2:
							effectRefers = this._makeEffectList.CGet<Refers>(resourceName);
							equipmentBonusType = makeItemSubTypeConfig.MetalEffect;
							break;
						case 3:
							effectRefers = this._makeEffectList.CGet<Refers>(resourceName);
							equipmentBonusType = makeItemSubTypeConfig.JadeEffect;
							break;
						case 4:
							effectRefers = this._makeEffectList.CGet<Refers>(resourceName);
							equipmentBonusType = makeItemSubTypeConfig.FabricEffect;
							break;
						}
						bool flag4 = null == resourceRefers;
						if (!flag4)
						{
							int maxCount = this._maxMakeResourceCountInts.Get((int)j);
							resourceRefers.gameObject.SetActive(maxCount > 0);
							bool flag5 = maxCount <= 0;
							if (!flag5)
							{
								int curCount = this._curMakeResourceCountInts.Get((int)j);
								int curResource;
								int curRequiredResource;
								bool curResourceMeet = this.CheckMakeResource((int)this._makeCount, curCount, j, out curResource, out curRequiredResource);
								this._makeRequiredResourceInts.Set((int)j, curRequiredResource);
								bool flag6 = !curResourceMeet;
								if (flag6)
								{
									resourceMeet = false;
								}
								resourceRefers.CGet<TextMeshProUGUI>("CountLabel").text = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Count, curCount, maxCount);
								LanguageKey key = curResourceMeet ? LanguageKey.LK_Make_Resource_Require_Meet : LanguageKey.LK_Make_Resource_Require_Not_Meet;
								string color = curResourceMeet ? "brightblue" : "brightred";
								resourceRefers.CGet<TextMeshProUGUI>("ResourceLabel").text = LocalStringManager.GetFormat(key, curResource.ToString().SetColor(color), curRequiredResource);
								resourceRefers.CGet<CImage>("Fill").fillAmount = (float)curCount / (float)maxCount;
								MaterialResources curMaterialResources = default(MaterialResources);
								for (int k = 0; k < 6; k++)
								{
									int value = this._curMakeResourceCountInts.Get(k);
									curMaterialResources.Set(k, (short)value);
								}
								bool flag7 = effectRefers;
								if (flag7)
								{
									bool hasEffect = makeItemSubTypeConfig.JadeEffect + makeItemSubTypeConfig.MetalEffect + makeItemSubTypeConfig.WoodEffect + makeItemSubTypeConfig.FabricEffect > 0;
									sbyte itemType = makeItemSubTypeConfig.Result.ItemType;
									bool flag8 = ItemType.IsEquipmentItemType(itemType) && hasEffect;
									if (flag8)
									{
										ValueTuple<string, string, string> equipmentBonusInfo = this.GetEquipmentBonusInfo(equipmentBonusType, itemType);
										string typeName = equipmentBonusInfo.Item1;
										string typeIcon = equipmentBonusInfo.Item2;
										string tipContent = equipmentBonusInfo.Item3;
										int percentage = ItemTemplateHelper.GetMaterialResourceBonusValuePercentage(itemType, makeItemSubTypeConfig.Result.TemplateId, equipmentBonusType, curMaterialResources);
										bool flag9 = equipmentBonusType == 4;
										if (flag9)
										{
											percentage = 170 - percentage;
										}
										string rateStr = string.Format("{0}%", percentage);
										effectRefers.CGet<TextMeshProUGUI>("EffectNameLabel").text = typeName;
										effectRefers.CGet<TextMeshProUGUI>("EffectRateLabel").text = rateStr;
										effectRefers.CGet<CImage>("EffectImage").SetSprite(typeIcon, false, null);
										effectRefers.CGet<CImage>("EffectImage").gameObject.SetActive(!typeIcon.IsNullOrEmpty());
										TooltipInvoker tipDisplayer = effectRefers.CGet<TooltipInvoker>("EffectTip");
										tipDisplayer.Type = TipType.Simple;
										bool flag10 = tipDisplayer.PresetParam == null || tipDisplayer.PresetParam.Length == 0;
										if (flag10)
										{
											tipDisplayer.PresetParam = new string[2];
										}
										tipDisplayer.PresetParam[0] = typeName;
										tipDisplayer.PresetParam[1] = tipContent;
									}
									else
									{
										effectRefers.gameObject.SetActive(false);
									}
								}
								CButtonObsolete btnMore = resourceRefers.CGet<CButtonObsolete>("ButtonMore");
								CButtonObsolete btnLess = resourceRefers.CGet<CButtonObsolete>("ButtonLess");
								CButtonObsolete btnMax = resourceRefers.CGet<CButtonObsolete>("ButtonMax");
								CButtonObsolete btnMin = resourceRefers.CGet<CButtonObsolete>("ButtonMin");
								bool addCountIsMeet = curCount < maxCount && this.CurMakeResourceTotalCount < this._maxMakeResourceTotalCount;
								int nextCount = Mathf.Min(curCount + 1, maxCount);
								int num;
								int num2;
								bool moreResourceIsMeet = this.CheckMakeResource((int)this._makeCount, nextCount, j, out num, out num2);
								btnMore.interactable = (addCountIsMeet && moreResourceIsMeet);
								TooltipInvoker btnMoreTip = btnMore.GetComponent<TooltipInvoker>();
								btnMoreTip.enabled = !moreResourceIsMeet;
								int nextMaxCount = this.GetMaxMeetResourceCount((int)this._makeCount, j);
								bool maxResourceIsMeet = curCount < nextMaxCount;
								btnMax.interactable = (addCountIsMeet && maxResourceIsMeet);
								TooltipInvoker btnMaxTip = btnMax.GetComponent<TooltipInvoker>();
								btnMaxTip.enabled = !maxResourceIsMeet;
								btnLess.interactable = (btnMin.interactable = (curCount > 0));
								btnMore.gameObject.SetActive(true);
								btnLess.gameObject.SetActive(true);
								btnMax.gameObject.SetActive(true);
								btnMin.gameObject.SetActive(true);
							}
						}
					}
					LifeSkillShorts needLifeSkill = default(LifeSkillShorts);
					LifeSkillShorts showLifeSkill = default(LifeSkillShorts);
					MaterialItem materialConfig = Config.Material.Instance[this.MakeOriginTemplateId];
					int reducePercent = this.GetBuildingAttainmentEffect(materialConfig.RequiredLifeSkillType);
					short requiredAttainment = 0;
					showLifeSkill.Set((int)materialConfig.RequiredLifeSkillType, 1);
					needLifeSkill.Set((int)materialConfig.RequiredLifeSkillType, requiredAttainment);
					bool lifeSkillMeet = this.CheckAndShowLifeSkill(needLifeSkill, showLifeSkill);
					bool timeMeet = SingletonObject.getInstance<TimeManager>().IsActionDayEnough((int)this._makeTime);
					CS$<>8__locals1.allMeet = true;
					short oneCost = this.GetToolDurabilityCost(this._currentTool, materialConfig.Grade);
					this._makeToolDurabilityCost = (int)oneCost;
					short totalCost = Convert.ToInt16((int)(oneCost * this._makeCount));
					bool toolMeet = this.CheckTool(totalCost, oneCost);
					MakeResultStage stage = this.CurMakeResult.TargetResultStage;
					short templateId = (stage.TemplateId >= 0) ? stage.TemplateId : stage.TemplateIdList.First<short>();
					bool storageMeet = !stage.IsInit || ItemTemplateHelper.IsTransferable(stage.ItemType, templateId) || this._buildingStoreToDropdown.Value == 0;
					bool flag11 = !toolMeet;
					if (flag11)
					{
						CS$<>8__locals1.allMeet = false;
						this.<OnCheckMakeCondition>g__RefreshCountButton|267_0(ref CS$<>8__locals1);
					}
					else
					{
						bool flag12 = !timeMeet;
						if (flag12)
						{
							this.SetConfirmButtonTip(7686, true, TipType.SingleDesc, null);
							CS$<>8__locals1.allMeet = false;
							this.<OnCheckMakeCondition>g__RefreshCountButton|267_0(ref CS$<>8__locals1);
						}
						bool flag13 = !lifeSkillMeet;
						if (flag13)
						{
							this.SetConfirmButtonTip(7667, true, TipType.SingleDesc, null);
							CS$<>8__locals1.allMeet = false;
							this.<OnCheckMakeCondition>g__RefreshCountButton|267_0(ref CS$<>8__locals1);
						}
						else
						{
							bool flag14 = !resourceMeet;
							if (flag14)
							{
								this.SetConfirmButtonTip(7681, true, TipType.SingleDesc, null);
								CS$<>8__locals1.allMeet = false;
								this.<OnCheckMakeCondition>g__RefreshCountButton|267_0(ref CS$<>8__locals1);
							}
							else
							{
								bool flag15 = this.CurMakeResourceTotalCount != this._maxMakeResourceTotalCount;
								if (flag15)
								{
									this.SetConfirmButtonTip(7678, true, TipType.SingleDesc, null);
									CS$<>8__locals1.allMeet = false;
									this.<OnCheckMakeCondition>g__RefreshCountButton|267_0(ref CS$<>8__locals1);
								}
								else
								{
									bool flag16 = !storageMeet;
									if (flag16)
									{
										this.SetConfirmButtonTip(7674, true, TipType.SingleDesc, null);
										CS$<>8__locals1.allMeet = false;
										this.<OnCheckMakeCondition>g__RefreshCountButton|267_0(ref CS$<>8__locals1);
									}
									else
									{
										this._buttonConfirm.interactable = CS$<>8__locals1.allMeet;
										this.SetConfirmButtonTip(-1, !this._buttonConfirm.interactable, TipType.SingleDesc, null);
										this.<OnCheckMakeCondition>g__RefreshCountButton|267_0(ref CS$<>8__locals1);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001AE5 RID: 6885 RVA: 0x000B6C0C File Offset: 0x000B4E0C
	private void RefreshMakeResult(Action action)
	{
		UI_Make.<>c__DisplayClass268_0 CS$<>8__locals1 = new UI_Make.<>c__DisplayClass268_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.action = action;
		bool flag = this._isWaitingMakeResultRequest || this._makeItemSubtypeIdList == null;
		if (!flag)
		{
			this._previewTip.enabled = false;
			this._isWaitingMakeResultRequest = true;
			UI_Make.<>c__DisplayClass268_0 CS$<>8__locals2 = CS$<>8__locals1;
			ItemDisplayData currentTool = this._currentTool;
			CS$<>8__locals2.toolKey = ((currentTool != null) ? currentTool.Key : ItemKey.Invalid);
			this._makeResultDict.Clear();
			CS$<>8__locals1.curSendCount = 0;
			CS$<>8__locals1.allSendCount = ((this._makeItemSubtypeIdList.Count > 1) ? (this._makeItemSubtypeIdList.Count + 1) : 1);
			bool flag2 = this._makeItemSubtypeIdList.Count > 1;
			if (flag2)
			{
				foreach (short makeItemSubtypeId in this._makeItemSubtypeIdList)
				{
					CS$<>8__locals1.<RefreshMakeResult>g__Send|0(makeItemSubtypeId);
				}
			}
			CS$<>8__locals1.<RefreshMakeResult>g__Send|0(-1);
		}
	}

	// Token: 0x06001AE6 RID: 6886 RVA: 0x000B6D1C File Offset: 0x000B4F1C
	private bool CheckTool(short totalCost = 0, short oneCost = 0)
	{
		this._toolDurabilityCost = totalCost;
		this._itemViewToolSelected.SetUsedDurability(totalCost, false);
		bool flag = this._currentTool == null;
		bool result;
		if (flag)
		{
			this.SetConfirmButtonTip(7692, true, TipType.SingleDesc, null);
			result = false;
		}
		else
		{
			bool flag2 = this.IsEmptyTool(this._currentTool);
			if (flag2)
			{
				result = true;
			}
			else
			{
				short durability = this._currentTool.Durability;
				bool toolIsMeet = durability >= totalCost || durability + oneCost > totalCost;
				bool flag3 = !toolIsMeet;
				if (flag3)
				{
					this.SetConfirmButtonTip(7689, true, TipType.SingleDesc, null);
					result = false;
				}
				else
				{
					result = true;
				}
			}
		}
		return result;
	}

	// Token: 0x06001AE7 RID: 6887 RVA: 0x000B6DB4 File Offset: 0x000B4FB4
	private ValueTuple<string, string, string> GetEquipmentBonusInfo(sbyte equipmentBonusType, sbyte itemType)
	{
		LanguageKey nameKey = LanguageKey.Invalid;
		string nameContent = string.Empty;
		bool isWeapon = itemType == 0;
		string iconName = string.Empty;
		LanguageKey tipKey = LanguageKey.Invalid;
		string tipContent = string.Empty;
		switch (equipmentBonusType)
		{
		case 0:
			nameKey = (isWeapon ? LanguageKey.LK_WeaponEquipAttack : LanguageKey.LK_ArmorEquipAttack);
			iconName = (isWeapon ? "building_shuxing_pojia" : "building_shuxing_poren");
			tipKey = (isWeapon ? LanguageKey.LK_WeaponEquipAttack_Tip : LanguageKey.LK_ArmorEquipAttack_Tip);
			break;
		case 1:
			nameKey = LanguageKey.LK_EquipDefense;
			iconName = "building_shuxing_jianren";
			tipKey = (isWeapon ? LanguageKey.LK_EquipDefense_Weapon_Tip : LanguageKey.LK_EquipDefense_Armor_Tip);
			break;
		case 2:
			nameKey = LanguageKey.LK_Combat_Attack_Value;
			iconName = "building_shuxing_chuantou";
			tipKey = LanguageKey.LK_Combat_Attack_Value_Tip;
			break;
		case 3:
			nameKey = LanguageKey.LK_Combat_Defend_Value;
			iconName = "building_shuxing_fangchuan";
			tipKey = LanguageKey.LK_Combat_Defend_Value_Tip;
			break;
		case 4:
			nameKey = LanguageKey.LK_Weight;
			iconName = "building_shuxing_zhongliang";
			tipKey = LanguageKey.LK_Weight_Tip;
			break;
		}
		nameContent = LocalStringManager.Get(nameKey);
		tipContent = LocalStringManager.Get(tipKey);
		return new ValueTuple<string, string, string>(nameContent, iconName, tipContent);
	}

	// Token: 0x06001AE8 RID: 6888 RVA: 0x000B6EB8 File Offset: 0x000B50B8
	private bool CheckMakeResource(int makeCount)
	{
		bool resourceMeet = true;
		for (sbyte i = 0; i < 8; i += 1)
		{
			int maxCount = this._maxMakeResourceCountInts.Get((int)i);
			bool flag = maxCount <= 0;
			if (!flag)
			{
				int curCount = this._curMakeResourceCountInts.Get((int)i);
				int num;
				int num2;
				bool flag2 = !this.CheckMakeResource(makeCount, curCount, i, out num, out num2);
				if (flag2)
				{
					resourceMeet = false;
				}
			}
		}
		return resourceMeet;
	}

	// Token: 0x06001AE9 RID: 6889 RVA: 0x000B6F28 File Offset: 0x000B5128
	private bool CheckMakeResource(int makeCount, int resourceCount, sbyte resourceType, out int curResource, out int curRequiredResource)
	{
		short amount = ItemTemplateHelper.GetCraftMaterialRequiredResourceAmount(this.MakeOriginTemplateId);
		curRequiredResource = resourceCount * makeCount * (int)amount;
		curResource = this._buildingModel.GetResourceCount(resourceType);
		return curResource >= curRequiredResource;
	}

	// Token: 0x06001AEA RID: 6890 RVA: 0x000B6F68 File Offset: 0x000B5168
	private int GetMaxMeetResourceCount(int makeCount, sbyte resourceType)
	{
		makeCount = Mathf.Max(1, makeCount);
		int curResource = this._buildingModel.GetResourceCount(resourceType);
		short amount = ItemTemplateHelper.GetCraftMaterialRequiredResourceAmount(this.MakeOriginTemplateId);
		return curResource / ((int)amount * makeCount);
	}

	// Token: 0x06001AEB RID: 6891 RVA: 0x000B6FA4 File Offset: 0x000B51A4
	private void ConfirmMake()
	{
		this._buttonConfirm.interactable = false;
		ItemDisplayData target = this._allItems.Find((ItemDisplayData d) => d.ContainsItemKey(this._currentTarget.Key));
		bool flag = target != null;
		if (flag)
		{
			target.Amount -= (int)this._makeCount;
		}
		ItemDisplayData tool = this._allItems.Find((ItemDisplayData d) => d.ContainsItemKey(this._currentTool.Key));
		bool flag2 = tool != null;
		if (flag2)
		{
			ItemDisplayData itemDisplayData = tool;
			itemDisplayData.Durability -= this._toolDurabilityCost;
		}
		List<short> resultTemplateIdList = new List<short>((int)this._makeCount);
		short itemTemplateId = (this._makeDropdown.value == 0) ? -1 : this._curMakeDropdownDataList[this._makeDropdown.value].Item2;
		for (int i = 0; i < (int)this._makeCount; i++)
		{
			bool flag3 = this._makeDropdown.value == 0;
			if (flag3)
			{
				itemTemplateId = this.CurMakeResult.TargetResultStage.GetGradeAndId(GameApp.Random).Item2;
			}
			resultTemplateIdList.Add(itemTemplateId);
		}
		MakeConditionArguments makeConditionArguments = new MakeConditionArguments
		{
			BuildingBlockKey = this._buildingBlockKey,
			CharId = this._taiwuCharId,
			IsManual = this._makeIsManual,
			MakeCount = this._makeCount,
			MakeItemSubTypeId = this._makeItemSubTypeId,
			MakeItemTypeId = this._makeItemTypeId,
			MaterialKey = this._currentTarget.Key,
			ResourceCount = this._curMakeResourceCountInts,
			ToolKey = this._currentTool.Key,
			IsPerfect = this._makePerfect
		};
		BuildingDomainMethod.AsyncCall.CheckMakeCondition(this, makeConditionArguments, delegate(int offset, RawDataPool dataPool)
		{
			bool isMeet = false;
			Serializer.Deserialize(dataPool, offset, ref isMeet);
			bool flag4 = !isMeet;
			if (flag4)
			{
				UIElement.FullScreenMask.Hide(false);
			}
			else
			{
				bool makePerfect = this._makePerfect;
				if (makePerfect)
				{
					this.CGet<GameObject>("PerfectEffectConfirm").SetActive(this._makePerfect);
					this.ShowPerfectEffectLoop(false, true);
					this.ShowResultPreviewImage(false);
					this.PlayCenterAnim(true, true, true);
					SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1.5f, new Action(base.<ConfirmMake>g__Action|3));
				}
				else
				{
					base.<ConfirmMake>g__Action|3();
				}
			}
		});
	}

	// Token: 0x170002CA RID: 714
	// (get) Token: 0x06001AEC RID: 6892 RVA: 0x000B7182 File Offset: 0x000B5382
	private short MakeOriginTemplateId
	{
		get
		{
			return this._currentTarget.Key.TemplateId;
		}
	}

	// Token: 0x06001AED RID: 6893 RVA: 0x000B7194 File Offset: 0x000B5394
	private void UpdateMakeState(bool resetCurrentTargetAndTool = true)
	{
		bool needLockOnMaking = this.NeedLockOnMaking;
		if (needLockOnMaking)
		{
			if (resetCurrentTargetAndTool)
			{
				List<ItemDisplayData> source = this.GetItemsSource(this._materialTogGroup);
				ItemDisplayData itemDisplayData;
				if ((itemDisplayData = source.Find((ItemDisplayData d) => d.ContainsItemKey(this._workingMakeItemData.MaterialKey))) == null && (itemDisplayData = this._allItems.Find((ItemDisplayData d) => d.ContainsItemKey(this._workingMakeItemData.MaterialKey))) == null)
				{
					ItemDisplayData itemDisplayData2 = new ItemDisplayData();
					itemDisplayData2.Key = this._workingMakeItemData.MaterialKey;
					itemDisplayData = itemDisplayData2;
					itemDisplayData2.Amount = 0;
				}
				ItemDisplayData target = itemDisplayData;
				this.ChangeCurrentTarget(target, false);
				bool flag = this._workingMakeItemData.ToolKey.IsValid();
				if (flag)
				{
					bool flag2 = this.IsEmptyTool(this._workingMakeItemData.ToolKey);
					ItemDisplayData tool;
					if (flag2)
					{
						tool = new ItemDisplayData
						{
							Key = this._emptyToolKey
						};
					}
					else
					{
						ItemDisplayData itemDisplayData3;
						if ((itemDisplayData3 = this._allItems.Find((ItemDisplayData d) => d.ContainsItemKey(this._workingMakeItemData.ToolKey))) == null)
						{
							ItemDisplayData itemDisplayData4 = new ItemDisplayData();
							itemDisplayData4.Key = this._workingMakeItemData.ToolKey;
							itemDisplayData4.Durability = 0;
							itemDisplayData3 = itemDisplayData4;
							itemDisplayData4.MaxDurability = ItemTemplateHelper.GetBaseMaxDurability(this._workingMakeItemData.ToolKey.ItemType, this._workingMakeItemData.ToolKey.TemplateId);
						}
						tool = itemDisplayData3;
					}
					this.ChangeCurrentTool(tool);
				}
			}
			this.ShowMakeRequireResourceList(false);
			this.ShowMakeEffectList(false);
			this._makePageSwitchController.gameObject.SetActive(false);
			this._makeSubTogGroup.gameObject.SetActive(false);
			this._makeCountButtonLess.gameObject.SetActive(false);
			this._makeCountButtonMore.gameObject.SetActive(false);
			this._makeCountButtonMin.gameObject.SetActive(false);
			this._makeCountButtonMax.gameObject.SetActive(false);
			this.HideLifeSkill();
			this._toolDurabilityText.transform.parent.gameObject.SetActive(false);
			this.RefreshToolAttainment(false);
			this.ShowMakeTotalCount(false);
			this.ShowResultPreviewImage(false);
			this._previewTip.enabled = false;
			this.SetConfirmButtonTip(-1, false, TipType.SingleDesc, null);
			bool flag3 = this._workingMakeItemData.LeftTime == 0;
			if (flag3)
			{
				this.PlayCenterAnim(false, false, false);
				this._makePanel.gameObject.SetActive(false);
			}
			else
			{
				this.PlayCenterAnim(true, true, false);
				this._makeTimeLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Making_Cost_Time, this._workingMakeItemData.LeftTime);
				this._makePanel.gameObject.SetActive(true);
			}
		}
		this._itemViewTools.ReRender();
		this._itemViewMaterial.ReRender();
		this.RefreshButtonPerfect(true);
	}

	// Token: 0x06001AEE RID: 6894 RVA: 0x000B7443 File Offset: 0x000B5643
	private void ConfirmGetMakeItems()
	{
		this._buttonConfirm.interactable = false;
		BuildingDomainMethod.Call.GetMakeItems(this.Element.GameDataListenerId, this._buildingBlockKey);
		this.RefreshAllItems();
	}

	// Token: 0x06001AEF RID: 6895 RVA: 0x000B7474 File Offset: 0x000B5674
	private void AutoFillResource()
	{
		bool flag = this._currentTarget == null;
		if (!flag)
		{
			sbyte resourceType = ItemTemplateHelper.GetResourceType(this._currentTarget.Key.ItemType, this._currentTarget.Key.TemplateId);
			bool flag2 = resourceType == 5 || resourceType == 0;
			if (flag2)
			{
				int curCount = this._maxMakeResourceCountInts.Get((int)resourceType);
				int num;
				int num2;
				bool curResourceMeet = this.CheckMakeResource((int)this._makeCount, curCount, resourceType, out num, out num2);
				bool flag3 = curResourceMeet;
				if (flag3)
				{
					this.ChangeMakeResourceCount(null, GameData.Domains.Character.ResourceType.GetName(resourceType), true, true, false);
				}
			}
		}
	}

	// Token: 0x06001AF0 RID: 6896 RVA: 0x000B7504 File Offset: 0x000B5704
	private void AutoFillResourceOnMakeEnd()
	{
		bool flag = this._lastMakeResourceCount.GetSum() <= 0;
		if (!flag)
		{
			this._curMakeResourceCountInts.Initialize();
			for (sbyte resourceType = 0; resourceType < 6; resourceType += 1)
			{
				short curCount = this._lastMakeResourceCount.Get((int)resourceType);
				bool flag2 = curCount <= 0;
				if (!flag2)
				{
					int num;
					int num2;
					bool curResourceMeet = this.CheckMakeResource((int)this._makeCount, (int)curCount, resourceType, out num, out num2);
					bool flag3 = curResourceMeet;
					if (flag3)
					{
						this.ChangeMakeResourceCount(null, GameData.Domains.Character.ResourceType.GetName(resourceType), true, true, true);
					}
				}
			}
		}
	}

	// Token: 0x06001AF1 RID: 6897 RVA: 0x000B7594 File Offset: 0x000B5794
	private void RefreshMakeDropDown()
	{
		UI_Make.<>c__DisplayClass281_0 CS$<>8__locals1 = new UI_Make.<>c__DisplayClass281_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.optionList = EasyPool.Get<List<string>>();
		CS$<>8__locals1.dataList = EasyPool.Get<List<ValueTuple<short, short, string>>>();
		bool flag = this._makeItemSubtypeIdList.Count > 1;
		if (flag)
		{
			foreach (short subTypeId in this._makeItemSubtypeIdList)
			{
				MakeResult makeResult;
				bool flag2 = this._makeResultDict.TryGetValue((int)subTypeId, out makeResult);
				if (flag2)
				{
					CS$<>8__locals1.<RefreshMakeDropDown>g__AddData|0(makeResult);
				}
			}
		}
		else
		{
			MakeResult makeResult2;
			bool flag3 = !this._makeResultDict.TryGetValue(-1, out makeResult2);
			if (flag3)
			{
				EasyPool.Free<List<string>>(CS$<>8__locals1.optionList);
				EasyPool.Free<List<ValueTuple<short, short, string>>>(CS$<>8__locals1.dataList);
				return;
			}
			CS$<>8__locals1.<RefreshMakeDropDown>g__AddData|0(makeResult2);
		}
		bool flag4 = CS$<>8__locals1.optionList.Count > 1;
		if (flag4)
		{
			string defaultName = LocalStringManager.Get(LanguageKey.LK_Default);
			CS$<>8__locals1.optionList.Insert(0, defaultName);
			CS$<>8__locals1.dataList.Insert(0, new ValueTuple<short, short, string>(-1, -1, defaultName));
		}
		bool same = true;
		bool flag5 = CS$<>8__locals1.optionList.Count == this._makeDropdownOptionList.Count;
		if (flag5)
		{
			for (int i = 0; i < this._makeDropdownOptionList.Count; i++)
			{
				bool flag6 = CS$<>8__locals1.optionList[i] != this._makeDropdownOptionList[i];
				if (flag6)
				{
					same = false;
					break;
				}
			}
		}
		else
		{
			same = false;
		}
		bool flag7 = !same;
		if (flag7)
		{
			this._makeDropdownOptionList.Clear();
			this._curMakeDropdownDataList.Clear();
			this._makeDropdownOptionList.AddRange(CS$<>8__locals1.optionList);
			this._curMakeDropdownDataList.AddRange(CS$<>8__locals1.dataList);
			this._makeDropdown.ClearOptions();
			this._makeDropdown.AddOptions(CS$<>8__locals1.optionList);
			CS$<>8__locals1.<RefreshMakeDropDown>g__ResetValue|1();
			this._lastMakeDropdownDataList.Clear();
			this._lastMakeDropdownDataList.AddRange(CS$<>8__locals1.dataList);
		}
		this.ShowMakeDropdown(CS$<>8__locals1.optionList.Count > 1, CS$<>8__locals1.optionList.Count);
		EasyPool.Free<List<string>>(CS$<>8__locals1.optionList);
		EasyPool.Free<List<ValueTuple<short, short, string>>>(CS$<>8__locals1.dataList);
	}

	// Token: 0x06001AF2 RID: 6898 RVA: 0x000B77FC File Offset: 0x000B59FC
	private void OnMakeDropdownValueChanged(int value)
	{
		int maxValue = Math.Max(0, this._curMakeDropdownDataList.Count - 1);
		this._selectedMakeDropDownValue = Math.Clamp(value, 0, maxValue);
		this._makeDropdown.SetValueWithoutNotify(this._selectedMakeDropDownValue);
		bool flag = !this._curMakeDropdownDataList.Count.CheckIndex(this._selectedMakeDropDownValue);
		if (!flag)
		{
			this._makeDropdown.captionText.text = this._makeDropdownOptionList[this._selectedMakeDropDownValue];
			CImage gradeImage = base.CGet<CImage>("GradeBack");
			short subTypeId = this._curMakeDropdownDataList[this._selectedMakeDropDownValue].Item1;
			bool flag2 = subTypeId < 0;
			if (flag2)
			{
				gradeImage.gameObject.SetActive(false);
				bool flag3 = this._makeSubTogGroup.ToggleGroup && !this._isWaitingMakeResultRequest;
				if (flag3)
				{
					CToggleObsolete active = this._makeSubTogGroup.ToggleGroup.GetActive();
					bool flag4 = active && active.isOn;
					if (flag4)
					{
						this._makeSubTogGroup.ToggleGroup.Set(active, false);
					}
				}
			}
			else
			{
				this._makeItemTemplateId = this._curMakeDropdownDataList[this._selectedMakeDropDownValue].Item2;
				sbyte grade = ItemTemplateHelper.GetGrade(MakeItemSubType.Instance[subTypeId].Result.ItemType, this._makeItemTemplateId);
				gradeImage.gameObject.SetActive(true);
				gradeImage.SetSprite(ItemView.GetGradeIcon(grade), false, null);
				TextMeshProUGUI componentInChildren = gradeImage.GetComponentInChildren<TextMeshProUGUI>();
				if (componentInChildren != null)
				{
					componentInChildren.SetText(ItemView.GetGradeText(grade), true);
				}
				this._needResetMakeDropDownValue = false;
				bool flag5 = this._makeItemSubtypeIdList.Count > 1 && this._makeSubTogGroup.ToggleGroup;
				if (flag5)
				{
					int index = this._makeItemSubtypeIdList.IndexOf(subTypeId);
					this._makeSubTogGroup.ToggleGroup.Set(index, true, true);
				}
				else
				{
					this.SelectMakeItemSubType(-1, false);
				}
				this._needResetMakeDropDownValue = true;
			}
		}
	}

	// Token: 0x06001AF3 RID: 6899 RVA: 0x000B7A08 File Offset: 0x000B5C08
	private void ShowMakeDropdown(bool show, int count = 0)
	{
		this._buildingStoreToDropdown.Active = (count > 0);
		bool flag = this._makeDropdown.gameObject.activeSelf != show;
		if (flag)
		{
			this._makeDropdown.gameObject.SetActive(show);
		}
		bool flag2 = !show;
		if (flag2)
		{
			this.ShowMakeDropdownMask(false);
		}
	}

	// Token: 0x06001AF4 RID: 6900 RVA: 0x000B7A64 File Offset: 0x000B5C64
	private void ShowMakeDropdownMask(bool show)
	{
		bool flag = this._makeDropdownMask.gameObject.activeSelf != show;
		if (flag)
		{
			this._makeDropdownMask.gameObject.SetActive(show);
		}
	}

	// Token: 0x06001AF5 RID: 6901 RVA: 0x000B7AA0 File Offset: 0x000B5CA0
	private void OnMakeDropdownShow(GameObject go)
	{
		UIManager.Instance.SetEscHandler(new Action(this.OnMakeDropdownHide));
		this.ShowMakeDropdownMask(true);
		CToggleObsolete[] toggles = this._makeDropdown.GetComponentsInChildren<CToggleObsolete>();
		PositionFollower positionFollower = this._makeDropdown.GetComponentInChildren<PositionFollower>();
		foreach (CToggleObsolete togCell in toggles)
		{
			bool flag = !togCell.gameObject.activeSelf;
			if (!flag)
			{
				togCell.transform.Find("Disable").gameObject.SetActive(togCell.isOn);
				bool flag2 = togCell.isOn && positionFollower;
				if (flag2)
				{
					positionFollower.Target = togCell.transform;
				}
			}
		}
		Transform trans = this._makeDropdown.transform.Find("Dropdown List");
		bool flag3 = !trans;
		if (!flag3)
		{
			RectTransform content = trans.GetComponentInChildren<CScrollRectLegacy>().Content;
			int childCount = content.childCount;
			for (int i = 1; i < childCount; i++)
			{
				Transform item = content.GetChild(i);
				CImage gradeBack = item.Find("Layout/GradeBack").GetComponent<CImage>();
				bool flag4 = i == 1;
				if (flag4)
				{
					bool activeSelf = gradeBack.gameObject.activeSelf;
					if (activeSelf)
					{
						gradeBack.gameObject.SetActive(false);
					}
				}
				else
				{
					bool flag5 = !gradeBack.gameObject.activeSelf;
					if (flag5)
					{
						gradeBack.gameObject.SetActive(true);
					}
					int index = i - 1;
					short subTypeId = this._curMakeDropdownDataList[index].Item1;
					short itemTemplateId = this._curMakeDropdownDataList[index].Item2;
					sbyte grade = ItemTemplateHelper.GetGrade(MakeItemSubType.Instance[subTypeId].Result.ItemType, itemTemplateId);
					CImage component = gradeBack.GetComponent<CImage>();
					if (component != null)
					{
						component.SetSprite(ItemView.GetGradeIcon(grade), false, null);
					}
					TextMeshProUGUI componentInChildren = gradeBack.GetComponentInChildren<TextMeshProUGUI>();
					if (componentInChildren != null)
					{
						componentInChildren.SetText(ItemView.GetGradeText(grade), true);
					}
					this.SetMouseTipDisplayer(item.GetComponent<TooltipInvoker>(), MakeItemSubType.Instance[subTypeId].Result.ItemType, itemTemplateId);
				}
			}
			this._makeDropdown.transform.Find("Dropdown List").GetComponent<Canvas>().sortingOrder = 640;
			Transform blocker = base.transform.parent.parent.Find("Blocker");
			bool flag6 = blocker != null;
			if (flag6)
			{
				blocker.GetComponent<Canvas>().sortingOrder = 639;
			}
		}
	}

	// Token: 0x06001AF6 RID: 6902 RVA: 0x000B7D40 File Offset: 0x000B5F40
	private void OnMakeDropdownHide()
	{
		this.ShowMakeDropdownMask(false);
		UIManager.Instance.SetEscHandler(null);
	}

	// Token: 0x06001AF7 RID: 6903 RVA: 0x000B7D58 File Offset: 0x000B5F58
	private void SetMouseTipDisplayer(TooltipInvoker tipDisplayer, sbyte itemType, short templateId)
	{
		tipDisplayer.RuntimeParam = null;
		TemplateKey itemKey = new TemplateKey(itemType, templateId);
		ItemDisplayData Data = new ItemDisplayData(itemKey.ItemType, itemKey.TemplateId);
		tipDisplayer.Type = TooltipManager.ItemTypeToTipType[Data.Key.ItemType];
		tipDisplayer.NeedRefresh = (UIElement.Combat.Exist && Data.Key.ItemType == 0 && Data.UsingType == ItemDisplayData.ItemUsingType.Equiped);
		tipDisplayer.RuntimeParam = new ArgumentBox().SetObject("ItemData", Data.Clone(-1));
		tipDisplayer.RuntimeParam.Set("ShowPageInfo", Data.Key.ItemType == 10);
		tipDisplayer.RuntimeParam.Set("TemplateDataOnly", true);
		tipDisplayer.RuntimeParam.Set("CharId", Data.OwnerCharId);
	}

	// Token: 0x06001AF8 RID: 6904 RVA: 0x000B7E38 File Offset: 0x000B6038
	private void ShowMakeRequireResourceList(bool isShow)
	{
		this._makeRequireResourceList.gameObject.SetActive(isShow);
		bool flag = !isShow;
		if (flag)
		{
			for (int i = 0; i < this._makeRequireResourceList.transform.childCount; i++)
			{
				this._makeRequireResourceList.transform.GetChild(i).gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06001AF9 RID: 6905 RVA: 0x000B7EA0 File Offset: 0x000B60A0
	private void ShowMakeEffectList(bool isShow)
	{
		this._makeEffectList.gameObject.SetActive(isShow);
		bool flag = !isShow;
		if (flag)
		{
			for (int i = 0; i < this._makeEffectList.transform.childCount; i++)
			{
				this._makeEffectList.transform.GetChild(i).gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06001AFA RID: 6906 RVA: 0x000B7F08 File Offset: 0x000B6108
	private void ShowMakeTotalCount(bool isShow)
	{
		this._makeTotalCountLabel.transform.parent.gameObject.SetActive(isShow);
		string content = isShow ? LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Count, this.CurMakeResourceTotalCount, this._maxMakeResourceTotalCount) : string.Empty;
		this._makeTotalCountLabel.text = content;
	}

	// Token: 0x06001AFB RID: 6907 RVA: 0x000B7F6A File Offset: 0x000B616A
	private void InitRepair()
	{
	}

	// Token: 0x06001AFC RID: 6908 RVA: 0x000B7F70 File Offset: 0x000B6170
	private void CheckRepairCondition()
	{
		this._buttonConfirm.interactable = false;
		bool flag = this._currentTarget == null;
		if (flag)
		{
			this.ShowMakeTotalCount(false);
			this.SetConfirmButtonTip(9733, true, TipType.SingleDesc, null);
		}
		else
		{
			this.ShowMakeTotalCount(true);
			this._makeRequiredResourceInts = default(ResourceInts);
			sbyte grade = ItemTemplateHelper.GetGrade(this._currentTarget.Key.ItemType, this.MakeOriginTemplateId);
			sbyte lifeSkillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(this._currentTarget.Key.ItemType, this.MakeOriginTemplateId);
			float factor = (this._currentTarget.Durability == 0) ? 1f : 0.5f;
			bool resourceMeet = true;
			for (sbyte i = 0; i < 6; i += 1)
			{
				Refers refers = this._makeRequireResourceList.CGet<Refers>(GameData.Domains.Character.ResourceType.GetName(i));
				bool flag2 = null == refers;
				if (!flag2)
				{
					int curCount = this._curMakeResourceCountInts.Get((int)i);
					refers.gameObject.SetActive(curCount > 0);
					bool flag3 = curCount <= 0;
					if (!flag3)
					{
						short baseResource = GlobalConfig.Instance.RepairBaseResourseRequirement[(int)grade];
						int curRequiredResource = (int)((float)curCount * factor * (float)baseResource);
						this._makeRequiredResourceInts.Set((int)i, curRequiredResource);
						int curResource = this._buildingModel.GetResourceCount(i);
						bool curResourceMeet = curRequiredResource <= curResource && curResource > 0;
						bool flag4 = !curResourceMeet;
						if (flag4)
						{
							resourceMeet = false;
						}
						int maxCount = this._maxMakeResourceCountInts.Get((int)i);
						refers.CGet<TextMeshProUGUI>("CountLabel").text = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Count, curCount, maxCount);
						LanguageKey key = curResourceMeet ? LanguageKey.LK_Make_Resource_Require_Meet : LanguageKey.LK_Make_Resource_Require_Not_Meet;
						string color = curResourceMeet ? "brightblue" : "brightred";
						refers.CGet<TextMeshProUGUI>("ResourceLabel").text = LocalStringManager.GetFormat(key, curResource.ToString().SetColor(color), curRequiredResource);
						refers.CGet<CImage>("Fill").fillAmount = 0f;
						refers.CGet<CButtonObsolete>("ButtonMore").interactable = false;
						refers.CGet<CButtonObsolete>("ButtonMore").GetComponent<TooltipInvoker>().enabled = false;
						refers.CGet<CButtonObsolete>("ButtonLess").interactable = false;
						refers.CGet<CButtonObsolete>("ButtonMax").interactable = false;
						refers.CGet<CButtonObsolete>("ButtonMax").GetComponent<TooltipInvoker>().enabled = false;
						refers.CGet<CButtonObsolete>("ButtonMin").interactable = false;
					}
				}
			}
			LifeSkillShorts needLifeSkill = default(LifeSkillShorts);
			LifeSkillShorts showLifeSkill = default(LifeSkillShorts);
			short requiredAttainment = (short)((float)GlobalConfig.Instance.RepairAttainments[(int)grade] * factor);
			requiredAttainment = this.GetAttainmentByBuildingEffect(lifeSkillType, requiredAttainment);
			needLifeSkill.Set((int)lifeSkillType, requiredAttainment);
			showLifeSkill.Set((int)lifeSkillType, 1);
			bool lifeSkillMeet = this.CheckAndShowLifeSkill(needLifeSkill, showLifeSkill);
			short durabilityCost = this.GetToolDurabilityCost(this._currentTool, grade);
			bool toolMeet = this.CheckTool(durabilityCost, durabilityCost);
			bool flag5 = !toolMeet;
			if (!flag5)
			{
				bool flag6 = !lifeSkillMeet;
				if (flag6)
				{
					this.SetConfirmButtonTip(7667, true, TipType.SingleDesc, null);
				}
				else
				{
					bool flag7 = !resourceMeet;
					if (flag7)
					{
						this.SetConfirmButtonTip(7681, true, TipType.SingleDesc, null);
					}
					else
					{
						bool targetMeet = this._currentTarget.Durability < this._currentTarget.MaxDurability;
						this._buttonConfirm.interactable = targetMeet;
						this.SetConfirmButtonTip(-1, true, TipType.SingleDesc, null);
					}
				}
			}
		}
	}

	// Token: 0x06001AFD RID: 6909 RVA: 0x000B82F0 File Offset: 0x000B64F0
	private void ConfirmRepair()
	{
		this._buttonConfirm.interactable = false;
		BuildingDomainMethod.AsyncCall.CheckRepairConditionIsMeet(this, this._taiwuCharId, this._currentTool.Key, this._currentTarget.Key, this._buildingBlockKey, delegate(int offset, RawDataPool dataPool)
		{
			bool isMeet = false;
			Serializer.Deserialize(dataPool, offset, ref isMeet);
			bool flag = !isMeet;
			if (flag)
			{
				UIElement.FullScreenMask.Hide(false);
			}
			else
			{
				BuildingDomainMethod.AsyncCall.RepairItemOptional(this, this._taiwuCharId, this._currentTool.Key, this._currentTarget.Key, this._currentTool.ItemSourceType, delegate(int offset2, RawDataPool dataPool2)
				{
					Serializer.Deserialize(dataPool2, offset2, ref this._resultItemDisplayData);
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					argBox.SetObject("ItemList", new List<ItemDisplayData>
					{
						this._resultItemDisplayData
					});
					argBox.Set("ObtainType", 7);
					argBox.Set("IsNew", false);
					UIElement.GetItem.SetOnInitArgs(argBox);
					UIManager.Instance.MaskUI(UIElement.GetItem);
					this.RefreshAllItems();
				});
			}
		});
	}

	// Token: 0x06001AFE RID: 6910 RVA: 0x000B8340 File Offset: 0x000B6540
	private void InitPoison()
	{
		this._poisonPageSwitchController = base.CGet<Refers>("Poison").CGet<HorizontalPageSwitchController>("PageSwitchController");
		this._poisonPageSwitchController.PageItemRefreshHandler = new Action<int, Refers>(this.RefreshPoisonPageSwitchItem);
		this._poisonPageSwitchController.SetItemSelectStateHandler = new Action<Refers, bool>(this.SetPoisonPageItemSelectState);
		this._poisonPageSwitchController.InitPageCount(0, -1, false);
		this._poisonPageSwitchController.RegisterOnSelectIndexChangeHandler(new Action<int>(this.OnSelectPoisonPageIndexChange));
		this._poisonPageSwitchController.gameObject.SetActive(false);
	}

	// Token: 0x06001AFF RID: 6911 RVA: 0x000B83D0 File Offset: 0x000B65D0
	private void RefreshPoisonPageSwitchItem(int index, Refers refers)
	{
		bool flag = !this._mixedPoisonMedicineIdList.CheckIndex(index);
		if (!flag)
		{
			TextMeshProUGUI textComponent = refers.CGet<TextMeshProUGUI>("Label");
			MedicineItem config = Medicine.Instance[this._mixedPoisonMedicineIdList[index]];
			textComponent.text = config.Name;
			refers.CGet<CToggleObsolete>("Toggle").onValueChanged.RemoveAllListeners();
			refers.CGet<CToggleObsolete>("Toggle").onValueChanged.AddListener(delegate(bool isOn)
			{
				this._poisonPageSwitchController.SetSelect(isOn ? index : -1, true);
			});
			TooltipInvoker tip = refers.CGet<TooltipInvoker>("Tip");
			tip.Type = TipType.MixPoison;
			tip.enabled = true;
			tip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("MedicineId", config.TemplateId);
		}
	}

	// Token: 0x06001B00 RID: 6912 RVA: 0x000B84B6 File Offset: 0x000B66B6
	private void SetPoisonPageItemSelectState(Refers refers, bool selectState)
	{
		refers.CGet<CToggleObsolete>("Toggle").isOn = selectState;
		MonoJoint componentInChildren = refers.GetComponentInChildren<MonoJoint>(true);
		if (componentInChildren != null)
		{
			componentInChildren.JointSync();
		}
	}

	// Token: 0x06001B01 RID: 6913 RVA: 0x000B84E0 File Offset: 0x000B66E0
	private void OnSelectPoisonPageIndexChange(int index)
	{
		bool flag = !this._mixedPoisonMedicineIdList.CheckIndex(index);
		if (flag)
		{
			this._targetMixedPoisonMedicineId = -1;
			this._itemViewSlot.SortAndFilter.IsOnAddPoison = false;
		}
		else
		{
			this._targetMixedPoisonMedicineId = this._mixedPoisonMedicineIdList[index];
			this._itemViewSlot.SortAndFilter.IsOnAddPoison = true;
		}
		this.ResetPoisonSlots(this._currentTarget);
		this._itemViewSlot.SortAndFilter.SetTargetMixedPoisonMedicine(this._targetMixedPoisonMedicineId);
		List<ItemDisplayData> itemList = this.SlotCacheItemList;
		this._itemViewSlot.SetItemList(ref itemList, false, null, false, null);
	}

	// Token: 0x06001B02 RID: 6914 RVA: 0x000B857F File Offset: 0x000B677F
	private void ClearSelectedPoisonPage()
	{
		this._poisonPageSwitchController.gameObject.SetActive(false);
		this._targetMixedPoisonMedicineId = -1;
		this._itemViewSlot.SortAndFilter.IsOnAddPoison = false;
	}

	// Token: 0x06001B03 RID: 6915 RVA: 0x000B85AC File Offset: 0x000B67AC
	private void RefreshPoisonSlotView(bool isAdd)
	{
		this.SlotCacheItemList.Clear();
		List<ItemDisplayData> sourceItems = this.GetItemsSource(this._slotTogGroup);
		this.SlotCacheItemList.AddRange(sourceItems.Where(delegate(ItemDisplayData d)
		{
			bool flag = d.Key.ItemType != 8;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !isAdd && d.PoisonIsIdentified && d.HasAnyPoison && this._currentTarget != null && d.Key == this._currentTarget.Key;
				if (flag2)
				{
					result = false;
				}
				else
				{
					MedicineItem config = Medicine.Instance[d.Key.TemplateId];
					EMedicineEffectType type = isAdd ? EMedicineEffectType.ApplyPoison : EMedicineEffectType.DetoxPoison;
					bool flag3 = config.EffectType != type;
					result = !flag3;
				}
			}
			return result;
		}));
		List<ItemDisplayData> itemList = this.SlotCacheItemList;
		this._itemViewSlot.SetItemList(ref itemList, false, null, false, null);
		this._itemViewSlot.SortAndFilter.ShowFilterType(ItemSortAndFilter.ItemFilterType.Medicine);
		this._itemViewSlot.SortAndFilter.LockFilterType(ItemSortAndFilter.ItemFilterType.Medicine, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
	}

	// Token: 0x06001B04 RID: 6916 RVA: 0x000B8644 File Offset: 0x000B6844
	private void CheckPoisonCondition()
	{
		this.HideLifeSkill();
		this._buttonConfirm.interactable = false;
		this.CheckTool(0, 0);
		bool flag = this._currentTarget == null;
		if (flag)
		{
			this.SetConfirmButtonTip(1377, true, TipType.SingleDesc, null);
			this.HidePreviewPoison();
		}
		else
		{
			bool flag2 = this._currentTarget.HasAnyPoison && !this._currentTarget.PoisonIsIdentified;
			if (flag2)
			{
				this.SetConfirmButtonTip(1374, true, TipType.SingleDesc, null);
				this.HidePreviewPoison();
			}
			else
			{
				LifeSkillShorts needLifeSkill = default(LifeSkillShorts);
				LifeSkillShorts showLifeSkill = default(LifeSkillShorts);
				bool haveNotChange = true;
				short durabilityCost = 0;
				for (int i = 0; i < this.SlotCount; i++)
				{
					short oldId = this._originSlotItemArray[i].Key.TemplateId;
					short curId = this._slotItemArray[i].Key.TemplateId;
					bool isSame = oldId == curId;
					bool isAddPoisonCondense = this._isAddPoisonCondense;
					if (isAddPoisonCondense)
					{
						bool condenseIsSame = this._originPoisonEffects.IsCondensed == this._tempPoisonEffects.IsCondensed && this._originPoisonEffects.PoisonSlotList.ContentIsSame(this._tempPoisonEffects.PoisonSlotList);
						isSame = (isSame && condenseIsSame);
					}
					bool flag3 = isSame || curId == -1;
					if (!flag3)
					{
						haveNotChange = false;
						sbyte lifeSkillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(8, curId);
						short requiredAttainment = ItemTemplateHelper.GetPoisonRequiredAttainment(8, curId);
						bool isCondensed = this._tempPoisonEffects.PoisonSlotList.CheckIndex(i) && this._tempPoisonEffects.PoisonSlotList[i].IsCondensed;
						bool isAddPoisonCondense2 = this._isAddPoisonCondense;
						if (isAddPoisonCondense2)
						{
							bool flag4 = isCondensed;
							short condensedAttainment;
							if (flag4)
							{
								condensedAttainment = this._tempPoisonEffects.PoisonSlotList[i].CondensedMedicineTemplateIdList.Max((short m) => ItemTemplateHelper.GetPoisonRequiredAttainment(8, m));
							}
							else
							{
								condensedAttainment = requiredAttainment;
							}
							int bonus = GlobalConfig.Instance.CondensePoisonRequiredAttainmentBonus;
							condensedAttainment = Convert.ToInt16((int)condensedAttainment * (100 + bonus) / 100);
							requiredAttainment = Math.Max(requiredAttainment, condensedAttainment);
						}
						short curAttainment = needLifeSkill.Get((int)lifeSkillType);
						requiredAttainment = this.GetAttainmentByBuildingEffect(lifeSkillType, requiredAttainment);
						needLifeSkill.Set((int)lifeSkillType, Math.Max(curAttainment, requiredAttainment));
						showLifeSkill.Set((int)lifeSkillType, 1);
						sbyte grade = ItemTemplateHelper.GetGrade(8, curId);
						bool flag5 = isCondensed;
						if (flag5)
						{
							sbyte condensedGrade = this._tempPoisonEffects.PoisonSlotList[i].CondensedMedicineTemplateIdList.Max((short m) => ItemTemplateHelper.GetGrade(8, m));
							grade = Math.Max(grade, condensedGrade);
						}
						short cost = this.GetToolDurabilityCost(this._currentTool, grade);
						durabilityCost = Math.Max(durabilityCost, cost);
					}
				}
				this.RefreshPreviewPoison();
				bool flag6 = haveNotChange;
				if (flag6)
				{
					this.SetConfirmButtonTip(1379, true, TipType.SingleDesc, null);
				}
				else
				{
					bool lifeSkillMeet = this.CheckAndShowLifeSkill(needLifeSkill, showLifeSkill);
					bool isAddPoisonCondense3 = this._isAddPoisonCondense;
					if (isAddPoisonCondense3)
					{
						List<PoisonSlot> poisonSlotList = this._tempPoisonEffects.PoisonSlotList;
						bool condenseMeet = poisonSlotList != null && poisonSlotList.All(delegate(PoisonSlot s)
						{
							bool medicineCountIsMax = s.MedicineCountIsMax;
							bool result;
							if (medicineCountIsMax)
							{
								result = true;
							}
							else
							{
								bool flag10 = this._originPoisonEffects.PoisonSlotList != null;
								if (flag10)
								{
									int findIndex = this._originPoisonEffects.PoisonSlotList.FindIndex((PoisonSlot o) => o.IsSameType(s.MedicineTemplateId));
									bool flag11 = findIndex >= 0;
									if (flag11)
									{
										PoisonSlot origin = this._originPoisonEffects.PoisonSlotList[findIndex];
										return origin.CurrentMedicineCount == s.CurrentMedicineCount;
									}
								}
								result = false;
							}
							return result;
						});
						bool flag7 = !condenseMeet;
						if (flag7)
						{
							this.SetConfirmButtonTip(1372, true, TipType.SingleDesc, null);
							return;
						}
					}
					bool toolMeet = this.CheckTool(durabilityCost, durabilityCost);
					bool flag8 = !toolMeet;
					if (!flag8)
					{
						bool flag9 = !lifeSkillMeet;
						if (flag9)
						{
							this.SetConfirmButtonTip(7667, true, TipType.SingleDesc, null);
						}
						else
						{
							this._buttonConfirm.interactable = true;
							this.SetConfirmButtonTip(-1, true, TipType.SingleDesc, null);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001B05 RID: 6917 RVA: 0x000B89E8 File Offset: 0x000B6BE8
	private void ConfirmPoison()
	{
		this._buttonConfirm.interactable = false;
		ItemKey[] slotItemKeys = (from d in this._slotItemArray
		where d != null
		select d.Key).ToArray<ItemKey>();
		BuildingDomainMethod.AsyncCall.CheckAddPoisonCondition(this, this._taiwuCharId, this._currentTool.Key, this._currentTarget.Key, slotItemKeys, this._buildingBlockKey, this._tempPoisonEffects, delegate(int offset, RawDataPool dataPool)
		{
			bool isMeet = false;
			Serializer.Deserialize(dataPool, offset, ref isMeet);
			bool flag = !isMeet;
			if (flag)
			{
				UIElement.FullScreenMask.Hide(false);
			}
			else
			{
				BuildingDomainMethod.AsyncCall.AddItemPoison(this, this._taiwuCharId, this._currentTool, this._currentTarget, this._slotItemArray, this._condensePoisonItemList, delegate(int offset2, RawDataPool dataPool2)
				{
					UI_Make.<>c__DisplayClass301_0 CS$<>8__locals1 = new UI_Make.<>c__DisplayClass301_0();
					CS$<>8__locals1.<>4__this = this;
					CS$<>8__locals1.result = new ValueTuple<bool, ItemDisplayData>(true, this._resultItemDisplayData);
					Serializer.Deserialize(dataPool2, offset2, ref CS$<>8__locals1.result);
					bool isAddPoisonCondense = this._isAddPoisonCondense;
					if (isAddPoisonCondense)
					{
						base.CGet<GameObject>("PerfectEffectConfirm").SetActive(true);
						this.ShowPerfectEffectLoop(false, true);
						SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1.5f, new Action(CS$<>8__locals1.<ConfirmPoison>g__Action|4));
					}
					else
					{
						CS$<>8__locals1.<ConfirmPoison>g__Action|4();
					}
				});
			}
		});
	}

	// Token: 0x06001B06 RID: 6918 RVA: 0x000B8A94 File Offset: 0x000B6C94
	private void InitButtonIdentity()
	{
		CButtonObsolete buttonIdentity = base.CGet<CButtonObsolete>("ButtonIdentity");
		buttonIdentity.OnInteractableChange.RemoveAllListeners();
		GameObject buttonIdentityDisableObj = buttonIdentity.transform.Find("Disable").gameObject;
		buttonIdentity.OnInteractableChange.AddListener(delegate(bool isInteractable)
		{
			buttonIdentityDisableObj.SetActive(!isInteractable);
		});
		Predicate<ItemDisplayData> <>9__4;
		AsyncMethodCallbackDelegate <>9__2;
		buttonIdentity.ClearAndAddListener(delegate
		{
			this.HidePreviewPoison();
			this.ShowUnidentifiedPoisonTip(false, false);
			buttonIdentity.interactable = false;
			UIElement.FullScreenMask.Show();
			IAsyncMethodRequestHandler <>4__this = this;
			int taiwuCharId = this._taiwuCharId;
			ItemDisplayData currentTarget = this._currentTarget;
			AsyncMethodCallbackDelegate callback;
			if ((callback = <>9__2) == null)
			{
				callback = (<>9__2 = delegate(int offset, RawDataPool dataPool)
				{
					List<ItemDisplayData> resultList = null;
					Serializer.Deserialize(dataPool, offset, ref resultList);
					this._curIdentifySuccess = (resultList != null && resultList.Count > 0);
					UI_Make <>4__this2 = this;
					bool curIdentifiedResultHasPoison;
					if (this._curIdentifySuccess)
					{
						curIdentifiedResultHasPoison = resultList.Any((ItemDisplayData d) => d.HasAnyPoison);
					}
					else
					{
						curIdentifiedResultHasPoison = false;
					}
					<>4__this2._curIdentifiedResultHasPoison = curIdentifiedResultHasPoison;
					UI_Make <>4__this3 = this;
					List<ItemDisplayData> resultList2 = resultList;
					ItemDisplayData resultItemDisplayData;
					if (resultList2 == null)
					{
						resultItemDisplayData = null;
					}
					else
					{
						Predicate<ItemDisplayData> match;
						if ((match = <>9__4) == null)
						{
							match = (<>9__4 = ((ItemDisplayData d) => d.RealKey.Id == this._currentTarget.RealKey.Id));
						}
						resultItemDisplayData = resultList2.Find(match);
					}
					<>4__this3._resultItemDisplayData = resultItemDisplayData;
					this._hasIdentifiedResult = true;
					Action <>9__6;
					this.ShowIdentifiedPoisonTip(true, delegate
					{
						YieldHelper instance = SingletonObject.getInstance<YieldHelper>();
						float sec = 1f;
						Action job;
						if ((job = <>9__6) == null)
						{
							job = (<>9__6 = delegate()
							{
								bool curIdentifiedResultHasPoison2 = this._curIdentifiedResultHasPoison;
								if (curIdentifiedResultHasPoison2)
								{
									ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
									argBox.SetObject("ItemList", resultList);
									argBox.Set("ObtainType", 10);
									UIElement.GetItem.SetOnInitArgs(argBox);
									UIManager.Instance.MaskUI(UIElement.GetItem);
								}
								UIElement.FullScreenMask.Hide(false);
								this._hasIdentifiedResult = false;
								this._curIdentifySuccess = false;
								this._curIdentifiedResultHasPoison = false;
								this.RefreshPoisonEffects(this._resultItemDisplayData);
								this.ClearSlot();
								this.RefreshAllItems();
							});
						}
						instance.DelaySecondsDo(sec, job);
					});
				});
			}
			ItemDomainMethod.AsyncCall.IdentifyPoisons(<>4__this, taiwuCharId, currentTarget, callback);
		});
	}

	// Token: 0x06001B07 RID: 6919 RVA: 0x000B8B28 File Offset: 0x000B6D28
	private void RefreshButtonIdentity()
	{
		CButtonObsolete buttonIdentity = base.CGet<CButtonObsolete>("ButtonIdentity");
		buttonIdentity.gameObject.SetActive(this.CurTabIsAboutPoison);
		bool flag = !this.CurTabIsAboutPoison;
		if (!flag)
		{
			TooltipInvoker tip = buttonIdentity.GetComponent<TooltipInvoker>();
			tip.Type = TipType.Simple;
			tip.enabled = true;
			string title = LocalStringManager.Get(LanguageKey.LK_Poison_Identify);
			string content = string.Empty;
			bool isInteractable = true;
			bool flag2 = this._currentTarget == null;
			if (flag2)
			{
				content = LocalStringManager.Get(LanguageKey.LK_Poison_Identify_No_Target);
				isInteractable = false;
			}
			else
			{
				bool poisonIsIdentified = this._currentTarget.PoisonIsIdentified;
				if (poisonIsIdentified)
				{
					content = LocalStringManager.Get(LanguageKey.LK_Poison_Identified);
					isInteractable = false;
				}
				else
				{
					int leftDays = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
					ItemDisplayData data = this._warehouseItems.Find(new Predicate<ItemDisplayData>(UI_Make.<RefreshButtonIdentity>g__Match|303_0));
					int needleAmount = (data != null) ? data.Amount : 0;
					data = this._treasuryItems.Find(new Predicate<ItemDisplayData>(UI_Make.<RefreshButtonIdentity>g__Match|303_0));
					needleAmount += ((data != null) ? data.Amount : 0);
					bool isInDoor = this.IsInDoor;
					if (isInDoor)
					{
						data = this._inventoryItems.Find(new Predicate<ItemDisplayData>(UI_Make.<RefreshButtonIdentity>g__Match|303_0));
						needleAmount += ((data != null) ? data.Amount : 0);
					}
					bool timeIsMeet = leftDays >= 1;
					bool itemIsMeet = needleAmount >= 1;
					bool flag3 = !timeIsMeet || !itemIsMeet;
					if (flag3)
					{
						isInteractable = false;
					}
					string timeStr = leftDays.ToString().SetColor(timeIsMeet ? "brightblue" : "brightred");
					string itemStr = needleAmount.ToString().SetColor(itemIsMeet ? "brightblue" : "brightred");
					content = LocalStringManager.GetFormat(LanguageKey.LK_Poison_Identify_Tip, itemStr, timeStr);
				}
			}
			bool flag4 = tip.PresetParam == null || tip.PresetParam.Length < 2;
			if (flag4)
			{
				tip.PresetParam = new string[2];
			}
			tip.PresetParam[0] = title;
			tip.PresetParam[1] = content;
			buttonIdentity.interactable = isInteractable;
		}
	}

	// Token: 0x06001B08 RID: 6920 RVA: 0x000B8D34 File Offset: 0x000B6F34
	private void RefreshPreviewPoison()
	{
		bool showPreviewPoison = this._tempPoisonEffects.IsThreeMixed && (this._tempPoisonEffects.IsIdentified || this._originPoisonEffects.GetTotalPoisonCount() == 0);
		bool flag = showPreviewPoison;
		if (flag)
		{
			this.ShowPreviewPoison(this._tempPoisonEffects);
		}
		else
		{
			this.HidePreviewPoison();
		}
	}

	// Token: 0x06001B09 RID: 6921 RVA: 0x000B8D90 File Offset: 0x000B6F90
	private void ShowPreviewPoison(FullPoisonEffects poisonEffects)
	{
		GameObject previewPoison = base.CGet<GameObject>("PreviewPoison");
		previewPoison.SetActive(true);
		TooltipInvoker previewPoisonTip = previewPoison.GetComponent<TooltipInvoker>();
		short medicineTemplateId = poisonEffects.GetMixedMedicineTemplateId();
		string medicineName = Medicine.Instance[medicineTemplateId].Name;
		previewPoison.GetComponentInChildren<TextMeshProUGUI>().text = medicineName;
		previewPoisonTip.Type = TipType.MixPoison;
		previewPoisonTip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("MedicineId", medicineTemplateId);
	}

	// Token: 0x06001B0A RID: 6922 RVA: 0x000B8DFD File Offset: 0x000B6FFD
	private void HidePreviewPoison()
	{
		base.CGet<GameObject>("PreviewPoison").SetActive(false);
	}

	// Token: 0x06001B0B RID: 6923 RVA: 0x000B8E14 File Offset: 0x000B7014
	private void CheckRemovePoisonCondition()
	{
		this.HideLifeSkill();
		this.CheckTool(0, 0);
		this._buttonConfirm.interactable = false;
		bool flag = this._currentTarget == null;
		if (flag)
		{
			this.SetConfirmButtonTip(9714, true, TipType.SingleDesc, null);
		}
		else
		{
			LifeSkillShorts needLifeSkill = default(LifeSkillShorts);
			LifeSkillShorts showLifeSkill = default(LifeSkillShorts);
			bool notSelected = true;
			short durabilityCost = 0;
			for (int i = 0; i < this.SlotCount; i++)
			{
				short oldId = this._originSlotItemArray[i].Key.TemplateId;
				short curId = this._slotItemArray[i].Key.TemplateId;
				bool flag2 = oldId != curId;
				if (flag2)
				{
					notSelected = false;
				}
				bool flag3 = !this._originSlotItemArray[i].Key.HasTemplate || !this._slotItemArray[i].Key.HasTemplate;
				if (!flag3)
				{
					bool flag4 = oldId == curId;
					if (!flag4)
					{
						sbyte grade = ItemTemplateHelper.GetGrade(8, curId);
						short cost = this.GetToolDurabilityCost(this._currentTool, grade);
						durabilityCost = Math.Max(durabilityCost, cost);
						sbyte lifeSkillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(8, oldId);
						short requiredAttainment = ItemTemplateHelper.GetPoisonRequiredAttainment(8, oldId);
						bool isRemovePoisonExtract = this._isRemovePoisonExtract;
						if (isRemovePoisonExtract)
						{
							int bonus = GlobalConfig.Instance.CondensePoisonRequiredAttainmentBonus;
							requiredAttainment = Convert.ToInt16((int)requiredAttainment * (100 + bonus) / 100);
						}
						short curAttainment = needLifeSkill.Get((int)lifeSkillType);
						requiredAttainment = this.GetAttainmentByBuildingEffect(lifeSkillType, requiredAttainment);
						needLifeSkill.Set((int)lifeSkillType, Math.Max(curAttainment, requiredAttainment));
						showLifeSkill.Set((int)lifeSkillType, 1);
					}
				}
			}
			this.RefreshPreviewPoison();
			bool flag5 = notSelected;
			if (flag5)
			{
				this.SetConfirmButtonTip(9716, true, TipType.SingleDesc, null);
			}
			else
			{
				bool lifeSkillMeet = this.CheckAndShowLifeSkill(needLifeSkill, showLifeSkill);
				bool toolMeet = this.CheckTool(durabilityCost, durabilityCost);
				bool flag6 = !toolMeet;
				if (!flag6)
				{
					bool flag7 = !lifeSkillMeet;
					if (flag7)
					{
						this.SetConfirmButtonTip(7667, true, TipType.SingleDesc, null);
					}
					else
					{
						this._buttonConfirm.interactable = true;
						this.SetConfirmButtonTip(-1, true, TipType.SingleDesc, null);
					}
				}
			}
		}
	}

	// Token: 0x06001B0C RID: 6924 RVA: 0x000B9038 File Offset: 0x000B7238
	private void ConfirmRemovePoison()
	{
		this._buttonConfirm.interactable = false;
		ItemKey[] slotItemKeys = (from d in this._slotItemArray
		where d != null
		select d.Key).ToArray<ItemKey>();
		BuildingDomainMethod.AsyncCall.CheckRemovePoisonCondition(this, this._taiwuCharId, this._currentTool.Key, this._currentTarget.Key, slotItemKeys, this._buildingBlockKey, this._isRemovePoisonExtract, delegate(int offset, RawDataPool dataPool)
		{
			bool isMeet = false;
			Serializer.Deserialize(dataPool, offset, ref isMeet);
			UIElement.FullScreenMask.Hide(false);
			bool flag = !isMeet;
			if (!flag)
			{
				BuildingDomainMethod.AsyncCall.RemoveItemPoison(this, this._taiwuCharId, this._currentTool, this._currentTarget, this._slotItemArray, this._isRemovePoisonExtract, delegate(int offset2, RawDataPool dataPool2)
				{
					UI_Make.<>c__DisplayClass308_0 CS$<>8__locals1 = new UI_Make.<>c__DisplayClass308_0();
					CS$<>8__locals1.<>4__this = this;
					CS$<>8__locals1.result = new ValueTuple<bool, List<ItemDisplayData>>(false, new List<ItemDisplayData>());
					Serializer.Deserialize(dataPool2, offset2, ref CS$<>8__locals1.result);
					bool isRemovePoisonExtract = this._isRemovePoisonExtract;
					if (isRemovePoisonExtract)
					{
						base.CGet<GameObject>("PerfectEffectConfirm").SetActive(true);
						this.ShowPerfectEffectLoop(false, true);
						SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1.5f, new Action(CS$<>8__locals1.<ConfirmRemovePoison>g__Action|4));
					}
					else
					{
						CS$<>8__locals1.<ConfirmRemovePoison>g__Action|4();
					}
				});
			}
		});
	}

	// Token: 0x06001B0D RID: 6925 RVA: 0x000B90E4 File Offset: 0x000B72E4
	public static ArgumentBox GetMakeBuildingInfo(BuildingBlockData blockData, BuildingBlockKey buildingBlockKey)
	{
		short templateId = blockData.TemplateId;
		bool isBambooHouse = templateId == 257 || templateId == 258;
		BuildingBlockItem configData = BuildingBlock.Instance[blockData.TemplateId];
		HashSet<UI_Make.UIMakeTab> allTabSet = new HashSet<UI_Make.UIMakeTab>();
		bool canMakeItem = configData.CanMakeItem;
		if (canMakeItem)
		{
			allTabSet.Add(UI_Make.UIMakeTab.Make);
		}
		bool flag = configData.RequireLifeSkillType == 9;
		if (flag)
		{
			allTabSet.Add(UI_Make.UIMakeTab.Poison);
			allTabSet.Add(UI_Make.UIMakeTab.RemovePoison);
		}
		sbyte requireLifeSkillType = configData.RequireLifeSkillType;
		bool flag2 = requireLifeSkillType == 6 || requireLifeSkillType == 7 || requireLifeSkillType == 10 || requireLifeSkillType == 11 || isBambooHouse;
		if (flag2)
		{
			allTabSet.Add(UI_Make.UIMakeTab.Refine);
		}
		requireLifeSkillType = configData.RequireLifeSkillType;
		bool flag3 = requireLifeSkillType == 6 || requireLifeSkillType == 7 || requireLifeSkillType == 10 || requireLifeSkillType == 11 || requireLifeSkillType == 8 || requireLifeSkillType == 9 || isBambooHouse;
		if (flag3)
		{
			allTabSet.Add(UI_Make.UIMakeTab.Repair);
		}
		bool flag4 = configData.RequireLifeSkillType == 10;
		if (flag4)
		{
			allTabSet.Add(UI_Make.UIMakeTab.Weave);
		}
		Location villageLocation = SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock();
		bool isTaiwuVillageBuilding = buildingBlockKey.AreaId == villageLocation.AreaId && buildingBlockKey.BlockId == villageLocation.BlockId;
		bool flag5 = configData.ArtisanOrderAvailable && isTaiwuVillageBuilding;
		if (flag5)
		{
			allTabSet.Add(UI_Make.UIMakeTab.CraftsmanPanel);
		}
		sbyte lifeSkillType = isBambooHouse ? 7 : configData.RequireLifeSkillType;
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
		argumentBox.SetObject("AllTab", allTabSet);
		argumentBox.SetObject("BuildingBlockData", blockData);
		argumentBox.SetObject("BuildingBlockKey", buildingBlockKey);
		argumentBox.Set("LifeSkillType", lifeSkillType);
		return argumentBox;
	}

	// Token: 0x06001B0E RID: 6926 RVA: 0x000B929A File Offset: 0x000B749A
	private void StopInverseParticleCoroutine()
	{
		base.StopCoroutine("InverseParticle");
	}

	// Token: 0x06001B0F RID: 6927 RVA: 0x000B92A9 File Offset: 0x000B74A9
	private void StartInverseParticleCoroutine(ParticleSystem particleSystems)
	{
		base.StartCoroutine(this.InverseParticle(particleSystems));
	}

	// Token: 0x06001B10 RID: 6928 RVA: 0x000B92BA File Offset: 0x000B74BA
	private IEnumerator InverseParticle(ParticleSystem particleSystems)
	{
		WaitForEndOfFrame wait = new WaitForEndOfFrame();
		float currentSimulationTime = particleSystems.time;
		bool useAutoRandomSeed = particleSystems.useAutoRandomSeed;
		particleSystems.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		particleSystems.useAutoRandomSeed = false;
		particleSystems.Play();
		float deltaTime = particleSystems.main.useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
		while (currentSimulationTime >= 0f)
		{
			currentSimulationTime -= deltaTime * particleSystems.main.simulationSpeed;
			particleSystems.Simulate(currentSimulationTime, true, true, true);
			yield return wait;
		}
		particleSystems.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		particleSystems.useAutoRandomSeed = useAutoRandomSeed;
		yield return null;
		yield break;
	}

	// Token: 0x06001B11 RID: 6929 RVA: 0x000B92D0 File Offset: 0x000B74D0
	private void ShowPerfectEffectLoop(bool show, bool hasAnim)
	{
		CImage perfectEffectLoopImage = base.CGet<CImage>("PerfectEffectLoopImage");
		GameObject perfectEffectLoop = base.CGet<GameObject>("PerfectEffectLoop");
		CanvasGroup canvasGroup = perfectEffectLoop.GetComponent<CanvasGroup>();
		canvasGroup.DOKill(false);
		perfectEffectLoopImage.DOKill(false);
		if (hasAnim)
		{
			int to = show ? 1 : 0;
			int from = show ? 0 : 1;
			perfectEffectLoopImage.DOFade((float)to, 0.5f).From((float)from, true, false).OnComplete(delegate
			{
				perfectEffectLoopImage.gameObject.SetActive(show);
			});
			canvasGroup.DOFade((float)to, 0.5f).From((float)from, true, false).OnComplete(delegate
			{
				perfectEffectLoop.gameObject.SetActive(show);
			});
			perfectEffectLoopImage.gameObject.SetActive(true);
			perfectEffectLoop.gameObject.SetActive(true);
		}
		else
		{
			perfectEffectLoopImage.gameObject.SetActive(show);
			perfectEffectLoop.gameObject.SetActive(show);
		}
	}

	// Token: 0x06001B12 RID: 6930 RVA: 0x000B9400 File Offset: 0x000B7600
	private void RefreshWeaveSlotView()
	{
		this._clothingWeaveTargetList.Clear();
		ItemDisplayData sameData = null;
		foreach (KeyValuePair<short, VoidValue> pair in this._ownedClothingSet)
		{
			short itemTemplateId = pair.Key;
			ItemKey itemKey = new ItemKey(3, 0, itemTemplateId, -1);
			ItemDisplayData data = new ItemDisplayData
			{
				Key = itemKey,
				Amount = 0
			};
			this._clothingWeaveTargetList.Add(data);
			bool flag = this._currentTarget != null;
			if (flag)
			{
				bool isSame = !this._currentTarget.IsWeaved && itemTemplateId == this._currentTarget.Key.TemplateId;
				bool flag2 = !isSame;
				if (flag2)
				{
					isSame = (this._currentTarget.IsWeaved && itemTemplateId == this._currentTarget.WeavedClothingTemplateId);
				}
				bool flag3 = isSame;
				if (flag3)
				{
					sameData = data;
				}
			}
		}
		List<ItemDisplayData> itemList = this._clothingWeaveTargetList;
		this._itemViewSlot.SortAndFilter.StaticAheadItemKeysList.Clear();
		bool flag4 = sameData != null;
		if (flag4)
		{
			this._itemViewSlot.SortAndFilter.StaticAheadItemKeysList.Add(sameData.Key);
		}
		this._itemViewSlot.SetItemList(ref itemList, false, null, false, null);
	}

	// Token: 0x06001B13 RID: 6931 RVA: 0x000B9564 File Offset: 0x000B7764
	private void CheckWeaveCondition()
	{
		this._buttonConfirm.interactable = false;
		this._previewTip.enabled = false;
		this.ShowResultPreviewImage(false);
		this.PlayCenterAnim(false, false, false);
		bool flag = this._currentTarget == null;
		if (flag)
		{
			this.SetConfirmButtonTip(7698, true, TipType.SingleDesc, null);
		}
		else
		{
			LifeSkillShorts needLifeSkill = default(LifeSkillShorts);
			LifeSkillShorts showLifeSkill = default(LifeSkillShorts);
			needLifeSkill.Set(10, 0);
			showLifeSkill.Set(10, 1);
			bool haveNotChange = true;
			short durabilityCost = 0;
			for (int i = 0; i < this.SlotCount; i++)
			{
				short oldId = this._originSlotItemArray[i].Key.TemplateId;
				short curId = this._slotItemArray[i].Key.TemplateId;
				bool isSame = oldId == curId;
				bool flag2 = !isSame;
				if (flag2)
				{
					haveNotChange = false;
				}
				bool flag3 = curId != -1 || oldId != -1;
				if (flag3)
				{
					short id = curId;
					bool flag4 = curId == -1;
					if (flag4)
					{
						id = oldId;
					}
					ClothingItem config = Clothing.Instance[id];
					short lastAttainment = needLifeSkill.Get(10);
					short requiredAttainment = this.GetAttainmentByBuildingEffect(10, config.WeaveNeedAttainment);
					needLifeSkill.Set(10, Math.Max(lastAttainment, requiredAttainment));
					showLifeSkill.Set(10, 1);
					bool flag5 = !isSame;
					if (flag5)
					{
						short cost = this.GetToolDurabilityCost(this._currentTool, config.Grade);
						durabilityCost = Math.Max(durabilityCost, cost);
					}
				}
			}
			bool lifeSkillMeet = this.CheckAndShowLifeSkill(needLifeSkill, showLifeSkill);
			bool flag6 = haveNotChange;
			if (flag6)
			{
				this.SetConfirmButtonTip(7706, true, TipType.SingleDesc, null);
			}
			else
			{
				this.ShowResultPreviewImage(true);
				this.PlayCenterAnim(false, false, false);
				this._previewTip.enabled = true;
				this._previewTip.Type = TipType.Clothing;
				ItemDisplayData itemData = this._currentTarget.Clone(-1);
				itemData.WeavedClothingTemplateId = this._slotItemArray.First<ItemDisplayData>().Key.TemplateId;
				this._previewTip.RuntimeParam = new ArgumentBox().SetObject("ItemData", itemData).Set("CharId", this._taiwuCharId);
				bool toolMeet = this.CheckTool(durabilityCost, durabilityCost);
				bool flag7 = !toolMeet;
				if (!flag7)
				{
					bool flag8 = !lifeSkillMeet;
					if (flag8)
					{
						this.SetConfirmButtonTip(7667, true, TipType.SingleDesc, null);
					}
					else
					{
						this._buttonConfirm.interactable = true;
						this.SetConfirmButtonTip(-1, true, TipType.SingleDesc, null);
					}
				}
			}
		}
	}

	// Token: 0x06001B14 RID: 6932 RVA: 0x000B97E8 File Offset: 0x000B79E8
	private void ConfirmWeave()
	{
		this._buttonConfirm.interactable = false;
		short targetId = this._slotItemArray.First<ItemDisplayData>().Key.TemplateId;
		BuildingDomainMethod.AsyncCall.WeaveClothingItem(this, this._currentTool, this._currentTarget, targetId, delegate(int offset, RawDataPool dataPool)
		{
			Serializer.Deserialize(dataPool, offset, ref this._resultItemDisplayData);
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("ItemList", new List<ItemDisplayData>
			{
				this._resultItemDisplayData
			});
			UIElement.GetItem.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.GetItem);
			for (int i = 0; i < this.SlotCount; i++)
			{
				this._originSlotItemArray[i] = this._slotItemArray[i];
			}
			this.RefreshAllItems();
			this._currentTarget.WeavedClothingTemplateId = this._resultItemDisplayData.WeavedClothingTemplateId;
			this.RefreshWeaveClothingView();
		});
	}

	// Token: 0x06001B15 RID: 6933 RVA: 0x000B983C File Offset: 0x000B7A3C
	private void RefreshWeaveClothingView()
	{
		UI_Make.<>c__DisplayClass317_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.currentClothingView = base.CGet<Refers>("CurrentClothingView");
		Refers weaveClothingViw = base.CGet<Refers>("WeaveClothingView");
		bool isWeaveTab = this._curTab == UI_Make.UIMakeTab.Weave;
		CS$<>8__locals1.currentClothingView.gameObject.SetActive(isWeaveTab);
		weaveClothingViw.gameObject.SetActive(isWeaveTab);
		bool flag = isWeaveTab && this._characterDisplayData != null;
		if (flag)
		{
			weaveClothingViw.transform.SetAsLastSibling();
			bool flag2 = this._currentTarget != null;
			if (flag2)
			{
				ClothingItem targetConfig = Clothing.Instance[this._currentTarget.WeavedClothingTemplateId];
				AvatarData previewData = new AvatarData(this._characterDisplayData.AvatarRelatedData.AvatarData);
				previewData.ClothDisplayId = targetConfig.DisplayId;
				this.<RefreshWeaveClothingView>g__Refresh|317_0(CS$<>8__locals1.currentClothingView, true, previewData, ref CS$<>8__locals1);
			}
			else
			{
				this.<RefreshWeaveClothingView>g__Refresh|317_0(CS$<>8__locals1.currentClothingView, false, null, ref CS$<>8__locals1);
			}
			ItemDisplayData slotItemData = this._slotItemArray.First<ItemDisplayData>();
			bool flag3 = slotItemData != null && slotItemData.Key.TemplateId >= 0 && (this._currentTarget == null || slotItemData.Key.TemplateId != this._currentTarget.WeavedClothingTemplateId);
			if (flag3)
			{
				ClothingItem slotConfig = Clothing.Instance[slotItemData.Key.TemplateId];
				this.<RefreshWeaveClothingView>g__Refresh|317_0(weaveClothingViw, true, new AvatarData(this._characterDisplayData.AvatarRelatedData.AvatarData)
				{
					ClothDisplayId = slotConfig.DisplayId
				}, ref CS$<>8__locals1);
			}
			else
			{
				this.<RefreshWeaveClothingView>g__Refresh|317_0(weaveClothingViw, false, null, ref CS$<>8__locals1);
			}
		}
	}

	// Token: 0x06001B16 RID: 6934 RVA: 0x000B99DF File Offset: 0x000B7BDF
	private void InitWeave()
	{
		this.InitWeaveClothingView(base.CGet<Refers>("CurrentClothingView"), true);
		this.InitWeaveClothingView(base.CGet<Refers>("WeaveClothingView"), false);
	}

	// Token: 0x06001B17 RID: 6935 RVA: 0x000B9A08 File Offset: 0x000B7C08
	private void InitWeaveSprite()
	{
		for (int i = 0; i < this._weaveHeadSprites.Length; i++)
		{
			int index = i + 1;
			this._weaveHeadSprites[index - 1] = ResLoader.SyncLoad<Sprite>(string.Format("RemakeResources/UIGraphics4.0/Building_UI/building_alterations_hand_{0}", index));
		}
	}

	// Token: 0x06001B18 RID: 6936 RVA: 0x000B9A54 File Offset: 0x000B7C54
	private void InitWeaveClothingView(Refers clothingView, bool isOrigin)
	{
		UI_Make.<>c__DisplayClass320_0 CS$<>8__locals1 = new UI_Make.<>c__DisplayClass320_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.clothingView = clothingView;
		byte gender = isOrigin ? this._weaveClothingDisplaySetting.ClothingDisplayOriginSettingGender : this._weaveClothingDisplaySetting.ClothingDisplayPreviewSettingGender;
		CS$<>8__locals1.genderToggle = CS$<>8__locals1.clothingView.CGet<CToggleObsolete>("GenderToggle");
		CS$<>8__locals1.genderToggle.isOn = (gender == 1);
		CS$<>8__locals1.<InitWeaveClothingView>g__RefreshToggleGroupIcon|2();
		bool flag = CS$<>8__locals1.genderToggle.onValueChanged.GetPersistentEventCount() == 2;
		if (flag)
		{
			CS$<>8__locals1.genderToggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				CS$<>8__locals1.<>4__this.RefreshWeaveClothingView();
				base.<InitWeaveClothingView>g__RefreshToggleGroupIcon|2();
			});
		}
		byte bodyType = isOrigin ? this._weaveClothingDisplaySetting.ClothingDisplayOriginSettingBodyType : this._weaveClothingDisplaySetting.ClothingDisplayPreviewSettingBodyType;
		CToggleGroupObsolete bodyToggleGroup = CS$<>8__locals1.clothingView.CGet<CToggleGroupObsolete>("BodyToggleGroup");
		bodyToggleGroup.InitPreOnToggle((int)bodyType);
		bodyToggleGroup.OnActiveToggleChange = delegate(CToggleObsolete newTog, CToggleObsolete oldTog)
		{
			CS$<>8__locals1.<>4__this.RefreshWeaveClothingView();
		};
	}

	// Token: 0x06001B19 RID: 6937 RVA: 0x000B9B40 File Offset: 0x000B7D40
	private void InitPoisonSpecialInteraction()
	{
		base.CGet<GameObject>("PoisonPerfectEffectSelect").SetActive(false);
		this.ShowPerfectEffectLoop(false, false);
		DG.Tweening.Sequence perfectEffectSelectSequence = this._perfectEffectSelectSequence;
		if (perfectEffectSelectSequence != null)
		{
			perfectEffectSelectSequence.Pause<DG.Tweening.Sequence>();
		}
		this.StopInverseParticleCoroutine();
		FullPoisonEffects fullPoisonEffects = this._tempPoisonEffects;
		if (fullPoisonEffects.PoisonSlotList == null)
		{
			fullPoisonEffects.PoisonSlotList = new List<PoisonSlot>();
		}
		this._tempPoisonEffects.Clear();
		fullPoisonEffects = this._originPoisonEffects;
		if (fullPoisonEffects.PoisonSlotList == null)
		{
			fullPoisonEffects.PoisonSlotList = new List<PoisonSlot>();
		}
		this._originPoisonEffects.Clear();
		this._condensePoisonItemList.Clear();
		this._isAddPoisonCondense = false;
		this._isRemovePoisonExtract = false;
		CButtonObsolete buttonCondense = base.CGet<Refers>("Poison").CGet<CButtonObsolete>("ButtonCondense");
		buttonCondense.ClearAndAddListener(delegate
		{
			bool isAddPoisonCondense = this._isAddPoisonCondense;
			if (isAddPoisonCondense)
			{
				this.ClearCondense();
			}
			this._isAddPoisonCondense = !this._isAddPoisonCondense;
			this.RefreshButtonCondense();
			this.RefreshSlots();
			this.CheckCondition(false);
			this.PlayCondenseEffect();
		});
		this.RefreshButtonCondense();
		CButtonObsolete buttonExtract = base.CGet<Refers>("RemovePoison").CGet<CButtonObsolete>("ButtonExtract");
		buttonExtract.ClearAndAddListener(delegate
		{
			this._isRemovePoisonExtract = !this._isRemovePoisonExtract;
			this.RefreshButtonExtract();
			this.CheckCondition(false);
			this.PlayExtractEffect();
		});
		this.RefreshButtonExtract();
	}

	// Token: 0x06001B1A RID: 6938 RVA: 0x000B9C48 File Offset: 0x000B7E48
	private void ClearCondense()
	{
		bool flag = this._originPoisonEffects.SameOf(this._tempPoisonEffects);
		if (!flag)
		{
			for (int i = 0; i < this._tempPoisonEffects.PoisonSlotList.Count; i++)
			{
				for (int j = 0; j < PoisonSlot.MaxMedicineCount; j++)
				{
					this.RemoveCondense(i);
				}
			}
			this._tempPoisonEffects.Assign(this._originPoisonEffects);
			this._condensePoisonItemList.Clear();
		}
	}

	// Token: 0x06001B1B RID: 6939 RVA: 0x000B9CC8 File Offset: 0x000B7EC8
	private void RefreshButtonCondense()
	{
		CButtonObsolete buttonCondense = base.CGet<Refers>("Poison").CGet<CButtonObsolete>("ButtonCondense");
		Transform selectedObj = buttonCondense.transform.Find("Select");
		selectedObj.gameObject.SetActive(this._isAddPoisonCondense);
	}

	// Token: 0x06001B1C RID: 6940 RVA: 0x000B9D10 File Offset: 0x000B7F10
	private void RefreshButtonExtract()
	{
		CButtonObsolete buttonExtract = base.CGet<Refers>("RemovePoison").CGet<CButtonObsolete>("ButtonExtract");
		Transform selectedObj = buttonExtract.transform.Find("Select");
		selectedObj.gameObject.SetActive(this._isRemovePoisonExtract);
	}

	// Token: 0x06001B1D RID: 6941 RVA: 0x000B9D58 File Offset: 0x000B7F58
	private void PlayCondenseEffect()
	{
		GameObject perfectEffectSelect = base.CGet<GameObject>("PoisonPerfectEffectSelect");
		ParticleSystem particle = perfectEffectSelect.GetComponentInChildren<ParticleSystem>();
		bool isAddPoisonCondense = this._isAddPoisonCondense;
		if (isAddPoisonCondense)
		{
			perfectEffectSelect.SetActive(true);
			this.StopInverseParticleCoroutine();
			particle.Play();
			this._perfectEffectSelectSequence = DOTween.Sequence();
			this._perfectEffectSelectSequence.AppendInterval(1f);
			this._perfectEffectSelectSequence.AppendCallback(delegate
			{
				this.ShowPerfectEffectLoop(true, true);
			});
			this._perfectEffectSelectSequence.Play<DG.Tweening.Sequence>();
		}
		else
		{
			DG.Tweening.Sequence perfectEffectSelectSequence = this._perfectEffectSelectSequence;
			if (perfectEffectSelectSequence != null)
			{
				perfectEffectSelectSequence.Pause<DG.Tweening.Sequence>();
			}
			this.StartInverseParticleCoroutine(particle);
			DG.Tweening.Sequence perfectEffectSelectSequence2 = this._perfectEffectSelectSequence;
			bool hasShow = perfectEffectSelectSequence2 != null && perfectEffectSelectSequence2.IsComplete();
			this.ShowPerfectEffectLoop(false, hasShow);
		}
	}

	// Token: 0x06001B1E RID: 6942 RVA: 0x000B9E18 File Offset: 0x000B8018
	private void PlayExtractEffect()
	{
		GameObject perfectEffectSelect = base.CGet<GameObject>("PoisonPerfectEffectSelect");
		ParticleSystem particle = perfectEffectSelect.GetComponentInChildren<ParticleSystem>();
		bool isRemovePoisonExtract = this._isRemovePoisonExtract;
		if (isRemovePoisonExtract)
		{
			perfectEffectSelect.SetActive(true);
			this.StopInverseParticleCoroutine();
			particle.Play();
			this._perfectEffectSelectSequence = DOTween.Sequence();
			this._perfectEffectSelectSequence.AppendInterval(1f);
			this._perfectEffectSelectSequence.AppendCallback(delegate
			{
				this.ShowPerfectEffectLoop(true, true);
			});
			this._perfectEffectSelectSequence.Play<DG.Tweening.Sequence>();
		}
		else
		{
			DG.Tweening.Sequence perfectEffectSelectSequence = this._perfectEffectSelectSequence;
			if (perfectEffectSelectSequence != null)
			{
				perfectEffectSelectSequence.Pause<DG.Tweening.Sequence>();
			}
			this.StartInverseParticleCoroutine(particle);
			DG.Tweening.Sequence perfectEffectSelectSequence2 = this._perfectEffectSelectSequence;
			bool hasShow = perfectEffectSelectSequence2 != null && perfectEffectSelectSequence2.IsComplete();
			this.ShowPerfectEffectLoop(false, hasShow);
		}
	}

	// Token: 0x06001B1F RID: 6943 RVA: 0x000B9ED8 File Offset: 0x000B80D8
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

	// Token: 0x170002CB RID: 715
	// (get) Token: 0x06001B20 RID: 6944 RVA: 0x000B9F49 File Offset: 0x000B8149
	private sbyte LiftSkillType
	{
		get
		{
			return UI_CraftsmanPanel.GetOperationNeedSkillType(this._currentCraftType);
		}
	}

	// Token: 0x170002CC RID: 716
	// (get) Token: 0x06001B21 RID: 6945 RVA: 0x000B9F58 File Offset: 0x000B8158
	private short CurItemSubType
	{
		get
		{
			ECraftType currentCraftType = this._currentCraftType;
			if (!true)
			{
			}
			short result;
			if (currentCraftType != ECraftType.Tea)
			{
				if (currentCraftType != ECraftType.Wine)
				{
					ArtisanOrder artisanOrder = this._artisanOrder;
					result = ((artisanOrder != null) ? artisanOrder.ItemSubType : -1);
				}
				else
				{
					result = 901;
				}
			}
			else
			{
				result = 900;
			}
			if (!true)
			{
			}
			return result;
		}
	}

	// Token: 0x170002CD RID: 717
	// (get) Token: 0x06001B22 RID: 6946 RVA: 0x000B9FA6 File Offset: 0x000B81A6
	private BuildingBlockItem BuildingConfigData
	{
		get
		{
			return BuildingBlock.Instance[this._blockData.TemplateId];
		}
	}

	// Token: 0x170002CE RID: 718
	// (get) Token: 0x06001B23 RID: 6947 RVA: 0x000B9FBD File Offset: 0x000B81BD
	private int ResourceCount
	{
		get
		{
			return SingletonObject.getInstance<BuildingModel>().GetResourceCount(this._productResourceType);
		}
	}

	// Token: 0x170002CF RID: 719
	// (get) Token: 0x06001B24 RID: 6948 RVA: 0x000B9FCF File Offset: 0x000B81CF
	private short SettlementId
	{
		get
		{
			return SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageSettlementId();
		}
	}

	// Token: 0x170002D0 RID: 720
	// (get) Token: 0x06001B25 RID: 6949 RVA: 0x000B9FDB File Offset: 0x000B81DB
	private List<int> ShopManagerList
	{
		get
		{
			return SingletonObject.getInstance<BuildingModel>().GetBuildingShopManager(this._buildingBlockKey);
		}
	}

	// Token: 0x170002D1 RID: 721
	// (get) Token: 0x06001B26 RID: 6950 RVA: 0x000B9FED File Offset: 0x000B81ED
	private BuildingOptionAutoGiveMemberPreset CurrArrangementSettingPresetData
	{
		get
		{
			return this._buildingModel.GetBuildingArrangementSettingPresetData(0);
		}
	}

	// Token: 0x170002D2 RID: 722
	// (get) Token: 0x06001B27 RID: 6951 RVA: 0x000B9FFB File Offset: 0x000B81FB
	private bool CurrInfluenceLeader
	{
		get
		{
			return this.CurrArrangementSettingPresetData.GetIsInfluenceLeader();
		}
	}

	// Token: 0x170002D3 RID: 723
	// (get) Token: 0x06001B28 RID: 6952 RVA: 0x000BA008 File Offset: 0x000B8208
	private bool CurrInfluenceMember
	{
		get
		{
			return this.CurrArrangementSettingPresetData.GetIsInfluenceMember();
		}
	}

	// Token: 0x06001B29 RID: 6953 RVA: 0x000BA018 File Offset: 0x000B8218
	public void InitMakeCraftsManPanel()
	{
		this._craftsRootPanel = base.CGet<Refers>("CraftsRootPanel");
		this._craftsManExtraOperatePanel = this._craftsRootPanel.CGet<CraftsManExtraOperatePanel>("CraftsManExtraOperatePanel");
		this._craftsManExtraOperatePanel.Init(this, new Action(this.OnClickBtnProductType), new Action(this.OnClickBtnAddAttainments), new Action<int>(this.OnChangeStock), (int)this._buildingBlockKey.BuildingBlockIndex, new Action(this.AddResource), new Action<Transform>(this.OnEnterFocusMode));
		this._villagerCraftPreviewPanel = this._craftsRootPanel.CGet<VillagerCraftPreviewPanel>("VillagerCraftPreviewPanel");
		this._buildingCraftsmanPanel = base.CGet<Refers>("BuildingCraftsmanPanel");
		this._managerLeaderViewRefer = this._buildingCraftsmanPanel.CGet<ResidentView>("ManagerLeaderViewBuildingRefer");
		this._managerLeaderViewRefer.CGet<Refers>("MatchVillagerRole").transform.SetParent(this._managerLeaderViewRefer.CGet<GameObject>("CharInfoHolder").transform, true);
		this._currentVillagerList = new List<VillagerRoleCharacterDisplayData>[VillagerRole.Instance.Count];
		this._lostVillagerList = new List<VillagerRoleCharacterDisplayData>[VillagerRole.Instance.Count];
		CButtonObsolete btnAutoAssign = this._buildingCraftsmanPanel.CGet<CButtonObsolete>("BtnAutoAssign");
		btnAutoAssign.ClearAndAddListener(delegate
		{
			BuildingDomainMethod.Call.QuickArrangeShopManager(this.Element.GameDataListenerId, this._buildingBlockKey);
			base.StartCoroutine(this.DisableShopQuickSelectButtonForAWhile());
		});
		CButtonObsolete btnAutoClear = this._buildingCraftsmanPanel.CGet<CButtonObsolete>("BtnAutoClear");
		btnAutoClear.ClearAndAddListener(delegate
		{
			this.UnSelectShopManager();
			base.StartCoroutine(this.DisableShopQuickSelectButtonForAWhile());
		});
		this.RefreshAllQuickSelectButtons();
		this.InitCraftTip();
	}

	// Token: 0x06001B2A RID: 6954 RVA: 0x000BA190 File Offset: 0x000B8390
	private void OnEnterFocusMode(Transform transform)
	{
		Action onClose = new Action(this.OnAddCraftResourceClose);
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("CallTriggerListner", true).SetObject("FocusTransform", transform).SetObject("ArtisanOrder", this._artisanOrder).SetObject("OnClose", onClose).Set("ResourceType", this._productResourceType).Set("BuildingCraftPanel", true);
		UIElement.AddCraftResource.SetOnInitArgs(argumentBox);
		UIManager.Instance.ShowUI(UIElement.AddCraftResource, true);
	}

	// Token: 0x06001B2B RID: 6955 RVA: 0x000BA21A File Offset: 0x000B841A
	private void OnAddCraftResourceClose()
	{
		this._craftsManExtraOperatePanel.CloseFocusModeUI();
		this.RefreshMakeCraftsManPanelData();
	}

	// Token: 0x06001B2C RID: 6956 RVA: 0x000BA230 File Offset: 0x000B8430
	public void SetShowCraftsManPanel(bool show)
	{
		ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
		argsBox.Set("IsOpenPanel", show);
		argsBox.Set("BuildingBlockTemplateId", this._blockData.TemplateId);
		GEvent.OnEvent(UiEvents.OpenBuildingCraftsmanPanel, argsBox);
		this._craftsRootPanel.gameObject.SetActive(show);
	}

	// Token: 0x06001B2D RID: 6957 RVA: 0x000BA288 File Offset: 0x000B8488
	public void RefreshMakeCraftsManPanelData()
	{
		this._productionPool = null;
		this._artisanOrder = null;
		this._currentCraftType = CommonUtils.GetCraftTypesByBuildingBlockId(this._blockData.TemplateId);
		this._productResourceType = UI_CraftsmanPanel.GetResourceTypeByCraftType(this._currentCraftType);
		this.NeedDataListenerId = true;
		ExtraDomainMethod.AsyncCall.GetBuildingArtisanOrderAfterUpdate(this, this._buildingBlockKey, delegate(int offset, RawDataPool pool)
		{
			Serializer.Deserialize(pool, offset, ref this._artisanOrder);
			bool flag = this._artisanOrder != null;
			if (flag)
			{
				this._craftsManExtraOperatePanel.storeToDropdown.ForceRefreshStorageType(this._artisanOrder.StorageType);
			}
			this.<RefreshMakeCraftsManPanelData>g__GetProductionPool|372_1();
		});
	}

	// Token: 0x06001B2E RID: 6958 RVA: 0x000BA2F0 File Offset: 0x000B84F0
	private void ProcessData()
	{
		this.InitCraftTip();
		this.UpdateShopManagersNew();
		this._craftsManExtraOperatePanel.SetProductType(this.CurItemSubType);
		this._craftsManExtraOperatePanel.Refresh(this._artisanOrder, (short)this._curLifeSkillType, this.ResourceCount, EPanelMode.Building, this._currentCraftType, this._isLeaderRoleMatch, this._shopManagerListCached[0]);
		this._villagerCraftPreviewPanel.SetProductionPool(this._productionPool, null, this.CurItemSubType);
	}

	// Token: 0x06001B2F RID: 6959 RVA: 0x000BA36A File Offset: 0x000B856A
	public void SelectShopManager(int charId)
	{
		BuildingDomainMethod.Call.SetShopManager(this._buildingBlockKey, (sbyte)this._selectingShopManagerIndex, charId);
	}

	// Token: 0x06001B30 RID: 6960 RVA: 0x000BA384 File Offset: 0x000B8584
	private void RefreshSelectCharacterBtn(int i, Refers childRefer, byte charPrefabType, sbyte lifeSkillType)
	{
		UI_Make.<>c__DisplayClass375_0 CS$<>8__locals1 = new UI_Make.<>c__DisplayClass375_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.index = i;
		CS$<>8__locals1.charIdList = (from id in this._availableWorker
		where !CS$<>8__locals1.<>4__this._shopManagerListCached.Contains(id)
		select id).ToList<int>();
		CS$<>8__locals1.skillValueDict = new Dictionary<int, short>();
		for (int j = 0; j < CS$<>8__locals1.charIdList.Count; j++)
		{
			bool flag = CS$<>8__locals1.charIdList[j] < 0;
			if (!flag)
			{
				int charId = CS$<>8__locals1.charIdList[j];
				AsyncMethodCallbackDelegate callback;
				if ((callback = CS$<>8__locals1.<>9__2) == null)
				{
					callback = (CS$<>8__locals1.<>9__2 = delegate(int offset, RawDataPool dataPool)
					{
						ValueTuple<int, int> skillValue = new ValueTuple<int, int>(-1, -1);
						Serializer.Deserialize(dataPool, offset, ref skillValue);
						bool flag2 = skillValue.Item1 > -1;
						if (flag2)
						{
							CS$<>8__locals1.skillValueDict.Add(skillValue.Item1, (short)skillValue.Item2);
						}
					});
				}
				CharacterDomainMethod.AsyncCall.GetLifeSkillAttainment(this, charId, lifeSkillType, callback);
			}
		}
		childRefer.CGet<CButtonObsolete>("SelectCharBtn").ClearAndAddListener(new Action(CS$<>8__locals1.<RefreshSelectCharacterBtn>g__OpenSelectChar|1));
		childRefer.CGet<CButtonObsolete>("ChangeButton").ClearAndAddListener(new Action(CS$<>8__locals1.<RefreshSelectCharacterBtn>g__OpenSelectChar|1));
	}

	// Token: 0x06001B31 RID: 6961 RVA: 0x000BA478 File Offset: 0x000B8678
	private void ShowSelectCharWithFilter(List<int> charIdList, Action<int> onSelect)
	{
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
		argumentBox.Clear();
		argumentBox.SetObject("callback", onSelect);
		argumentBox.SetObject("charIdList", charIdList);
		argumentBox.SetObject("filterType", new List<global::CharacterTable.CharacterTableCommonFilterTypes>
		{
			global::CharacterTable.CharacterTableCommonFilterTypes.Resident
		});
		argumentBox.SetObject("usingPages", new List<ECharacterTableType>
		{
			ECharacterTableType.Villager,
			ECharacterTableType.GeneralProperty,
			ECharacterTableType.MainAndAttackProperty,
			ECharacterTableType.HitProperty,
			ECharacterTableType.LifeSkill,
			ECharacterTableType.CombatSkill,
			ECharacterTableType.Personality,
			ECharacterTableType.ItemAndResource,
			ECharacterTableType.Command,
			ECharacterTableType.LegendBookCompetitors,
			ECharacterTableType.LegendBookFallen
		});
		UIElement.SelectCharLegacy.SetOnInitArgs(argumentBox);
		UIManager.Instance.ShowUI(UIElement.SelectCharLegacy, true);
	}

	// Token: 0x06001B32 RID: 6962 RVA: 0x000BA550 File Offset: 0x000B8750
	private void FillShopManagerList()
	{
		List<int> buildingMemberList = this._buildingModel.GetBuildingShopManager(this._buildingBlockKey);
		bool flag = buildingMemberList != null;
		if (flag)
		{
			for (int i = 0; i < this._shopManagerListCached.Length; i++)
			{
				bool flag2 = i < buildingMemberList.Count;
				if (flag2)
				{
					this._shopManagerListCached[i] = buildingMemberList[i];
				}
				else
				{
					this._shopManagerListCached[i] = -1;
				}
			}
		}
		else
		{
			for (int j = 0; j < this._shopManagerListCached.Length; j++)
			{
				this._shopManagerListCached[j] = -1;
			}
		}
		this.MatchVillagerRoleAction(this._isLeaderRoleMatch);
	}

	// Token: 0x06001B33 RID: 6963 RVA: 0x000BA5F4 File Offset: 0x000B87F4
	private IEnumerator DisableShopQuickSelectButtonForAWhile()
	{
		this._buildingCraftsmanPanel.CGet<CButtonObsolete>("BtnAutoAssign").interactable = false;
		yield return new WaitForSeconds(0.5f);
		this.RefreshAllQuickSelectButtons();
		yield break;
	}

	// Token: 0x06001B34 RID: 6964 RVA: 0x000BA603 File Offset: 0x000B8803
	private void RefreshAllQuickSelectButtons()
	{
		BuildingDomainMethod.AsyncCall.CanQuickArrangeShopManager(this, this._buildingBlockKey, delegate(int offset, RawDataPool pool)
		{
			bool canQuickArrangeShopManager = false;
			Serializer.Deserialize(pool, offset, ref canQuickArrangeShopManager);
			this.RefreshQuickSelectButton(this._buildingCraftsmanPanel.CGet<CButtonObsolete>("BtnAutoAssign"), canQuickArrangeShopManager);
		});
	}

	// Token: 0x06001B35 RID: 6965 RVA: 0x000BA620 File Offset: 0x000B8820
	public void UnSelectShopManager()
	{
		for (sbyte i = 0; i < 7; i += 1)
		{
			bool flag = !this.CurrInfluenceLeader && i == 0;
			if (!flag)
			{
				bool flag2 = !this.CurrInfluenceMember && i != 0;
				if (!flag2)
				{
					BuildingDomainMethod.Call.SetShopManager(this._buildingBlockKey, i, -1);
				}
			}
		}
	}

	// Token: 0x06001B36 RID: 6966 RVA: 0x000BA67C File Offset: 0x000B887C
	private void UpdatePropertyValueData(BuildingBlockItem configData = null)
	{
		if (configData == null)
		{
			configData = this.BuildingConfigData;
		}
		bool flag = configData != null && configData.TemplateId != 0;
		if (flag)
		{
			foreach (int charId in this._villagerList)
			{
				bool flag2 = configData.RequireLifeSkillType >= 0;
				if (flag2)
				{
					CharacterDomainMethod.Call.GetLifeSkillAttainment(this.Element.GameDataListenerId, charId, configData.RequireLifeSkillType);
				}
				else
				{
					bool flag3 = configData.RequireCombatSkillType >= 0;
					if (!flag3)
					{
						throw new Exception(string.Format("Require skill of building {0} not filled", configData.TemplateId));
					}
					CharacterDomainMethod.Call.GetCombatSkillAttainment(this.Element.GameDataListenerId, charId, configData.RequireCombatSkillType);
				}
			}
		}
	}

	// Token: 0x06001B37 RID: 6967 RVA: 0x000BA764 File Offset: 0x000B8964
	private void RefreshQuickSelectButton(CButtonObsolete button, bool interactable)
	{
		button.interactable = interactable;
		button.transform.Find("Normal").gameObject.SetActive(interactable);
		button.transform.Find("Disable").gameObject.SetActive(!interactable);
		TooltipInvoker tipDisplayer = button.GetComponent<TooltipInvoker>();
		TooltipInvoker tooltipInvoker = tipDisplayer;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
		}
		tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(interactable ? LanguageKey.LK_Building_QuickArrangeTip : LanguageKey.LK_Building_QuickArrangeTip_Disable));
		tipDisplayer.Refresh(false, -1);
	}

	// Token: 0x06001B38 RID: 6968 RVA: 0x000BA804 File Offset: 0x000B8A04
	private void UpdateShopManagersNew()
	{
		sbyte lifeSkillType = UI_CraftsmanPanel.GetOperationNeedSkillType(this._currentCraftType);
		int studentAmount = 0;
		int i = 0;
		while (i < 7)
		{
			int charId = this.ShopManagerList[i];
			bool flag = i == 0;
			ResidentView residentView;
			if (flag)
			{
				residentView = this._managerLeaderViewRefer;
				this._managerLeaderViewRefer.RenderManagerLeaderCraftsmanPanel(charId, this.BuildingConfigData, new Action<bool>(this.MatchVillagerRoleAction), new Action<int>(this.OnQuickAssignRole));
				goto IL_D5;
			}
			Transform child = this._buildingCraftsmanPanel.CGet<GameObject>("MemberHolder").transform.GetChild(i - 1);
			child.gameObject.SetActive(true);
			residentView = child.GetComponent<ResidentView>();
			bool flag2 = residentView == null;
			if (!flag2)
			{
				residentView.RenderManagerMemberInfo(charId, this.BuildingConfigData, this._buildingBlockKey);
				bool flag3 = charId >= 0;
				if (flag3)
				{
					studentAmount++;
				}
				goto IL_D5;
			}
			IL_EB:
			i++;
			continue;
			IL_D5:
			Refers childRefer = residentView.GetComponent<Refers>();
			this.RefreshSelectCharacterBtn(i, childRefer, 1, lifeSkillType);
			goto IL_EB;
		}
		this.UpdateShopTitle();
		this.FillShopManagerList();
		TaiwuDomainMethod.Call.GetVillagerRoleCharacterDisplayDataOnPanel(this.Element.GameDataListenerId);
	}

	// Token: 0x06001B39 RID: 6969 RVA: 0x000BA92C File Offset: 0x000B8B2C
	private void MatchVillagerRoleAction(bool isMatch)
	{
		bool flag = this._isLeaderRoleMatch == isMatch;
		if (!flag)
		{
			this._isLeaderRoleMatch = isMatch;
			base.DelayFrameCall(delegate
			{
				this._buildingCraftsmanPanel.CGet<CRawImage>("teacherBase").gameObject.SetActive(isMatch);
				this._buildingCraftsmanPanel.CGet<CRawImage>("teacherBaseRed").gameObject.SetActive(!isMatch);
				this.RefreshMakeCraftsManPanelData();
				this.InitCraftTip();
			}, 1U);
		}
	}

	// Token: 0x06001B3A RID: 6970 RVA: 0x000BA984 File Offset: 0x000B8B84
	private void UpdateShopTitle()
	{
		StringBuilder _sb = EasyPool.Get<StringBuilder>();
		_sb.Clear();
		_sb.Append(this.BuildingConfigData.LeaderName).Append("(").Append(this.GetLeaderCount()).Append("/").Append(1).Append(")");
		this._buildingCraftsmanPanel.CGet<TextMeshProUGUI>("teacherAmountTxt").text = _sb.ToString();
		_sb.Clear();
		_sb.Append(this.BuildingConfigData.MemberName).Append("(").Append(this.GetMemberCount()).Append("/").Append(6).Append(")");
		this._buildingCraftsmanPanel.CGet<TextMeshProUGUI>("studentAmountTxt").text = _sb.ToString();
		EasyPool.Free<StringBuilder>(_sb);
	}

	// Token: 0x06001B3B RID: 6971 RVA: 0x000BAA68 File Offset: 0x000B8C68
	private int GetLeaderCount()
	{
		List<int> managerList = this.ShopManagerList;
		return (managerList[0] >= 0) ? 1 : 0;
	}

	// Token: 0x06001B3C RID: 6972 RVA: 0x000BAA90 File Offset: 0x000B8C90
	private int GetMemberCount()
	{
		List<int> managerList = this.ShopManagerList;
		sbyte count = 0;
		for (int i = 0; i < managerList.Count; i++)
		{
			bool flag = managerList[i] != -1;
			if (flag)
			{
				count += 1;
			}
		}
		return (int)count - this.GetLeaderCount();
	}

	// Token: 0x06001B3D RID: 6973 RVA: 0x000BAAE4 File Offset: 0x000B8CE4
	private int GetCurManagerId()
	{
		int managerIndex = 0;
		return this.ShopManagerList.CheckIndex(managerIndex) ? this.ShopManagerList[managerIndex] : -1;
	}

	// Token: 0x06001B3E RID: 6974 RVA: 0x000BAB18 File Offset: 0x000B8D18
	private void InitCraftTip()
	{
		int managerId = this.GetCurManagerId();
		bool flag = managerId < 0;
		if (flag)
		{
			this._villagerCraftPreviewPanel.ShowTip(LanguageKey.LK_Building_Craft_Tip_NoManager);
		}
		else
		{
			bool isLeaderRoleMatch = this._isLeaderRoleMatch;
			if (isLeaderRoleMatch)
			{
				this._villagerCraftPreviewPanel.ShowTip(LanguageKey.LK_VillagerCraftPreviewPanel_Tip_Normal);
			}
			else
			{
				this._villagerCraftPreviewPanel.ShowTip(LanguageKey.LK_Building_Craft_Tip_ManagerRoleNotMatch);
			}
		}
	}

	// Token: 0x06001B3F RID: 6975 RVA: 0x000BAB78 File Offset: 0x000B8D78
	private void OnQuickAssignRole(int villagerId)
	{
		short roleKey = this.BuildingConfigData.VillagerRoleTemplateIds[0];
		VillagerRoleUtils.ConfirmAndAssignRole(villagerId, roleKey, null, null, this);
	}

	// Token: 0x06001B40 RID: 6976 RVA: 0x000BABA0 File Offset: 0x000B8DA0
	private void HandleVillagerDisplayData(List<VillagerRoleCharacterDisplayData> villagerRoleCharacterDisplayDataList)
	{
		this.ResetVillagerList();
		for (int i = 0; i < villagerRoleCharacterDisplayDataList.Count; i++)
		{
			VillagerRoleCharacterDisplayData data = villagerRoleCharacterDisplayDataList[i];
			bool flag = (data.Flags & 1) > 0;
			if (flag)
			{
				this._currentVillagerList[(int)data.RoleTemplateId].Add(data);
			}
			else
			{
				bool flag2 = (data.Flags & 2) > 0;
				if (flag2)
				{
					this._lostVillagerList[(int)data.RoleTemplateId].Add(data);
				}
			}
		}
	}

	// Token: 0x06001B41 RID: 6977 RVA: 0x000BAC22 File Offset: 0x000B8E22
	private void ResetVillagerList()
	{
		UI_Make.ResetVillagerList(this._currentVillagerList);
		UI_Make.ResetVillagerList(this._lostVillagerList);
	}

	// Token: 0x06001B42 RID: 6978 RVA: 0x000BAC40 File Offset: 0x000B8E40
	private static void ResetVillagerList(List<VillagerRoleCharacterDisplayData>[] villagerList)
	{
		for (int i = 0; i < villagerList.Length; i++)
		{
			List<VillagerRoleCharacterDisplayData> t = villagerList[i];
			bool flag = t == null;
			if (flag)
			{
				t = new List<VillagerRoleCharacterDisplayData>();
				villagerList[i] = t;
			}
			else
			{
				t.Clear();
			}
		}
	}

	// Token: 0x06001B43 RID: 6979 RVA: 0x000BAC85 File Offset: 0x000B8E85
	private void OnBuildingManagerUpdate(ArgumentBox argBox)
	{
		this.RefreshMakeCraftsManPanelData();
	}

	// Token: 0x06001B44 RID: 6980 RVA: 0x000BAC8F File Offset: 0x000B8E8F
	private void OnSetVillagerRole(ArgumentBox _)
	{
		this.UpdateShopManagersNew();
	}

	// Token: 0x06001B45 RID: 6981 RVA: 0x000BAC99 File Offset: 0x000B8E99
	private void OnConfirmVillagerCraftInputMaterial(ArgumentBox argumentBox)
	{
	}

	// Token: 0x06001B46 RID: 6982 RVA: 0x000BAC9C File Offset: 0x000B8E9C
	private void OnClickBtnProductType()
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("ArtisanOrder", this._artisanOrder).SetObject("OnConfirm", new Action(this.<OnClickBtnProductType>g__OnConfirm|397_0));
		UIElement.SelectProductType.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.SelectProductType, true);
	}

	// Token: 0x06001B47 RID: 6983 RVA: 0x000BACF8 File Offset: 0x000B8EF8
	private void OnClickBtnAddAttainments()
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>().SetObject("ArtisanOrder", this._artisanOrder).SetObject("ProductionPool", this._productionPool).SetObject("CurrentCraftType", this._currentCraftType).Set("ItemSubType", this.CurItemSubType);
		UIElement.VillagerCraftInputMaterial.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.VillagerCraftInputMaterial, true);
	}

	// Token: 0x06001B48 RID: 6984 RVA: 0x000BAD70 File Offset: 0x000B8F70
	private void OnChangeStock(int stock)
	{
		if (!true)
		{
		}
		ItemSourceType itemSourceType2;
		switch (stock)
		{
		case 0:
			itemSourceType2 = ItemSourceType.Inventory;
			break;
		case 1:
			itemSourceType2 = ItemSourceType.Warehouse;
			break;
		case 2:
			itemSourceType2 = ItemSourceType.Treasury;
			break;
		case 3:
			itemSourceType2 = ItemSourceType.Stock;
			break;
		default:
			throw new NotImplementedException(string.Format("not supported: {0}", stock));
		}
		if (!true)
		{
		}
		ItemSourceType itemSourceType = itemSourceType2;
		ExtraDomainMethod.Call.SetArtisanOrderStorageType(this._artisanOrder, itemSourceType);
		this.RefreshMakeCraftsManPanelData();
	}

	// Token: 0x06001B49 RID: 6985 RVA: 0x000BADDC File Offset: 0x000B8FDC
	private void AddResource()
	{
		List<ItemKey> inventoryItems;
		this._craftsManExtraOperatePanel.PutResourceDic.TryGetValue(1, out inventoryItems);
		List<ItemKey> warehouseItems;
		this._craftsManExtraOperatePanel.PutResourceDic.TryGetValue(2, out warehouseItems);
		List<ItemKey> resourcesItems;
		this._craftsManExtraOperatePanel.PutResourceDic.TryGetValue(9, out resourcesItems);
		SingletonObject.getInstance<BuildingModel>().RefreshResources();
		this.RefreshMakeCraftsManPanelData();
	}

	// Token: 0x06001B4B RID: 6987 RVA: 0x000BB10C File Offset: 0x000B930C
	[CompilerGenerated]
	private void <OnEnable>g__InitTogGroup|162_1(CToggleGroupObsolete toggleGroup, ref UI_Make.<>c__DisplayClass162_0 A_2)
	{
		toggleGroup.Set(A_2.curKey, true, false);
		toggleGroup.SetInteractable(this._isSettlement, 0);
		MonoJoint[] componentsInChildren = toggleGroup.Get(0).gameObject.GetComponentsInChildren<MonoJoint>(true);
		if (componentsInChildren != null)
		{
			componentsInChildren.ForEach(delegate(int i, MonoJoint joint)
			{
				joint.JointSync();
				return false;
			});
		}
	}

	// Token: 0x06001B59 RID: 7001 RVA: 0x000BB334 File Offset: 0x000B9534
	[CompilerGenerated]
	private void <OnCheckMakeCondition>g__RefreshCountButton|267_0(ref UI_Make.<>c__DisplayClass267_0 A_1)
	{
		bool flag = this._currentTool == null;
		if (flag)
		{
			this._makeCountButtonLess.interactable = false;
			this._makeCountButtonMore.interactable = false;
			this._makeCountButtonMin.interactable = false;
			this._makeCountButtonMax.interactable = false;
		}
		else
		{
			this._makeCountButtonMin.interactable = (this._makeCountButtonLess.interactable = (this._makeCount > 1));
			bool canAdd = A_1.allMeet && this.CheckMakeResource((int)(this._makeCount + 1)) && (int)this._makeCount < this._maxMakeCount;
			this._makeCountButtonMax.interactable = (this._makeCountButtonMore.interactable = canAdd);
		}
	}

	// Token: 0x06001B62 RID: 7010 RVA: 0x000BB6AB File Offset: 0x000B98AB
	[CompilerGenerated]
	internal static bool <RefreshButtonIdentity>g__Match|303_0(ItemDisplayData i)
	{
		return i.Key.ItemType == 12 && i.Key.TemplateId == 264;
	}

	// Token: 0x06001B66 RID: 7014 RVA: 0x000BB86C File Offset: 0x000B9A6C
	[CompilerGenerated]
	private void <RefreshWeaveClothingView>g__Refresh|317_0(Refers clothingView, bool show, AvatarData avatarData, ref UI_Make.<>c__DisplayClass317_0 A_4)
	{
		bool flag = this._weaveHeadSprites.First<Sprite>() == null;
		if (flag)
		{
			show = false;
		}
		Game.Components.Avatar.Avatar avatar = clothingView.CGet<Game.Components.Avatar.Avatar>("Avatar");
		avatar.gameObject.SetActive(show);
		GameObject unselected = clothingView.CGet<GameObject>("Unselected");
		unselected.SetActive(!show);
		CToggleObsolete genderToggle = clothingView.CGet<CToggleObsolete>("GenderToggle");
		genderToggle.gameObject.SetActive(show);
		CToggleGroupObsolete bodyToggleGroup = clothingView.CGet<CToggleGroupObsolete>("BodyToggleGroup");
		bodyToggleGroup.gameObject.SetActive(show);
		bool flag2 = show;
		if (flag2)
		{
			sbyte gender = genderToggle.isOn ? 1 : 0;
			CToggleObsolete active = bodyToggleGroup.GetActive();
			int bodyType = (active != null) ? active.Key : 1;
			byte avatarId = Convert.ToByte(bodyType * 2 - (int)gender);
			avatarData.AvatarId = avatarId;
			avatar.RefreshAsClothTree(avatarData, 18, this._weaveHeadSprites, "");
			bool isOrigin = clothingView == A_4.currentClothingView;
			bool flag3 = isOrigin;
			if (flag3)
			{
				this._weaveClothingDisplaySetting.ClothingDisplayOriginSettingGender = (byte)gender;
				this._weaveClothingDisplaySetting.ClothingDisplayOriginSettingBodyType = (byte)bodyType;
			}
			else
			{
				this._weaveClothingDisplaySetting.ClothingDisplayPreviewSettingGender = (byte)gender;
				this._weaveClothingDisplaySetting.ClothingDisplayPreviewSettingBodyType = (byte)bodyType;
			}
			GameDataBridge.AddDataModification<WeaveClothingDisplaySetting>(5, 77, ulong.MaxValue, uint.MaxValue, this._weaveClothingDisplaySetting);
		}
	}

	// Token: 0x06001B6E RID: 7022 RVA: 0x000BBACC File Offset: 0x000B9CCC
	[CompilerGenerated]
	private void <RefreshMakeCraftsManPanelData>g__GetProductionPool|372_1()
	{
		bool flag = this._artisanOrder == null;
		if (flag)
		{
			this._productionPool = null;
			this.ProcessData();
		}
		else
		{
			ExtraDomainMethod.AsyncCall.GetArtisanOrderProductionPool(this, this._artisanOrder, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._productionPool);
				this.ProcessData();
			});
		}
	}

	// Token: 0x06001B71 RID: 7025 RVA: 0x000BBB5E File Offset: 0x000B9D5E
	[CompilerGenerated]
	private void <OnClickBtnProductType>g__OnConfirm|397_0()
	{
		ExtraDomainMethod.Call.SetArtisanOrderProductionType(this._artisanOrder, this._artisanOrder.ItemSubType);
		this.RefreshMakeCraftsManPanelData();
	}

	// Token: 0x040014CE RID: 5326
	private ItemScrollView _itemViewTools;

	// Token: 0x040014CF RID: 5327
	private ItemView _itemViewToolSelected;

	// Token: 0x040014D0 RID: 5328
	private ItemView _itemViewTargetSelected;

	// Token: 0x040014D1 RID: 5329
	private CButtonObsolete _buttonConfirm;

	// Token: 0x040014D2 RID: 5330
	private TextMeshProUGUI _toolDurabilityText;

	// Token: 0x040014D3 RID: 5331
	private CToggleGroupObsolete _pageTogGroup;

	// Token: 0x040014D4 RID: 5332
	private TextMeshProUGUI _confirmNormalLabel;

	// Token: 0x040014D5 RID: 5333
	private TextMeshProUGUI _confirmDisableLabel;

	// Token: 0x040014D6 RID: 5334
	private RectTransform _requireLifeSkillList;

	// Token: 0x040014D7 RID: 5335
	private Refers _slotTemplate;

	// Token: 0x040014D8 RID: 5336
	private Refers[] _slots;

	// Token: 0x040014D9 RID: 5337
	private ItemScrollView _itemViewSlot;

	// Token: 0x040014DA RID: 5338
	private Refers _poisonTip;

	// Token: 0x040014DB RID: 5339
	private ItemScrollView _itemViewMaterial;

	// Token: 0x040014DC RID: 5340
	private ItemScrollView _itemViewEquip;

	// Token: 0x040014DD RID: 5341
	private TextMeshProUGUI _makeTotalCountLabel;

	// Token: 0x040014DE RID: 5342
	private TooltipInvoker _confirmButtonTipDisplayer;

	// Token: 0x040014DF RID: 5343
	private CToggleGroupObsolete _toolTogGroup;

	// Token: 0x040014E0 RID: 5344
	private CToggleGroupObsolete _slotTogGroup;

	// Token: 0x040014E1 RID: 5345
	private CToggleGroupObsolete _materialTogGroup;

	// Token: 0x040014E2 RID: 5346
	private CToggleGroupObsolete _equipTogGroup;

	// Token: 0x040014E3 RID: 5347
	private CImage _colorBack;

	// Token: 0x040014E4 RID: 5348
	private CImage _colorLeft;

	// Token: 0x040014E5 RID: 5349
	private CImage _colorRight;

	// Token: 0x040014E6 RID: 5350
	private TextMeshProUGUI _makeTimeLabel;

	// Token: 0x040014E7 RID: 5351
	private Refers _makeTimeRoot;

	// Token: 0x040014E8 RID: 5352
	private TooltipInvoker _previewTip;

	// Token: 0x040014E9 RID: 5353
	private Refers _refineRequireResourceList;

	// Token: 0x040014EA RID: 5354
	private HorizontalPageSwitchController _makePageSwitchController;

	// Token: 0x040014EB RID: 5355
	private SubTogGroup _makeSubTogGroup;

	// Token: 0x040014EC RID: 5356
	private Refers _makeRequireResourceList;

	// Token: 0x040014ED RID: 5357
	private Refers _makeEffectList;

	// Token: 0x040014EE RID: 5358
	private CButtonObsolete _makeCountButtonLess;

	// Token: 0x040014EF RID: 5359
	private CButtonObsolete _makeCountButtonMore;

	// Token: 0x040014F0 RID: 5360
	private CButtonObsolete _makeCountButtonMin;

	// Token: 0x040014F1 RID: 5361
	private CButtonObsolete _makeCountButtonMax;

	// Token: 0x040014F2 RID: 5362
	private Refers _makePanel;

	// Token: 0x040014F3 RID: 5363
	private int _buildingAttainmentEffect;

	// Token: 0x040014F4 RID: 5364
	private bool _haveBuildingUpgradeMakeItem;

	// Token: 0x040014F5 RID: 5365
	private CDropdownLegacy _makeDropdown;

	// Token: 0x040014F6 RID: 5366
	private BuildingStoreToDropdown _buildingStoreToDropdown;

	// Token: 0x040014F7 RID: 5367
	private GameObject _makeDropdownMask;

	// Token: 0x040014F8 RID: 5368
	private readonly List<ItemSortAndFilter.ItemFilterType> _poisonItemFilterTypeList = new List<ItemSortAndFilter.ItemFilterType>
	{
		ItemSortAndFilter.ItemFilterType.Invalid,
		ItemSortAndFilter.ItemFilterType.Equip,
		ItemSortAndFilter.ItemFilterType.Food,
		ItemSortAndFilter.ItemFilterType.Medicine,
		ItemSortAndFilter.ItemFilterType.Book
	};

	// Token: 0x040014F9 RID: 5369
	private bool _hasIdentifiedResult;

	// Token: 0x040014FA RID: 5370
	private bool _curIdentifySuccess;

	// Token: 0x040014FB RID: 5371
	private bool _curIdentifiedResultHasPoison;

	// Token: 0x040014FC RID: 5372
	private Action _curIdentifiedResultAction;

	// Token: 0x040014FD RID: 5373
	private readonly List<short> _mixedPoisonMedicineIdList = new List<short>();

	// Token: 0x040014FE RID: 5374
	private short _targetMixedPoisonMedicineId = -1;

	// Token: 0x040014FF RID: 5375
	private HorizontalPageSwitchController _poisonPageSwitchController;

	// Token: 0x04001500 RID: 5376
	private readonly FullPoisonEffects _originPoisonEffects = new FullPoisonEffects();

	// Token: 0x04001501 RID: 5377
	private readonly FullPoisonEffects _tempPoisonEffects = new FullPoisonEffects();

	// Token: 0x04001502 RID: 5378
	private readonly List<ItemDisplayData> _condensePoisonItemList = new List<ItemDisplayData>();

	// Token: 0x04001503 RID: 5379
	private short _makeCount = 1;

	// Token: 0x04001504 RID: 5380
	private int _maxMakeCount = 1;

	// Token: 0x04001505 RID: 5381
	private int _makeToolDurabilityCost;

	// Token: 0x04001506 RID: 5382
	private short _makeTime = 1;

	// Token: 0x04001507 RID: 5383
	private readonly Dictionary<short, List<short>> _makeTypeDict = new Dictionary<short, List<short>>();

	// Token: 0x04001508 RID: 5384
	private readonly List<short> _makeTypeList = new List<short>();

	// Token: 0x04001509 RID: 5385
	private short _makeItemTypeId;

	// Token: 0x0400150A RID: 5386
	private short _makeItemSubTypeId;

	// Token: 0x0400150B RID: 5387
	private short _makeItemTemplateId = -1;

	// Token: 0x0400150C RID: 5388
	private List<short> _makeItemSubtypeIdList;

	// Token: 0x0400150D RID: 5389
	private short _maxMakeResourceTotalCount;

	// Token: 0x0400150E RID: 5390
	private ResourceInts _maxMakeResourceCountInts;

	// Token: 0x0400150F RID: 5391
	private ResourceInts _curMakeResourceCountInts;

	// Token: 0x04001510 RID: 5392
	private MaterialResources _lastMakeResourceCount;

	// Token: 0x04001511 RID: 5393
	private bool _makeIsManual;

	// Token: 0x04001512 RID: 5394
	private bool _needKeepTypeSelectionOnTargetChanged;

	// Token: 0x04001513 RID: 5395
	private bool _needKeepSubtypeSelectionOnNoneTarget;

	// Token: 0x04001514 RID: 5396
	private MakeItemData _workingMakeItemData;

	// Token: 0x04001515 RID: 5397
	private MakeItemData _finishedMakeItemData;

	// Token: 0x04001516 RID: 5398
	private ResourceInts _makeRequiredResourceInts;

	// Token: 0x04001517 RID: 5399
	private int _curMakeItemSubToggleIndex = -1;

	// Token: 0x04001518 RID: 5400
	private readonly Dictionary<string, List<GameObject>> _makeResourceSpriteDict = new Dictionary<string, List<GameObject>>();

	// Token: 0x04001519 RID: 5401
	private Coroutine[] _corChangeMakeResourceCount = new Coroutine[6];

	// Token: 0x0400151A RID: 5402
	private readonly Dictionary<int, MakeResult> _makeResultDict = new Dictionary<int, MakeResult>();

	// Token: 0x0400151B RID: 5403
	private bool _isWaitingMakeResultRequest;

	// Token: 0x0400151C RID: 5404
	private bool _makePerfect;

	// Token: 0x0400151D RID: 5405
	private bool _isAddPoisonCondense;

	// Token: 0x0400151E RID: 5406
	private bool _isRemovePoisonExtract;

	// Token: 0x0400151F RID: 5407
	private readonly List<string> _makeDropdownOptionList = new List<string>();

	// Token: 0x04001520 RID: 5408
	[TupleElementNames(new string[]
	{
		"subTypeId",
		"templateId",
		"name"
	})]
	private readonly List<ValueTuple<short, short, string>> _curMakeDropdownDataList = new List<ValueTuple<short, short, string>>();

	// Token: 0x04001521 RID: 5409
	[TupleElementNames(new string[]
	{
		"subTypeId",
		"templateId",
		"name"
	})]
	private readonly List<ValueTuple<short, short, string>> _lastMakeDropdownDataList = new List<ValueTuple<short, short, string>>();

	// Token: 0x04001522 RID: 5410
	private bool _needResetMakeDropDownValue;

	// Token: 0x04001523 RID: 5411
	private int _selectedMakeDropDownValue;

	// Token: 0x04001524 RID: 5412
	private bool _needAutoFillResourceOnMakeEnd;

	// Token: 0x04001525 RID: 5413
	private bool _isShowingGetItem;

	// Token: 0x04001526 RID: 5414
	private readonly Dictionary<ItemSourceType, List<ItemDisplayData>> _slotItemListDict = new Dictionary<ItemSourceType, List<ItemDisplayData>>
	{
		{
			ItemSourceType.Inventory,
			new List<ItemDisplayData>()
		},
		{
			ItemSourceType.Warehouse,
			new List<ItemDisplayData>()
		},
		{
			ItemSourceType.Treasury,
			new List<ItemDisplayData>()
		}
	};

	// Token: 0x04001527 RID: 5415
	private readonly Dictionary<ItemSourceType, ItemSourceChange> _slotItemChangeDict = new Dictionary<ItemSourceType, ItemSourceChange>
	{
		{
			ItemSourceType.Inventory,
			new ItemSourceChange(ItemSourceType.Inventory)
		},
		{
			ItemSourceType.Warehouse,
			new ItemSourceChange(ItemSourceType.Warehouse)
		},
		{
			ItemSourceType.Treasury,
			new ItemSourceChange(ItemSourceType.Treasury)
		}
	};

	// Token: 0x04001528 RID: 5416
	private ItemSourceType _curSlotItemSourceType = ItemSourceType.Inventory;

	// Token: 0x04001529 RID: 5417
	private readonly ItemDisplayData _invalidItemDisplayData = new ItemDisplayData
	{
		Key = ItemKey.Invalid
	};

	// Token: 0x0400152A RID: 5418
	private BuildingBlockKey _buildingBlockKey;

	// Token: 0x0400152B RID: 5419
	private BuildingBlockData _blockData;

	// Token: 0x0400152C RID: 5420
	private bool _isSettlement;

	// Token: 0x0400152D RID: 5421
	private const short EmptyId = -1;

	// Token: 0x0400152E RID: 5422
	private short _toolDurabilityCost;

	// Token: 0x0400152F RID: 5423
	private ItemDisplayData _currentTool;

	// Token: 0x04001530 RID: 5424
	private ItemDisplayData _currentTarget;

	// Token: 0x04001531 RID: 5425
	private BuildingModel _buildingModel;

	// Token: 0x04001532 RID: 5426
	private LifeSkillShorts _lifeSkillAttainments;

	// Token: 0x04001533 RID: 5427
	private List<ItemDisplayData> _inventoryItems = new List<ItemDisplayData>();

	// Token: 0x04001534 RID: 5428
	private List<ItemDisplayData> _warehouseItems = new List<ItemDisplayData>();

	// Token: 0x04001535 RID: 5429
	private List<ItemDisplayData> _treasuryItems = new List<ItemDisplayData>();

	// Token: 0x04001536 RID: 5430
	private List<ItemDisplayData> _equipmentItems = new List<ItemDisplayData>();

	// Token: 0x04001537 RID: 5431
	private List<ItemDisplayData> _allItems = new List<ItemDisplayData>();

	// Token: 0x04001538 RID: 5432
	private List<ItemDisplayData> _itemCacheTools = new List<ItemDisplayData>();

	// Token: 0x04001539 RID: 5433
	private List<ItemDisplayData> _itemCacheTargets = new List<ItemDisplayData>();

	// Token: 0x0400153A RID: 5434
	private const float ParticleAnimSimulateTime = 0.42f;

	// Token: 0x0400153B RID: 5435
	private readonly Dictionary<sbyte, string> _makeBuildingEffectDict = new Dictionary<sbyte, string>
	{
		{
			6,
			"ForgingBuildingEffect"
		},
		{
			7,
			"WoodworkingBuildingEffect"
		},
		{
			11,
			"JadeBuildingEffect"
		},
		{
			14,
			"CookingBuildingEffect"
		},
		{
			10,
			"WeavingBuildingEffect"
		},
		{
			8,
			"MedicineBuildingEffect"
		},
		{
			9,
			"ToxicologyBuildingEffect"
		}
	};

	// Token: 0x0400153C RID: 5436
	private const string RepairIndoorEffectName = "IndoorRepairEffect";

	// Token: 0x0400153D RID: 5437
	private const string RepairOutdoorEffectName = "OutdoorRepairEffect";

	// Token: 0x0400153E RID: 5438
	private string _curEffectName;

	// Token: 0x0400153F RID: 5439
	private UI_Make.UIMakeTab _curTab;

	// Token: 0x04001540 RID: 5440
	private HashSet<UI_Make.UIMakeTab> _allTabSet;

	// Token: 0x04001541 RID: 5441
	private sbyte _curLifeSkillType;

	// Token: 0x04001542 RID: 5442
	private readonly ItemDisplayData[] _slotItemArray = new ItemDisplayData[UI_Make.MaxSlotCount];

	// Token: 0x04001543 RID: 5443
	private readonly ItemDisplayData[] _originSlotItemArray = new ItemDisplayData[UI_Make.MaxSlotCount];

	// Token: 0x04001544 RID: 5444
	private int _taiwuCharId;

	// Token: 0x04001545 RID: 5445
	private ItemKey _repairToolKey;

	// Token: 0x04001546 RID: 5446
	private ItemKey _repairedItemkey;

	// Token: 0x04001547 RID: 5447
	private LifeSkillMonitor _lifeSkillMonitor;

	// Token: 0x04001548 RID: 5448
	private ItemDisplayData _resultItemDisplayData = new ItemDisplayData();

	// Token: 0x04001549 RID: 5449
	private readonly HashSetAsDictionary<short> _ownedClothingSet = new HashSetAsDictionary<short>();

	// Token: 0x0400154A RID: 5450
	private readonly List<ItemDisplayData> _clothingWeaveTargetList = new List<ItemDisplayData>();

	// Token: 0x0400154B RID: 5451
	private CharacterDisplayData _characterDisplayData;

	// Token: 0x0400154C RID: 5452
	private ItemKey _emptyToolKey;

	// Token: 0x0400154D RID: 5453
	private List<ItemKey> _villagerNeededItemSet = new List<ItemKey>();

	// Token: 0x0400154E RID: 5454
	private bool _needRefreshAfterEvent;

	// Token: 0x0400154F RID: 5455
	private readonly Sprite[] _weaveHeadSprites = new Sprite[6];

	// Token: 0x04001550 RID: 5456
	private WeaveClothingDisplaySetting _weaveClothingDisplaySetting;

	// Token: 0x04001551 RID: 5457
	private Action _onClickConfirm;

	// Token: 0x04001552 RID: 5458
	private DG.Tweening.Sequence _perfectEffectSelectSequence;

	// Token: 0x04001553 RID: 5459
	private Refers _craftsRootPanel;

	// Token: 0x04001554 RID: 5460
	private Refers _buildingCraftsmanPanel;

	// Token: 0x04001555 RID: 5461
	private Refers _predictDomainRefer;

	// Token: 0x04001556 RID: 5462
	private CraftsManExtraOperatePanel _craftsManExtraOperatePanel;

	// Token: 0x04001557 RID: 5463
	private ResidentView _managerLeaderViewRefer;

	// Token: 0x04001558 RID: 5464
	private VillagerCraftPreviewPanel _villagerCraftPreviewPanel;

	// Token: 0x04001559 RID: 5465
	private ProductionPool _productionPool;

	// Token: 0x0400155A RID: 5466
	private ArtisanOrder _artisanOrder;

	// Token: 0x0400155B RID: 5467
	private sbyte _productResourceType;

	// Token: 0x0400155C RID: 5468
	private int _productResourceAdded;

	// Token: 0x0400155D RID: 5469
	private ECraftType _currentCraftType;

	// Token: 0x0400155E RID: 5470
	private bool _isLeaderRoleMatch;

	// Token: 0x0400155F RID: 5471
	private sbyte _craftsmanType;

	// Token: 0x04001560 RID: 5472
	private List<int> _availableWorker = new List<int>();

	// Token: 0x04001561 RID: 5473
	private List<int> _availableChildren = new List<int>();

	// Token: 0x04001562 RID: 5474
	private readonly int[] _shopManagerListCached = new int[7];

	// Token: 0x04001563 RID: 5475
	private readonly Dictionary<int, CharacterDisplayData> _charDisplayDataDict = new Dictionary<int, CharacterDisplayData>();

	// Token: 0x04001564 RID: 5476
	private readonly int[] _operatorListCached = new int[3];

	// Token: 0x04001565 RID: 5477
	private int _selectingShopManagerIndex;

	// Token: 0x04001566 RID: 5478
	private readonly List<int> _villagerList = new List<int>();

	// Token: 0x04001567 RID: 5479
	private List<VillagerRoleCharacterDisplayData>[] _currentVillagerList;

	// Token: 0x04001568 RID: 5480
	private List<VillagerRoleCharacterDisplayData>[] _lostVillagerList;

	// Token: 0x0200135F RID: 4959
	public enum UIMakeTab
	{
		// Token: 0x04009D94 RID: 40340
		None,
		// Token: 0x04009D95 RID: 40341
		Make,
		// Token: 0x04009D96 RID: 40342
		Repair,
		// Token: 0x04009D97 RID: 40343
		Poison,
		// Token: 0x04009D98 RID: 40344
		RemovePoison,
		// Token: 0x04009D99 RID: 40345
		Refine,
		// Token: 0x04009D9A RID: 40346
		Weave,
		// Token: 0x04009D9B RID: 40347
		CraftsmanPanel
	}

	// Token: 0x02001360 RID: 4960
	private enum TogType
	{
		// Token: 0x04009D9D RID: 40349
		Inventory,
		// Token: 0x04009D9E RID: 40350
		Warehouse,
		// Token: 0x04009D9F RID: 40351
		Treasury
	}
}
