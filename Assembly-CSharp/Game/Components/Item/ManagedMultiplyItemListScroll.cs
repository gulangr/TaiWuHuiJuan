using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item.Apply;
using GameData.Domains.Item.Display;
using GameDataExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Components.Item
{
	// Token: 0x02000EF3 RID: 3827
	[RequireComponent(typeof(SimpleMultiplyItemListScroll))]
	public class ManagedMultiplyItemListScroll : MonoBehaviour
	{
		// Token: 0x170013D6 RID: 5078
		// (get) Token: 0x0600AF79 RID: 44921 RVA: 0x004FF80B File Offset: 0x004FDA0B
		public bool HasInit
		{
			get
			{
				return this.core != null && this.core.HasInit;
			}
		}

		// Token: 0x170013D7 RID: 5079
		// (get) Token: 0x0600AF7A RID: 44922 RVA: 0x004FF829 File Offset: 0x004FDA29
		public bool IsItemListScrollConfigured
		{
			get
			{
				return this._itemListScrollConfigured;
			}
		}

		// Token: 0x170013D8 RID: 5080
		// (get) Token: 0x0600AF7B RID: 44923 RVA: 0x004FF831 File Offset: 0x004FDA31
		public string SearchText
		{
			get
			{
				return this._searchText;
			}
		}

		// Token: 0x170013D9 RID: 5081
		// (get) Token: 0x0600AF7C RID: 44924 RVA: 0x004FF839 File Offset: 0x004FDA39
		public bool IsMultiItemSelect
		{
			get
			{
				return this.core != null && this.core.IsMultiItemSelect;
			}
		}

		// Token: 0x170013DA RID: 5082
		// (get) Token: 0x0600AF7D RID: 44925 RVA: 0x004FF857 File Offset: 0x004FDA57
		public ItemListScroll CurMultiplyScrollView
		{
			get
			{
				SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
				return (simpleMultiplyItemListScroll != null) ? simpleMultiplyItemListScroll.CurMultiplyScrollView : null;
			}
		}

		// Token: 0x170013DB RID: 5083
		// (get) Token: 0x0600AF7E RID: 44926 RVA: 0x004FF86B File Offset: 0x004FDA6B
		public ItemListScroll SelectedScrollView
		{
			get
			{
				SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
				return (simpleMultiplyItemListScroll != null) ? simpleMultiplyItemListScroll.SelectedScrollView : null;
			}
		}

		// Token: 0x170013DC RID: 5084
		// (get) Token: 0x0600AF7F RID: 44927 RVA: 0x004FF87F File Offset: 0x004FDA7F
		public CToggle SwitchSelection
		{
			get
			{
				return this.toggleSwitchSelection;
			}
		}

		// Token: 0x170013DD RID: 5085
		// (get) Token: 0x0600AF80 RID: 44928 RVA: 0x004FF887 File Offset: 0x004FDA87
		public Dictionary<ItemDisplayData, int> SelectedMultiplyItemDict
		{
			get
			{
				SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
				return (simpleMultiplyItemListScroll != null) ? simpleMultiplyItemListScroll.SelectedMultiplyItemDict : null;
			}
		}

		// Token: 0x170013DE RID: 5086
		// (get) Token: 0x0600AF81 RID: 44929 RVA: 0x004FF89B File Offset: 0x004FDA9B
		public List<ItemDisplayData> SelectedMultiplyItemOrderedList
		{
			get
			{
				SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
				return (simpleMultiplyItemListScroll != null) ? simpleMultiplyItemListScroll.SelectedMultiplyItemOrderedList : null;
			}
		}

		// Token: 0x170013DF RID: 5087
		// (get) Token: 0x0600AF82 RID: 44930 RVA: 0x004FF8AF File Offset: 0x004FDAAF
		// (set) Token: 0x0600AF83 RID: 44931 RVA: 0x004FF8C4 File Offset: 0x004FDAC4
		public Func<ItemDisplayData, bool> CanSelectItemPredicate
		{
			get
			{
				SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
				return (simpleMultiplyItemListScroll != null) ? simpleMultiplyItemListScroll.CanSelectItemPredicate : null;
			}
			set
			{
				bool flag = this.core != null;
				if (flag)
				{
					this.core.CanSelectItemPredicate = value;
				}
			}
		}

		// Token: 0x170013E0 RID: 5088
		// (get) Token: 0x0600AF84 RID: 44932 RVA: 0x004FF8EE File Offset: 0x004FDAEE
		// (set) Token: 0x0600AF85 RID: 44933 RVA: 0x004FF904 File Offset: 0x004FDB04
		public Func<ItemDisplayData, int, int> GetSelectLimitOverride
		{
			get
			{
				SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
				return (simpleMultiplyItemListScroll != null) ? simpleMultiplyItemListScroll.GetSelectLimitOverride : null;
			}
			set
			{
				bool flag = this.core != null;
				if (flag)
				{
					this.core.GetSelectLimitOverride = value;
				}
			}
		}

		// Token: 0x170013E1 RID: 5089
		// (get) Token: 0x0600AF86 RID: 44934 RVA: 0x004FF92E File Offset: 0x004FDB2E
		// (set) Token: 0x0600AF87 RID: 44935 RVA: 0x004FF944 File Offset: 0x004FDB44
		public Func<ItemDisplayData, string> GetSelectCountLimitTip
		{
			get
			{
				SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
				return (simpleMultiplyItemListScroll != null) ? simpleMultiplyItemListScroll.GetSelectCountLimitTip : null;
			}
			set
			{
				bool flag = this.core != null;
				if (flag)
				{
					this.core.GetSelectCountLimitTip = value;
				}
			}
		}

		// Token: 0x170013E2 RID: 5090
		// (get) Token: 0x0600AF88 RID: 44936 RVA: 0x004FF96E File Offset: 0x004FDB6E
		// (set) Token: 0x0600AF89 RID: 44937 RVA: 0x004FF984 File Offset: 0x004FDB84
		public int MaxSelectCount
		{
			get
			{
				SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
				return (simpleMultiplyItemListScroll != null) ? simpleMultiplyItemListScroll.MaxSelectCount : 0;
			}
			set
			{
				bool flag = this.core != null;
				if (flag)
				{
					this.core.MaxSelectCount = value;
				}
			}
		}

		// Token: 0x170013E3 RID: 5091
		// (get) Token: 0x0600AF8A RID: 44938 RVA: 0x004FF9AF File Offset: 0x004FDBAF
		public int TotalSelectedCount
		{
			get
			{
				SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
				return (simpleMultiplyItemListScroll != null) ? simpleMultiplyItemListScroll.TotalSelectedCount : 0;
			}
		}

		// Token: 0x170013E4 RID: 5092
		// (get) Token: 0x0600AF8B RID: 44939 RVA: 0x004FF9C3 File Offset: 0x004FDBC3
		public bool IsSelectedListExpanded
		{
			get
			{
				return this.core != null && this.core.IsSelectedListExpanded;
			}
		}

		// Token: 0x170013E5 RID: 5093
		// (get) Token: 0x0600AF8C RID: 44940 RVA: 0x004FF9E1 File Offset: 0x004FDBE1
		public bool IsBatchSelectionOperation
		{
			get
			{
				return this._batchSelectionOperation;
			}
		}

		// Token: 0x170013E6 RID: 5094
		// (get) Token: 0x0600AF8D RID: 44941 RVA: 0x004FF9E9 File Offset: 0x004FDBE9
		// (set) Token: 0x0600AF8E RID: 44942 RVA: 0x004FF9F1 File Offset: 0x004FDBF1
		public bool AutoShowSelectedList
		{
			get
			{
				return this.autoShowSelectedList;
			}
			set
			{
				this.autoShowSelectedList = value;
			}
		}

		// Token: 0x0600AF8F RID: 44943 RVA: 0x004FF9FC File Offset: 0x004FDBFC
		private void Awake()
		{
			bool flag = this.core == null;
			if (flag)
			{
				this.core = base.GetComponent<SimpleMultiplyItemListScroll>();
			}
		}

		// Token: 0x0600AF90 RID: 44944 RVA: 0x004FFA28 File Offset: 0x004FDC28
		public void SetupItemListScroll(ManagedMultiplyItemListScrollSetup setup)
		{
			bool flag = setup == null;
			if (!flag)
			{
				this.Awake();
				ItemListScroll itemScroll = this.CurMultiplyScrollView;
				bool flag2 = itemScroll == null;
				if (!flag2)
				{
					this._amountGenerator = setup.AmountGenerator;
					this.ApplyAmountGeneratorToScrolls();
					bool itemListScrollConfigured = this._itemListScrollConfigured;
					if (!itemListScrollConfigured)
					{
						itemScroll.OnSortAndFilterChangedCallback = delegate()
						{
							this.OnMainSortAndFilterChanged();
							Action onMainSortAndFilterChanged = setup.OnMainSortAndFilterChanged;
							if (onMainSortAndFilterChanged != null)
							{
								onMainSortAndFilterChanged();
							}
						};
						itemScroll.Init(setup.MainSortSaveKey, setup.SortType, setup.EnableRowInteraction, setup.OnRender, setup.OnClick, setup.ColumnType, null, null, null);
						ItemListScroll selectedScroll = this.SelectedScrollView;
						bool flag3 = selectedScroll != null && selectedScroll != itemScroll;
						if (flag3)
						{
							string selectedKey = string.IsNullOrEmpty(setup.SelectedSortSaveKey) ? (setup.MainSortSaveKey + "_Selected") : setup.SelectedSortSaveKey;
							ESortAndFilterControllerType selectedSortType = setup.SelectedSortType ?? setup.SortType;
							selectedScroll.Init(selectedKey, selectedSortType, setup.EnableRowInteraction, setup.OnRender, setup.OnClick, setup.ColumnType, null, null, null);
						}
						this._itemListScrollConfigured = true;
					}
				}
			}
		}

		// Token: 0x0600AF91 RID: 44945 RVA: 0x004FFBCC File Offset: 0x004FDDCC
		public void Init([TupleElementNames(new string[]
		{
			"data",
			"count"
		})] Action<List<ValueTuple<ItemDisplayData, int>>> onContentChange = null)
		{
			this.Awake();
			bool flag = this.core == null;
			if (!flag)
			{
				this.ResolveSearchField();
				this.ResolveMainSortAndFilterRoot();
				this.ApplyItemListScrollChromeReferences();
				bool flag2 = !this.core.HasInit;
				if (flag2)
				{
					this.core.Init(delegate([TupleElementNames(new string[]
					{
						"data",
						"count"
					})] List<ValueTuple<ItemDisplayData, int>> list)
					{
						Action<List<ValueTuple<ItemDisplayData, int>>> onContentChange2 = onContentChange;
						if (onContentChange2 != null)
						{
							onContentChange2(list);
						}
						this.ApplyAmountGeneratorToScrolls();
						this.RefreshListAmountDisplay();
					});
				}
				this.BindOperationChrome();
				this.ApplyOperationChrome(false);
				this.RefreshSelectedCountLabel();
			}
		}

		// Token: 0x0600AF92 RID: 44946 RVA: 0x004FFC5E File Offset: 0x004FDE5E
		public void RefreshListAmountDisplay()
		{
			this.ApplyAmountGeneratorToScrolls();
			ItemListScroll curMultiplyScrollView = this.CurMultiplyScrollView;
			if (curMultiplyScrollView != null)
			{
				curMultiplyScrollView.ReRender();
			}
			ItemListScroll selectedScrollView = this.SelectedScrollView;
			if (selectedScrollView != null)
			{
				selectedScrollView.ReRender();
			}
			this.RefreshSelectedCountLabel();
		}

		// Token: 0x0600AF93 RID: 44947 RVA: 0x004FFC94 File Offset: 0x004FDE94
		public void RefreshSelectedCountLabel()
		{
			bool flag = this.selectedLabel == null;
			if (!flag)
			{
				bool flag2 = !this.IsMultiItemSelect;
				if (flag2)
				{
					this.selectedLabel.text = string.Empty;
				}
				else
				{
					SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
					if (simpleMultiplyItemListScroll != null)
					{
						simpleMultiplyItemListScroll.SyncTotalSelectedCount();
					}
					Func<int> getSelectedCountForLabel = this.GetSelectedCountForLabel;
					int total = (getSelectedCountForLabel != null) ? getSelectedCountForLabel() : this.TotalSelectedCount;
					this.selectedLabel.text = ((this.MaxSelectCount > 0) ? LanguageKey.LK_SelectItem_SelectedLabel.TrFormat(string.Format("{0}/{1}", total, this.MaxSelectCount)) : LanguageKey.LK_SelectItem_SelectedLabel.TrFormat(total));
				}
			}
		}

		// Token: 0x0600AF94 RID: 44948 RVA: 0x004FFD50 File Offset: 0x004FDF50
		private void ApplyAmountGeneratorToScrolls()
		{
			bool flag = this._amountGenerator == null;
			if (!flag)
			{
				ItemListScroll itemScroll = this.CurMultiplyScrollView;
				bool flag2 = itemScroll != null;
				if (flag2)
				{
					itemScroll.CustomAmountDataGenerator = this._amountGenerator;
				}
				ItemListScroll selectedScroll = this.SelectedScrollView;
				bool flag3 = selectedScroll != null && selectedScroll != itemScroll;
				if (flag3)
				{
					selectedScroll.CustomAmountDataGenerator = this._amountGenerator;
				}
			}
		}

		// Token: 0x0600AF95 RID: 44949 RVA: 0x004FFDB8 File Offset: 0x004FDFB8
		public void SyncSelectedListPanelVisibility()
		{
			bool flag = this.core == null || !this.IsMultiItemSelect || this.SelectedScrollView == null;
			if (!flag)
			{
				bool hasSelection = this.SelectedMultiplyItemDict.Count > 0;
				bool flag2 = !this.autoShowSelectedList;
				if (flag2)
				{
					bool flag3 = !hasSelection;
					if (flag3)
					{
						this.SetSelectedListVisible(false);
					}
				}
				else
				{
					this.SetSelectedListVisible(hasSelection);
				}
			}
		}

		// Token: 0x0600AF96 RID: 44950 RVA: 0x004FFE28 File Offset: 0x004FE028
		public void RefreshMultiplyOperationChrome()
		{
			this.RefreshSelectAllButtonState();
			this.RefreshSelectedCountLabel();
		}

		// Token: 0x0600AF97 RID: 44951 RVA: 0x004FFE39 File Offset: 0x004FE039
		public void SetItems(List<ItemDisplayData> items)
		{
			SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
			if (simpleMultiplyItemListScroll != null)
			{
				simpleMultiplyItemListScroll.SetItems(items);
			}
		}

		// Token: 0x0600AF98 RID: 44952 RVA: 0x004FFE4E File Offset: 0x004FE04E
		public void SetFilter(Func<ItemDisplayData, bool> filter)
		{
			this._userFilter = filter;
			this.ApplyCombinedFilterToCore();
		}

		// Token: 0x0600AF99 RID: 44953 RVA: 0x004FFE60 File Offset: 0x004FE060
		public void SetSearchText(string value, bool refresh = true)
		{
			this._searchText = (value ?? string.Empty);
			bool flag = this.searchField != null && this.searchField.text != this._searchText;
			if (flag)
			{
				this.searchField.SetTextWithoutNotify(this._searchText);
			}
			this.ApplyCombinedFilterToCore();
			bool flag2 = refresh && this.HasInit;
			if (flag2)
			{
				ItemListScroll curMultiplyScrollView = this.CurMultiplyScrollView;
				if (curMultiplyScrollView != null)
				{
					curMultiplyScrollView.ReRender();
				}
				this.RefreshSelectAllButtonState();
			}
		}

		// Token: 0x0600AF9A RID: 44954 RVA: 0x004FFEED File Offset: 0x004FE0ED
		public void ClearSearchText(bool refresh = true)
		{
			this.SetSearchText(string.Empty, refresh);
		}

		// Token: 0x0600AF9B RID: 44955 RVA: 0x004FFEFC File Offset: 0x004FE0FC
		public void SetSelectedListExpanded(bool expanded)
		{
			SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
			if (simpleMultiplyItemListScroll != null)
			{
				simpleMultiplyItemListScroll.SetSelectedListExpanded(expanded);
			}
		}

		// Token: 0x0600AF9C RID: 44956 RVA: 0x004FFF14 File Offset: 0x004FE114
		public void EnterMultiplyMode(bool showSelectedListOnEnter = true)
		{
			SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
			if (simpleMultiplyItemListScroll != null)
			{
				simpleMultiplyItemListScroll.EnterMultiplyMode(showSelectedListOnEnter);
			}
			this.ApplyOperationChrome(true);
			bool flag = !showSelectedListOnEnter;
			if (flag)
			{
				SimpleMultiplyItemListScroll simpleMultiplyItemListScroll2 = this.core;
				if (simpleMultiplyItemListScroll2 != null)
				{
					simpleMultiplyItemListScroll2.SetSelectedListVisible(false);
				}
			}
		}

		// Token: 0x0600AF9D RID: 44957 RVA: 0x004FFF58 File Offset: 0x004FE158
		public void ShowSwitchSelectionChromeVisible(bool visible)
		{
			if (visible)
			{
				this.ApplyOperationChrome(true);
			}
			else
			{
				this.ShowSwitchSelectionChrome(false);
			}
		}

		// Token: 0x0600AF9E RID: 44958 RVA: 0x004FFF7D File Offset: 0x004FE17D
		public void SetSelectedListVisible(bool visible)
		{
			SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
			if (simpleMultiplyItemListScroll != null)
			{
				simpleMultiplyItemListScroll.SetSelectedListVisible(visible);
			}
		}

		// Token: 0x0600AF9F RID: 44959 RVA: 0x004FFF92 File Offset: 0x004FE192
		public void ExitMultiplyMode()
		{
			SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
			if (simpleMultiplyItemListScroll != null)
			{
				simpleMultiplyItemListScroll.ExitMultiplyMode();
			}
			this.ApplyOperationChrome(false);
		}

		// Token: 0x0600AFA0 RID: 44960 RVA: 0x004FFFAF File Offset: 0x004FE1AF
		public void TryExitMultiplyMode(Action onConfirm)
		{
			SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
			if (simpleMultiplyItemListScroll != null)
			{
				simpleMultiplyItemListScroll.TryExitMultiplyMode(onConfirm);
			}
		}

		// Token: 0x0600AFA1 RID: 44961 RVA: 0x004FFFC4 File Offset: 0x004FE1C4
		public void RefreshItems()
		{
			SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
			if (simpleMultiplyItemListScroll != null)
			{
				simpleMultiplyItemListScroll.RefreshItems();
			}
		}

		// Token: 0x0600AFA2 RID: 44962 RVA: 0x004FFFD8 File Offset: 0x004FE1D8
		public void OnRenderItemMultiply(ITradeableContent tempData, RowItemLine view)
		{
			SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
			if (simpleMultiplyItemListScroll != null)
			{
				simpleMultiplyItemListScroll.OnRenderItemMultiply(tempData, view);
			}
		}

		// Token: 0x0600AFA3 RID: 44963 RVA: 0x004FFFF0 File Offset: 0x004FE1F0
		public void HandleRowClick(ItemDisplayData tempData, RowItemLine view)
		{
			bool flag = !this.CanOperateSelectionNow();
			if (!flag)
			{
				SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
				if (simpleMultiplyItemListScroll != null)
				{
					simpleMultiplyItemListScroll.HandleRowClick(tempData, view);
				}
				this.RefreshSelectionVisual(new bool?(this.IsSelectedListExpanded));
			}
		}

		// Token: 0x0600AFA4 RID: 44964 RVA: 0x00500033 File Offset: 0x004FE233
		public void BeginBatchSelectionOperation()
		{
			this._batchSelectionOperation = true;
			this.SetSelectedListExpanded(false);
		}

		// Token: 0x0600AFA5 RID: 44965 RVA: 0x00500045 File Offset: 0x004FE245
		public void EndBatchSelectionOperation()
		{
			this._batchSelectionOperation = false;
			this.RefreshSelectionVisual(new bool?(false));
		}

		// Token: 0x0600AFA6 RID: 44966 RVA: 0x0050005C File Offset: 0x004FE25C
		public void ApplySelectAllInFilteredList(bool select)
		{
			bool flag = !this.CanOperateSelectionNow();
			if (flag)
			{
				this.RefreshSelectAllButtonState();
			}
			else
			{
				SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
				if (simpleMultiplyItemListScroll != null)
				{
					simpleMultiplyItemListScroll.ApplySelectAllInFilteredList(select);
				}
			}
		}

		// Token: 0x0600AFA7 RID: 44967 RVA: 0x00500094 File Offset: 0x004FE294
		public void RefreshSelectionVisual(bool? refreshSelectedList = null)
		{
			bool refreshSelected = refreshSelectedList ?? (!this._batchSelectionOperation && this.IsSelectedListExpanded);
			this.ApplyAmountGeneratorToScrolls();
			ItemListScroll curMultiplyScrollView = this.CurMultiplyScrollView;
			if (curMultiplyScrollView != null)
			{
				curMultiplyScrollView.ReRender();
			}
			bool flag = refreshSelected;
			if (flag)
			{
				SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
				if (simpleMultiplyItemListScroll != null)
				{
					simpleMultiplyItemListScroll.RefreshSelectedScrollListPublic();
				}
				ItemListScroll selectedScrollView = this.SelectedScrollView;
				if (selectedScrollView != null)
				{
					selectedScrollView.ReRender();
				}
			}
			this.RefreshSelectedCountLabel();
			this.RefreshSelectAllButtonState();
		}

		// Token: 0x0600AFA8 RID: 44968 RVA: 0x0050011A File Offset: 0x004FE31A
		public void CollectFilteredSourceItems(List<ItemDisplayData> buffer)
		{
			SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
			if (simpleMultiplyItemListScroll != null)
			{
				simpleMultiplyItemListScroll.CollectFilteredSourceItems(buffer);
			}
		}

		// Token: 0x0600AFA9 RID: 44969 RVA: 0x00500130 File Offset: 0x004FE330
		public void ClearMultiplySelection(bool notifyContentChange = true)
		{
			bool flag = this.core == null;
			if (!flag)
			{
				this.core.ClearMultiplySelection();
				bool flag2 = !this._batchSelectionOperation;
				if (flag2)
				{
					this.RefreshSelectionVisual(new bool?(false));
				}
			}
		}

		// Token: 0x0600AFAA RID: 44970 RVA: 0x00500178 File Offset: 0x004FE378
		public void RefreshSelectAllButtonState()
		{
			SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
			if (simpleMultiplyItemListScroll != null)
			{
				simpleMultiplyItemListScroll.SyncTotalSelectedCount();
			}
			bool flag = this.buttonSelectAll != null;
			if (flag)
			{
				Func<bool> canEnableSelectAllButton = this.CanEnableSelectAllButton;
				bool canSelectAll = (canEnableSelectAllButton != null) ? canEnableSelectAllButton() : this.CanSelectAnyInFilteredList();
				this.buttonSelectAll.interactable = (canSelectAll && this.CanOperateSelectionNow());
			}
			bool flag2 = this.buttonClearSelection != null;
			if (flag2)
			{
				this.buttonClearSelection.interactable = (this.CanOperateSelectionNow() && this.TotalSelectedCount > 0);
			}
		}

		// Token: 0x0600AFAB RID: 44971 RVA: 0x0050020C File Offset: 0x004FE40C
		private bool CanSelectAnyInFilteredList()
		{
			ItemListScroll curMultiplyScrollView = this.CurMultiplyScrollView;
			IReadOnlyList<ITradeableContent> filtered = (curMultiplyScrollView != null) ? curMultiplyScrollView.FilteredData : null;
			return filtered != null && filtered.Count > 0 && filtered.Any(delegate(ITradeableContent d)
			{
				ItemDisplayData id = d as ItemDisplayData;
				return id != null && this.CanSelectItemForSelectAll(id);
			});
		}

		// Token: 0x0600AFAC RID: 44972 RVA: 0x00500252 File Offset: 0x004FE452
		public void OnExitMultiplyOperation(ArgumentBox args)
		{
			SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
			if (simpleMultiplyItemListScroll != null)
			{
				simpleMultiplyItemListScroll.OnExitMultiplyOperation(args);
			}
		}

		// Token: 0x0600AFAD RID: 44973 RVA: 0x00500268 File Offset: 0x004FE468
		private void OnMainSortAndFilterChanged()
		{
			bool flag = !this.HasInit;
			if (!flag)
			{
				SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
				if (simpleMultiplyItemListScroll != null)
				{
					simpleMultiplyItemListScroll.RefreshItems();
				}
				ItemListScroll curMultiplyScrollView = this.CurMultiplyScrollView;
				if (curMultiplyScrollView != null)
				{
					curMultiplyScrollView.ReRender();
				}
				this.RefreshSelectAllButtonState();
			}
		}

		// Token: 0x0600AFAE RID: 44974 RVA: 0x005002B0 File Offset: 0x004FE4B0
		private void ApplyCombinedFilterToCore()
		{
			bool flag = this.core == null;
			if (!flag)
			{
				this.core.SetFilter((ItemDisplayData data) => this.PassesCombinedFilter(data));
			}
		}

		// Token: 0x0600AFAF RID: 44975 RVA: 0x005002E8 File Offset: 0x004FE4E8
		private bool PassesCombinedFilter(ItemDisplayData data)
		{
			bool flag = this._userFilter != null && !this._userFilter(data);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this._searchText.IsNullOrEmpty();
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool flag3 = this.SearchMatchPredicate != null;
					if (flag3)
					{
						result = this.SearchMatchPredicate(data, this._searchText);
					}
					else
					{
						result = data.GetName(false).Contains(this._searchText);
					}
				}
			}
			return result;
		}

		// Token: 0x0600AFB0 RID: 44976 RVA: 0x00500364 File Offset: 0x004FE564
		private void ApplyItemListScrollChromeReferences()
		{
			ItemListScroll itemScroll = this.CurMultiplyScrollView;
			bool flag = itemScroll == null;
			if (!flag)
			{
				bool flag2 = this.toggleSwitchSelection != null;
				if (flag2)
				{
					FieldInfo field = typeof(ItemListScroll).GetField("switchSelection", BindingFlags.Instance | BindingFlags.NonPublic);
					if (field != null)
					{
						field.SetValue(itemScroll, this.toggleSwitchSelection);
					}
				}
				FieldInfo field2 = typeof(ItemListScroll).GetField("toggleSelectAll", BindingFlags.Instance | BindingFlags.NonPublic);
				if (field2 != null)
				{
					field2.SetValue(itemScroll, null);
				}
			}
		}

		// Token: 0x0600AFB1 RID: 44977 RVA: 0x005003E4 File Offset: 0x004FE5E4
		private void BindOperationChrome()
		{
			bool chromeBound = this._chromeBound;
			if (!chromeBound)
			{
				this._chromeBound = true;
				bool flag = this.toggleSwitchSelection != null;
				if (flag)
				{
					this.toggleSwitchSelection.onValueChanged.RemoveAllListeners();
					this.toggleSwitchSelection.onValueChanged.AddListener(new UnityAction<bool>(this.OnSwitchSelectionChanged));
					this.toggleSwitchSelection.SetIsOnWithoutNotify(false);
				}
				bool flag2 = this.buttonSelectAll != null;
				if (flag2)
				{
					this.buttonSelectAll.ClearAndAddListener(new Action(this.OnSelectAllButtonClicked));
					this.buttonSelectAll.gameObject.SetActive(false);
				}
				bool flag3 = this.buttonClearSelection != null;
				if (flag3)
				{
					this.buttonClearSelection.ClearAndAddListener(new Action(this.OnClearSelectionClicked));
					this.buttonClearSelection.gameObject.SetActive(false);
				}
				bool flag4 = this.searchField != null;
				if (flag4)
				{
					this.searchField.onValueChanged.RemoveListener(new UnityAction<string>(this.OnSearchFieldChanged));
					this.searchField.onValueChanged.AddListener(new UnityAction<string>(this.OnSearchFieldChanged));
					bool flag5 = this.showSearchChromeInMultiplyMode;
					if (flag5)
					{
						this.searchField.gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x0600AFB2 RID: 44978 RVA: 0x0050053C File Offset: 0x004FE73C
		private void OnSearchFieldChanged(string value)
		{
			bool flag = this.searchField != null && CommonUtils.FixToShowAbleString(ref value, this.searchField.textComponent.font);
			if (flag)
			{
				this.searchField.SetTextWithoutNotify(value);
			}
			this._searchText = (value ?? string.Empty);
			this.ApplyCombinedFilterToCore();
			bool flag2 = !this.HasInit;
			if (!flag2)
			{
				SimpleMultiplyItemListScroll simpleMultiplyItemListScroll = this.core;
				if (simpleMultiplyItemListScroll != null)
				{
					simpleMultiplyItemListScroll.RefreshItems();
				}
				ItemListScroll curMultiplyScrollView = this.CurMultiplyScrollView;
				if (curMultiplyScrollView != null)
				{
					curMultiplyScrollView.ReRender();
				}
				this.RefreshSelectAllButtonState();
			}
		}

		// Token: 0x0600AFB3 RID: 44979 RVA: 0x005005D4 File Offset: 0x004FE7D4
		private void OnSwitchSelectionChanged(bool isOn)
		{
			bool flag = this.core == null;
			if (!flag)
			{
				this.core.SetSelectedListExpanded(isOn);
				this.core.RefreshItems();
			}
		}

		// Token: 0x0600AFB4 RID: 44980 RVA: 0x00500610 File Offset: 0x004FE810
		private void OnSelectAllButtonClicked()
		{
			bool flag = !this.CanOperateSelectionNow();
			if (!flag)
			{
				this.BeginBatchSelectionOperation();
				try
				{
					bool flag2 = this.SelectAllHandler != null;
					if (flag2)
					{
						this.SelectAllHandler();
					}
					else
					{
						this.ApplySelectAllInFilteredList(true);
					}
				}
				finally
				{
					this.EndBatchSelectionOperation();
				}
			}
		}

		// Token: 0x0600AFB5 RID: 44981 RVA: 0x00500678 File Offset: 0x004FE878
		private void OnClearSelectionClicked()
		{
			bool flag = !this.CanOperateSelectionNow();
			if (!flag)
			{
				this.BeginBatchSelectionOperation();
				try
				{
					bool flag2 = this.ClearSelectionHandler != null;
					if (flag2)
					{
						this.ClearSelectionHandler();
					}
					else
					{
						this.ClearMultiplySelection(true);
					}
				}
				finally
				{
					this.EndBatchSelectionOperation();
				}
			}
		}

		// Token: 0x0600AFB6 RID: 44982 RVA: 0x005006E0 File Offset: 0x004FE8E0
		private bool CanOperateSelectionNow()
		{
			Func<bool> canOperateSelection = this.CanOperateSelection;
			return canOperateSelection == null || canOperateSelection();
		}

		// Token: 0x0600AFB7 RID: 44983 RVA: 0x005006F4 File Offset: 0x004FE8F4
		private void ResolveSearchField()
		{
			bool flag = this.searchField != null || !this.autoResolveSearchField;
			if (!flag)
			{
				foreach (TMP_InputField field in base.GetComponentsInChildren<TMP_InputField>(true))
				{
					bool flag2 = field.name.IndexOf("Search", StringComparison.OrdinalIgnoreCase) < 0;
					if (!flag2)
					{
						this.searchField = field;
						break;
					}
				}
			}
		}

		// Token: 0x0600AFB8 RID: 44984 RVA: 0x00500764 File Offset: 0x004FE964
		private void ResolveMainSortAndFilterRoot()
		{
			bool flag = this.mainSortAndFilterRoot != null;
			if (!flag)
			{
				ItemListScroll itemScroll = this.CurMultiplyScrollView;
				bool flag2 = itemScroll == null;
				if (!flag2)
				{
					SortAndFilter sortAndFilter = itemScroll.GetComponentInChildren<SortAndFilter>(true);
					bool flag3 = sortAndFilter != null;
					if (flag3)
					{
						this.mainSortAndFilterRoot = sortAndFilter.gameObject;
					}
				}
			}
		}

		// Token: 0x0600AFB9 RID: 44985 RVA: 0x005007BC File Offset: 0x004FE9BC
		private void ApplyOperationChrome(bool show)
		{
			this.ShowSelectAllChrome(show);
			this.ShowClearButtonChrome(show);
			this.ShowSwitchSelectionChrome(show);
			this.ShowSearchChrome(show);
			this.ShowSortAndFilterChrome(show);
			bool flag = this.selectedLabel != null;
			if (flag)
			{
				this.selectedLabel.gameObject.SetActive(show);
			}
			bool flag2 = !show;
			if (flag2)
			{
				this.RefreshSelectAllButtonState();
				this.RefreshSelectedCountLabel();
				bool flag3 = this.showSearchChromeInMultiplyMode;
				if (flag3)
				{
					this.ClearSearchText(false);
				}
			}
			else
			{
				this.RefreshSelectedCountLabel();
				this.RefreshSelectAllButtonState();
			}
		}

		// Token: 0x0600AFBA RID: 44986 RVA: 0x00500850 File Offset: 0x004FEA50
		private void ShowSelectAllChrome(bool show)
		{
			bool flag = this.selectAllChromeRoot != null;
			if (flag)
			{
				this.selectAllChromeRoot.SetActive(show);
			}
			else
			{
				bool flag2 = this.buttonSelectAll != null;
				if (flag2)
				{
					this.buttonSelectAll.gameObject.SetActive(show);
				}
			}
			bool flag3 = !show;
			if (flag3)
			{
				this.RefreshSelectAllButtonState();
			}
		}

		// Token: 0x0600AFBB RID: 44987 RVA: 0x005008B0 File Offset: 0x004FEAB0
		private bool CanSelectItemForSelectAll(ItemDisplayData data)
		{
			bool flag = data == null || this.core == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool isLocked = data.IsLocked;
				if (isLocked)
				{
					result = false;
				}
				else
				{
					bool flag2 = !this.PassesCombinedFilter(data);
					if (flag2)
					{
						result = false;
					}
					else
					{
						bool flag3 = this.CanSelectItemPredicate != null && !this.CanSelectItemPredicate(data);
						result = (!flag3 && (this.core.MaxSelectCount <= 0 || this.core.TotalSelectedCount < this.core.MaxSelectCount));
					}
				}
			}
			return result;
		}

		// Token: 0x0600AFBC RID: 44988 RVA: 0x0050094C File Offset: 0x004FEB4C
		private void ShowClearButtonChrome(bool show)
		{
			bool flag = this.buttonClearSelection != null;
			if (flag)
			{
				this.buttonClearSelection.gameObject.SetActive(show);
			}
		}

		// Token: 0x0600AFBD RID: 44989 RVA: 0x0050097C File Offset: 0x004FEB7C
		private void ShowSwitchSelectionChrome(bool show)
		{
			bool flag = this.core == null || this.SelectedScrollView == null || this.toggleSwitchSelection == null;
			if (!flag)
			{
				bool flag2 = this.switchSelectionChromeRoot != null;
				if (flag2)
				{
					this.switchSelectionChromeRoot.SetActive(show);
				}
				else
				{
					this.toggleSwitchSelection.gameObject.SetActive(show);
					Transform parent = this.toggleSwitchSelection.transform.parent;
					bool flag3 = parent != null && parent.parent != null;
					if (flag3)
					{
						parent.parent.gameObject.SetActive(show);
					}
				}
				bool flag4 = !show;
				if (flag4)
				{
					this.toggleSwitchSelection.SetIsOnWithoutNotify(false);
					this.core.SetSelectedListVisible(false);
				}
			}
		}

		// Token: 0x0600AFBE RID: 44990 RVA: 0x00500A58 File Offset: 0x004FEC58
		private void ShowSearchChrome(bool show)
		{
			bool flag = this.searchField == null || !this.showSearchChromeInMultiplyMode;
			if (!flag)
			{
				this.searchField.gameObject.SetActive(show);
				Transform parent = this.searchField.transform.parent;
				bool flag2 = parent != null && parent.name.IndexOf("Search", StringComparison.OrdinalIgnoreCase) >= 0;
				if (flag2)
				{
					parent.gameObject.SetActive(show);
				}
			}
		}

		// Token: 0x0600AFBF RID: 44991 RVA: 0x00500AE0 File Offset: 0x004FECE0
		private void ShowSortAndFilterChrome(bool show)
		{
			bool flag = this.mainSortAndFilterRoot == null || !this.showMainSortAndFilterInMultiplyMode;
			if (!flag)
			{
				this.mainSortAndFilterRoot.SetActive(show);
			}
		}

		// Token: 0x04008816 RID: 34838
		private const BindingFlags ItemListNonPublic = BindingFlags.Instance | BindingFlags.NonPublic;

		// Token: 0x04008817 RID: 34839
		[SerializeField]
		private SimpleMultiplyItemListScroll core;

		// Token: 0x04008818 RID: 34840
		[Header("多选操作栏（须在 Inspector 拖入引用）")]
		[Tooltip("显示/隐藏已选列表")]
		[SerializeField]
		private CToggle toggleSwitchSelection;

		// Token: 0x04008819 RID: 34841
		[Tooltip("已选数量文本")]
		[SerializeField]
		private TextMeshProUGUI selectedLabel;

		// Token: 0x0400881A RID: 34842
		[Tooltip("全选按钮")]
		[SerializeField]
		private CButton buttonSelectAll;

		// Token: 0x0400881B RID: 34843
		[Tooltip("清空选中")]
		[SerializeField]
		private CButton buttonClearSelection;

		// Token: 0x0400881C RID: 34844
		[Tooltip("显示已选开关的显隐根节点（一般为 SwitchSelection 父级，可选）")]
		[SerializeField]
		private GameObject switchSelectionChromeRoot;

		// Token: 0x0400881D RID: 34845
		[Tooltip("全选按钮区域的显隐根节点（可选）")]
		[SerializeField]
		private GameObject selectAllChromeRoot;

		// Token: 0x0400881E RID: 34846
		[Header("已选列表")]
		[Tooltip("为 true 时，SyncSelectedListPanelVisibility 会在有选中项时自动展开已选列表；为 false 时仅在有选中项被清空时自动收起")]
		[SerializeField]
		private bool autoShowSelectedList = false;

		// Token: 0x0400881F RID: 34847
		[Header("搜索（可选）")]
		[SerializeField]
		private TMP_InputField searchField;

		// Token: 0x04008820 RID: 34848
		[SerializeField]
		private bool autoResolveSearchField = true;

		// Token: 0x04008821 RID: 34849
		[SerializeField]
		private bool showSearchChromeInMultiplyMode = true;

		// Token: 0x04008822 RID: 34850
		[Header("排序筛选（可选）")]
		[SerializeField]
		private GameObject mainSortAndFilterRoot;

		// Token: 0x04008823 RID: 34851
		[SerializeField]
		private bool showMainSortAndFilterInMultiplyMode;

		// Token: 0x04008824 RID: 34852
		private bool _itemListScrollConfigured;

		// Token: 0x04008825 RID: 34853
		private bool _chromeBound;

		// Token: 0x04008826 RID: 34854
		private bool _batchSelectionOperation;

		// Token: 0x04008827 RID: 34855
		private string _searchText = string.Empty;

		// Token: 0x04008828 RID: 34856
		private Func<ItemDisplayData, bool> _userFilter;

		// Token: 0x04008829 RID: 34857
		private Func<ITradeableContent, string> _amountGenerator;

		// Token: 0x0400882A RID: 34858
		public Func<bool> CanOperateSelection;

		// Token: 0x0400882B RID: 34859
		public Func<ItemDisplayData, string, bool> SearchMatchPredicate;

		// Token: 0x0400882C RID: 34860
		public Action SelectAllHandler;

		// Token: 0x0400882D RID: 34861
		public Action ClearSelectionHandler;

		// Token: 0x0400882E RID: 34862
		public Func<int> GetSelectedCountForLabel;

		// Token: 0x0400882F RID: 34863
		public Func<bool> CanEnableSelectAllButton;
	}
}
