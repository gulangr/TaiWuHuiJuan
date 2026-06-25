using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll.Item;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Components.Item
{
	// Token: 0x02000EF5 RID: 3829
	public class SimpleMultiplyItemListScroll : MonoBehaviour
	{
		// Token: 0x170013FE RID: 5118
		// (get) Token: 0x0600B042 RID: 45122 RVA: 0x005053FF File Offset: 0x005035FF
		// (set) Token: 0x0600B043 RID: 45123 RVA: 0x00505407 File Offset: 0x00503607
		public bool HasInit { get; private set; }

		// Token: 0x170013FF RID: 5119
		// (get) Token: 0x0600B044 RID: 45124 RVA: 0x00505410 File Offset: 0x00503610
		// (set) Token: 0x0600B045 RID: 45125 RVA: 0x00505418 File Offset: 0x00503618
		public bool IsMultiItemSelect { get; private set; }

		// Token: 0x17001400 RID: 5120
		// (get) Token: 0x0600B046 RID: 45126 RVA: 0x00505421 File Offset: 0x00503621
		public ItemListScroll CurMultiplyScrollView
		{
			get
			{
				return this.itemScroll;
			}
		}

		// Token: 0x17001401 RID: 5121
		// (get) Token: 0x0600B047 RID: 45127 RVA: 0x00505429 File Offset: 0x00503629
		public ItemListScroll SelectedScrollView
		{
			get
			{
				return this.selectedScroll;
			}
		}

		// Token: 0x17001402 RID: 5122
		// (get) Token: 0x0600B048 RID: 45128 RVA: 0x00505431 File Offset: 0x00503631
		public CToggle SwitchSelection
		{
			get
			{
				return (this.itemScroll != null) ? this.itemScroll.SwitchSelection : null;
			}
		}

		// Token: 0x17001403 RID: 5123
		// (get) Token: 0x0600B049 RID: 45129 RVA: 0x0050544F File Offset: 0x0050364F
		public Dictionary<ItemDisplayData, int> SelectedMultiplyItemDict
		{
			get
			{
				return this._selection.SelectedItemDict;
			}
		}

		// Token: 0x17001404 RID: 5124
		// (get) Token: 0x0600B04A RID: 45130 RVA: 0x0050545C File Offset: 0x0050365C
		public List<ItemDisplayData> SelectedMultiplyItemOrderedList
		{
			get
			{
				return this._selection.SelectedItemOrderedList;
			}
		}

		// Token: 0x17001405 RID: 5125
		// (get) Token: 0x0600B04B RID: 45131 RVA: 0x00505469 File Offset: 0x00503669
		public CToggle ToggleSelectAll
		{
			get
			{
				return (this.itemScroll != null) ? this.itemScroll.ToggleSelectAll : null;
			}
		}

		// Token: 0x17001406 RID: 5126
		// (get) Token: 0x0600B04C RID: 45132 RVA: 0x00505487 File Offset: 0x00503687
		// (set) Token: 0x0600B04D RID: 45133 RVA: 0x0050548F File Offset: 0x0050368F
		public int MaxSelectCount { get; set; }

		// Token: 0x17001407 RID: 5127
		// (get) Token: 0x0600B04E RID: 45134 RVA: 0x00505498 File Offset: 0x00503698
		// (set) Token: 0x0600B04F RID: 45135 RVA: 0x005054A0 File Offset: 0x005036A0
		public int TotalSelectedCount { get; private set; }

		// Token: 0x17001408 RID: 5128
		// (get) Token: 0x0600B050 RID: 45136 RVA: 0x005054AC File Offset: 0x005036AC
		public bool IsSelectedListExpanded
		{
			get
			{
				bool result;
				if (this.selectedScroll != null)
				{
					CToggle switchSelection = this.SwitchSelection;
					result = (switchSelection != null && switchSelection.isOn);
				}
				else
				{
					result = false;
				}
				return result;
			}
		}

		// Token: 0x0600B051 RID: 45137 RVA: 0x005054DD File Offset: 0x005036DD
		public void RefreshSelectedScrollListPublic()
		{
			this.RefreshSelectedScrollList();
		}

		// Token: 0x0600B052 RID: 45138 RVA: 0x005054E8 File Offset: 0x005036E8
		public void Init([TupleElementNames(new string[]
		{
			"data",
			"count"
		})] Action<List<ValueTuple<ItemDisplayData, int>>> onContentChange = null)
		{
			bool hasInit = this.HasInit;
			if (!hasInit)
			{
				this.HasInit = true;
				this._onContentChange = onContentChange;
				bool flag = this.itemScroll == null;
				if (!flag)
				{
					bool flag2 = this.itemScroll.BtnMultiplySelect != null;
					if (flag2)
					{
						this.itemScroll.BtnMultiplySelect.gameObject.SetActive(false);
					}
					bool flag3 = this.itemScroll.ToggleMultiplyLock != null;
					if (flag3)
					{
						this.itemScroll.ToggleMultiplyLock.gameObject.SetActive(false);
					}
					bool flag4 = this.selectedScroll == null && this.itemScroll.SwitchSelection != null;
					if (flag4)
					{
						this.itemScroll.SwitchSelection.gameObject.SetActive(false);
						bool flag5 = this.itemScroll.SwitchSelection.transform.parent != null && this.itemScroll.SwitchSelection.transform.parent.parent != null;
						if (flag5)
						{
							this.itemScroll.SwitchSelection.transform.parent.parent.gameObject.SetActive(false);
						}
					}
					else
					{
						bool flag6 = this.selectedScroll != null && this.itemScroll.SwitchSelection != null;
						if (flag6)
						{
							this._defaultItemScrollHeightFull = ((this.itemScrollHeightFull > 0f) ? this.itemScrollHeightFull : this.itemScroll.RectTransform.sizeDelta.y);
							bool flag7 = this.itemScrollHeightFull <= 0f;
							if (flag7)
							{
								this.itemScrollHeightFull = this._defaultItemScrollHeightFull;
							}
							this.itemScroll.SwitchSelection.onValueChanged.RemoveAllListeners();
							this.itemScroll.SwitchSelection.onValueChanged.AddListener(new UnityAction<bool>(this.OnSwitchSelectionChanged));
							this.itemScroll.SwitchSelection.SetIsOnWithoutNotify(false);
							this.selectedScroll.gameObject.SetActive(false);
							GameObject gameObject = this.selectedListTitle;
							if (gameObject != null)
							{
								gameObject.SetActive(false);
							}
							this.ApplyItemScrollHeight(false);
							this.ApplySelectedScrollSortFilterVisibility();
						}
					}
					CToggle toggleSelectAll = this.itemScroll.ToggleSelectAll;
					bool flag8 = toggleSelectAll != null;
					if (flag8)
					{
						toggleSelectAll.gameObject.SetActive(false);
						toggleSelectAll.onValueChanged.RemoveAllListeners();
						toggleSelectAll.onValueChanged.AddListener(new UnityAction<bool>(this.ApplySelectAllInFilteredList));
						toggleSelectAll.isOn = false;
					}
				}
			}
		}

		// Token: 0x0600B053 RID: 45139 RVA: 0x00505789 File Offset: 0x00503989
		public void SetItems(List<ItemDisplayData> items)
		{
			this._sourceItems = (items ?? new List<ItemDisplayData>());
			this.RebuildSourceItemLookup();
			this.RefreshItems();
		}

		// Token: 0x0600B054 RID: 45140 RVA: 0x005057AC File Offset: 0x005039AC
		public void SetFilter(Func<ItemDisplayData, bool> filter)
		{
			this._filter = filter;
			bool hasInit = this.HasInit;
			if (hasInit)
			{
				this.RefreshItems();
			}
		}

		// Token: 0x0600B055 RID: 45141 RVA: 0x005057D4 File Offset: 0x005039D4
		public void SetSelectedListExpanded(bool expanded)
		{
			bool flag = this.selectedScroll == null || this.SwitchSelection == null || !this.IsMultiItemSelect;
			if (!flag)
			{
				bool flag2 = this.SwitchSelection.isOn == expanded;
				if (flag2)
				{
					this.ApplySelectedListLayout(expanded);
					this.RefreshItems();
				}
				else
				{
					this.SwitchSelection.SetIsOnWithoutNotify(expanded);
					this.ApplySelectedListLayout(expanded);
					this.RefreshItems();
				}
			}
		}

		// Token: 0x0600B056 RID: 45142 RVA: 0x00505850 File Offset: 0x00503A50
		public void EnterMultiplyMode(bool showSelectedListOnEnter = true)
		{
			bool isMultiItemSelect = this.IsMultiItemSelect;
			if (isMultiItemSelect)
			{
				this.ShowSwitchSelectionChrome(true);
				bool flag = !showSelectedListOnEnter;
				if (flag)
				{
					this.SetSelectedListVisible(false);
				}
			}
			else
			{
				this.IsMultiItemSelect = true;
				this.ClearMultiplySelection();
				this.ShowSwitchSelectionChrome(true);
				bool flag2 = !showSelectedListOnEnter;
				if (flag2)
				{
					this.ApplySelectedListLayout(false);
				}
				this.RefreshItems();
			}
		}

		// Token: 0x0600B057 RID: 45143 RVA: 0x005058B1 File Offset: 0x00503AB1
		public void ShowSwitchSelectionChromeVisible(bool visible)
		{
			this.ShowSwitchSelectionChrome(visible);
		}

		// Token: 0x0600B058 RID: 45144 RVA: 0x005058BC File Offset: 0x00503ABC
		public void SetSelectedListVisible(bool visible)
		{
			bool flag = !this.IsMultiItemSelect || this.selectedScroll == null;
			if (!flag)
			{
				bool flag2 = !visible;
				if (flag2)
				{
					CToggle switchSelection = this.SwitchSelection;
					if (switchSelection != null)
					{
						switchSelection.SetIsOnWithoutNotify(false);
					}
					this.ApplySelectedListLayout(false);
					this.RefreshItems();
				}
				else
				{
					this.SetSelectedListExpanded(true);
				}
			}
		}

		// Token: 0x0600B059 RID: 45145 RVA: 0x0050591C File Offset: 0x00503B1C
		public void ExitMultiplyMode()
		{
			bool flag = !this.IsMultiItemSelect;
			if (!flag)
			{
				this.IsMultiItemSelect = false;
				this.ClearMultiplySelection();
				this.ShowSwitchSelectionChrome(false);
				this.ApplyItemScrollHeight(false);
				this.RefreshItems();
			}
		}

		// Token: 0x0600B05A RID: 45146 RVA: 0x00505960 File Offset: 0x00503B60
		public void TryExitMultiplyMode(Action onConfirm)
		{
			bool flag = !this.IsMultiItemSelect;
			if (flag)
			{
				Action onConfirm2 = onConfirm;
				if (onConfirm2 != null)
				{
					onConfirm2();
				}
			}
			else
			{
				bool flag2 = this.SelectedMultiplyItemDict.Count == 0;
				if (flag2)
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
					DialogCmd dialogCmd = new DialogCmd
					{
						Type = 1,
						Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention),
						Content = LocalStringManager.GetFormat(LanguageKey.LK_Item_Exit_Multiply, Array.Empty<object>()),
						Yes = delegate()
						{
							this.ExitMultiplyMode();
							Action onConfirm4 = onConfirm;
							if (onConfirm4 != null)
							{
								onConfirm4();
							}
						}
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
				}
			}
		}

		// Token: 0x0600B05B RID: 45147 RVA: 0x00505A44 File Offset: 0x00503C44
		public void RefreshItems()
		{
			bool flag = !this.HasInit || this.itemScroll == null;
			if (!flag)
			{
				this._displayItems.Clear();
				bool isMultiItemSelect = this.IsMultiItemSelect;
				if (isMultiItemSelect)
				{
					foreach (ItemDisplayData d in this._sourceItems)
					{
						bool flag2 = this._filter != null && !this._filter(d);
						if (!flag2)
						{
							d.Interactable = true;
							this._displayItems.Add(d);
						}
					}
				}
				else
				{
					foreach (ItemDisplayData d2 in this._sourceItems)
					{
						d2.Interactable = (this._filter == null || this._filter(d2));
						this._displayItems.Add(d2);
					}
				}
				this.itemScroll.SetItemList(this._displayItems);
				this.RefreshSelectedScrollList();
			}
		}

		// Token: 0x0600B05C RID: 45148 RVA: 0x00505B90 File Offset: 0x00503D90
		private void OnSwitchSelectionChanged(bool isOn)
		{
			this.ApplySelectedListLayout(isOn);
			this.RefreshItems();
		}

		// Token: 0x0600B05D RID: 45149 RVA: 0x00505BA4 File Offset: 0x00503DA4
		private void RefreshSelectedScrollList()
		{
			bool flag = this.selectedScroll == null;
			if (!flag)
			{
				bool flag2;
				if (this.IsMultiItemSelect)
				{
					CToggle switchSelection = this.SwitchSelection;
					flag2 = (switchSelection != null && switchSelection.isOn);
				}
				else
				{
					flag2 = false;
				}
				bool showSelected = flag2;
				this._selectedItems.Clear();
				bool flag3 = showSelected;
				if (flag3)
				{
					foreach (KeyValuePair<ItemDisplayData, int> keyValuePair in this.SelectedMultiplyItemDict)
					{
						ItemDisplayData itemDisplayData;
						int num;
						keyValuePair.Deconstruct(out itemDisplayData, out num);
						ItemDisplayData data = itemDisplayData;
						int count = num;
						this._selectedItems.Add(data.Clone(count));
					}
				}
				this.selectedScroll.SetItemList(this._selectedItems);
				this.ApplySelectedListLayout(showSelected);
			}
		}

		// Token: 0x0600B05E RID: 45150 RVA: 0x00505C80 File Offset: 0x00503E80
		private void ApplySelectedListLayout(bool showSelectedList)
		{
			bool show = showSelectedList && this.selectedScroll != null && this.IsMultiItemSelect;
			GameObject gameObject = this.selectedListTitle;
			if (gameObject != null)
			{
				gameObject.SetActive(show);
			}
			bool flag = this.selectedScroll != null;
			if (flag)
			{
				this.selectedScroll.gameObject.SetActive(show);
			}
			this.ApplyItemScrollHeight(show);
			this.ApplySelectedScrollSortFilterVisibility();
		}

		// Token: 0x0600B05F RID: 45151 RVA: 0x00505CF0 File Offset: 0x00503EF0
		private void ApplySelectedScrollSortFilterVisibility()
		{
			bool flag = this.selectedScrollSortAndFilterRoot == null;
			if (!flag)
			{
				bool flag2;
				if (this.IsMultiItemSelect)
				{
					CToggle switchSelection = this.SwitchSelection;
					if (switchSelection != null && switchSelection.isOn && this.selectedScroll != null)
					{
						flag2 = this.selectedScroll.gameObject.activeSelf;
						goto IL_4E;
					}
				}
				flag2 = false;
				IL_4E:
				bool showSelectedPanel = flag2;
				this.selectedScrollSortAndFilterRoot.SetActive(showSelectedPanel && this.showSelectedScrollSortAndFilter);
			}
		}

		// Token: 0x0600B060 RID: 45152 RVA: 0x00505D64 File Offset: 0x00503F64
		private void ApplyItemScrollHeight(bool useCompactHeight)
		{
			bool flag = this.itemScroll == null || this.selectedScroll == null;
			if (!flag)
			{
				float fullHeight = (this.itemScrollHeightFull > 0f) ? this.itemScrollHeightFull : this._defaultItemScrollHeightFull;
				float height = (useCompactHeight && this.itemScrollHeightWithSelectedVisible > 0f) ? this.itemScrollHeightWithSelectedVisible : fullHeight;
				bool flag2 = height <= 0f;
				if (!flag2)
				{
					Vector2 size = this.itemScroll.RectTransform.sizeDelta;
					this.itemScroll.RectTransform.sizeDelta = size.SetY(height);
				}
			}
		}

		// Token: 0x0600B061 RID: 45153 RVA: 0x00505E08 File Offset: 0x00504008
		private void ShowSwitchSelectionChrome(bool show)
		{
			bool flag = this.selectedScroll == null || this.SwitchSelection == null;
			if (!flag)
			{
				this.SwitchSelection.gameObject.SetActive(show);
				Transform parent = this.SwitchSelection.transform.parent;
				bool flag2 = parent != null && parent.parent != null;
				if (flag2)
				{
					parent.parent.gameObject.SetActive(show);
				}
				bool flag3 = !show;
				if (flag3)
				{
					this.SwitchSelection.SetIsOnWithoutNotify(false);
					this.ApplySelectedListLayout(false);
				}
			}
		}

		// Token: 0x0600B062 RID: 45154 RVA: 0x00505EAC File Offset: 0x005040AC
		public void OnRenderItemMultiply(ITradeableContent tempData, RowItemLine view)
		{
			ItemDisplayData displayTemp;
			bool flag;
			if (this.IsMultiItemSelect)
			{
				displayTemp = (tempData as ItemDisplayData);
				flag = (displayTemp == null);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (!flag2)
			{
				ItemDisplayData itemData = this.ResolveSourceItem(displayTemp);
				bool flag3 = itemData == null;
				if (!flag3)
				{
					int selectedCount;
					this.SelectedMultiplyItemDict.TryGetValue(itemData, out selectedCount);
					bool isSelected = selectedCount > 0;
					bool isInSelectedList = this._selectedItems.Contains(displayTemp);
					view.SetSelected(isSelected && !isInSelectedList);
					view.RowItemMain.HideInteractionState();
					bool flag4 = isSelected;
					if (flag4)
					{
						view.SetInteractable(true, true);
					}
					else
					{
						view.SetInteractable(this.CanSelectItem(itemData), true);
					}
					view.SetDisabled(!view.Interactable);
				}
			}
		}

		// Token: 0x0600B063 RID: 45155 RVA: 0x00505F64 File Offset: 0x00504164
		public void HandleRowClick(ItemDisplayData tempData, RowItemLine view)
		{
			SimpleMultiplyItemListScroll.<>c__DisplayClass66_0 CS$<>8__locals1 = new SimpleMultiplyItemListScroll.<>c__DisplayClass66_0();
			CS$<>8__locals1.<>4__this = this;
			bool flag = !this.IsMultiItemSelect || tempData == null || view == null || !view.Interactable;
			if (!flag)
			{
				CS$<>8__locals1.itemData = this.ResolveSourceItem(tempData);
				bool flag2 = CS$<>8__locals1.itemData == null;
				if (!flag2)
				{
					this.SelectedMultiplyItemDict.TryGetValue(CS$<>8__locals1.itemData, out CS$<>8__locals1.selectedCount);
					bool isSelected = CS$<>8__locals1.selectedCount > 0;
					bool isInSelectedList = this._selectedItems.Contains(tempData);
					SimpleMultiplyItemListScroll.<>c__DisplayClass66_0 CS$<>8__locals2 = CS$<>8__locals1;
					CToggle switchSelection = this.SwitchSelection;
					CS$<>8__locals2.scroll = ((switchSelection != null && switchSelection.isOn && isSelected && isInSelectedList && this.selectedScroll != null) ? this.selectedScroll : this.itemScroll);
					bool flag3 = isSelected;
					if (flag3)
					{
						this.SetItemSelectCount(CS$<>8__locals1.itemData.RealKey, 0, true);
					}
					else
					{
						CS$<>8__locals1.limitCount = this.GetSelectLimit(CS$<>8__locals1.itemData);
						bool flag4 = CS$<>8__locals1.limitCount <= 0;
						if (!flag4)
						{
							CS$<>8__locals1.scroll.HandleClickItem(CS$<>8__locals1.itemData, view, delegate(RowItemLine row)
							{
								CS$<>8__locals1.<>4__this.OnClickToSelect(row, CS$<>8__locals1.itemData, CS$<>8__locals1.selectedCount, CS$<>8__locals1.limitCount, CS$<>8__locals1.scroll);
							});
						}
					}
				}
			}
		}

		// Token: 0x0600B064 RID: 45156 RVA: 0x0050609C File Offset: 0x0050429C
		public void ApplySelectAllInFilteredList(bool select)
		{
			bool flag = !this.IsMultiItemSelect;
			if (!flag)
			{
				this.SetSelectedListExpanded(false);
				List<ValueTuple<ItemDisplayData, int>> changeList = null;
				if (select)
				{
					changeList = new List<ValueTuple<ItemDisplayData, int>>();
					this.EnsureSourceItemLookup();
					this.RefreshTotalSelectedCount();
					int runningSelected = this.TotalSelectedCount;
					IReadOnlyList<ITradeableContent> filtered = this.itemScroll.FilteredData;
					for (int i = 0; i < filtered.Count; i++)
					{
						ItemDisplayData tempData = filtered[i] as ItemDisplayData;
						bool flag2 = tempData == null;
						if (!flag2)
						{
							ItemDisplayData data = this.ResolveSourceItem(tempData);
							bool flag3 = data == null || !this.CanSelectItem(data);
							if (!flag3)
							{
								bool flag4 = this.MaxSelectCount > 0 && runningSelected >= this.MaxSelectCount;
								if (flag4)
								{
									break;
								}
								int limit = this.GetSelectLimitForBatch(data, runningSelected);
								bool flag5 = limit <= 0;
								if (!flag5)
								{
									int selectedCount = Mathf.Min(data.Amount, limit);
									bool flag6 = this.SetItemSelectCount(data.RealKey, selectedCount, false);
									if (flag6)
									{
										runningSelected += selectedCount;
										changeList.Add(new ValueTuple<ItemDisplayData, int>(data, selectedCount));
									}
								}
							}
						}
					}
				}
				else
				{
					foreach (ItemDisplayData itemData in this.SelectedMultiplyItemDict.Keys.ToList<ItemDisplayData>())
					{
						this.SetItemSelectCount(itemData.RealKey, 0, false);
					}
				}
				this.FinishSelectionBatch(changeList, true, false);
			}
		}

		// Token: 0x0600B065 RID: 45157 RVA: 0x00506248 File Offset: 0x00504448
		public void CollectFilteredSourceItems(List<ItemDisplayData> buffer)
		{
			buffer.Clear();
			ItemListScroll itemListScroll = this.itemScroll;
			IReadOnlyList<ITradeableContent> filtered = (itemListScroll != null) ? itemListScroll.FilteredData : null;
			bool flag = filtered == null || filtered.Count == 0;
			if (!flag)
			{
				this.EnsureSourceItemLookup();
				for (int i = 0; i < filtered.Count; i++)
				{
					ItemDisplayData tempData = filtered[i] as ItemDisplayData;
					bool flag2 = tempData == null;
					if (!flag2)
					{
						ItemDisplayData source = this.ResolveSourceItem(tempData);
						bool flag3 = source != null;
						if (flag3)
						{
							buffer.Add(source);
						}
					}
				}
			}
		}

		// Token: 0x0600B066 RID: 45158 RVA: 0x005062DE File Offset: 0x005044DE
		public void ClearMultiplySelection()
		{
			this.SetSelectedListExpanded(false);
			this._selection.Clear();
			this.RefreshTotalSelectedCount();
			ItemListScroll itemListScroll = this.itemScroll;
			if (itemListScroll != null)
			{
				itemListScroll.ReRender();
			}
			this.NotifyContentChange(null);
		}

		// Token: 0x0600B067 RID: 45159 RVA: 0x00506318 File Offset: 0x00504518
		public void RefreshSelectAllToggleState()
		{
			CToggle ta = this.ToggleSelectAll;
			bool flag = ta == null || !ta.gameObject.activeInHierarchy;
			if (!flag)
			{
				IReadOnlyList<ITradeableContent> filtered = this.itemScroll.FilteredData;
				bool canSelectAll = filtered.Count > 0 && filtered.Any(delegate(ITradeableContent d)
				{
					ItemDisplayData id2 = d as ItemDisplayData;
					return id2 != null && this.CanSelectItem(id2);
				});
				bool isAll = false;
				bool flag2 = this.SelectedMultiplyItemOrderedList.Count > 0 && canSelectAll;
				if (flag2)
				{
					HashSet<ItemKey> selectedKeys = new HashSet<ItemKey>();
					foreach (ItemDisplayData selected in this.SelectedMultiplyItemOrderedList)
					{
						selectedKeys.Add(selected.RealKey);
					}
					isAll = true;
					for (int i = 0; i < filtered.Count; i++)
					{
						ItemDisplayData id = filtered[i] as ItemDisplayData;
						bool flag3 = id == null || !this.CanSelectItem(id);
						if (!flag3)
						{
							bool flag4 = selectedKeys.Contains(id.RealKey);
							if (!flag4)
							{
								isAll = false;
								break;
							}
						}
					}
				}
				ta.SetIsOnWithoutNotify(isAll);
				ta.interactable = canSelectAll;
			}
		}

		// Token: 0x0600B068 RID: 45160 RVA: 0x00506464 File Offset: 0x00504664
		public void OnExitMultiplyOperation(ArgumentBox _)
		{
			this.ExitMultiplyMode();
		}

		// Token: 0x0600B069 RID: 45161 RVA: 0x00506470 File Offset: 0x00504670
		private void OnClickToSelect(RowItemLine row, ItemDisplayData itemData, int selectedCount, int limitCount, ItemListScroll scroll)
		{
			bool flag = itemData.Amount == 1;
			if (flag)
			{
				this.SetItemSelectCount(itemData.RealKey, (selectedCount == 1) ? 0 : 1, true);
			}
			else
			{
				Func<ItemDisplayData, string> getSelectCountLimitTip = this.GetSelectCountLimitTip;
				string limitTip = ((getSelectCountLimitTip != null) ? getSelectCountLimitTip(itemData) : null) ?? string.Empty;
				scroll.SetItemToSelectCountMode(row, delegate(int count)
				{
					this.SetItemSelectCount(itemData.RealKey, count, true);
				}, delegate
				{
					this.SetItemSelectCount(itemData.RealKey, 0, true);
				}, selectedCount, limitCount, 1, limitTip, false, null, false);
			}
		}

		// Token: 0x0600B06A RID: 45162 RVA: 0x00506510 File Offset: 0x00504710
		private bool CanSelectItem(ItemDisplayData itemData)
		{
			bool isLocked = itemData.IsLocked;
			bool result;
			if (isLocked)
			{
				result = false;
			}
			else
			{
				bool flag = this._filter != null && !this._filter(itemData);
				if (flag)
				{
					result = false;
				}
				else
				{
					bool flag2 = this.CanSelectItemPredicate != null && !this.CanSelectItemPredicate(itemData);
					result = (!flag2 && (this.MaxSelectCount <= 0 || this.TotalSelectedCount < this.MaxSelectCount));
				}
			}
			return result;
		}

		// Token: 0x0600B06B RID: 45163 RVA: 0x00506590 File Offset: 0x00504790
		private int GetSelectLimit(ItemDisplayData itemData)
		{
			int limit = itemData.Amount;
			bool flag = this.MaxSelectCount > 0;
			if (flag)
			{
				limit = Mathf.Min(limit, this.MaxSelectCount - this.TotalSelectedCount);
			}
			bool flag2 = this.GetSelectLimitOverride != null;
			if (flag2)
			{
				limit = this.GetSelectLimitOverride(itemData, limit);
			}
			return limit;
		}

		// Token: 0x0600B06C RID: 45164 RVA: 0x005065E8 File Offset: 0x005047E8
		private bool SetItemSelectCount(ItemKey itemKey, int count, bool notify = true)
		{
			ItemDisplayData data = this.ResolveSourceItem(itemKey);
			bool flag = data == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int lastCount;
				bool flag2 = this.SelectedMultiplyItemDict.TryGetValue(data, out lastCount) && count == lastCount;
				if (flag2)
				{
					result = false;
				}
				else
				{
					int index = this.SelectedMultiplyItemOrderedList.IndexOf(data);
					bool flag3 = this.SelectedMultiplyItemOrderedList.CheckIndex(index);
					if (flag3)
					{
						this.SelectedMultiplyItemOrderedList[index] = data;
						this.SelectedMultiplyItemDict[data] = count;
						bool flag4 = count <= 0;
						if (flag4)
						{
							this.SelectedMultiplyItemDict.Remove(data);
							this.SelectedMultiplyItemOrderedList.Remove(data);
						}
					}
					else
					{
						bool flag5 = count <= 0;
						if (flag5)
						{
							return false;
						}
						this.SelectedMultiplyItemOrderedList.Add(data);
						this.SelectedMultiplyItemDict.Add(data, count);
					}
					bool flag6 = !notify;
					if (flag6)
					{
						result = true;
					}
					else
					{
						this.FinishSelectionBatch(new List<ValueTuple<ItemDisplayData, int>>
						{
							new ValueTuple<ItemDisplayData, int>(data, count)
						}, true, this.IsSelectedListExpanded);
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x0600B06D RID: 45165 RVA: 0x00506704 File Offset: 0x00504904
		private void FinishSelectionBatch([TupleElementNames(new string[]
		{
			"data",
			"count"
		})] List<ValueTuple<ItemDisplayData, int>> changeList = null, bool selectionOnly = false, bool refreshSelectedList = true)
		{
			this.RefreshTotalSelectedCount();
			if (selectionOnly)
			{
				bool flag = refreshSelectedList && this.IsSelectedListExpanded;
				if (flag)
				{
					this.RefreshSelectedScrollList();
				}
			}
			else
			{
				this.RefreshItems();
			}
			this.itemScroll.ReRender();
			bool flag2 = refreshSelectedList && this.IsSelectedListExpanded;
			if (flag2)
			{
				ItemListScroll itemListScroll = this.selectedScroll;
				if (itemListScroll != null)
				{
					itemListScroll.ReRender();
				}
			}
			this.RefreshSelectAllToggleState();
			this.NotifyContentChange(changeList);
		}

		// Token: 0x0600B06E RID: 45166 RVA: 0x00506780 File Offset: 0x00504980
		private void RebuildSourceItemLookup()
		{
			bool flag = this._sourceItems == null || this._sourceItems.Count == 0;
			if (flag)
			{
				Dictionary<ItemKey, ItemDisplayData> sourceItemByKey = this._sourceItemByKey;
				if (sourceItemByKey != null)
				{
					sourceItemByKey.Clear();
				}
			}
			else
			{
				if (this._sourceItemByKey == null)
				{
					this._sourceItemByKey = new Dictionary<ItemKey, ItemDisplayData>(this._sourceItems.Count);
				}
				this._sourceItemByKey.Clear();
				foreach (ItemDisplayData item in this._sourceItems)
				{
					bool flag2 = item != null;
					if (flag2)
					{
						this._sourceItemByKey[item.RealKey] = item;
					}
				}
			}
		}

		// Token: 0x0600B06F RID: 45167 RVA: 0x00506848 File Offset: 0x00504A48
		private void EnsureSourceItemLookup()
		{
			bool flag = this._sourceItemByKey == null || this._sourceItemByKey.Count != this._sourceItems.Count;
			if (flag)
			{
				this.RebuildSourceItemLookup();
			}
		}

		// Token: 0x0600B070 RID: 45168 RVA: 0x00506888 File Offset: 0x00504A88
		private ItemDisplayData ResolveSourceItem(ItemDisplayData data)
		{
			bool flag = data == null;
			ItemDisplayData result;
			if (flag)
			{
				result = null;
			}
			else
			{
				ItemDisplayData source;
				bool flag2 = this._sourceItemByKey != null && this._sourceItemByKey.TryGetValue(data.RealKey, out source);
				if (flag2)
				{
					result = source;
				}
				else
				{
					result = data;
				}
			}
			return result;
		}

		// Token: 0x0600B071 RID: 45169 RVA: 0x005068D0 File Offset: 0x00504AD0
		private ItemDisplayData ResolveSourceItem(ItemKey itemKey)
		{
			ItemDisplayData source;
			bool flag = this._sourceItemByKey != null && this._sourceItemByKey.TryGetValue(itemKey, out source);
			ItemDisplayData result;
			if (flag)
			{
				result = source;
			}
			else
			{
				result = this._sourceItems.Find((ItemDisplayData d) => d.RealKey.Equals(itemKey));
			}
			return result;
		}

		// Token: 0x0600B072 RID: 45170 RVA: 0x0050692C File Offset: 0x00504B2C
		private int GetSelectLimitForBatch(ItemDisplayData itemData, int runningSelected)
		{
			int limit = itemData.Amount;
			bool flag = this.MaxSelectCount > 0;
			if (flag)
			{
				limit = Mathf.Min(limit, this.MaxSelectCount - runningSelected);
			}
			bool flag2 = this.GetSelectLimitOverride != null;
			if (flag2)
			{
				limit = this.GetSelectLimitOverride(itemData, limit);
			}
			return limit;
		}

		// Token: 0x0600B073 RID: 45171 RVA: 0x0050697D File Offset: 0x00504B7D
		private void RefreshTotalSelectedCount()
		{
			this.TotalSelectedCount = this.SelectedMultiplyItemDict.Values.Sum();
		}

		// Token: 0x0600B074 RID: 45172 RVA: 0x00506997 File Offset: 0x00504B97
		public void SyncTotalSelectedCount()
		{
			this.RefreshTotalSelectedCount();
		}

		// Token: 0x0600B075 RID: 45173 RVA: 0x005069A0 File Offset: 0x00504BA0
		private void NotifyContentChange([TupleElementNames(new string[]
		{
			"data",
			"count"
		})] List<ValueTuple<ItemDisplayData, int>> changeList)
		{
			Action<List<ValueTuple<ItemDisplayData, int>>> onContentChange = this._onContentChange;
			if (onContentChange != null)
			{
				onContentChange(changeList);
			}
		}

		// Token: 0x04008855 RID: 34901
		[SerializeField]
		private ItemListScroll itemScroll;

		// Token: 0x04008856 RID: 34902
		[SerializeField]
		private ItemListScroll selectedScroll;

		// Token: 0x04008857 RID: 34903
		[Header("标题（随已选列表显隐）")]
		[SerializeField]
		private GameObject selectedListTitle;

		// Token: 0x04008858 RID: 34904
		[Header("已选列表")]
		[Tooltip("为 true 且已选列表展开时，显示 selectedScrollSortAndFilterRoot")]
		[SerializeField]
		private bool showSelectedScrollSortAndFilter;

		// Token: 0x04008859 RID: 34905
		[Tooltip("已选列表的排序筛选区域（直接拖预制体节点，勿通过 ItemListScroll 访问）")]
		[SerializeField]
		private GameObject selectedScrollSortAndFilterRoot;

		// Token: 0x0400885A RID: 34906
		[Header("主列表高度")]
		[Tooltip("未展开「显示已选」时主列表高度；为 0 则在 Init 时取当前 RectTransform 高度")]
		[SerializeField]
		private float itemScrollHeightFull;

		// Token: 0x0400885B RID: 34907
		[Tooltip("展开「显示已选」时主列表高度（需配置 selectedScroll）")]
		[SerializeField]
		private float itemScrollHeightWithSelectedVisible;

		// Token: 0x0400885C RID: 34908
		private float _defaultItemScrollHeightFull;

		// Token: 0x0400885D RID: 34909
		private readonly MultiplyOperationHandler _selection = new MultiplyOperationHandler();

		// Token: 0x0400885E RID: 34910
		private List<ItemDisplayData> _sourceItems = new List<ItemDisplayData>();

		// Token: 0x0400885F RID: 34911
		private readonly List<ItemDisplayData> _displayItems = new List<ItemDisplayData>();

		// Token: 0x04008860 RID: 34912
		private readonly List<ItemDisplayData> _selectedItems = new List<ItemDisplayData>();

		// Token: 0x04008861 RID: 34913
		private Func<ItemDisplayData, bool> _filter;

		// Token: 0x04008862 RID: 34914
		[TupleElementNames(new string[]
		{
			"data",
			"count"
		})]
		private Action<List<ValueTuple<ItemDisplayData, int>>> _onContentChange;

		// Token: 0x04008863 RID: 34915
		private Dictionary<ItemKey, ItemDisplayData> _sourceItemByKey;

		// Token: 0x04008864 RID: 34916
		public Func<ItemDisplayData, bool> CanSelectItemPredicate;

		// Token: 0x04008865 RID: 34917
		public Func<ItemDisplayData, int, int> GetSelectLimitOverride;

		// Token: 0x04008866 RID: 34918
		public Func<ItemDisplayData, string> GetSelectCountLimitTip;
	}
}
