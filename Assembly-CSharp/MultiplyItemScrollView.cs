using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CharacterDataMonitor;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using MonoMod.Utils;
using TMPro;
using UnityEngine;

// Token: 0x02000233 RID: 563
public class MultiplyItemScrollView : Refers
{
	// Token: 0x17000383 RID: 899
	// (get) Token: 0x060023ED RID: 9197 RVA: 0x00107D14 File Offset: 0x00105F14
	public long SelectedMultiplyItemTotalWorth
	{
		get
		{
			return this.SelectedMultiplyItemDict.Sum((KeyValuePair<ItemDisplayData, int> pair) => pair.Key.Value * (long)pair.Value);
		}
	}

	// Token: 0x17000384 RID: 900
	// (get) Token: 0x060023EE RID: 9198 RVA: 0x00107D40 File Offset: 0x00105F40
	private List<ItemDisplayData> CurInventoryItems
	{
		get
		{
			return (this._customItems == null) ? this._itemDict.GetValueOrDefault(this.InventoryItemSourceType) : this._customItems;
		}
	}

	// Token: 0x17000385 RID: 901
	// (get) Token: 0x060023EF RID: 9199 RVA: 0x00107D63 File Offset: 0x00105F63
	private List<ItemDisplayData> CurWarehouseItems
	{
		get
		{
			return this._itemDict.GetValueOrDefault(this.WarehouseItemSourceType);
		}
	}

	// Token: 0x17000386 RID: 902
	// (get) Token: 0x060023F0 RID: 9200 RVA: 0x00107D76 File Offset: 0x00105F76
	private static int TaiwuCharId
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}
	}

	// Token: 0x17000387 RID: 903
	// (get) Token: 0x060023F1 RID: 9201 RVA: 0x00107D82 File Offset: 0x00105F82
	private bool CurCharacterIsTaiwu
	{
		get
		{
			return this.CurCharId == MultiplyItemScrollView.TaiwuCharId;
		}
	}

	// Token: 0x17000388 RID: 904
	// (get) Token: 0x060023F2 RID: 9202 RVA: 0x00107D91 File Offset: 0x00105F91
	public ItemOperationType.EItemOperationType CurrItemOperation
	{
		get
		{
			return this._currItemOperation;
		}
	}

	// Token: 0x17000389 RID: 905
	// (get) Token: 0x060023F3 RID: 9203 RVA: 0x00107D99 File Offset: 0x00105F99
	public Dictionary<ItemDisplayData, int> SelectedMultiplyItemDict
	{
		get
		{
			return this._multiplyOperationHandler.SelectedItemDict;
		}
	}

	// Token: 0x1700038A RID: 906
	// (get) Token: 0x060023F4 RID: 9204 RVA: 0x00107DA6 File Offset: 0x00105FA6
	public List<ItemDisplayData> SelectedMultiplyItemOrderedList
	{
		get
		{
			return this._multiplyOperationHandler.SelectedItemOrderedList;
		}
	}

	// Token: 0x1700038B RID: 907
	// (get) Token: 0x060023F5 RID: 9205 RVA: 0x00107DB3 File Offset: 0x00105FB3
	// (set) Token: 0x060023F6 RID: 9206 RVA: 0x00107DBB File Offset: 0x00105FBB
	public bool IsMultiItemSelect { get; private set; }

	// Token: 0x1700038C RID: 908
	// (get) Token: 0x060023F7 RID: 9207 RVA: 0x00107DC4 File Offset: 0x00105FC4
	private CButtonObsolete BtnMultiplySelect
	{
		get
		{
			CButtonObsolete btnMultiplySelect;
			bool flag = this.SourceRootRefer.CTryGet<CButtonObsolete>("BtnMultiplySelect", out btnMultiplySelect);
			CButtonObsolete result;
			if (flag)
			{
				result = btnMultiplySelect;
			}
			else
			{
				result = null;
			}
			return result;
		}
	}

	// Token: 0x1700038D RID: 909
	// (get) Token: 0x060023F8 RID: 9208 RVA: 0x00107DF1 File Offset: 0x00105FF1
	public CButtonObsolete BtnMultiplyOption
	{
		get
		{
			return this.SourceRootRefer.CGet<CButtonObsolete>("BtnMultiplyOption");
		}
	}

	// Token: 0x1700038E RID: 910
	// (get) Token: 0x060023F9 RID: 9209 RVA: 0x00107E03 File Offset: 0x00106003
	private List<ItemDisplayData> UsableToolOrderedList
	{
		get
		{
			return this._multiplyOperationHandler.UsableToolOrderedList;
		}
	}

	// Token: 0x1700038F RID: 911
	// (get) Token: 0x060023FA RID: 9210 RVA: 0x00107E10 File Offset: 0x00106010
	// (set) Token: 0x060023FB RID: 9211 RVA: 0x00107E18 File Offset: 0x00106018
	public bool IsInventoryMultiply { get; set; }

	// Token: 0x17000390 RID: 912
	// (get) Token: 0x060023FC RID: 9212 RVA: 0x00107E21 File Offset: 0x00106021
	public BaseItemScrollView CurMultiplyScrollView
	{
		get
		{
			return this.IsInventoryMultiply ? this._inventoryScroll : this._warehouseScroll;
		}
	}

	// Token: 0x17000391 RID: 913
	// (get) Token: 0x060023FD RID: 9213 RVA: 0x00107E39 File Offset: 0x00106039
	private BaseItemScrollView OtherScrollView
	{
		get
		{
			return (!this.IsInventoryMultiply) ? this._inventoryScroll : this._warehouseScroll;
		}
	}

	// Token: 0x17000392 RID: 914
	// (get) Token: 0x060023FE RID: 9214 RVA: 0x00107E51 File Offset: 0x00106051
	private List<ItemDisplayData> CurMultiplyItems
	{
		get
		{
			return this.IsInventoryMultiply ? this.CurInventoryItems : this.CurWarehouseItems;
		}
	}

	// Token: 0x17000393 RID: 915
	// (get) Token: 0x060023FF RID: 9215 RVA: 0x00107E69 File Offset: 0x00106069
	private List<ItemDisplayData> OtherItems
	{
		get
		{
			return (!this.IsInventoryMultiply) ? this.CurInventoryItems : this.CurWarehouseItems;
		}
	}

	// Token: 0x17000394 RID: 916
	// (get) Token: 0x06002400 RID: 9216 RVA: 0x00107E81 File Offset: 0x00106081
	private ItemSourceType CurMultiplyItemSourceType
	{
		get
		{
			return this.IsInventoryMultiply ? this.InventoryItemSourceType : this.WarehouseItemSourceType;
		}
	}

	// Token: 0x17000395 RID: 917
	// (get) Token: 0x06002401 RID: 9217 RVA: 0x00107E9C File Offset: 0x0010609C
	private ItemGradeFilterSetting.ItemGradeFilterSourceType CurItemGradeFilterSourceType
	{
		get
		{
			ItemSourceType itemSourceType = this.IsInventoryMultiply ? this.InventoryItemSourceType : this.WarehouseItemSourceType;
			return ItemGradeFilterSetting.GetItemGradeFilterSourceType(itemSourceType, itemSourceType == ItemSourceType.Inventory, false);
		}
	}

	// Token: 0x17000396 RID: 918
	// (get) Token: 0x06002402 RID: 9218 RVA: 0x00107ED0 File Offset: 0x001060D0
	// (set) Token: 0x06002403 RID: 9219 RVA: 0x00107ED8 File Offset: 0x001060D8
	public bool IsEmptyTool
	{
		get
		{
			return this._isEmptyTool;
		}
		set
		{
			this._isEmptyTool = value;
			CButtonObsolete btnEmptyTool = this.BtnEmptyTool;
			if (btnEmptyTool != null)
			{
				btnEmptyTool.GetComponent<Refers>().CGet<GameObject>("CheckMark").SetActive(this._isEmptyTool);
			}
		}
	}

	// Token: 0x17000397 RID: 919
	// (get) Token: 0x06002404 RID: 9220 RVA: 0x00107F09 File Offset: 0x00106109
	public Refers SourceRootRefer
	{
		get
		{
			return this.IsInventoryMultiply ? this.InventoryRefer : this.WarehouseRefer;
		}
	}

	// Token: 0x17000398 RID: 920
	// (get) Token: 0x06002405 RID: 9221 RVA: 0x00107F21 File Offset: 0x00106121
	private Refers InventoryRefer
	{
		get
		{
			return base.CGet<Refers>("Inventory");
		}
	}

	// Token: 0x17000399 RID: 921
	// (get) Token: 0x06002406 RID: 9222 RVA: 0x00107F2E File Offset: 0x0010612E
	private Refers WarehouseRefer
	{
		get
		{
			return base.CGet<Refers>("Warehouse");
		}
	}

	// Token: 0x1700039A RID: 922
	// (get) Token: 0x06002407 RID: 9223 RVA: 0x00107F3C File Offset: 0x0010613C
	private CButtonObsolete BtnEmptyTool
	{
		get
		{
			CButtonObsolete btnEmptyTool;
			bool flag = this.SourceRootRefer.CTryGet<CButtonObsolete>("BtnEmptyTool", out btnEmptyTool);
			CButtonObsolete result;
			if (flag)
			{
				result = btnEmptyTool;
			}
			else
			{
				result = null;
			}
			return result;
		}
	}

	// Token: 0x1700039B RID: 923
	// (get) Token: 0x06002408 RID: 9224 RVA: 0x00107F69 File Offset: 0x00106169
	private bool NeedShowBtnEmptyTool
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700039C RID: 924
	// (get) Token: 0x06002409 RID: 9225 RVA: 0x00107F6C File Offset: 0x0010616C
	// (set) Token: 0x0600240A RID: 9226 RVA: 0x00107F74 File Offset: 0x00106174
	public int MaxSelectCount { get; set; }

	// Token: 0x1700039D RID: 925
	// (get) Token: 0x0600240B RID: 9227 RVA: 0x00107F7D File Offset: 0x0010617D
	// (set) Token: 0x0600240C RID: 9228 RVA: 0x00107F85 File Offset: 0x00106185
	public int TotalSelectedCount { get; private set; }

	// Token: 0x0600240D RID: 9229 RVA: 0x00107F90 File Offset: 0x00106190
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
		this.IsEmptyTool = false;
		this._resourceMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<ResourceMonitor>(MultiplyItemScrollView.TaiwuCharId, false);
		this._lifeSkillMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<LifeSkillMonitor>(MultiplyItemScrollView.TaiwuCharId, false);
		bool flag = this.Names.Contains("Inventory");
		if (flag)
		{
			this._inventoryScroll = this.InventoryRefer.CGet<ItemScrollView>("ItemScrollView");
			this.<Init>g__InitButton|91_0(this.InventoryRefer, true);
		}
		bool flag2 = this.Names.Contains("Warehouse");
		if (flag2)
		{
			this._warehouseScroll = this.WarehouseRefer.CGet<GroupedItemScrollView>("WarehouseItemGroupedScroll");
			this.<Init>g__InitButton|91_0(this.WarehouseRefer, false);
		}
	}

	// Token: 0x0600240E RID: 9230 RVA: 0x0010808C File Offset: 0x0010628C
	public void SetCustomItemList(List<ItemDisplayData> items)
	{
		this._customItems = items;
	}

	// Token: 0x0600240F RID: 9231 RVA: 0x00108098 File Offset: 0x00106298
	public bool IsSelected(ItemDisplayData itemData)
	{
		int selectedCount;
		this.SelectedMultiplyItemDict.TryGetValue(itemData, out selectedCount);
		return selectedCount > 0;
	}

	// Token: 0x06002410 RID: 9232 RVA: 0x001080C0 File Offset: 0x001062C0
	public void OnRenderItemMultiply(ItemDisplayData itemData, ItemView view)
	{
		MultiplyItemScrollView.<>c__DisplayClass94_0 CS$<>8__locals1 = new MultiplyItemScrollView.<>c__DisplayClass94_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.itemData = itemData;
		CS$<>8__locals1.view = view;
		this.SelectedMultiplyItemDict.TryGetValue(CS$<>8__locals1.itemData, out CS$<>8__locals1.selectedCount);
		CS$<>8__locals1.isSelected = (CS$<>8__locals1.selectedCount > 0);
		bool isSelected = CS$<>8__locals1.isSelected;
		if (isSelected)
		{
			CS$<>8__locals1.view.SetSelectedCount(CS$<>8__locals1.selectedCount);
		}
		CS$<>8__locals1.view.SetHighLight(CS$<>8__locals1.isSelected);
		CS$<>8__locals1.view.SetSelectState(CS$<>8__locals1.isSelected);
		bool flag = !CS$<>8__locals1.view.IsLocked;
		if (flag)
		{
			CS$<>8__locals1.view.SetInteractable(true);
		}
		CS$<>8__locals1.usedToolList = null;
		CS$<>8__locals1.availableToolList = null;
		CS$<>8__locals1.useEmptyTool = false;
		bool isSelected2 = CS$<>8__locals1.isSelected;
		if (isSelected2)
		{
			int index = this.GetSelectedOrder(CS$<>8__locals1.itemData);
			CS$<>8__locals1.view.SetSelectedOrder(true, index);
			Dictionary<ItemDisplayData, short> usedToolDict = this._multiplyOperationHandler.GetUsedToolDict(CS$<>8__locals1.itemData);
			CS$<>8__locals1.usedToolList = new List<ItemDisplayData>(usedToolDict.Keys);
			bool openMultiOperation = this.OpenMultiOperation;
			if (openMultiOperation)
			{
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
			}
		}
		else
		{
			CS$<>8__locals1.view.SetSelectedOrder(false, 0);
			bool openMultiOperation2 = this.OpenMultiOperation;
			if (openMultiOperation2)
			{
				switch (this._currItemOperation)
				{
				case ItemOperationType.EItemOperationType.Repair:
				case ItemOperationType.EItemOperationType.Disassemble:
				{
					sbyte itemLifeSkillType3 = ItemTemplateHelper.GetCraftRequiredLifeSkillType(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
					short required3 = (this._currItemOperation == ItemOperationType.EItemOperationType.Repair) ? ItemTemplateHelper.GetRepairRequiredAttainment(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId, CS$<>8__locals1.itemData.Durability) : ItemTemplateHelper.GetDisassembleRequiredAttainment(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
					short attainment = this._lifeSkillMonitor.Attainments[(int)itemLifeSkillType3];
					short emptyToolAttainment = UI_Make.GetFinalAttainment(-1, attainment, itemLifeSkillType3);
					CS$<>8__locals1.useEmptyTool = (emptyToolAttainment >= required3);
					bool hasAvailableTool = false;
					bool flag2 = !CS$<>8__locals1.useEmptyTool;
					if (flag2)
					{
						CS$<>8__locals1.availableToolList = this.GetAvailableToolList(CS$<>8__locals1.itemData);
						hasAvailableTool = (CS$<>8__locals1.availableToolList != null && CS$<>8__locals1.availableToolList.Count > 0);
						bool flag3 = hasAvailableTool;
						if (flag3)
						{
							ItemDisplayData tool = CS$<>8__locals1.availableToolList.First<ItemDisplayData>();
							attainment += UI_Make.GetToolAttainment(tool.Key.TemplateId, itemLifeSkillType3);
							attainment = Math.Max(0, attainment);
						}
					}
					bool useNormalTool = hasAvailableTool && attainment >= required3;
					bool interactable = CS$<>8__locals1.useEmptyTool || useNormalTool;
					bool flag4 = interactable;
					if (flag4)
					{
						CS$<>8__locals1.view.HideInteractionState();
					}
					else
					{
						bool flag5 = hasAvailableTool;
						if (flag5)
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
				case ItemOperationType.EItemOperationType.Discard:
					CS$<>8__locals1.view.HideInteractionState();
					break;
				case ItemOperationType.EItemOperationType.SpecialBreakConvertToExp:
					CS$<>8__locals1.view.SetInteractable(!this.IsMeet);
					break;
				case ItemOperationType.EItemOperationType.Feeding:
				{
					bool flag6 = this._isFeedDurabilityMax && this._isFeedTameMax;
					if (flag6)
					{
						CS$<>8__locals1.view.SetInteractable(false);
						CS$<>8__locals1.view.SetItemNotCanSelectReason(LocalStringManager.Get(LanguageKey.LK_ItemFeedCarrier_Tip_Full).SetColor("brightred"));
						CS$<>8__locals1.view.SetHateAndLoveIconVisibility(false, false);
					}
					else
					{
						CarrierItem config = Carrier.Instance[this._enterFeedingItem.Key.TemplateId];
						CS$<>8__locals1.view.SetHateAndLoveIconVisibility(config.HateFoodType.Contains(CS$<>8__locals1.itemData.Key.TemplateId), config.LoveFoodType.Contains(CS$<>8__locals1.itemData.Key.TemplateId));
					}
					break;
				}
				}
			}
			Dictionary<sbyte, List<ItemKey>> dictionary;
			bool reachMaxValue = this._currItemOperation == ItemOperationType.EItemOperationType.PutCraftResource && this.GetPutResourceWorth(out dictionary, null) >= (long)this._valueNeeded;
			bool flag7 = (this.MaxSelectCount > 0 && this.TotalSelectedCount >= this.MaxSelectCount) || reachMaxValue;
			if (flag7)
			{
				CS$<>8__locals1.view.SetInteractable(false);
				CS$<>8__locals1.view.SetItemNotCanSelectReason(LocalStringManager.Get(LanguageKey.LK_ItemSelectCountIsMax).SetColor("brightred"));
			}
		}
		CS$<>8__locals1.view.SetClickEvent(delegate
		{
			MultiplyItemScrollView.<>c__DisplayClass94_1 CS$<>8__locals2 = new MultiplyItemScrollView.<>c__DisplayClass94_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			CS$<>8__locals1.<>4__this.HideGradeLimitTip();
			CS$<>8__locals2.toolList = (CS$<>8__locals1.isSelected ? CS$<>8__locals1.usedToolList : CS$<>8__locals1.availableToolList);
			bool flag8 = CS$<>8__locals1.isSelected && !CS$<>8__locals1.<>4__this.ReSelectAmountWhenSelected;
			if (flag8)
			{
				CS$<>8__locals1.<>4__this.SetItemSelectCount(CS$<>8__locals1.itemData, 0, CS$<>8__locals2.toolList);
				CS$<>8__locals1.<>4__this.CheckPutResource();
			}
			else
			{
				CS$<>8__locals2.limitTip = string.Empty;
				CS$<>8__locals2.limitCount = CS$<>8__locals1.itemData.Amount;
				switch (CS$<>8__locals1.<>4__this._currItemOperation)
				{
				case ItemOperationType.EItemOperationType.Repair:
				case ItemOperationType.EItemOperationType.Disassemble:
				{
					bool flag9 = !CS$<>8__locals1.useEmptyTool;
					if (flag9)
					{
						sbyte grade = ItemTemplateHelper.GetGrade(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
						bool flag10 = CS$<>8__locals1.availableToolList == null;
						if (flag10)
						{
							CS$<>8__locals2.limitCount = 0;
						}
						else
						{
							bool hasZero = CS$<>8__locals1.availableToolList.Exists(delegate(ItemDisplayData d)
							{
								CraftToolItem toolConfig = CraftTool.Instance[d.Key.TemplateId];
								short cost = toolConfig.DurabilityCost[(int)grade];
								return cost == 0;
							});
							bool flag11 = !hasZero;
							if (flag11)
							{
								CS$<>8__locals2.limitCount = CS$<>8__locals1.availableToolList.Sum(delegate(ItemDisplayData d)
								{
									short durability = CS$<>8__locals2.CS$<>8__locals1.<>4__this._multiplyOperationHandler.GetToolRemainDurability(d);
									CraftToolItem toolConfig = CraftTool.Instance[d.Key.TemplateId];
									short cost = toolConfig.DurabilityCost[(int)grade];
									return (int)((durability % cost == 0) ? (durability / cost) : (durability / cost + 1));
								});
							}
						}
						CS$<>8__locals2.limitTip = LocalStringManager.Get(LanguageKey.LK_SelectCount_Limit_Tool);
					}
					break;
				}
				case ItemOperationType.EItemOperationType.Transfer:
				{
					bool flag12 = UIManager.Instance.IsFocusElement(UIElement.EventWindow);
					if (flag12)
					{
						CS$<>8__locals2.limitCount = CS$<>8__locals1.<>4__this.MaxSelectCount - CS$<>8__locals1.<>4__this.TotalSelectedCount;
						CS$<>8__locals2.limitTip = LocalStringManager.Get(LanguageKey.LK_ItemSelectCountIsMax).SetColor("brightred");
					}
					break;
				}
				case ItemOperationType.EItemOperationType.Take:
				{
					bool flag13 = CS$<>8__locals1.<>4__this.CurCharId != MultiplyItemScrollView.TaiwuCharId;
					if (flag13)
					{
						CS$<>8__locals2.limitCount = ((CS$<>8__locals1.itemData.Value == 0L) ? 0 : Convert.ToInt32((CS$<>8__locals1.<>4__this.MaxWorthCanBeLentToTaiwu - CS$<>8__locals1.<>4__this.SelectedMultiplyItemTotalWorth) / CS$<>8__locals1.itemData.Value));
						CS$<>8__locals2.limitTip = LocalStringManager.Get(LanguageKey.LK_SelectCount_Limit_Take);
					}
					break;
				}
				case ItemOperationType.EItemOperationType.Feeding:
				{
					int leftDurability;
					int leftTame;
					CS$<>8__locals1.<>4__this.RefreshFeedState(out leftDurability, out leftTame);
					int addDurability = GameData.Domains.Extra.SharedMethods.GetFoodAddCarrierDurability(CS$<>8__locals1.<>4__this._enterFeedingItem.Key.TemplateId, CS$<>8__locals1.itemData.Key.TemplateId, 1);
					int durabilityCount = (addDurability <= 0) ? CS$<>8__locals1.itemData.Amount : Mathf.CeilToInt((float)leftDurability / (float)addDurability);
					int tameCount = 0;
					bool flag14 = ItemTemplateHelper.HasCarrierTame(CS$<>8__locals1.<>4__this._enterFeedingItem.Key.ItemType, CS$<>8__locals1.<>4__this._enterFeedingItem.Key.TemplateId);
					if (flag14)
					{
						int addTame = GameData.Domains.Extra.SharedMethods.GetFoodAddCarrierTamePoint(CS$<>8__locals1.<>4__this._enterFeedingItem.Key.TemplateId, CS$<>8__locals1.itemData.Key.TemplateId, 1);
						bool flag15 = addTame > 0;
						if (flag15)
						{
							tameCount = Mathf.CeilToInt((float)leftTame / (float)addTame);
						}
					}
					CS$<>8__locals2.limitCount = Mathf.Max(durabilityCount, tameCount);
					CS$<>8__locals2.limitTip = LocalStringManager.Get(LanguageKey.LK_SelectCount_Limit_Feeding);
					break;
				}
				case ItemOperationType.EItemOperationType.Confiscate:
				{
					bool flag16 = UIManager.Instance.IsFocusElement(UIElement.EventWindow);
					if (flag16)
					{
						bool isMiscResource = ItemTemplateHelper.IsMiscResource(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
						bool flag17 = !isMiscResource;
						if (flag17)
						{
							CS$<>8__locals2.limitCount = CS$<>8__locals1.<>4__this.MaxSelectCount - CS$<>8__locals1.<>4__this.TotalSelectedCount;
							CS$<>8__locals2.limitTip = LocalStringManager.Get(LanguageKey.LK_SelectCount_Limit_Confiscate);
						}
					}
					break;
				}
				case ItemOperationType.EItemOperationType.PutCraftResource:
				{
					Dictionary<sbyte, List<ItemKey>> dictionary2;
					long remainValue = (long)CS$<>8__locals1.<>4__this._valueNeeded - CS$<>8__locals1.<>4__this.GetPutResourceWorth(out dictionary2, CS$<>8__locals1.itemData);
					sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
					bool flag18 = resourceType == -1;
					if (flag18)
					{
						CS$<>8__locals2.limitCount = Convert.ToInt32((remainValue + CS$<>8__locals1.itemData.Value - 1L) / CS$<>8__locals1.itemData.Value);
					}
					else
					{
						CS$<>8__locals2.limitCount = (int)remainValue;
					}
					CS$<>8__locals2.limitTip = LocalStringManager.Get(LanguageKey.LK_SelectCount_Limit_Take);
					break;
				}
				}
				bool flag19 = CS$<>8__locals2.limitCount <= 0;
				if (!flag19)
				{
					BaseItemScrollView curMultiplyScrollView = CS$<>8__locals1.<>4__this.CurMultiplyScrollView;
					BaseItemScrollView baseItemScrollView = curMultiplyScrollView;
					ItemScrollView itemScrollView = baseItemScrollView as ItemScrollView;
					if (itemScrollView == null)
					{
						GroupedItemScrollView groupedItemScrollView = baseItemScrollView as GroupedItemScrollView;
						if (groupedItemScrollView == null)
						{
							CS$<>8__locals2.<OnRenderItemMultiply>g__Action|1(CS$<>8__locals1.view);
						}
						else
						{
							groupedItemScrollView.HandleClickItem(CS$<>8__locals1.itemData, CS$<>8__locals1.view, new Action<ItemView>(CS$<>8__locals2.<OnRenderItemMultiply>g__Action|1));
						}
					}
					else
					{
						itemScrollView.HandleClickItem(CS$<>8__locals1.itemData, CS$<>8__locals1.view, new Action<ItemView>(CS$<>8__locals2.<OnRenderItemMultiply>g__Action|1));
					}
				}
			}
		});
	}

	// Token: 0x06002411 RID: 9233 RVA: 0x00108674 File Offset: 0x00106874
	public void OnRenderItemExchangeTools(ItemDisplayData itemData, ItemView view)
	{
		MultiplyItemScrollView.<>c__DisplayClass95_0 CS$<>8__locals1 = new MultiplyItemScrollView.<>c__DisplayClass95_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.itemData = itemData;
		CS$<>8__locals1.view = view;
		this.SelectedMultiplyItemDict.TryGetValue(CS$<>8__locals1.itemData, out CS$<>8__locals1.selectedCount);
		CS$<>8__locals1.isSelected = (CS$<>8__locals1.selectedCount > 0);
		bool isSelected = CS$<>8__locals1.isSelected;
		if (isSelected)
		{
			CS$<>8__locals1.view.SetSelectedCount(CS$<>8__locals1.selectedCount);
		}
		CS$<>8__locals1.view.SetHighLight(CS$<>8__locals1.isSelected);
		CS$<>8__locals1.view.SetSelectState(CS$<>8__locals1.isSelected);
		CS$<>8__locals1.view.ShowDurability();
		bool isSelected2 = CS$<>8__locals1.isSelected;
		if (isSelected2)
		{
			int index = this.GetSelectedOrder(CS$<>8__locals1.itemData);
			CS$<>8__locals1.view.SetSelectedOrder(true, index);
			CS$<>8__locals1.view.HideInteractionState();
			bool flag = this.TotalSelectedCount <= 0;
			if (flag)
			{
				CS$<>8__locals1.view.SetInteractable(true);
			}
		}
		else
		{
			CS$<>8__locals1.view.SetSelectedOrder(false, 0);
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
			MultiplyItemScrollView.<>c__DisplayClass95_1 CS$<>8__locals2 = new MultiplyItemScrollView.<>c__DisplayClass95_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			CS$<>8__locals1.<>4__this.HideGradeLimitTip();
			bool isSelected3 = CS$<>8__locals1.isSelected;
			if (isSelected3)
			{
				CS$<>8__locals1.<>4__this.SetItemSelectCount(CS$<>8__locals1.itemData, 0, null);
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
					BaseItemScrollView curMultiplyScrollView = CS$<>8__locals1.<>4__this.CurMultiplyScrollView;
					BaseItemScrollView baseItemScrollView = curMultiplyScrollView;
					ItemScrollView itemScrollView = baseItemScrollView as ItemScrollView;
					if (itemScrollView == null)
					{
						GroupedItemScrollView groupedItemScrollView = baseItemScrollView as GroupedItemScrollView;
						if (groupedItemScrollView == null)
						{
							CS$<>8__locals2.<OnRenderItemExchangeTools>g__Action|1(CS$<>8__locals1.view);
						}
						else
						{
							groupedItemScrollView.HandleClickItem(CS$<>8__locals1.itemData, CS$<>8__locals1.view, new Action<ItemView>(CS$<>8__locals2.<OnRenderItemExchangeTools>g__Action|1));
						}
					}
					else
					{
						itemScrollView.HandleClickItem(CS$<>8__locals1.itemData, CS$<>8__locals1.view, new Action<ItemView>(CS$<>8__locals2.<OnRenderItemExchangeTools>g__Action|1));
					}
				}
			}
		});
	}

	// Token: 0x06002412 RID: 9234 RVA: 0x00108818 File Offset: 0x00106A18
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

	// Token: 0x06002413 RID: 9235 RVA: 0x00108880 File Offset: 0x00106A80
	public int GetAlreadySelectItemCount()
	{
		return this.SelectedMultiplyItemDict.Keys.Count;
	}

	// Token: 0x06002414 RID: 9236 RVA: 0x001088A4 File Offset: 0x00106AA4
	public int GetSelectedOrder(ItemDisplayData itemData)
	{
		return this.SelectedMultiplyItemOrderedList.IndexOf(itemData);
	}

	// Token: 0x06002415 RID: 9237 RVA: 0x001088C2 File Offset: 0x00106AC2
	public void SetFilter(Func<ItemDisplayData, bool> filter)
	{
		this._filter = filter;
	}

	// Token: 0x06002416 RID: 9238 RVA: 0x001088CC File Offset: 0x00106ACC
	private bool SetItemSelectCount(ItemView itemView, int count, List<ItemDisplayData> toolList)
	{
		bool selectIsSuccess = this.SetItemSelectCount(itemView.Data, count, toolList);
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
		this.CheckPutResource();
		return selectIsSuccess;
	}

	// Token: 0x06002417 RID: 9239 RVA: 0x00108930 File Offset: 0x00106B30
	private long GetPutResourceWorth(out Dictionary<sbyte, List<ItemKey>> keyDic, ItemDisplayData execption = null)
	{
		keyDic = new Dictionary<sbyte, List<ItemKey>>();
		long value = 0L;
		foreach (KeyValuePair<ItemDisplayData, int> item in this.SelectedMultiplyItemDict)
		{
			bool flag = execption != null && execption.Key == item.Key.Key;
			if (!flag)
			{
				bool flag2 = !ItemTemplateHelper.IsMiscResource(item.Key.Key.ItemType, item.Key.Key.TemplateId);
				if (flag2)
				{
					value += item.Key.Value * (long)item.Value;
				}
				else
				{
					value += (long)item.Value;
				}
				bool flag3 = !keyDic.ContainsKey(item.Key.ItemSourceType);
				if (flag3)
				{
					keyDic[item.Key.ItemSourceType] = new List<ItemKey>();
				}
				keyDic[item.Key.ItemSourceType].Add(item.Key.Key);
			}
		}
		return value;
	}

	// Token: 0x06002418 RID: 9240 RVA: 0x00108A70 File Offset: 0x00106C70
	private void CheckPutResource()
	{
		bool flag = this.CurrItemOperation == ItemOperationType.EItemOperationType.PutCraftResource;
		if (flag)
		{
			Dictionary<sbyte, List<ItemKey>> dic;
			long value = this.GetPutResourceWorth(out dic, null);
			ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("Value", value);
			args.SetObject("KeyDic", dic);
			GEvent.OnEvent(UiEvents.OnPutResourcePreview, args);
		}
	}

	// Token: 0x06002419 RID: 9241 RVA: 0x00108AC8 File Offset: 0x00106CC8
	private bool SetItemSelectCount(ItemDisplayData data, int count, List<ItemDisplayData> toolList)
	{
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
			List<ValueTuple<ItemDisplayData, int>> changeList = new List<ValueTuple<ItemDisplayData, int>>
			{
				new ValueTuple<ItemDisplayData, int>(data, count)
			};
			this.SendItemMultiplyOperationContentChange(changeList);
			this.CurMultiplyScrollView.ReRender();
			result = true;
		}
		return result;
	}

	// Token: 0x0600241A RID: 9242 RVA: 0x00108D10 File Offset: 0x00106F10
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
		this.SendItemMultiplyOperationContentChange(null);
		this.CurMultiplyScrollView.ReRender();
	}

	// Token: 0x0600241B RID: 9243 RVA: 0x00108DE0 File Offset: 0x00106FE0
	private void RefreshSelectedCount()
	{
		this.TotalSelectedCount = this.SelectedMultiplyItemDict.Sum(delegate(KeyValuePair<ItemDisplayData, int> p)
		{
			ItemDisplayData data = p.Key;
			return ItemTemplateHelper.IsMiscResource(data.Key.ItemType, data.Key.TemplateId) ? 1 : p.Value;
		});
	}

	// Token: 0x0600241C RID: 9244 RVA: 0x00108E14 File Offset: 0x00107014
	private void SendItemMultiplyOperationContentChange([TupleElementNames(new string[]
	{
		"itemKey",
		"count"
	})] List<ValueTuple<ItemDisplayData, int>> changeList = null)
	{
		bool flag = this._multiplyOperationHandler == null;
		if (!flag)
		{
			bool flag2 = this._multiplyItemOperationType == ItemOperationType.EItemOperationType.Feeding;
			if (flag2)
			{
				int num;
				int num2;
				this.RefreshFeedState(out num, out num2);
				this.CurMultiplyScrollView.ReRender();
			}
			this.RefreshSelectedCount();
			ArgumentBox args = this.GetItemMultiplyViewArgs();
			GEvent.OnEvent(UiEvents.ItemMultiplyOperationContentChange, args);
			Action<List<ValueTuple<ItemDisplayData, int>>> onContentChange = this._onContentChange;
			if (onContentChange != null)
			{
				onContentChange(changeList);
			}
		}
	}

	// Token: 0x0600241D RID: 9245 RVA: 0x00108E88 File Offset: 0x00107088
	private void RefreshFeedState(out int leftDurability, out int leftTame)
	{
		leftDurability = 0;
		leftTame = 0;
		bool flag = this._multiplyItemOperationType != ItemOperationType.EItemOperationType.Feeding;
		if (!flag)
		{
			int addDurability = this.SelectedMultiplyItemDict.Sum((KeyValuePair<ItemDisplayData, int> pair) => GameData.Domains.Extra.SharedMethods.GetFoodAddCarrierDurability(this._enterFeedingItem.Key.TemplateId, pair.Key.Key.TemplateId, pair.Value));
			int curDurability = Mathf.Clamp(addDurability + (int)this._enterFeedingItem.Durability, 0, (int)this._enterFeedingItem.MaxDurability);
			this._isFeedDurabilityMax = (curDurability >= (int)this._enterFeedingItem.MaxDurability);
			leftDurability = (int)this._enterFeedingItem.MaxDurability - curDurability;
			bool flag2 = ItemTemplateHelper.HasCarrierTame(this._enterFeedingItem.Key.ItemType, this._enterFeedingItem.Key.TemplateId);
			if (flag2)
			{
				int addTame = this.SelectedMultiplyItemDict.Sum((KeyValuePair<ItemDisplayData, int> pair) => GameData.Domains.Extra.SharedMethods.GetFoodAddCarrierTamePoint(this._enterFeedingItem.Key.TemplateId, pair.Key.Key.TemplateId, pair.Value));
				int curTame = Mathf.Clamp(addTame + this._enterFeedingItem.CarrierTamePoint, 0, 100);
				this._isFeedTameMax = (curTame >= 100);
				leftTame = 100 - curTame;
			}
			else
			{
				this._isFeedTameMax = true;
			}
		}
	}

	// Token: 0x0600241E RID: 9246 RVA: 0x00108F8C File Offset: 0x0010718C
	public void RefreshMultiplyAvailableTool()
	{
		List<ItemDisplayData> itemList = new List<ItemDisplayData>();
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
	}

	// Token: 0x0600241F RID: 9247 RVA: 0x0010901C File Offset: 0x0010721C
	private void OnClickMultiplyFeed()
	{
		List<MultiplyOperation> operationList = this._multiplyOperationHandler.GetOperationList(false);
		foreach (MultiplyOperation operation in operationList)
		{
			ExtraDomainMethod.Call.FeedCarrier(this._enterFeedingItem.Key, operation.Target, operation.Count, (ItemSourceType)operation.TargetItemSourceType);
			this.DealItemDisplayData(operation.Target, false);
			int addDurability = GameData.Domains.Extra.SharedMethods.GetFoodAddCarrierDurability(this._enterFeedingItem.Key.TemplateId, operation.Target.TemplateId, 1);
			this._enterFeedingItem.Durability = (short)Mathf.Clamp((int)this._enterFeedingItem.Durability + addDurability, 0, (int)this._enterFeedingItem.MaxDurability);
			int addTame = GameData.Domains.Extra.SharedMethods.GetFoodAddCarrierTamePoint(this._enterFeedingItem.Key.TemplateId, operation.Target.TemplateId, 1);
			this._enterFeedingItem.CarrierTamePoint = Mathf.Clamp(this._enterFeedingItem.CarrierTamePoint + addTame, 0, 100);
		}
		int num;
		int num2;
		this.RefreshFeedState(out num, out num2);
		this.CurMultiplyScrollView.SetItemList(ref this._canOperateItems);
		this.RefreshItems();
	}

	// Token: 0x06002420 RID: 9248 RVA: 0x00109160 File Offset: 0x00107360
	private void DealItemDisplayData(ItemKey target, bool isAdd)
	{
	}

	// Token: 0x06002421 RID: 9249 RVA: 0x00109164 File Offset: 0x00107364
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

	// Token: 0x06002422 RID: 9250 RVA: 0x001091FA File Offset: 0x001073FA
	private void MultiplyDiscard()
	{
	}

	// Token: 0x06002423 RID: 9251 RVA: 0x00109200 File Offset: 0x00107400
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

	// Token: 0x06002424 RID: 9252 RVA: 0x00109298 File Offset: 0x00107498
	private void MultiplyDisassemble()
	{
		List<MultiplyOperation> operationList = this._multiplyOperationHandler.GetOperationList(false);
		this.ClearMultiplyItemUsingState(operationList);
		this.RefreshUsedTool();
		ItemDomainMethod.AsyncCall.DisassembleItemList(null, MultiplyItemScrollView.TaiwuCharId, operationList, delegate(int offset, RawDataPool dataPool)
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
				List<ItemDisplayData> usedToolList = allUsedToolDict.Keys.ToPoolList<ItemDisplayData>();
				usedToolList.RemoveAll((ItemDisplayData data) => data.Durability > 0);
				argBox.SetObject("UsedToolList", usedToolList);
				UIElement.GetItem.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.GetItem);
				Action onGetItemHide = null;
				onGetItemHide = delegate()
				{
					EasyPool.Free<List<ItemDisplayData>>(resultList);
					EasyPool.Free<List<ItemDisplayData>>(usedToolList);
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
			this._multiplyOperationHandler.Clear();
			this.RefreshMultiplyCanOperateItems();
			this.RefreshMultiplyAvailableTool();
			bool flag3 = this.OtherScrollView;
			if (flag3)
			{
				List<ItemDisplayData> otherMultiplyItems = this.OtherItems;
				this.OtherScrollView.SetItemList(ref otherMultiplyItems);
			}
			this.SendItemMultiplyOperationContentChange(null);
		});
	}

	// Token: 0x06002425 RID: 9253 RVA: 0x001092DB File Offset: 0x001074DB
	private void OnClickMultiplyRepair()
	{
		this.MultiplyRepair();
	}

	// Token: 0x06002426 RID: 9254 RVA: 0x001092E8 File Offset: 0x001074E8
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
				this._canOperateItems.Remove(data);
			}
		}
		BuildingDomainMethod.AsyncCall.RepairItemList(null, MultiplyItemScrollView.TaiwuCharId, operationList, delegate(int offset, RawDataPool dataPool)
		{
			List<ItemDisplayData> dataList = EasyPool.Get<List<ItemDisplayData>>();
			dataList.Clear();
			Serializer.Deserialize(dataPool, offset, ref dataList);
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Clear();
			argBox.SetObject("ItemList", dataList);
			argBox.Set("ObtainType", 7);
			Dictionary<ItemDisplayData, short> allUsedToolDict = this._multiplyOperationHandler.AllUsedToolDict;
			List<ItemDisplayData> toolList = allUsedToolDict.Keys.ToPoolList<ItemDisplayData>();
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
				EasyPool.Free<List<ItemDisplayData>>(toolList);
				EasyPool.Free<ArgumentBox>(argBox);
				UIElement getItem2 = UIElement.GetItem;
				getItem2.OnHide = (Action)Delegate.Remove(getItem2.OnHide, onGetItemHide);
			};
			UIElement getItem = UIElement.GetItem;
			getItem.OnHide = (Action)Delegate.Combine(getItem.OnHide, onGetItemHide);
			this.RefreshUsedTool();
			this._multiplyOperationHandler.Clear();
			this.RefreshMultiplyAvailableTool();
			this.CurMultiplyScrollView.SetItemList(ref this._canOperateItems);
			bool flag = this.OtherScrollView;
			if (flag)
			{
				List<ItemDisplayData> otherMultiplyItems = this.OtherItems;
				this.OtherScrollView.SetItemList(ref otherMultiplyItems);
			}
			this.SendItemMultiplyOperationContentChange(null);
		});
	}

	// Token: 0x06002427 RID: 9255 RVA: 0x00109398 File Offset: 0x00107598
	private void OnClickMultiplyPutPoisonMaterial()
	{
	}

	// Token: 0x06002428 RID: 9256 RVA: 0x0010939C File Offset: 0x0010759C
	private void OnClickMultiplyVillagerCraft()
	{
		ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("SelectedMultiplyItemDict", this.SelectedMultiplyItemDict);
		GEvent.OnEvent(UiEvents.OnConfirmVillagerCraftInputMaterial, args);
	}

	// Token: 0x06002429 RID: 9257 RVA: 0x001093D1 File Offset: 0x001075D1
	private void OnClickMultiplyTransfer()
	{
	}

	// Token: 0x0600242A RID: 9258 RVA: 0x001093D4 File Offset: 0x001075D4
	public void EnterMultiplyMode(bool isEmptyTool = false)
	{
		bool isMultiItemSelect = this.IsMultiItemSelect;
		if (!isMultiItemSelect)
		{
			this.IsMultiItemSelect = true;
			this._currItemOperation = this._multiplyItemOperationType;
			this.IsEmptyTool = isEmptyTool;
			CButtonObsolete btnEmptyTool = this.BtnEmptyTool;
			if (btnEmptyTool != null)
			{
				btnEmptyTool.gameObject.SetActive(this.NeedShowBtnEmptyTool);
			}
			this.ClearMultiplySelection();
			this.HideMultiplySelectButton();
			this.RefreshMultiplyCanOperateItems();
			bool flag = this._inventoryScroll;
			if (flag)
			{
				this._inventoryScroll.MySortAndFilter.SaveFilter();
			}
			bool flag2 = this._warehouseScroll;
			if (flag2)
			{
				this._warehouseScroll.MySortAndFilter.SaveFilter();
			}
			this.RefreshSort();
			bool openMultiOperation = this.OpenMultiOperation;
			if (openMultiOperation)
			{
				ArgumentBox args = this.GetItemMultiplyViewArgs();
				UIElement.ItemMultiplyOperationOld.SetOnInitArgs(args);
				UIManager.Instance.ShowUI(UIElement.ItemMultiplyOperationOld, true);
			}
			Action onEnterMultiplyOperation = this._onEnterMultiplyOperation;
			if (onEnterMultiplyOperation != null)
			{
				onEnterMultiplyOperation();
			}
		}
	}

	// Token: 0x0600242B RID: 9259 RVA: 0x001094CC File Offset: 0x001076CC
	public void EnterSpecialBreakConvertToExpMultiplyMode()
	{
		bool isMultiItemSelect = this.IsMultiItemSelect;
		if (!isMultiItemSelect)
		{
			this.IsMultiItemSelect = true;
			this._currItemOperation = (this._multiplyItemOperationType = ItemOperationType.EItemOperationType.SpecialBreakConvertToExp);
			this.IsEmptyTool = false;
			CButtonObsolete btnEmptyTool = this.BtnEmptyTool;
			if (btnEmptyTool != null)
			{
				btnEmptyTool.gameObject.SetActive(this.NeedShowBtnEmptyTool);
			}
			this.RefreshMultiplyCanOperateItems();
			this.ClearMultiplySelection();
		}
	}

	// Token: 0x0600242C RID: 9260 RVA: 0x00109534 File Offset: 0x00107734
	public void EnterKongsangSpecialInteractMultiplyMode()
	{
		bool isMultiItemSelect = this.IsMultiItemSelect;
		if (!isMultiItemSelect)
		{
			this.IsMultiItemSelect = true;
			this._currItemOperation = (this._multiplyItemOperationType = ItemOperationType.EItemOperationType.KongsangSpecialInteract);
			this.IsEmptyTool = false;
			CButtonObsolete btnEmptyTool = this.BtnEmptyTool;
			if (btnEmptyTool != null)
			{
				btnEmptyTool.gameObject.SetActive(this.NeedShowBtnEmptyTool);
			}
			this.RefreshMultiplyCanOperateItems();
			this.ClearMultiplySelection();
		}
	}

	// Token: 0x0600242D RID: 9261 RVA: 0x0010959C File Offset: 0x0010779C
	public void EnterFeedingMode(ItemDisplayData itemData)
	{
		this._multiplyItemOperationType = ItemOperationType.EItemOperationType.Feeding;
		this._enterFeedingItem = itemData;
		int num;
		int num2;
		this.RefreshFeedState(out num, out num2);
		this.EnterMultiplyMode(false);
	}

	// Token: 0x0600242E RID: 9262 RVA: 0x001095CB File Offset: 0x001077CB
	public void EnterRepairMode(bool isEmptyTool)
	{
		this._multiplyItemOperationType = ItemOperationType.EItemOperationType.Repair;
		this.EnterMultiplyMode(isEmptyTool);
	}

	// Token: 0x0600242F RID: 9263 RVA: 0x001095DD File Offset: 0x001077DD
	public void EnterDisassembleMode(bool isEmptyTool)
	{
		this._multiplyItemOperationType = ItemOperationType.EItemOperationType.Disassemble;
		this.EnterMultiplyMode(isEmptyTool);
	}

	// Token: 0x06002430 RID: 9264 RVA: 0x001095F0 File Offset: 0x001077F0
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

	// Token: 0x06002431 RID: 9265 RVA: 0x00109630 File Offset: 0x00107830
	public void EnterPutCraftResourceMode(int need)
	{
		bool isMultiItemSelect = this.IsMultiItemSelect;
		if (!isMultiItemSelect)
		{
			this._valueNeeded = need;
			this.IsMultiItemSelect = true;
			this._currItemOperation = (this._multiplyItemOperationType = ItemOperationType.EItemOperationType.PutCraftResource);
			this.RefreshMultiplyCanOperateItems();
			this.ClearMultiplySelection();
		}
	}

	// Token: 0x06002432 RID: 9266 RVA: 0x00109678 File Offset: 0x00107878
	public void TrySelectItemInValue(Func<ItemDisplayData, bool> predicate)
	{
		MultiplyItemScrollView.<>c__DisplayClass128_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		bool flag = this._currItemOperation != ItemOperationType.EItemOperationType.PutCraftResource;
		if (!flag)
		{
			Dictionary<sbyte, List<ItemKey>> dictionary;
			long curValue = this.GetPutResourceWorth(out dictionary, null);
			List<ItemDisplayData> tempList = this.SelectedMultiplyItemDict.Keys.ToList<ItemDisplayData>();
			CS$<>8__locals1.candidates = this._inventoryScroll.SortAndFilter.OutputItemList.Where(predicate).ToList<ItemDisplayData>();
			bool needSelect = true;
			bool flag2 = curValue >= (long)this._valueNeeded;
			if (flag2)
			{
				needSelect = false;
			}
			else
			{
				needSelect = false;
				foreach (ItemDisplayData candidate in CS$<>8__locals1.candidates)
				{
					int count;
					bool flag3 = !this.SelectedMultiplyItemDict.TryGetValue(candidate, out count) || count < candidate.Amount;
					if (flag3)
					{
						needSelect = true;
						break;
					}
				}
			}
			foreach (ItemDisplayData item in tempList)
			{
				this.SetItemSelectCount(item, 0, null);
			}
			bool flag4 = !needSelect;
			if (flag4)
			{
				this.CheckPutResource();
			}
			else
			{
				int remain = this._valueNeeded;
				for (int i = 0; i < CS$<>8__locals1.candidates.Count; i++)
				{
					int tempAmount = CS$<>8__locals1.candidates[i].Amount;
					while (tempAmount > 0 && remain > 0)
					{
						tempAmount--;
						remain -= (int)CS$<>8__locals1.candidates[i].Value;
					}
					bool flag5 = remain < 0 && this.<TrySelectItemInValue>g__TrySetCandidateSmaller|128_0((int)CS$<>8__locals1.candidates[i].Value + remain, i + 1, CS$<>8__locals1.candidates[i].Grade, ref CS$<>8__locals1);
					if (flag5)
					{
						tempAmount++;
					}
					this.SetItemSelectCount(CS$<>8__locals1.candidates[i], CS$<>8__locals1.candidates[i].Amount - tempAmount, null);
					bool flag6 = remain <= 0;
					if (flag6)
					{
						break;
					}
				}
				this.CheckPutResource();
			}
		}
	}

	// Token: 0x06002433 RID: 9267 RVA: 0x001098D8 File Offset: 0x00107AD8
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

	// Token: 0x06002434 RID: 9268 RVA: 0x0010991C File Offset: 0x00107B1C
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

	// Token: 0x06002435 RID: 9269 RVA: 0x0010995C File Offset: 0x00107B5C
	private void RefreshMultiplyCanOperateItems()
	{
		this._canOperateItems.Clear();
		List<ItemDisplayData> items = this.CurMultiplyItems;
		IEnumerable<ItemDisplayData> enumerable;
		if (this._customItems == null)
		{
			enumerable = items.Where(delegate(ItemDisplayData d)
			{
				switch (this._currItemOperation)
				{
				case ItemOperationType.EItemOperationType.Invalid:
					return false;
				case ItemOperationType.EItemOperationType.Repair:
					return d.Durability < d.MaxDurability && ItemTemplateHelper.IsRepairable(d.Key.ItemType, d.Key.TemplateId);
				case ItemOperationType.EItemOperationType.Disassemble:
					return ItemTemplateHelper.GetCanDisassemble(d.Key.ItemType, d.Key.TemplateId);
				case ItemOperationType.EItemOperationType.Transfer:
					return this.CheckItemIsRealTransferable(d);
				case ItemOperationType.EItemOperationType.Discard:
					return this.CheckItemIsRealTransferable(d);
				case ItemOperationType.EItemOperationType.SpecialBreakConvertToExp:
				{
					bool flag = !ItemTemplateHelper.IsTransferable(d.Key.ItemType, d.Key.TemplateId);
					return !flag;
				}
				case ItemOperationType.EItemOperationType.Feeding:
					return ItemTemplateHelper.IsFeedingAble(d.Key.ItemType, d.Key.TemplateId);
				case ItemOperationType.EItemOperationType.Confiscate:
					return true;
				case ItemOperationType.EItemOperationType.PutPoisonMaterial:
				{
					bool flag2 = d.Key.ItemType == 5;
					if (flag2)
					{
						MaterialItem config = Config.Material.Instance[d.Key.TemplateId];
						return config.InnatePoisons.IsNonZero();
					}
					bool flag3 = d.Key.ItemType == 4;
					if (flag3)
					{
						CarrierItem config2 = Carrier.Instance[d.Key.TemplateId];
						return config2.InnatePoisons.IsNonZero();
					}
					return false;
				}
				case ItemOperationType.EItemOperationType.ExchangeTools:
					return true;
				case ItemOperationType.EItemOperationType.FixItem:
					return true;
				case ItemOperationType.EItemOperationType.VillagerCraft:
					return this._filter(d);
				case ItemOperationType.EItemOperationType.KongsangSpecialInteract:
					return EatingItems.IsWugKing(d.Key);
				}
				throw new ArgumentOutOfRangeException();
			});
		}
		else
		{
			IEnumerable<ItemDisplayData> customItems = this._customItems;
			enumerable = customItems;
		}
		IEnumerable<ItemDisplayData> itemList = enumerable;
		this._canOperateItems.AddRange(itemList);
		this.CurMultiplyScrollView.SetItemList(ref this._canOperateItems);
		Dictionary<ItemDisplayData, int> reselectDict = EasyPool.Get<Dictionary<ItemDisplayData, int>>();
		reselectDict.Clear();
		reselectDict.AddRange(this.SelectedMultiplyItemDict);
		this._multiplyOperationHandler.Clear();
		this.RefreshMultiplyAvailableTool();
		foreach (KeyValuePair<ItemDisplayData, int> pair in reselectDict)
		{
			ItemDisplayData itemData = pair.Key;
			int amount = pair.Value;
			ItemOperationType.EItemOperationType currItemOperation = this._currItemOperation;
			ItemOperationType.EItemOperationType eitemOperationType = currItemOperation;
			if (eitemOperationType > ItemOperationType.EItemOperationType.Disassemble)
			{
			}
			List<ItemDisplayData> availableToolList = this.GetAvailableToolList(itemData);
			this.SetItemSelectCount(itemData, amount, availableToolList);
		}
		EasyPool.Free<Dictionary<ItemDisplayData, int>>(reselectDict);
	}

	// Token: 0x06002436 RID: 9270 RVA: 0x00109A74 File Offset: 0x00107C74
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

	// Token: 0x06002437 RID: 9271 RVA: 0x00109ADC File Offset: 0x00107CDC
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
					args.SetObject("EnterFeedingItem", this._enterFeedingItem);
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
						bool flag2 = id > -1;
						if (flag2)
						{
							certainMaterialList.Add(id);
						}
					}
					List<short> baseMaterial = ItemTemplateHelper.GetAllDisassemblyMaterial(itemData2.Key.ItemType, itemData2.Key.TemplateId);
					bool flag3 = baseMaterial != null;
					if (flag3)
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
		bool flag4 = UIManager.Instance.IsElementActive(UIElement.Warehouse);
		if (flag4)
		{
			RectTransform rectTrans = this.IsInventoryMultiply ? base.CGet<RectTransform>("InventoryMultiplyOperationPos") : base.CGet<RectTransform>("WarehouseMultiplyOperationPos");
			args.SetObject("Pos", rectTrans.position);
			args.Set("Height", rectTrans.rect.height);
		}
		return args;
	}

	// Token: 0x06002438 RID: 9272 RVA: 0x00109E4C File Offset: 0x0010804C
	public void TryExitMultiplyMode()
	{
		bool flag = this.SelectedMultiplyItemDict.Count == 0;
		if (flag)
		{
			this.ExitMultiplyMode();
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
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}
	}

	// Token: 0x06002439 RID: 9273 RVA: 0x00109EE8 File Offset: 0x001080E8
	public void ExitMultiplyMode()
	{
		this.ShowMultiplySelectButton(null, true);
		bool flag = !this.IsMultiItemSelect;
		if (!flag)
		{
			this.IsMultiItemSelect = false;
			this.IsEmptyTool = false;
			this.HideGradeLimitTip();
			CButtonObsolete btnEmptyTool = this.BtnEmptyTool;
			if (btnEmptyTool != null)
			{
				btnEmptyTool.gameObject.SetActive(false);
			}
			this.ClearMultiplySelection();
			this._currItemOperation = ItemOperationType.EItemOperationType.Invalid;
			this._multiplyItemOperationType = ItemOperationType.EItemOperationType.Invalid;
			this.RefreshItems();
			this.RefreshSort();
			bool isInventoryMultiply = this.IsInventoryMultiply;
			if (isInventoryMultiply)
			{
				this._inventoryScroll.MySortAndFilter.LoadFilter();
			}
			else
			{
				this._warehouseScroll.MySortAndFilter.LoadFilter();
			}
			Action onExitMultiplyOperation = this._onExitMultiplyOperation;
			if (onExitMultiplyOperation != null)
			{
				onExitMultiplyOperation();
			}
		}
	}

	// Token: 0x0600243A RID: 9274 RVA: 0x00109FA3 File Offset: 0x001081A3
	public void ClearMultiplySelection()
	{
		this.TotalSelectedCount = 0;
		this._multiplyOperationHandler.Clear();
		this.CurMultiplyScrollView.ReRender();
	}

	// Token: 0x0600243B RID: 9275 RVA: 0x00109FC6 File Offset: 0x001081C6
	private void ClearMultiplyItemUsingState(List<MultiplyOperation> multiplyOperationList)
	{
	}

	// Token: 0x0600243C RID: 9276 RVA: 0x00109FC9 File Offset: 0x001081C9
	public void OnExitMultiplyOperation(ArgumentBox argsBox)
	{
		this.ExitMultiplyMode();
	}

	// Token: 0x0600243D RID: 9277 RVA: 0x00109FD4 File Offset: 0x001081D4
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
			CButtonObsolete btnEmptyTool = this.BtnEmptyTool;
			if (btnEmptyTool != null)
			{
				btnEmptyTool.gameObject.SetActive(this.NeedShowBtnEmptyTool);
			}
		}
	}

	// Token: 0x0600243E RID: 9278 RVA: 0x0010A054 File Offset: 0x00108254
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

	// Token: 0x0600243F RID: 9279 RVA: 0x0010A0EC File Offset: 0x001082EC
	public void OnItemMultiplyOperationCancelSelection(ArgumentBox argsBox)
	{
		ItemDisplayData itemData;
		bool flag = argsBox.Get<ItemDisplayData>("ItemData", out itemData);
		if (flag)
		{
			Dictionary<ItemDisplayData, short> usedToolDict = this._multiplyOperationHandler.GetUsedToolDict(itemData);
			List<ItemDisplayData> usedToolList = new List<ItemDisplayData>(usedToolDict.Keys);
			this.SetItemSelectCount(itemData, 0, usedToolList);
		}
	}

	// Token: 0x06002440 RID: 9280 RVA: 0x0010A130 File Offset: 0x00108330
	private void RefreshSort()
	{
		ItemOperationType.EItemOperationType currItemOperation = this._currItemOperation;
		ItemOperationType.EItemOperationType eitemOperationType = currItemOperation;
		if (eitemOperationType <= ItemOperationType.EItemOperationType.Disassemble)
		{
			if (eitemOperationType == ItemOperationType.EItemOperationType.Repair)
			{
				this.CurMultiplyScrollView.SortAndFilter.ShowFilterType(ItemSortAndFilter.ItemFilterType.Equip);
				this.CurMultiplyScrollView.SortAndFilter.LockFilterType(ItemSortAndFilter.ItemFilterType.Equip, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
				return;
			}
			if (eitemOperationType == ItemOperationType.EItemOperationType.Disassemble)
			{
				this.CurMultiplyScrollView.SortAndFilter.ShowFilterType(UI_CharacterMenuItems.DisassembleFilterTypes);
				this.CurMultiplyScrollView.SortAndFilter.LockFilterType(UI_CharacterMenuItems.DisassembleFilterTypes, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
				return;
			}
		}
		else
		{
			if (eitemOperationType == ItemOperationType.EItemOperationType.Feeding)
			{
				this.CurMultiplyScrollView.SortAndFilter.ShowFilterType(ItemSortAndFilter.ItemFilterType.Material);
				this.CurMultiplyScrollView.SortAndFilter.LockFilterType(ItemSortAndFilter.ItemFilterType.Material, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
				return;
			}
			if (eitemOperationType == ItemOperationType.EItemOperationType.KongsangSpecialInteract)
			{
				this.CurMultiplyScrollView.SortAndFilter.ShowFilterType(ItemSortAndFilter.ItemFilterType.Medicine);
				this.CurMultiplyScrollView.SortAndFilter.LockFilterType(ItemSortAndFilter.ItemFilterType.Medicine, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
				return;
			}
		}
		this.CurMultiplyScrollView.SortAndFilter.ShowFilterType(ItemSortAndFilter.ItemFilterType.Invalid);
		this.CurMultiplyScrollView.SortAndFilter.LockFilterType(ItemSortAndFilter.ItemFilterType.Invalid, ItemSortAndFilter.LockFilterTypeToggleActionMode.Default);
	}

	// Token: 0x06002441 RID: 9281 RVA: 0x0010A23C File Offset: 0x0010843C
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

	// Token: 0x06002442 RID: 9282 RVA: 0x0010A32C File Offset: 0x0010852C
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

	// Token: 0x06002443 RID: 9283 RVA: 0x0010A498 File Offset: 0x00108698
	private void ClearItemUsingState(ItemDisplayData itemData)
	{
		bool flag = this.IsMultiItemSelect && this.CurMultiplyItemSourceType != ItemSourceType.Inventory;
		if (!flag)
		{
			ItemDisplayData.ClearItemUsingState(itemData, this.CurInventoryItems);
		}
	}

	// Token: 0x06002444 RID: 9284 RVA: 0x0010A4D0 File Offset: 0x001086D0
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

	// Token: 0x06002445 RID: 9285 RVA: 0x0010A53C File Offset: 0x0010873C
	public bool CheckCurItemHasGradeLimit(ItemKey itemKey)
	{
		return this.CheckItemHasGradeLimit(itemKey, this.IsInventoryMultiply);
	}

	// Token: 0x06002446 RID: 9286 RVA: 0x0010A54C File Offset: 0x0010874C
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

	// Token: 0x06002447 RID: 9287 RVA: 0x0010A5CC File Offset: 0x001087CC
	public bool CheckItemGradeLower(ItemKey itemKey, bool isInventory)
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
			result = (grade < 0 || grade > ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId));
		}
		return result;
	}

	// Token: 0x06002448 RID: 9288 RVA: 0x0010A64C File Offset: 0x0010884C
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

	// Token: 0x06002449 RID: 9289 RVA: 0x0010A6B8 File Offset: 0x001088B8
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

	// Token: 0x0600244A RID: 9290 RVA: 0x0010A728 File Offset: 0x00108928
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

	// Token: 0x0600244B RID: 9291 RVA: 0x0010A7E8 File Offset: 0x001089E8
	private void ShowGradeLimitTip(ItemView itemView, string content)
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

	// Token: 0x0600244C RID: 9292 RVA: 0x0010A970 File Offset: 0x00108B70
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

	// Token: 0x0600244D RID: 9293 RVA: 0x0010A9B0 File Offset: 0x00108BB0
	public void RefreshItems()
	{
		bool isMultiItemSelect = this.IsMultiItemSelect;
		if (isMultiItemSelect)
		{
			this._multiplyOperationHandler.Clear();
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
				this._inventoryScroll.SetItemList(ref items, false, null, false, null);
			}
			else
			{
				List<ItemDisplayData> items2 = this.CurWarehouseItems;
				this._warehouseScroll.SetItemList(ref items2);
			}
		}
	}

	// Token: 0x0600244E RID: 9294 RVA: 0x0010AA34 File Offset: 0x00108C34
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
			button.GetComponentInChildren<MonoJoint>(true).JointSync();
		}
	}

	// Token: 0x0600244F RID: 9295 RVA: 0x0010AA9C File Offset: 0x00108C9C
	public void HideMultiplySelectButton()
	{
		CButtonObsolete btnMultiplySelect = this.BtnMultiplySelect;
		if (btnMultiplySelect != null)
		{
			btnMultiplySelect.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002451 RID: 9297 RVA: 0x0010AB08 File Offset: 0x00108D08
	[CompilerGenerated]
	private void <Init>g__InitButton|91_0(Refers refers, bool isInventory)
	{
		CButtonObsolete btnMultiplyOption;
		bool flag = refers.CTryGet<CButtonObsolete>("BtnMultiplyOption", out btnMultiplyOption);
		if (flag)
		{
			btnMultiplyOption.gameObject.SetActive(this.CurCharacterIsTaiwu && this._isTaiWuTeam);
			btnMultiplyOption.ClearAndAddListener(delegate
			{
				this.IsInventoryMultiply = isInventory;
				this.OpenMultiplyOption(btnMultiplyOption);
			});
		}
		CButtonObsolete btnMultiplySelect;
		bool flag2 = refers.CTryGet<CButtonObsolete>("BtnMultiplySelect", out btnMultiplySelect);
		if (flag2)
		{
			this.ShowMultiplySelectButton(btnMultiplySelect, true);
			bool flag3 = btnMultiplySelect;
			if (flag3)
			{
				btnMultiplySelect.ClearAndAddListener(delegate
				{
					this.IsInventoryMultiply = isInventory;
					this.EnterMultiplyMode(false);
				});
			}
		}
		CButtonObsolete btnEmptyTool;
		bool flag4 = refers.CTryGet<CButtonObsolete>("BtnEmptyTool", out btnEmptyTool);
		if (flag4)
		{
			GameObject checkMark = btnEmptyTool.GetComponent<Refers>().CGet<GameObject>("CheckMark");
			checkMark.SetActive(false);
			btnEmptyTool.ClearAndAddListener(delegate
			{
				this.IsEmptyTool = !this.IsEmptyTool;
				this.CurMultiplyScrollView.ReRender();
			});
		}
	}

	// Token: 0x06002458 RID: 9304 RVA: 0x0010B158 File Offset: 0x00109358
	[CompilerGenerated]
	private bool <TrySelectItemInValue>g__TrySetCandidateSmaller|128_0(int targetValue, int startIndex, sbyte grade, ref MultiplyItemScrollView.<>c__DisplayClass128_0 A_4)
	{
		int currentValue = int.MaxValue;
		int targetIndex = -1;
		int currentGrade = (int)grade;
		for (int i = startIndex; i < A_4.candidates.Count; i++)
		{
			bool flag = A_4.candidates[i].Value >= (long)targetValue && A_4.candidates[i].Value <= (long)currentValue && (int)A_4.candidates[i].Grade < currentGrade;
			if (flag)
			{
				targetIndex = i;
				currentGrade = (int)A_4.candidates[i].Grade;
				currentValue = (int)A_4.candidates[i].Value;
			}
		}
		bool flag2 = targetIndex >= 0;
		if (flag2)
		{
			this.SetItemSelectCount(A_4.candidates[targetIndex], 1, null);
		}
		return targetIndex >= 0;
	}

	// Token: 0x04001B39 RID: 6969
	private Dictionary<ItemSourceType, List<ItemDisplayData>> _itemDict;

	// Token: 0x04001B3A RID: 6970
	[NonSerialized]
	public long MaxWorthCanBeLentToTaiwu;

	// Token: 0x04001B3B RID: 6971
	private GroupedItemScrollView _warehouseScroll;

	// Token: 0x04001B3C RID: 6972
	private ItemScrollView _inventoryScroll;

	// Token: 0x04001B3D RID: 6973
	[NonSerialized]
	public ItemSourceType InventoryItemSourceType = ItemSourceType.Inventory;

	// Token: 0x04001B3E RID: 6974
	[NonSerialized]
	public ItemSourceType WarehouseItemSourceType = ItemSourceType.Warehouse;

	// Token: 0x04001B3F RID: 6975
	private List<ItemDisplayData> _customItems;

	// Token: 0x04001B40 RID: 6976
	[NonSerialized]
	public int CurCharId;

	// Token: 0x04001B41 RID: 6977
	private bool _isTaiWuTeam;

	// Token: 0x04001B42 RID: 6978
	private ItemOperationType.EItemOperationType _currItemOperation;

	// Token: 0x04001B43 RID: 6979
	private List<ItemDisplayData> _canOperateItems = new List<ItemDisplayData>();

	// Token: 0x04001B44 RID: 6980
	private ItemDisplayData _enterFeedingItem;

	// Token: 0x04001B45 RID: 6981
	private readonly MultiplyOperationHandler _multiplyOperationHandler = new MultiplyOperationHandler();

	// Token: 0x04001B46 RID: 6982
	private int _valueNeeded;

	// Token: 0x04001B48 RID: 6984
	public bool ReSelectAmountWhenSelected = false;

	// Token: 0x04001B49 RID: 6985
	private ItemOperationType.EItemOperationType _multiplyItemOperationType = ItemOperationType.EItemOperationType.Disassemble;

	// Token: 0x04001B4B RID: 6987
	private ItemGradeFilterSetting _itemGradeFilterSetting;

	// Token: 0x04001B4C RID: 6988
	private bool _isEmptyTool;

	// Token: 0x04001B4D RID: 6989
	private ResourceMonitor _resourceMonitor;

	// Token: 0x04001B4E RID: 6990
	private LifeSkillMonitor _lifeSkillMonitor;

	// Token: 0x04001B4F RID: 6991
	private Action _onEnterMultiplyOperation;

	// Token: 0x04001B50 RID: 6992
	private Action _onExitMultiplyOperation;

	// Token: 0x04001B51 RID: 6993
	[TupleElementNames(new string[]
	{
		"data",
		"count"
	})]
	private Action<List<ValueTuple<ItemDisplayData, int>>> _onContentChange;

	// Token: 0x04001B52 RID: 6994
	public bool IsMeet;

	// Token: 0x04001B55 RID: 6997
	private bool _isFeedDurabilityMax;

	// Token: 0x04001B56 RID: 6998
	private bool _isFeedTameMax;

	// Token: 0x04001B57 RID: 6999
	private Func<ItemDisplayData, bool> _filter;

	// Token: 0x04001B58 RID: 7000
	public bool OpenMultiOperation = true;
}
