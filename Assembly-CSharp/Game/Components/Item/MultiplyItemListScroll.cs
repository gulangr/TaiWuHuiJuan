using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CharacterDataMonitor;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item;
using Game.Views.CharacterMenu;
using Game.Views.Cricket.Combat;
using Game.Views.Item;
using Game.Views.Make;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Story;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Components.Item
{
	// Token: 0x02000EF4 RID: 3828
	public class MultiplyItemListScroll : MonoBehaviour
	{
		// Token: 0x0600AFC3 RID: 44995 RVA: 0x00500B6E File Offset: 0x004FED6E
		public void HighLightItemView(RowItem itemView)
		{
			this.CurMultiplyScrollView.HighLightItemView(itemView);
		}

		// Token: 0x0600AFC4 RID: 44996 RVA: 0x00500B7E File Offset: 0x004FED7E
		public void CancelHighLightItemView()
		{
			this.CurMultiplyScrollView.CancelHighLightItemView();
		}

		// Token: 0x0600AFC5 RID: 44997 RVA: 0x00500B8D File Offset: 0x004FED8D
		public bool IsTaiwuGearMate(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(charId);
		}

		// Token: 0x170013E7 RID: 5095
		// (get) Token: 0x0600AFC6 RID: 44998 RVA: 0x00500B9A File Offset: 0x004FED9A
		public long SelectedMultiplyItemTotalWorth
		{
			get
			{
				return this.SelectedMultiplyItemDict.Sum((KeyValuePair<ItemDisplayData, int> pair) => pair.Key.Value * (long)pair.Value);
			}
		}

		// Token: 0x170013E8 RID: 5096
		// (get) Token: 0x0600AFC7 RID: 44999 RVA: 0x00500BC6 File Offset: 0x004FEDC6
		private List<ItemDisplayData> Items
		{
			get
			{
				Dictionary<ItemSourceType, List<ItemDisplayData>> itemDict = this._itemDict;
				return (itemDict != null) ? itemDict.GetValueOrDefault(this.ItemSourceType) : null;
			}
		}

		// Token: 0x170013E9 RID: 5097
		// (get) Token: 0x0600AFC8 RID: 45000 RVA: 0x00500BE0 File Offset: 0x004FEDE0
		private static int TaiwuCharId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x170013EA RID: 5098
		// (get) Token: 0x0600AFC9 RID: 45001 RVA: 0x00500BEC File Offset: 0x004FEDEC
		private bool CurCharacterIsTaiwu
		{
			get
			{
				return this.CurCharId == MultiplyItemListScroll.TaiwuCharId;
			}
		}

		// Token: 0x170013EB RID: 5099
		// (get) Token: 0x0600AFCA RID: 45002 RVA: 0x00500BFB File Offset: 0x004FEDFB
		public ItemOperationType.EItemOperationType CurrItemOperation
		{
			get
			{
				return this._currItemOperation;
			}
		}

		// Token: 0x170013EC RID: 5100
		// (get) Token: 0x0600AFCB RID: 45003 RVA: 0x00500C03 File Offset: 0x004FEE03
		public Dictionary<ItemDisplayData, int> SelectedMultiplyItemDict
		{
			get
			{
				return this.CurOperationHandler.SelectedItemDict;
			}
		}

		// Token: 0x170013ED RID: 5101
		// (get) Token: 0x0600AFCC RID: 45004 RVA: 0x00500C10 File Offset: 0x004FEE10
		public List<ItemDisplayData> SelectedMultiplyItemOrderedList
		{
			get
			{
				return this.CurOperationHandler.SelectedItemOrderedList;
			}
		}

		// Token: 0x170013EE RID: 5102
		// (get) Token: 0x0600AFCD RID: 45005 RVA: 0x00500C1D File Offset: 0x004FEE1D
		private bool _isFeedingCricket
		{
			get
			{
				return this._feedingTarget != null && this._feedingTarget.Key.ItemType == 11;
			}
		}

		// Token: 0x170013EF RID: 5103
		// (get) Token: 0x0600AFCE RID: 45006 RVA: 0x00500C3E File Offset: 0x004FEE3E
		private MultiplyOperationHandler CurOperationHandler
		{
			get
			{
				return this._multiplyOperationHandlerDict[this.ItemSourceType];
			}
		}

		// Token: 0x170013F0 RID: 5104
		// (get) Token: 0x0600AFCF RID: 45007 RVA: 0x00500C51 File Offset: 0x004FEE51
		// (set) Token: 0x0600AFD0 RID: 45008 RVA: 0x00500C59 File Offset: 0x004FEE59
		public bool IsMultiItemSelect { get; private set; }

		// Token: 0x170013F1 RID: 5105
		// (get) Token: 0x0600AFD1 RID: 45009 RVA: 0x00500C62 File Offset: 0x004FEE62
		public CButton BtnMultiplySelect
		{
			get
			{
				return this.CurMultiplyScrollView.BtnMultiplySelect;
			}
		}

		// Token: 0x170013F2 RID: 5106
		// (get) Token: 0x0600AFD2 RID: 45010 RVA: 0x00500C6F File Offset: 0x004FEE6F
		public CToggle SwitchSelection
		{
			get
			{
				return this.CurMultiplyScrollView.SwitchSelection;
			}
		}

		// Token: 0x170013F3 RID: 5107
		// (get) Token: 0x0600AFD3 RID: 45011 RVA: 0x00500C7C File Offset: 0x004FEE7C
		public CToggle ToggleSelectAll
		{
			get
			{
				return this.CurMultiplyScrollView.ToggleSelectAll;
			}
		}

		// Token: 0x170013F4 RID: 5108
		// (get) Token: 0x0600AFD4 RID: 45012 RVA: 0x00500C89 File Offset: 0x004FEE89
		public CToggle ToggleMultiplyLock
		{
			get
			{
				return this.CurMultiplyScrollView.ToggleMultiplyLock;
			}
		}

		// Token: 0x170013F5 RID: 5109
		// (get) Token: 0x0600AFD5 RID: 45013 RVA: 0x00500C96 File Offset: 0x004FEE96
		public GameObject ObjMultiplyLockTip
		{
			get
			{
				return this.CurMultiplyScrollView.ObjMultiplyLockTip;
			}
		}

		// Token: 0x170013F6 RID: 5110
		// (get) Token: 0x0600AFD6 RID: 45014 RVA: 0x00500CA3 File Offset: 0x004FEEA3
		// (set) Token: 0x0600AFD7 RID: 45015 RVA: 0x00500CAB File Offset: 0x004FEEAB
		public bool IsMultiplyLock { get; private set; }

		// Token: 0x170013F7 RID: 5111
		// (get) Token: 0x0600AFD8 RID: 45016 RVA: 0x00500CB4 File Offset: 0x004FEEB4
		private List<ItemDisplayData> UsableToolOrderedList
		{
			get
			{
				return this.CurOperationHandler.UsableToolOrderedList;
			}
		}

		// Token: 0x170013F8 RID: 5112
		// (get) Token: 0x0600AFD9 RID: 45017 RVA: 0x00500CC1 File Offset: 0x004FEEC1
		public ItemListScroll CurMultiplyScrollView
		{
			get
			{
				return this.itemScroll;
			}
		}

		// Token: 0x170013F9 RID: 5113
		// (get) Token: 0x0600AFDA RID: 45018 RVA: 0x00500CC9 File Offset: 0x004FEEC9
		private List<ItemDisplayData> CurMultiplyItems
		{
			get
			{
				return this.Items;
			}
		}

		// Token: 0x170013FA RID: 5114
		// (get) Token: 0x0600AFDB RID: 45019 RVA: 0x00500CD1 File Offset: 0x004FEED1
		private ItemSourceType CurMultiplyItemSourceType
		{
			get
			{
				return this.ItemSourceType;
			}
		}

		// Token: 0x170013FB RID: 5115
		// (get) Token: 0x0600AFDC RID: 45020 RVA: 0x00500CD9 File Offset: 0x004FEED9
		// (set) Token: 0x0600AFDD RID: 45021 RVA: 0x00500CE1 File Offset: 0x004FEEE1
		public int MaxSelectCount { get; set; }

		// Token: 0x170013FC RID: 5116
		// (get) Token: 0x0600AFDE RID: 45022 RVA: 0x00500CEA File Offset: 0x004FEEEA
		// (set) Token: 0x0600AFDF RID: 45023 RVA: 0x00500CF2 File Offset: 0x004FEEF2
		public int TotalSelectedCount { get; private set; }

		// Token: 0x170013FD RID: 5117
		// (get) Token: 0x0600AFE0 RID: 45024 RVA: 0x00500CFB File Offset: 0x004FEEFB
		// (set) Token: 0x0600AFE1 RID: 45025 RVA: 0x00500D03 File Offset: 0x004FEF03
		public bool HasInit { get; private set; }

		// Token: 0x0600AFE2 RID: 45026 RVA: 0x00500D0C File Offset: 0x004FEF0C
		public void Init(int charId, ItemKey emptyToolKey, CharacterDisplayData taiwuCharacterDisplayData, Action onEnterMultiplyOperation = null, Action onExitMultiplyOperation = null, Action onEnterMultiplyLock = null, Action onExitMultiplyLock = null, [TupleElementNames(new string[]
		{
			"data",
			"count"
		})] Action<List<ValueTuple<ItemDisplayData, int>>> onContentChange = null)
		{
			bool hasInit = this.HasInit;
			if (!hasInit)
			{
				this.HasInit = true;
				this.CurCharId = charId;
				this._emptyToolKey = emptyToolKey;
				this._taiwuCharacterDisplayData = taiwuCharacterDisplayData;
				this._onEnterMultiplyOperation = onEnterMultiplyOperation;
				this._onExitMultiplyOperation = onExitMultiplyOperation;
				this._onEnterMultiplyLock = onEnterMultiplyLock;
				this._onExitMultiplyLock = onExitMultiplyLock;
				this._onContentChange = onContentChange;
				this.RequestData(null);
				this.itemMultiplyOperationPanel.Hide();
				bool flag = this.multiplyBg != null;
				if (flag)
				{
					this.multiplyBg.SetActive(false);
				}
				this.<Init>g__InitButton|91_0(this.itemScroll);
			}
		}

		// Token: 0x0600AFE3 RID: 45027 RVA: 0x00500DB0 File Offset: 0x004FEFB0
		private void RequestData(Action action = null)
		{
			this._resourceMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<ResourceMonitor>(MultiplyItemListScroll.TaiwuCharId, false);
			this._lifeSkillMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<LifeSkillMonitor>(MultiplyItemListScroll.TaiwuCharId, false);
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
			{
				Action action2 = action;
				if (action2 != null)
				{
					action2();
				}
			});
		}

		// Token: 0x0600AFE4 RID: 45028 RVA: 0x00500E10 File Offset: 0x004FF010
		public void Set(Dictionary<ItemSourceType, List<ItemDisplayData>> itemDict)
		{
			this._itemDict = itemDict;
			for (int index = 0; index < this.SelectedMultiplyItemOrderedList.Count; index++)
			{
				ItemDisplayData oldData = this.SelectedMultiplyItemOrderedList[index];
				ItemDisplayData newData = this.CurMultiplyItems.Find((ItemDisplayData d) => d.ContainsItemKey(oldData.RealKey));
				this.SelectedMultiplyItemOrderedList[index] = newData;
				int selectedCount = this.SelectedMultiplyItemDict[oldData];
				this.SelectedMultiplyItemDict.Remove(oldData);
				this.SelectedMultiplyItemDict[newData] = selectedCount;
			}
		}

		// Token: 0x0600AFE5 RID: 45029 RVA: 0x00500EB8 File Offset: 0x004FF0B8
		public bool IsSelected(ItemDisplayData itemData)
		{
			int selectedCount;
			this.SelectedMultiplyItemDict.TryGetValue(itemData, out selectedCount);
			return selectedCount > 0;
		}

		// Token: 0x0600AFE6 RID: 45030 RVA: 0x00500EE0 File Offset: 0x004FF0E0
		private void SelectAll(bool isSelect)
		{
			if (isSelect)
			{
				this.SelectedMultiplyItemDict.Clear();
				this.SelectedMultiplyItemOrderedList.Clear();
				IReadOnlyList<ITradeableContent> canOperateItemList = this.CurMultiplyScrollView.FilteredData;
				using (IEnumerator<ITradeableContent> enumerator = canOperateItemList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ITradeableContent tempData = enumerator.Current;
						ItemDisplayData data = this.CurMultiplyItems.Find((ItemDisplayData d) => d.RealKey.Equals(tempData.RealKey));
						bool usingTypeNeedConfirm = data.GetUsingTypeNeedConfirm();
						bool flag = usingTypeNeedConfirm;
						if (!flag)
						{
							List<ItemDisplayData> toolList;
							bool interactable = this.CheckInteractable(data, out toolList);
							bool flag2 = interactable;
							if (flag2)
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

		// Token: 0x0600AFE7 RID: 45031 RVA: 0x00501068 File Offset: 0x004FF268
		public void OnRenderItemMultiply(ITradeableContent tempData, RowItemLine view)
		{
			MultiplyItemListScroll.<>c__DisplayClass96_0 CS$<>8__locals1 = new MultiplyItemListScroll.<>c__DisplayClass96_0();
			CS$<>8__locals1.tempData = tempData;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.view = view;
			CS$<>8__locals1.itemData = this.CurMultiplyItems.Find((ItemDisplayData d) => d.RealKey.Equals(CS$<>8__locals1.tempData.RealKey));
			CS$<>8__locals1.view.RowItemMain.SetFavoriteStatus(RowItemMain.FavoriteStatus.None);
			this.SelectedMultiplyItemDict.TryGetValue(CS$<>8__locals1.itemData, out CS$<>8__locals1.selectedCount);
			CS$<>8__locals1.isSelected = (CS$<>8__locals1.selectedCount > 0);
			bool isInSelectedList = this._selectedItems.Contains(CS$<>8__locals1.tempData);
			CS$<>8__locals1.view.SetSelected(CS$<>8__locals1.isSelected && !isInSelectedList);
			CS$<>8__locals1.curMultiplyScrollView = (((this.SwitchSelection.isOn & CS$<>8__locals1.isSelected) && isInSelectedList) ? this.selectedScroll : this.CurMultiplyScrollView);
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
						if (eitemOperationType != ItemOperationType.EItemOperationType.Feeding)
						{
							CS$<>8__locals1.view.RowItemMain.HideInteractionState();
						}
						else
						{
							bool isFeedingCricket = this._isFeedingCricket;
							if (isFeedingCricket)
							{
								CS$<>8__locals1.view.RowItemMain.SetFavoriteStatus(RowItemMain.FavoriteStatus.None);
							}
							else
							{
								CS$<>8__locals1.<OnRenderItemMultiply>g__RefreshFavoriteStatus|3();
							}
						}
					}
					else
					{
						short required = ItemTemplateHelper.GetDisassembleRequiredAttainment(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
						sbyte itemLifeSkillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
						CS$<>8__locals1.view.RowItemMain.ShowInteractionStateAttainment(itemLifeSkillType, required, true);
					}
				}
				else
				{
					short required2 = ItemTemplateHelper.GetRepairRequiredAttainment(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId, CS$<>8__locals1.itemData.Durability);
					sbyte itemLifeSkillType2 = ItemTemplateHelper.GetCraftRequiredLifeSkillType(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
					CS$<>8__locals1.view.RowItemMain.ShowInteractionStateAttainment(itemLifeSkillType2, required2, true);
				}
				CS$<>8__locals1.view.SetInteractable(true, true);
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
					bool hasAnySkillMatchedTool;
					bool hasAvailableTool;
					bool interactable = this.CheckInteractableWithTool(CS$<>8__locals1.itemData, out itemLifeSkillType3, out required3, out hasAnySkillMatchedTool, out hasAvailableTool, ref availableToolList);
					bool flag = interactable;
					if (flag)
					{
						CS$<>8__locals1.view.RowItemMain.HideInteractionState();
					}
					else
					{
						bool flag2 = hasAvailableTool || hasAnySkillMatchedTool;
						if (flag2)
						{
							CS$<>8__locals1.view.RowItemMain.ShowInteractionStateAttainment(itemLifeSkillType3, required3, false);
						}
						else
						{
							string content = LocalStringManager.Get(LanguageKey.LK_Tool_Not_Enough).SetColor("brightred");
							CS$<>8__locals1.view.RowItemMain.SetItemNotCanSelectReason(content);
						}
					}
					CS$<>8__locals1.view.SetInteractable(interactable, true);
					break;
				}
				case ItemOperationType.EItemOperationType.Take:
				{
					bool isTaiwuGearMate = this.IsTaiwuGearMate(this.CurCharId);
					int num;
					bool canTakeItem = ViewCharacterMenuItems.CanTakeItem(CS$<>8__locals1.itemData, this.MaxWorthCanBeLentToTaiwu, isTaiwuGearMate, out num);
					CS$<>8__locals1.view.SetInteractable(canTakeItem, true);
					break;
				}
				case ItemOperationType.EItemOperationType.Discard:
					CS$<>8__locals1.view.RowItemMain.HideInteractionState();
					CS$<>8__locals1.view.SetInteractable(true, true);
					break;
				case ItemOperationType.EItemOperationType.SpecialBreakConvertToExp:
					CS$<>8__locals1.view.SetInteractable(true, true);
					break;
				case ItemOperationType.EItemOperationType.Feeding:
				{
					bool isFeedingCricket2 = this._isFeedingCricket;
					if (isFeedingCricket2)
					{
						bool isFeedSpiritMax = this._isFeedSpiritMax;
						if (isFeedSpiritMax)
						{
							CS$<>8__locals1.view.SetInteractable(false, true);
							CS$<>8__locals1.view.RowItemMain.SetItemNotCanSelectReason(LanguageKey.LK_Cricket_Feeding_Tip_NoNeed.Tr().SetColor("brightred"));
						}
						else
						{
							CS$<>8__locals1.view.SetInteractable(true, true);
						}
						CS$<>8__locals1.view.RowItemMain.SetFavoriteStatus(RowItemMain.FavoriteStatus.None);
					}
					else
					{
						bool flag3 = this._isFeedDurabilityMax && this._isFeedTameMax;
						if (flag3)
						{
							CS$<>8__locals1.view.SetInteractable(false, true);
							CS$<>8__locals1.view.RowItemMain.SetItemNotCanSelectReason(LocalStringManager.Get(LanguageKey.LK_ItemFeedCarrier_Tip_Full).SetColor("brightred"));
						}
						else
						{
							CS$<>8__locals1.view.SetInteractable(true, true);
						}
						CS$<>8__locals1.<OnRenderItemMultiply>g__RefreshFavoriteStatus|3();
					}
					break;
				}
				}
				bool flag4 = this.MaxSelectCount > 0 && this.TotalSelectedCount >= this.MaxSelectCount;
				if (flag4)
				{
					CS$<>8__locals1.view.SetInteractable(false, true);
					CS$<>8__locals1.view.RowItemMain.SetItemNotCanSelectReason(LocalStringManager.Get(LanguageKey.LK_ItemSelectCountIsMax).SetColor("brightred"));
				}
				bool isConsume = ItemOperationType.IsConsume(this._currItemOperation);
				bool interactable2 = !isConsume || !CS$<>8__locals1.itemData.IsLocked;
				bool flag5 = !interactable2;
				if (flag5)
				{
					CS$<>8__locals1.view.SetInteractable(false, true);
					CS$<>8__locals1.view.RowItemMain.HideInteractionState();
				}
			}
			CS$<>8__locals1.toolList = (CS$<>8__locals1.isSelected ? usedToolList : availableToolList);
			CS$<>8__locals1.view.SetDisabled(!CS$<>8__locals1.view.Interactable);
			bool flag6 = !CS$<>8__locals1.view.Interactable;
			if (!flag6)
			{
				CS$<>8__locals1.view.SetClickEvent(delegate
				{
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
						bool flag7 = limitCount <= 0;
						if (!flag7)
						{
							CS$<>8__locals1.curMultiplyScrollView.HandleClickItem(CS$<>8__locals1.itemData, CS$<>8__locals1.view, delegate(RowItemLine v)
							{
								CS$<>8__locals1.<OnRenderItemMultiply>g__Click|2(v, limitCount, limitTip, CS$<>8__locals1.toolList);
							});
						}
					}
				});
			}
		}

		// Token: 0x0600AFE8 RID: 45032 RVA: 0x005015DC File Offset: 0x004FF7DC
		private bool CheckInteractableWithTool(ItemDisplayData itemData, out sbyte itemLifeSkillType, out short required, out bool hasAnySkillMatchedTool, out bool hasAvailableTool, ref List<ItemDisplayData> availableToolList)
		{
			bool useEmptyTool = this.CheckUseEmptyTool(itemData, out itemLifeSkillType, out required);
			hasAvailableTool = false;
			hasAnySkillMatchedTool = false;
			bool flag = !useEmptyTool;
			if (flag)
			{
				availableToolList = this.GetAvailableToolList(itemData, out hasAnySkillMatchedTool);
				List<ItemDisplayData> list = availableToolList;
				hasAvailableTool = (list != null && list.Count > 0);
			}
			bool useNormalTool = hasAvailableTool;
			return useEmptyTool || useNormalTool;
		}

		// Token: 0x0600AFE9 RID: 45033 RVA: 0x0050163C File Offset: 0x004FF83C
		private bool CheckUseEmptyTool(ItemDisplayData itemData, out sbyte itemLifeSkillType, out short required)
		{
			itemLifeSkillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(itemData.Key.ItemType, itemData.Key.TemplateId);
			required = ((this._currItemOperation == ItemOperationType.EItemOperationType.Repair) ? ItemTemplateHelper.GetRepairRequiredAttainment(itemData.Key.ItemType, itemData.Key.TemplateId, itemData.Durability) : ItemTemplateHelper.GetDisassembleRequiredAttainment(itemData.Key.ItemType, itemData.Key.TemplateId));
			short attainment = this._lifeSkillMonitor.Attainments.GetOrDefault((int)itemLifeSkillType);
			short emptyToolAttainment = ViewMake.GetFinalAttainment(-1, attainment, itemLifeSkillType);
			return emptyToolAttainment >= required;
		}

		// Token: 0x0600AFEA RID: 45034 RVA: 0x005016DC File Offset: 0x004FF8DC
		private bool CheckInteractable(ITradeableContent content, out List<ItemDisplayData> toolList)
		{
			ItemDisplayData itemData = (ItemDisplayData)content;
			toolList = null;
			bool isLocked = itemData.IsLocked;
			bool result;
			if (isLocked)
			{
				result = false;
			}
			else
			{
				int selectedCount;
				this.SelectedMultiplyItemDict.TryGetValue(itemData, out selectedCount);
				bool isSelected = selectedCount > 0;
				bool flag = isSelected;
				if (flag)
				{
					bool isResource = itemData.IsResource;
					if (isResource)
					{
						toolList = null;
					}
					else
					{
						Dictionary<ItemDisplayData, short> usedToolDict = this.CurOperationHandler.GetUsedToolDict(itemData);
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
							bool flag4;
							bool interactableWithTool = this.CheckInteractableWithTool(itemData, out b, out num, out flag3, out flag4, ref availableToolList);
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
						{
							bool isFeedingCricket = this._isFeedingCricket;
							if (isFeedingCricket)
							{
								return !this._isFeedSpiritMax;
							}
							return !this._isFeedDurabilityMax || !this._isFeedTameMax;
						}
						}
					}
					result = !itemData.IsLocked;
				}
			}
			return result;
		}

		// Token: 0x0600AFEB RID: 45035 RVA: 0x00501844 File Offset: 0x004FFA44
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
				bool useEmptyTool = this.CheckUseEmptyTool(itemData, out b, out num);
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
								short durability = this.CurOperationHandler.GetToolRemainDurability(d);
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
				bool isFeedingCricket = this._isFeedingCricket;
				if (isFeedingCricket)
				{
					int leftSpirit;
					int num2;
					this.RefreshFeedState(out leftSpirit, out num2);
					sbyte grade2 = Misc.Instance[itemData.Key.TemplateId].Grade;
					int addSpiritPer = GlobalConfig.Instance.CricketBloodDewAddSpirit[(int)grade2];
					limitCount = ((addSpiritPer <= 0) ? itemData.Amount : Mathf.CeilToInt((float)leftSpirit / (float)addSpiritPer));
					limitCount = Mathf.Min(limitCount, itemData.Amount);
					limitTip = LanguageKey.LK_Cricket_Feeding_Tip_NoNeed.Tr();
				}
				else
				{
					bool flag5 = this._feedingTarget != null;
					if (flag5)
					{
						int leftDurability;
						int leftTame;
						this.RefreshFeedState(out leftDurability, out leftTame);
						int addDurability = GameData.Domains.Extra.SharedMethods.GetFoodAddCarrierDurability(this._feedingTarget.Key.TemplateId, itemData.Key.TemplateId, 1);
						int durabilityCount = (addDurability <= 0) ? itemData.Amount : Mathf.CeilToInt((float)leftDurability / (float)addDurability);
						int tameCount = 0;
						bool flag6 = ItemTemplateHelper.HasCarrierTame(this._feedingTarget.Key.ItemType, this._feedingTarget.Key.TemplateId);
						if (flag6)
						{
							int addTame = GameData.Domains.Extra.SharedMethods.GetFoodAddCarrierTamePoint(this._feedingTarget.Key.TemplateId, itemData.Key.TemplateId, 1);
							bool flag7 = addTame > 0;
							if (flag7)
							{
								tameCount = Mathf.CeilToInt((float)leftTame / (float)addTame);
							}
						}
						limitCount = Mathf.Max(durabilityCount, tameCount);
						limitTip = LocalStringManager.Get(LanguageKey.LK_SelectCount_Limit_Feeding);
					}
				}
				break;
			}
			case ItemOperationType.EItemOperationType.Confiscate:
			{
				bool flag8 = UIManager.Instance.IsFocusElement(UIElement.EventWindow);
				if (flag8)
				{
					bool isMiscResource = ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
					bool flag9 = !isMiscResource;
					if (flag9)
					{
						limitCount = this.MaxSelectCount - this.TotalSelectedCount;
						limitTip = LocalStringManager.Get(LanguageKey.LK_SelectCount_Limit_Confiscate);
					}
				}
				break;
			}
			}
		}

		// Token: 0x0600AFEC RID: 45036 RVA: 0x00501B80 File Offset: 0x004FFD80
		public void OnRenderItemExchangeTools(ItemDisplayData itemData, RowItemLine view)
		{
			MultiplyItemListScroll.<>c__DisplayClass101_0 CS$<>8__locals1 = new MultiplyItemListScroll.<>c__DisplayClass101_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.itemData = itemData;
			CS$<>8__locals1.view = view;
			this.SelectedMultiplyItemDict.TryGetValue(CS$<>8__locals1.itemData, out CS$<>8__locals1.selectedCount);
			CS$<>8__locals1.isSelected = (CS$<>8__locals1.selectedCount > 0);
			CS$<>8__locals1.view.SetSelected(CS$<>8__locals1.isSelected);
			bool isSelected = CS$<>8__locals1.isSelected;
			if (isSelected)
			{
				CS$<>8__locals1.view.RowItemMain.HideInteractionState();
				bool flag = this.TotalSelectedCount <= 0;
				if (flag)
				{
					CS$<>8__locals1.view.SetInteractable(true, true);
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
						CS$<>8__locals1.view.SetInteractable(false, true);
						CS$<>8__locals1.view.RowItemMain.SetInteractionStateLockText(LocalStringManager.Get(LanguageKey.LK_Item_Operation_Locked));
					}
				}
				bool flag4 = this.TotalSelectedCount >= this.MaxSelectCount && this.MaxSelectCount > 0;
				if (flag4)
				{
					CS$<>8__locals1.view.SetInteractable(false, true);
				}
			}
			CS$<>8__locals1.view.SetClickEvent(delegate
			{
				MultiplyItemListScroll.<>c__DisplayClass101_1 CS$<>8__locals2 = new MultiplyItemListScroll.<>c__DisplayClass101_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
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
						CS$<>8__locals1.<>4__this.CurMultiplyScrollView.HandleClickItem(CS$<>8__locals1.itemData, CS$<>8__locals1.view, new Action<RowItemLine>(CS$<>8__locals2.<OnRenderItemExchangeTools>g__Action|1));
					}
				}
			});
		}

		// Token: 0x0600AFED RID: 45037 RVA: 0x00501CCC File Offset: 0x004FFECC
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

		// Token: 0x0600AFEE RID: 45038 RVA: 0x00501D34 File Offset: 0x004FFF34
		public int GetAlreadySelectItemCount()
		{
			return this.SelectedMultiplyItemDict.Keys.Count;
		}

		// Token: 0x0600AFEF RID: 45039 RVA: 0x00501D58 File Offset: 0x004FFF58
		public int GetSelectedOrder(ItemDisplayData itemData)
		{
			return this.SelectedMultiplyItemOrderedList.IndexOf(itemData);
		}

		// Token: 0x0600AFF0 RID: 45040 RVA: 0x00501D76 File Offset: 0x004FFF76
		public void SetFilter(Func<ItemDisplayData, bool> filter)
		{
			this._filter = filter;
		}

		// Token: 0x0600AFF1 RID: 45041 RVA: 0x00501D80 File Offset: 0x004FFF80
		private bool SetItemSelectCount(RowItemLine itemView, int count, List<ItemDisplayData> toolList)
		{
			return this.SetItemSelectCount(itemView.RowItemMain.Data.RealKey, count, toolList, true);
		}

		// Token: 0x0600AFF2 RID: 45042 RVA: 0x00501DB0 File Offset: 0x004FFFB0
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
				Dictionary<ItemDisplayData, short> usedToolDict = this.CurOperationHandler.GetUsedToolDict(data);
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
						short remainDurability = this.CurOperationHandler.GetToolRemainDurability(tool);
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
				this.CurOperationHandler.RefreshAllUsedToolDict();
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

		// Token: 0x0600AFF3 RID: 45043 RVA: 0x00502038 File Offset: 0x00500238
		public void SelectItem(List<ValueTuple<ItemDisplayData, int>> selectionList)
		{
			this.ClearMultiplySelection(false);
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

		// Token: 0x0600AFF4 RID: 45044 RVA: 0x0050210C File Offset: 0x0050030C
		private void RefreshSelectedCount()
		{
			this.TotalSelectedCount = this.SelectedMultiplyItemDict.Sum(delegate(KeyValuePair<ItemDisplayData, int> p)
			{
				ItemDisplayData data = p.Key;
				return ItemTemplateHelper.IsMiscResource(data.Key.ItemType, data.Key.TemplateId) ? 1 : p.Value;
			});
			this.itemMultiplyOperationPanel.RefreshSelectedItem(this.TotalSelectedCount);
		}

		// Token: 0x0600AFF5 RID: 45045 RVA: 0x00502160 File Offset: 0x00500360
		private void SendItemMultiplyOperationContentChange([TupleElementNames(new string[]
		{
			"itemKey",
			"count"
		})] List<ValueTuple<ItemDisplayData, int>> changeList = null)
		{
			bool flag = this.CurOperationHandler == null;
			if (!flag)
			{
				this.RequestData(delegate
				{
					ArgumentBox args = this.GetItemMultiplyViewArgs();
					GEvent.OnEvent(UiEvents.ItemMultiplyOperationContentChange, args);
					Action<List<ValueTuple<ItemDisplayData, int>>> onContentChange = this._onContentChange;
					if (onContentChange != null)
					{
						onContentChange(changeList);
					}
				});
			}
		}

		// Token: 0x0600AFF6 RID: 45046 RVA: 0x005021A4 File Offset: 0x005003A4
		private void RefreshFeedState(out int leftDurability, out int leftTame)
		{
			leftDurability = 0;
			leftTame = 0;
			this._isFeedSpiritMax = false;
			bool flag = this._multiplyItemOperationType != ItemOperationType.EItemOperationType.Feeding || this._feedingTarget == null;
			if (!flag)
			{
				bool isFeedingCricket = this._isFeedingCricket;
				if (isFeedingCricket)
				{
					int spiritMax = GlobalConfig.Instance.CricketSpiritMax;
					int addSpirit = this.SelectedMultiplyItemDict.Sum(delegate(KeyValuePair<ItemDisplayData, int> pair)
					{
						sbyte grade = Misc.Instance[pair.Key.Key.TemplateId].Grade;
						return GlobalConfig.Instance.CricketBloodDewAddSpirit[(int)grade] * pair.Value;
					});
					int curSpirit = Mathf.Clamp(addSpirit + this._feedingTarget.CricketData.Spirit, 0, spiritMax);
					this._isFeedSpiritMax = (curSpirit >= spiritMax);
					leftDurability = spiritMax - curSpirit;
					this._isFeedDurabilityMax = true;
					this._isFeedTameMax = true;
				}
				else
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
		}

		// Token: 0x0600AFF7 RID: 45047 RVA: 0x0050235C File Offset: 0x0050055C
		public void RefreshMultiplyAvailableTool()
		{
			List<ItemDisplayData> itemList = EasyPool.Get<List<ItemDisplayData>>();
			foreach (KeyValuePair<ItemSourceType, List<ItemDisplayData>> keyValuePair in this._itemDict)
			{
				ItemSourceType itemSourceType;
				List<ItemDisplayData> list;
				keyValuePair.Deconstruct(out itemSourceType, out list);
				List<ItemDisplayData> value = list;
				bool flag = value != null && value.Count > 0;
				if (flag)
				{
					itemList.AddRange(value);
				}
			}
			IOrderedEnumerable<ItemDisplayData> toolList = from d in itemList
			where d.Key.ItemType == 6 && !d.IsLocked && d.Durability > 0 && !this.SelectedMultiplyItemDict.ContainsKey(d)
			orderby ItemTemplateHelper.GetGrade(d.Key.ItemType, d.Key.TemplateId)
			select d;
			this.UsableToolOrderedList.Clear();
			this.UsableToolOrderedList.AddRange(toolList);
			EasyPool.Free<List<ItemDisplayData>>(itemList);
		}

		// Token: 0x0600AFF8 RID: 45048 RVA: 0x00502438 File Offset: 0x00500638
		private void OnClickMultiplyFeed()
		{
			bool isFeedingCricket = this._isFeedingCricket;
			if (isFeedingCricket)
			{
				this.OnClickMultiplyFeedCricket();
			}
			else
			{
				List<ItemDisplayData> costItemList = (from d in this.SelectedMultiplyItemOrderedList
				orderby ItemTemplateHelper.GetGrade(d.RealKey.ItemType, d.RealKey.TemplateId)
				select d).ToList<ItemDisplayData>();
				int num;
				foreach (ItemDisplayData itemData in costItemList)
				{
					int count;
					this.SelectedMultiplyItemDict.TryGetValue(itemData, out count);
					Inventory inventory = itemData.GetOperationInventoryFromPool(count, false);
					foreach (KeyValuePair<ItemKey, int> keyValuePair in inventory.Items)
					{
						ItemKey itemKey2;
						keyValuePair.Deconstruct(out itemKey2, out num);
						ItemKey itemKey = itemKey2;
						int amount = num;
						bool flag = this._feedingTarget.Durability == this._feedingTarget.MaxDurability && this._feedingTarget.CarrierTamePoint == GlobalConfig.Instance.MaxCarrierTamePoint;
						if (flag)
						{
							break;
						}
						ExtraDomainMethod.Call.FeedCarrier(this._feedingTarget.Key, itemKey, amount, itemData.ItemSourceTypeEnum);
						itemData.ChangeAmount(itemKey, false, amount);
						bool flag2 = itemData.Amount <= 0;
						if (flag2)
						{
							this.CurMultiplyItems.Remove(itemData);
							this.SelectedMultiplyItemDict.Remove(itemData);
							this.SelectedMultiplyItemOrderedList.Remove(itemData);
						}
						else
						{
							bool flag3 = count > itemData.Amount;
							if (flag3)
							{
								this.SelectedMultiplyItemDict[itemData] = itemData.Amount;
							}
						}
						int addDurability = GameData.Domains.Extra.SharedMethods.GetFoodAddCarrierDurability(this._feedingTarget.Key.TemplateId, itemKey.TemplateId, amount);
						this._feedingTarget.Durability = (short)Mathf.Clamp((int)this._feedingTarget.Durability + addDurability, 0, (int)this._feedingTarget.MaxDurability);
						int addTame = GameData.Domains.Extra.SharedMethods.GetFoodAddCarrierTamePoint(this._feedingTarget.Key.TemplateId, itemKey.TemplateId, amount);
						this._feedingTarget.CarrierTamePoint = Mathf.Clamp(this._feedingTarget.CarrierTamePoint + addTame, 0, GlobalConfig.Instance.MaxCarrierTamePoint);
					}
					ItemDisplayData.ReturnInventoryToPool(inventory);
				}
				this.ClearMultiplySelection(false);
				int num2;
				this.RefreshFeedState(out num, out num2);
				this.RefreshSelectedCount();
				this.RefreshMultiplyCanOperateItems();
				this.SendItemMultiplyOperationContentChange(null);
				GEvent.OnEvent(UiEvents.ItemMultiplyOperationFinish, null);
			}
		}

		// Token: 0x0600AFF9 RID: 45049 RVA: 0x005026F8 File Offset: 0x005008F8
		private void OnClickMultiplyFeedCricket()
		{
			List<ItemKeyAndCount> keys = new List<ItemKeyAndCount>();
			foreach (ItemDisplayData itemData in this.SelectedMultiplyItemOrderedList)
			{
				int count;
				this.SelectedMultiplyItemDict.TryGetValue(itemData, out count);
				keys.Add(new ItemKeyAndCount(itemData.RealKey, count));
			}
			ItemDisplayData itemDisplayData = this.SelectedMultiplyItemOrderedList.FirstOrDefault<ItemDisplayData>();
			ItemSourceType sourceType = (itemDisplayData != null) ? itemDisplayData.ItemSourceTypeEnum : ItemSourceType.Inventory;
			TaiwuDomainMethod.Call.FeedingCricket(-1, this._feedingTarget.Key, keys, sourceType);
			int num;
			foreach (ItemDisplayData itemData2 in this.SelectedMultiplyItemOrderedList.ToList<ItemDisplayData>())
			{
				int count2;
				this.SelectedMultiplyItemDict.TryGetValue(itemData2, out count2);
				Inventory inventory = itemData2.GetOperationInventoryFromPool(count2, false);
				foreach (KeyValuePair<ItemKey, int> keyValuePair in inventory.Items)
				{
					ItemKey itemKey2;
					keyValuePair.Deconstruct(out itemKey2, out num);
					ItemKey itemKey = itemKey2;
					int amount = num;
					itemData2.ChangeAmount(itemKey, false, amount);
				}
				bool flag = itemData2.Amount <= 0;
				if (flag)
				{
					this.CurMultiplyItems.Remove(itemData2);
					this.SelectedMultiplyItemDict.Remove(itemData2);
				}
				else
				{
					bool flag2 = count2 > itemData2.Amount;
					if (flag2)
					{
						this.SelectedMultiplyItemDict[itemData2] = itemData2.Amount;
					}
				}
				ItemDisplayData.ReturnInventoryToPool(inventory);
			}
			this.SelectedMultiplyItemOrderedList.Clear();
			int addSpirit = keys.Sum(delegate(ItemKeyAndCount k)
			{
				sbyte grade = Misc.Instance[k.ItemKey.TemplateId].Grade;
				return GlobalConfig.Instance.CricketBloodDewAddSpirit[(int)grade] * k.Count;
			});
			int spiritMax = GlobalConfig.Instance.CricketSpiritMax;
			this._feedingTarget.CricketData.Spirit = Mathf.Min(this._feedingTarget.CricketData.Spirit + addSpirit, spiritMax);
			this.ClearMultiplySelection(false);
			int num2;
			this.RefreshFeedState(out num, out num2);
			this.RefreshSelectedCount();
			this.RefreshMultiplyCanOperateItems();
			this.SendItemMultiplyOperationContentChange(null);
			GEvent.OnEvent(UiEvents.ItemMultiplyOperationFinish, null);
		}

		// Token: 0x0600AFFA RID: 45050 RVA: 0x0050296C File Offset: 0x00500B6C
		private void OnClickMultiplyDiscard()
		{
			this.MultiplyDiscard();
		}

		// Token: 0x0600AFFB RID: 45051 RVA: 0x00502978 File Offset: 0x00500B78
		private void MultiplyDiscard()
		{
			Inventory inventory = ItemDisplayData.GetInventoryFromPool();
			List<MultiplyOperation> operationList = this.CurOperationHandler.GetOperationList(true);
			using (List<MultiplyOperation>.Enumerator enumerator = operationList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MultiplyOperation operation = enumerator.Current;
					ItemDisplayData data = this.SelectedMultiplyItemOrderedList.Find((ItemDisplayData d) => d.ContainsItemKey(operation.Target));
					this.ClearItemUsingState(data);
					bool flag = ItemTemplateHelper.IsMiscResource(data.Key.ItemType, data.Key.TemplateId);
					if (flag)
					{
						ItemDomainMethod.Call.DiscardItem(MultiplyItemListScroll.TaiwuCharId, data.Key, data.ItemSourceType, operation.Count);
						data.Amount -= operation.Count;
					}
					else
					{
						inventory.OfflineAdd(operation.Target, operation.Count);
						data.ChangeAmount(operation.Target, false, 1);
					}
					bool flag2 = data.Amount <= 0;
					if (flag2)
					{
						this.CurMultiplyItems.Remove(data);
					}
				}
			}
			bool flag3 = inventory.InventoryItemTotalCount > 0;
			if (flag3)
			{
				ItemDomainMethod.Call.DiscardItemInventory(MultiplyItemListScroll.TaiwuCharId, inventory, (sbyte)this.CurMultiplyItemSourceType);
			}
			ItemDisplayData.ReturnInventoryToPool(inventory);
			this.ClearMultiplySelection(false);
			this.RefreshItems();
			GEvent.OnEvent(UiEvents.ItemMultiplyOperationFinish, null);
		}

		// Token: 0x0600AFFC RID: 45052 RVA: 0x00502B18 File Offset: 0x00500D18
		private void OnClickMultiplyDisassemble()
		{
			this.MultiplyDisassemble();
		}

		// Token: 0x0600AFFD RID: 45053 RVA: 0x00502B24 File Offset: 0x00500D24
		private void MultiplyDisassemble()
		{
			List<MultiplyOperation> operationList = this.CurOperationHandler.GetOperationList(false);
			this.ClearMultiplyItemUsingState(operationList);
			this.RefreshUsedTool();
			ItemDomainMethod.AsyncCall.DisassembleItemList(null, MultiplyItemListScroll.TaiwuCharId, operationList, delegate(int offset, RawDataPool dataPool)
			{
				List<ItemDisplayData> resultList = EasyPool.Get<List<ItemDisplayData>>();
				resultList.Clear();
				Serializer.Deserialize(dataPool, offset, ref resultList);
				Dictionary<ItemDisplayData, short> allUsedToolDict = this.CurOperationHandler.AllUsedToolDict;
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
					argBox.Set("IntoWarehouse", false);
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
				this.ClearMultiplySelection(false);
				this.RefreshItems();
				GEvent.OnEvent(UiEvents.ItemMultiplyOperationFinish, null);
			});
		}

		// Token: 0x0600AFFE RID: 45054 RVA: 0x00502B67 File Offset: 0x00500D67
		private void OnClickMultiplyRepair()
		{
			this.MultiplyRepair();
		}

		// Token: 0x0600AFFF RID: 45055 RVA: 0x00502B74 File Offset: 0x00500D74
		private void MultiplyRepair()
		{
			List<MultiplyOperation> operationList = this.CurOperationHandler.GetOperationList(false);
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
			BuildingDomainMethod.AsyncCall.RepairItemList(null, MultiplyItemListScroll.TaiwuCharId, operationList, delegate(int offset, RawDataPool dataPool)
			{
				List<ItemDisplayData> dataList = EasyPool.Get<List<ItemDisplayData>>();
				dataList.Clear();
				Serializer.Deserialize(dataPool, offset, ref dataList);
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Clear();
				argBox.SetObject("ItemList", dataList);
				argBox.Set("ObtainType", 7);
				Dictionary<ItemDisplayData, short> allUsedToolDict = this.CurOperationHandler.AllUsedToolDict;
				List<ItemDisplayData> toolList = allUsedToolDict.Keys.ToList<ItemDisplayData>();
				foreach (KeyValuePair<ItemDisplayData, short> pair in allUsedToolDict)
				{
					ItemDisplayData key = pair.Key;
					key.Durability -= pair.Value;
				}
				toolList.RemoveAll((ItemDisplayData data) => data.Durability > 0);
				argBox.SetObject("UsedToolList", toolList);
				argBox.Set("IsNew", false);
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
				this.ClearMultiplySelection(false);
				this.RefreshItems();
				GEvent.OnEvent(UiEvents.ItemMultiplyOperationFinish, null);
			});
		}

		// Token: 0x0600B000 RID: 45056 RVA: 0x00502C18 File Offset: 0x00500E18
		private void OnClickMultiplyPutPoisonMaterial()
		{
			Inventory inventory = ItemDisplayData.GetInventoryFromPool();
			List<MultiplyOperation> operationList = this.CurOperationHandler.GetOperationList(true);
			using (List<MultiplyOperation>.Enumerator enumerator = operationList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MultiplyOperation operation = enumerator.Current;
					ItemDisplayData data = this.SelectedMultiplyItemOrderedList.Find((ItemDisplayData d) => d.ContainsItemKey(operation.Target));
					this.ClearItemUsingState(data);
					inventory.OfflineAdd(operation.Target, operation.Count);
					data.ChangeAmount(operation.Target, false, 1);
					bool flag = data.Amount <= 0;
					if (flag)
					{
						this.CurMultiplyItems.Remove(data);
					}
				}
			}
			StoryDomainMethod.Call.DropPoisonsToWugJug(-1, inventory);
			ItemDisplayData.ReturnInventoryToPool(inventory);
			this.ClearMultiplySelection(false);
			this.RefreshItems();
			GEvent.OnEvent(UiEvents.ItemMultiplyOperationFinish, null);
		}

		// Token: 0x0600B001 RID: 45057 RVA: 0x00502D2C File Offset: 0x00500F2C
		private void OnClickMultiplyVillagerCraft()
		{
			ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("SelectedMultiplyItemDict", this.SelectedMultiplyItemDict);
			GEvent.OnEvent(UiEvents.OnConfirmVillagerCraftInputMaterial, args);
		}

		// Token: 0x0600B002 RID: 45058 RVA: 0x00502D64 File Offset: 0x00500F64
		private void OnClickMultiplyTransfer()
		{
			List<MultiplyOperation> operationList = this.CurOperationHandler.GetOperationList(true);
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, this.CurCharId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._beforeTransferCharacterDisplayData);
			});
			List<MultiplyOperation> resourceItemOperationList = (from o in operationList
			where ItemTemplateHelper.IsMiscResource(o.Target.ItemType, o.Target.TemplateId)
			select o).ToList<MultiplyOperation>();
			ResourceInts resourceInts = default(ResourceInts);
			using (List<MultiplyOperation>.Enumerator enumerator = resourceItemOperationList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MultiplyOperation operation = enumerator.Current;
					operationList.Remove(operation);
					ItemDisplayData data = this.SelectedMultiplyItemOrderedList.Find((ItemDisplayData d) => d.ContainsItemKey(operation.Target));
					data.Amount -= operation.Count;
					bool flag = data.Amount <= 0;
					if (flag)
					{
						this.CurMultiplyItems.Remove(data);
					}
					sbyte resourceType = ItemTemplateHelper.GetMiscResourceType(operation.Target.ItemType, operation.Target.TemplateId);
					resourceInts.Add(resourceType, operation.Count);
				}
			}
			bool flag2 = resourceInts.IsNonZero();
			if (flag2)
			{
				CharacterDomainMethod.Call.TransferResourcesWithDebt(MultiplyItemListScroll.TaiwuCharId, this.CurCharId, resourceInts, true);
			}
			Inventory inventory = ItemDisplayData.GetInventoryFromPool();
			using (List<MultiplyOperation>.Enumerator enumerator2 = operationList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					MultiplyOperation operation = enumerator2.Current;
					ItemDisplayData data2 = this.SelectedMultiplyItemOrderedList.Find((ItemDisplayData d) => d.ContainsItemKey(operation.Target));
					this.ClearItemUsingState(data2);
					inventory.OfflineAdd(operation.Target, operation.Count);
					data2.ChangeAmount(operation.Target, false, 1);
					bool flag3 = data2.Amount <= 0;
					if (flag3)
					{
						this.CurMultiplyItems.Remove(data2);
					}
				}
			}
			bool flag4 = inventory.Items.Count > 0;
			if (flag4)
			{
				CharacterDomainMethod.Call.TransferInventoryItemInventoryWithDebt(MultiplyItemListScroll.TaiwuCharId, this.CurCharId, inventory, true);
				ItemDisplayData.ReturnInventoryToPool(inventory);
			}
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(null, this.CurCharId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._afterTransferCharacterDisplayData);
				this.DispatchTransferResult(this.CurCharId);
			});
			this.ClearMultiplySelection(false);
			this.RefreshItems();
			GEvent.OnEvent(UiEvents.ItemMultiplyOperationFinish, null);
		}

		// Token: 0x0600B003 RID: 45059 RVA: 0x00503014 File Offset: 0x00501214
		private void DispatchTransferResult(int charId)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharId", charId);
			argBox.Set<CharacterDisplayData>("BeforeTransferCharacterDisplayData", this._beforeTransferCharacterDisplayData);
			argBox.Set<CharacterDisplayData>("AfterTransferCharacterDisplayData", this._afterTransferCharacterDisplayData);
			GEvent.OnEvent(UiEvents.MultiplyTransferItemResult, argBox);
		}

		// Token: 0x0600B004 RID: 45060 RVA: 0x00503068 File Offset: 0x00501268
		public void EnterMultiplyMode()
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
				bool flag2 = this.multiplyBg != null;
				if (flag2)
				{
					this.multiplyBg.SetActive(true);
				}
				this.ClearMultiplySelection(true);
				this.HideMultiplySelectButton();
				this.HideToggleMultiplyLock();
				this.RefreshMultiplyAvailableTool();
				this.RefreshMultiplyCanOperateItems();
				this.SaveFilterStates();
				this.RefreshSort();
				this.ShowItemMultiplyOperationPanel();
				Action onEnterMultiplyOperation = this._onEnterMultiplyOperation;
				if (onEnterMultiplyOperation != null)
				{
					onEnterMultiplyOperation();
				}
			}
		}

		// Token: 0x0600B005 RID: 45061 RVA: 0x0050315A File Offset: 0x0050135A
		public void ShowItemMultiplyOperationPanel()
		{
			this.RequestData(delegate
			{
				ArgumentBox args = this.GetItemMultiplyViewArgs();
				this.itemMultiplyOperationPanel.Show(args);
			});
		}

		// Token: 0x0600B006 RID: 45062 RVA: 0x00503170 File Offset: 0x00501370
		public void EnterSpecialBreakConvertToExpMultiplyMode()
		{
			bool isMultiItemSelect = this.IsMultiItemSelect;
			if (!isMultiItemSelect)
			{
				this.IsMultiItemSelect = true;
				this._currItemOperation = (this._multiplyItemOperationType = ItemOperationType.EItemOperationType.SpecialBreakConvertToExp);
				this.RefreshMultiplyCanOperateItems();
				this.ClearMultiplySelection(true);
			}
		}

		// Token: 0x0600B007 RID: 45063 RVA: 0x005031B4 File Offset: 0x005013B4
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

		// Token: 0x0600B008 RID: 45064 RVA: 0x005031F5 File Offset: 0x005013F5
		public void EnterRepairMode()
		{
			this._multiplyItemOperationType = ItemOperationType.EItemOperationType.Repair;
			this.EnterMultiplyMode();
		}

		// Token: 0x0600B009 RID: 45065 RVA: 0x00503206 File Offset: 0x00501406
		public void EnterDisassembleMode()
		{
			this._multiplyItemOperationType = ItemOperationType.EItemOperationType.Disassemble;
			this.EnterMultiplyMode();
		}

		// Token: 0x0600B00A RID: 45066 RVA: 0x00503218 File Offset: 0x00501418
		public void EnterPutPoisonMaterialMode()
		{
			bool isMultiItemSelect = this.IsMultiItemSelect;
			if (!isMultiItemSelect)
			{
				this.IsMultiItemSelect = true;
				this._currItemOperation = (this._multiplyItemOperationType = ItemOperationType.EItemOperationType.PutPoisonMaterial);
				this.RefreshMultiplyCanOperateItems();
				this.ClearMultiplySelection(true);
			}
		}

		// Token: 0x0600B00B RID: 45067 RVA: 0x0050325C File Offset: 0x0050145C
		public void EnterVillagerCraftMode()
		{
			bool isMultiItemSelect = this.IsMultiItemSelect;
			if (!isMultiItemSelect)
			{
				this.IsMultiItemSelect = true;
				this._currItemOperation = (this._multiplyItemOperationType = ItemOperationType.EItemOperationType.VillagerCraft);
				this.RefreshMultiplyCanOperateItems();
				this.ClearMultiplySelection(true);
			}
		}

		// Token: 0x0600B00C RID: 45068 RVA: 0x005032A0 File Offset: 0x005014A0
		public void EnterEventWindowMultiplyMode(sbyte operationType)
		{
			bool isMultiItemSelect = this.IsMultiItemSelect;
			if (!isMultiItemSelect)
			{
				this.IsMultiItemSelect = true;
				this._multiplyItemOperationType = (ItemOperationType.EItemOperationType)operationType;
				this._currItemOperation = (ItemOperationType.EItemOperationType)operationType;
				this.RefreshMultiplyCanOperateItems();
				this.ClearMultiplySelection(true);
			}
		}

		// Token: 0x0600B00D RID: 45069 RVA: 0x005032E4 File Offset: 0x005014E4
		public void RefreshMultiplyCanOperateItems()
		{
			this._canOperateItems.Clear();
			List<ItemDisplayData> items = this.CurMultiplyItems ?? new List<ItemDisplayData>();
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
					bool flag = d.Key.ItemType == 12;
					if (flag)
					{
						result = false;
					}
					else
					{
						bool flag2 = !ItemTemplateHelper.IsTransferable(d.Key.ItemType, d.Key.TemplateId);
						result = !flag2;
					}
					break;
				}
				case ItemOperationType.EItemOperationType.Feeding:
				{
					bool isFeedingCricket = this._isFeedingCricket;
					if (isFeedingCricket)
					{
						result = ViewCharacterMenuItems.IsBloodDew(d);
					}
					else
					{
						result = ItemTemplateHelper.IsFeedingAble(d.Key.ItemType, d.Key.TemplateId);
					}
					break;
				}
				case ItemOperationType.EItemOperationType.Confiscate:
					result = true;
					break;
				case ItemOperationType.EItemOperationType.PutPoisonMaterial:
				{
					bool flag3 = d.Key.ItemType == 5;
					if (flag3)
					{
						MaterialItem config = Config.Material.Instance[d.Key.TemplateId];
						result = config.InnatePoisons.IsNonZero();
					}
					else
					{
						bool flag4 = d.Key.ItemType == 4;
						if (flag4)
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
				this._canOperateItems.AddRange(itemList);
				this.CurMultiplyScrollView.SetItemList(this._canOperateItems);
				this._selectedItems.Clear();
				foreach (KeyValuePair<ItemDisplayData, int> keyValuePair in this.SelectedMultiplyItemDict)
				{
					ItemDisplayData itemDisplayData;
					int num;
					keyValuePair.Deconstruct(out itemDisplayData, out num);
					ItemDisplayData data2 = itemDisplayData;
					int count = num;
					ItemDisplayData tempData = data2.Clone(count);
					this._selectedItems.Add(tempData);
				}
				this.selectedScroll.SetItemList(this._selectedItems);
			}
			else
			{
				this._canOperateItems.AddRange(itemList);
				this.CurMultiplyScrollView.SetItemList(this._canOperateItems);
			}
			this.RefreshToggleSelectAll();
		}

		// Token: 0x0600B00E RID: 45070 RVA: 0x0050344C File Offset: 0x0050164C
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

		// Token: 0x0600B00F RID: 45071 RVA: 0x005034B4 File Offset: 0x005016B4
		private ArgumentBox GetItemMultiplyViewArgs()
		{
			ArgumentBox args = EasyPool.Get<ArgumentBox>();
			args.SetObject("ItemDict", this.SelectedMultiplyItemDict);
			args.SetObject("ItemList", this.SelectedMultiplyItemOrderedList);
			args.SetObject("ToolDict", this.CurOperationHandler.AllUsedToolDict);
			args.SetObject("EmptyToolKey", this._emptyToolKey);
			args.SetObject("CharData", this._taiwuCharacterDisplayData);
			args.Set("ItemOperationType", this._currItemOperation);
			args.Set("ItemSourceType", this.CurMultiplyItemSourceType);
			foreach (ItemDisplayData itemData in this.SelectedMultiplyItemOrderedList)
			{
				Dictionary<ItemDisplayData, short> usedToolDict = this.CurOperationHandler.GetUsedToolDict(itemData);
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
				if (eitemOperationType == ItemOperationType.EItemOperationType.Disassemble)
				{
					ResourceInts getResource = default(ResourceInts);
					List<short> certainMaterialList = new List<short>();
					List<short> chanceMaterialList = new List<short>();
					foreach (ItemDisplayData itemData2 in this.SelectedMultiplyItemOrderedList)
					{
						int amount = this.SelectedMultiplyItemDict[itemData2];
						ResourceInts resource = MultiplyItemListScroll.GetDisassembleResources(itemData2, amount);
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
			this.GetGetItemMultiplyViewArgsForFeeding(args);
			return args;
		}

		// Token: 0x0600B010 RID: 45072 RVA: 0x005037CC File Offset: 0x005019CC
		private void GetGetItemMultiplyViewArgsForFeeding(ArgumentBox args)
		{
			bool flag = this._feedingTargetList.Count == 0;
			if (flag)
			{
				this._feedingTargetList.Clear();
				bool isFeedingCricket = this._isFeedingCricket;
				if (isFeedingCricket)
				{
					int spiritMax = GlobalConfig.Instance.CricketSpiritMax;
					this._feedingTargetList.AddRange(from d in this.CurMultiplyItems
					where d.Key.ItemType == 11 && d.CricketData != null && d.Durability > 0 && d.CricketData.Spirit < spiritMax
					select d);
					this._feedingTargetList.Sort((ItemDisplayData a, ItemDisplayData b) => b.CricketData.Spirit.CompareTo(a.CricketData.Spirit));
				}
				else
				{
					this._feedingTargetList.AddRange(this.CurMultiplyItems.Where(new Func<ItemDisplayData, bool>(ViewCharacterMenuItems.CheckNeedFeedCarrier)));
					this._feedingTargetList.Sort(delegate(ItemDisplayData a, ItemDisplayData b)
					{
						bool flag5 = b.CarrierTamePoint != a.CarrierTamePoint;
						int result;
						if (flag5)
						{
							result = b.CarrierTamePoint.CompareTo(a.CarrierTamePoint);
						}
						else
						{
							result = b.Durability.CompareTo(a.Durability);
						}
						return result;
					});
				}
				bool flag2 = this._feedingTargetOnEnter != null;
				if (flag2)
				{
					bool isValid = this._isFeedingCricket ? (this._feedingTargetOnEnter.Key.ItemType == 11) : ViewCharacterMenuItems.CheckNeedFeedCarrier(this._feedingTargetOnEnter);
					bool flag3 = isValid && this._feedingTargetList.Remove(this._feedingTargetOnEnter);
					if (flag3)
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
			List<ItemDisplayData> items = this.CurMultiplyItems;
			bool hasCarrierFeeding = items != null && items.Any(new Func<ItemDisplayData, bool>(ViewCharacterMenuItems.CheckNeedFeedCarrier));
			args.Set("HasCarrierFeeding", hasCarrierFeeding);
			bool hasCricketFeeding = false;
			bool flag4 = items != null && CricketPolymorphHelper.IsCricketPolymorphEnabled;
			if (flag4)
			{
				int cricketSpiritMax = GlobalConfig.Instance.CricketSpiritMax;
				hasCricketFeeding = (items.Any((ItemDisplayData d) => d.Key.ItemType == 11 && d.CricketData != null && d.CricketData.Spirit < cricketSpiritMax) && items.Any(new Func<ItemDisplayData, bool>(ViewCharacterMenuItems.IsBloodDew)));
			}
			args.Set("HasCricketFeeding", hasCricketFeeding);
		}

		// Token: 0x0600B011 RID: 45073 RVA: 0x00503A04 File Offset: 0x00501C04
		public static ResourceInts GetDisassembleResources(ItemDisplayData itemDisplayData, int amount)
		{
			ItemKey itemKey = itemDisplayData.Key;
			bool flag = ItemType.IsEquipmentItemType(itemKey.ItemType);
			ResourceInts resourceInts;
			if (flag)
			{
				resourceInts = ItemTemplateHelper.GetDisassembleResources(itemDisplayData.MaterialResources, itemKey.ItemType, itemKey.TemplateId, amount);
			}
			else
			{
				bool flag2 = itemKey.ItemType == 12;
				if (flag2)
				{
					MiscItem miscConfig = Misc.Instance[itemKey.TemplateId];
					MakeItemSubTypeItem makeConfig = MakeItemSubType.Instance[miscConfig.MakeItemSubType];
					bool flag3 = makeConfig == null;
					MaterialResources presetResources;
					if (flag3)
					{
						presetResources = default(MaterialResources);
					}
					else
					{
						presetResources = makeConfig.MaxMaterialResources;
					}
					resourceInts = ItemTemplateHelper.GetDisassembleResources(presetResources, itemKey.ItemType, itemKey.TemplateId, amount);
				}
				else
				{
					resourceInts = ItemTemplateHelper.GetDisassembleResources(default(MaterialResources), itemKey.ItemType, itemKey.TemplateId, amount);
				}
			}
			return resourceInts;
		}

		// Token: 0x0600B012 RID: 45074 RVA: 0x00503ADC File Offset: 0x00501CDC
		public void TryExitMultiplyMode(Action onConfirm)
		{
			bool isMultiplyLock = this.IsMultiplyLock;
			if (isMultiplyLock)
			{
				this.ExitMultiplyLock();
				Action onConfirm2 = onConfirm;
				if (onConfirm2 != null)
				{
					onConfirm2();
				}
			}
			bool flag = this.SelectedMultiplyItemDict.Count == 0;
			if (flag)
			{
				this.ExitMultiplyMode();
				Action onConfirm3 = onConfirm;
				if (onConfirm3 != null)
				{
					onConfirm3();
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
					Action onConfirm4 = onConfirm;
					if (onConfirm4 != null)
					{
						onConfirm4();
					}
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		}

		// Token: 0x0600B013 RID: 45075 RVA: 0x00503BC0 File Offset: 0x00501DC0
		public void ExitMultiplyMode()
		{
			this.ShowMultiplySelectButton(null, true);
			this.ShowToggleMultiplyLock(null, true);
			bool flag = !this.IsMultiItemSelect;
			if (!flag)
			{
				this.IsMultiItemSelect = false;
				this.SwitchSelection.isOn = false;
				this.SwitchSelection.gameObject.SetActive(false);
				this.SwitchSelection.transform.parent.parent.gameObject.SetActive(false);
				this.ToggleSelectAll.gameObject.SetActive(false);
				bool flag2 = this.multiplyBg != null;
				if (flag2)
				{
					this.multiplyBg.SetActive(false);
				}
				this._feedingTarget = null;
				this._feedingTargetOnEnter = null;
				this._feedingTargetList.Clear();
				this.ClearMultiplySelection(true);
				this._currItemOperation = (this._multiplyItemOperationType = ItemOperationType.EItemOperationType.Invalid);
				this.RefreshItems();
				bool flag3 = this.HasAnySavedFilterState();
				if (flag3)
				{
					this.ResetAllTogglesVisible();
					this.RestoreFilterStates();
				}
				else
				{
					this.RefreshSort();
				}
				Action onExitMultiplyOperation = this._onExitMultiplyOperation;
				if (onExitMultiplyOperation != null)
				{
					onExitMultiplyOperation();
				}
				this.itemMultiplyOperationPanel.Hide();
				AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			}
		}

		// Token: 0x0600B014 RID: 45076 RVA: 0x00503CF8 File Offset: 0x00501EF8
		private void RefreshToggleSelectAll()
		{
			bool canSelectAll = this.itemScroll.FilteredData.Count > 0 && this.itemScroll.FilteredData.Any(delegate(ITradeableContent d)
			{
				List<ItemDisplayData> list;
				return this.CheckInteractable(d, out list) && !((ItemDisplayData)d).GetUsingTypeNeedConfirm();
			});
			bool isAll = this.SelectedMultiplyItemOrderedList.Count > 0 && this.itemScroll.FilteredData.Where(delegate(ITradeableContent d)
			{
				List<ItemDisplayData> list;
				return this.CheckInteractable(d, out list) && !((ItemDisplayData)d).GetUsingTypeNeedConfirm();
			}).All((ITradeableContent d) => this.SelectedMultiplyItemOrderedList.Any((ItemDisplayData s) => s.RealKey.Equals(d.RealKey)));
			this.ToggleSelectAll.SetIsOnWithoutNotify(isAll);
			this.ToggleSelectAll.interactable = canSelectAll;
		}

		// Token: 0x0600B015 RID: 45077 RVA: 0x00503D94 File Offset: 0x00501F94
		public void ClearMultiplySelection(bool isAll)
		{
			this.TotalSelectedCount = 0;
			if (isAll)
			{
				foreach (KeyValuePair<ItemSourceType, MultiplyOperationHandler> keyValuePair in this._multiplyOperationHandlerDict)
				{
					ItemSourceType itemSourceType;
					MultiplyOperationHandler multiplyOperationHandler;
					keyValuePair.Deconstruct(out itemSourceType, out multiplyOperationHandler);
					MultiplyOperationHandler handler = multiplyOperationHandler;
					handler.Clear();
				}
			}
			else
			{
				this.CurOperationHandler.Clear();
			}
			this.CurMultiplyScrollView.ReRender();
		}

		// Token: 0x0600B016 RID: 45078 RVA: 0x00503E20 File Offset: 0x00502020
		private void ClearMultiplyItemUsingState(List<MultiplyOperation> multiplyOperationList)
		{
			using (List<MultiplyOperation>.Enumerator enumerator = multiplyOperationList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MultiplyOperation operation = enumerator.Current;
					ItemDisplayData data = this.SelectedMultiplyItemOrderedList.Find((ItemDisplayData d) => d.ContainsItemKey(operation.Target));
					this.ClearItemUsingState(data);
					data.ChangeAmount(operation.Target, false, 1);
					bool flag = data.Amount <= 0;
					if (flag)
					{
						this.CurMultiplyItems.Remove(data);
					}
				}
			}
		}

		// Token: 0x0600B017 RID: 45079 RVA: 0x00503EC8 File Offset: 0x005020C8
		public void OnExitMultiplyOperation(ArgumentBox argsBox)
		{
			this.ExitMultiplyMode();
		}

		// Token: 0x0600B018 RID: 45080 RVA: 0x00503ED4 File Offset: 0x005020D4
		public void OnItemMultiplyOperationTypeChange(ArgumentBox argsBox)
		{
			Enum type;
			bool flag = argsBox.Get("ItemOperationType", out type);
			if (flag)
			{
				ItemOperationType.EItemOperationType newOpType = (ItemOperationType.EItemOperationType)type;
				bool isCricket;
				bool flag2 = newOpType == ItemOperationType.EItemOperationType.Feeding && argsBox.Get("IsCricketFeeding", out isCricket) && isCricket != this._isFeedingCricket;
				if (flag2)
				{
					this._feedingTarget = null;
					this._feedingTargetList.Clear();
					List<ItemDisplayData> items = this.CurMultiplyItems;
					bool flag3 = items != null;
					if (flag3)
					{
						bool flag4 = isCricket && CricketPolymorphHelper.IsCricketPolymorphEnabled;
						if (flag4)
						{
							int spiritMax = GlobalConfig.Instance.CricketSpiritMax;
							ItemDisplayData target = items.FirstOrDefault((ItemDisplayData d) => d.Key.ItemType == 11 && d.CricketData != null && d.CricketData.Spirit < spiritMax);
							bool flag5 = target != null;
							if (flag5)
							{
								this._feedingTarget = target;
							}
						}
						else
						{
							ItemDisplayData target2 = items.FirstOrDefault(new Func<ItemDisplayData, bool>(ViewCharacterMenuItems.CheckNeedFeedCarrier));
							bool flag6 = target2 != null;
							if (flag6)
							{
								this._feedingTarget = target2;
							}
						}
					}
					this._currItemOperation = newOpType;
					this._multiplyItemOperationType = this._currItemOperation;
					this.ClearMultiplySelection(false);
					this.RefreshSort();
					this.RefreshItems();
					this.RequestData(delegate
					{
						ArgumentBox args = this.GetItemMultiplyViewArgs();
						GEvent.OnEvent(UiEvents.ItemMultiplyOperationContentChange, args);
					});
				}
				else
				{
					this._currItemOperation = newOpType;
					this._multiplyItemOperationType = this._currItemOperation;
					this.ClearMultiplySelection(false);
					this.RefreshSort();
					this.RefreshItems();
					this.RequestData(delegate
					{
						ArgumentBox args = this.GetItemMultiplyViewArgs();
						GEvent.OnEvent(UiEvents.ItemMultiplyOperationContentChange, args);
					});
				}
			}
		}

		// Token: 0x0600B019 RID: 45081 RVA: 0x00504050 File Offset: 0x00502250
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
		}

		// Token: 0x0600B01A RID: 45082 RVA: 0x005040E8 File Offset: 0x005022E8
		public void OnItemMultiplyOperationCancelSelection(ArgumentBox argsBox)
		{
			ItemDisplayData itemData;
			bool flag = argsBox.Get<ItemDisplayData>("ItemData", out itemData);
			if (flag)
			{
				Dictionary<ItemDisplayData, short> usedToolDict = this.CurOperationHandler.GetUsedToolDict(itemData);
				List<ItemDisplayData> usedToolList = new List<ItemDisplayData>(usedToolDict.Keys);
				this.SetItemSelectCount(itemData.RealKey, 0, usedToolList, true);
			}
		}

		// Token: 0x0600B01B RID: 45083 RVA: 0x00504132 File Offset: 0x00502332
		private void RefreshSort()
		{
			this.RefreshSort(this.CurMultiplyScrollView);
			this.RefreshSort(this.selectedScroll);
		}

		// Token: 0x0600B01C RID: 45084 RVA: 0x00504150 File Offset: 0x00502350
		private void RefreshSort(ItemListScroll itemListScroll)
		{
			itemListScroll.SortAndFilterController.ClearAllFilter();
			int lineId = EFilterLine.MainFilter.ToInt();
			ItemOperationType.EItemOperationType currItemOperation = this._currItemOperation;
			ItemOperationType.EItemOperationType eitemOperationType = currItemOperation;
			if (eitemOperationType != ItemOperationType.EItemOperationType.Repair)
			{
				if (eitemOperationType != ItemOperationType.EItemOperationType.Disassemble)
				{
					if (eitemOperationType != ItemOperationType.EItemOperationType.Feeding)
					{
						itemListScroll.SortAndFilterController.SetToggleIsOn(lineId, -1);
						itemListScroll.SortAndFilterController.SetToggleVisible(lineId, -1);
					}
					else
					{
						bool isFeedingCricket = this._isFeedingCricket;
						if (isFeedingCricket)
						{
							itemListScroll.SortAndFilterController.SetToggleIsOn(lineId, EMainFilterKeys.Misc.ToInt());
							itemListScroll.SortAndFilterController.SetToggleVisible(lineId, EMainFilterKeys.Misc.ToInt());
						}
						else
						{
							itemListScroll.SortAndFilterController.SetToggleIsOn(lineId, EMainFilterKeys.Material.ToInt());
							itemListScroll.SortAndFilterController.SetToggleVisible(lineId, EMainFilterKeys.Material.ToInt());
							itemListScroll.SortAndFilterController.SetDropdownOption(EFilterLine.MaterialFilter.ToInt(), 0, EMaterialSubFilterKeys.Food.ToInt());
							itemListScroll.SortAndFilterController.SetDropdownMenuVisible(EFilterLine.MaterialFilter.ToInt(), 0, false);
						}
					}
				}
				else
				{
					itemListScroll.SortAndFilterController.SetToggleIsOn(lineId, -1);
					itemListScroll.SortAndFilterController.SetToggleVisible(lineId, MultiplyItemListScroll.DisassembleFilterTypes, false);
				}
			}
			else
			{
				itemListScroll.SortAndFilterController.SetToggleIsOn(lineId, EMainFilterKeys.Equip.ToInt());
				itemListScroll.SortAndFilterController.SetToggleVisible(lineId, EMainFilterKeys.Equip.ToInt());
			}
			bool flag = this._currItemOperation != ItemOperationType.EItemOperationType.Feeding;
			if (flag)
			{
				itemListScroll.SortAndFilterController.SetDropdownMenuVisible(EFilterLine.MaterialFilter.ToInt(), 0, true);
			}
		}

		// Token: 0x0600B01D RID: 45085 RVA: 0x005042E7 File Offset: 0x005024E7
		private void SaveFilterStates()
		{
			MultiplyItemListScroll.SaveFilterState(this.CurMultiplyScrollView);
			MultiplyItemListScroll.SaveFilterState(this.selectedScroll);
		}

		// Token: 0x0600B01E RID: 45086 RVA: 0x00504302 File Offset: 0x00502502
		private static void SaveFilterState(ItemListScroll scroll)
		{
			if (scroll != null)
			{
				SortAndFilterController<ITradeableContent> sortAndFilterController = scroll.SortAndFilterController;
				if (sortAndFilterController != null)
				{
					sortAndFilterController.SaveFilterStateFromUI();
				}
			}
		}

		// Token: 0x0600B01F RID: 45087 RVA: 0x0050431C File Offset: 0x0050251C
		private bool HasAnySavedFilterState()
		{
			ItemListScroll curMultiplyScrollView = this.CurMultiplyScrollView;
			bool? flag;
			if (curMultiplyScrollView == null)
			{
				flag = null;
			}
			else
			{
				SortAndFilterController<ITradeableContent> sortAndFilterController = curMultiplyScrollView.SortAndFilterController;
				flag = ((sortAndFilterController != null) ? new bool?(sortAndFilterController.HasSavedFilterState) : null);
			}
			bool? flag2 = flag;
			bool result;
			if (!flag2.GetValueOrDefault())
			{
				ItemListScroll itemListScroll = this.selectedScroll;
				bool? flag3;
				if (itemListScroll == null)
				{
					flag3 = null;
				}
				else
				{
					SortAndFilterController<ITradeableContent> sortAndFilterController2 = itemListScroll.SortAndFilterController;
					flag3 = ((sortAndFilterController2 != null) ? new bool?(sortAndFilterController2.HasSavedFilterState) : null);
				}
				flag2 = flag3;
				result = flag2.GetValueOrDefault();
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x0600B020 RID: 45088 RVA: 0x005043AC File Offset: 0x005025AC
		private void ResetAllTogglesVisible()
		{
			int lineId = EFilterLine.MainFilter.ToInt();
			ItemListScroll curMultiplyScrollView = this.CurMultiplyScrollView;
			if (curMultiplyScrollView != null)
			{
				SortAndFilterController<ITradeableContent> sortAndFilterController = curMultiplyScrollView.SortAndFilterController;
				if (sortAndFilterController != null)
				{
					sortAndFilterController.ResetAllTogglesVisible(lineId);
				}
			}
			ItemListScroll itemListScroll = this.selectedScroll;
			if (itemListScroll != null)
			{
				SortAndFilterController<ITradeableContent> sortAndFilterController2 = itemListScroll.SortAndFilterController;
				if (sortAndFilterController2 != null)
				{
					sortAndFilterController2.ResetAllTogglesVisible(lineId);
				}
			}
		}

		// Token: 0x0600B021 RID: 45089 RVA: 0x00504402 File Offset: 0x00502602
		private void RestoreFilterStates()
		{
			ItemListScroll curMultiplyScrollView = this.CurMultiplyScrollView;
			if (curMultiplyScrollView != null)
			{
				SortAndFilterController<ITradeableContent> sortAndFilterController = curMultiplyScrollView.SortAndFilterController;
				if (sortAndFilterController != null)
				{
					sortAndFilterController.RestoreFilterState();
				}
			}
			ItemListScroll itemListScroll = this.selectedScroll;
			if (itemListScroll != null)
			{
				SortAndFilterController<ITradeableContent> sortAndFilterController2 = itemListScroll.SortAndFilterController;
				if (sortAndFilterController2 != null)
				{
					sortAndFilterController2.RestoreFilterState();
				}
			}
		}

		// Token: 0x0600B022 RID: 45090 RVA: 0x00504440 File Offset: 0x00502640
		private void RefreshUsedTool()
		{
			foreach (KeyValuePair<ItemDisplayData, short> pair in this.CurOperationHandler.AllUsedToolDict)
			{
				ItemDisplayData tool = pair.Key;
				bool flag = tool.Durability <= 0;
				if (flag)
				{
					bool flag2 = this.Items.Exists((ItemDisplayData i) => i == tool);
					if (flag2)
					{
						this.Items.Remove(tool);
					}
				}
			}
		}

		// Token: 0x0600B023 RID: 45091 RVA: 0x005044F0 File Offset: 0x005026F0
		public List<ItemDisplayData> GetAvailableToolList(ItemDisplayData itemData, out bool hasAnySkillMatchedTool)
		{
			hasAnySkillMatchedTool = false;
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
				short required = (this._currItemOperation == ItemOperationType.EItemOperationType.Repair) ? ItemTemplateHelper.GetRepairRequiredAttainment(itemData.Key.ItemType, itemData.Key.TemplateId, itemData.Durability) : ItemTemplateHelper.GetDisassembleRequiredAttainment(itemData.Key.ItemType, itemData.Key.TemplateId);
				hasAnySkillMatchedTool = this.UsableToolOrderedList.Any(delegate(ItemDisplayData d)
				{
					CraftToolItem toolConfig = CraftTool.Instance[d.Key.TemplateId];
					bool skillIsMeet = toolConfig.RequiredLifeSkillTypes.Contains(itemLifeSkillType);
					bool flag2 = !skillIsMeet;
					return !flag2;
				});
				List<ItemDisplayData> availableToolList = this.UsableToolOrderedList.Where(delegate(ItemDisplayData d)
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
						short charAttainment = this._lifeSkillMonitor.Attainments[(int)itemLifeSkillType];
						short finalAttainment = ViewMake.GetFinalAttainment(d.Key.TemplateId, charAttainment, itemLifeSkillType);
						bool attainmentIsMeet = finalAttainment >= required;
						bool flag3 = !attainmentIsMeet;
						if (flag3)
						{
							result2 = false;
						}
						else
						{
							short used;
							this.CurOperationHandler.AllUsedToolDict.TryGetValue(d, out used);
							bool durabilityIsMeet = used < d.Durability;
							bool flag4 = !durabilityIsMeet;
							result2 = !flag4;
						}
					}
					return result2;
				}).OrderBy(delegate(ItemDisplayData d)
				{
					CraftToolItem toolConfig = CraftTool.Instance[d.Key.TemplateId];
					sbyte grade = ItemTemplateHelper.GetGrade(itemData.Key.ItemType, itemData.Key.TemplateId);
					return toolConfig.DurabilityCost[(int)grade];
				}).ToList<ItemDisplayData>();
				result = availableToolList;
			}
			return result;
		}

		// Token: 0x0600B024 RID: 45092 RVA: 0x00504654 File Offset: 0x00502854
		private void ClearItemUsingState(ItemDisplayData itemData)
		{
			bool flag = this.IsMultiItemSelect && this.CurMultiplyItemSourceType != ItemSourceType.Inventory;
			if (!flag)
			{
				ItemDisplayData.ClearItemUsingState(itemData, this.Items);
			}
		}

		// Token: 0x0600B025 RID: 45093 RVA: 0x0050468C File Offset: 0x0050288C
		public void RefreshItems()
		{
			bool isMultiItemSelect = this.IsMultiItemSelect;
			if (isMultiItemSelect)
			{
				this.RefreshMultiplyAvailableTool();
				this.RefreshSelectedCount();
				this.RefreshMultiplyCanOperateItems();
				this.SendItemMultiplyOperationContentChange(null);
			}
			else
			{
				bool isMultiplyLock = this.IsMultiplyLock;
				if (isMultiplyLock)
				{
					this.RefreshMultiplyCanLockItems();
				}
				else
				{
					this._canOperateItems.Clear();
					List<ItemDisplayData> items = this.CurMultiplyItems;
					this.CurMultiplyScrollView.SetItemList(items);
				}
			}
		}

		// Token: 0x0600B026 RID: 45094 RVA: 0x005046FC File Offset: 0x005028FC
		public void ShowMultiplySelectButton(CButton button = null, bool canTransfer = true)
		{
			if (button == null)
			{
				button = this.BtnMultiplySelect;
			}
			bool flag = !button;
			if (!flag)
			{
				bool isGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
				bool isShow = this.CurCharacterIsTaiwu && !isGuiding && canTransfer;
				button.gameObject.SetActive(isShow);
				button.interactable = true;
			}
		}

		// Token: 0x0600B027 RID: 45095 RVA: 0x00504756 File Offset: 0x00502956
		public void HideMultiplySelectButton()
		{
			CButton btnMultiplySelect = this.BtnMultiplySelect;
			if (btnMultiplySelect != null)
			{
				btnMultiplySelect.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600B028 RID: 45096 RVA: 0x00504774 File Offset: 0x00502974
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

		// Token: 0x0600B029 RID: 45097 RVA: 0x005047E4 File Offset: 0x005029E4
		private void EnterMultiplyLock()
		{
			bool isMultiplyLock = this.IsMultiplyLock;
			if (!isMultiplyLock)
			{
				this.IsMultiplyLock = true;
				this.HideMultiplySelectButton();
				this.ToggleMultiplyLock.SetIsOnWithoutNotify(true);
				this.ObjMultiplyLockTip.SetActive(true);
				this.ClearMultiplySelection(false);
				this.RefreshItems();
				Action onEnterMultiplyLock = this._onEnterMultiplyLock;
				if (onEnterMultiplyLock != null)
				{
					onEnterMultiplyLock();
				}
			}
		}

		// Token: 0x0600B02A RID: 45098 RVA: 0x00504848 File Offset: 0x00502A48
		private void ExitMultiplyLock()
		{
			this.IsMultiplyLock = false;
			this.ToggleMultiplyLock.SetIsOnWithoutNotify(false);
			this.ObjMultiplyLockTip.SetActive(false);
			this.ClearMultiplySelection(false);
			this.RefreshItems();
			Action onExitMultiplyLock = this._onExitMultiplyLock;
			if (onExitMultiplyLock != null)
			{
				onExitMultiplyLock();
			}
		}

		// Token: 0x0600B02B RID: 45099 RVA: 0x0050489C File Offset: 0x00502A9C
		private void RefreshMultiplyCanLockItems()
		{
			this._canOperateItems.Clear();
			this._canOperateItems.AddRange(from d in this.CurMultiplyItems
			where d.IsTransferable && !d.IsResource
			select d);
			this.CurMultiplyScrollView.SetItemList(this._canOperateItems);
		}

		// Token: 0x0600B02C RID: 45100 RVA: 0x00504900 File Offset: 0x00502B00
		public void ShowToggleMultiplyLock(CToggle button = null, bool canTransfer = true)
		{
			if (button == null)
			{
				button = this.ToggleMultiplyLock;
			}
			bool flag = !button;
			if (!flag)
			{
				bool isGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
				bool isShow = this.CurCharacterIsTaiwu && !isGuiding && canTransfer;
				button.gameObject.SetActive(isShow);
				button.interactable = true;
			}
		}

		// Token: 0x0600B02D RID: 45101 RVA: 0x0050495A File Offset: 0x00502B5A
		public void HideToggleMultiplyLock()
		{
			CToggle toggleMultiplyLock = this.ToggleMultiplyLock;
			if (toggleMultiplyLock != null)
			{
				toggleMultiplyLock.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600B030 RID: 45104 RVA: 0x00504A4C File Offset: 0x00502C4C
		[CompilerGenerated]
		private void <Init>g__InitButton|91_0(ItemListScroll itemListScroll)
		{
			bool flag = itemListScroll == null;
			if (!flag)
			{
				CButton btnMultiplySelect = itemListScroll.BtnMultiplySelect;
				bool flag2 = btnMultiplySelect;
				if (flag2)
				{
					this.ShowMultiplySelectButton(btnMultiplySelect, true);
					bool flag3 = btnMultiplySelect;
					if (flag3)
					{
						btnMultiplySelect.ClearAndAddListener(delegate
						{
							this.EnterMultiplyMode();
						});
					}
				}
				CToggle switchSelection = itemListScroll.SwitchSelection;
				bool flag4 = switchSelection;
				if (flag4)
				{
					switchSelection.gameObject.SetActive(false);
					switchSelection.transform.parent.parent.gameObject.SetActive(false);
				}
				CToggle toggleSelectAll = itemListScroll.ToggleSelectAll;
				bool flag5 = toggleSelectAll;
				if (flag5)
				{
					toggleSelectAll.gameObject.SetActive(false);
					toggleSelectAll.onValueChanged.RemoveAllListeners();
					toggleSelectAll.onValueChanged.AddListener(delegate(bool isOn)
					{
						this.SelectAll(isOn);
					});
					toggleSelectAll.isOn = false;
				}
				CToggle toggleMultiplyLock = itemListScroll.ToggleMultiplyLock;
				bool flag6 = toggleMultiplyLock;
				if (flag6)
				{
					this.ShowToggleMultiplyLock(toggleMultiplyLock, true);
					toggleMultiplyLock.onValueChanged.RemoveAllListeners();
					toggleMultiplyLock.onValueChanged.AddListener(delegate(bool isOn)
					{
						if (isOn)
						{
							this.EnterMultiplyLock();
						}
						else
						{
							this.ExitMultiplyLock();
						}
					});
					toggleMultiplyLock.SetIsOnWithoutNotify(false);
				}
				GameObject objMultiplyLockTip = itemListScroll.ObjMultiplyLockTip;
				bool flag7 = objMultiplyLockTip;
				if (flag7)
				{
					objMultiplyLockTip.SetActive(false);
				}
			}
		}

		// Token: 0x04008830 RID: 34864
		[SerializeField]
		private ItemListScroll itemScroll;

		// Token: 0x04008831 RID: 34865
		[SerializeField]
		private ItemListScroll selectedScroll;

		// Token: 0x04008832 RID: 34866
		[SerializeField]
		private ItemMultiplyOperationPanel itemMultiplyOperationPanel;

		// Token: 0x04008833 RID: 34867
		[SerializeField]
		private GameObject multiplyBg;

		// Token: 0x04008834 RID: 34868
		public static readonly List<int> DisassembleFilterTypes = new List<int>
		{
			EMainFilterKeys.Equip.ToInt(),
			EMainFilterKeys.Material.ToInt(),
			EMainFilterKeys.Misc.ToInt()
		};

		// Token: 0x04008835 RID: 34869
		private Dictionary<ItemSourceType, List<ItemDisplayData>> _itemDict;

		// Token: 0x04008836 RID: 34870
		[NonSerialized]
		public long MaxWorthCanBeLentToTaiwu;

		// Token: 0x04008837 RID: 34871
		[NonSerialized]
		public ItemSourceType ItemSourceType = ItemSourceType.Inventory;

		// Token: 0x04008838 RID: 34872
		[NonSerialized]
		public int CurCharId;

		// Token: 0x04008839 RID: 34873
		private ItemOperationType.EItemOperationType _currItemOperation;

		// Token: 0x0400883A RID: 34874
		private List<ItemDisplayData> _canOperateItems = new List<ItemDisplayData>();

		// Token: 0x0400883B RID: 34875
		private List<ItemDisplayData> _selectedItems = new List<ItemDisplayData>();

		// Token: 0x0400883C RID: 34876
		private ItemDisplayData _feedingTarget;

		// Token: 0x0400883D RID: 34877
		private ItemDisplayData _feedingTargetOnEnter;

		// Token: 0x0400883E RID: 34878
		private readonly List<ItemDisplayData> _feedingTargetList = new List<ItemDisplayData>();

		// Token: 0x0400883F RID: 34879
		private readonly Dictionary<ItemSourceType, MultiplyOperationHandler> _multiplyOperationHandlerDict = new Dictionary<ItemSourceType, MultiplyOperationHandler>
		{
			{
				ItemSourceType.Inventory,
				new MultiplyOperationHandler()
			},
			{
				ItemSourceType.Warehouse,
				new MultiplyOperationHandler()
			},
			{
				ItemSourceType.Treasury,
				new MultiplyOperationHandler()
			},
			{
				ItemSourceType.Stock,
				new MultiplyOperationHandler()
			}
		};

		// Token: 0x04008842 RID: 34882
		private ItemOperationType.EItemOperationType _multiplyItemOperationType = ItemOperationType.EItemOperationType.Disassemble;

		// Token: 0x04008843 RID: 34883
		private ResourceMonitor _resourceMonitor;

		// Token: 0x04008844 RID: 34884
		private LifeSkillMonitor _lifeSkillMonitor;

		// Token: 0x04008845 RID: 34885
		private Action _onEnterMultiplyOperation;

		// Token: 0x04008846 RID: 34886
		private Action _onExitMultiplyOperation;

		// Token: 0x04008847 RID: 34887
		private Action _onEnterMultiplyLock;

		// Token: 0x04008848 RID: 34888
		private Action _onExitMultiplyLock;

		// Token: 0x04008849 RID: 34889
		[TupleElementNames(new string[]
		{
			"data",
			"count"
		})]
		private Action<List<ValueTuple<ItemDisplayData, int>>> _onContentChange;

		// Token: 0x0400884C RID: 34892
		private bool _isFeedDurabilityMax;

		// Token: 0x0400884D RID: 34893
		private bool _isFeedTameMax;

		// Token: 0x0400884E RID: 34894
		private bool _isFeedSpiritMax;

		// Token: 0x0400884F RID: 34895
		private ItemKey _emptyToolKey;

		// Token: 0x04008850 RID: 34896
		private CharacterDisplayData _taiwuCharacterDisplayData;

		// Token: 0x04008851 RID: 34897
		private Func<ItemDisplayData, bool> _filter;

		// Token: 0x04008853 RID: 34899
		private CharacterDisplayData _beforeTransferCharacterDisplayData;

		// Token: 0x04008854 RID: 34900
		private CharacterDisplayData _afterTransferCharacterDisplayData;
	}
}
