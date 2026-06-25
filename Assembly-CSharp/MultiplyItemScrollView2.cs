using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CharacterDataMonitor;
using CommonSortAndFilterLegacy.Item;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using Game.Views.Item;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x02000234 RID: 564
public class MultiplyItemScrollView2 : Refers
{
	// Token: 0x0600245B RID: 9307 RVA: 0x0010B430 File Offset: 0x00109630
	public bool IsTaiwuGearMate(int charId)
	{
		return SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(charId);
	}

	// Token: 0x1700039E RID: 926
	// (get) Token: 0x0600245C RID: 9308 RVA: 0x0010B43D File Offset: 0x0010963D
	public long SelectedMultiplyItemTotalWorth
	{
		get
		{
			return this.SelectedMultiplyItemDict.Sum((KeyValuePair<ItemDisplayData, int> pair) => pair.Key.Value * (long)pair.Value);
		}
	}

	// Token: 0x1700039F RID: 927
	// (get) Token: 0x0600245D RID: 9309 RVA: 0x0010B469 File Offset: 0x00109669
	private List<ItemDisplayData> CurInventoryItems
	{
		get
		{
			return this._itemDict.GetValueOrDefault(this.InventoryItemSourceType);
		}
	}

	// Token: 0x170003A0 RID: 928
	// (get) Token: 0x0600245E RID: 9310 RVA: 0x0010B47C File Offset: 0x0010967C
	private List<ItemDisplayData> CurWarehouseItems
	{
		get
		{
			return this._itemDict.GetValueOrDefault(this.WarehouseItemSourceType);
		}
	}

	// Token: 0x170003A1 RID: 929
	// (get) Token: 0x0600245F RID: 9311 RVA: 0x0010B48F File Offset: 0x0010968F
	private static int TaiwuCharId
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}
	}

	// Token: 0x170003A2 RID: 930
	// (get) Token: 0x06002460 RID: 9312 RVA: 0x0010B49B File Offset: 0x0010969B
	private bool CurCharacterIsTaiwu
	{
		get
		{
			return this.CurCharId == MultiplyItemScrollView2.TaiwuCharId;
		}
	}

	// Token: 0x170003A3 RID: 931
	// (get) Token: 0x06002461 RID: 9313 RVA: 0x0010B4AA File Offset: 0x001096AA
	public ItemOperationType.EItemOperationType CurrItemOperation
	{
		get
		{
			return this._currItemOperation;
		}
	}

	// Token: 0x170003A4 RID: 932
	// (get) Token: 0x06002462 RID: 9314 RVA: 0x0010B4B2 File Offset: 0x001096B2
	public Dictionary<ItemDisplayData, int> SelectedMultiplyItemDict
	{
		get
		{
			return this._multiplyOperationHandler.SelectedItemDict;
		}
	}

	// Token: 0x170003A5 RID: 933
	// (get) Token: 0x06002463 RID: 9315 RVA: 0x0010B4BF File Offset: 0x001096BF
	public List<ItemDisplayData> SelectedMultiplyItemOrderedList
	{
		get
		{
			return this._multiplyOperationHandler.SelectedItemOrderedList;
		}
	}

	// Token: 0x170003A6 RID: 934
	// (get) Token: 0x06002464 RID: 9316 RVA: 0x0010B4CC File Offset: 0x001096CC
	// (set) Token: 0x06002465 RID: 9317 RVA: 0x0010B4D4 File Offset: 0x001096D4
	public bool IsMultiItemSelect { get; private set; }

	// Token: 0x170003A7 RID: 935
	// (get) Token: 0x06002466 RID: 9318 RVA: 0x0010B4E0 File Offset: 0x001096E0
	public CButtonObsolete BtnMultiplySelect
	{
		get
		{
			CButtonObsolete btnMultiplySelect;
			return this.SourceRootRefer.CTryGet<CButtonObsolete>("BtnMultiplySelect", out btnMultiplySelect) ? btnMultiplySelect : null;
		}
	}

	// Token: 0x170003A8 RID: 936
	// (get) Token: 0x06002467 RID: 9319 RVA: 0x0010B505 File Offset: 0x00109705
	public CommonSwitch SwitchSelection
	{
		get
		{
			return this.SourceRootRefer.CGet<CommonSwitch>("SwitchSelection");
		}
	}

	// Token: 0x170003A9 RID: 937
	// (get) Token: 0x06002468 RID: 9320 RVA: 0x0010B517 File Offset: 0x00109717
	public CToggleObsolete ToggleSelectAll
	{
		get
		{
			return this.SourceRootRefer.CGet<CToggleObsolete>("ToggleSelectAll");
		}
	}

	// Token: 0x170003AA RID: 938
	// (get) Token: 0x06002469 RID: 9321 RVA: 0x0010B529 File Offset: 0x00109729
	private List<ItemDisplayData> UsableToolOrderedList
	{
		get
		{
			return this._multiplyOperationHandler.UsableToolOrderedList;
		}
	}

	// Token: 0x170003AB RID: 939
	// (get) Token: 0x0600246A RID: 9322 RVA: 0x0010B536 File Offset: 0x00109736
	// (set) Token: 0x0600246B RID: 9323 RVA: 0x0010B53E File Offset: 0x0010973E
	public bool IsInventoryMultiply { get; set; }

	// Token: 0x170003AC RID: 940
	// (get) Token: 0x0600246C RID: 9324 RVA: 0x0010B547 File Offset: 0x00109747
	public BaseItemScrollView2 CurMultiplyScrollView
	{
		get
		{
			return this.IsInventoryMultiply ? this._inventoryScroll : this._warehouseScroll;
		}
	}

	// Token: 0x170003AD RID: 941
	// (get) Token: 0x0600246D RID: 9325 RVA: 0x0010B55F File Offset: 0x0010975F
	private BaseItemScrollView2 OtherScrollView
	{
		get
		{
			return (!this.IsInventoryMultiply) ? this._inventoryScroll : this._warehouseScroll;
		}
	}

	// Token: 0x170003AE RID: 942
	// (get) Token: 0x0600246E RID: 9326 RVA: 0x0010B577 File Offset: 0x00109777
	private List<ItemDisplayData> CurMultiplyItems
	{
		get
		{
			return this.IsInventoryMultiply ? this.CurInventoryItems : this.CurWarehouseItems;
		}
	}

	// Token: 0x170003AF RID: 943
	// (get) Token: 0x0600246F RID: 9327 RVA: 0x0010B58F File Offset: 0x0010978F
	private List<ItemDisplayData> OtherItems
	{
		get
		{
			return (!this.IsInventoryMultiply) ? this.CurInventoryItems : this.CurWarehouseItems;
		}
	}

	// Token: 0x170003B0 RID: 944
	// (get) Token: 0x06002470 RID: 9328 RVA: 0x0010B5A7 File Offset: 0x001097A7
	private ItemSourceType CurMultiplyItemSourceType
	{
		get
		{
			return this.IsInventoryMultiply ? this.InventoryItemSourceType : this.WarehouseItemSourceType;
		}
	}

	// Token: 0x170003B1 RID: 945
	// (get) Token: 0x06002471 RID: 9329 RVA: 0x0010B5C0 File Offset: 0x001097C0
	private ItemGradeFilterSetting.ItemGradeFilterSourceType CurItemGradeFilterSourceType
	{
		get
		{
			ItemSourceType itemSourceType = this.IsInventoryMultiply ? this.InventoryItemSourceType : this.WarehouseItemSourceType;
			return ItemGradeFilterSetting.GetItemGradeFilterSourceType(itemSourceType, itemSourceType == ItemSourceType.Inventory, false);
		}
	}

	// Token: 0x170003B2 RID: 946
	// (get) Token: 0x06002472 RID: 9330 RVA: 0x0010B5F4 File Offset: 0x001097F4
	public Refers SourceRootRefer
	{
		get
		{
			return this.IsInventoryMultiply ? this.InventoryRefer : this.WarehouseRefer;
		}
	}

	// Token: 0x170003B3 RID: 947
	// (get) Token: 0x06002473 RID: 9331 RVA: 0x0010B60C File Offset: 0x0010980C
	private Refers InventoryRefer
	{
		get
		{
			return base.CGet<Refers>("Inventory");
		}
	}

	// Token: 0x170003B4 RID: 948
	// (get) Token: 0x06002474 RID: 9332 RVA: 0x0010B619 File Offset: 0x00109819
	private Refers WarehouseRefer
	{
		get
		{
			return base.CGet<Refers>("Warehouse");
		}
	}

	// Token: 0x170003B5 RID: 949
	// (get) Token: 0x06002475 RID: 9333 RVA: 0x0010B626 File Offset: 0x00109826
	private ItemMultiplyOperationPanel ItemMultiplyOperationPanel
	{
		get
		{
			return base.CGet<ItemMultiplyOperationPanel>("ItemMultiplyOperationPanel");
		}
	}

	// Token: 0x170003B6 RID: 950
	// (get) Token: 0x06002476 RID: 9334 RVA: 0x0010B633 File Offset: 0x00109833
	// (set) Token: 0x06002477 RID: 9335 RVA: 0x0010B63B File Offset: 0x0010983B
	public int MaxSelectCount { get; set; }

	// Token: 0x170003B7 RID: 951
	// (get) Token: 0x06002478 RID: 9336 RVA: 0x0010B644 File Offset: 0x00109844
	// (set) Token: 0x06002479 RID: 9337 RVA: 0x0010B64C File Offset: 0x0010984C
	public int TotalSelectedCount { get; private set; }

	// Token: 0x0600247A RID: 9338 RVA: 0x0010B658 File Offset: 0x00109858
	public void Init(int charId, Dictionary<ItemSourceType, List<ItemDisplayData>> itemDict, Action onEnter, Action onExit, [TupleElementNames(new string[]
	{
		"data",
		"count"
	})] Action<List<ValueTuple<ItemDisplayData, int>>> onContentChange, bool isTaiwuTeam = true)
	{
		this._itemDict = itemDict;
		this.CurCharId = charId;
		this._onEnterMultiplyOperation = onEnter;
		this._onExitMultiplyOperation = onExit;
		this._onContentChange = onContentChange;
		this._isTaiWuTeam = isTaiwuTeam;
		this.IsInventoryMultiply = true;
		this._itemGradeFilterSetting = SingletonObject.getInstance<GameSort>().GetItemGradeFilterSetting();
		this._resourceMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<ResourceMonitor>(MultiplyItemScrollView2.TaiwuCharId, false);
		this._lifeSkillMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<LifeSkillMonitor>(MultiplyItemScrollView2.TaiwuCharId, false);
		this.ItemMultiplyOperationPanel.Hide();
		bool flag = this.Names.Contains("Inventory");
		if (flag)
		{
			this._inventoryScroll = this.InventoryRefer.CGet<GroupedItemScrollView2>("GroupedItemScrollView");
			this._selectedItemScroll = this.InventoryRefer.CGet<ItemScrollViewForCommonTableRow>("SelectedItemScrollView");
			this.<Init>g__InitButton|87_0(this.InventoryRefer, true);
		}
		bool flag2 = this.Names.Contains("Warehouse");
		if (flag2)
		{
			this._warehouseScroll = this.WarehouseRefer.CGet<GroupedItemScrollView2>("GroupedItemScrollView");
			this.<Init>g__InitButton|87_0(this.WarehouseRefer, false);
		}
	}

	// Token: 0x0600247B RID: 9339 RVA: 0x0010B770 File Offset: 0x00109970
	public bool IsSelected(ItemDisplayData itemData)
	{
		int selectedCount;
		this.SelectedMultiplyItemDict.TryGetValue(itemData, out selectedCount);
		return selectedCount > 0;
	}

	// Token: 0x0600247C RID: 9340 RVA: 0x0010B798 File Offset: 0x00109998
	private void SelectAll(bool isSelect)
	{
		if (isSelect)
		{
			List<ItemDisplayData> canOperateItemList = this.CurMultiplyScrollView.OutputItemList.ToList<ItemDisplayData>();
			using (List<ItemDisplayData>.Enumerator enumerator = canOperateItemList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ItemDisplayData tempData = enumerator.Current;
					ItemDisplayData data = this.CurMultiplyItems.Find((ItemDisplayData d) => d.RealKey.Equals(tempData.RealKey));
					List<ItemDisplayData> toolList;
					bool interactable = this.CheckInteractable(data, out toolList);
					bool flag = interactable;
					if (flag)
					{
						int limitCount;
						string text;
						this.PrepareSelect(data, toolList, out limitCount, out text);
						int lastSelectedCount;
						this.SelectedMultiplyItemDict.TryGetValue(data, out lastSelectedCount);
						int selectedCount = Mathf.Min(data.Amount, limitCount + lastSelectedCount);
						this.SetItemSelectCount(data.RealKey, selectedCount, toolList, false);
					}
				}
			}
		}
		else
		{
			List<ItemDisplayData> selectedItemList = this.SelectedMultiplyItemDict.Keys.ToList<ItemDisplayData>();
			foreach (ItemDisplayData itemData in selectedItemList)
			{
				this.SetItemSelectCount(itemData.RealKey, 0, null, false);
			}
		}
		int num;
		int num2;
		this.RefreshFeedState(out num, out num2);
		this.RefreshMultiplyCanOperateItems();
		this.SendItemMultiplyOperationContentChange(null);
	}

	// Token: 0x0600247D RID: 9341 RVA: 0x0010B900 File Offset: 0x00109B00
	public void OnRenderItemMultiply(ItemDisplayData tempData, CommonTableRowForItem view)
	{
		MultiplyItemScrollView2.<>c__DisplayClass90_0 CS$<>8__locals1 = new MultiplyItemScrollView2.<>c__DisplayClass90_0();
		CS$<>8__locals1.tempData = tempData;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.view = view;
		CS$<>8__locals1.itemData = this.CurMultiplyItems.Find((ItemDisplayData d) => d.RealKey.Equals(CS$<>8__locals1.tempData.RealKey));
		CS$<>8__locals1.view.SetFavoriteStatus(CommonTableRowForItem.FavoriteStatus.None);
		this.SelectedMultiplyItemDict.TryGetValue(CS$<>8__locals1.itemData, out CS$<>8__locals1.selectedCount);
		CS$<>8__locals1.isSelected = (CS$<>8__locals1.selectedCount > 0);
		CS$<>8__locals1.view.SetSelectState(CS$<>8__locals1.isSelected);
		bool flag = !CS$<>8__locals1.view.IsLocked;
		if (flag)
		{
			CS$<>8__locals1.view.SetInteractable(true);
		}
		List<ItemDisplayData> usedToolList = null;
		List<ItemDisplayData> availableToolList = null;
		bool isSelected = CS$<>8__locals1.isSelected;
		if (isSelected)
		{
			int index = this.GetSelectedOrder(CS$<>8__locals1.itemData);
			ItemOperationType.EItemOperationType currItemOperation = this._currItemOperation;
			ItemOperationType.EItemOperationType eitemOperationType = currItemOperation;
			if (eitemOperationType != ItemOperationType.EItemOperationType.Repair)
			{
				if (eitemOperationType != ItemOperationType.EItemOperationType.Disassemble)
				{
					if (eitemOperationType != ItemOperationType.EItemOperationType.SpecialBreakConvertToExp)
					{
						CS$<>8__locals1.view.HideInteractionState();
					}
				}
				else
				{
					short required = ItemTemplateHelper.GetDisassembleRequiredAttainment(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
					sbyte itemLifeSkillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
					CS$<>8__locals1.view.ShowInteractionStateAttainment(itemLifeSkillType, required, true);
				}
			}
			else
			{
				short required2 = ItemTemplateHelper.GetRepairRequiredAttainment(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId, CS$<>8__locals1.itemData.Durability);
				sbyte itemLifeSkillType2 = ItemTemplateHelper.GetCraftRequiredLifeSkillType(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
				CS$<>8__locals1.view.ShowInteractionStateAttainment(itemLifeSkillType2, required2, true);
			}
			CS$<>8__locals1.view.SetSelectState(true);
		}
		else
		{
			switch (this._currItemOperation)
			{
			case ItemOperationType.EItemOperationType.Repair:
			case ItemOperationType.EItemOperationType.Disassemble:
			{
				sbyte itemLifeSkillType3;
				short required3;
				bool hasAvailableTool;
				bool interactable = this.CheckInteractableWithTool(CS$<>8__locals1.itemData, out itemLifeSkillType3, out required3, out hasAvailableTool, ref availableToolList);
				bool flag2 = interactable;
				if (flag2)
				{
					CS$<>8__locals1.view.HideInteractionState();
				}
				else
				{
					bool flag3 = hasAvailableTool;
					if (flag3)
					{
						CS$<>8__locals1.view.ShowInteractionStateAttainment(itemLifeSkillType3, required3, false);
					}
					else
					{
						string content = LocalStringManager.Get(LanguageKey.LK_Tool_Not_Enough).SetColor("brightred");
						CS$<>8__locals1.view.SetInteractable(false);
						CS$<>8__locals1.view.SetItemNotCanSelectReason(content);
					}
				}
				break;
			}
			case ItemOperationType.EItemOperationType.Take:
			{
				bool isTaiwuGearMate = this.IsTaiwuGearMate(this.CurCharId);
				int num;
				bool canTakeItem = UI_CharacterMenuItems.CanTakeItem(CS$<>8__locals1.itemData, this.MaxWorthCanBeLentToTaiwu, isTaiwuGearMate, out num);
				CS$<>8__locals1.view.SetInteractable(canTakeItem);
				break;
			}
			case ItemOperationType.EItemOperationType.Discard:
				CS$<>8__locals1.view.HideInteractionState();
				break;
			case ItemOperationType.EItemOperationType.SpecialBreakConvertToExp:
				CS$<>8__locals1.view.SetInteractable(true);
				break;
			case ItemOperationType.EItemOperationType.Feeding:
			{
				bool flag4 = this._isFeedDurabilityMax && this._isFeedTameMax;
				if (flag4)
				{
					CS$<>8__locals1.view.SetInteractable(false);
					CS$<>8__locals1.view.SetItemNotCanSelectReason(LocalStringManager.Get(LanguageKey.LK_ItemFeedCarrier_Tip_Full).SetColor("brightred"));
					CS$<>8__locals1.view.SetFavoriteStatus(CommonTableRowForItem.FavoriteStatus.None);
				}
				else
				{
					CS$<>8__locals1.view.SetInteractable(true);
					ItemDisplayData feedingTarget = this._feedingTarget;
					short templateId = (feedingTarget != null) ? feedingTarget.Key.TemplateId : -1;
					CarrierItem config = Carrier.Instance[templateId];
					bool love = config != null && config.LoveFoodType.Contains(CS$<>8__locals1.itemData.Key.TemplateId);
					bool hate = config != null && config.HateFoodType.Contains(CS$<>8__locals1.itemData.Key.TemplateId);
					CommonTableRowForItem.FavoriteStatus status = love ? CommonTableRowForItem.FavoriteStatus.Love : (hate ? CommonTableRowForItem.FavoriteStatus.Hate : CommonTableRowForItem.FavoriteStatus.None);
					CS$<>8__locals1.view.SetFavoriteStatus(status);
				}
				break;
			}
			}
			bool flag5 = this.MaxSelectCount > 0 && this.TotalSelectedCount >= this.MaxSelectCount;
			if (flag5)
			{
				CS$<>8__locals1.view.SetInteractable(false);
				CS$<>8__locals1.view.SetItemNotCanSelectReason(LocalStringManager.Get(LanguageKey.LK_ItemSelectCountIsMax).SetColor("brightred"));
			}
			CS$<>8__locals1.view.IsLocked = !CS$<>8__locals1.view.Interactable;
			bool interactable2 = CS$<>8__locals1.view.Interactable;
			if (interactable2)
			{
				CS$<>8__locals1.view.HideInteractionState();
			}
		}
		CS$<>8__locals1.toolList = (CS$<>8__locals1.isSelected ? usedToolList : availableToolList);
		CS$<>8__locals1.view.SetClickEvent(delegate
		{
			CS$<>8__locals1.<>4__this.HideGradeLimitTip();
			bool isSelected2 = CS$<>8__locals1.isSelected;
			if (isSelected2)
			{
				CS$<>8__locals1.<>4__this.SetItemSelectCount(CS$<>8__locals1.itemData.RealKey, 0, CS$<>8__locals1.toolList, true);
			}
			else
			{
				int limitCount;
				string limitTip;
				CS$<>8__locals1.<>4__this.PrepareSelect(CS$<>8__locals1.itemData, CS$<>8__locals1.toolList, out limitCount, out limitTip);
				bool flag6 = limitCount <= 0;
				if (!flag6)
				{
					CS$<>8__locals1.<>4__this.CurMultiplyScrollView.HandleClickItem(CS$<>8__locals1.itemData, CS$<>8__locals1.view, delegate(CommonTableRowForItem v)
					{
						CS$<>8__locals1.<OnRenderItemMultiply>g__Click|2(v, limitCount, limitTip, CS$<>8__locals1.toolList);
					});
				}
			}
		});
	}

	// Token: 0x0600247E RID: 9342 RVA: 0x0010BDAC File Offset: 0x00109FAC
	private bool CheckInteractableWithTool(ItemDisplayData itemData, out sbyte itemLifeSkillType, out short required, out bool hasAvailableTool, ref List<ItemDisplayData> availableToolList)
	{
		short attainment;
		bool useEmptyTool = this.CheckUseEmptyTool(itemData, out itemLifeSkillType, out required, out attainment);
		hasAvailableTool = false;
		bool flag = !useEmptyTool;
		if (flag)
		{
			availableToolList = this.GetAvailableToolList(itemData);
			hasAvailableTool = (availableToolList != null && availableToolList.Count > 0);
			bool flag2 = hasAvailableTool;
			if (flag2)
			{
				ItemDisplayData tool = availableToolList.First<ItemDisplayData>();
				attainment += UI_Make.GetToolAttainment(tool.Key.TemplateId, itemLifeSkillType);
				attainment = Math.Max(0, attainment);
			}
		}
		bool useNormalTool = hasAvailableTool && attainment >= required;
		return useEmptyTool || useNormalTool;
	}

	// Token: 0x0600247F RID: 9343 RVA: 0x0010BE48 File Offset: 0x0010A048
	private bool CheckUseEmptyTool(ItemDisplayData itemData, out sbyte itemLifeSkillType, out short required, out short attainment)
	{
		itemLifeSkillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(itemData.Key.ItemType, itemData.Key.TemplateId);
		required = ((this._currItemOperation == ItemOperationType.EItemOperationType.Repair) ? ItemTemplateHelper.GetRepairRequiredAttainment(itemData.Key.ItemType, itemData.Key.TemplateId, itemData.Durability) : ItemTemplateHelper.GetDisassembleRequiredAttainment(itemData.Key.ItemType, itemData.Key.TemplateId));
		attainment = this._lifeSkillMonitor.Attainments.GetOrDefault((int)itemLifeSkillType);
		short emptyToolAttainment = UI_Make.GetFinalAttainment(-1, attainment, itemLifeSkillType);
		return emptyToolAttainment >= required;
	}

	// Token: 0x06002480 RID: 9344 RVA: 0x0010BEEC File Offset: 0x0010A0EC
	private bool CheckInteractable(ItemDisplayData itemData, out List<ItemDisplayData> toolList)
	{
		int selectedCount;
		this.SelectedMultiplyItemDict.TryGetValue(itemData, out selectedCount);
		bool isSelected = selectedCount > 0;
		bool flag = isSelected;
		bool result;
		if (flag)
		{
			bool isResource = itemData.IsResource;
			if (isResource)
			{
				toolList = null;
			}
			else
			{
				Dictionary<ItemDisplayData, short> usedToolDict = this._multiplyOperationHandler.GetUsedToolDict(itemData);
				List<ItemDisplayData> usedToolList = (usedToolDict.Count > 0) ? new List<ItemDisplayData>(usedToolDict.Keys) : null;
				toolList = usedToolList;
			}
			result = (itemData.Amount > 0);
		}
		else
		{
			toolList = null;
			bool interactable = this.MaxSelectCount <= 0 || this.TotalSelectedCount < this.MaxSelectCount;
			bool flag2 = interactable;
			if (flag2)
			{
				switch (this._currItemOperation)
				{
				case ItemOperationType.EItemOperationType.Repair:
				case ItemOperationType.EItemOperationType.Disassemble:
				{
					List<ItemDisplayData> availableToolList = null;
					sbyte b;
					short num;
					bool flag3;
					bool interactableWithTool = this.CheckInteractableWithTool(itemData, out b, out num, out flag3, ref availableToolList);
					toolList = availableToolList;
					return interactableWithTool;
				}
				case ItemOperationType.EItemOperationType.Transfer:
					return true;
				case ItemOperationType.EItemOperationType.Discard:
					return true;
				case ItemOperationType.EItemOperationType.SpecialBreakConvertToExp:
					return true;
				case ItemOperationType.EItemOperationType.Feeding:
					return !this._isFeedDurabilityMax || !this._isFeedTameMax;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x06002481 RID: 9345 RVA: 0x0010C00C File Offset: 0x0010A20C
	private void PrepareSelect(ItemDisplayData itemData, List<ItemDisplayData> availableToolList, out int limitCount, out string limitTip)
	{
		limitTip = string.Empty;
		limitCount = itemData.Amount;
		switch (this._currItemOperation)
		{
		case ItemOperationType.EItemOperationType.Repair:
		case ItemOperationType.EItemOperationType.Disassemble:
		{
			sbyte b;
			short num;
			short num2;
			bool useEmptyTool = this.CheckUseEmptyTool(itemData, out b, out num, out num2);
			bool flag = !useEmptyTool;
			if (flag)
			{
				sbyte grade = ItemTemplateHelper.GetGrade(itemData.Key.ItemType, itemData.Key.TemplateId);
				bool flag2 = availableToolList == null;
				if (flag2)
				{
					limitCount = 0;
				}
				else
				{
					bool hasZero = availableToolList.Exists(delegate(ItemDisplayData d)
					{
						CraftToolItem toolConfig = CraftTool.Instance[d.Key.TemplateId];
						short cost = toolConfig.DurabilityCost[(int)grade];
						return cost == 0;
					});
					bool flag3 = !hasZero;
					if (flag3)
					{
						limitCount = availableToolList.Sum(delegate(ItemDisplayData d)
						{
							short durability = this._multiplyOperationHandler.GetToolRemainDurability(d);
							CraftToolItem toolConfig = CraftTool.Instance[d.Key.TemplateId];
							short cost = toolConfig.DurabilityCost[(int)grade];
							return (int)((durability % cost == 0) ? (durability / cost) : (durability / cost + 1));
						});
					}
				}
				limitTip = LocalStringManager.Get(LanguageKey.LK_SelectCount_Limit_Tool);
			}
			break;
		}
		case ItemOperationType.EItemOperationType.Transfer:
		{
			bool flag4 = UIManager.Instance.IsFocusElement(UIElement.EventWindow);
			if (flag4)
			{
				limitCount = this.MaxSelectCount - this.TotalSelectedCount;
				limitTip = LocalStringManager.Get(LanguageKey.LK_ItemSelectCountIsMax).SetColor("brightred");
			}
			break;
		}
		case ItemOperationType.EItemOperationType.Take:
			limitCount = ((itemData.Value == 0L) ? 0 : Convert.ToInt32((this.MaxWorthCanBeLentToTaiwu - this.SelectedMultiplyItemTotalWorth) / itemData.Value));
			limitTip = LocalStringManager.Get(LanguageKey.LK_SelectCount_Limit_Take);
			break;
		case ItemOperationType.EItemOperationType.Feeding:
		{
			int leftDurability;
			int leftTame;
			this.RefreshFeedState(out leftDurability, out leftTame);
			int addDurability = GameData.Domains.Extra.SharedMethods.GetFoodAddCarrierDurability(this._feedingTarget.Key.TemplateId, itemData.Key.TemplateId, 1);
			int durabilityCount = (addDurability <= 0) ? itemData.Amount : Mathf.CeilToInt((float)leftDurability / (float)addDurability);
			int tameCount = 0;
			bool flag5 = ItemTemplateHelper.HasCarrierTame(this._feedingTarget.Key.ItemType, this._feedingTarget.Key.TemplateId);
			if (flag5)
			{
				int addTame = GameData.Domains.Extra.SharedMethods.GetFoodAddCarrierTamePoint(this._feedingTarget.Key.TemplateId, itemData.Key.TemplateId, 1);
				bool flag6 = addTame > 0;
				if (flag6)
				{
					tameCount = Mathf.CeilToInt((float)leftTame / (float)addTame);
				}
			}
			limitCount = Mathf.Max(durabilityCount, tameCount);
			limitTip = LocalStringManager.Get(LanguageKey.LK_SelectCount_Limit_Feeding);
			break;
		}
		case ItemOperationType.EItemOperationType.Confiscate:
		{
			bool flag7 = UIManager.Instance.IsFocusElement(UIElement.EventWindow);
			if (flag7)
			{
				bool isMiscResource = ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
				bool flag8 = !isMiscResource;
				if (flag8)
				{
					limitCount = this.MaxSelectCount - this.TotalSelectedCount;
					limitTip = LocalStringManager.Get(LanguageKey.LK_SelectCount_Limit_Confiscate);
				}
			}
			break;
		}
		}
	}

	// Token: 0x06002482 RID: 9346 RVA: 0x0010C2B4 File Offset: 0x0010A4B4
	public void OnRenderItemExchangeTools(ItemDisplayData itemData, CommonTableRowForItem view)
	{
		MultiplyItemScrollView2.<>c__DisplayClass95_0 CS$<>8__locals1 = new MultiplyItemScrollView2.<>c__DisplayClass95_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.itemData = itemData;
		CS$<>8__locals1.view = view;
		this.SelectedMultiplyItemDict.TryGetValue(CS$<>8__locals1.itemData, out CS$<>8__locals1.selectedCount);
		CS$<>8__locals1.isSelected = (CS$<>8__locals1.selectedCount > 0);
		CS$<>8__locals1.view.SetSelectState(CS$<>8__locals1.isSelected);
		bool isSelected = CS$<>8__locals1.isSelected;
		if (isSelected)
		{
			int index = this.GetSelectedOrder(CS$<>8__locals1.itemData);
			CS$<>8__locals1.view.HideInteractionState();
			bool flag = this.TotalSelectedCount <= 0;
			if (flag)
			{
				CS$<>8__locals1.view.SetInteractable(true);
			}
		}
		else
		{
			bool flag2 = this.TotalSelectedCount > 0;
			if (flag2)
			{
				bool flag3 = !CS$<>8__locals1.itemData.Key.TemplateEquals(this.GetAlreadySelectItem());
				if (flag3)
				{
					CS$<>8__locals1.view.SetInteractable(false);
					CS$<>8__locals1.view.SetInteractionStateLockText(LocalStringManager.Get(LanguageKey.LK_Item_Operation_Locked));
				}
			}
			bool flag4 = this.TotalSelectedCount >= this.MaxSelectCount && this.MaxSelectCount > 0;
			if (flag4)
			{
				CS$<>8__locals1.view.SetInteractable(false);
			}
		}
		CS$<>8__locals1.view.SetClickEvent(delegate
		{
			MultiplyItemScrollView2.<>c__DisplayClass95_1 CS$<>8__locals2 = new MultiplyItemScrollView2.<>c__DisplayClass95_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			CS$<>8__locals1.<>4__this.HideGradeLimitTip();
			bool isSelected2 = CS$<>8__locals1.isSelected;
			if (isSelected2)
			{
				CS$<>8__locals1.<>4__this.SetItemSelectCount(CS$<>8__locals1.itemData.RealKey, 0, null, true);
			}
			else
			{
				CS$<>8__locals2.limitTip = string.Empty;
				CS$<>8__locals2.limitCount = CS$<>8__locals1.<>4__this.MaxSelectCount - CS$<>8__locals1.<>4__this.TotalSelectedCount;
				ItemOperationType.EItemOperationType currItemOperation = CS$<>8__locals1.<>4__this._currItemOperation;
				ItemOperationType.EItemOperationType eitemOperationType = currItemOperation;
				if (eitemOperationType == ItemOperationType.EItemOperationType.Confiscate)
				{
					bool flag5 = UIManager.Instance.IsFocusElement(UIElement.EventWindow);
					if (flag5)
					{
						bool isMiscResource = ItemTemplateHelper.IsMiscResource(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
						bool flag6 = !isMiscResource;
						if (flag6)
						{
							CS$<>8__locals2.limitCount = CS$<>8__locals1.<>4__this.MaxSelectCount - CS$<>8__locals1.<>4__this.TotalSelectedCount;
							CS$<>8__locals2.limitTip = LocalStringManager.Get(LanguageKey.LK_SelectCount_Limit_Confiscate);
						}
					}
				}
				bool flag7 = CS$<>8__locals2.limitCount <= 0;
				if (!flag7)
				{
					CS$<>8__locals1.<>4__this.CurMultiplyScrollView.HandleClickItem(CS$<>8__locals1.itemData, CS$<>8__locals1.view, new Action<CommonTableRowForItem>(CS$<>8__locals2.<OnRenderItemExchangeTools>g__Action|1));
				}
			}
		});
	}

	// Token: 0x06002483 RID: 9347 RVA: 0x0010C400 File Offset: 0x0010A600
	public ItemKey GetAlreadySelectItem()
	{
		using (Dictionary<ItemDisplayData, int>.Enumerator enumerator = this.SelectedMultiplyItemDict.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				KeyValuePair<ItemDisplayData, int> pair = enumerator.Current;
				return pair.Key.Key;
			}
		}
		return ItemKey.Invalid;
	}

	// Token: 0x06002484 RID: 9348 RVA: 0x0010C468 File Offset: 0x0010A668
	public int GetAlreadySelectItemCount()
	{
		return this.SelectedMultiplyItemDict.Keys.Count;
	}

	// Token: 0x06002485 RID: 9349 RVA: 0x0010C48C File Offset: 0x0010A68C
	public int GetSelectedOrder(ItemDisplayData itemData)
	{
		return this.SelectedMultiplyItemOrderedList.IndexOf(itemData);
	}

	// Token: 0x06002486 RID: 9350 RVA: 0x0010C4AA File Offset: 0x0010A6AA
	public void SetFilter(Func<ItemDisplayData, bool> filter)
	{
		this._filter = filter;
	}

	// Token: 0x06002487 RID: 9351 RVA: 0x0010C4B4 File Offset: 0x0010A6B4
	private bool SetItemSelectCount(CommonTableRowForItem itemView, int count, List<ItemDisplayData> toolList)
	{
		bool selectIsSuccess = this.SetItemSelectCount(itemView.Data.RealKey, count, toolList, true);
		ItemDisplayData.ItemUsingType usingType;
		bool flag = this.IsMultiItemSelect && selectIsSuccess && count > 0 && this.CheckNeedNotification(itemView.Data, out usingType);
		if (flag)
		{
			this.ShowGradeLimitTip(itemView, this.GetNotificationTextByUsingType(usingType));
		}
		else
		{
			this.HideGradeLimitTip();
		}
		return selectIsSuccess;
	}

	// Token: 0x06002488 RID: 9352 RVA: 0x0010C518 File Offset: 0x0010A718
	private bool SetItemSelectCount(ItemKey itemKey, int count, List<ItemDisplayData> toolList, bool isFinished = true)
	{
		ItemDisplayData data = this.CurMultiplyItems.Find((ItemDisplayData d) => d.RealKey.Equals(itemKey));
		int lastCount;
		bool flag = this.SelectedMultiplyItemDict.TryGetValue(data, out lastCount) && count == lastCount;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			int index = this.SelectedMultiplyItemOrderedList.IndexOf(data);
			bool flag2 = this.SelectedMultiplyItemOrderedList.CheckIndex(index);
			if (flag2)
			{
				this.SelectedMultiplyItemOrderedList[index] = data;
				this.SelectedMultiplyItemDict[data] = count;
				bool flag3 = count <= 0;
				if (flag3)
				{
					this.SelectedMultiplyItemDict.Remove(data);
					this.SelectedMultiplyItemOrderedList.Remove(data);
				}
			}
			else
			{
				bool flag4 = count <= 0;
				if (flag4)
				{
					return false;
				}
				this.SelectedMultiplyItemOrderedList.Add(data);
				this.SelectedMultiplyItemDict.Add(data, count);
			}
			sbyte grade = ItemTemplateHelper.GetGrade(data.Key.ItemType, data.Key.TemplateId);
			Dictionary<ItemDisplayData, short> usedToolDict = this._multiplyOperationHandler.GetUsedToolDict(data);
			usedToolDict.Clear();
			bool flag5 = toolList != null;
			if (flag5)
			{
				int needCount = count;
				foreach (ItemDisplayData tool in toolList)
				{
					bool flag6 = needCount <= 0;
					if (flag6)
					{
						break;
					}
					CraftToolItem toolConfig = CraftTool.Instance[tool.Key.TemplateId];
					short durabilityCost = toolConfig.DurabilityCost[(int)grade];
					short remainDurability = this._multiplyOperationHandler.GetToolRemainDurability(tool);
					bool flag7 = remainDurability <= 0;
					if (!flag7)
					{
						bool flag8 = durabilityCost == 0;
						int meetCount;
						if (flag8)
						{
							meetCount = needCount;
						}
						else
						{
							meetCount = (int)((remainDurability % durabilityCost == 0) ? (remainDurability / durabilityCost) : (remainDurability / durabilityCost + 1));
							meetCount = Math.Min(needCount, meetCount);
						}
						int used = Math.Min(meetCount * (int)durabilityCost, (int)remainDurability);
						usedToolDict[tool] = (short)used;
						needCount -= meetCount;
					}
				}
			}
			this._multiplyOperationHandler.RefreshAllUsedToolDict();
			this.RefreshMultiplyAvailableTool();
			this.RefreshSelectedCount();
			if (isFinished)
			{
				int num;
				int num2;
				this.RefreshFeedState(out num, out num2);
				List<ValueTuple<ItemDisplayData, int>> changeList = new List<ValueTuple<ItemDisplayData, int>>
				{
					new ValueTuple<ItemDisplayData, int>(data, count)
				};
				this.SendItemMultiplyOperationContentChange(changeList);
				this.RefreshMultiplyCanOperateItems();
			}
			result = true;
		}
		return result;
	}

	// Token: 0x06002489 RID: 9353 RVA: 0x0010C7A0 File Offset: 0x0010A9A0
	public void SelectItem(List<ValueTuple<ItemDisplayData, int>> selectionList)
	{
		this.ClearMultiplySelection();
		foreach (ValueTuple<ItemDisplayData, int> valueTuple in selectionList)
		{
			ItemDisplayData data = valueTuple.Item1;
			int count = valueTuple.Item2;
			int index = this.SelectedMultiplyItemOrderedList.IndexOf(data);
			bool flag = this.SelectedMultiplyItemOrderedList.CheckIndex(index);
			if (flag)
			{
				this.SelectedMultiplyItemOrderedList[index] = data;
				this.SelectedMultiplyItemDict[data] = count;
			}
			else
			{
				this.SelectedMultiplyItemOrderedList.Add(data);
				this.SelectedMultiplyItemDict.Add(data, count);
			}
		}
		this.RefreshSelectedCount();
		this.SendItemMultiplyOperationContentChange(null);
		this.RefreshMultiplyCanOperateItems();
	}

	// Token: 0x0600248A RID: 9354 RVA: 0x0010C874 File Offset: 0x0010AA74
	private void RefreshSelectedCount()
	{
		this.TotalSelectedCount = this.SelectedMultiplyItemDict.Sum(delegate(KeyValuePair<ItemDisplayData, int> p)
		{
			ItemDisplayData data = p.Key;
			return ItemTemplateHelper.IsMiscResource(data.Key.ItemType, data.Key.TemplateId) ? 1 : p.Value;
		});
	}

	// Token: 0x0600248B RID: 9355 RVA: 0x0010C8A8 File Offset: 0x0010AAA8
	private void SendItemMultiplyOperationContentChange([TupleElementNames(new string[]
	{
		"itemKey",
		"count"
	})] List<ValueTuple<ItemDisplayData, int>> changeList = null)
	{
		bool flag = this._multiplyOperationHandler == null;
		if (!flag)
		{
			ArgumentBox args = this.GetItemMultiplyViewArgs();
			GEvent.OnEvent(UiEvents.ItemMultiplyOperationContentChange, args);
			Action<List<ValueTuple<ItemDisplayData, int>>> onContentChange = this._onContentChange;
			if (onContentChange != null)
			{
				onContentChange(changeList);
			}
		}
	}

	// Token: 0x0600248C RID: 9356 RVA: 0x0010C8F0 File Offset: 0x0010AAF0
	private void RefreshFeedState(out int leftDurability, out int leftTame)
	{
		leftDurability = 0;
		leftTame = 0;
		bool flag = this._multiplyItemOperationType != ItemOperationType.EItemOperationType.Feeding || this._feedingTarget == null;
		if (!flag)
		{
			int addDurability = this.SelectedMultiplyItemDict.Sum((KeyValuePair<ItemDisplayData, int> pair) => GameData.Domains.Extra.SharedMethods.GetFoodAddCarrierDurability(this._feedingTarget.Key.TemplateId, pair.Key.Key.TemplateId, pair.Value));
			int curDurability = Mathf.Clamp(addDurability + (int)this._feedingTarget.Durability, 0, (int)this._feedingTarget.MaxDurability);
			this._isFeedDurabilityMax = (curDurability >= (int)this._feedingTarget.MaxDurability);
			leftDurability = (int)this._feedingTarget.MaxDurability - curDurability;
			bool flag2 = ItemTemplateHelper.HasCarrierTame(this._feedingTarget.Key.ItemType, this._feedingTarget.Key.TemplateId);
			if (flag2)
			{
				int addTame = this.SelectedMultiplyItemDict.Sum((KeyValuePair<ItemDisplayData, int> pair) => GameData.Domains.Extra.SharedMethods.GetFoodAddCarrierTamePoint(this._feedingTarget.Key.TemplateId, pair.Key.Key.TemplateId, pair.Value));
				int curTame = Mathf.Clamp(addTame + this._feedingTarget.CarrierTamePoint, 0, GlobalConfig.Instance.MaxCarrierTamePoint);
				this._isFeedTameMax = (curTame >= GlobalConfig.Instance.MaxCarrierTamePoint);
				leftTame = GlobalConfig.Instance.MaxCarrierTamePoint - curTame;
			}
			else
			{
				this._isFeedTameMax = true;
			}
		}
	}

	// Token: 0x0600248D RID: 9357 RVA: 0x0010CA14 File Offset: 0x0010AC14
	public void RefreshMultiplyAvailableTool()
	{
		List<ItemDisplayData> itemList = EasyPool.Get<List<ItemDisplayData>>();
		itemList.AddRange(this.CurInventoryItems);
		bool flag = this.CurWarehouseItems != null;
		if (flag)
		{
			itemList.AddRange(this.CurWarehouseItems);
		}
		IOrderedEnumerable<ItemDisplayData> toolList = from d in itemList
		where d.Key.ItemType == 6 && d.Durability > 0 && !this.SelectedMultiplyItemDict.ContainsKey(d)
		orderby ItemTemplateHelper.GetGrade(d.Key.ItemType, d.Key.TemplateId)
		select d;
		this.UsableToolOrderedList.Clear();
		this.UsableToolOrderedList.AddRange(toolList);
		EasyPool.Free<List<ItemDisplayData>>(itemList);
	}

	// Token: 0x0600248E RID: 9358 RVA: 0x0010CAA9 File Offset: 0x0010ACA9
	private void OnClickMultiplyFeed()
	{
	}

	// Token: 0x0600248F RID: 9359 RVA: 0x0010CAAC File Offset: 0x0010ACAC
	private void OnClickMultiplyDiscard()
	{
		bool hasLimitItem = this.CheckAllSelectedItemsHasAnyLimit();
		bool flag = hasLimitItem;
		if (flag)
		{
			DialogCmd dialogCmd = new DialogCmd();
			dialogCmd.Type = 1;
			dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
			dialogCmd.Content = LocalStringManager.GetFormat(LanguageKey.LK_Item_DiscardWarning_Multiply, Array.Empty<object>());
			dialogCmd.Yes = new Action(this.MultiplyDiscard);
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}
		else
		{
			this.MultiplyDiscard();
		}
	}

	// Token: 0x06002490 RID: 9360 RVA: 0x0010CB42 File Offset: 0x0010AD42
	private void MultiplyDiscard()
	{
	}

	// Token: 0x06002491 RID: 9361 RVA: 0x0010CB48 File Offset: 0x0010AD48
	private void OnClickMultiplyDisassemble()
	{
		bool hasLimitItem = this.CheckAllSelectedItemsHasAnyLimit();
		bool flag = hasLimitItem;
		if (flag)
		{
			DialogCmd dialogCmd = new DialogCmd();
			dialogCmd.Type = 1;
			dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
			dialogCmd.Content = LocalStringManager.GetFormat(LanguageKey.LK_Item_DisassembleWarning_Multiply, Array.Empty<object>());
			dialogCmd.Yes = new Action(this.MultiplyDisassemble);
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}
		else
		{
			this.MultiplyDisassemble();
		}
	}

	// Token: 0x06002492 RID: 9362 RVA: 0x0010CBE0 File Offset: 0x0010ADE0
	private void MultiplyDisassemble()
	{
		List<MultiplyOperation> operationList = this._multiplyOperationHandler.GetOperationList(false);
		this.ClearMultiplyItemUsingState(operationList);
		this.RefreshUsedTool();
		ItemDomainMethod.AsyncCall.DisassembleItemList(null, MultiplyItemScrollView2.TaiwuCharId, operationList, delegate(int offset, RawDataPool dataPool)
		{
			List<ItemDisplayData> resultList = EasyPool.Get<List<ItemDisplayData>>();
			resultList.Clear();
			Serializer.Deserialize(dataPool, offset, ref resultList);
			Dictionary<ItemDisplayData, short> allUsedToolDict = this._multiplyOperationHandler.AllUsedToolDict;
			foreach (KeyValuePair<ItemDisplayData, short> pair in allUsedToolDict)
			{
				ItemDisplayData key = pair.Key;
				key.Durability -= pair.Value;
			}
			bool flag = resultList != null && resultList.Count > 0;
			if (flag)
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Clear();
				argBox.SetObject("ItemList", resultList);
				argBox.Set("ObtainType", 8);
				List<ItemDisplayData> usedToolList = allUsedToolDict.Keys.ToList<ItemDisplayData>();
				usedToolList.RemoveAll((ItemDisplayData data) => data.Durability > 0);
				argBox.SetObject("UsedToolList", usedToolList);
				UIElement.GetItem.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.GetItem);
				Action onGetItemHide = null;
				onGetItemHide = delegate()
				{
					EasyPool.Free<List<ItemDisplayData>>(resultList);
					EasyPool.Free<ArgumentBox>(argBox);
					UIElement getItem2 = UIElement.GetItem;
					getItem2.OnHide = (Action)Delegate.Remove(getItem2.OnHide, onGetItemHide);
				};
				UIElement getItem = UIElement.GetItem;
				getItem.OnHide = (Action)Delegate.Combine(getItem.OnHide, onGetItemHide);
				using (List<ItemDisplayData>.Enumerator enumerator2 = resultList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ItemDisplayData data = enumerator2.Current;
						ItemDisplayData old = this.CurMultiplyItems.Find((ItemDisplayData d) => d.Key.TemplateEquals(data.Key));
						bool flag2 = old == null;
						if (flag2)
						{
							this.CurMultiplyItems.Add(data);
						}
						else
						{
							old.Amount += data.Amount;
						}
					}
				}
			}
			this.RefreshUsedTool();
			this.RefreshItems();
		});
	}

	// Token: 0x06002493 RID: 9363 RVA: 0x0010CC23 File Offset: 0x0010AE23
	private void OnClickMultiplyRepair()
	{
		this.MultiplyRepair();
	}

	// Token: 0x06002494 RID: 9364 RVA: 0x0010CC30 File Offset: 0x0010AE30
	private void MultiplyRepair()
	{
		List<MultiplyOperation> operationList = this._multiplyOperationHandler.GetOperationList(false);
		ItemDisplayData data;
		using (List<MultiplyOperation>.Enumerator enumerator = operationList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				MultiplyOperation operation = enumerator.Current;
				data = this.SelectedMultiplyItemOrderedList.Find((ItemDisplayData d) => d.ContainsItemKey(operation.Target));
				data.Durability = data.MaxDurability;
			}
		}
		BuildingDomainMethod.AsyncCall.RepairItemList(null, MultiplyItemScrollView2.TaiwuCharId, operationList, delegate(int offset, RawDataPool dataPool)
		{
			List<ItemDisplayData> dataList = EasyPool.Get<List<ItemDisplayData>>();
			dataList.Clear();
			Serializer.Deserialize(dataPool, offset, ref dataList);
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Clear();
			argBox.SetObject("ItemList", dataList);
			argBox.Set("ObtainType", 7);
			Dictionary<ItemDisplayData, short> allUsedToolDict = this._multiplyOperationHandler.AllUsedToolDict;
			List<ItemDisplayData> toolList = allUsedToolDict.Keys.ToList<ItemDisplayData>();
			foreach (KeyValuePair<ItemDisplayData, short> pair in allUsedToolDict)
			{
				ItemDisplayData key = pair.Key;
				key.Durability -= pair.Value;
			}
			toolList.RemoveAll((ItemDisplayData data) => data.Durability > 0);
			argBox.SetObject("UsedToolList", toolList);
			UIElement.GetItem.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.GetItem);
			Action onGetItemHide = null;
			onGetItemHide = delegate()
			{
				EasyPool.Free<List<ItemDisplayData>>(dataList);
				EasyPool.Free<ArgumentBox>(argBox);
				UIElement getItem2 = UIElement.GetItem;
				getItem2.OnHide = (Action)Delegate.Remove(getItem2.OnHide, onGetItemHide);
			};
			UIElement getItem = UIElement.GetItem;
			getItem.OnHide = (Action)Delegate.Combine(getItem.OnHide, onGetItemHide);
			this.RefreshUsedTool();
			this.RefreshItems();
		});
	}

	// Token: 0x06002495 RID: 9365 RVA: 0x0010CCD4 File Offset: 0x0010AED4
	private void OnClickMultiplyPutPoisonMaterial()
	{
	}

	// Token: 0x06002496 RID: 9366 RVA: 0x0010CCD8 File Offset: 0x0010AED8
	private void OnClickMultiplyVillagerCraft()
	{
		ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("SelectedMultiplyItemDict", this.SelectedMultiplyItemDict);
		GEvent.OnEvent(UiEvents.OnConfirmVillagerCraftInputMaterial, args);
	}

	// Token: 0x06002497 RID: 9367 RVA: 0x0010CD0D File Offset: 0x0010AF0D
	private void OnClickMultiplyTransfer()
	{
	}

	// Token: 0x06002498 RID: 9368 RVA: 0x0010CD10 File Offset: 0x0010AF10
	private void EnterMultiplyMode()
	{
		bool isMultiItemSelect = this.IsMultiItemSelect;
		if (!isMultiItemSelect)
		{
			this.IsMultiItemSelect = true;
			bool flag = this._multiplyItemOperationType == ItemOperationType.EItemOperationType.Invalid;
			if (flag)
			{
				this._multiplyItemOperationType = ItemOperationType.EItemOperationType.Disassemble;
			}
			this._currItemOperation = this._multiplyItemOperationType;
			this.SwitchSelection.gameObject.SetActive(true);
			this.SwitchSelection.transform.parent.parent.gameObject.SetActive(true);
			this.ToggleSelectAll.gameObject.SetActive(true);
			this.ClearMultiplySelection();
			this.HideMultiplySelectButton();
			this.RefreshMultiplyAvailableTool();
			this.RefreshMultiplyCanOperateItems();
			this.RefreshSort();
			this.ShowItemMultiplyOperationPanel();
			Action onEnterMultiplyOperation = this._onEnterMultiplyOperation;
			if (onEnterMultiplyOperation != null)
			{
				onEnterMultiplyOperation();
			}
		}
	}

	// Token: 0x06002499 RID: 9369 RVA: 0x0010CDD8 File Offset: 0x0010AFD8
	public void ShowItemMultiplyOperationPanel()
	{
		ArgumentBox args = this.GetItemMultiplyViewArgs();
		this.ItemMultiplyOperationPanel.Show(args);
	}

	// Token: 0x0600249A RID: 9370 RVA: 0x0010CDFC File Offset: 0x0010AFFC
	public void EnterSpecialBreakConvertToExpMultiplyMode()
	{
		bool isMultiItemSelect = this.IsMultiItemSelect;
		if (!isMultiItemSelect)
		{
			this.IsMultiItemSelect = true;
			this._currItemOperation = (this._multiplyItemOperationType = ItemOperationType.EItemOperationType.SpecialBreakConvertToExp);
			this.RefreshMultiplyCanOperateItems();
			this.ClearMultiplySelection();
		}
	}

	// Token: 0x0600249B RID: 9371 RVA: 0x0010CE3C File Offset: 0x0010B03C
	public void EnterFeedingMode(ItemDisplayData feedingTarget)
	{
		this._multiplyItemOperationType = ItemOperationType.EItemOperationType.Feeding;
		this._feedingTargetOnEnter = feedingTarget;
		this._feedingTarget = feedingTarget;
		this._feedingTargetList.Clear();
		int num;
		int num2;
		this.RefreshFeedState(out num, out num2);
		this.EnterMultiplyMode();
	}

	// Token: 0x0600249C RID: 9372 RVA: 0x0010CE7D File Offset: 0x0010B07D
	public void EnterRepairMode()
	{
		this._multiplyItemOperationType = ItemOperationType.EItemOperationType.Repair;
		this.EnterMultiplyMode();
	}

	// Token: 0x0600249D RID: 9373 RVA: 0x0010CE8E File Offset: 0x0010B08E
	public void EnterDisassembleMode()
	{
		this._multiplyItemOperationType = ItemOperationType.EItemOperationType.Disassemble;
		this.EnterMultiplyMode();
	}

	// Token: 0x0600249E RID: 9374 RVA: 0x0010CEA0 File Offset: 0x0010B0A0
	public void EnterPutPoisonMaterialMode()
	{
		bool isMultiItemSelect = this.IsMultiItemSelect;
		if (!isMultiItemSelect)
		{
			this.IsMultiItemSelect = true;
			this._currItemOperation = (this._multiplyItemOperationType = ItemOperationType.EItemOperationType.PutPoisonMaterial);
			this.RefreshMultiplyCanOperateItems();
			this.ClearMultiplySelection();
		}
	}

	// Token: 0x0600249F RID: 9375 RVA: 0x0010CEE0 File Offset: 0x0010B0E0
	public void EnterVillagerCraftMode()
	{
		bool isMultiItemSelect = this.IsMultiItemSelect;
		if (!isMultiItemSelect)
		{
			this.IsMultiItemSelect = true;
			this._currItemOperation = (this._multiplyItemOperationType = ItemOperationType.EItemOperationType.VillagerCraft);
			this.RefreshMultiplyCanOperateItems();
			this.ClearMultiplySelection();
		}
	}

	// Token: 0x060024A0 RID: 9376 RVA: 0x0010CF24 File Offset: 0x0010B124
	public void EnterEventWindowMultiplyMode(sbyte operationType)
	{
		bool isMultiItemSelect = this.IsMultiItemSelect;
		if (!isMultiItemSelect)
		{
			this.IsMultiItemSelect = true;
			this._multiplyItemOperationType = (ItemOperationType.EItemOperationType)operationType;
			this._currItemOperation = (ItemOperationType.EItemOperationType)operationType;
			this.RefreshMultiplyCanOperateItems();
			this.ClearMultiplySelection();
		}
	}

	// Token: 0x060024A1 RID: 9377 RVA: 0x0010CF64 File Offset: 0x0010B164
	public void RefreshMultiplyCanOperateItems()
	{
		this._canOperateItems.Clear();
		List<ItemDisplayData> items = this.CurMultiplyItems;
		IEnumerable<ItemDisplayData> itemList = items.Where(delegate(ItemDisplayData d)
		{
			bool result;
			switch (this._currItemOperation)
			{
			case ItemOperationType.EItemOperationType.Invalid:
				result = false;
				break;
			case ItemOperationType.EItemOperationType.Repair:
			{
				bool canRepair = d.Durability < d.MaxDurability && ItemTemplateHelper.IsRepairable(d.Key.ItemType, d.Key.TemplateId);
				result = canRepair;
				break;
			}
			case ItemOperationType.EItemOperationType.Disassemble:
			{
				bool canDisassemble = ItemTemplateHelper.GetCanDisassemble(d.Key.ItemType, d.Key.TemplateId);
				result = canDisassemble;
				break;
			}
			case ItemOperationType.EItemOperationType.Transfer:
				result = this.CheckItemIsRealTransferable(d);
				break;
			case ItemOperationType.EItemOperationType.Take:
				result = this.CheckItemIsRealTransferable(d);
				break;
			case ItemOperationType.EItemOperationType.Discard:
				result = this.CheckItemIsRealTransferable(d);
				break;
			case ItemOperationType.EItemOperationType.SpecialBreakConvertToExp:
			{
				bool flag2 = d.Key.ItemType == 12;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !ItemTemplateHelper.IsTransferable(d.Key.ItemType, d.Key.TemplateId);
					result = !flag3;
				}
				break;
			}
			case ItemOperationType.EItemOperationType.Feeding:
				result = ItemTemplateHelper.IsFeedingAble(d.Key.ItemType, d.Key.TemplateId);
				break;
			case ItemOperationType.EItemOperationType.Confiscate:
				result = true;
				break;
			case ItemOperationType.EItemOperationType.PutPoisonMaterial:
			{
				bool flag4 = d.Key.ItemType == 5;
				if (flag4)
				{
					MaterialItem config = Config.Material.Instance[d.Key.TemplateId];
					result = config.InnatePoisons.IsNonZero();
				}
				else
				{
					bool flag5 = d.Key.ItemType == 4;
					if (flag5)
					{
						CarrierItem config2 = Carrier.Instance[d.Key.TemplateId];
						result = config2.InnatePoisons.IsNonZero();
					}
					else
					{
						result = false;
					}
				}
				break;
			}
			case ItemOperationType.EItemOperationType.ExchangeTools:
				result = true;
				break;
			case ItemOperationType.EItemOperationType.FixItem:
				result = true;
				break;
			case ItemOperationType.EItemOperationType.VillagerCraft:
				result = this._filter(d);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		});
		foreach (ItemDisplayData data in itemList)
		{
			data.Interactable = true;
		}
		bool isOn = this.SwitchSelection.isOn;
		if (isOn)
		{
			foreach (ItemDisplayData data2 in itemList)
			{
				int lastCount;
				this.SelectedMultiplyItemDict.TryGetValue(data2, out lastCount);
				int count = data2.Amount - lastCount;
				bool flag = count > 0;
				if (flag)
				{
					ItemDisplayData tempData = data2.Clone(count);
					this._canOperateItems.Add(tempData);
				}
			}
			this.CurMultiplyScrollView.SetItemList(ref this._canOperateItems);
			this._selectedItems.Clear();
			foreach (KeyValuePair<ItemDisplayData, int> keyValuePair in this.SelectedMultiplyItemDict)
			{
				ItemDisplayData itemDisplayData;
				int num;
				keyValuePair.Deconstruct(out itemDisplayData, out num);
				ItemDisplayData data3 = itemDisplayData;
				int count2 = num;
				ItemDisplayData tempData2 = data3.Clone(count2);
				this._selectedItems.Add(tempData2);
			}
			this._selectedItemScroll.SetItemList(ref this._selectedItems, false, null, null, null, true);
		}
		else
		{
			this._canOperateItems.AddRange(itemList);
			this.CurMultiplyScrollView.SetItemList(ref this._canOperateItems);
		}
		this.RefreshToggleSelectAll();
	}

	// Token: 0x060024A2 RID: 9378 RVA: 0x0010D134 File Offset: 0x0010B334
	private bool CheckItemIsRealTransferable(ItemDisplayData d)
	{
		bool flag = ItemTemplateHelper.IsTransferable(d.Key.ItemType, d.Key.TemplateId);
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			bool flag2 = ItemTemplateHelper.MiscResourceCanExchange(d.Key.ItemType, d.Key.TemplateId) && d.Amount > 0;
			result = flag2;
		}
		return result;
	}

	// Token: 0x060024A3 RID: 9379 RVA: 0x0010D19C File Offset: 0x0010B39C
	private ArgumentBox GetItemMultiplyViewArgs()
	{
		ArgumentBox args = EasyPool.Get<ArgumentBox>();
		args.SetObject("ItemDict", this.SelectedMultiplyItemDict);
		args.SetObject("ItemList", this.SelectedMultiplyItemOrderedList);
		args.SetObject("ToolDict", this._multiplyOperationHandler.AllUsedToolDict);
		args.Set("ItemOperationType", this._currItemOperation);
		args.Set("ItemSourceType", this.CurMultiplyItemSourceType);
		foreach (ItemDisplayData itemData in this.SelectedMultiplyItemOrderedList)
		{
			Dictionary<ItemDisplayData, short> usedToolDict = this._multiplyOperationHandler.GetUsedToolDict(itemData);
			bool flag = usedToolDict.Count == 0;
			if (flag)
			{
				args.Set("UseEmptyTool", true);
			}
		}
		ItemOperationType.EItemOperationType currItemOperation = this._currItemOperation;
		ItemOperationType.EItemOperationType eitemOperationType = currItemOperation;
		if (eitemOperationType != ItemOperationType.EItemOperationType.Repair)
		{
			if (eitemOperationType != ItemOperationType.EItemOperationType.Disassemble)
			{
				if (eitemOperationType == ItemOperationType.EItemOperationType.Feeding)
				{
					bool flag2 = this._feedingTargetList.Count == 0;
					if (flag2)
					{
						this._feedingTargetList.Clear();
						this._feedingTargetList.AddRange(this.CurMultiplyItems.Where(new Func<ItemDisplayData, bool>(UI_CharacterMenuItems.CheckNeedFeedCarrier)));
						this._feedingTargetList.Sort((ItemDisplayData a, ItemDisplayData b) => b.CarrierTamePoint.CompareTo(a.CarrierTamePoint));
						bool flag3 = this._feedingTargetOnEnter != null && UI_CharacterMenuItems.CheckNeedFeedCarrier(this._feedingTargetOnEnter);
						if (flag3)
						{
							bool flag4 = this._feedingTargetList.Remove(this._feedingTargetOnEnter);
							if (flag4)
							{
								this._feedingTargetList.Insert(0, this._feedingTargetOnEnter);
							}
						}
						else
						{
							ItemDisplayData target = this._feedingTargetList.FirstOrDefault<ItemDisplayData>();
							this.SetFeedingTarget(target);
						}
					}
					args.SetObject("FeedingTarget", this._feedingTarget);
					args.SetObject("FeedingTargetList", this._feedingTargetList);
				}
			}
			else
			{
				ResourceInts getResource = default(ResourceInts);
				List<short> certainMaterialList = new List<short>();
				List<short> chanceMaterialList = new List<short>();
				foreach (ItemDisplayData itemData2 in this.SelectedMultiplyItemOrderedList)
				{
					int amount = this.SelectedMultiplyItemDict[itemData2];
					ResourceInts resource = UI_CharacterMenuItems.GetDisassembleResources(itemData2, amount);
					getResource.Add(ref resource);
					short[] refinedList = itemData2.RefiningEffects.GetAllMaterialTemplateIds();
					foreach (short id in refinedList)
					{
						bool flag5 = id > -1;
						if (flag5)
						{
							certainMaterialList.Add(id);
						}
					}
					List<short> baseMaterial = ItemTemplateHelper.GetAllDisassemblyMaterial(itemData2.Key.ItemType, itemData2.Key.TemplateId);
					bool flag6 = baseMaterial != null;
					if (flag6)
					{
						chanceMaterialList.AddRange(baseMaterial);
					}
				}
				args.SetObject("GetResource", getResource);
				args.SetObject("CertainMaterial", certainMaterialList);
				args.SetObject("ChanceMaterial", chanceMaterialList);
			}
		}
		else
		{
			ResourceInts allNeedResource = default(ResourceInts);
			foreach (ItemDisplayData itemData3 in this.SelectedMultiplyItemOrderedList)
			{
				ResourceInts needResource = ItemTemplateHelper.GetRepairNeedResources(itemData3.MaterialResources, itemData3.Key, itemData3.Durability);
				allNeedResource.Add(ref needResource);
			}
			args.SetObject("NeedResource", allNeedResource);
			ResourceInts curResource = new ResourceInts(this._resourceMonitor.Resources);
			args.SetObject("Resource", curResource);
		}
		bool flag7 = UIManager.Instance.IsElementActive(UIElement.Warehouse);
		if (flag7)
		{
			RectTransform rectTrans = this.IsInventoryMultiply ? base.CGet<RectTransform>("InventoryMultiplyOperationPos") : base.CGet<RectTransform>("WarehouseMultiplyOperationPos");
			args.SetObject("Pos", rectTrans.position);
			args.Set("Height", rectTrans.rect.height);
		}
		return args;
	}

	// Token: 0x060024A4 RID: 9380 RVA: 0x0010D5F8 File Offset: 0x0010B7F8
	public void TryExitMultiplyMode(Action onConfirm)
	{
		bool flag = this.SelectedMultiplyItemDict.Count == 0;
		if (flag)
		{
			this.ExitMultiplyMode();
			Action onConfirm2 = onConfirm;
			if (onConfirm2 != null)
			{
				onConfirm2();
			}
		}
		else
		{
			DialogCmd dialogCmd = new DialogCmd();
			dialogCmd.Type = 1;
			dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
			dialogCmd.Content = LocalStringManager.GetFormat(LanguageKey.LK_Item_Exit_Multiply, Array.Empty<object>());
			dialogCmd.Yes = delegate()
			{
				this.ExitMultiplyMode();
				Action onConfirm3 = onConfirm;
				if (onConfirm3 != null)
				{
					onConfirm3();
				}
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}
	}

	// Token: 0x060024A5 RID: 9381 RVA: 0x0010D6B8 File Offset: 0x0010B8B8
	public void ExitMultiplyMode()
	{
		this.ShowMultiplySelectButton(null, true);
		bool flag = !this.IsMultiItemSelect;
		if (!flag)
		{
			this.IsMultiItemSelect = false;
			this.HideGradeLimitTip();
			this.SwitchSelection.isOn = false;
			this.SwitchSelection.gameObject.SetActive(false);
			this.SwitchSelection.transform.parent.parent.gameObject.SetActive(false);
			this.ToggleSelectAll.gameObject.SetActive(false);
			this._feedingTarget = null;
			this._feedingTargetOnEnter = null;
			this._feedingTargetList.Clear();
			this.ClearMultiplySelection();
			this._currItemOperation = (this._multiplyItemOperationType = ItemOperationType.EItemOperationType.Invalid);
			this.RefreshItems();
			this.RefreshSort();
			Action onExitMultiplyOperation = this._onExitMultiplyOperation;
			if (onExitMultiplyOperation != null)
			{
				onExitMultiplyOperation();
			}
			this.ItemMultiplyOperationPanel.Hide();
		}
	}

	// Token: 0x060024A6 RID: 9382 RVA: 0x0010D7A0 File Offset: 0x0010B9A0
	private void RefreshToggleSelectAll()
	{
		bool isAll = this.SwitchSelection.isOn ? ((this._canOperateItems.Count == 0 && this._selectedItems.Count > 0) || (this._canOperateItems.Count > 0 && this._canOperateItems.Any(delegate(ItemDisplayData d)
		{
			List<ItemDisplayData> list;
			return !this.CheckInteractable(d, out list);
		}))) : (this.SelectedMultiplyItemOrderedList.Count > 0 && this._canOperateItems.Where(delegate(ItemDisplayData d)
		{
			List<ItemDisplayData> list;
			return this.CheckInteractable(d, out list);
		}).All((ItemDisplayData d) => this.SelectedMultiplyItemOrderedList.Any((ItemDisplayData s) => s.RealKey.Equals(d.RealKey))));
		bool flag = isAll == this.ToggleSelectAll.isOn;
		if (!flag)
		{
			this.ToggleSelectAll.SetIsOnWithoutNotify(isAll);
			MonoJoint objOn = this.ToggleSelectAll.GetComponent<Refers>().CGet<MonoJoint>("On");
			objOn.gameObject.SetActive(isAll);
			objOn.JointSync();
		}
	}

	// Token: 0x060024A7 RID: 9383 RVA: 0x0010D888 File Offset: 0x0010BA88
	public void ClearMultiplySelection()
	{
		this.TotalSelectedCount = 0;
		this._multiplyOperationHandler.Clear();
		this.CurMultiplyScrollView.ReRender();
	}

	// Token: 0x060024A8 RID: 9384 RVA: 0x0010D8AB File Offset: 0x0010BAAB
	private void ClearMultiplyItemUsingState(List<MultiplyOperation> multiplyOperationList)
	{
	}

	// Token: 0x060024A9 RID: 9385 RVA: 0x0010D8AE File Offset: 0x0010BAAE
	public void OnExitMultiplyOperation(ArgumentBox argsBox)
	{
		this.ExitMultiplyMode();
	}

	// Token: 0x060024AA RID: 9386 RVA: 0x0010D8B8 File Offset: 0x0010BAB8
	public void OnItemMultiplyOperationTypeChange(ArgumentBox argsBox)
	{
		Enum type;
		bool flag = argsBox.Get("ItemOperationType", out type);
		if (flag)
		{
			this._currItemOperation = (ItemOperationType.EItemOperationType)type;
			this._multiplyItemOperationType = this._currItemOperation;
			this.RefreshItems();
			this.RefreshSort();
			ArgumentBox args = this.GetItemMultiplyViewArgs();
			GEvent.OnEvent(UiEvents.ItemMultiplyOperationContentChange, args);
			this.HideGradeLimitTip();
		}
	}

	// Token: 0x060024AB RID: 9387 RVA: 0x0010D91C File Offset: 0x0010BB1C
	public void OnItemMultiplyOperationConfirm(ArgumentBox argsBox)
	{
		switch (this._currItemOperation)
		{
		case ItemOperationType.EItemOperationType.Repair:
			this.OnClickMultiplyRepair();
			break;
		case ItemOperationType.EItemOperationType.Disassemble:
			this.OnClickMultiplyDisassemble();
			break;
		case ItemOperationType.EItemOperationType.Transfer:
			this.OnClickMultiplyTransfer();
			break;
		case ItemOperationType.EItemOperationType.Take:
			this.OnClickMultiplyTransfer();
			break;
		case ItemOperationType.EItemOperationType.Discard:
			this.OnClickMultiplyDiscard();
			break;
		case ItemOperationType.EItemOperationType.Feeding:
			this.OnClickMultiplyFeed();
			break;
		case ItemOperationType.EItemOperationType.PutPoisonMaterial:
			this.OnClickMultiplyPutPoisonMaterial();
			break;
		case ItemOperationType.EItemOperationType.VillagerCraft:
			this.OnClickMultiplyVillagerCraft();
			break;
		}
		this.HideGradeLimitTip();
	}

	// Token: 0x060024AC RID: 9388 RVA: 0x0010D9BC File Offset: 0x0010BBBC
	public void OnItemMultiplyOperationCancelSelection(ArgumentBox argsBox)
	{
		ItemDisplayData itemData;
		bool flag = argsBox.Get<ItemDisplayData>("ItemData", out itemData);
		if (flag)
		{
			Dictionary<ItemDisplayData, short> usedToolDict = this._multiplyOperationHandler.GetUsedToolDict(itemData);
			List<ItemDisplayData> usedToolList = new List<ItemDisplayData>(usedToolDict.Keys);
			this.SetItemSelectCount(itemData.RealKey, 0, usedToolList, true);
		}
	}

	// Token: 0x060024AD RID: 9389 RVA: 0x0010DA08 File Offset: 0x0010BC08
	private void RefreshSort()
	{
		int lineId = EFilterLine.MainFilter.ToInt();
		ItemOperationType.EItemOperationType currItemOperation = this._currItemOperation;
		ItemOperationType.EItemOperationType eitemOperationType = currItemOperation;
		if (eitemOperationType != ItemOperationType.EItemOperationType.Repair)
		{
			if (eitemOperationType != ItemOperationType.EItemOperationType.Disassemble)
			{
				if (eitemOperationType != ItemOperationType.EItemOperationType.Feeding)
				{
					this.CurMultiplyScrollView.SortAndFilterController.SetToggleIsOn(lineId, -1);
					this.CurMultiplyScrollView.SortAndFilterController.SetToggleVisible(lineId, -1);
				}
				else
				{
					this.CurMultiplyScrollView.SortAndFilterController.SetToggleIsOn(lineId, EMainFilterKeys.Material.ToInt());
					this.CurMultiplyScrollView.SortAndFilterController.SetToggleVisible(lineId, EMainFilterKeys.Material.ToInt());
				}
			}
			else
			{
				this.CurMultiplyScrollView.SortAndFilterController.SetToggleIsOn(lineId, -1);
				List<int> toggleIndexList = (from t in UI_CharacterMenuItems.DisassembleFilterTypes2
				select t.ToInt()).ToList<int>();
				this.CurMultiplyScrollView.SortAndFilterController.SetToggleVisible(lineId, toggleIndexList);
			}
		}
		else
		{
			this.CurMultiplyScrollView.SortAndFilterController.SetToggleIsOn(lineId, EMainFilterKeys.Equip.ToInt());
			this.CurMultiplyScrollView.SortAndFilterController.SetToggleVisible(lineId, EMainFilterKeys.Equip.ToInt());
		}
	}

	// Token: 0x060024AE RID: 9390 RVA: 0x0010DB3C File Offset: 0x0010BD3C
	private void RefreshUsedTool()
	{
		foreach (KeyValuePair<ItemDisplayData, short> pair in this._multiplyOperationHandler.AllUsedToolDict)
		{
			ItemDisplayData tool = pair.Key;
			bool flag = tool.Durability <= 0;
			if (flag)
			{
				bool flag2 = this.CurInventoryItems.Exists((ItemDisplayData i) => i == tool);
				if (flag2)
				{
					this.CurInventoryItems.Remove(tool);
				}
				else
				{
					List<ItemDisplayData> curWarehouseItems = this.CurWarehouseItems;
					bool flag3 = curWarehouseItems != null && curWarehouseItems.Exists((ItemDisplayData i) => i == tool);
					if (flag3)
					{
						this.CurWarehouseItems.Remove(tool);
					}
				}
			}
		}
	}

	// Token: 0x060024AF RID: 9391 RVA: 0x0010DC2C File Offset: 0x0010BE2C
	public List<ItemDisplayData> GetAvailableToolList(ItemDisplayData itemData)
	{
		bool canCraft = ItemTemplateHelper.GetCanDisassemble(itemData.Key.ItemType, itemData.Key.TemplateId) || ItemTemplateHelper.IsRepairable(itemData.Key.ItemType, itemData.Key.TemplateId);
		bool flag = !canCraft;
		List<ItemDisplayData> result;
		if (flag)
		{
			result = null;
		}
		else
		{
			sbyte itemLifeSkillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(itemData.Key.ItemType, itemData.Key.TemplateId);
			Dictionary<bool, List<ItemDisplayData>> tempToolDict = this.UsableToolOrderedList.Where(delegate(ItemDisplayData d)
			{
				CraftToolItem toolConfig = CraftTool.Instance[d.Key.TemplateId];
				bool skillIsMeet = toolConfig.RequiredLifeSkillTypes.Contains(itemLifeSkillType);
				bool flag2 = !skillIsMeet;
				bool result2;
				if (flag2)
				{
					result2 = false;
				}
				else
				{
					int attainment = (int)this._lifeSkillMonitor.Attainments[(int)itemLifeSkillType];
					attainment += (int)UI_Make.GetToolAttainment(d.Key.TemplateId, itemLifeSkillType);
					attainment = Math.Max(0, attainment);
					short required = (this._currItemOperation == ItemOperationType.EItemOperationType.Repair) ? ItemTemplateHelper.GetRepairRequiredAttainment(itemData.Key.ItemType, itemData.Key.TemplateId, itemData.Durability) : ItemTemplateHelper.GetDisassembleRequiredAttainment(itemData.Key.ItemType, itemData.Key.TemplateId);
					bool attainmentIsMeet = attainment >= (int)required;
					bool flag3 = !attainmentIsMeet;
					if (flag3)
					{
						result2 = false;
					}
					else
					{
						short used;
						this._multiplyOperationHandler.AllUsedToolDict.TryGetValue(d, out used);
						bool durabilityIsMeet = used < d.Durability;
						bool flag4 = !durabilityIsMeet;
						result2 = !flag4;
					}
				}
				return result2;
			}).GroupBy(delegate(ItemDisplayData d)
			{
				CraftToolItem toolConfig = CraftTool.Instance[d.Key.TemplateId];
				sbyte grade = ItemTemplateHelper.GetGrade(itemData.Key.ItemType, itemData.Key.TemplateId);
				short durabilityCost = toolConfig.DurabilityCost[(int)grade];
				return durabilityCost == 0;
			}).ToDictionary((IGrouping<bool, ItemDisplayData> g) => g.Key, (IGrouping<bool, ItemDisplayData> g) => g.ToList<ItemDisplayData>());
			List<ItemDisplayData> zeroGroup;
			tempToolDict.TryGetValue(true, out zeroGroup);
			if (zeroGroup == null)
			{
				zeroGroup = new List<ItemDisplayData>();
			}
			List<ItemDisplayData> nonZeroGroup;
			tempToolDict.TryGetValue(false, out nonZeroGroup);
			if (nonZeroGroup == null)
			{
				nonZeroGroup = new List<ItemDisplayData>();
			}
			List<ItemDisplayData> tempToolList = zeroGroup.Concat(nonZeroGroup).ToList<ItemDisplayData>();
			List<ItemDisplayData> availableToolList = new List<ItemDisplayData>();
			availableToolList.AddRange(tempToolList);
			result = availableToolList;
		}
		return result;
	}

	// Token: 0x060024B0 RID: 9392 RVA: 0x0010DD98 File Offset: 0x0010BF98
	private void ClearItemUsingState(ItemDisplayData itemData)
	{
		bool flag = this.IsMultiItemSelect && this.CurMultiplyItemSourceType != ItemSourceType.Inventory;
		if (!flag)
		{
			ItemDisplayData.ClearItemUsingState(itemData, this.CurInventoryItems);
		}
	}

	// Token: 0x060024B1 RID: 9393 RVA: 0x0010DDD0 File Offset: 0x0010BFD0
	public bool CheckAllSelectedItemsHasAnyLimit()
	{
		foreach (ItemDisplayData data in this.SelectedMultiplyItemOrderedList)
		{
			bool flag = this.CheckCurItemHasGradeLimit(data.Key);
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060024B2 RID: 9394 RVA: 0x0010DE3C File Offset: 0x0010C03C
	public bool CheckCurItemHasGradeLimit(ItemKey itemKey)
	{
		return this.CheckItemHasGradeLimit(itemKey, this.IsInventoryMultiply);
	}

	// Token: 0x060024B3 RID: 9395 RVA: 0x0010DE4C File Offset: 0x0010C04C
	public bool CheckItemHasGradeLimit(ItemKey itemKey, bool isInventory)
	{
		bool isMiscResource = ItemTemplateHelper.IsMiscResource(itemKey.ItemType, itemKey.TemplateId);
		bool flag = isMiscResource;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			ItemSourceType itemSourceType = isInventory ? this.InventoryItemSourceType : this.WarehouseItemSourceType;
			ItemGradeFilterSetting.ItemGradeFilterSourceType itemGradeFilterSourceType = ItemGradeFilterSetting.GetItemGradeFilterSourceType(itemSourceType, this.InventoryItemSourceType == ItemSourceType.Inventory, false);
			sbyte grade = this._itemGradeFilterSetting.GetGrade(itemGradeFilterSourceType);
			result = (grade > 0 && grade <= ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId));
		}
		return result;
	}

	// Token: 0x060024B4 RID: 9396 RVA: 0x0010DECC File Offset: 0x0010C0CC
	public bool CheckNeedNotification(ItemDisplayData itemDisplayData, out ItemDisplayData.ItemUsingType usingType)
	{
		bool flag = itemDisplayData.UsingType == ItemDisplayData.ItemUsingType.Reading || itemDisplayData.UsingType == ItemDisplayData.ItemUsingType.Referring || itemDisplayData.UsingType == ItemDisplayData.ItemUsingType.Equiped || itemDisplayData.UsingType == ItemDisplayData.ItemUsingType.EquipmentPlaned;
		bool result;
		if (flag)
		{
			usingType = itemDisplayData.UsingType;
			result = true;
		}
		else
		{
			bool isInCurrentCricketPreset = itemDisplayData.IsInCurrentCricketPreset;
			if (isInCurrentCricketPreset)
			{
				usingType = ItemDisplayData.ItemUsingType.EquipmentPlaned;
				result = true;
			}
			else
			{
				usingType = ItemDisplayData.ItemUsingType.Invalid;
				result = this.CheckCurItemHasGradeLimit(itemDisplayData.Key);
			}
		}
		return result;
	}

	// Token: 0x060024B5 RID: 9397 RVA: 0x0010DF38 File Offset: 0x0010C138
	private string GetNotificationTextByUsingType(ItemDisplayData.ItemUsingType usingType)
	{
		switch (usingType)
		{
		case ItemDisplayData.ItemUsingType.Reading:
			return LocalStringManager.Get(LanguageKey.LK_Multiply_Select_Reading_Tip);
		case ItemDisplayData.ItemUsingType.EquipmentPlaned:
			return LocalStringManager.Get(LanguageKey.LK_Multiply_Select_EquipPlan_Tip);
		case ItemDisplayData.ItemUsingType.Equiped:
			return LocalStringManager.Get(LanguageKey.LK_Multiply_Select_Equiped_Tip);
		case ItemDisplayData.ItemUsingType.Referring:
			return LocalStringManager.Get(LanguageKey.LK_Multiply_Select_Referring_Tip);
		}
		return LocalStringManager.Get(LanguageKey.LK_Multiply_Select_Grade_Tip);
	}

	// Token: 0x060024B6 RID: 9398 RVA: 0x0010DFA8 File Offset: 0x0010C1A8
	private void OpenMultiplyOption(CButtonObsolete button)
	{
		bool flag = UIManager.Instance.IsFocusElement(UIElement.ItemMultiplyOptionOld);
		if (flag)
		{
			UIManager.Instance.HideUI(UIElement.ItemMultiplyOptionOld);
		}
		else
		{
			RectTransform rectTrans = button.GetComponent<RectTransform>();
			Vector3 localPos = default(Vector3).SetY(rectTrans.rect.height * 0.5f);
			Vector3 pos = rectTrans.TransformPoint(localPos);
			ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("AnchorItem", rectTrans).SetObject("Pos", pos).Set("Type", this.CurItemGradeFilterSourceType);
			UIElement.ItemMultiplyOptionOld.SetOnInitArgs(args);
			UIManager.Instance.ShowUI(UIElement.ItemMultiplyOptionOld, true);
		}
	}

	// Token: 0x060024B7 RID: 9399 RVA: 0x0010E068 File Offset: 0x0010C268
	private void ShowGradeLimitTip(CommonTableRowForItem itemView, string content)
	{
		bool flag = this._currItemOperation == ItemOperationType.EItemOperationType.Repair || this._currItemOperation == ItemOperationType.EItemOperationType.Take;
		if (!flag)
		{
			RectTransform tip;
			bool flag2 = this.CTryGet<RectTransform>("GradeLimitTip", out tip);
			if (flag2)
			{
				bool flag3 = !tip.gameObject.activeSelf;
				if (flag3)
				{
					tip.gameObject.SetActive(true);
				}
				PositionFollower follower = tip.GetComponent<PositionFollower>();
				RectTransform itemRectTrans = itemView.GetComponent<RectTransform>();
				follower.Target = itemRectTrans;
				follower.Offset = new Vector2(itemRectTrans.sizeDelta.x * 0.5f, -itemRectTrans.sizeDelta.y);
				follower.Excute();
				TextMeshProUGUI tipContentComp = tip.GetComponentInChildren<TextMeshProUGUI>();
				tipContentComp.text = content;
				Rect itemRect = CommonUtils.RectTransToScreenPos(tip, UIManager.Instance.UiCamera);
				Rect scrollRect = CommonUtils.RectTransToScreenPos(this.CurMultiplyScrollView.ViewportRectTransform, UIManager.Instance.UiCamera);
				bool isOverlap = itemRect.Overlaps(scrollRect);
				bool flag4 = !isOverlap;
				if (flag4)
				{
					follower.Offset = new Vector2(itemRectTrans.sizeDelta.x * 0.5f, tip.sizeDelta.y);
				}
				CanvasGroup canvasGroup = tip.GetComponent<CanvasGroup>();
				canvasGroup.DOKill(false);
				canvasGroup.alpha = 1f;
				canvasGroup.DOFade(0.2f, 1f).SetLoops(5, LoopType.Yoyo);
				canvasGroup.DOFade(0f, 0.2f).SetDelay(5f);
			}
		}
	}

	// Token: 0x060024B8 RID: 9400 RVA: 0x0010E1F0 File Offset: 0x0010C3F0
	public void HideGradeLimitTip()
	{
		RectTransform tip;
		bool flag = this.CTryGet<RectTransform>("GradeLimitTip", out tip);
		if (flag)
		{
			bool activeSelf = tip.gameObject.activeSelf;
			if (activeSelf)
			{
				tip.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060024B9 RID: 9401 RVA: 0x0010E230 File Offset: 0x0010C430
	public void RefreshItems()
	{
		bool isMultiItemSelect = this.IsMultiItemSelect;
		if (isMultiItemSelect)
		{
			this._multiplyOperationHandler.Clear();
			this.RefreshMultiplyAvailableTool();
			this.RefreshSelectedCount();
			this.RefreshMultiplyCanOperateItems();
			this.SendItemMultiplyOperationContentChange(null);
		}
		else
		{
			this._canOperateItems.Clear();
			bool isInventoryMultiply = this.IsInventoryMultiply;
			if (isInventoryMultiply)
			{
				List<ItemDisplayData> items = this.CurInventoryItems;
				this._inventoryScroll.SetItemList(ref items);
			}
			else
			{
				List<ItemDisplayData> items2 = this.CurWarehouseItems;
				this._warehouseScroll.SetItemList(ref items2);
			}
		}
		bool flag = this.OtherScrollView;
		if (flag)
		{
			List<ItemDisplayData> otherMultiplyItems = this.OtherItems;
			this.OtherScrollView.SetItemList(ref otherMultiplyItems);
		}
	}

	// Token: 0x060024BA RID: 9402 RVA: 0x0010E2E8 File Offset: 0x0010C4E8
	public void ShowMultiplySelectButton(CButtonObsolete button = null, bool canTransfer = true)
	{
		if (button == null)
		{
			button = this.BtnMultiplySelect;
		}
		bool flag = !button;
		if (!flag)
		{
			button.gameObject.SetActive(this.CurCharacterIsTaiwu && this._isTaiWuTeam);
			button.interactable = (!SingletonObject.getInstance<TutorialChapterModel>().InGuiding && canTransfer);
		}
	}

	// Token: 0x060024BB RID: 9403 RVA: 0x0010E343 File Offset: 0x0010C543
	public void HideMultiplySelectButton()
	{
		CButtonObsolete btnMultiplySelect = this.BtnMultiplySelect;
		if (btnMultiplySelect != null)
		{
			btnMultiplySelect.gameObject.SetActive(false);
		}
	}

	// Token: 0x060024BC RID: 9404 RVA: 0x0010E360 File Offset: 0x0010C560
	public void SetFeedingTarget(ItemDisplayData target)
	{
		bool isChanged = target != this._feedingTarget;
		this._feedingTarget = target;
		bool flag = target != this._feedingTargetOnEnter;
		if (flag)
		{
			this._feedingTargetOnEnter = null;
		}
		bool isOn = this.ToggleSelectAll.isOn;
		if (isOn)
		{
			bool flag2 = isChanged;
			if (flag2)
			{
				this.SelectAll(false);
			}
		}
		else
		{
			int num;
			int num2;
			this.RefreshFeedState(out num, out num2);
		}
		this.CurMultiplyScrollView.ReRender();
	}

	// Token: 0x060024BE RID: 9406 RVA: 0x0010E428 File Offset: 0x0010C628
	[CompilerGenerated]
	private void <Init>g__InitButton|87_0(Refers refers, bool isInventory)
	{
		CButtonObsolete btnMultiplySelect;
		bool flag = refers.CTryGet<CButtonObsolete>("BtnMultiplySelect", out btnMultiplySelect);
		if (flag)
		{
			this.ShowMultiplySelectButton(btnMultiplySelect, true);
			bool flag2 = btnMultiplySelect;
			if (flag2)
			{
				btnMultiplySelect.ClearAndAddListener(delegate
				{
					this.IsInventoryMultiply = isInventory;
					this.EnterMultiplyMode();
				});
			}
		}
		CommonSwitch switchSelection;
		bool flag3 = refers.CTryGet<CommonSwitch>("SwitchSelection", out switchSelection);
		if (flag3)
		{
			switchSelection.gameObject.SetActive(false);
			switchSelection.transform.parent.parent.gameObject.SetActive(false);
		}
		CToggleObsolete toggleSelectAll;
		bool flag4 = refers.CTryGet<CToggleObsolete>("ToggleSelectAll", out toggleSelectAll);
		if (flag4)
		{
			toggleSelectAll.gameObject.SetActive(false);
			toggleSelectAll.onValueChanged.RemoveAllListeners();
			toggleSelectAll.onValueChanged.AddListener(delegate(bool isOn)
			{
				this.IsInventoryMultiply = isInventory;
				this.SelectAll(isOn);
			});
			toggleSelectAll.isOn = false;
		}
	}

	// Token: 0x04001B59 RID: 7001
	private Dictionary<ItemSourceType, List<ItemDisplayData>> _itemDict;

	// Token: 0x04001B5A RID: 7002
	[NonSerialized]
	public long MaxWorthCanBeLentToTaiwu;

	// Token: 0x04001B5B RID: 7003
	private GroupedItemScrollView2 _warehouseScroll;

	// Token: 0x04001B5C RID: 7004
	private GroupedItemScrollView2 _inventoryScroll;

	// Token: 0x04001B5D RID: 7005
	private ItemScrollViewForCommonTableRow _selectedItemScroll;

	// Token: 0x04001B5E RID: 7006
	[NonSerialized]
	public ItemSourceType InventoryItemSourceType = ItemSourceType.Inventory;

	// Token: 0x04001B5F RID: 7007
	[NonSerialized]
	public ItemSourceType WarehouseItemSourceType = ItemSourceType.Warehouse;

	// Token: 0x04001B60 RID: 7008
	[NonSerialized]
	public int CurCharId;

	// Token: 0x04001B61 RID: 7009
	private bool _isTaiWuTeam;

	// Token: 0x04001B62 RID: 7010
	private ItemOperationType.EItemOperationType _currItemOperation;

	// Token: 0x04001B63 RID: 7011
	private List<ItemDisplayData> _canOperateItems = new List<ItemDisplayData>();

	// Token: 0x04001B64 RID: 7012
	private List<ItemDisplayData> _selectedItems = new List<ItemDisplayData>();

	// Token: 0x04001B65 RID: 7013
	private ItemDisplayData _feedingTarget;

	// Token: 0x04001B66 RID: 7014
	private ItemDisplayData _feedingTargetOnEnter;

	// Token: 0x04001B67 RID: 7015
	private readonly List<ItemDisplayData> _feedingTargetList = new List<ItemDisplayData>();

	// Token: 0x04001B68 RID: 7016
	private readonly MultiplyOperationHandler _multiplyOperationHandler = new MultiplyOperationHandler();

	// Token: 0x04001B6A RID: 7018
	private ItemOperationType.EItemOperationType _multiplyItemOperationType = ItemOperationType.EItemOperationType.Disassemble;

	// Token: 0x04001B6C RID: 7020
	private ItemGradeFilterSetting _itemGradeFilterSetting;

	// Token: 0x04001B6D RID: 7021
	private ResourceMonitor _resourceMonitor;

	// Token: 0x04001B6E RID: 7022
	private LifeSkillMonitor _lifeSkillMonitor;

	// Token: 0x04001B6F RID: 7023
	private Action _onEnterMultiplyOperation;

	// Token: 0x04001B70 RID: 7024
	private Action _onExitMultiplyOperation;

	// Token: 0x04001B71 RID: 7025
	[TupleElementNames(new string[]
	{
		"data",
		"count"
	})]
	private Action<List<ValueTuple<ItemDisplayData, int>>> _onContentChange;

	// Token: 0x04001B74 RID: 7028
	private bool _isFeedDurabilityMax;

	// Token: 0x04001B75 RID: 7029
	private bool _isFeedTameMax;

	// Token: 0x04001B76 RID: 7030
	private Func<ItemDisplayData, bool> _filter;
}
