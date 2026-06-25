using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Item;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Book;
using Game.Components.SortAndFilter.Item;
using Game.Components.SortAndFilter.Item.Apply;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Story;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.World;
using GameData.Domains.World.Display;
using GameData.Serializer;
using GameData.Utilities;
using GameDataExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.SectInteract.Wudang
{
	// Token: 0x020009DF RID: 2527
	public class ViewDefendHeavenlyTreeFeed : UIBase
	{
		// Token: 0x17000DA3 RID: 3491
		// (get) Token: 0x06007B95 RID: 31637 RVA: 0x0039692D File Offset: 0x00394B2D
		private ViewDefendHeavenlyTreeFeed.TogKey CurTogKey
		{
			get
			{
				return (ViewDefendHeavenlyTreeFeed.TogKey)this.tabToggleGroup.GetActiveIndex();
			}
		}

		// Token: 0x06007B96 RID: 31638 RVA: 0x0039693C File Offset: 0x00394B3C
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<Location>("treeLocation", out this._treeLocation);
			argsBox.Get("enemyCount", out this._enemyCount);
			argsBox.Get("villagerCount", out this._villagerCount);
			argsBox.Get<DefendHeavenlyTreeDisplayData>("displayData", out this._defendHeavenlyTreeDisplayData);
			argsBox.Get<Action>("onConfirm", out this._onConfirm);
			this._multiplyReady = false;
			this._dialogQueue.Clear();
		}

		// Token: 0x06007B97 RID: 31639 RVA: 0x003969B8 File Offset: 0x00394BB8
		private void Awake()
		{
			this.tabToggleGroup.OnActiveIndexChange += this.TabToggleGroupOnOnActiveIndexChange;
			this.tabToggleGroup.Init(-1);
			ItemListScroll scroll = this.multiplyItemListScroll.CurMultiplyScrollView;
			scroll.CustomAmountDataGenerator = new Func<ITradeableContent, string>(this.HeavenlyFeedAmountCellText);
			scroll.Init("ViewDefendHeavenlyTreeFeedResource", ESortAndFilterControllerType.SortOnly, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderItem), new Action<ITradeableContent, RowItemLine>(this.OnClickItem), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, null, null);
			ItemListScroll selectedScroll = this.multiplyItemListScroll.SelectedScrollView;
			bool flag = selectedScroll != null;
			if (flag)
			{
				selectedScroll.CustomAmountDataGenerator = new Func<ITradeableContent, string>(this.HeavenlyFeedAmountCellText);
				selectedScroll.Init("ViewDefendHeavenlyTreeFeedSelected", ESortAndFilterControllerType.SortOnly, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderItem), new Action<ITradeableContent, RowItemLine>(this.OnClickItem), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, null, null);
			}
			bool flag2 = this.switchMultiplyMode != null;
			if (flag2)
			{
				this.switchMultiplyMode.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnMultiplyModeSwitchChanged));
				this.switchMultiplyMode.onValueChanged.AddListener(new UnityAction<bool>(this.OnMultiplyModeSwitchChanged));
				this.switchMultiplyMode.SetIsOnWithoutNotify(false);
			}
			bool flag3 = this.buttonSelectAll != null;
			if (flag3)
			{
				this.buttonSelectAll.ClearAndAddListener(new Action(this.OnSelectAllButtonClicked));
			}
			bool flag4 = this.buttonClearMultiplySelection != null;
			if (flag4)
			{
				this.buttonClearMultiplySelection.ClearAndAddListener(new Action(this.OnClearMultiplySelectionClicked));
			}
			bool flag5 = this.searchField != null;
			if (flag5)
			{
				this.searchField.onValueChanged.ResetListener(new Action<string>(this.OnSearch));
			}
			this.EnsureMultiplyListInitialized();
			this.EnsureFeedScrollSortFilter(ViewDefendHeavenlyTreeFeed.TogKey.Resource);
		}

		// Token: 0x06007B98 RID: 31640 RVA: 0x00396B7C File Offset: 0x00394D7C
		private void OnDestroy()
		{
			bool flag = this.switchMultiplyMode != null;
			if (flag)
			{
				this.switchMultiplyMode.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnMultiplyModeSwitchChanged));
			}
			CButton cbutton = this.buttonSelectAll;
			if (cbutton != null)
			{
				cbutton.RemoveAllListeners();
			}
			CButton cbutton2 = this.buttonClearMultiplySelection;
			if (cbutton2 != null)
			{
				cbutton2.RemoveAllListeners();
			}
		}

		// Token: 0x06007B99 RID: 31641 RVA: 0x00396BDC File Offset: 0x00394DDC
		private void OnEnable()
		{
			GEvent.Add(UiEvents.ViewDefendHeavenlyTreeRefresh, new GEvent.Callback(this.OnViewDefendHeavenlyTreeRefresh));
			GEvent.Add(UiEvents.ExitMultiplyOperation, new GEvent.Callback(this.OnExitMultiplyOperationFromEvent));
			this._ensureMultiplyOnNextReady = true;
			this.tabToggleGroup.Set(0, false);
			bool multiplyReady = this._multiplyReady;
			if (multiplyReady)
			{
				this.RefreshAfterMultiplyReady();
			}
			else
			{
				this.Refresh();
			}
		}

		// Token: 0x06007B9A RID: 31642 RVA: 0x00396C50 File Offset: 0x00394E50
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.ViewDefendHeavenlyTreeRefresh, new GEvent.Callback(this.OnViewDefendHeavenlyTreeRefresh));
			GEvent.Remove(UiEvents.ExitMultiplyOperation, new GEvent.Callback(this.OnExitMultiplyOperationFromEvent));
			this._feedChainGeneration++;
			this._feedChainActive = false;
			bool flag = this._multiplyReady && this.multiplyItemListScroll != null && this.multiplyItemListScroll.IsMultiItemSelect;
			if (flag)
			{
				this.multiplyItemListScroll.ExitMultiplyMode();
			}
			this._suppressMultiplySwitchEvent = true;
			CToggle ctoggle = this.switchMultiplyMode;
			if (ctoggle != null)
			{
				ctoggle.SetIsOnWithoutNotify(false);
			}
			this._suppressMultiplySwitchEvent = false;
			this.SyncFeedMultiplyChrome();
			this.RefreshSwitchMultiplyModeLabel();
			TMP_InputField tmp_InputField = this.searchField;
			if (tmp_InputField != null)
			{
				tmp_InputField.SetTextWithoutNotify(string.Empty);
			}
			this._searchText = string.Empty;
		}

		// Token: 0x06007B9B RID: 31643 RVA: 0x00396D2D File Offset: 0x00394F2D
		private void TabToggleGroupOnOnActiveIndexChange(int newIndex, int oldIndex)
		{
			this.RefreshItemList((ViewDefendHeavenlyTreeFeed.TogKey)newIndex);
		}

		// Token: 0x06007B9C RID: 31644 RVA: 0x00396D38 File Offset: 0x00394F38
		private static string GetFeedScrollSortSaveKey(ViewDefendHeavenlyTreeFeed.TogKey togKey)
		{
			return (togKey == ViewDefendHeavenlyTreeFeed.TogKey.Resource) ? "ViewDefendHeavenlyTreeFeedResource" : "ViewDefendHeavenlyTreeFeedBook";
		}

		// Token: 0x06007B9D RID: 31645 RVA: 0x00396D49 File Offset: 0x00394F49
		private static SortAndFilterController<ITradeableContent> CreateFeedScrollSortFilterController(ISortAndFilterView sortAndFilter, ViewDefendHeavenlyTreeFeed.TogKey togKey)
		{
			return (togKey == ViewDefendHeavenlyTreeFeed.TogKey.Resource) ? new MaterialAsRootSortAndFilterController(sortAndFilter) : new ViewDefendHeavenlyTreeFeed.FeedBookSortAndFilterController(sortAndFilter);
		}

		// Token: 0x06007B9E RID: 31646 RVA: 0x00396D5C File Offset: 0x00394F5C
		private void EnsureFeedScrollSortFilter(ViewDefendHeavenlyTreeFeed.TogKey togKey)
		{
			bool flag = !this._multiplyReady;
			if (!flag)
			{
				ItemListScroll itemListScroll = this.multiplyItemListScroll.CurMultiplyScrollView;
				bool flag2 = itemListScroll == null;
				if (!flag2)
				{
					bool flag3 = ViewDefendHeavenlyTreeFeed.IsFeedScrollSortFilterMatched(itemListScroll.SortAndFilterController, togKey);
					if (!flag3)
					{
						SortAndFilter sortAndFilter = itemListScroll.GetComponentInChildren<SortAndFilter>(true);
						bool flag4 = sortAndFilter == null;
						if (!flag4)
						{
							this.ReplaceFeedScrollSortFilter(itemListScroll, ViewDefendHeavenlyTreeFeed.CreateFeedScrollSortFilterController(sortAndFilter, togKey), ViewDefendHeavenlyTreeFeed.GetFeedScrollSortSaveKey(togKey));
						}
					}
				}
			}
		}

		// Token: 0x06007B9F RID: 31647 RVA: 0x00396DD4 File Offset: 0x00394FD4
		private static bool IsFeedScrollSortFilterMatched(SortAndFilterController<ITradeableContent> controller, ViewDefendHeavenlyTreeFeed.TogKey togKey)
		{
			if (!true)
			{
			}
			bool result;
			if (togKey != ViewDefendHeavenlyTreeFeed.TogKey.Resource)
			{
				result = (togKey == ViewDefendHeavenlyTreeFeed.TogKey.Book && controller is ViewDefendHeavenlyTreeFeed.FeedBookSortAndFilterController);
			}
			else
			{
				result = (controller is MaterialAsRootSortAndFilterController);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007BA0 RID: 31648 RVA: 0x00396E14 File Offset: 0x00395014
		private void ReplaceFeedScrollSortFilter(ItemListScroll itemListScroll, SortAndFilterController<ITradeableContent> newController, string sortSaveKey)
		{
			Type listType = typeof(ItemListScroll);
			FieldInfo field = listType.GetField("_sortAndFilterController", BindingFlags.Instance | BindingFlags.NonPublic);
			SortAndFilterController<ITradeableContent> oldController = ((field != null) ? field.GetValue(itemListScroll) : null) as SortAndFilterController<ITradeableContent>;
			if (oldController != null)
			{
				oldController.UninitForReplace();
			}
			newController.Init(new Action(this.OnFeedScrollSortAndFilterChanged), sortSaveKey);
			FieldInfo field2 = listType.GetField("_sortAndFilterController", BindingFlags.Instance | BindingFlags.NonPublic);
			if (field2 != null)
			{
				field2.SetValue(itemListScroll, newController);
			}
			FieldInfo field3 = listType.GetField("scroll", BindingFlags.Instance | BindingFlags.NonPublic);
			ListStyleGeneralScroll rowScroll = ((field3 != null) ? field3.GetValue(itemListScroll) : null) as ListStyleGeneralScroll;
			bool flag = rowScroll != null;
			if (flag)
			{
				rowScroll.SetSortController(newController);
			}
			FieldInfo field4 = listType.GetField("cardScroll", BindingFlags.Instance | BindingFlags.NonPublic);
			CardStyleGeneralScroll cardScroll = ((field4 != null) ? field4.GetValue(itemListScroll) : null) as CardStyleGeneralScroll;
			bool flag2 = cardScroll != null;
			if (flag2)
			{
				cardScroll.SetSortController(newController);
			}
		}

		// Token: 0x06007BA1 RID: 31649 RVA: 0x00396EEC File Offset: 0x003950EC
		private void OnFeedScrollSortAndFilterChanged()
		{
			bool flag = !this._multiplyReady;
			if (!flag)
			{
				this.multiplyItemListScroll.RefreshItems();
				this.multiplyItemListScroll.CurMultiplyScrollView.ReRender();
				this.RefreshSelectAllButtonInteractable();
			}
		}

		// Token: 0x06007BA2 RID: 31650 RVA: 0x00396F30 File Offset: 0x00395130
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (!(a == "ButtonCloseView"))
			{
				if (!(a == "ButtonConfirm"))
				{
					if (a == "ButtonClearMultiplySelection")
					{
						this.OnClearMultiplySelectionClicked();
					}
				}
				else
				{
					this.OnClickConfirm();
				}
			}
			else
			{
				this.QuickHide();
			}
		}

		// Token: 0x06007BA3 RID: 31651 RVA: 0x00396F8C File Offset: 0x0039518C
		private void OnMultiplyModeSwitchChanged(bool isOn)
		{
			bool suppressMultiplySwitchEvent = this._suppressMultiplySwitchEvent;
			if (!suppressMultiplySwitchEvent)
			{
				this.EnsureMultiplyListInitialized();
				bool flag = !this._multiplyReady;
				if (!flag)
				{
					this.EnsureFeedMultiplyLogicActive();
					bool flag2 = this.multiplyItemListScroll.SelectedMultiplyItemDict.Count == 0;
					if (flag2)
					{
						this._suppressMultiplySwitchEvent = true;
						CToggle ctoggle = this.switchMultiplyMode;
						if (ctoggle != null)
						{
							ctoggle.SetIsOnWithoutNotify(false);
						}
						this._suppressMultiplySwitchEvent = false;
						this.multiplyItemListScroll.SetSelectedListVisible(false);
					}
					else
					{
						this.multiplyItemListScroll.SetSelectedListVisible(true);
						this.multiplyItemListScroll.SetSelectedListExpanded(isOn);
						this.SyncFeedMultiplyChrome();
						this.multiplyItemListScroll.CurMultiplyScrollView.ReRender();
						ItemListScroll selectedScrollView = this.multiplyItemListScroll.SelectedScrollView;
						if (selectedScrollView != null)
						{
							selectedScrollView.ReRender();
						}
					}
				}
			}
		}

		// Token: 0x06007BA4 RID: 31652 RVA: 0x0039705C File Offset: 0x0039525C
		private void OnSelectAllButtonClicked()
		{
			bool flag = !this._multiplyReady || !this.multiplyItemListScroll.IsMultiItemSelect;
			if (!flag)
			{
				this.multiplyItemListScroll.ApplySelectAllInFilteredList(true);
				this.OnMultiplyContentChanged(null);
			}
		}

		// Token: 0x06007BA5 RID: 31653 RVA: 0x003970A0 File Offset: 0x003952A0
		private void OnExitMultiplyOperationFromEvent(ArgumentBox argsBox)
		{
			bool flag = !this._multiplyReady;
			if (!flag)
			{
				this.multiplyItemListScroll.ClearMultiplySelection();
				this.EnsureFeedMultiplyLogicActive();
				this.SyncMultiplyPanelWithSelection();
				this.multiplyItemListScroll.CurMultiplyScrollView.ReRender();
				this.RefreshMultiplySelectionUi();
				this.RefreshButtonConfirm();
			}
		}

		// Token: 0x06007BA6 RID: 31654 RVA: 0x003970F8 File Offset: 0x003952F8
		private void OnClearMultiplySelectionClicked()
		{
			bool flag = !this._multiplyReady || !this.multiplyItemListScroll.IsMultiItemSelect;
			if (!flag)
			{
				this.multiplyItemListScroll.ClearMultiplySelection();
				bool flag2 = this.buttonSelectAll == null;
				if (flag2)
				{
					CToggle ta = this.multiplyItemListScroll.ToggleSelectAll;
					bool flag3 = ta != null;
					if (flag3)
					{
						ta.SetIsOnWithoutNotify(false);
					}
				}
				this.OnMultiplyContentChanged(null);
			}
		}

		// Token: 0x06007BA7 RID: 31655 RVA: 0x0039716C File Offset: 0x0039536C
		private void SyncFeedMultiplyChrome()
		{
			bool flag = !this._multiplyReady || this.multiplyItemListScroll == null;
			if (!flag)
			{
				bool show = this.multiplyItemListScroll.IsMultiItemSelect;
				CToggle ta = this.multiplyItemListScroll.ToggleSelectAll;
				bool flag2 = ta != null;
				if (flag2)
				{
					bool showBuiltInSelectAllToggle = show && this.buttonSelectAll == null;
					ta.gameObject.SetActive(showBuiltInSelectAllToggle);
					bool flag3 = showBuiltInSelectAllToggle;
					if (flag3)
					{
						this.multiplyItemListScroll.RefreshSelectAllToggleState();
					}
				}
				bool flag4 = this.buttonSelectAll != null;
				if (flag4)
				{
					this.buttonSelectAll.gameObject.SetActive(show);
					bool flag5 = show;
					if (flag5)
					{
						this.RefreshSelectAllButtonInteractable();
					}
				}
				bool flag6 = this.buttonClearMultiplySelection != null;
				if (flag6)
				{
					this.buttonClearMultiplySelection.gameObject.SetActive(show);
				}
			}
		}

		// Token: 0x06007BA8 RID: 31656 RVA: 0x00397250 File Offset: 0x00395450
		private void RefreshSelectAllButtonInteractable()
		{
			bool flag = this.buttonSelectAll == null || !this.buttonSelectAll.gameObject.activeSelf;
			if (!flag)
			{
				IReadOnlyList<ITradeableContent> fd = this.multiplyItemListScroll.CurMultiplyScrollView.FilteredData;
				this.buttonSelectAll.interactable = (fd != null && fd.Any(delegate(ITradeableContent d)
				{
					ItemDisplayData id = d as ItemDisplayData;
					return id != null && this.CanFeedItemSelectForGrowth(id);
				}));
			}
		}

		// Token: 0x06007BA9 RID: 31657 RVA: 0x003972C0 File Offset: 0x003954C0
		private void ApplyFeedMultiplyModeForCurrentData()
		{
			bool flag = !this._multiplyReady;
			if (!flag)
			{
				bool ensureMultiplyOnNextReady = this._ensureMultiplyOnNextReady;
				if (ensureMultiplyOnNextReady)
				{
					this._ensureMultiplyOnNextReady = false;
				}
				this.EnsureFeedMultiplyLogicActive();
				this.SyncMultiplyPanelWithSelection();
			}
		}

		// Token: 0x06007BAA RID: 31658 RVA: 0x003972FC File Offset: 0x003954FC
		private void EnsureFeedMultiplyLogicActive()
		{
			bool flag = !this.multiplyItemListScroll.IsMultiItemSelect;
			if (flag)
			{
				this.multiplyItemListScroll.EnterMultiplyMode(false);
			}
			else
			{
				this.multiplyItemListScroll.ShowSwitchSelectionChromeVisible(true);
			}
			this.multiplyItemListScroll.SetFilter(new Func<ItemDisplayData, bool>(this.PassesFeedListFilter));
		}

		// Token: 0x06007BAB RID: 31659 RVA: 0x00397350 File Offset: 0x00395550
		private void SyncMultiplyPanelWithSelection()
		{
			bool flag = !this._multiplyReady;
			if (!flag)
			{
				bool flag2 = this.multiplyItemListScroll.SelectedMultiplyItemDict.Count == 0;
				if (flag2)
				{
					this.multiplyItemListScroll.SetSelectedListVisible(false);
					this._suppressMultiplySwitchEvent = true;
					CToggle ctoggle = this.switchMultiplyMode;
					if (ctoggle != null)
					{
						ctoggle.SetIsOnWithoutNotify(false);
					}
					this._suppressMultiplySwitchEvent = false;
				}
				this.SyncFeedMultiplyChrome();
				this.RefreshSwitchMultiplyModeLabel();
			}
		}

		// Token: 0x06007BAC RID: 31660 RVA: 0x003973C4 File Offset: 0x003955C4
		private int GetSelectedEntryCount()
		{
			bool flag = !this._multiplyReady;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				result = this.multiplyItemListScroll.SelectedMultiplyItemDict.Count;
			}
			return result;
		}

		// Token: 0x06007BAD RID: 31661 RVA: 0x003973F8 File Offset: 0x003955F8
		private void RefreshSwitchMultiplyModeLabel()
		{
			bool flag = this.switchMultiplyModeLabel == null;
			if (!flag)
			{
				this.switchMultiplyModeLabel.text = this.GetSelectedEntryCount().ToString();
			}
		}

		// Token: 0x06007BAE RID: 31662 RVA: 0x00397434 File Offset: 0x00395634
		private void EnsureMultiplyListInitialized()
		{
			bool flag = this._multiplyReady || this.multiplyItemListScroll == null;
			if (!flag)
			{
				bool flag2 = !this.multiplyItemListScroll.HasInit;
				if (flag2)
				{
					this.multiplyItemListScroll.Init(new Action<List<ValueTuple<ItemDisplayData, int>>>(this.OnMultiplyContentChanged));
				}
				this.multiplyItemListScroll.CanSelectItemPredicate = new Func<ItemDisplayData, bool>(this.CanFeedItemSelectForGrowth);
				this.multiplyItemListScroll.GetSelectLimitOverride = new Func<ItemDisplayData, int, int>(this.GetFeedSelectLimit);
				this.multiplyItemListScroll.GetSelectCountLimitTip = new Func<ItemDisplayData, string>(this.GetFeedSelectCountLimitTip);
				this._multiplyReady = true;
				this.RefreshSwitchMultiplyModeLabel();
			}
		}

		// Token: 0x06007BAF RID: 31663 RVA: 0x003974DC File Offset: 0x003956DC
		private void OnMultiplyContentChanged([TupleElementNames(new string[]
		{
			"data",
			"count"
		})] List<ValueTuple<ItemDisplayData, int>> _ = null)
		{
			this.SyncMultiplyPanelWithSelection();
			this.RefreshMultiplySelectionUi();
			this.multiplyItemListScroll.RefreshItems();
			this.multiplyItemListScroll.CurMultiplyScrollView.ReRender();
			ItemListScroll selectedScrollView = this.multiplyItemListScroll.SelectedScrollView;
			if (selectedScrollView != null)
			{
				selectedScrollView.ReRender();
			}
			this.multiplyItemListScroll.RefreshSelectAllToggleState();
			this.RefreshSelectAllButtonInteractable();
			this.RefreshButtonConfirm();
		}

		// Token: 0x06007BB0 RID: 31664 RVA: 0x00397548 File Offset: 0x00395748
		private string HeavenlyFeedAmountCellText(ITradeableContent content)
		{
			ItemDisplayData item = content as ItemDisplayData;
			bool flag = item == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				ItemDisplayData ownedData = this.FindDisplayData(item.RealKey) ?? item;
				string ownedAmountStr = CommonUtils.GetDisplayStringForNum(ownedData.Amount, 100000);
				bool flag2 = !this.multiplyItemListScroll.IsMultiItemSelect;
				if (flag2)
				{
					result = ownedAmountStr;
				}
				else
				{
					ItemDisplayData selectedData = this.multiplyItemListScroll.SelectedMultiplyItemOrderedList.Find((ItemDisplayData d) => d.RealKey.Equals(item.RealKey));
					int selectedCount = 0;
					bool isMultiplySelected = selectedData != null && this.multiplyItemListScroll.SelectedMultiplyItemDict.TryGetValue(selectedData, out selectedCount) && selectedCount > 0;
					bool flag3 = !isMultiplySelected;
					if (flag3)
					{
						result = ownedAmountStr;
					}
					else
					{
						string selectedAmountStr = CommonUtils.GetDisplayStringForNum(selectedCount, 100000);
						result = selectedAmountStr + "/" + ownedAmountStr;
					}
				}
			}
			return result;
		}

		// Token: 0x06007BB1 RID: 31665 RVA: 0x00397640 File Offset: 0x00395840
		public void OnRenderItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, false);
			bool flag = this._multiplyReady && this.multiplyItemListScroll.IsMultiItemSelect;
			if (flag)
			{
				this.multiplyItemListScroll.OnRenderItemMultiply(itemData, rowItemLine);
				this.ApplyFeedMultiplyRowTip(itemData as ItemDisplayData, rowItemLine);
			}
			else
			{
				bool can = this.PassesFeedListFilter(itemData as ItemDisplayData);
				rowItemLine.SetInteractable(can, true);
				rowItemLine.SetDisabled(!can);
			}
		}

		// Token: 0x06007BB2 RID: 31666 RVA: 0x003976C4 File Offset: 0x003958C4
		private void ApplyFeedMultiplyRowTip(ItemDisplayData data, RowItemLine rowItemLine)
		{
			bool flag = data == null || rowItemLine == null || rowItemLine.Interactable || this._treeData == null || !this._treeData.IsGrowPointMax;
			if (!flag)
			{
				TooltipInvoker tip = rowItemLine.TipDisplayer;
				bool flag2 = tip == null;
				if (!flag2)
				{
					tip.enabled = true;
					tip.PresetParam = new string[]
					{
						LanguageKey.LK_DefendHeavenlyTree_Feed_IsMax.Tr()
					};
				}
			}
		}

		// Token: 0x06007BB3 RID: 31667 RVA: 0x0039773C File Offset: 0x0039593C
		public void OnClickItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			bool flag = this._multiplyReady && this.multiplyItemListScroll.IsMultiItemSelect;
			if (flag)
			{
				this.HandleFeedMultiplyRowClick(itemData as ItemDisplayData, rowItemLine);
			}
		}

		// Token: 0x06007BB4 RID: 31668 RVA: 0x00397778 File Offset: 0x00395978
		private void HandleFeedMultiplyRowClick(ItemDisplayData tempData, RowItemLine view)
		{
			bool flag = tempData == null || view == null || !view.Interactable;
			if (!flag)
			{
				ItemDisplayData itemData = this.GetFeedInventoryItem(tempData.RealKey);
				bool flag2 = itemData == null;
				if (!flag2)
				{
					int selectedCount;
					this.multiplyItemListScroll.SelectedMultiplyItemDict.TryGetValue(itemData, out selectedCount);
					bool isSelected = selectedCount > 0;
					bool isInSelectedList = isSelected && tempData != itemData;
					ItemListScroll scroll = (isInSelectedList && this.multiplyItemListScroll.SelectedScrollView != null) ? this.multiplyItemListScroll.SelectedScrollView : this.multiplyItemListScroll.CurMultiplyScrollView;
					bool flag3 = isSelected;
					if (flag3)
					{
						this.SetFeedMultiplyCount(itemData, 0);
					}
					else
					{
						int growthLimit = this.GetMaxSelectableCountForGrowth(itemData);
						bool flag4 = growthLimit <= 0;
						if (!flag4)
						{
							bool flag5 = itemData.Amount == 1;
							if (flag5)
							{
								this.SetFeedMultiplyCount(itemData, 1);
							}
							else
							{
								scroll.HandleClickItem(itemData, view, delegate(RowItemLine row)
								{
									this.OpenFeedSelectCountMode(scroll, row, itemData, growthLimit);
								});
							}
						}
					}
				}
			}
		}

		// Token: 0x06007BB5 RID: 31669 RVA: 0x003978C4 File Offset: 0x00395AC4
		private void OpenFeedSelectCountMode(ItemListScroll scroll, RowItemLine itemView, ItemDisplayData itemData, int growthLimit)
		{
			ViewDefendHeavenlyTreeFeed.<>c__DisplayClass74_0 CS$<>8__locals1 = new ViewDefendHeavenlyTreeFeed.<>c__DisplayClass74_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.itemData = itemData;
			CS$<>8__locals1.itemView = itemView;
			CS$<>8__locals1.scroll = scroll;
			CS$<>8__locals1.scroll.HighLightItemView(CS$<>8__locals1.itemView);
			CS$<>8__locals1.scrollRect = CS$<>8__locals1.scroll.InfiniteScroll.Scroll;
			RectTransform itemRectTrans = CS$<>8__locals1.itemView.transform as RectTransform;
			int ownedAmount = CS$<>8__locals1.itemData.Amount;
			int effectiveMax = (growthLimit > 0) ? Math.Min(growthLimit, ownedAmount) : ownedAmount;
			int maxCount = effectiveMax;
			int initSelectCount = effectiveMax;
			int minCount = 1;
			initSelectCount = Mathf.Clamp(initSelectCount, minCount, maxCount);
			string limitTip = this.GetFeedSelectCountLimitTip(CS$<>8__locals1.itemData);
			bool flag = growthLimit >= ownedAmount;
			if (flag)
			{
				limitTip = string.Empty;
			}
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("MinCount", minCount);
			argBox.Set("MaxCount", maxCount);
			argBox.Set("InitCount", initSelectCount);
			argBox.Set("LimitCount", growthLimit);
			argBox.Set("LimitTip", limitTip);
			int changeValue = ItemTemplateHelper.GetItemCountUnit(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
			argBox.Set("ChangeValue", changeValue);
			argBox.SetObject("FollowOffset", Vector2.zero);
			argBox.SetObject("OnConfirmSetCount", new Action<int>(delegate(int count)
			{
				CS$<>8__locals1.<>4__this.SetFeedMultiplyCount(CS$<>8__locals1.itemData, count);
			}));
			argBox.SetObject("OnCancelSetCount", new Action(delegate
			{
				CS$<>8__locals1.<>4__this.SetFeedMultiplyCount(CS$<>8__locals1.itemData, 0);
			}));
			argBox.SetObject("ItemRectTrans", itemRectTrans);
			argBox.Set("ZeroValid", false);
			CS$<>8__locals1.originalParent = CS$<>8__locals1.itemView.transform.parent;
			UIElement.SetSelectCount.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.SetSelectCount, true);
			UIElement setSelectCount = UIElement.SetSelectCount;
			setSelectCount.OnShowed = (Action)Delegate.Combine(setSelectCount.OnShowed, new Action(CS$<>8__locals1.<OpenFeedSelectCountMode>g__OnFeedSelectCountShowed|2));
			UIElement setSelectCount2 = UIElement.SetSelectCount;
			setSelectCount2.OnHide = (Action)Delegate.Combine(setSelectCount2.OnHide, new Action(CS$<>8__locals1.<OpenFeedSelectCountMode>g__OnFeedSelectCountHidden|3));
		}

		// Token: 0x06007BB6 RID: 31670 RVA: 0x00397AF4 File Offset: 0x00395CF4
		private void SetFeedMultiplyCount(ItemDisplayData itemData, int count)
		{
			itemData = (this.GetFeedInventoryItem(itemData.RealKey) ?? itemData);
			Dictionary<ItemDisplayData, int> dict = this.multiplyItemListScroll.SelectedMultiplyItemDict;
			List<ItemDisplayData> ordered = this.multiplyItemListScroll.SelectedMultiplyItemOrderedList;
			int lastCount;
			bool flag = dict.TryGetValue(itemData, out lastCount) && count == lastCount;
			if (!flag)
			{
				int index = ordered.IndexOf(itemData);
				bool flag2 = ordered.CheckIndex(index);
				if (flag2)
				{
					ordered[index] = itemData;
					dict[itemData] = count;
					bool flag3 = count <= 0;
					if (flag3)
					{
						dict.Remove(itemData);
						ordered.Remove(itemData);
					}
				}
				else
				{
					bool flag4 = count > 0;
					if (!flag4)
					{
						return;
					}
					ordered.Add(itemData);
					dict.Add(itemData, count);
				}
				this.multiplyItemListScroll.SyncTotalSelectedCount();
				this.OnMultiplyContentChanged(null);
			}
		}

		// Token: 0x06007BB7 RID: 31671 RVA: 0x00397BCC File Offset: 0x00395DCC
		private void OnSearch(string value)
		{
			bool flag = this.searchField != null && CommonUtils.FixToShowAbleString(ref value, this.searchField.textComponent.font);
			if (flag)
			{
				this.searchField.SetTextWithoutNotify(value);
			}
			this._searchText = (value ?? string.Empty);
			bool flag2 = !this._multiplyReady;
			if (!flag2)
			{
				this.multiplyItemListScroll.SetFilter(new Func<ItemDisplayData, bool>(this.PassesFeedListFilter));
				this.multiplyItemListScroll.RefreshItems();
				this.multiplyItemListScroll.CurMultiplyScrollView.ReRender();
				this.RefreshSelectAllButtonInteractable();
			}
		}

		// Token: 0x06007BB8 RID: 31672 RVA: 0x00397C70 File Offset: 0x00395E70
		private bool PassesFeedListFilter(ItemDisplayData d)
		{
			bool flag = !this.HeavenlyTreeFeedItemPassesFilter(d);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this._searchText.IsNullOrEmpty();
				result = (flag2 || d.GetName(false).Contains(this._searchText));
			}
			return result;
		}

		// Token: 0x06007BB9 RID: 31673 RVA: 0x00397CB9 File Offset: 0x00395EB9
		private void RefreshWudangOrgTaskStatus()
		{
			this._wudangOrgTaskStatus = SingletonObject.getInstance<BuildingModel>().GetOrgTaskStatus(4);
		}

		// Token: 0x06007BBA RID: 31674 RVA: 0x00397CCD File Offset: 0x00395ECD
		private static int GetGrowthThreshold()
		{
			return 900;
		}

		// Token: 0x06007BBB RID: 31675 RVA: 0x00397CD4 File Offset: 0x00395ED4
		private int CalcItemFeedGrowDelta(ItemDisplayData item)
		{
			bool flag = item == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				this.RefreshWudangOrgTaskStatus();
				bool isResource = item.IsResource;
				if (isResource)
				{
					result = GameData.Domains.World.SharedMethods.GetHeavenlyTreeGrewUpValueByResource(this._wudangOrgTaskStatus, item.Amount);
				}
				else
				{
					bool flag2 = item.RealKey.ItemType == 10;
					if (flag2)
					{
						result = GameData.Domains.World.SharedMethods.GetHeavenlyTreeGrewUpValueByBook(this._wudangOrgTaskStatus);
					}
					else
					{
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x06007BBC RID: 31676 RVA: 0x00397D3C File Offset: 0x00395F3C
		private int CalcSelectionPreviewGrowDelta(ItemDisplayData exclude = null)
		{
			bool flag = !this._multiplyReady || this._treeData == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int simulatedGrow = (int)this._treeData.GrowPoint;
				int max = ViewDefendHeavenlyTreeFeed.GetGrowthThreshold();
				int totalDelta = 0;
				foreach (ItemDisplayData item in this.BuildFeedQueueFromSelection())
				{
					bool flag2 = exclude != null && item.RealKey.Equals(exclude.RealKey);
					if (!flag2)
					{
						int step = this.CalcItemFeedGrowDelta(item);
						int cappedStep = Math.Min(step, Math.Max(0, max - simulatedGrow));
						bool flag3 = cappedStep <= 0;
						if (flag3)
						{
							break;
						}
						simulatedGrow += cappedStep;
						totalDelta += cappedStep;
					}
				}
				result = totalDelta;
			}
			return result;
		}

		// Token: 0x06007BBD RID: 31677 RVA: 0x00397E24 File Offset: 0x00396024
		private int GetSelectedGrowDeltaExcluding(ItemDisplayData exclude = null)
		{
			return this.CalcSelectionPreviewGrowDelta(exclude);
		}

		// Token: 0x06007BBE RID: 31678 RVA: 0x00397E30 File Offset: 0x00396030
		private int GetRemainingGrowCapacityExcluding(ItemDisplayData exclude = null)
		{
			bool flag = this._treeData == null || this._treeData.IsGrowPointMax;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				result = Math.Max(0, ViewDefendHeavenlyTreeFeed.GetGrowthThreshold() - (int)this._treeData.GrowPoint - this.GetSelectedGrowDeltaExcluding(exclude));
			}
			return result;
		}

		// Token: 0x06007BBF RID: 31679 RVA: 0x00397E80 File Offset: 0x00396080
		private int GetRemainingFeedOperationBudgetExcluding(ItemDisplayData exclude = null)
		{
			int remainActionPoint = SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth;
			int maxOps = remainActionPoint / 100;
			return Math.Max(0, maxOps - this.GetFeedOperationCountExcluding(exclude));
		}

		// Token: 0x06007BC0 RID: 31680 RVA: 0x00397EB4 File Offset: 0x003960B4
		private int GetFeedOperationCountExcluding(ItemDisplayData exclude = null)
		{
			bool flag = !this._multiplyReady;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int count = 0;
				foreach (KeyValuePair<ItemDisplayData, int> kv in this.multiplyItemListScroll.SelectedMultiplyItemDict)
				{
					bool flag2 = kv.Value <= 0;
					if (!flag2)
					{
						bool flag3 = exclude != null && kv.Key.RealKey.Equals(exclude.RealKey);
						if (!flag3)
						{
							count += ((kv.Key.RealKey.ItemType == 10) ? kv.Value : 1);
						}
					}
				}
				result = count;
			}
			return result;
		}

		// Token: 0x06007BC1 RID: 31681 RVA: 0x00397F88 File Offset: 0x00396188
		private int GetMaxSelectableCountForGrowth(ItemDisplayData data)
		{
			int remaining = this.GetRemainingGrowCapacityExcluding(data);
			bool flag = remaining <= 0 || data == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int apBudget = this.GetRemainingFeedOperationBudgetExcluding(data);
				bool isResource = data.IsResource;
				if (isResource)
				{
					bool flag2 = data.Amount < 100 || apBudget < 1;
					if (flag2)
					{
						result = 0;
					}
					else
					{
						int growthCap = this.GetMaxResourceCountForRemainingGrowth(remaining, data.Amount);
						result = ((growthCap <= 0) ? 0 : Math.Min(growthCap, data.Amount));
					}
				}
				else
				{
					bool flag3 = data.RealKey.ItemType != 10;
					if (flag3)
					{
						result = 0;
					}
					else
					{
						int perBook = GameData.Domains.World.SharedMethods.GetHeavenlyTreeGrewUpValueByBook(this._wudangOrgTaskStatus);
						bool flag4 = perBook <= 0 || apBudget <= 0;
						if (flag4)
						{
							result = 0;
						}
						else
						{
							int bookGrowthCap = ViewDefendHeavenlyTreeFeed.GetMaxBookCountForRemainingGrowth(remaining, data.Amount, perBook);
							result = ((bookGrowthCap <= 0) ? 0 : Math.Min(bookGrowthCap, apBudget));
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06007BC2 RID: 31682 RVA: 0x00398078 File Offset: 0x00396278
		private static int GetMaxBookCountForRemainingGrowth(int remainingGrowth, int ownedAmount, int perBook)
		{
			bool flag = remainingGrowth <= 0 || ownedAmount <= 0 || perBook <= 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = remainingGrowth < perBook;
				if (flag2)
				{
					result = Math.Min(ownedAmount, 1);
				}
				else
				{
					result = Math.Min(ownedAmount, remainingGrowth / perBook);
				}
			}
			return result;
		}

		// Token: 0x06007BC3 RID: 31683 RVA: 0x003980C0 File Offset: 0x003962C0
		private int GetMaxResourceCountForRemainingGrowth(int remainingGrowth, int ownedAmount)
		{
			int minResource = 100;
			bool flag = remainingGrowth <= 0 || ownedAmount < minResource;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int minNeeded = Math.Max(minResource, this.GetMinResourceCountForRemainingGrowth(remainingGrowth));
				bool flag2 = ownedAmount < minNeeded;
				if (flag2)
				{
					int growth = GameData.Domains.World.SharedMethods.GetHeavenlyTreeGrewUpValueByResource(this._wudangOrgTaskStatus, ownedAmount);
					result = ((growth > 0 && growth <= remainingGrowth) ? ownedAmount : 0);
				}
				else
				{
					result = minNeeded;
				}
			}
			return result;
		}

		// Token: 0x06007BC4 RID: 31684 RVA: 0x00398124 File Offset: 0x00396324
		private int GetMinResourceCountForRemainingGrowth(int remainingGrowth)
		{
			bool flag = remainingGrowth <= 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = this._wudangOrgTaskStatus == 0;
				if (flag2)
				{
					result = remainingGrowth * 100;
				}
				else
				{
					result = (remainingGrowth * 100 + 2) / 3;
				}
			}
			return result;
		}

		// Token: 0x06007BC5 RID: 31685 RVA: 0x00398164 File Offset: 0x00396364
		private bool CanFeedItemSelectForGrowth(ItemDisplayData data)
		{
			bool flag = data == null || this._treeData == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool isGrowPointMax = this._treeData.IsGrowPointMax;
				if (isGrowPointMax)
				{
					result = false;
				}
				else
				{
					bool flag2 = !this.PassesFeedListFilter(data);
					if (flag2)
					{
						result = false;
					}
					else
					{
						int selected;
						bool flag3 = this.multiplyItemListScroll.SelectedMultiplyItemDict.TryGetValue(data, out selected) && selected > 0;
						result = (flag3 || this.GetMaxSelectableCountForGrowth(data) > 0);
					}
				}
			}
			return result;
		}

		// Token: 0x06007BC6 RID: 31686 RVA: 0x003981E4 File Offset: 0x003963E4
		private int GetFeedSelectLimit(ItemDisplayData data, int baseLimit)
		{
			SectStoryHeavenlyTreeExtendable treeData = this._treeData;
			bool flag = treeData != null && treeData.IsGrowPointMax;
			int result;
			if (flag)
			{
				int atMax;
				bool flag2 = this.multiplyItemListScroll.SelectedMultiplyItemDict.TryGetValue(data, out atMax);
				if (flag2)
				{
					result = Math.Min(baseLimit, atMax);
				}
				else
				{
					result = 0;
				}
			}
			else
			{
				int growLimit = this.GetMaxSelectableCountForGrowth(data);
				bool flag3 = growLimit <= 0;
				if (flag3)
				{
					int current;
					bool flag4 = this.multiplyItemListScroll.SelectedMultiplyItemDict.TryGetValue(data, out current) && current > 0;
					if (flag4)
					{
						result = Math.Min(baseLimit, current);
					}
					else
					{
						result = 0;
					}
				}
				else
				{
					result = Math.Min(baseLimit, growLimit);
				}
			}
			return result;
		}

		// Token: 0x06007BC7 RID: 31687 RVA: 0x00398288 File Offset: 0x00396488
		private string GetFeedSelectCountLimitTip(ItemDisplayData data)
		{
			bool flag = data == null || this._treeData == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				bool flag2 = this._treeData.IsGrowPointMax || this.GetRemainingGrowCapacityExcluding(data) <= 0;
				if (flag2)
				{
					result = LanguageKey.LK_DefendHeavenlyTree_Feed_IsMax.Tr();
				}
				else
				{
					result = string.Empty;
				}
			}
			return result;
		}

		// Token: 0x06007BC8 RID: 31688 RVA: 0x003982E8 File Offset: 0x003964E8
		private bool HeavenlyTreeFeedItemPassesFilter(ItemDisplayData d)
		{
			bool flag = d == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				sbyte itemType = d.RealKey.ItemType;
				sbyte b = itemType;
				if (b != 10)
				{
					result = (b == 12 && d.Amount >= 100);
				}
				else
				{
					List<short> readBookList = this._treeData.ReadBookList;
					bool hasRead = readBookList != null && readBookList.Contains(d.RealKey.TemplateId);
					List<short> availableBookList = this._defendHeavenlyTreeDisplayData.AvailableBookList;
					bool canRead = availableBookList != null && availableBookList.Contains(d.RealKey.TemplateId);
					result = (!hasRead && canRead);
				}
			}
			return result;
		}

		// Token: 0x06007BC9 RID: 31689 RVA: 0x0039838C File Offset: 0x0039658C
		private void ClearMultiplySelection()
		{
			bool flag = !this._multiplyReady;
			if (!flag)
			{
				this.multiplyItemListScroll.ClearMultiplySelection();
				this.OnMultiplyContentChanged(null);
			}
		}

		// Token: 0x06007BCA RID: 31690 RVA: 0x003983C0 File Offset: 0x003965C0
		private ItemDisplayData FindDisplayData(ItemKey key)
		{
			DefendHeavenlyTreeDisplayData defendHeavenlyTreeDisplayData = this._defendHeavenlyTreeDisplayData;
			bool flag = ((defendHeavenlyTreeDisplayData != null) ? defendHeavenlyTreeDisplayData.ResourceItemList : null) != null;
			if (flag)
			{
				foreach (ItemDisplayData d in this._defendHeavenlyTreeDisplayData.ResourceItemList)
				{
					bool flag2 = d.RealKey.Equals(key);
					if (flag2)
					{
						return d;
					}
				}
			}
			DefendHeavenlyTreeDisplayData defendHeavenlyTreeDisplayData2 = this._defendHeavenlyTreeDisplayData;
			bool flag3 = ((defendHeavenlyTreeDisplayData2 != null) ? defendHeavenlyTreeDisplayData2.BookItemList : null) != null;
			if (flag3)
			{
				foreach (ItemDisplayData d2 in this._defendHeavenlyTreeDisplayData.BookItemList)
				{
					bool flag4 = d2.RealKey.Equals(key);
					if (flag4)
					{
						return d2;
					}
				}
			}
			return null;
		}

		// Token: 0x06007BCB RID: 31691 RVA: 0x003984D4 File Offset: 0x003966D4
		private ItemDisplayData GetFeedInventoryItem(ItemKey key)
		{
			foreach (ItemDisplayData d in this._feedInventoryList)
			{
				bool flag = d.RealKey.Equals(key);
				if (flag)
				{
					return d;
				}
			}
			return this.FindDisplayData(key);
		}

		// Token: 0x06007BCC RID: 31692 RVA: 0x0039854C File Offset: 0x0039674C
		private int GetTotalSelectedBookUnitCount()
		{
			bool flag = !this._multiplyReady;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int i = 0;
				foreach (KeyValuePair<ItemDisplayData, int> kv in this.multiplyItemListScroll.SelectedMultiplyItemDict)
				{
					ItemDisplayData key = kv.Key;
					sbyte? b = (key != null) ? new sbyte?(key.RealKey.ItemType) : null;
					int? num = (b != null) ? new int?((int)b.GetValueOrDefault()) : null;
					int num2 = 10;
					bool flag2 = num.GetValueOrDefault() == num2 & num != null;
					if (flag2)
					{
						i += kv.Value;
					}
				}
				result = i;
			}
			return result;
		}

		// Token: 0x06007BCD RID: 31693 RVA: 0x00398634 File Offset: 0x00396834
		private void RefreshMultiplySelectionUi()
		{
			Dictionary<sbyte, int> resourceTotals = new Dictionary<sbyte, int>();
			bool multiplyReady = this._multiplyReady;
			if (multiplyReady)
			{
				foreach (KeyValuePair<ItemDisplayData, int> kv in this.multiplyItemListScroll.SelectedMultiplyItemDict)
				{
					ItemDisplayData data = this.FindDisplayData(kv.Key.RealKey);
					bool flag = data == null;
					if (!flag)
					{
						bool isResource = data.IsResource;
						if (isResource)
						{
							int sum;
							bool flag2 = !resourceTotals.TryGetValue(data.ResourceType, out sum);
							if (flag2)
							{
								sum = 0;
							}
							resourceTotals[data.ResourceType] = sum + kv.Value;
						}
					}
				}
			}
			for (int index = 0; index < this.resourceItems.Length; index++)
			{
				DefendHeavenlyTreeFeedResourceItem resourceItem = this.resourceItems[index];
				int amt;
				bool flag3 = resourceTotals.TryGetValue((sbyte)index, out amt);
				if (flag3)
				{
					resourceItem.Show((sbyte)index, amt);
				}
				else
				{
					resourceItem.Hide();
				}
			}
			int bookSlots = Mathf.Min(this.GetTotalSelectedBookUnitCount(), this.bookItems.Length);
			for (int i = 0; i < this.bookItems.Length; i++)
			{
				bool flag4 = i < bookSlots;
				if (flag4)
				{
					this.bookItems[i].Show();
				}
				else
				{
					this.bookItems[i].Hide();
				}
			}
			bool flag5 = this._treeData == null;
			if (!flag5)
			{
				bool flag6 = !this._multiplyReady || this.multiplyItemListScroll.SelectedMultiplyItemDict.Count == 0;
				if (flag6)
				{
					this.treeView.PreviewProgress((int)this._treeData.GrowPoint, 0);
				}
				else
				{
					int previewDelta = this.CalcSelectionPreviewGrowDelta(null);
					this.treeView.PreviewProgress((int)this._treeData.GrowPoint, previewDelta);
					this.FeedLogPreviewState("selection changed");
				}
			}
		}

		// Token: 0x06007BCE RID: 31694 RVA: 0x00398830 File Offset: 0x00396A30
		private List<ItemDisplayData> BuildFeedQueueFromSelection()
		{
			List<ItemDisplayData> list = new List<ItemDisplayData>();
			bool flag = !this._multiplyReady;
			List<ItemDisplayData> result;
			if (flag)
			{
				result = list;
			}
			else
			{
				foreach (KeyValuePair<ItemDisplayData, int> kv in from x in this.multiplyItemListScroll.SelectedMultiplyItemDict
				orderby x.Key.RealKey.TemplateId, x.Key.RealKey.ItemType
				select x)
				{
					ItemDisplayData src = this.FindDisplayData(kv.Key.RealKey);
					bool flag2 = src == null;
					if (!flag2)
					{
						bool flag3 = src.RealKey.ItemType == 10;
						if (flag3)
						{
							for (int i = 0; i < kv.Value; i++)
							{
								list.Add(src.Clone(1));
							}
						}
						else
						{
							list.Add(src.Clone(kv.Value));
						}
					}
				}
				result = list;
			}
			return result;
		}

		// Token: 0x06007BCF RID: 31695 RVA: 0x0039896C File Offset: 0x00396B6C
		private void SyncTreeDataFromDisplay()
		{
			DefendHeavenlyTreeDisplayData defendHeavenlyTreeDisplayData = this._defendHeavenlyTreeDisplayData;
			bool flag = ((defendHeavenlyTreeDisplayData != null) ? defendHeavenlyTreeDisplayData.HeavenlyTreeList : null) == null;
			if (!flag)
			{
				SectStoryHeavenlyTreeExtendable tree = this._defendHeavenlyTreeDisplayData.HeavenlyTreeList.Find((SectStoryHeavenlyTreeExtendable t) => t.Location == this._treeLocation);
				bool flag2 = tree == null;
				if (!flag2)
				{
					this._treeData = tree;
					this.treeView.SetData(this._treeData, this._enemyCount, this._villagerCount);
				}
			}
		}

		// Token: 0x06007BD0 RID: 31696 RVA: 0x003989E1 File Offset: 0x00396BE1
		private int GetFeedOperationCount()
		{
			return this.BuildFeedQueueFromSelection().Count;
		}

		// Token: 0x06007BD1 RID: 31697 RVA: 0x003989EE File Offset: 0x00396BEE
		private int GetTotalFeedActionPointCost()
		{
			return this.GetFeedOperationCount() * 100;
		}

		// Token: 0x06007BD2 RID: 31698 RVA: 0x003989FC File Offset: 0x00396BFC
		private bool HasEnoughActionPointForFeedQueue()
		{
			int remainActionPoint = SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth;
			return this.GetTotalFeedActionPointCost() <= remainActionPoint;
		}

		// Token: 0x06007BD3 RID: 31699 RVA: 0x00398A28 File Offset: 0x00396C28
		private void OnViewDefendHeavenlyTreeRefresh(ArgumentBox argumentBox)
		{
			int previousEnemyCount = this._enemyCount;
			ushort lastTreeGrowPoint = this._treeData.GrowPoint;
			argumentBox.Get<DefendHeavenlyTreeDisplayData>("displayData", out this._defendHeavenlyTreeDisplayData);
			argumentBox.Get("enemyCount", out this._enemyCount);
			argumentBox.Get("villagerCount", out this._villagerCount);
			this.RefreshAfterMultiplyReady();
			bool flag = this._treeData.GrowPoint > lastTreeGrowPoint;
			if (flag)
			{
				this.FeedLog(string.Format("[刷新事件] GrowPoint {0}→{1} ", lastTreeGrowPoint, this._treeData.GrowPoint) + string.Format("(Δ{0})", (int)(this._treeData.GrowPoint - lastTreeGrowPoint)));
				this.treeView.PlayEffectUpgrade();
			}
			bool flag2 = previousEnemyCount < this._enemyCount;
			if (flag2)
			{
				this._dialogQueue.Enqueue(ViewDefendHeavenlyTreeFeed.ETreeFeedDialogType.EnemyShowup);
			}
			this.TryExcuteDialog();
		}

		// Token: 0x06007BD4 RID: 31700 RVA: 0x00398B10 File Offset: 0x00396D10
		private void TryExcuteDialog()
		{
			bool flag = this._dialogQueue.Count == 0;
			if (!flag)
			{
				this._stringBuilding.Clear();
				ViewDefendHeavenlyTreeFeed.ETreeFeedDialogType dialogType = this._dialogQueue.Dequeue();
				DialogCmd cmd = new DialogCmd
				{
					Title = LanguageKey.LK_DefendHeavenlyTree_Feed.Tr()
				};
				ViewDefendHeavenlyTreeFeed.ETreeFeedDialogType etreeFeedDialogType = dialogType;
				ViewDefendHeavenlyTreeFeed.ETreeFeedDialogType etreeFeedDialogType2 = etreeFeedDialogType;
				if (etreeFeedDialogType2 == ViewDefendHeavenlyTreeFeed.ETreeFeedDialogType.EnemyShowup)
				{
					this._stringBuilding.AppendLine(LanguageKey.LK_Wudang_Feed_EnemyAppear.Tr());
					this._stringBuilding.AppendLine(string.Empty);
					this._stringBuilding.Append(this.GetHeavenlyTreeGrowStatusType(this._treeData.GrowPoint).SetColor("grey"));
					cmd.Content = this._stringBuilding.ToString();
					cmd.Type = 1;
					cmd.Yes = new Action(this.QuickHide);
					cmd.No = new Action(this.TryExcuteDialog);
					cmd.DialogType = EDialogType.None;
					cmd.GroupYesText = LanguageKey.LK_Wudang_Feed_ViewDetail.Tr();
					cmd.GroupNoText = LanguageKey.LK_Wudang_Feed_Ignore.Tr();
				}
				CommonUtils.ShowDialog(cmd);
			}
		}

		// Token: 0x06007BD5 RID: 31701 RVA: 0x00398C2C File Offset: 0x00396E2C
		private string GetHeavenlyTreeGrowStatusType(ushort growPoint)
		{
			int descIndex = (int)(GameData.Domains.World.SharedMethods.GetHeavenlyTreeTemplateIdByGrowValue(growPoint) - 598 + 1);
			string languageKey = string.Format("{0}{1}", "Event_SectStoryWudang_GrowDesc", descIndex);
			return LocalStringManager.Get(languageKey);
		}

		// Token: 0x06007BD6 RID: 31702 RVA: 0x00398C6C File Offset: 0x00396E6C
		private void RefreshAfterMultiplyReady()
		{
			this.EnsureMultiplyListInitialized();
			this._treeData = this._defendHeavenlyTreeDisplayData.HeavenlyTreeList.Find((SectStoryHeavenlyTreeExtendable t) => t.Location == this._treeLocation);
			this.treeView.SetData(this._treeData, this._enemyCount, this._villagerCount);
			this.RefreshWudangOrgTaskStatus();
			bool multiplyReady = this._multiplyReady;
			if (multiplyReady)
			{
				this.EnsureFeedScrollSortFilter(this.CurTogKey);
				this.multiplyItemListScroll.SetFilter(new Func<ItemDisplayData, bool>(this.PassesFeedListFilter));
				this.SyncInventoryListFromCurrentTab();
				this.ApplyFeedMultiplyModeForCurrentData();
				this.multiplyItemListScroll.RefreshItems();
				this.multiplyItemListScroll.CurMultiplyScrollView.ReRender();
			}
			this.RefreshButtonConfirm();
			bool flag = !this._feedChainActive && !this._eventTriggered;
			if (flag)
			{
				this.operationMask.SetActive(false);
			}
			List<short> availableBookList = this._defendHeavenlyTreeDisplayData.AvailableBookList;
			int availableBookCount = (availableBookList != null) ? availableBookList.Count : 0;
			List<short> readBookList = this._treeData.ReadBookList;
			int readBookCount = (readBookList != null) ? readBookList.Count : 0;
			bool canReadBook = availableBookCount > 0 && readBookCount < availableBookCount;
			this.tabToggleGroup.Get(1).interactable = canReadBook;
		}

		// Token: 0x06007BD7 RID: 31703 RVA: 0x00398DA4 File Offset: 0x00396FA4
		private void SyncInventoryListFromCurrentTab()
		{
			this._feedInventoryList.Clear();
			List<ItemDisplayData> source = (this.CurTogKey == ViewDefendHeavenlyTreeFeed.TogKey.Resource) ? this._defendHeavenlyTreeDisplayData.ResourceItemList : this._defendHeavenlyTreeDisplayData.BookItemList;
			bool flag = source != null;
			if (flag)
			{
				this._feedInventoryList.AddRange(source);
			}
			this.multiplyItemListScroll.SetItems(this._feedInventoryList);
		}

		// Token: 0x06007BD8 RID: 31704 RVA: 0x00398E08 File Offset: 0x00397008
		private void Refresh()
		{
			this.RefreshAfterMultiplyReady();
			bool multiplyReady = this._multiplyReady;
			if (multiplyReady)
			{
				this.RefreshItemList(this.CurTogKey);
			}
		}

		// Token: 0x06007BD9 RID: 31705 RVA: 0x00398E34 File Offset: 0x00397034
		private void RefreshItemList(ViewDefendHeavenlyTreeFeed.TogKey togKey)
		{
			bool flag = !this._multiplyReady;
			if (!flag)
			{
				bool isMultiItemSelect = this.multiplyItemListScroll.IsMultiItemSelect;
				if (isMultiItemSelect)
				{
					this.ClearMultiplySelection();
				}
				if (togKey != ViewDefendHeavenlyTreeFeed.TogKey.Resource)
				{
					if (togKey != ViewDefendHeavenlyTreeFeed.TogKey.Book)
					{
						throw new ArgumentOutOfRangeException("togKey", togKey, null);
					}
					this.tabToggleGroup.SetWithoutNotify(1);
				}
				else
				{
					this.tabToggleGroup.SetWithoutNotify(0);
				}
				this.EnsureFeedScrollSortFilter(togKey);
				this.SyncInventoryListFromCurrentTab();
				this.multiplyItemListScroll.SetFilter(new Func<ItemDisplayData, bool>(this.PassesFeedListFilter));
				this.multiplyItemListScroll.RefreshItems();
			}
		}

		// Token: 0x06007BDA RID: 31706 RVA: 0x00398EDC File Offset: 0x003970DC
		private void RefreshButtonConfirm()
		{
			int remainActionPoint = SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth;
			int remainDays = remainActionPoint / 10;
			int costPerFeed = 100;
			bool hasSelection = this._multiplyReady && this.multiplyItemListScroll.SelectedMultiplyItemDict.Count > 0;
			int totalCostActionPoint = hasSelection ? this.GetTotalFeedActionPointCost() : costPerFeed;
			int totalCostDays = totalCostActionPoint / 10;
			bool isTimeMeet = hasSelection ? this.HasEnoughActionPointForFeedQueue() : (costPerFeed <= remainActionPoint);
			this.buttonConfirm.interactable = (isTimeMeet && hasSelection && !this._treeData.IsGrowPointMax);
			this.tipButtonConfirm.enabled = !this.buttonConfirm.interactable;
			bool isGrowPointMax = this._treeData.IsGrowPointMax;
			if (isGrowPointMax)
			{
				this.tipButtonConfirm.PresetParam = new string[]
				{
					LanguageKey.LK_DefendHeavenlyTree_Feed_IsMax.Tr().SetColor("brightred")
				};
			}
			else
			{
				bool flag = !hasSelection;
				if (flag)
				{
					this.tipButtonConfirm.PresetParam = new string[]
					{
						LanguageKey.LK_Multiply_Select_None_Tip.Tr().ColorReplace()
					};
				}
				else
				{
					bool flag2 = !isTimeMeet;
					if (flag2)
					{
						this.tipButtonConfirm.PresetParam = new string[]
						{
							LanguageKey.LK_DefendHeavenlyTree_Feed_NotEnoughTime.Tr().SetColor("brightred")
						};
					}
				}
			}
			string color = isTimeMeet ? "brightblue" : "brightred";
			this.textCost.text = string.Format("{0}/{1}", remainDays.ToString().SetColor(color), totalCostDays);
		}

		// Token: 0x06007BDB RID: 31707 RVA: 0x0039905C File Offset: 0x0039725C
		private void OnClickConfirm()
		{
			bool flag = !this._multiplyReady || this.multiplyItemListScroll.SelectedMultiplyItemDict.Count == 0;
			if (flag)
			{
				Debug.LogError("Multiply selection is empty");
			}
			else
			{
				List<ItemDisplayData> queue = this.BuildFeedQueueFromSelection();
				bool flag2 = queue.Count == 0;
				if (!flag2)
				{
					bool flag3 = !this.HasEnoughActionPointForFeedQueue();
					if (flag3)
					{
						this.RefreshButtonConfirm();
					}
					else
					{
						this._selectedFeedItem = queue[0];
						this.operationMask.SetActive(true);
						float delayTime = 0f;
						HashSet<sbyte> playedResourceTypes = new HashSet<sbyte>();
						foreach (KeyValuePair<ItemDisplayData, int> kv in this.multiplyItemListScroll.SelectedMultiplyItemDict)
						{
							ItemDisplayData data = this.FindDisplayData(kv.Key.RealKey);
							bool flag4 = data == null || !data.IsResource;
							if (!flag4)
							{
								bool flag5 = !playedResourceTypes.Add(data.ResourceType);
								if (!flag5)
								{
									delayTime = Mathf.Max(delayTime, this.resourceItems[(int)data.ResourceType].PlayEffectDisappear(this.treeView.transform.position));
								}
							}
						}
						int bookUnitCount = this.GetTotalSelectedBookUnitCount();
						bool flag6 = bookUnitCount > 0;
						if (flag6)
						{
							int playCount = Mathf.Min(bookUnitCount, this.bookItems.Length);
							for (int i = 0; i < playCount; i++)
							{
								delayTime = Mathf.Max(delayTime, this.bookItems[i].PlayEffectDisappear(this.treeView.transform.position));
							}
						}
						this.BeginFeedChain(queue);
						SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(delayTime, delegate
						{
							this.RefreshDisplayThenFeedAt(0, queue, this._feedChainGeneration);
						});
					}
				}
			}
		}

		// Token: 0x06007BDC RID: 31708 RVA: 0x00399258 File Offset: 0x00397458
		private void BeginFeedChain(IReadOnlyList<ItemDisplayData> queue)
		{
			this._feedChainGeneration++;
			this._feedChainActive = true;
			this._activeFeedQueue = queue;
			this.RefreshWudangOrgTaskStatus();
			SectStoryHeavenlyTreeExtendable treeData = this._treeData;
			this._feedChainStartGrowPoint = (int)((treeData != null) ? treeData.GrowPoint : 0);
			this._feedChainPreviewTotalDelta = this.CalcSelectionPreviewGrowDelta(null);
			this.FeedLog(string.Format("===== 投喂链开始 gen={0} queue={1} ", this._feedChainGeneration, queue.Count) + string.Format("startGrow={0} previewDelta={1} ", this._feedChainStartGrowPoint, this._feedChainPreviewTotalDelta) + string.Format("previewEnd={0}/{1} ", this._feedChainStartGrowPoint + this._feedChainPreviewTotalDelta, ViewDefendHeavenlyTreeFeed.GetGrowthThreshold()) + string.Format("orgTaskStatus={0} clientAP={1}", this._wudangOrgTaskStatus, SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth));
			this.LogFeedQueue(queue);
			this.FeedLogPreviewState("confirm");
		}

		// Token: 0x06007BDD RID: 31709 RVA: 0x00399358 File Offset: 0x00397558
		private void RefreshDisplayThenFeedAt(int index, IReadOnlyList<ItemDisplayData> queue, int chainGeneration)
		{
			bool flag = !this.IsFeedChainCallbackValid(chainGeneration);
			if (!flag)
			{
				bool flag2 = index >= queue.Count;
				if (flag2)
				{
					this.CompleteFeedChain(chainGeneration, "index>=count");
				}
				else
				{
					this.FeedLog(string.Format("→ GetDefendHeavenlyTreeDisplayData  before feed [{0}/{1}]", index, queue.Count));
					this.RequestDefendHeavenlyTreeDisplayData(chainGeneration, delegate
					{
						bool flag3 = !this.IsFeedChainCallbackValid(chainGeneration);
						if (!flag3)
						{
							ViewDefendHeavenlyTreeFeed <>4__this = this;
							string format = "  display ok before feed [{0}] growPoint={1} treeId={2} ";
							object arg = index;
							SectStoryHeavenlyTreeExtendable treeData = this._treeData;
							object arg2 = (treeData != null) ? new ushort?(treeData.GrowPoint) : null;
							SectStoryHeavenlyTreeExtendable treeData2 = this._treeData;
							string str = string.Format(format, arg, arg2, (treeData2 != null) ? new int?(treeData2.Id) : null);
							string format2 = "isMax={0}";
							SectStoryHeavenlyTreeExtendable treeData3 = this._treeData;
							<>4__this.FeedLog(str + string.Format(format2, (treeData3 != null) ? new bool?(treeData3.IsGrowPointMax) : null));
							this.ExecuteSingleFeed(index, queue, chainGeneration);
						}
					});
				}
			}
		}

		// Token: 0x06007BDE RID: 31710 RVA: 0x00399414 File Offset: 0x00397614
		private void ExecuteSingleFeed(int index, IReadOnlyList<ItemDisplayData> queue, int chainGeneration)
		{
			bool flag = !this.IsFeedChainCallbackValid(chainGeneration);
			if (!flag)
			{
				bool flag2 = index >= queue.Count;
				if (flag2)
				{
					this.CompleteFeedChain(chainGeneration, "execute:index>=count");
				}
				else
				{
					int remainActionPoint = SingletonObject.getInstance<BasicGameData>().ActionPointCurrMonth;
					bool flag3 = remainActionPoint < 100;
					if (flag3)
					{
						this.FeedLog(string.Format("× 行动力不足中止 step={0} clientAP={1} need={2}", index, remainActionPoint, 100));
						this.AbortFeedChainDueToInsufficientActionPoint("insufficient AP before feed");
					}
					else
					{
						bool flag4 = this._treeData == null;
						if (flag4)
						{
							this.FeedLog(string.Format("× 找不到神木 location={0}，中止 step={1}", this._treeLocation, index));
							this.AbortFeedChainFatal(chainGeneration, "tree missing on display");
						}
						else
						{
							ItemDisplayData item = queue[index];
							this._selectedFeedItem = item;
							ushort growBeforeFeed = this._treeData.GrowPoint;
							int expectedDelta = this.CalcItemFeedGrowDelta(item);
							this.FeedLog(string.Format("→ DefendHeavenlyTreeFeed [{0}/{1}] treeId={2} ", index, queue.Count, this._treeData.Id) + string.Format("{0} growBefore={1} expectedDelta={2} ", ViewDefendHeavenlyTreeFeed.FormatFeedItem(item), growBeforeFeed, expectedDelta) + string.Format("(clientAP={0})", remainActionPoint));
							Action <>9__1;
							StoryDomainMethod.AsyncCall.DefendHeavenlyTreeFeed(this, this._treeData.Id, item, delegate(int offset, RawDataPool pool)
							{
								bool flag5 = !this.IsFeedChainCallbackValid(chainGeneration);
								if (!flag5)
								{
									Serializer.Deserialize(pool, offset, ref this._eventTriggered);
									bool eventTriggered = this._eventTriggered;
									if (eventTriggered)
									{
										this.FeedLog(string.Format("  ※ eventTriggered step={0}/{1}，关闭刷怪 UI 后继续下一项 ", index, queue.Count) + string.Format("growBefore={0} expectedStep={1}", growBeforeFeed, expectedDelta));
										TaiwuEventDomainMethod.Call.CloseUI("CreateRandomEnemyAroundHeavenlyTree");
										this._eventTriggered = false;
									}
									this.FeedLog(string.Format("← DefendHeavenlyTreeFeed ok [{0}]，拉取 display 核对成长…", index));
									ViewDefendHeavenlyTreeFeed <>4__this = this;
									int chainGeneration2 = chainGeneration;
									Action onSynced;
									if ((onSynced = <>9__1) == null)
									{
										onSynced = (<>9__1 = delegate()
										{
											bool flag6 = !this.IsFeedChainCallbackValid(chainGeneration);
											if (!flag6)
											{
												SectStoryHeavenlyTreeExtendable treeData = this._treeData;
												ushort growAfterFeed = (treeData != null) ? treeData.GrowPoint : growBeforeFeed;
												int actualDelta = (int)(growAfterFeed - growBeforeFeed);
												this.LogFeedStepResult(index, queue.Count, item, (int)growBeforeFeed, expectedDelta, actualDelta);
												bool flag7 = this._treeData != null && this._treeData.IsGrowPointMax;
												if (flag7)
												{
													this.FeedLog(string.Format("  ※ 成长已满，跳过剩余 {0} 项", queue.Count - index - 1));
													this.CompleteFeedChain(chainGeneration, "grow max after feed");
												}
												else
												{
													this.RefreshDisplayThenFeedAt(index + 1, queue, chainGeneration);
												}
											}
										});
									}
									<>4__this.RequestDefendHeavenlyTreeDisplayData(chainGeneration2, onSynced);
								}
							});
						}
					}
				}
			}
		}

		// Token: 0x06007BDF RID: 31711 RVA: 0x00399614 File Offset: 0x00397814
		private void RequestDefendHeavenlyTreeDisplayData(int chainGeneration, Action onSynced)
		{
			StoryDomainMethod.AsyncCall.GetDefendHeavenlyTreeDisplayData(this, true, this._treeLocation, new List<Location>
			{
				this._treeLocation
			}, delegate(int offset, RawDataPool pool)
			{
				bool flag = !this.IsFeedChainCallbackValid(chainGeneration);
				if (!flag)
				{
					Serializer.Deserialize(pool, offset, ref this._defendHeavenlyTreeDisplayData);
					this.SyncTreeDataFromDisplay();
					Action onSynced2 = onSynced;
					if (onSynced2 != null)
					{
						onSynced2();
					}
				}
			});
		}

		// Token: 0x06007BE0 RID: 31712 RVA: 0x0039966C File Offset: 0x0039786C
		private void CompleteFeedChain(int chainGeneration, string reason)
		{
			bool flag = !this.IsFeedChainCallbackValid(chainGeneration);
			if (!flag)
			{
				SectStoryHeavenlyTreeExtendable treeData = this._treeData;
				ushort actualGrow = (treeData != null) ? treeData.GrowPoint : 0;
				int actualTotalDelta = (int)actualGrow - this._feedChainStartGrowPoint;
				this.FeedLog(string.Concat(new string[]
				{
					string.Format("===== 投喂链结束 gen={0} reason={1} ", chainGeneration, reason),
					string.Format("startGrow={0} endGrow={1} ", this._feedChainStartGrowPoint, actualGrow),
					string.Format("actualTotalDelta={0} previewTotalDelta={1} ", actualTotalDelta, this._feedChainPreviewTotalDelta),
					string.Format("diff(end)={0} ", this._feedChainStartGrowPoint + this._feedChainPreviewTotalDelta - (int)actualGrow),
					"(正数=预览偏高)"
				}));
				bool flag2 = actualTotalDelta != this._feedChainPreviewTotalDelta;
				if (flag2)
				{
					this.FeedLog("  ※ 累计成长与预览不一致：检查 game-data 公式/门派任务状态是否与客户端 GetHeavenlyTreeGrewUpValue* 一致，或是否中途成长已满截断");
				}
				this._eventTriggered = false;
				this._feedChainActive = false;
				this.operationMask.SetActive(false);
				this.ClearMultiplySelection();
				this.multiplyItemListScroll.CurMultiplyScrollView.ReRender();
				Action onConfirm = this._onConfirm;
				if (onConfirm != null)
				{
					onConfirm();
				}
			}
		}

		// Token: 0x06007BE1 RID: 31713 RVA: 0x00399798 File Offset: 0x00397998
		private void AbortFeedChainDueToInsufficientActionPoint(string reason)
		{
			SectStoryHeavenlyTreeExtendable treeData = this._treeData;
			ushort actualGrow = (treeData != null) ? treeData.GrowPoint : 0;
			this.FeedLog(string.Concat(new string[]
			{
				"===== 投喂链中止（行动力） reason=",
				reason,
				" ",
				string.Format("startGrow={0} nowGrow={1} ", this._feedChainStartGrowPoint, actualGrow),
				string.Format("previewTotalDelta={0}（已喂部分不会回滚）", this._feedChainPreviewTotalDelta)
			}));
			this._feedChainActive = false;
			this.operationMask.SetActive(false);
			this.ClearMultiplySelection();
			this.multiplyItemListScroll.CurMultiplyScrollView.ReRender();
			this.RefreshAfterMultiplyReady();
		}

		// Token: 0x06007BE2 RID: 31714 RVA: 0x00399848 File Offset: 0x00397A48
		private void AbortFeedChainFatal(int chainGeneration, string reason)
		{
			this.FeedLog("===== 投喂链异常中止 reason=" + reason);
			this._feedChainActive = false;
			this.operationMask.SetActive(false);
			this.ClearMultiplySelection();
			this.multiplyItemListScroll.CurMultiplyScrollView.ReRender();
			Action onConfirm = this._onConfirm;
			if (onConfirm != null)
			{
				onConfirm();
			}
		}

		// Token: 0x06007BE3 RID: 31715 RVA: 0x003998A6 File Offset: 0x00397AA6
		private bool IsFeedChainCallbackValid(int chainGeneration)
		{
			return this._feedChainActive && chainGeneration == this._feedChainGeneration;
		}

		// Token: 0x06007BE4 RID: 31716 RVA: 0x003998BC File Offset: 0x00397ABC
		private void LogFeedStepResult(int index, int queueCount, ItemDisplayData item, int growBeforeFeed, int expectedDelta, int actualDelta)
		{
			bool flag = expectedDelta == actualDelta;
			if (flag)
			{
				this.FeedLog(string.Format("  ✓ step[{0}/{1}] {2} Δgrow={3} (与预览一致)", new object[]
				{
					index,
					queueCount,
					ViewDefendHeavenlyTreeFeed.FormatFeedItem(item),
					actualDelta
				}));
			}
			else
			{
				string str = string.Format("  ✗ step[{0}/{1}] {2} ", index, queueCount, ViewDefendHeavenlyTreeFeed.FormatFeedItem(item));
				string format = "grow {0}→{1} ";
				object arg = growBeforeFeed;
				SectStoryHeavenlyTreeExtendable treeData = this._treeData;
				this.FeedLog(str + string.Format(format, arg, (treeData != null) ? new ushort?(treeData.GrowPoint) : null) + string.Format("expectedΔ={0} actualΔ={1} diff={2} ", expectedDelta, actualDelta, expectedDelta - actualDelta) + "(正数=预览高于实际)");
				this.FeedLog("    可能原因: ①客户端门派状态与 EventHelper 不一致 ②成长已满被截断 ③treeId 无效未加成长 ④资源数量与队列快照不一致导致服务端扣量不同");
			}
		}

		// Token: 0x06007BE5 RID: 31717 RVA: 0x003999A8 File Offset: 0x00397BA8
		private void FeedLogPreviewState(string context)
		{
			bool flag = !this.enableFeedChainDebugLog || this._treeData == null;
			if (!flag)
			{
				this.RefreshWudangOrgTaskStatus();
				int previewDelta = this.CalcSelectionPreviewGrowDelta(null);
				this.FeedLog(string.Format("[预览] {0} curGrow={1} previewΔ={2} ", context, this._treeData.GrowPoint, previewDelta) + string.Format("→{0} ", Math.Min(ViewDefendHeavenlyTreeFeed.GetGrowthThreshold(), (int)this._treeData.GrowPoint + previewDelta)) + string.Format("feedOps={0} orgTaskStatus={1} ", this.GetFeedOperationCount(), this._wudangOrgTaskStatus) + string.Format("entries={0}", this.GetSelectedEntryCount()));
			}
		}

		// Token: 0x06007BE6 RID: 31718 RVA: 0x00399A6C File Offset: 0x00397C6C
		private void LogFeedQueue(IReadOnlyList<ItemDisplayData> queue)
		{
			bool flag = !this.enableFeedChainDebugLog;
			if (!flag)
			{
				for (int i = 0; i < queue.Count; i++)
				{
					ItemDisplayData item = queue[i];
					this.FeedLog(string.Format("  queue[{0}] {1} expectedΔ={2}", i, ViewDefendHeavenlyTreeFeed.FormatFeedItem(item), this.CalcItemFeedGrowDelta(item)));
				}
			}
		}

		// Token: 0x06007BE7 RID: 31719 RVA: 0x00399AD4 File Offset: 0x00397CD4
		private static string FormatFeedItem(ItemDisplayData item)
		{
			bool flag = item == null;
			string result;
			if (flag)
			{
				result = "null";
			}
			else
			{
				bool isResource = item.IsResource;
				if (isResource)
				{
					result = string.Format("Resource type={0} amount={1}", item.ResourceType, item.Amount);
				}
				else
				{
					bool flag2 = item.RealKey.ItemType == 10;
					if (flag2)
					{
						result = string.Format("SkillBook tpl={0} amount={1}", item.RealKey.TemplateId, item.Amount);
					}
					else
					{
						result = string.Format("{0} tpl={1} amount={2}", item.RealKey.ItemType, item.RealKey.TemplateId, item.Amount);
					}
				}
			}
			return result;
		}

		// Token: 0x06007BE8 RID: 31720 RVA: 0x00399B98 File Offset: 0x00397D98
		private void FeedLog(string message)
		{
			bool flag = !this.enableFeedChainDebugLog;
			if (!flag)
			{
				Debug.Log("[DefendHeavenlyTreeFeed] " + message, this);
			}
		}

		// Token: 0x04005DEE RID: 24046
		[SerializeField]
		private DefendHeavenlyTreeView treeView;

		// Token: 0x04005DEF RID: 24047
		[SerializeField]
		private CButton buttonConfirm;

		// Token: 0x04005DF0 RID: 24048
		[SerializeField]
		private TooltipInvoker tipButtonConfirm;

		// Token: 0x04005DF1 RID: 24049
		[SerializeField]
		private CToggleGroup tabToggleGroup;

		// Token: 0x04005DF2 RID: 24050
		[SerializeField]
		private SimpleMultiplyItemListScroll multiplyItemListScroll;

		// Token: 0x04005DF3 RID: 24051
		[SerializeField]
		private CToggle switchMultiplyMode;

		// Token: 0x04005DF4 RID: 24052
		[SerializeField]
		private TextMeshProUGUI switchMultiplyModeLabel;

		// Token: 0x04005DF5 RID: 24053
		[SerializeField]
		private CButton buttonSelectAll;

		// Token: 0x04005DF6 RID: 24054
		[SerializeField]
		private CButton buttonClearMultiplySelection;

		// Token: 0x04005DF7 RID: 24055
		[SerializeField]
		private TMP_InputField searchField;

		// Token: 0x04005DF8 RID: 24056
		[SerializeField]
		private TextMeshProUGUI textCost;

		// Token: 0x04005DF9 RID: 24057
		[SerializeField]
		private GameObject operationMask;

		// Token: 0x04005DFA RID: 24058
		[SerializeField]
		private DefendHeavenlyTreeFeedResourceItem[] resourceItems;

		// Token: 0x04005DFB RID: 24059
		[SerializeField]
		private DefendHeavenlyTreeFeedBookItem[] bookItems;

		// Token: 0x04005DFC RID: 24060
		[Header("调试")]
		[SerializeField]
		private bool enableFeedChainDebugLog;

		// Token: 0x04005DFD RID: 24061
		private Location _treeLocation;

		// Token: 0x04005DFE RID: 24062
		private int _enemyCount;

		// Token: 0x04005DFF RID: 24063
		private int _villagerCount;

		// Token: 0x04005E00 RID: 24064
		private Action _onConfirm;

		// Token: 0x04005E01 RID: 24065
		private DefendHeavenlyTreeDisplayData _defendHeavenlyTreeDisplayData;

		// Token: 0x04005E02 RID: 24066
		private SectStoryHeavenlyTreeExtendable _treeData;

		// Token: 0x04005E03 RID: 24067
		private bool _eventTriggered;

		// Token: 0x04005E04 RID: 24068
		private readonly List<ItemDisplayData> _feedInventoryList = new List<ItemDisplayData>();

		// Token: 0x04005E05 RID: 24069
		private string _searchText;

		// Token: 0x04005E06 RID: 24070
		private bool _multiplyReady;

		// Token: 0x04005E07 RID: 24071
		private bool _suppressMultiplySwitchEvent;

		// Token: 0x04005E08 RID: 24072
		private bool _ensureMultiplyOnNextReady;

		// Token: 0x04005E09 RID: 24073
		private ItemDisplayData _selectedFeedItem;

		// Token: 0x04005E0A RID: 24074
		private Queue<ViewDefendHeavenlyTreeFeed.ETreeFeedDialogType> _dialogQueue = new Queue<ViewDefendHeavenlyTreeFeed.ETreeFeedDialogType>();

		// Token: 0x04005E0B RID: 24075
		private StringBuilder _stringBuilding = new StringBuilder();

		// Token: 0x04005E0C RID: 24076
		private const string GROW_DESC_LSTRING_NAME = "Event_SectStoryWudang_GrowDesc";

		// Token: 0x04005E0D RID: 24077
		private const string FEED_RESOURCE_SORT_SAVE_KEY = "ViewDefendHeavenlyTreeFeedResource";

		// Token: 0x04005E0E RID: 24078
		private const string FEED_BOOK_SORT_SAVE_KEY = "ViewDefendHeavenlyTreeFeedBook";

		// Token: 0x04005E0F RID: 24079
		private const BindingFlags ItemListNonPublic = BindingFlags.Instance | BindingFlags.NonPublic;

		// Token: 0x04005E10 RID: 24080
		private sbyte _wudangOrgTaskStatus;

		// Token: 0x04005E11 RID: 24081
		private bool _feedChainActive;

		// Token: 0x04005E12 RID: 24082
		private int _feedChainGeneration;

		// Token: 0x04005E13 RID: 24083
		private IReadOnlyList<ItemDisplayData> _activeFeedQueue;

		// Token: 0x04005E14 RID: 24084
		private int _feedChainStartGrowPoint;

		// Token: 0x04005E15 RID: 24085
		private int _feedChainPreviewTotalDelta;

		// Token: 0x02001F60 RID: 8032
		private enum TogKey
		{
			// Token: 0x0400CD5B RID: 52571
			Resource,
			// Token: 0x0400CD5C RID: 52572
			Book
		}

		// Token: 0x02001F61 RID: 8033
		private sealed class FeedBookSortAndFilterController : ItemSortAndFilterController
		{
			// Token: 0x0600F3C2 RID: 62402 RVA: 0x0061E5B4 File Offset: 0x0061C7B4
			public FeedBookSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.LK_Reading_Select_Book_FilterTitle)
			{
			}

			// Token: 0x0600F3C3 RID: 62403 RVA: 0x0061E5C4 File Offset: 0x0061C7C4
			protected override IEnumerable<FilterLineBase<ITradeableContent>> GenerateFilterLines()
			{
				yield return new BookRootFilterLine();
				yield return new AllBookDetailedFilterLine();
				yield return new CombatSkillBookDetailedFilterLine();
				yield return new LifeSkillBookDetailedFilterLine();
				yield break;
			}
		}

		// Token: 0x02001F62 RID: 8034
		private enum ETreeFeedDialogType
		{
			// Token: 0x0400CD5E RID: 52574
			EnemyShowup
		}
	}
}
