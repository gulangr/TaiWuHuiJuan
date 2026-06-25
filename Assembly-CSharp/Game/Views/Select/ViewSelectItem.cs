using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.CellContent;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item;
using Game.Views.Item;
using Game.Views.Profession;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using GameDataExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Select
{
	// Token: 0x020007AF RID: 1967
	public class ViewSelectItem : UIBase
	{
		// Token: 0x17000BA8 RID: 2984
		// (get) Token: 0x06005F87 RID: 24455 RVA: 0x002BCE22 File Offset: 0x002BB022
		public ItemSourceType SelectedSourceType
		{
			get
			{
				return ViewSelectItem.ToggleSourceTypes.GetOrDefault(this.itemSourceType.GetActiveIndex(), ItemSourceType.Inventory);
			}
		}

		// Token: 0x17000BA9 RID: 2985
		// (get) Token: 0x06005F88 RID: 24456 RVA: 0x002BCE3C File Offset: 0x002BB03C
		private IReadOnlyList<ITradeableContent> SelectedData
		{
			get
			{
				ItemSourceType selectedSourceType = this.SelectedSourceType;
				if (!true)
				{
				}
				IReadOnlyList<ITradeableContent> readOnlyList3;
				switch (selectedSourceType)
				{
				case ItemSourceType.Inventory:
				{
					IReadOnlyList<ITradeableContent> readOnlyList2;
					if (!this._usingExternalData)
					{
						SelectItemDisplayData data = this._data;
						IReadOnlyList<ITradeableContent> readOnlyList = (data != null) ? data.InventoryItems : null;
						readOnlyList2 = readOnlyList;
					}
					else
					{
						SelectItemConfig config = this._config;
						readOnlyList2 = ((config != null) ? config.ExternalItems : null);
					}
					readOnlyList3 = readOnlyList2;
					break;
				}
				case ItemSourceType.Warehouse:
				{
					IReadOnlyList<ITradeableContent> readOnlyList4;
					if (!this._usingExternalData)
					{
						IReadOnlyList<ITradeableContent> readOnlyList = this._data.WarehouseItems;
						readOnlyList4 = readOnlyList;
					}
					else
					{
						SelectItemConfig config2 = this._config;
						readOnlyList4 = ((config2 != null) ? config2.ExternalWarehouseItems : null);
					}
					readOnlyList3 = readOnlyList4;
					break;
				}
				case ItemSourceType.Treasury:
				{
					IReadOnlyList<ITradeableContent> readOnlyList5;
					if (!this._usingExternalData)
					{
						IReadOnlyList<ITradeableContent> readOnlyList = this._data.TreasuryItems;
						readOnlyList5 = readOnlyList;
					}
					else
					{
						SelectItemConfig config3 = this._config;
						readOnlyList5 = ((config3 != null) ? config3.ExternalTreasuryItems : null);
					}
					readOnlyList3 = readOnlyList5;
					break;
				}
				default:
					throw new ArgumentOutOfRangeException("SelectedSourceType", this.SelectedSourceType, null);
				}
				if (!true)
				{
				}
				IReadOnlyList<ITradeableContent> result = readOnlyList3;
				return result ?? Array.Empty<ITradeableContent>();
			}
		}

		// Token: 0x06005F89 RID: 24457 RVA: 0x002BCF28 File Offset: 0x002BB128
		private ISortAndFilterView GetSortAndFilterView()
		{
			return this.sortAndFilter;
		}

		// Token: 0x17000BAA RID: 2986
		// (get) Token: 0x06005F8A RID: 24458 RVA: 0x002BCF40 File Offset: 0x002BB140
		private bool _usingExternalData
		{
			get
			{
				SelectItemConfig config = this._config;
				if (((config != null) ? config.ExternalItems : null) == null)
				{
					SelectItemConfig config2 = this._config;
					if (((config2 != null) ? config2.ExternalWarehouseItems : null) == null)
					{
						SelectItemConfig config3 = this._config;
						return ((config3 != null) ? config3.ExternalTreasuryItems : null) != null;
					}
				}
				return true;
			}
		}

		// Token: 0x17000BAB RID: 2987
		// (get) Token: 0x06005F8B RID: 24459 RVA: 0x002BCF8D File Offset: 0x002BB18D
		private int TotalSelectedAmount
		{
			get
			{
				return this._selectedItems.Sum((SelectedItemData s) => s.SelectedAmount);
			}
		}

		// Token: 0x17000BAC RID: 2988
		// (get) Token: 0x06005F8C RID: 24460 RVA: 0x002BCFB9 File Offset: 0x002BB1B9
		private int TotalSelectedRow
		{
			get
			{
				return this._selectedItems.Count<SelectedItemData>();
			}
		}

		// Token: 0x17000BAD RID: 2989
		// (get) Token: 0x06005F8D RID: 24461 RVA: 0x002BCFC6 File Offset: 0x002BB1C6
		private IReadOnlyList<SelectedItemData> ActiveSelectedItems
		{
			get
			{
				return this._selectedItems;
			}
		}

		// Token: 0x17000BAE RID: 2990
		// (get) Token: 0x06005F8E RID: 24462 RVA: 0x002BCFCE File Offset: 0x002BB1CE
		private bool SplitSelectedAmountIntoSingleEntries
		{
			get
			{
				SelectItemConfig config = this._config;
				return config != null && config.SplitSelectedAmountIntoSingleEntries;
			}
		}

		// Token: 0x06005F8F RID: 24463 RVA: 0x002BCFE4 File Offset: 0x002BB1E4
		private bool IsSameSelectedItem(ITradeableContent left, ITradeableContent right)
		{
			bool flag = left == null || right == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = left == right;
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool flag3 = this._config != null && this._config.CheckSameByReferenceOnly;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = !left.RealKey.Equals(right.RealKey);
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool leftSourceValid = left.ItemSourceType >= 0;
							bool rightSourceValid = right.ItemSourceType >= 0;
							bool flag5 = leftSourceValid && rightSourceValid;
							result = (!flag5 || left.ItemSourceType == right.ItemSourceType);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06005F90 RID: 24464 RVA: 0x002BD090 File Offset: 0x002BB290
		private void RemoveSingleSelectedEntry(ITradeableContent itemData)
		{
			bool flag = itemData == null;
			if (!flag)
			{
				bool splitSelectedAmountIntoSingleEntries = this.SplitSelectedAmountIntoSingleEntries;
				if (splitSelectedAmountIntoSingleEntries)
				{
					SelectedItemData splitRemove = this._selectedItems.LastOrDefault((SelectedItemData t) => t.ItemData == itemData);
					bool flag2 = splitRemove != null;
					if (flag2)
					{
						this._selectedItems.Remove(splitRemove);
					}
				}
				else
				{
					SelectedItemData toRemove = this._selectedItems.LastOrDefault((SelectedItemData t) => t.ItemData == itemData);
					bool flag3 = toRemove == null && itemData.ItemSourceType >= 0;
					if (flag3)
					{
						toRemove = this._selectedItems.LastOrDefault((SelectedItemData t) => t.ItemData.RealKey.Equals(itemData.RealKey) && t.ItemData.ItemSourceType == itemData.ItemSourceType);
					}
					bool flag4 = toRemove == null;
					if (flag4)
					{
						toRemove = this._selectedItems.LastOrDefault((SelectedItemData t) => t.ItemData.RealKey.Equals(itemData.RealKey) && t.ItemData.ItemSourceType < 0);
					}
					bool flag5 = toRemove == null;
					if (!flag5)
					{
						this._selectedItems.Remove(toRemove);
					}
				}
			}
		}

		// Token: 0x06005F91 RID: 24465 RVA: 0x002BD18C File Offset: 0x002BB38C
		private SelectedItemData FindSelectedItem(ITradeableContent itemData)
		{
			bool flag = itemData == null;
			SelectedItemData result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = this._selectedItems.Find((SelectedItemData s) => this.IsSameSelectedItem(s.ItemData, itemData));
			}
			return result;
		}

		// Token: 0x06005F92 RID: 24466 RVA: 0x002BD1DC File Offset: 0x002BB3DC
		private int GetActiveSelectedAmount(ITradeableContent itemData)
		{
			bool flag = itemData == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = itemData.Key.ItemType == 10 && !itemData.IsSpecialInteract;
				if (flag2)
				{
					result = (from s in this._selectedItems
					where s.ItemData == itemData
					select s).Sum((SelectedItemData s) => s.SelectedAmount);
				}
				else
				{
					bool splitSelectedAmountIntoSingleEntries = this.SplitSelectedAmountIntoSingleEntries;
					if (splitSelectedAmountIntoSingleEntries)
					{
						result = (from s in this._selectedItems
						where s.ItemData == itemData
						select s).Sum((SelectedItemData s) => s.SelectedAmount);
					}
					else
					{
						result = (from s in this._selectedItems
						where this.IsSameSelectedItem(s.ItemData, itemData)
						select s).Sum((SelectedItemData s) => s.SelectedAmount);
					}
				}
			}
			return result;
		}

		// Token: 0x06005F93 RID: 24467 RVA: 0x002BD308 File Offset: 0x002BB508
		private int GetResourceValueMaxSelectableAmount(ITradeableContent itemData)
		{
			bool flag = itemData == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				SelectItemConfig config = this._config;
				bool flag2 = !(((config != null) ? new int?(config.ResourceMaxValue) : null) >= 0);
				if (flag2)
				{
					result = itemData.Amount;
				}
				else
				{
					bool flag3 = !ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
					if (flag3)
					{
						result = itemData.Amount;
					}
					else
					{
						int baseValue = ItemTemplateHelper.GetBaseValue(itemData.Key.ItemType, itemData.Key.TemplateId);
						bool flag4 = baseValue <= 0;
						if (flag4)
						{
							result = itemData.Amount;
						}
						else
						{
							result = Math.Min(itemData.Amount, this._config.ResourceMaxValue / baseValue);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06005F94 RID: 24468 RVA: 0x002BD3EC File Offset: 0x002BB5EC
		private int GetRemainingCandidateAmount(ITradeableContent itemData)
		{
			bool flag = itemData == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int maxSelectable = this.GetResourceValueMaxSelectableAmount(itemData);
				result = Math.Max(0, maxSelectable - this.GetActiveSelectedAmount(itemData));
			}
			return result;
		}

		// Token: 0x06005F95 RID: 24469 RVA: 0x002BD424 File Offset: 0x002BB624
		private int GetRemainingSelectableAmount(ITradeableContent itemData)
		{
			int remainingByItem = this.GetRemainingCandidateAmount(itemData);
			bool flag = this._config.SelectItemMode == ESelectItemMode.RowSelect;
			int result;
			if (flag)
			{
				result = remainingByItem;
			}
			else
			{
				bool flag2 = this._config.MaxSelectCount > 0;
				if (flag2)
				{
					int remainingByTotal = this._config.MaxSelectCount - this.TotalSelectedAmount;
					result = Math.Max(0, Math.Min(remainingByItem, remainingByTotal));
				}
				else
				{
					result = remainingByItem;
				}
			}
			return result;
		}

		// Token: 0x06005F96 RID: 24470 RVA: 0x002BD490 File Offset: 0x002BB690
		private void AddSelectedItemsAsSingleUnits(ITradeableContent itemData, int amount)
		{
			bool flag = itemData == null || amount <= 0;
			if (!flag)
			{
				int amountToAdd = Math.Min(amount, this.GetRemainingSelectableAmount(itemData));
				bool flag2 = amountToAdd <= 0;
				if (!flag2)
				{
					for (int i = 0; i < amountToAdd; i++)
					{
						this._selectedItems.Add(new SelectedItemData(itemData, 1));
					}
				}
			}
		}

		// Token: 0x06005F97 RID: 24471 RVA: 0x002BD4F4 File Offset: 0x002BB6F4
		private bool IsCurrentEditingItem(ITradeableContent itemData)
		{
			bool flag = itemData == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this._editingItemData != null;
				if (flag2)
				{
					result = this.IsSameSelectedItem(itemData, this._editingItemData);
				}
				else
				{
					result = itemData.RealKey.Equals(this._editingItemKey);
				}
			}
			return result;
		}

		// Token: 0x06005F98 RID: 24472 RVA: 0x002BD544 File Offset: 0x002BB744
		public override void OnInit(ArgumentBox argBox)
		{
			argBox.Get<SelectItemConfig>("SelectItemConfig", out this._config);
			bool displayBg;
			argBox.Get("DisplayBg", out displayBg);
			base.GetComponent<Canvas>().sortingOrder = (displayBg ? this.sortingLayerFront : this.sortingLayerNormal);
			this.bg.enabled = displayBg;
			if (this._config == null)
			{
				this._config = new SelectItemConfig();
			}
			this._useCustomColumns = (this._config.ColumnFlags != null);
			ESelectItemColumnType newFlag = this._config.ColumnFlags.GetValueOrDefault();
			this._columnTypeFlagsChanged = (newFlag != this._columnTypeFlags);
			this._columnTypeFlags = newFlag;
			this._selectedItems.Clear();
			bool flag;
			if (this._config.InitialSelectedItems != null)
			{
				SelectItemConfig config = this._config;
				flag = (config == null || config.OperationMode != ESelectItemOperationMode.NoPreSelect);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				bool splitSelectedAmountIntoSingleEntries = this.SplitSelectedAmountIntoSingleEntries;
				if (splitSelectedAmountIntoSingleEntries)
				{
					foreach (SelectedItemData item in this._config.InitialSelectedItems)
					{
						bool flag3 = ((item != null) ? item.ItemData : null) == null || item.SelectedAmount <= 0;
						if (!flag3)
						{
							for (int i = 0; i < item.SelectedAmount; i++)
							{
								this._selectedItems.Add(new SelectedItemData(item.ItemData, 1));
							}
						}
					}
				}
				else
				{
					this._selectedItems.AddRange(this._config.InitialSelectedItems);
				}
			}
			this._selectedAreaExpanded = ViewSelectItem._selectedAreaExpandedPersistent;
			this.selectedAreaSwitchToggle.SetIsOnWithoutNotify(this._selectedAreaExpanded);
			bool flag4 = this._sortAndFilterController != null;
			if (flag4)
			{
				SelectItemConfig config2 = this._config;
				string sortKey = (config2 != null) ? config2.SortKey : null;
				bool flag5 = string.IsNullOrWhiteSpace(sortKey);
				if (flag5)
				{
					sortKey = "SelectItem";
				}
				this._sortAndFilterController.InitSaveSortKey(sortKey);
			}
			else
			{
				this._pendingClearSort = true;
			}
			this._currentFilterType = ((this._useCustomColumns || this._sortAndFilterController == null) ? ESelectItemFilterType.All : this._sortAndFilterController.GetCurrentFilterType());
			this._needRebuildColumns = true;
			this._selectedScrollInitialized = false;
			this._searchText = string.Empty;
			bool flag6 = this.searchText != null;
			if (flag6)
			{
				this.searchText.SetTextWithoutNotify(string.Empty);
			}
			bool flag7 = this._config.CustomTextToolTipSetter == null;
			if (flag7)
			{
				bool flag8 = string.IsNullOrEmpty(this._config.CustomTextTips);
				if (flag8)
				{
					this.CustomTextTipDisplayer.enabled = false;
				}
				else
				{
					this.CustomTextTipDisplayer.enabled = true;
					this.CustomTextTipDisplayer.Type = TipType.SingleDesc;
					this.CustomTextTipDisplayer.IsLanguageKey = false;
					TooltipInvoker customTextTipDisplayer = this.CustomTextTipDisplayer;
					if (customTextTipDisplayer.RuntimeParam == null)
					{
						customTextTipDisplayer.RuntimeParam = new ArgumentBox();
					}
					this.CustomTextTipDisplayer.RuntimeParam.Set("arg0", this._config.CustomTextTips);
				}
			}
			this.RebuildAvailableMainFilterTogglesCache();
			this.ApplyMainFilterToggleVisibility();
			this.ApplySourceAndSortFilterLayoutVisibility();
			this.ResetItemSourceTypeToFirstValid();
			bool showProfessionPreview = this._config.ShowProfessionPreview;
			if (showProfessionPreview)
			{
				UIElement.ProfessionPreview.Show();
			}
			this.NeedDataListenerId = true;
			this.Element.OnListenerIdReady = new Action(this.OnListenerIdReady);
			this._isCardMode = true;
			this.RefreshCardMode();
		}

		// Token: 0x06005F99 RID: 24473 RVA: 0x002BD8E4 File Offset: 0x002BBAE4
		private IEnumerable<ColumnDefinition> GenerateColumnDefinitions()
		{
			return SelectItemColumnHelper.GetColumnDefinitionsByFlags(this._columnTypeFlags);
		}

		// Token: 0x06005F9A RID: 24474 RVA: 0x002BD904 File Offset: 0x002BBB04
		private void PrepareRowTemplateContainers()
		{
			Transform containerRoot = this.rowTemplate.ContainerRoot;
			for (int i = containerRoot.childCount - 1; i >= 0; i--)
			{
				Transform child = containerRoot.GetChild(i);
				bool flag = child.GetComponent<RowCellContainer>() != null;
				if (flag)
				{
					Object.Destroy(child.gameObject);
				}
			}
			List<ColumnDefinition> columnDefinitions = this.GetCurrentColumnDefinitions(false).ToList<ColumnDefinition>();
			foreach (ColumnDefinition columnDef in columnDefinitions)
			{
				bool flag2 = columnDef is ColumnDefinition<ITradeableContent, ITradeableContent>;
				RowCellContainer containerTemplate;
				if (flag2)
				{
					containerTemplate = this.itemIconAndNameCellContainer;
				}
				else
				{
					bool flag3 = columnDef is ColumnDefinition<ITradeableContent, BookPageInfoData>;
					if (flag3)
					{
						containerTemplate = this.bookPageInfoCellContainer;
					}
					else
					{
						containerTemplate = this.singleTextCellContainer;
					}
				}
				bool flag4 = containerTemplate == null;
				if (flag4)
				{
					Debug.LogError(string.Format("[ViewSelectItem] containerTemplate is null for column: {0}", columnDef.TableHeadLabel));
				}
				else
				{
					RowCellContainer container = Object.Instantiate<RowCellContainer>(containerTemplate, containerRoot);
					container.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x06005F9B RID: 24475 RVA: 0x002BDA34 File Offset: 0x002BBC34
		private void Awake()
		{
			this.scroll.OnRowClicked += this.OnClickItem;
			this.itemSourceType.Init(0);
			this.itemSourceType.OnActiveIndexChange += this.ItemSourceTypeOnOnActiveIndexChange;
			this.rowTemplate.gameObject.SetActive(false);
			this.singleTextCellContainer.gameObject.SetActive(false);
			this.itemIconAndNameCellContainer.gameObject.SetActive(false);
			bool flag = this.bookPageInfoCellContainer != null;
			if (flag)
			{
				this.bookPageInfoCellContainer.gameObject.SetActive(false);
			}
			this.InitSortAndFilter();
			this.RebuildAvailableMainFilterTogglesCache();
			this.ApplyMainFilterToggleVisibility();
			this.PrepareRowTemplateContainers();
			this.scroll.SetRowTemplate(this.rowTemplate);
			this.selectedScroll.SetRowTemplate(this.rowTemplate);
			this.scroll.Init<ITradeableContent>(this.GetCurrentColumnDefinitions(false), true, new Action<int, GameObject>(this.OnRowRender), null);
			this.selectedScroll.Init<ITradeableContent>(this.GetSelectedAreaColumnDefinitions(), true, null, null);
			this.scroll.SetSortController(this._sortAndFilterController);
			this.selectedScroll.SetSortController(this._sortAndFilterController);
			this.cardScroll.Init<ITradeableContent>(this.GetCurrentColumnDefinitions(false), true, new Action<int, GameObject>(this.OnCardItemRender), new Action<int, RowItem>(this.OnClickItem));
			this.cardScroll.SetSortController(this._sortAndFilterController);
			this.selectedCardScroll.ClearInfinityScrollCache();
			this.selectedCardScroll.Init<ItemDisplayData>(this.GetSelectedListColumnDefinitions(), true, new Action<int, GameObject>(this.OnCardItemRenderForSelected), null);
			this.selectedCardScroll.RowSelectedProvider = new Func<int, object, bool>(this.IsActiveSelectedItem);
			this.selectedScroll.RowSelectedProvider = new Func<int, object, bool>(this.IsActiveSelectedItem);
			this.cardModeToggleGroup.Init(1);
			this.cardModeToggleGroup.OnActiveIndexChange += this.OnCardModeToggleChanged;
			this.RefreshCardMode();
			this.RefreshCardModeForSelected();
			this.selectedAreaSwitchToggle.isOn = this._selectedAreaExpanded;
			this.selectedAreaSwitchToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnSelectedAreaSwitchChanged));
			this.btnFill.ClearAndAddListener(new Action(this.OnSelectAll));
			this.btnClear.ClearAndAddListener(new Action(this.OnDeselectAll));
			this.selectedScroll.OnRowClicked += this.OnSelectedRowClicked;
			this.selectedCardScroll.OnRowClicked += this.OnSelectedRowClicked;
			this.closeButton.ClearAndAddListener(new Action(this.OnCloseClicked));
			bool flag2 = this.searchText != null;
			if (flag2)
			{
				this.searchText.onEndEdit.AddListener(new UnityAction<string>(this.OnSearchInputChanged));
				this.searchText.onValueChanged.AddListener(new UnityAction<string>(this.OnSearchInputChanged));
			}
		}

		// Token: 0x06005F9C RID: 24476 RVA: 0x002BDD34 File Offset: 0x002BBF34
		private void OnDisable()
		{
			bool showProfessionPreview = this._config.ShowProfessionPreview;
			if (showProfessionPreview)
			{
				UIElement.ProfessionPreview.Hide(false);
			}
		}

		// Token: 0x06005F9D RID: 24477 RVA: 0x002BDD60 File Offset: 0x002BBF60
		private void OnSelectedRowClicked(int index, RowItem rowItem)
		{
			bool flag = index < 0 || index >= this._selectedItems.Count;
			if (!flag)
			{
				this._selectedItems.RemoveAt(index);
				this.Refresh();
			}
		}

		// Token: 0x06005F9E RID: 24478 RVA: 0x002BDDA0 File Offset: 0x002BBFA0
		private void InitSortAndFilter()
		{
			SelectItemConfig config = this._config;
			string sortKey = (config != null) ? config.SortKey : null;
			bool flag = string.IsNullOrWhiteSpace(sortKey);
			if (flag)
			{
				sortKey = "SelectItem";
			}
			this._sortAndFilterController = new ItemSortAndFilterController(this.GetSortAndFilterView(), LanguageKey.EventEditor_Error_DuplicateGroupKey);
			this._sortAndFilterController.Init(new Action(this.OnSortAndFilterChanged), sortKey);
			this._sortAndFilterController.OnFilterTypeChanged += this.OnFilterTypeChanged;
			this._sortAndFilterController.ItemSortController.EnablePriorityCompare = false;
			bool pendingClearSort = this._pendingClearSort;
			if (pendingClearSort)
			{
				this._sortAndFilterController.SetSortState(new SortStateData
				{
					ItemStates = new List<SortItemState>()
				});
				this._pendingClearSort = false;
			}
		}

		// Token: 0x06005F9F RID: 24479 RVA: 0x002BDE58 File Offset: 0x002BC058
		private void OnFilterTypeChanged(ESelectItemFilterType newFilterType)
		{
			bool useCustomColumns = this._useCustomColumns;
			if (!useCustomColumns)
			{
				bool flag = this._currentFilterType != newFilterType;
				if (flag)
				{
					this._currentFilterType = newFilterType;
					this._needRebuildColumns = true;
				}
			}
		}

		// Token: 0x06005FA0 RID: 24480 RVA: 0x002BDE92 File Offset: 0x002BC092
		private void OnSortAndFilterChanged()
		{
			this.Refresh();
		}

		// Token: 0x06005FA1 RID: 24481 RVA: 0x002BDE9C File Offset: 0x002BC09C
		private void OnListenerIdReady()
		{
			bool usingExternalData = this._usingExternalData;
			if (usingExternalData)
			{
				this.Refresh();
				this.Element.ShowAfterRefresh();
			}
			else
			{
				int gameDataListenerId = this.Element.GameDataListenerId;
				SelectItemConfig config = this._config;
				TaiwuDomainMethod.Call.GetAllItemsForSelect(gameDataListenerId, (config != null) ? config.Rules : null);
			}
		}

		// Token: 0x06005FA2 RID: 24482 RVA: 0x002BDEF0 File Offset: 0x002BC0F0
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				bool flag = notification.Type == 1;
				if (flag)
				{
					bool flag2 = notification.DomainId == 5 && notification.MethodId == 193;
					if (flag2)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._data);
						this.RebuildAvailableMainFilterTogglesCache();
						this.ApplyMainFilterToggleVisibility();
						this.Refresh();
						this.Element.ShowAfterRefresh();
					}
				}
			}
		}

		// Token: 0x06005FA3 RID: 24483 RVA: 0x002BDFB0 File Offset: 0x002BC1B0
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = btnName == "Close";
			if (flag)
			{
				this.TryClose();
			}
			else
			{
				bool flag2 = btnName == "Confirm";
				if (flag2)
				{
					this.OnClickConfirm();
				}
			}
		}

		// Token: 0x06005FA4 RID: 24484 RVA: 0x002BDFF4 File Offset: 0x002BC1F4
		private void TryClose()
		{
			bool flag = !this._config.CanClose;
			if (!flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x06005FA5 RID: 24485 RVA: 0x002BE01D File Offset: 0x002BC21D
		private void OnCloseClicked()
		{
			this.TryClose();
		}

		// Token: 0x06005FA6 RID: 24486 RVA: 0x002BE028 File Offset: 0x002BC228
		private void Update()
		{
			bool triggerButtonDown = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
			this.inputCounter -= Time.deltaTime;
			bool flag = this.IsSearchInputActive();
			if (flag)
			{
				this.inputCounter = 0.2f;
			}
			bool flag2 = this.confirm.interactable && triggerButtonDown && this.inputCounter < 0f;
			if (flag2)
			{
				this.OnClickConfirm();
			}
		}

		// Token: 0x06005FA7 RID: 24487 RVA: 0x002BE0A4 File Offset: 0x002BC2A4
		private bool IsSearchInputActive()
		{
			return this.searchText != null && this.searchText.isFocused;
		}

		// Token: 0x06005FA8 RID: 24488 RVA: 0x002BE0D2 File Offset: 0x002BC2D2
		public override void QuickHide()
		{
			this.ClearSearchInput();
			base.QuickHide();
		}

		// Token: 0x06005FA9 RID: 24489 RVA: 0x002BE0E4 File Offset: 0x002BC2E4
		private void OnRowRender(int index, GameObject rowObj)
		{
			RowItem rowItem = rowObj.GetComponent<RowItem>();
			bool flag = rowItem == null || rowItem.TipDisplayer == null;
			if (!flag)
			{
				ITradeableContent data = this._filteredData.GetOrDefault(index);
				ItemDisplayData itemData = data as ItemDisplayData;
				bool flag2 = itemData == null;
				if (flag2)
				{
					rowItem.TipDisplayer.enabled = false;
				}
				else
				{
					TooltipInvoker tipDisplayer = rowItem.TipDisplayer;
					tipDisplayer.enabled = true;
					tipDisplayer.RuntimeParam = null;
					bool isSkillBook = itemData.Key.ItemType == 10;
					RowItemLine.SetMouseTipDisplayer(isSkillBook, itemData, tipDisplayer);
					bool isDisabled = this.CheckRowDisable(index, itemData);
					rowItem.SetInteractable(!isDisabled, true);
					rowItem.SetDisabled(isDisabled);
					RowItemMain rowItemMain = rowObj.GetComponentInChildren<RowItemMain>();
					ViewSelectItem.ApplyItemSelectionBlockVisual(itemData, rowItemMain, isDisabled);
				}
			}
		}

		// Token: 0x06005FAA RID: 24490 RVA: 0x002BE1B2 File Offset: 0x002BC3B2
		private void ItemSourceTypeOnOnActiveIndexChange(int arg1, int arg2)
		{
			this.Refresh();
		}

		// Token: 0x06005FAB RID: 24491 RVA: 0x002BE1BC File Offset: 0x002BC3BC
		private void OnClickConfirm()
		{
			SelectItemConfig config = this._config;
			if (config != null)
			{
				SelectItemsCallback callback = config.Callback;
				if (callback != null)
				{
					callback(this.ActiveSelectedItems.ToList<SelectedItemData>());
				}
			}
			this.QuickHide();
		}

		// Token: 0x06005FAC RID: 24492 RVA: 0x002BE1F0 File Offset: 0x002BC3F0
		private void OnClickItem(int index, RowItem rowItem)
		{
			ITradeableContent itemData = this._filteredData.GetOrDefault(index);
			bool flag = itemData == null;
			if (!flag)
			{
				bool flag2 = UIManager.Instance.IsElementActive(UIElement.SetSelectCount);
				if (flag2)
				{
					bool flag3 = this.IsCurrentEditingItem(itemData);
					if (flag3)
					{
						GEvent.OnEvent(UiEvents.OnConfirmSetSelectCount, null);
					}
				}
				else
				{
					SelectItemConfig config = this._config;
					bool flag4 = config != null && config.MaxSelectCount == 1;
					if (flag4)
					{
						this.HandleSingleReplaceClick(index, rowItem);
					}
					else
					{
						this.HandleNormalClick(index, rowItem);
					}
					this.Refresh();
				}
			}
		}

		// Token: 0x06005FAD RID: 24493 RVA: 0x002BE284 File Offset: 0x002BC484
		private void HandleSingleReplaceClick(int index, RowItem rowItem)
		{
			ITradeableContent itemData = this._filteredData.GetOrDefault(index);
			bool flag = itemData == null;
			if (!flag)
			{
				SelectedItemData existItem = this.FindSelectedItem(itemData);
				bool flag2 = existItem != null;
				if (flag2)
				{
					this._selectedItems.Remove(existItem);
				}
				else
				{
					bool flag3 = itemData.Amount <= 1;
					if (flag3)
					{
						this._selectedItems.Clear();
						this._selectedItems.Add(new SelectedItemData(itemData, 1));
					}
					else
					{
						bool flag4;
						if (ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId))
						{
							SelectItemConfig config = this._config;
							flag4 = (((config != null) ? new int?(config.ResourceMaxValue) : null) >= 0);
						}
						else
						{
							flag4 = false;
						}
						bool flag5 = flag4;
						if (flag5)
						{
							this.ShowSelectCountPopup(itemData, rowItem, delegate(int count)
							{
								this._selectedItems.Clear();
								this._selectedItems.Add(new SelectedItemData(itemData, count));
							});
						}
						else
						{
							bool flag6 = this._config.SelectItemMode == ESelectItemMode.ItemSelect;
							if (flag6)
							{
								this._selectedItems.Clear();
								this._selectedItems.Add(new SelectedItemData(itemData, 1));
							}
							else
							{
								this.ShowSelectCountPopup(itemData, rowItem, delegate(int count)
								{
									this._selectedItems.Clear();
									this._selectedItems.Add(new SelectedItemData(itemData, count));
								});
							}
						}
					}
				}
			}
		}

		// Token: 0x06005FAE RID: 24494 RVA: 0x002BE410 File Offset: 0x002BC610
		private void HandleNormalClick(int index, RowItem rowItem)
		{
			ITradeableContent itemData = this._filteredData.GetOrDefault(index);
			bool flag = itemData == null;
			if (!flag)
			{
				bool splitSelectedAmountIntoSingleEntries = this.SplitSelectedAmountIntoSingleEntries;
				if (splitSelectedAmountIntoSingleEntries)
				{
					int activeSelectedAmount = this.GetActiveSelectedAmount(itemData);
					bool flag2 = activeSelectedAmount > 0;
					if (flag2)
					{
						this.RemoveSingleSelectedEntry(itemData);
					}
					else
					{
						int remainingAmount = this.GetRemainingSelectableAmount(itemData);
						bool flag3 = remainingAmount <= 0;
						if (!flag3)
						{
							bool flag4 = remainingAmount > 1;
							if (flag4)
							{
								this.ShowSelectCountPopup(itemData, rowItem, delegate(int count)
								{
									this.AddSelectedItemsAsSingleUnits(itemData, count);
								});
							}
							else
							{
								this.AddSelectedItemsAsSingleUnits(itemData, 1);
							}
						}
					}
				}
				else
				{
					SelectedItemData existing = this.FindSelectedItem(itemData);
					bool flag5 = existing != null;
					if (flag5)
					{
						this._selectedItems.Remove(existing);
					}
					else
					{
						bool flag6 = itemData.Amount > 1;
						if (flag6)
						{
							this.ShowSelectCountPopup(itemData, rowItem, null);
						}
						else
						{
							this.AddSelectedItem(itemData, 1);
						}
					}
				}
			}
		}

		// Token: 0x06005FAF RID: 24495 RVA: 0x002BE548 File Offset: 0x002BC748
		private void ShowSelectCountPopup(ITradeableContent itemData, RowItem rowItem, Action<int> onConfirm = null)
		{
			bool flag = rowItem == null;
			if (!flag)
			{
				this._editingItemKey = itemData.RealKey;
				this._editingItemData = itemData;
				RectTransform itemRectTrans = rowItem.transform as RectTransform;
				Transform originalParent = rowItem.transform.parent;
				int originalSiblingIndex = rowItem.transform.GetSiblingIndex();
				int maxSelectable = this.GetRemainingSelectableAmount(itemData);
				bool flag2 = maxSelectable <= 0;
				if (!flag2)
				{
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					argBox.Set("MaxCount", maxSelectable);
					argBox.Set("InitCount", maxSelectable);
					argBox.Set("MinCount", 1);
					argBox.SetObject("ItemRectTrans", itemRectTrans);
					argBox.SetObject("OnConfirmSetCount", new Action<int>(delegate(int count)
					{
						bool flag3 = count > 0;
						if (flag3)
						{
							bool flag4 = onConfirm != null;
							if (flag4)
							{
								onConfirm(count);
							}
							else
							{
								bool splitSelectedAmountIntoSingleEntries = this.SplitSelectedAmountIntoSingleEntries;
								if (splitSelectedAmountIntoSingleEntries)
								{
									this.AddSelectedItemsAsSingleUnits(itemData, count);
								}
								else
								{
									this.AddSelectedItem(itemData, count);
								}
							}
							this.Refresh();
						}
						this.scroll.OnPointerEnter();
					}));
					Action onShow = null;
					Action onHide = null;
					onShow = delegate()
					{
						UIElement setSelectCount3 = UIElement.SetSelectCount;
						setSelectCount3.OnShowed = (Action)Delegate.Remove(setSelectCount3.OnShowed, onShow);
						bool isCardMode = this._isCardMode;
						if (isCardMode)
						{
							this.cardScroll.InfiniteScroll.Scroll.SetScrollEnable(false);
						}
						else
						{
							this.scroll.InfiniteScroll.Scroll.SetScrollEnable(false);
						}
						ViewSetSelectCount viewSetSelectCount = UIElement.SetSelectCount.UiBase as ViewSetSelectCount;
						bool flag3 = viewSetSelectCount != null;
						if (flag3)
						{
							rowItem.transform.SetParent(UIElement.SetSelectCount.UiBase.transform);
							viewSetSelectCount.ShowTip();
							viewSetSelectCount.SetTipPosition(rowItem.transform as RectTransform);
						}
						rowItem.SetClickEvent(delegate
						{
							GEvent.OnEvent(UiEvents.OnConfirmSetSelectCount, null);
						});
					};
					onHide = delegate()
					{
						UIElement setSelectCount3 = UIElement.SetSelectCount;
						setSelectCount3.OnHide = (Action)Delegate.Remove(setSelectCount3.OnHide, onHide);
						this._editingItemData = null;
						bool isCardMode = this._isCardMode;
						if (isCardMode)
						{
							this.cardScroll.InfiniteScroll.Scroll.SetScrollEnable(true);
							this.cardScroll.InfiniteScroll.ReRender();
						}
						else
						{
							this.scroll.InfiniteScroll.Scroll.SetScrollEnable(true);
							this.scroll.InfiniteScroll.ReRender();
						}
						bool flag3 = rowItem != null;
						if (flag3)
						{
							rowItem.transform.SetParent(originalParent);
							rowItem.transform.SetSiblingIndex(originalSiblingIndex);
						}
						ViewSetSelectCount viewSetSelectCount = UIElement.SetSelectCount.UiBase as ViewSetSelectCount;
						bool flag4 = viewSetSelectCount != null;
						if (flag4)
						{
							viewSetSelectCount.HideTip();
						}
					};
					UIElement setSelectCount = UIElement.SetSelectCount;
					setSelectCount.OnShowed = (Action)Delegate.Combine(setSelectCount.OnShowed, onShow);
					UIElement setSelectCount2 = UIElement.SetSelectCount;
					setSelectCount2.OnHide = (Action)Delegate.Combine(setSelectCount2.OnHide, onHide);
					UIElement.SetSelectCount.SetOnInitArgs(argBox);
					UIManager.Instance.ShowUI(UIElement.SetSelectCount, true);
				}
			}
		}

		// Token: 0x06005FB0 RID: 24496 RVA: 0x002BE6EC File Offset: 0x002BC8EC
		private int AddSelectedItem(ITradeableContent itemData, int amount)
		{
			bool flag = this._config.MaxSelectCount > 0;
			if (flag)
			{
				bool flag2 = this._config.SelectItemMode == ESelectItemMode.ItemSelect;
				if (flag2)
				{
					int remaining = this._config.MaxSelectCount - this.TotalSelectedAmount;
					bool flag3 = remaining <= 0;
					if (flag3)
					{
						return 0;
					}
					amount = Math.Min(amount, remaining);
				}
				else
				{
					bool flag4 = this._config.MaxSelectCount <= this.TotalSelectedRow;
					if (flag4)
					{
						return 0;
					}
				}
			}
			bool flag5 = amount > 0;
			if (flag5)
			{
				this._selectedItems.Add(new SelectedItemData(itemData, amount));
			}
			return amount;
		}

		// Token: 0x06005FB1 RID: 24497 RVA: 0x002BE796 File Offset: 0x002BC996
		private void OnSelectedAreaSwitchChanged(bool isOn)
		{
			this._selectedAreaExpanded = isOn;
			ViewSelectItem._selectedAreaExpandedPersistent = isOn;
			this.UpdateSelectedArea();
		}

		// Token: 0x06005FB2 RID: 24498 RVA: 0x002BE7B0 File Offset: 0x002BC9B0
		private void OnSelectAll()
		{
			using (List<ITradeableContent>.Enumerator enumerator = this._filteredData.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ITradeableContent item = enumerator.Current;
					bool flag = this.IsItemSelectionBlocked(item);
					if (!flag)
					{
						bool flag2 = this.CheckReachMaxSelectAmount();
						if (flag2)
						{
							break;
						}
						bool flag3 = !this._config.IsAllowSameTemplateIdItem && this._selectedItems.Any((SelectedItemData x) => x.ItemData.Key.TemplateId == item.Key.TemplateId);
						if (!flag3)
						{
							int amountToAdd = this.GetRemainingSelectableAmount(item);
							bool flag4 = amountToAdd <= 0;
							if (!flag4)
							{
								bool flag5 = this._config.MaxSelectCount > 0 && this._config.SelectItemMode == ESelectItemMode.ItemSelect;
								if (flag5)
								{
									int remaining = this._config.MaxSelectCount - this.TotalSelectedAmount;
									amountToAdd = Math.Min(amountToAdd, remaining);
								}
								else
								{
									bool flag6 = this._config.MaxSelectCount > 0 && this._config.SelectItemMode == ESelectItemMode.RowSelect && this.SplitSelectedAmountIntoSingleEntries;
									if (flag6)
									{
										int remainingRows = this._config.MaxSelectCount - this.TotalSelectedRow;
										amountToAdd = Math.Min(amountToAdd, remainingRows);
									}
								}
								bool flag7 = amountToAdd <= 0;
								if (!flag7)
								{
									bool splitSelectedAmountIntoSingleEntries = this.SplitSelectedAmountIntoSingleEntries;
									if (splitSelectedAmountIntoSingleEntries)
									{
										this.AddSelectedItemsAsSingleUnits(item, amountToAdd);
									}
									else
									{
										this._selectedItems.Add(new SelectedItemData(item, amountToAdd));
									}
								}
							}
						}
					}
				}
			}
			this.Refresh();
		}

		// Token: 0x06005FB3 RID: 24499 RVA: 0x002BE974 File Offset: 0x002BCB74
		private bool CheckReachMaxSelectAmount()
		{
			bool flag = this._config.MaxSelectCount > 0 && this._config.SelectItemMode == ESelectItemMode.ItemSelect && this.TotalSelectedAmount >= this._config.MaxSelectCount;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = this._config.MaxSelectCount > 0 && this._config.SelectItemMode == ESelectItemMode.RowSelect && this.TotalSelectedRow >= this._config.MaxSelectCount;
				result = flag2;
			}
			return result;
		}

		// Token: 0x06005FB4 RID: 24500 RVA: 0x002BEA02 File Offset: 0x002BCC02
		private void OnDeselectAll()
		{
			this._selectedItems.Clear();
			this.Refresh();
		}

		// Token: 0x06005FB5 RID: 24501 RVA: 0x002BEA18 File Offset: 0x002BCC18
		private string GetSelectedAmountText(ITradeableContent data)
		{
			bool flag = data == null;
			string result;
			if (flag)
			{
				result = "0";
			}
			else
			{
				bool splitSelectedAmountIntoSingleEntries = this.SplitSelectedAmountIntoSingleEntries;
				if (splitSelectedAmountIntoSingleEntries)
				{
					result = "1/1";
				}
				else
				{
					SelectedItemData selected = this.FindSelectedItem(data);
					result = ((selected != null) ? selected.SelectedAmount.ToString() : "0");
				}
			}
			return result;
		}

		// Token: 0x06005FB6 RID: 24502 RVA: 0x002BEA69 File Offset: 0x002BCC69
		private IEnumerable<ColumnDefinition> GetSelectedListColumnDefinitions()
		{
			ColumnDefinition<ITradeableContent, ITradeableContent> columnDefinition = new ColumnDefinition<ITradeableContent, ITradeableContent>();
			columnDefinition.LayoutOption = new LayoutOption
			{
				MinWidth = 150f,
				FlexibleWidth = 1f,
				PreferredWidth = 300f,
				Priority = 1
			};
			columnDefinition.TableHeadLabel = (() => LanguageKey.LK_Item.Tr());
			columnDefinition.CellDataGenerator = ((ITradeableContent data) => data);
			columnDefinition.SortId = -1;
			yield return columnDefinition;
			ColumnDefinition<ITradeableContent, string> columnDefinition2 = new ColumnDefinition<ITradeableContent, string>();
			columnDefinition2.LayoutOption = new LayoutOption
			{
				MinWidth = 50f,
				FlexibleWidth = 0.5f,
				PreferredWidth = 80f,
				Priority = 1
			};
			columnDefinition2.TableHeadLabel = (() => LanguageKey.LK_Count.Tr());
			columnDefinition2.CellDataGenerator = new Func<ITradeableContent, string>(this.GetSelectedAmountText);
			columnDefinition2.SortId = -1;
			yield return columnDefinition2;
			yield break;
		}

		// Token: 0x06005FB7 RID: 24503 RVA: 0x002BEA79 File Offset: 0x002BCC79
		private void OnCardModeToggleChanged(int currentIndex, int previousIndex)
		{
			this._isCardMode = (currentIndex == 1);
			this.RefreshCardMode();
			this.Refresh();
		}

		// Token: 0x06005FB8 RID: 24504 RVA: 0x002BEA94 File Offset: 0x002BCC94
		private void RefreshCardMode()
		{
			this.cardScroll.gameObject.SetActive(this._isCardMode);
			this.scroll.gameObject.SetActive(!this._isCardMode);
			this.RefreshCardModeForSelected();
		}

		// Token: 0x06005FB9 RID: 24505 RVA: 0x002BEACF File Offset: 0x002BCCCF
		private void RefreshCardModeForSelected()
		{
			this.selectedCardScroll.gameObject.SetActive(this._isCardMode);
			this.selectedScroll.gameObject.SetActive(!this._isCardMode);
		}

		// Token: 0x06005FBA RID: 24506 RVA: 0x002BEB04 File Offset: 0x002BCD04
		private void OnCardItemRender(int index, GameObject rowObject)
		{
			bool flag = index < 0 || index >= this._filteredData.Count;
			if (!flag)
			{
				ITradeableContent rowData = this._filteredData[index];
				RowItemLine rowItemLine = rowObject.GetComponent<RowItemLine>();
				RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
				rowItemMain.SetData(rowData);
				rowItemLine.Set(rowItemMain, true);
				bool IsSpecial = rowData != null && rowData.Key.ItemType == 12 && (rowData.Key.TemplateId == 475 || rowData.Key.TemplateId == 389 || rowData.Key.TemplateId == 388 || rowData.Key.TemplateId >= 476 || rowData.Key.TemplateId == 483);
				int selectedAmount = this.GetActiveSelectedAmount(rowData);
				bool isDisabled = this.CheckRowDisable(index, rowData);
				rowItemLine.SetInteractable(!isDisabled, true);
				rowItemLine.SetDisabled(isDisabled);
				ViewSelectItem.ApplyItemSelectionBlockVisual(rowData, rowItemMain, isDisabled);
				bool flag2 = selectedAmount > 0;
				if (flag2)
				{
					rowItemMain.ItemBack.SetCountInfo(IsSpecial ? string.Empty : string.Format("{0}/{1}", selectedAmount, rowData.Amount), null);
				}
				else
				{
					rowItemMain.ItemBack.SetCount(IsSpecial ? 0 : rowData.Amount, false);
				}
			}
		}

		// Token: 0x06005FBB RID: 24507 RVA: 0x002BEC68 File Offset: 0x002BCE68
		private void OnCardItemRenderForSelected(int index, GameObject rowObject)
		{
			bool flag = index < 0 || index >= this._selectedItems.Count;
			if (!flag)
			{
				ITradeableContent rowData = this._selectedItems[index].ItemData;
				RowItemLine rowItemLine = rowObject.GetComponent<RowItemLine>();
				RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
				rowItemMain.SetData(rowData);
				rowItemLine.Set(rowItemMain, true);
				rowItemMain.ItemBack.SetCountInfo(this.GetSelectedAmountText(rowData), null);
			}
		}

		// Token: 0x06005FBC RID: 24508 RVA: 0x002BECDC File Offset: 0x002BCEDC
		private void Refresh()
		{
			this.UpdateTitleAndInfo();
			bool showOnlyInventory = this._config != null && this._config.Rules != null && this._config.Rules.OnlyFromInventory;
			this.ApplySourceAndSortFilterLayoutVisibility();
			int tempIndex = 0;
			foreach (CToggle item in this.itemSourceType.GetAll())
			{
				item.interactable = (tempIndex == 0 || !showOnlyInventory);
				tempIndex++;
			}
			bool needRebuild = this._columnTypeFlagsChanged || this._needRebuildColumns;
			bool flag = needRebuild;
			if (flag)
			{
				this._columnTypeFlagsChanged = false;
				this._needRebuildColumns = false;
				this.RebuildColumnStructure();
				SelectItemConfig config = this._config;
				string sortKey = (config != null) ? config.SortKey : null;
				bool flag2 = string.IsNullOrWhiteSpace(sortKey);
				if (flag2)
				{
					sortKey = "SelectItem";
				}
				this._sortAndFilterController.InitSaveSortKey(sortKey);
			}
			this.ApplySortAndFilter();
			this.UpdateConfirmButtonState();
			this.scroll.RowSelectedProvider = new Func<int, object, bool>(this.IsItemSelected);
			this.scroll.RowDisabledProvider = new Func<int, object, bool>(this.CheckRowDisable);
			bool isCardMode = this._isCardMode;
			if (isCardMode)
			{
				this.cardScroll.RowSelectedProvider = new Func<int, object, bool>(this.IsItemSelected);
				this.cardScroll.RowDisabledProvider = new Func<int, object, bool>(this.CheckRowDisable);
				this.cardScroll.SetData<ITradeableContent>(this._filteredData, -1);
			}
			else
			{
				this.scroll.SetData<ITradeableContent>(this._filteredData, -1);
			}
			this.UpdateSelectedArea();
			this.UpdateProfessionPreview();
		}

		// Token: 0x06005FBD RID: 24509 RVA: 0x002BEEA0 File Offset: 0x002BD0A0
		private void RebuildColumnStructure()
		{
			this.PrepareRowTemplateContainers();
			this.scroll.ClearInfinityScrollCache();
			this.scroll.Init<ITradeableContent>(this.GetCurrentColumnDefinitions(false), true, new Action<int, GameObject>(this.OnRowRender), null);
			this.scroll.SetSortController(this._sortAndFilterController);
			this.selectedScroll.ClearInfinityScrollCache();
			this.selectedScroll.Init<ITradeableContent>(this.GetSelectedAreaColumnDefinitions(), true, null, null);
			this.selectedScroll.SetSortController(this._sortAndFilterController);
			this.cardScroll.ClearInfinityScrollCache();
			this.cardScroll.Init<ITradeableContent>(this.GetCurrentColumnDefinitions(false), true, new Action<int, GameObject>(this.OnCardItemRender), new Action<int, RowItem>(this.OnClickItem));
			this.cardScroll.SetSortController(this._sortAndFilterController);
		}

		// Token: 0x06005FBE RID: 24510 RVA: 0x002BEF71 File Offset: 0x002BD171
		private IEnumerable<ColumnDefinition> GetSelectedAreaColumnDefinitions()
		{
			return this.GetCurrentColumnDefinitions(true);
		}

		// Token: 0x06005FBF RID: 24511 RVA: 0x002BEF7A File Offset: 0x002BD17A
		private IEnumerable<ColumnDefinition> GetCurrentColumnDefinitions(bool forSelectedArea = false)
		{
			bool useCustomColumns = this._useCustomColumns;
			IEnumerable<ColumnDefinition> columns;
			if (useCustomColumns)
			{
				columns = this.GenerateColumnDefinitions();
			}
			else
			{
				columns = SelectItemColumnHelper.GetColumnDefinitions(this._currentFilterType);
			}
			Func<ITradeableContent, string> <>9__0;
			foreach (ColumnDefinition col in columns)
			{
				ColumnDefinition<ITradeableContent, string> amountCol = col as ColumnDefinition<ITradeableContent, string>;
				bool flag = amountCol != null && amountCol.SortId == 17;
				if (flag)
				{
					ColumnDefinition<ITradeableContent, string> columnDefinition = amountCol;
					Func<ITradeableContent, string> cellDataGenerator;
					if ((cellDataGenerator = <>9__0) == null)
					{
						cellDataGenerator = (<>9__0 = delegate(ITradeableContent data)
						{
							bool flag2 = forSelectedArea && this.SplitSelectedAmountIntoSingleEntries;
							string result;
							if (flag2)
							{
								result = "1/1";
							}
							else
							{
								int selectedAmount = this.GetActiveSelectedAmount(data);
								bool flag3 = data.GetContentType() > 0;
								if (flag3)
								{
									result = "-";
								}
								else
								{
									bool flag4 = selectedAmount > 0;
									if (flag4)
									{
										result = string.Format("{0}/{1}", selectedAmount, data.Amount);
									}
									else
									{
										result = data.Amount.ToString();
									}
								}
							}
							return result;
						});
					}
					columnDefinition.CellDataGenerator = cellDataGenerator;
				}
				yield return col;
				amountCol = null;
				col = null;
			}
			IEnumerator<ColumnDefinition> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06005FC0 RID: 24512 RVA: 0x002BEF94 File Offset: 0x002BD194
		private void UpdateTitleAndInfo()
		{
			bool flag = !string.IsNullOrEmpty(this._config.Title);
			if (flag)
			{
				this.titleLabel.text = this._config.Title;
			}
			else
			{
				this.titleLabel.text = LanguageKey.LK_UI_SelectItem.Tr();
			}
			bool flag2 = !string.IsNullOrEmpty(this._config.SelectedTitle);
			if (flag2)
			{
				this.titleSelectedItemLabel.text = this._config.Title;
			}
			else
			{
				this.titleSelectedItemLabel.text = LanguageKey.LK_SelectItem_Selected_AreaTitle.Tr();
			}
			Func<IReadOnlyList<SelectedItemData>, string> customTextGenerator = this._config.CustomTextGenerator;
			string customText = ((customTextGenerator != null) ? customTextGenerator(this.ActiveSelectedItems) : null) ?? string.Empty;
			bool showCustomText = !string.IsNullOrEmpty(customText) || this._config.CustomTextSetter != null;
			this.customTextArea.SetActive(showCustomText);
			bool flag3 = showCustomText;
			if (flag3)
			{
				this.customTextLabel.text = customText;
			}
			Action<IReadOnlyList<SelectedItemData>, TextMeshProUGUI, GameObject> customTextSetter = this._config.CustomTextSetter;
			if (customTextSetter != null)
			{
				customTextSetter(this.ActiveSelectedItems, this.customTextLabel, this.customTextArea);
			}
			Action<IReadOnlyList<SelectedItemData>, TooltipInvoker> customTextToolTipSetter = this._config.CustomTextToolTipSetter;
			if (customTextToolTipSetter != null)
			{
				customTextToolTipSetter(this.ActiveSelectedItems, this.CustomTextTipDisplayer);
			}
			this.closeButton.gameObject.SetActive(this._config.CanClose);
		}

		// Token: 0x06005FC1 RID: 24513 RVA: 0x002BF0F8 File Offset: 0x002BD2F8
		private void UpdateSelectedCountLabel()
		{
			int totalSelected = (this._config.SelectItemMode == ESelectItemMode.ItemSelect) ? this.TotalSelectedAmount : this.TotalSelectedRow;
			int maxCount = this._config.MaxSelectCount;
			string countText = (maxCount > 0) ? string.Format("{0}/{1}", totalSelected, maxCount) : totalSelected.ToString();
			bool flag = this._config.SelectedToggleKey == LanguageKey.Invalid;
			if (flag)
			{
				this.selectedCountLabel.text = LanguageKey.LK_SelectItem_SelectedLabel.TrFormat(countText);
			}
			else
			{
				this.selectedCountLabel.text = this._config.SelectedToggleKey.Tr();
			}
		}

		// Token: 0x06005FC2 RID: 24514 RVA: 0x002BF19C File Offset: 0x002BD39C
		private void UpdateSelectedArea()
		{
			bool showSelectedArea = this._config.ShowSelectedArea;
			this.selectedScrollArea.SetActive(showSelectedArea && this._selectedAreaExpanded);
			bool flag = this.selectedAreaSwitchRoot != null;
			if (flag)
			{
				this.selectedAreaSwitchRoot.SetActive(showSelectedArea);
			}
			else
			{
				this.selectedAreaSwitchToggle.gameObject.SetActive(showSelectedArea);
			}
			bool showProfessionPreview = this._config.ShowProfessionPreview;
			if (showProfessionPreview)
			{
				this.selectedAreaSwitchRoot.SetActive(false);
			}
			this.btnClear.gameObject.SetActive(this._config.ShowQuickButton);
			this.btnFill.gameObject.SetActive(this._config.ShowQuickButton);
			bool showQuickButton = this._config.ShowQuickButton;
			if (showQuickButton)
			{
				this.btnClear.interactable = (this._selectedItems.Count > 0);
				this.btnFill.interactable = !this.CheckSelectAll();
				this.btnClear.GetComponent<DisableStyleRoot>().SetStyleEffect(!this.btnClear.interactable, false);
				this.btnFill.GetComponent<DisableStyleRoot>().SetStyleEffect(!this.btnFill.interactable, false);
			}
			this.UpdateSelectedCountLabel();
			bool flag2 = showSelectedArea;
			if (flag2)
			{
				bool isCardMode = this._isCardMode;
				if (isCardMode)
				{
					this.selectedCardScroll.SetData<ITradeableContent>(from s in this._selectedItems
					select s.ItemData, -1);
				}
				else
				{
					this.selectedScroll.SetData<ITradeableContent>(from s in this._selectedItems
					select s.ItemData, -1);
				}
			}
		}

		// Token: 0x06005FC3 RID: 24515 RVA: 0x002BF360 File Offset: 0x002BD560
		private void UpdateProfessionPreview()
		{
			bool flag = !this._config.ShowProfessionPreview;
			if (!flag)
			{
				ViewProfessionPreview ui = UIElement.ProfessionPreview.UiBaseAs<ViewProfessionPreview>();
				bool flag2 = ui == null;
				if (!flag2)
				{
					bool flag3 = this._selectedItems.Count == 0;
					if (flag3)
					{
						ui.Set(-1);
					}
					else
					{
						ui.Set(ViewSelectItem.ProfessionItemMap.GetValueOrDefault(this._selectedItems[0].ItemData.RealKey.TemplateId, -1));
					}
				}
			}
		}

		// Token: 0x06005FC4 RID: 24516 RVA: 0x002BF3E4 File Offset: 0x002BD5E4
		private bool CheckSelectAll()
		{
			bool flag = this._config == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this._config.SelectItemMode == ESelectItemMode.ItemSelect;
				if (flag2)
				{
					bool flag3 = this._config.MaxSelectCount > 0 && this.TotalSelectedAmount >= this._config.MaxSelectCount;
					result = (flag3 || this.TotalSelectedAmount >= (from t in this._filteredData
					where !this.IsItemSelectionBlocked(t)
					select t).Sum(new Func<ITradeableContent, int>(this.GetResourceValueMaxSelectableAmount)));
				}
				else
				{
					bool flag4 = this._config.MaxSelectCount > 0 && this.TotalSelectedRow >= this._config.MaxSelectCount;
					if (flag4)
					{
						result = true;
					}
					else
					{
						bool splitSelectedAmountIntoSingleEntries = this.SplitSelectedAmountIntoSingleEntries;
						if (splitSelectedAmountIntoSingleEntries)
						{
							result = (this.TotalSelectedRow >= (from t in this._filteredData
							where !this.IsItemSelectionBlocked(t)
							select t).Sum(new Func<ITradeableContent, int>(this.GetResourceValueMaxSelectableAmount)));
						}
						else
						{
							result = (this.TotalSelectedRow >= this._filteredData.Count((ITradeableContent t) => !this.IsItemSelectionBlocked(t) && this.GetResourceValueMaxSelectableAmount(t) > 0));
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06005FC5 RID: 24517 RVA: 0x002BF524 File Offset: 0x002BD724
		private void PrepareSelectedRowTemplateContainers(RowItem template)
		{
			bool flag = template == null;
			if (!flag)
			{
				Transform containerRoot = template.ContainerRoot;
				for (int i = containerRoot.childCount - 1; i >= 0; i--)
				{
					Transform child = containerRoot.GetChild(i);
					bool flag2 = child.GetComponent<RowCellContainer>() != null;
					if (flag2)
					{
						Object.Destroy(child.gameObject);
					}
				}
				RowCellContainer c = Object.Instantiate<RowCellContainer>(this.itemIconAndNameCellContainer, containerRoot);
				c.gameObject.SetActive(true);
				RowCellContainer c2 = Object.Instantiate<RowCellContainer>(this.singleTextCellContainer, containerRoot);
				c2.gameObject.SetActive(true);
			}
		}

		// Token: 0x06005FC6 RID: 24518 RVA: 0x002BF5CC File Offset: 0x002BD7CC
		private void UpdateConfirmButtonState()
		{
			bool allowEmpty = this._config.AllowEmpty;
			bool canConfirm;
			if (allowEmpty)
			{
				canConfirm = true;
			}
			else
			{
				int totalSelected = this.TotalSelectedAmount;
				bool flag = this._config.MinSelectCount >= 0;
				if (flag)
				{
					canConfirm = (totalSelected >= this._config.MinSelectCount);
				}
				else
				{
					bool flag2 = this._config.MaxSelectCount > 0;
					if (flag2)
					{
						canConfirm = (totalSelected >= this._config.MaxSelectCount);
					}
					else
					{
						canConfirm = (totalSelected > 0);
					}
				}
			}
			this.confirm.interactable = canConfirm;
		}

		// Token: 0x06005FC7 RID: 24519 RVA: 0x002BF65C File Offset: 0x002BD85C
		private bool IsItemSelected(int index, object rowData)
		{
			ITradeableContent itemData = rowData as ITradeableContent;
			bool flag = itemData == null;
			return !flag && this.GetActiveSelectedAmount(itemData) > 0;
		}

		// Token: 0x06005FC8 RID: 24520 RVA: 0x002BF690 File Offset: 0x002BD890
		private bool IsItemSelectionBlocked(ITradeableContent itemData)
		{
			bool flag = itemData == null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				bool flag2 = !this._config.CanSelectLockedItem && itemData.IsLocked && itemData.OwnerCharId == taiwuCharId;
				result = (flag2 || itemData.UnavailableType > ItemDisplayData.ItemUnavailableType.Valid);
			}
			return result;
		}

		// Token: 0x06005FC9 RID: 24521 RVA: 0x002BF6EC File Offset: 0x002BD8EC
		private static string GetUnavailableTypeLockText(ItemDisplayData.ItemUnavailableType unavailableType)
		{
			if (!true)
			{
			}
			string result;
			switch (unavailableType)
			{
			case ItemDisplayData.ItemUnavailableType.NoMeat:
				result = LanguageKey.LK_ItemUnavailable_NoMeat.Tr();
				break;
			case ItemDisplayData.ItemUnavailableType.NoAlcohol:
				result = LanguageKey.LK_ItemUnavailable_NoAlcohol.Tr();
				break;
			case ItemDisplayData.ItemUnavailableType.HighGrade:
				result = LanguageKey.LK_ItemUnavailable_HighGrade.Tr();
				break;
			case ItemDisplayData.ItemUnavailableType.AttainmentNotMeet:
				result = LanguageKey.LK_Item_Operation_AttainmentNotMeet.Tr();
				break;
			default:
				result = LanguageKey.LK_Item_Operation_Locked.Tr();
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06005FCA RID: 24522 RVA: 0x002BF764 File Offset: 0x002BD964
		private static void ApplyItemSelectionBlockVisual(ITradeableContent itemData, RowItemMain rowItemMain, bool isDisabled)
		{
			bool flag = rowItemMain == null;
			if (!flag)
			{
				bool flag2 = !isDisabled;
				if (flag2)
				{
					bool flag3 = !itemData.IsLocked;
					if (flag3)
					{
						rowItemMain.HideInteractionState();
					}
				}
				else
				{
					bool flag4 = itemData.UnavailableType > ItemDisplayData.ItemUnavailableType.Valid;
					if (flag4)
					{
						rowItemMain.SetInteractionStateLockText(ViewSelectItem.GetUnavailableTypeLockText(itemData.UnavailableType));
					}
					else
					{
						rowItemMain.ShowInteractionStateLocked();
					}
				}
			}
		}

		// Token: 0x06005FCB RID: 24523 RVA: 0x002BF7C8 File Offset: 0x002BD9C8
		private bool CheckRowDisable(int index, object rowData)
		{
			bool flag = this.IsSameTemplateIdItemSelected(index, rowData);
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				ITradeableContent itemData = rowData as ITradeableContent;
				bool flag2 = itemData != null && this.GetActiveSelectedAmount(itemData) > 0;
				if (flag2)
				{
					result = false;
				}
				else
				{
					ITradeableContent blockedItem = rowData as ITradeableContent;
					bool flag3 = blockedItem != null && this.IsItemSelectionBlocked(blockedItem);
					if (flag3)
					{
						result = true;
					}
					else
					{
						ITradeableContent cappedItem = rowData as ITradeableContent;
						bool flag4 = cappedItem != null && this.GetActiveSelectedAmount(cappedItem) <= 0 && this.GetRemainingSelectableAmount(cappedItem) <= 0;
						if (flag4)
						{
							result = true;
						}
						else
						{
							bool flag5 = this._config.DisableWhenMaxSelected && this.CheckReachMaxSelectAmount();
							result = flag5;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06005FCC RID: 24524 RVA: 0x002BF888 File Offset: 0x002BDA88
		private bool IsSameTemplateIdItemSelected(int index, object rowData)
		{
			bool isAllowSameTemplateIdItem = this._config.IsAllowSameTemplateIdItem;
			bool result;
			if (isAllowSameTemplateIdItem)
			{
				result = false;
			}
			else
			{
				ITradeableContent itemData = rowData as ITradeableContent;
				bool flag = itemData == null;
				result = (flag || this._selectedItems.Any((SelectedItemData s) => s.ItemData.Key.TemplateId == itemData.Key.TemplateId && !this.IsSameSelectedItem(s.ItemData, itemData)));
			}
			return result;
		}

		// Token: 0x06005FCD RID: 24525 RVA: 0x002BF8F4 File Offset: 0x002BDAF4
		private bool IsSelectedItemCancelled(int index, object rowData)
		{
			return false;
		}

		// Token: 0x06005FCE RID: 24526 RVA: 0x002BF908 File Offset: 0x002BDB08
		private bool IsActiveSelectedItem(int index, object rowData)
		{
			bool flag = index < 0 || index >= this._selectedItems.Count;
			return !flag;
		}

		// Token: 0x06005FCF RID: 24527 RVA: 0x002BF93C File Offset: 0x002BDB3C
		private void ApplySortAndFilter()
		{
			this._filteredData.Clear();
			IReadOnlyList<ITradeableContent> sourceData = this.SelectedData;
			bool flag = this._sortAndFilterController == null;
			if (flag)
			{
				bool flag2 = sourceData != null;
				if (flag2)
				{
					this._filteredData.AddRange(sourceData);
				}
			}
			else
			{
				Func<ITradeableContent, bool> filter = this._sortAndFilterController.GenerateFilter();
				bool flag3 = sourceData == null;
				if (flag3)
				{
					this._sortAndFilterController.SetFilteredCount(0);
				}
				else
				{
					foreach (ITradeableContent item in sourceData)
					{
						bool flag4 = (item.GetContentType() != 0 || filter(item)) && this.IsItemMatchSearch(item);
						if (flag4)
						{
							this._filteredData.Add(item);
						}
					}
					Comparison<ITradeableContent> comparer = this._sortAndFilterController.GenerateComparer(this._filteredData);
					this._filteredData.Sort(comparer);
					this._sortAndFilterController.AfterFilter(sourceData);
				}
			}
		}

		// Token: 0x06005FD0 RID: 24528 RVA: 0x002BFA4C File Offset: 0x002BDC4C
		private bool IsItemMatchSearch(ITradeableContent item)
		{
			bool flag = item == null || string.IsNullOrWhiteSpace(this._searchText);
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				string itemName = item.GetName(false);
				result = (!string.IsNullOrEmpty(itemName) && itemName.IndexOf(this._searchText, StringComparison.OrdinalIgnoreCase) >= 0);
			}
			return result;
		}

		// Token: 0x06005FD1 RID: 24529 RVA: 0x002BFAA0 File Offset: 0x002BDCA0
		private void ResetItemSourceTypeToFirstValid()
		{
			bool showOnlyInventory = this._config != null && this._config.Rules != null && this._config.Rules.OnlyFromInventory;
			SelectItemConfig config = this._config;
			bool hideSourceToggles = config != null && config.HideSourceToggles;
			int targetIndex = (showOnlyInventory || hideSourceToggles) ? 0 : this.itemSourceType.GetFirstInteractable();
			bool flag = targetIndex < 0;
			if (flag)
			{
				targetIndex = 0;
			}
			bool flag2 = this.itemSourceType.GetActiveIndex() != targetIndex;
			if (flag2)
			{
				this.itemSourceType.SetWithoutNotify(targetIndex);
			}
		}

		// Token: 0x06005FD2 RID: 24530 RVA: 0x002BFB2C File Offset: 0x002BDD2C
		private void ApplySourceAndSortFilterLayoutVisibility()
		{
			bool showOnlyInventory = this._config != null && this._config.Rules != null && this._config.Rules.OnlyFromInventory;
			SelectItemConfig config = this._config;
			bool hideSourceToggles = config != null && config.HideSourceToggles;
			this.itemSourceType.gameObject.SetActive(!hideSourceToggles);
			bool flag = !hideSourceToggles;
			if (flag)
			{
				ItemSourceToggleHelper.RefreshInteractableForInteract(this.itemSourceType, !showOnlyInventory, false);
			}
			SelectItemConfig config2 = this._config;
			bool hideSortAndFilter = config2 != null && config2.HideSortAndFilter;
			ISortAndFilterView sortAndFilterView = this.GetSortAndFilterView();
			bool flag2 = this.sortAndFilterLayout.gameObject.activeSelf == hideSortAndFilter;
			if (flag2)
			{
				this.sortAndFilterLayout.gameObject.SetActive(!hideSortAndFilter);
			}
			bool isSortAndFilterVisible = sortAndFilterView.gameObject.activeSelf;
			this.contentTarget.SetParent(isSortAndFilterVisible ? this.sortFilterVisibleContainer : this.sortFilterHiddenContainer, false);
		}

		// Token: 0x06005FD3 RID: 24531 RVA: 0x002BFC1C File Offset: 0x002BDE1C
		private void ApplyMainFilterToggleVisibility()
		{
			bool flag = this._sortAndFilterController == null;
			if (!flag)
			{
				int lineId = 0;
				this._sortAndFilterController.ResetAllTogglesVisible(lineId);
				this._sortAndFilterController.ResetAllTogglesInteractable(lineId);
				List<int> visibleToggles = this.GetConfiguredMainFilterToggles();
				bool hasVisibleLimit = visibleToggles.Count > 0;
				bool flag2 = hasVisibleLimit;
				if (flag2)
				{
					this._sortAndFilterController.SetToggleVisible(new ToggleIdIndex(lineId, ToggleKey.AllKey), false);
					for (int index = 0; index < 7; index++)
					{
						bool isVisible = visibleToggles.Contains(index);
						this._sortAndFilterController.SetToggleVisible(new ToggleIdIndex(lineId, new ToggleKey
						{
							Index = index,
							IsAll = false
						}), isVisible);
					}
				}
				List<int> interactableToggles = this.GetInteractableMainFilterToggles(visibleToggles);
				this._sortAndFilterController.SetTogglesInteractable(lineId, interactableToggles);
				bool allowAllToggle = !hasVisibleLimit;
				this._sortAndFilterController.SetToggleInteractable(new ToggleIdIndex(lineId, ToggleKey.AllKey), allowAllToggle);
				int currentToggleIndex = ViewSelectItem.ConvertFilterTypeToMainFilterToggleIndex(this._sortAndFilterController.GetCurrentFilterType());
				bool isAll = currentToggleIndex < 0;
				bool isCurrentVisible = isAll ? allowAllToggle : (!hasVisibleLimit || visibleToggles.Contains(currentToggleIndex));
				bool isCurrentInteractable = isAll ? allowAllToggle : interactableToggles.Contains(currentToggleIndex);
				bool flag3 = !isCurrentVisible || !isCurrentInteractable;
				if (flag3)
				{
					currentToggleIndex = (allowAllToggle ? -1 : ((interactableToggles.Count > 0) ? interactableToggles[0] : (hasVisibleLimit ? visibleToggles[0] : -1)));
				}
				this._sortAndFilterController.SetToggleIsOnWithoutNotify(lineId, currentToggleIndex);
				ESelectItemFilterType newFilterType = this._sortAndFilterController.GetCurrentFilterType();
				bool flag4 = newFilterType != this._currentFilterType;
				if (flag4)
				{
					this._currentFilterType = newFilterType;
					this._needRebuildColumns = true;
				}
			}
		}

		// Token: 0x06005FD4 RID: 24532 RVA: 0x002BFDD4 File Offset: 0x002BDFD4
		private void RebuildAvailableMainFilterTogglesCache()
		{
			this._availableMainFilterTogglesBySource.Clear();
			ItemSourceType sourceType = ItemSourceType.Inventory;
			IReadOnlyList<ITradeableContent> items;
			if (!this._usingExternalData)
			{
				SelectItemDisplayData data = this._data;
				IReadOnlyList<ITradeableContent> readOnlyList = (data != null) ? data.InventoryItems : null;
				items = readOnlyList;
			}
			else
			{
				SelectItemConfig config = this._config;
				items = ((config != null) ? config.ExternalItems : null);
			}
			this.CacheAvailableMainFilterToggles(sourceType, items);
			ItemSourceType sourceType2 = ItemSourceType.Warehouse;
			IReadOnlyList<ITradeableContent> items2;
			if (!this._usingExternalData)
			{
				SelectItemDisplayData data2 = this._data;
				IReadOnlyList<ITradeableContent> readOnlyList = (data2 != null) ? data2.WarehouseItems : null;
				items2 = readOnlyList;
			}
			else
			{
				SelectItemConfig config2 = this._config;
				items2 = ((config2 != null) ? config2.ExternalWarehouseItems : null);
			}
			this.CacheAvailableMainFilterToggles(sourceType2, items2);
			ItemSourceType sourceType3 = ItemSourceType.Treasury;
			IReadOnlyList<ITradeableContent> items3;
			if (!this._usingExternalData)
			{
				SelectItemDisplayData data3 = this._data;
				IReadOnlyList<ITradeableContent> readOnlyList = (data3 != null) ? data3.TreasuryItems : null;
				items3 = readOnlyList;
			}
			else
			{
				SelectItemConfig config3 = this._config;
				items3 = ((config3 != null) ? config3.ExternalTreasuryItems : null);
			}
			this.CacheAvailableMainFilterToggles(sourceType3, items3);
		}

		// Token: 0x06005FD5 RID: 24533 RVA: 0x002BFE98 File Offset: 0x002BE098
		private void CacheAvailableMainFilterToggles(ItemSourceType sourceType, IReadOnlyList<ITradeableContent> items)
		{
			bool flag = items == null;
			if (!flag)
			{
				HashSet<int> toggles = new HashSet<int>();
				foreach (ITradeableContent item in items)
				{
					int toggle = ViewSelectItem.GetMainFilterToggleIndex(item);
					bool flag2 = toggle >= 0;
					if (flag2)
					{
						toggles.Add(toggle);
					}
				}
				this._availableMainFilterTogglesBySource[sourceType] = toggles;
			}
		}

		// Token: 0x06005FD6 RID: 24534 RVA: 0x002BFF1C File Offset: 0x002BE11C
		private List<int> GetConfiguredMainFilterToggles()
		{
			SelectItemConfig config = this._config;
			bool flag = ((config != null) ? config.VisibleMainFilterToggles : null) == null || this._config.VisibleMainFilterToggles.Count == 0;
			List<int> result;
			if (flag)
			{
				result = new List<int>();
			}
			else
			{
				result = (from index in this._config.VisibleMainFilterToggles
				where index >= 0 && index < 7
				select index).Distinct<int>().ToList<int>();
			}
			return result;
		}

		// Token: 0x06005FD7 RID: 24535 RVA: 0x002BFFA0 File Offset: 0x002BE1A0
		private List<int> GetInteractableMainFilterToggles(List<int> visibleToggles)
		{
			List<int> interactable = (visibleToggles.Count > 0) ? new List<int>(visibleToggles) : Enumerable.Range(0, 7).ToList<int>();
			HashSet<int> availableToggles;
			bool flag = this._availableMainFilterTogglesBySource.TryGetValue(this.SelectedSourceType, out availableToggles);
			if (flag)
			{
				interactable = interactable.Where(new Func<int, bool>(availableToggles.Contains)).ToList<int>();
			}
			return interactable;
		}

		// Token: 0x06005FD8 RID: 24536 RVA: 0x002C0004 File Offset: 0x002BE204
		private static int GetMainFilterToggleIndex(ITradeableContent item)
		{
			bool flag = item == null;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				bool flag2 = item.GetContentType() != 0;
				if (flag2)
				{
					result = 6;
				}
				else
				{
					sbyte itemType = item.RealKey.ItemType;
					bool flag3 = ItemFilterCommon.IsFood(itemType);
					if (flag3)
					{
						result = 0;
					}
					else
					{
						bool flag4 = ItemFilterCommon.IsMedicine(itemType);
						if (flag4)
						{
							result = 1;
						}
						else
						{
							bool flag5 = ItemFilterCommon.IsEquip(itemType);
							if (flag5)
							{
								result = 2;
							}
							else
							{
								bool flag6 = ItemFilterCommon.IsSkillBook(itemType);
								if (flag6)
								{
									result = 3;
								}
								else
								{
									bool flag7 = ItemFilterCommon.IsCraftTool(itemType);
									if (flag7)
									{
										result = 4;
									}
									else
									{
										bool flag8 = ItemFilterCommon.IsMaterial(itemType);
										if (flag8)
										{
											result = 5;
										}
										else
										{
											result = 6;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06005FD9 RID: 24537 RVA: 0x002C00A4 File Offset: 0x002BE2A4
		private static int ConvertFilterTypeToMainFilterToggleIndex(ESelectItemFilterType filterType)
		{
			if (!true)
			{
			}
			int result;
			switch (filterType)
			{
			case ESelectItemFilterType.Food:
				result = 0;
				break;
			case ESelectItemFilterType.Medicine:
				result = 1;
				break;
			case ESelectItemFilterType.Equipment:
			case ESelectItemFilterType.EquipmentWeapon:
			case ESelectItemFilterType.EquipmentArmor:
			case ESelectItemFilterType.EquipmentAccessory:
			case ESelectItemFilterType.EquipmentClothing:
			case ESelectItemFilterType.EquipmentCarrier:
				result = 2;
				break;
			case ESelectItemFilterType.Book:
				result = 3;
				break;
			case ESelectItemFilterType.Tool:
				result = 4;
				break;
			case ESelectItemFilterType.Material:
				result = 5;
				break;
			case ESelectItemFilterType.Misc:
			case ESelectItemFilterType.MiscCricket:
				result = 6;
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

		// Token: 0x06005FDA RID: 24538 RVA: 0x002C011D File Offset: 0x002BE31D
		private void OnSearchInputChanged(string text)
		{
			this._searchText = (text ?? string.Empty);
			this.Refresh();
		}

		// Token: 0x06005FDB RID: 24539 RVA: 0x002C0138 File Offset: 0x002BE338
		private void ClearSearchInput()
		{
			this._searchText = string.Empty;
			bool flag = this.searchText != null;
			if (flag)
			{
				this.searchText.SetTextWithoutNotify(string.Empty);
			}
		}

		// Token: 0x06005FDD RID: 24541 RVA: 0x002C01E4 File Offset: 0x002BE3E4
		// Note: this type is marked as 'beforefieldinit'.
		static ViewSelectItem()
		{
			ItemSourceType[] array = new ItemSourceType[3];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.4636993D3E1DA4E9D6B8F87B79E8F7C6D018580D52661950EABC3845C5897A4D).FieldHandle);
			ViewSelectItem.ToggleSourceTypes = array;
			ViewSelectItem.ProfessionItemMap = new Dictionary<short, int>
			{
				{
					286,
					0
				},
				{
					287,
					1
				},
				{
					288,
					2
				},
				{
					280,
					3
				},
				{
					281,
					4
				},
				{
					282,
					5
				},
				{
					283,
					6
				},
				{
					284,
					7
				},
				{
					285,
					8
				},
				{
					277,
					9
				},
				{
					278,
					10
				},
				{
					279,
					11
				},
				{
					289,
					12
				},
				{
					290,
					13
				},
				{
					291,
					14
				},
				{
					292,
					15
				},
				{
					293,
					16
				},
				{
					294,
					17
				}
			};
			ViewSelectItem._selectedAreaExpandedPersistent = false;
		}

		// Token: 0x04004221 RID: 16929
		private static readonly ItemSourceType[] ToggleSourceTypes;

		// Token: 0x04004222 RID: 16930
		private static readonly Dictionary<short, int> ProfessionItemMap;

		// Token: 0x04004223 RID: 16931
		[SerializeField]
		private CToggleGroup itemSourceType;

		// Token: 0x04004224 RID: 16932
		[SerializeField]
		private ListStyleGeneralScroll scroll;

		// Token: 0x04004225 RID: 16933
		[SerializeField]
		private CButton confirm;

		// Token: 0x04004226 RID: 16934
		[SerializeField]
		private CRawImage bg;

		// Token: 0x04004227 RID: 16935
		[SerializeField]
		private int sortingLayerNormal = 699;

		// Token: 0x04004228 RID: 16936
		[SerializeField]
		private int sortingLayerFront = 710;

		// Token: 0x04004229 RID: 16937
		[Header("行模板配置")]
		[SerializeField]
		private RowItem rowTemplate;

		// Token: 0x0400422A RID: 16938
		[SerializeField]
		private RowCellContainer singleTextCellContainer;

		// Token: 0x0400422B RID: 16939
		[SerializeField]
		private RowCellContainer itemIconAndNameCellContainer;

		// Token: 0x0400422C RID: 16940
		[SerializeField]
		private RowCellContainer bookPageInfoCellContainer;

		// Token: 0x0400422D RID: 16941
		[Header("排序筛选")]
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x0400422E RID: 16942
		[SerializeField]
		private GameObject sortAndFilterLayout;

		// Token: 0x0400422F RID: 16943
		[SerializeField]
		private CButton btnFill;

		// Token: 0x04004230 RID: 16944
		[SerializeField]
		private CButton btnClear;

		// Token: 0x04004231 RID: 16945
		[SerializeField]
		private TMP_InputField searchText;

		// Token: 0x04004232 RID: 16946
		[Header("排序筛选容器控制")]
		[SerializeField]
		private RectTransform sortFilterVisibleContainer;

		// Token: 0x04004233 RID: 16947
		[SerializeField]
		private RectTransform sortFilterHiddenContainer;

		// Token: 0x04004234 RID: 16948
		[SerializeField]
		private RectTransform contentTarget;

		// Token: 0x04004235 RID: 16949
		[Header("图标模式相关组件")]
		[SerializeField]
		private CardStyleGeneralScroll cardScroll;

		// Token: 0x04004236 RID: 16950
		[SerializeField]
		private CToggleGroup cardModeToggleGroup;

		// Token: 0x04004237 RID: 16951
		[Header("标题和信息")]
		[SerializeField]
		private TextMeshProUGUI titleLabel;

		// Token: 0x04004238 RID: 16952
		[SerializeField]
		private TextMeshProUGUI titleSelectedItemLabel;

		// Token: 0x04004239 RID: 16953
		[SerializeField]
		private TextMeshProUGUI selectedCountLabel;

		// Token: 0x0400423A RID: 16954
		[SerializeField]
		private TextMeshProUGUI customTextLabel;

		// Token: 0x0400423B RID: 16955
		[SerializeField]
		private GameObject customTextArea;

		// Token: 0x0400423C RID: 16956
		[SerializeField]
		private CButton closeButton;

		// Token: 0x0400423D RID: 16957
		[SerializeField]
		private TooltipInvoker CustomTextTipDisplayer;

		// Token: 0x0400423E RID: 16958
		[Header("已选物品区域")]
		[SerializeField]
		private GameObject selectedScrollArea;

		// Token: 0x0400423F RID: 16959
		[SerializeField]
		private ListStyleGeneralScroll selectedScroll;

		// Token: 0x04004240 RID: 16960
		[SerializeField]
		private GameObject selectedAreaSwitchRoot;

		// Token: 0x04004241 RID: 16961
		[SerializeField]
		private CToggle selectedAreaSwitchToggle;

		// Token: 0x04004242 RID: 16962
		[Header("已选图标模式相关组件")]
		[SerializeField]
		private CardStyleGeneralScroll selectedCardScroll;

		// Token: 0x04004243 RID: 16963
		private SelectItemDisplayData _data;

		// Token: 0x04004244 RID: 16964
		private ESelectItemColumnType _columnTypeFlags;

		// Token: 0x04004245 RID: 16965
		private bool _columnTypeFlagsChanged;

		// Token: 0x04004246 RID: 16966
		private bool _useCustomColumns;

		// Token: 0x04004247 RID: 16967
		private ItemSortAndFilterController _sortAndFilterController;

		// Token: 0x04004248 RID: 16968
		private readonly List<ITradeableContent> _filteredData = new List<ITradeableContent>();

		// Token: 0x04004249 RID: 16969
		private SelectItemConfig _config;

		// Token: 0x0400424A RID: 16970
		private readonly List<SelectedItemData> _selectedItems = new List<SelectedItemData>();

		// Token: 0x0400424B RID: 16971
		private ESelectItemFilterType _currentFilterType = ESelectItemFilterType.All;

		// Token: 0x0400424C RID: 16972
		private bool _needRebuildColumns;

		// Token: 0x0400424D RID: 16973
		private bool _selectedAreaExpanded = true;

		// Token: 0x0400424E RID: 16974
		private bool _selectedScrollInitialized;

		// Token: 0x0400424F RID: 16975
		private bool _isCardMode;

		// Token: 0x04004250 RID: 16976
		private bool _pendingClearSort;

		// Token: 0x04004251 RID: 16977
		private readonly Dictionary<ItemSourceType, HashSet<int>> _availableMainFilterTogglesBySource = new Dictionary<ItemSourceType, HashSet<int>>();

		// Token: 0x04004252 RID: 16978
		private string _searchText = string.Empty;

		// Token: 0x04004253 RID: 16979
		private ItemKey _editingItemKey;

		// Token: 0x04004254 RID: 16980
		private ITradeableContent _editingItemData;

		// Token: 0x04004255 RID: 16981
		private static bool _selectedAreaExpandedPersistent;

		// Token: 0x04004256 RID: 16982
		private float inputCounter = 0.1f;
	}
}
