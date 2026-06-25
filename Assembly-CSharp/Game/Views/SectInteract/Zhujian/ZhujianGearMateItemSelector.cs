using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Item;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item.Apply;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009C9 RID: 2505
	public class ZhujianGearMateItemSelector : MonoBehaviour
	{
		// Token: 0x17000D7B RID: 3451
		// (get) Token: 0x06007979 RID: 31097 RVA: 0x0038736F File Offset: 0x0038556F
		private ItemListScroll ItemList
		{
			get
			{
				return (this.multiplyItemListScroll != null) ? this.multiplyItemListScroll.CurMultiplyScrollView : null;
			}
		}

		// Token: 0x1400007D RID: 125
		// (add) Token: 0x0600797A RID: 31098 RVA: 0x00387390 File Offset: 0x00385590
		// (remove) Token: 0x0600797B RID: 31099 RVA: 0x003873C8 File Offset: 0x003855C8
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnSelectionChanged;

		// Token: 0x1400007E RID: 126
		// (add) Token: 0x0600797C RID: 31100 RVA: 0x00387400 File Offset: 0x00385600
		// (remove) Token: 0x0600797D RID: 31101 RVA: 0x00387438 File Offset: 0x00385638
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int> OnSourceTypeChanged;

		// Token: 0x1400007F RID: 127
		// (add) Token: 0x0600797E RID: 31102 RVA: 0x00387470 File Offset: 0x00385670
		// (remove) Token: 0x0600797F RID: 31103 RVA: 0x003874A8 File Offset: 0x003856A8
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ItemKey> OnItemSelected;

		// Token: 0x14000080 RID: 128
		// (add) Token: 0x06007980 RID: 31104 RVA: 0x003874E0 File Offset: 0x003856E0
		// (remove) Token: 0x06007981 RID: 31105 RVA: 0x00387518 File Offset: 0x00385718
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnFilterManuallyChanged;

		// Token: 0x17000D7C RID: 3452
		// (get) Token: 0x06007982 RID: 31106 RVA: 0x00387550 File Offset: 0x00385750
		public Dictionary<ItemKey, int> SelectedItems
		{
			get
			{
				Dictionary<ItemKey, int> merged = new Dictionary<ItemKey, int>();
				foreach (Dictionary<ItemKey, int> sourceItems in this._selectedItemsBySource.Values)
				{
					foreach (KeyValuePair<ItemKey, int> kvp in sourceItems)
					{
						bool flag = merged.ContainsKey(kvp.Key);
						if (flag)
						{
							Dictionary<ItemKey, int> dictionary = merged;
							ItemKey key = kvp.Key;
							dictionary[key] += kvp.Value;
						}
						else
						{
							merged[kvp.Key] = kvp.Value;
						}
					}
				}
				return merged;
			}
		}

		// Token: 0x06007983 RID: 31107 RVA: 0x00387648 File Offset: 0x00385848
		[return: TupleElementNames(new string[]
		{
			"key",
			"count",
			"source"
		})]
		public IEnumerable<ValueTuple<ItemKey, int, ItemSourceType>> GetAllSelectedItemsWithSource()
		{
			foreach (KeyValuePair<int, Dictionary<ItemKey, int>> keyValuePair in this._selectedItemsBySource)
			{
				int num;
				Dictionary<ItemKey, int> dictionary;
				keyValuePair.Deconstruct(out num, out dictionary);
				int source = num;
				Dictionary<ItemKey, int> items = dictionary;
				foreach (KeyValuePair<ItemKey, int> kvp in items)
				{
					yield return new ValueTuple<ItemKey, int, ItemSourceType>(kvp.Key, kvp.Value, (ItemSourceType)source);
					kvp = default(KeyValuePair<ItemKey, int>);
				}
				Dictionary<ItemKey, int>.Enumerator enumerator2 = default(Dictionary<ItemKey, int>.Enumerator);
				items = null;
			}
			Dictionary<int, Dictionary<ItemKey, int>>.Enumerator enumerator = default(Dictionary<int, Dictionary<ItemKey, int>>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06007984 RID: 31108 RVA: 0x00387658 File Offset: 0x00385858
		private Dictionary<ItemKey, int> GetCurrentSelectedItems()
		{
			int currentSource = ZhujianGearMateItemSelector.ResolveSourceType(this.sourceToggleGroup.GetActiveIndex());
			Dictionary<ItemKey, int> items;
			bool flag = !this._selectedItemsBySource.TryGetValue(currentSource, out items);
			if (flag)
			{
				items = new Dictionary<ItemKey, int>();
				this._selectedItemsBySource[currentSource] = items;
			}
			return items;
		}

		// Token: 0x06007985 RID: 31109 RVA: 0x003876A7 File Offset: 0x003858A7
		public void SetSortControllerFactory(Func<ISortAndFilterView, SortAndFilterController<ITradeableContent>> factory)
		{
			this._sortControllerFactory = factory;
		}

		// Token: 0x06007986 RID: 31110 RVA: 0x003876B4 File Offset: 0x003858B4
		public void SetTitle(string key)
		{
			bool flag = this.titleText != null;
			if (flag)
			{
				this.titleText.text = LocalStringManager.Get(key);
			}
		}

		// Token: 0x06007987 RID: 31111 RVA: 0x003876E4 File Offset: 0x003858E4
		public void SetBlocked(bool blocked)
		{
			this._blocked = blocked;
			this.blockOverlay.SetActive(blocked);
			bool multiplyReady = this._multiplyReady;
			if (multiplyReady)
			{
				this.multiplyItemListScroll.RefreshMultiplyOperationChrome();
			}
		}

		// Token: 0x06007988 RID: 31112 RVA: 0x0038771C File Offset: 0x0038591C
		public void Init()
		{
			this.EnsureMultiplyItemListScrollReference();
			bool flag = this.multiplyItemListScroll != null;
			if (flag)
			{
				this.multiplyItemListScroll.SetupItemListScroll(new ManagedMultiplyItemListScrollSetup
				{
					MainSortSaveKey = "GearMateAttribute",
					SelectedSortSaveKey = "GearMateAttribute_Selected",
					SortType = ESortAndFilterControllerType.Item,
					EnableRowInteraction = true,
					OnRender = new Action<ITradeableContent, RowItemLine>(this.OnItemRender),
					OnClick = new Action<ITradeableContent, RowItemLine>(this.OnItemClick),
					ColumnType = (ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount),
					AmountGenerator = new Func<ITradeableContent, string>(this.AmountCellDataGenerator),
					OnMainSortAndFilterChanged = new Action(this.OnItemListSortAndFilterChanged)
				});
			}
			bool flag2 = this._sortControllerFactory != null && this.ItemList != null;
			if (flag2)
			{
				ISortAndFilterView sortAndFilterView = this.GetSortAndFilterView();
				bool flag3 = sortAndFilterView != null;
				if (flag3)
				{
					this.ReplaceItemListSortController(this._sortControllerFactory(sortAndFilterView));
				}
			}
			this.InitMultiplyItemListScroll();
			this.sourceToggleGroup.Init(-1);
			this.sourceToggleGroup.OnActiveIndexChange += this.OnSourceToggleChanged;
		}

		// Token: 0x06007989 RID: 31113 RVA: 0x00387836 File Offset: 0x00385A36
		private void OnDestroy()
		{
			this.sourceToggleGroup.OnActiveIndexChange -= this.OnSourceToggleChanged;
		}

		// Token: 0x0600798A RID: 31114 RVA: 0x00387854 File Offset: 0x00385A54
		private void EnsureMultiplyItemListScrollReference()
		{
			bool flag = this.multiplyItemListScroll != null;
			if (!flag)
			{
				this.multiplyItemListScroll = base.GetComponentInChildren<ManagedMultiplyItemListScroll>(true);
			}
		}

		// Token: 0x0600798B RID: 31115 RVA: 0x00387884 File Offset: 0x00385A84
		private void InitMultiplyItemListScroll()
		{
			bool flag = this.multiplyItemListScroll == null;
			if (!flag)
			{
				bool flag2 = !this.multiplyItemListScroll.HasInit;
				if (flag2)
				{
					this.multiplyItemListScroll.Init(new Action<List<ValueTuple<ItemDisplayData, int>>>(this.OnMultiplyContentChanged));
				}
				this.multiplyItemListScroll.CanSelectItemPredicate = new Func<ItemDisplayData, bool>(this.CanSelectMultiplyItem);
				this.multiplyItemListScroll.GetSelectLimitOverride = new Func<ItemDisplayData, int, int>(this.GetMultiplySelectLimit);
				this.multiplyItemListScroll.GetSelectCountLimitTip = delegate(ItemDisplayData item)
				{
					Func<ItemDisplayData, string> getDisabledItemTip = this.GetDisabledItemTip;
					return ((getDisabledItemTip != null) ? getDisabledItemTip(item) : null) ?? string.Empty;
				};
				this.multiplyItemListScroll.CanOperateSelection = (() => !this._blocked);
				bool flag3 = !this.multiplyItemListScroll.IsMultiItemSelect;
				if (flag3)
				{
					this.multiplyItemListScroll.EnterMultiplyMode(false);
				}
				this._multiplyReady = true;
				this.SyncSelectedListPanel();
				this.multiplyItemListScroll.RefreshListAmountDisplay();
			}
		}

		// Token: 0x0600798C RID: 31116 RVA: 0x0038796C File Offset: 0x00385B6C
		public void SetSortAndFilterVisible(bool value)
		{
			Component sortAndFilter = this.GetSortAndFilterView() as Component;
			bool flag = sortAndFilter != null;
			if (flag)
			{
				sortAndFilter.gameObject.SetActive(value);
			}
		}

		// Token: 0x0600798D RID: 31117 RVA: 0x003879A0 File Offset: 0x00385BA0
		private void OnSourceToggleChanged(int newIndex, int oldIndex)
		{
			bool flag = newIndex < 0;
			if (!flag)
			{
				bool flag2 = this._multiplyReady && oldIndex >= 0;
				if (flag2)
				{
					this.PersistMultiplySelectionForSource(ZhujianGearMateItemSelector.ResolveSourceType(oldIndex));
				}
				Action<int> onSourceTypeChanged = this.OnSourceTypeChanged;
				if (onSourceTypeChanged != null)
				{
					onSourceTypeChanged(newIndex);
				}
				bool multiplyReady = this._multiplyReady;
				if (multiplyReady)
				{
					this.LoadMultiplySelectionForCurrentSource();
				}
				this.Refresh();
				this.UpdateSelectAllToggleState();
			}
		}

		// Token: 0x0600798E RID: 31118 RVA: 0x00387A10 File Offset: 0x00385C10
		private void UpdateSelectAllToggleState()
		{
			bool multiplyReady = this._multiplyReady;
			if (multiplyReady)
			{
				this.multiplyItemListScroll.RefreshMultiplyOperationChrome();
			}
		}

		// Token: 0x0600798F RID: 31119 RVA: 0x00387A34 File Offset: 0x00385C34
		private void SyncSelectedListPanel()
		{
			bool multiplyReady = this._multiplyReady;
			if (multiplyReady)
			{
				this.multiplyItemListScroll.SyncSelectedListPanelVisibility();
			}
		}

		// Token: 0x06007990 RID: 31120 RVA: 0x00387A58 File Offset: 0x00385C58
		public void SetSourceToggleInteractable(int index, bool interactable)
		{
			this.sourceToggleGroup.SetInteractable(interactable, index);
			CToggle[] toggles = this.sourceToggleGroup.GetComponentsInChildren<CToggle>(true);
			bool flag = index < 0 || index >= toggles.Length;
			if (!flag)
			{
				CToggle toggle = toggles[index];
				bool flag2 = toggle == null;
				if (!flag2)
				{
					Graphic targetGraphic = toggle.targetGraphic;
					bool flag3 = targetGraphic == null;
					if (!flag3)
					{
						TooltipInvoker tipDisplayer = targetGraphic.GetComponent<TooltipInvoker>();
						bool flag4 = tipDisplayer == null;
						if (flag4)
						{
							tipDisplayer = targetGraphic.gameObject.AddComponent<TooltipInvoker>();
						}
						bool flag5 = !interactable;
						if (flag5)
						{
							tipDisplayer.enabled = true;
							tipDisplayer.Type = TipType.SingleDesc;
							tipDisplayer.RuntimeParam = new ArgumentBox();
							tipDisplayer.RuntimeParam.Set("arg0", LanguageKey.LK_Location_IsNotInSettlement.Tr());
						}
						else
						{
							tipDisplayer.enabled = false;
						}
					}
				}
			}
		}

		// Token: 0x06007991 RID: 31121 RVA: 0x00387B38 File Offset: 0x00385D38
		public void SetSourceToggleGroupInteractable(bool interactable)
		{
			this.sourceToggleGroup.SetInteractable(interactable);
		}

		// Token: 0x06007992 RID: 31122 RVA: 0x00387B48 File Offset: 0x00385D48
		public void SetSourceToggle(int index)
		{
			this.sourceToggleGroup.Set(index, false);
		}

		// Token: 0x06007993 RID: 31123 RVA: 0x00387B5C File Offset: 0x00385D5C
		private string AmountCellDataGenerator(ITradeableContent item)
		{
			ItemDisplayData displayData = item as ItemDisplayData;
			bool flag = displayData == null;
			string result;
			if (flag)
			{
				result = item.Amount.ToString();
			}
			else
			{
				int maxCount = displayData.Amount;
				bool flag2 = this.multiplyItemListScroll != null && this.multiplyItemListScroll.IsMultiItemSelect;
				if (flag2)
				{
					int selectedCount = this.GetMultiplySelectedCount(displayData);
					bool flag3 = selectedCount > 0;
					if (flag3)
					{
						result = CommonUtils.GetDisplayStringForNum(selectedCount, 100000) + "/" + CommonUtils.GetDisplayStringForNum(maxCount, 100000);
					}
					else
					{
						result = CommonUtils.GetDisplayStringForNum(maxCount, 100000);
					}
				}
				else
				{
					Dictionary<ItemKey, int> currentSelectedItems = this.GetCurrentSelectedItems();
					int selected;
					bool flag4 = currentSelectedItems.TryGetValue(displayData.Key, out selected);
					if (flag4)
					{
						result = string.Format("{0}/{1}", selected, maxCount);
					}
					else
					{
						result = maxCount.ToString();
					}
				}
			}
			return result;
		}

		// Token: 0x06007994 RID: 31124 RVA: 0x00387C4C File Offset: 0x00385E4C
		private int GetMultiplySelectedCount(ItemDisplayData displayData)
		{
			ManagedMultiplyItemListScroll managedMultiplyItemListScroll = this.multiplyItemListScroll;
			bool flag = ((managedMultiplyItemListScroll != null) ? managedMultiplyItemListScroll.SelectedMultiplyItemDict : null) == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				foreach (KeyValuePair<ItemDisplayData, int> keyValuePair in this.multiplyItemListScroll.SelectedMultiplyItemDict)
				{
					ItemDisplayData itemDisplayData;
					int num;
					keyValuePair.Deconstruct(out itemDisplayData, out num);
					ItemDisplayData data = itemDisplayData;
					int count = num;
					bool flag2 = data.RealKey.Equals(displayData.RealKey);
					if (flag2)
					{
						return count;
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x06007995 RID: 31125 RVA: 0x00387CF8 File Offset: 0x00385EF8
		private void OnItemRender(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			ItemDisplayData item = itemData as ItemDisplayData;
			bool flag = item == null;
			if (!flag)
			{
				RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
				rowItemMain.SetData(itemData);
				rowItemLine.Set(rowItemMain, true);
				bool flag2 = this._multiplyReady && this.multiplyItemListScroll.IsMultiItemSelect;
				if (flag2)
				{
					this.multiplyItemListScroll.OnRenderItemMultiply(itemData, rowItemLine);
					this.ApplyRowTip(item, rowItemLine);
				}
				else
				{
					Dictionary<ItemKey, int> currentSelectedItems = this.GetCurrentSelectedItems();
					rowItemLine.SetSelected(currentSelectedItems.ContainsKey(item.Key));
					this.ApplyRowTip(item, rowItemLine);
				}
			}
		}

		// Token: 0x06007996 RID: 31126 RVA: 0x00387D8C File Offset: 0x00385F8C
		private void ApplyRowTip(ItemDisplayData item, RowItemLine rowItemLine)
		{
			bool interactable = !item.IsLocked;
			bool flag = this.GetMaxSelectableCount != null;
			if (flag)
			{
				interactable = (interactable && this.GetMaxSelectableCount(item) > 0);
			}
			bool flag2 = this._multiplyReady && this.multiplyItemListScroll.IsMultiItemSelect;
			if (flag2)
			{
				bool interactable2 = rowItemLine.Interactable;
				if (interactable2)
				{
					RowItemLine.SetMouseTipDisplayer(true, item, rowItemLine.TipDisplayer);
				}
			}
			else
			{
				rowItemLine.SetInteractable(interactable, true);
				rowItemLine.SetDisabled(!interactable);
				bool flag3 = !interactable;
				if (flag3)
				{
					TooltipInvoker tipDisplayer = rowItemLine.TipDisplayer;
					bool flag4 = tipDisplayer == null;
					if (!flag4)
					{
						tipDisplayer.enabled = true;
						Func<ItemDisplayData, string> getDisabledItemTip = this.GetDisabledItemTip;
						string tipText = ((getDisabledItemTip != null) ? getDisabledItemTip(item) : null) ?? "";
						tipDisplayer.Type = TipType.SingleDesc;
						tipDisplayer.RuntimeParam = new ArgumentBox();
						tipDisplayer.RuntimeParam.Set("arg0", tipText);
					}
				}
				else
				{
					RowItemLine.SetMouseTipDisplayer(true, item, rowItemLine.TipDisplayer);
				}
			}
		}

		// Token: 0x06007997 RID: 31127 RVA: 0x00387E99 File Offset: 0x00386099
		public void SetItems(List<ItemDisplayData> items)
		{
			this._allItems = (items ?? new List<ItemDisplayData>());
			this.Refresh();
		}

		// Token: 0x06007998 RID: 31128 RVA: 0x00387EB3 File Offset: 0x003860B3
		public void SetBaseFilter(Predicate<ItemDisplayData> filter)
		{
			this._baseFilter = filter;
			this.Refresh();
		}

		// Token: 0x06007999 RID: 31129 RVA: 0x00387EC4 File Offset: 0x003860C4
		public void Refresh()
		{
			int targetSource = ZhujianGearMateItemSelector.ResolveSourceType(this.sourceToggleGroup.GetActiveIndex());
			List<ItemDisplayData> sourceFiltered = (from item in this._allItems
			where (int)item.ItemSourceType == targetSource
			where this._baseFilter == null || this._baseFilter(item)
			select item).ToList<ItemDisplayData>();
			ItemListScroll itemList = this.ItemList;
			if (itemList != null)
			{
				SortAndFilterController<ITradeableContent> sortAndFilterController = itemList.SortAndFilterController;
				if (sortAndFilterController != null)
				{
					sortAndFilterController.NotifyDataChanged(this._allItems.Cast<ITradeableContent>().ToList<ITradeableContent>());
				}
			}
			bool multiplyReady = this._multiplyReady;
			if (multiplyReady)
			{
				this.multiplyItemListScroll.SetItems(sourceFiltered);
			}
			else
			{
				ItemListScroll itemList2 = this.ItemList;
				if (itemList2 != null)
				{
					itemList2.SetItemList(sourceFiltered.Cast<ITradeableContent>().ToList<ITradeableContent>());
				}
			}
			this.UpdateSelectAllToggleState();
			this.SyncSelectedListPanel();
		}

		// Token: 0x0600799A RID: 31130 RVA: 0x00387F94 File Offset: 0x00386194
		private static int ResolveSourceType(int toggleIndex)
		{
			if (!true)
			{
			}
			int result;
			switch (toggleIndex)
			{
			case 0:
				result = 1;
				break;
			case 1:
				result = 2;
				break;
			case 2:
				result = 3;
				break;
			default:
				result = -1;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600799B RID: 31131 RVA: 0x00387FD4 File Offset: 0x003861D4
		public void ClearSelection()
		{
			this.GetCurrentSelectedItems().Clear();
			bool multiplyReady = this._multiplyReady;
			if (multiplyReady)
			{
				this.multiplyItemListScroll.ClearMultiplySelection(true);
			}
			else
			{
				ItemListScroll itemList = this.ItemList;
				if (itemList != null)
				{
					itemList.ReRender();
				}
				this.UpdateSelectAllToggleState();
				this.SyncSelectedListPanel();
				Action onSelectionChanged = this.OnSelectionChanged;
				if (onSelectionChanged != null)
				{
					onSelectionChanged();
				}
			}
		}

		// Token: 0x0600799C RID: 31132 RVA: 0x0038803C File Offset: 0x0038623C
		public void ClearAllSelections(bool skipAction = false)
		{
			foreach (Dictionary<ItemKey, int> items in this._selectedItemsBySource.Values)
			{
				items.Clear();
			}
			bool multiplyReady = this._multiplyReady;
			if (multiplyReady)
			{
				this._suppressMultiplySelectionEvent = skipAction;
				this.multiplyItemListScroll.ClearMultiplySelection(!skipAction);
				this._suppressMultiplySelectionEvent = false;
			}
			else
			{
				ItemListScroll itemList = this.ItemList;
				if (itemList != null)
				{
					itemList.ReRender();
				}
				this.UpdateSelectAllToggleState();
				this.SyncSelectedListPanel();
				bool flag = !skipAction;
				if (flag)
				{
					Action onSelectionChanged = this.OnSelectionChanged;
					if (onSelectionChanged != null)
					{
						onSelectionChanged();
					}
				}
			}
		}

		// Token: 0x0600799D RID: 31133 RVA: 0x003880FC File Offset: 0x003862FC
		public void SetMaterialFilter(int materialSubFilterIndex)
		{
			ItemListScroll itemList = this.ItemList;
			SortAndFilterController<ITradeableContent> sortController = (itemList != null) ? itemList.SortAndFilterController : null;
			bool flag = sortController == null;
			if (!flag)
			{
				this._isSettingMaterialFilter = true;
				sortController.ClearAllFilter();
				bool flag2 = materialSubFilterIndex == -1;
				if (flag2)
				{
					this.Refresh();
					this._isSettingMaterialFilter = false;
				}
				else
				{
					bool hasMainFilter = sortController.FilterLines.Exists((FilterLineBase<ITradeableContent> line) => line.Id == 0);
					bool flag3 = hasMainFilter;
					if (flag3)
					{
						sortController.SetToggleIsOn(0, 5);
					}
					sortController.SetDropdownOption(6, 0, materialSubFilterIndex);
					this.Refresh();
					this._isSettingMaterialFilter = false;
				}
			}
		}

		// Token: 0x0600799E RID: 31134 RVA: 0x003881A4 File Offset: 0x003863A4
		private void OnItemClick(ITradeableContent content, RowItemLine rowItemLine)
		{
			bool blocked = this._blocked;
			if (!blocked)
			{
				ItemDisplayData item = content as ItemDisplayData;
				bool flag = item == null;
				if (!flag)
				{
					bool flag2 = UIManager.Instance.IsElementActive(UIElement.SetSelectCount);
					if (flag2)
					{
						bool flag3 = item.Key.Equals(this._editingItemKey);
						if (flag3)
						{
							GEvent.OnEvent(UiEvents.OnConfirmSetSelectCount, null);
						}
					}
					else
					{
						bool flag4 = this._multiplyReady && this.multiplyItemListScroll.IsMultiItemSelect;
						if (flag4)
						{
							this._editingItemKey = item.Key;
							this.multiplyItemListScroll.HandleRowClick(item, rowItemLine);
						}
						else
						{
							this.HandleItemClickWithoutMultiply(item, rowItemLine);
						}
					}
				}
			}
		}

		// Token: 0x0600799F RID: 31135 RVA: 0x0038825C File Offset: 0x0038645C
		private void HandleItemClickWithoutMultiply(ItemDisplayData item, RowItemLine rowItemLine)
		{
			Dictionary<ItemKey, int> currentSelectedItems = this.GetCurrentSelectedItems();
			bool wasSelected = currentSelectedItems.ContainsKey(item.Key);
			bool flag = wasSelected;
			if (flag)
			{
				currentSelectedItems.Remove(item.Key);
				ItemListScroll itemList = this.ItemList;
				if (itemList != null)
				{
					itemList.ReRender();
				}
				this.UpdateSelectAllToggleState();
				Action onSelectionChanged = this.OnSelectionChanged;
				if (onSelectionChanged != null)
				{
					onSelectionChanged();
				}
			}
			else
			{
				int maxSelectable = item.Amount;
				bool flag2 = this.GetMaxSelectableCount != null;
				if (flag2)
				{
					maxSelectable = this.GetMaxSelectableCount(item);
				}
				bool flag3 = maxSelectable <= 0;
				if (!flag3)
				{
					bool flag4 = item.Amount > 1;
					if (flag4)
					{
						currentSelectedItems[item.Key] = maxSelectable;
						ItemListScroll itemList2 = this.ItemList;
						if (itemList2 != null)
						{
							itemList2.ReRender();
						}
						this._editingItemKey = item.Key;
						ItemListScroll itemList3 = this.ItemList;
						if (itemList3 != null)
						{
							itemList3.SetItemToSelectCountMode(rowItemLine, delegate(int count)
							{
								currentSelectedItems[item.Key] = count;
								Action<ItemKey> onItemSelected2 = this.OnItemSelected;
								if (onItemSelected2 != null)
								{
									onItemSelected2(item.Key);
								}
								ItemListScroll itemList5 = this.ItemList;
								if (itemList5 != null)
								{
									itemList5.ReRender();
								}
								this.UpdateSelectAllToggleState();
								Action onSelectionChanged3 = this.OnSelectionChanged;
								if (onSelectionChanged3 != null)
								{
									onSelectionChanged3();
								}
							}, delegate
							{
								currentSelectedItems.Remove(item.Key);
								ItemListScroll itemList5 = this.ItemList;
								if (itemList5 != null)
								{
									itemList5.ReRender();
								}
								this.UpdateSelectAllToggleState();
								Action onSelectionChanged3 = this.OnSelectionChanged;
								if (onSelectionChanged3 != null)
								{
									onSelectionChanged3();
								}
							}, maxSelectable, maxSelectable, 1, null, false, null, false);
						}
					}
					else
					{
						currentSelectedItems[item.Key] = 1;
						ItemListScroll itemList4 = this.ItemList;
						if (itemList4 != null)
						{
							itemList4.ReRender();
						}
						Action<ItemKey> onItemSelected = this.OnItemSelected;
						if (onItemSelected != null)
						{
							onItemSelected(item.Key);
						}
						this.UpdateSelectAllToggleState();
						Action onSelectionChanged2 = this.OnSelectionChanged;
						if (onSelectionChanged2 != null)
						{
							onSelectionChanged2();
						}
					}
				}
			}
		}

		// Token: 0x060079A0 RID: 31136 RVA: 0x00388414 File Offset: 0x00386614
		private void OnMultiplyContentChanged([TupleElementNames(new string[]
		{
			"data",
			"count"
		})] List<ValueTuple<ItemDisplayData, int>> changeList)
		{
			this.PersistMultiplySelectionForCurrentSource();
			ManagedMultiplyItemListScroll managedMultiplyItemListScroll = this.multiplyItemListScroll;
			if (managedMultiplyItemListScroll != null)
			{
				managedMultiplyItemListScroll.RefreshListAmountDisplay();
			}
			this.UpdateSelectAllToggleState();
			this.SyncSelectedListPanel();
			bool suppressMultiplySelectionEvent = this._suppressMultiplySelectionEvent;
			if (!suppressMultiplySelectionEvent)
			{
				bool flag = changeList != null;
				if (flag)
				{
					foreach (ValueTuple<ItemDisplayData, int> valueTuple in changeList)
					{
						ItemDisplayData data = valueTuple.Item1;
						int count = valueTuple.Item2;
						bool flag2 = count > 0;
						if (flag2)
						{
							Action<ItemKey> onItemSelected = this.OnItemSelected;
							if (onItemSelected != null)
							{
								onItemSelected(data.Key);
							}
						}
					}
				}
				Action onSelectionChanged = this.OnSelectionChanged;
				if (onSelectionChanged != null)
				{
					onSelectionChanged();
				}
			}
		}

		// Token: 0x060079A1 RID: 31137 RVA: 0x003884E0 File Offset: 0x003866E0
		private void PersistMultiplySelectionForCurrentSource()
		{
			this.PersistMultiplySelectionForSource(ZhujianGearMateItemSelector.ResolveSourceType(this.sourceToggleGroup.GetActiveIndex()));
		}

		// Token: 0x060079A2 RID: 31138 RVA: 0x003884FC File Offset: 0x003866FC
		private void PersistMultiplySelectionForSource(int source)
		{
			Dictionary<ItemKey, int> dict;
			bool flag = !this._selectedItemsBySource.TryGetValue(source, out dict);
			if (flag)
			{
				dict = new Dictionary<ItemKey, int>();
				this._selectedItemsBySource[source] = dict;
			}
			dict.Clear();
			bool flag2 = !this._multiplyReady || this.multiplyItemListScroll == null;
			if (!flag2)
			{
				foreach (KeyValuePair<ItemDisplayData, int> keyValuePair in this.multiplyItemListScroll.SelectedMultiplyItemDict)
				{
					ItemDisplayData itemDisplayData;
					int num;
					keyValuePair.Deconstruct(out itemDisplayData, out num);
					ItemDisplayData data = itemDisplayData;
					int count = num;
					bool flag3 = count > 0;
					if (flag3)
					{
						dict[data.Key] = count;
					}
				}
			}
		}

		// Token: 0x060079A3 RID: 31139 RVA: 0x003885D4 File Offset: 0x003867D4
		private void LoadMultiplySelectionForCurrentSource()
		{
			bool flag = !this._multiplyReady || this.multiplyItemListScroll == null;
			if (!flag)
			{
				Dictionary<ItemDisplayData, int> dict = this.multiplyItemListScroll.SelectedMultiplyItemDict;
				List<ItemDisplayData> list = this.multiplyItemListScroll.SelectedMultiplyItemOrderedList;
				dict.Clear();
				list.Clear();
				int source = ZhujianGearMateItemSelector.ResolveSourceType(this.sourceToggleGroup.GetActiveIndex());
				Dictionary<ItemKey, int> current = this.GetCurrentSelectedItems();
				using (Dictionary<ItemKey, int>.Enumerator enumerator = current.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<ItemKey, int> kvp = enumerator.Current;
						ItemDisplayData data = this._allItems.Find((ItemDisplayData d) => d.Key.Equals(kvp.Key) && (int)d.ItemSourceType == source);
						bool flag2 = data == null;
						if (!flag2)
						{
							dict[data] = kvp.Value;
							list.Add(data);
						}
					}
				}
				this.multiplyItemListScroll.RefreshItems();
				this.multiplyItemListScroll.RefreshListAmountDisplay();
			}
		}

		// Token: 0x060079A4 RID: 31140 RVA: 0x00388700 File Offset: 0x00386900
		private bool CanSelectMultiplyItem(ItemDisplayData item)
		{
			bool isLocked = item.IsLocked;
			bool result;
			if (isLocked)
			{
				result = false;
			}
			else
			{
				bool flag = this._baseFilter != null && !this._baseFilter(item);
				if (flag)
				{
					result = false;
				}
				else
				{
					bool flag2 = this.GetMaxSelectableCount != null && this.GetMaxSelectableCount(item) <= 0;
					result = !flag2;
				}
			}
			return result;
		}

		// Token: 0x060079A5 RID: 31141 RVA: 0x00388768 File Offset: 0x00386968
		private int GetMultiplySelectLimit(ItemDisplayData item, int defaultLimit)
		{
			bool flag = this.GetMaxSelectableCount == null;
			int result;
			if (flag)
			{
				result = defaultLimit;
			}
			else
			{
				result = Mathf.Min(defaultLimit, this.GetMaxSelectableCount(item));
			}
			return result;
		}

		// Token: 0x060079A6 RID: 31142 RVA: 0x003887A0 File Offset: 0x003869A0
		private ISortAndFilterView GetSortAndFilterView()
		{
			return (this.ItemList != null) ? this.ItemList.GetComponentInChildren<SortAndFilter>(true) : null;
		}

		// Token: 0x060079A7 RID: 31143 RVA: 0x003887D0 File Offset: 0x003869D0
		private void ReplaceItemListSortController(SortAndFilterController<ITradeableContent> newController)
		{
			bool flag = this.ItemList == null;
			if (!flag)
			{
				Type listType = typeof(ItemListScroll);
				FieldInfo field = listType.GetField("_sortAndFilterController", BindingFlags.Instance | BindingFlags.NonPublic);
				SortAndFilterController<ITradeableContent> oldCtrl = ((field != null) ? field.GetValue(this.ItemList) : null) as SortAndFilterController<ITradeableContent>;
				if (oldCtrl != null)
				{
					oldCtrl.UninitForReplace();
				}
				newController.Init(new Action(this.OnItemListSortAndFilterChanged), "GearMateAttribute");
				FieldInfo field2 = listType.GetField("_sortAndFilterController", BindingFlags.Instance | BindingFlags.NonPublic);
				if (field2 != null)
				{
					field2.SetValue(this.ItemList, newController);
				}
				FieldInfo field3 = listType.GetField("scroll", BindingFlags.Instance | BindingFlags.NonPublic);
				ListStyleGeneralScroll rowScroll = ((field3 != null) ? field3.GetValue(this.ItemList) : null) as ListStyleGeneralScroll;
				bool flag2 = rowScroll != null;
				if (flag2)
				{
					rowScroll.SetSortController(newController);
				}
				FieldInfo field4 = listType.GetField("cardScroll", BindingFlags.Instance | BindingFlags.NonPublic);
				CardStyleGeneralScroll cardScroll = ((field4 != null) ? field4.GetValue(this.ItemList) : null) as CardStyleGeneralScroll;
				bool flag3 = cardScroll != null;
				if (flag3)
				{
					cardScroll.SetSortController(newController);
				}
			}
		}

		// Token: 0x060079A8 RID: 31144 RVA: 0x003888D8 File Offset: 0x00386AD8
		private void OnItemListSortAndFilterChanged()
		{
			bool flag = !this._isSettingMaterialFilter;
			if (flag)
			{
				Action onFilterManuallyChanged = this.OnFilterManuallyChanged;
				if (onFilterManuallyChanged != null)
				{
					onFilterManuallyChanged();
				}
			}
			this.UpdateSelectAllToggleState();
			this.Refresh();
		}

		// Token: 0x04005C21 RID: 23585
		[SerializeField]
		private ManagedMultiplyItemListScroll multiplyItemListScroll;

		// Token: 0x04005C22 RID: 23586
		[SerializeField]
		private CToggleGroup sourceToggleGroup;

		// Token: 0x04005C23 RID: 23587
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x04005C24 RID: 23588
		[SerializeField]
		private GameObject blockOverlay;

		// Token: 0x04005C25 RID: 23589
		private List<ItemDisplayData> _allItems = new List<ItemDisplayData>();

		// Token: 0x04005C26 RID: 23590
		private readonly Dictionary<int, Dictionary<ItemKey, int>> _selectedItemsBySource = new Dictionary<int, Dictionary<ItemKey, int>>();

		// Token: 0x04005C27 RID: 23591
		private Predicate<ItemDisplayData> _baseFilter;

		// Token: 0x04005C28 RID: 23592
		private Func<ISortAndFilterView, SortAndFilterController<ITradeableContent>> _sortControllerFactory;

		// Token: 0x04005C29 RID: 23593
		private bool _blocked;

		// Token: 0x04005C2A RID: 23594
		private bool _multiplyReady;

		// Token: 0x04005C2B RID: 23595
		private bool _suppressMultiplySelectionEvent;

		// Token: 0x04005C2C RID: 23596
		private bool _isSettingMaterialFilter;

		// Token: 0x04005C2D RID: 23597
		private const string SortSaveKey = "GearMateAttribute";

		// Token: 0x04005C2E RID: 23598
		private const string SelectedSortSaveKey = "GearMateAttribute_Selected";

		// Token: 0x04005C2F RID: 23599
		private const BindingFlags ItemListNonPublic = BindingFlags.Instance | BindingFlags.NonPublic;

		// Token: 0x04005C34 RID: 23604
		public Func<ItemDisplayData, int> GetMaxSelectableCount;

		// Token: 0x04005C35 RID: 23605
		public Func<ItemDisplayData, string> GetDisabledItemTip;

		// Token: 0x04005C36 RID: 23606
		private ItemKey _editingItemKey;
	}
}
