using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.SortAndFilter;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.ListStyleGeneralScroll
{
	// Token: 0x02000EA4 RID: 3748
	public class ListStyleGeneralScroll : MonoBehaviour, ITableHead
	{
		// Token: 0x1700139A RID: 5018
		// (get) Token: 0x0600AD8A RID: 44426 RVA: 0x004F259A File Offset: 0x004F079A
		public IReadOnlyList<object> DataList
		{
			get
			{
				return this._dataList;
			}
		}

		// Token: 0x1700139B RID: 5019
		// (get) Token: 0x0600AD8B RID: 44427 RVA: 0x004F25A2 File Offset: 0x004F07A2
		// (set) Token: 0x0600AD8C RID: 44428 RVA: 0x004F25AA File Offset: 0x004F07AA
		public Func<ListStyleGeneralScroll.CellStyleContext, ListStyleGeneralScroll.CellStyle> CellStyleProvider { get; set; }

		// Token: 0x1700139C RID: 5020
		// (get) Token: 0x0600AD8D RID: 44429 RVA: 0x004F25B3 File Offset: 0x004F07B3
		// (set) Token: 0x0600AD8E RID: 44430 RVA: 0x004F25BB File Offset: 0x004F07BB
		public Func<int, object, bool> RowDisabledProvider { get; set; }

		// Token: 0x1700139D RID: 5021
		// (get) Token: 0x0600AD8F RID: 44431 RVA: 0x004F25C4 File Offset: 0x004F07C4
		// (set) Token: 0x0600AD90 RID: 44432 RVA: 0x004F25CC File Offset: 0x004F07CC
		public Func<int, object, bool> RowSelectedProvider { get; set; }

		// Token: 0x1700139E RID: 5022
		// (get) Token: 0x0600AD91 RID: 44433 RVA: 0x004F25D5 File Offset: 0x004F07D5
		// (set) Token: 0x0600AD92 RID: 44434 RVA: 0x004F25DD File Offset: 0x004F07DD
		public bool RowInteractionEnabled { get; set; }

		// Token: 0x1700139F RID: 5023
		// (get) Token: 0x0600AD93 RID: 44435 RVA: 0x004F25E6 File Offset: 0x004F07E6
		public int SelectedIndex
		{
			get
			{
				return this._selectedIndex;
			}
		}

		// Token: 0x170013A0 RID: 5024
		// (get) Token: 0x0600AD94 RID: 44436 RVA: 0x004F25EE File Offset: 0x004F07EE
		public InfinityScroll InfiniteScroll
		{
			get
			{
				return this.infiniteScroll;
			}
		}

		// Token: 0x170013A1 RID: 5025
		// (get) Token: 0x0600AD95 RID: 44437 RVA: 0x004F25F6 File Offset: 0x004F07F6
		public RectTransform RectTransform
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x14000086 RID: 134
		// (add) Token: 0x0600AD96 RID: 44438 RVA: 0x004F2604 File Offset: 0x004F0804
		// (remove) Token: 0x0600AD97 RID: 44439 RVA: 0x004F263C File Offset: 0x004F083C
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int, RowItem> OnRowClicked;

		// Token: 0x0600AD98 RID: 44440 RVA: 0x004F2671 File Offset: 0x004F0871
		private void Awake()
		{
			this.tableHeadCellTemplate.gameObject.SetActive(false);
		}

		// Token: 0x0600AD99 RID: 44441 RVA: 0x004F2686 File Offset: 0x004F0886
		private void OnDisable()
		{
			this.infiniteScroll.Scroll.Content.anchoredPosition = Vector2.zero;
		}

		// Token: 0x0600AD9A RID: 44442 RVA: 0x004F26A4 File Offset: 0x004F08A4
		public void Init<TRow>(IEnumerable<ColumnDefinition> columnDefinitions, bool enableRowInteraction = true, Action<int, GameObject> onItemRender = null, Action<int, RowItem> onClick = null)
		{
			this._columnDefinitions.Clear();
			this._columnDefinitions.AddRange(columnDefinitions);
			this._tableHeadCells.Clear();
			if (this._dataList == null)
			{
				this._dataList = new List<object>();
			}
			this._columnSortIds = (from c in this._columnDefinitions
			select c.SortId).ToArray<short>();
			CommonUtils.PrepareEnoughChildren(this.tableHeadRoot.transform, this.tableHeadCellTemplate.gameObject, this._columnDefinitions.Count, null);
			for (int i = 0; i < this._columnDefinitions.Count; i++)
			{
				TableHeadCell headCell = this.tableHeadRoot.transform.GetChild(i).GetComponent<TableHeadCell>();
				headCell.Init(this._columnDefinitions[i], i < this._columnDefinitions.Count - 1, i, new Action<int, short>(this.OnTableHeadCellClicked));
				this._tableHeadCells.Add(headCell);
			}
			bool flag = this._sortController != null;
			if (flag)
			{
				ISortAndFilterController sortController = this._sortController;
				sortController.UnregisterTableHead(this);
				sortController.RegisterTableHead(this, this._columnSortIds);
				this.SyncSortStateFromController();
			}
			bool flag2 = !this._initialized;
			if (flag2)
			{
				this.infiniteScroll.OnItemHide += this.OnItemHide;
				this.infiniteScroll.OnItemRender += this.OnItemRender;
				bool flag3 = onItemRender != null;
				if (flag3)
				{
					this.infiniteScroll.OnItemRender += onItemRender;
				}
				bool flag4 = onClick != null;
				if (flag4)
				{
					this.OnRowClicked += onClick;
				}
				this._initialized = true;
			}
			this.RowInteractionEnabled = enableRowInteraction;
		}

		// Token: 0x0600AD9B RID: 44443 RVA: 0x004F2872 File Offset: 0x004F0A72
		public void OnPointerEnter()
		{
			CScrollRect scroll = this.infiniteScroll.Scroll;
			if (scroll != null)
			{
				scroll.OnPointerEnter(null);
			}
		}

		// Token: 0x0600AD9C RID: 44444 RVA: 0x004F288D File Offset: 0x004F0A8D
		public void ClearInfinityScrollCache()
		{
			this.infiniteScroll.ClearCache();
		}

		// Token: 0x0600AD9D RID: 44445 RVA: 0x004F289C File Offset: 0x004F0A9C
		public void SetRowTemplate(RowItem rowTemplate)
		{
			bool flag = rowTemplate == null;
			if (!flag)
			{
				this.infiniteScroll.srcPrefab = rowTemplate.gameObject;
				this.ClearInfinityScrollCache();
			}
		}

		// Token: 0x0600AD9E RID: 44446 RVA: 0x004F28D0 File Offset: 0x004F0AD0
		public void SetData<TRow>(IEnumerable<TRow> data, int selectedIndex = -1)
		{
			if (this._dataList == null)
			{
				this._dataList = new List<object>();
			}
			this._dataList.Clear();
			foreach (TRow item in data)
			{
				this._dataList.Add(item);
			}
			this._selectedIndex = selectedIndex;
			this.infiniteScroll.SetDataCount(this._dataList.Count);
			bool flag = this.autoShowEmptyState && this.emptyState != null;
			if (flag)
			{
				this.emptyState.SetActive(this._dataList.Count == 0);
			}
		}

		// Token: 0x0600AD9F RID: 44447 RVA: 0x004F299C File Offset: 0x004F0B9C
		private void OnItemRender(int index, GameObject rowObject)
		{
			RowItem rowItem = rowObject.GetComponent<RowItem>();
			rowItem.Init(this._columnDefinitions, true);
			object rowData = this._dataList[index];
			rowItem.SetData(rowData, this.ShowBottomLine || index < this._dataList.Count - 1, null);
			bool rowInteractionEnabled = this.RowInteractionEnabled;
			if (rowInteractionEnabled)
			{
				Func<int, object, bool> rowDisabledProvider = this.RowDisabledProvider;
				bool isDisabled = rowDisabledProvider != null && rowDisabledProvider(index, rowData);
				rowItem.SetRowInteraction(!isDisabled, index, isDisabled ? null : new Action<int, RowItem>(this.OnRowButtonClicked));
				rowItem.SetInteractable(!isDisabled, true);
				rowItem.SetDisabled(isDisabled);
				bool isSelected = (this.RowSelectedProvider != null) ? this.RowSelectedProvider(index, rowData) : (index == this._selectedIndex);
				rowItem.SetSelected(!isDisabled && isSelected);
			}
			else
			{
				rowItem.SetRowInteraction(false, -1, null);
				rowItem.SetSelected(false);
			}
		}

		// Token: 0x0600ADA0 RID: 44448 RVA: 0x004F2A8C File Offset: 0x004F0C8C
		private void OnItemHide(GameObject rowObject)
		{
			RowItem rowItem = rowObject.GetComponent<RowItem>();
			rowItem.OnItemHide();
		}

		// Token: 0x0600ADA1 RID: 44449 RVA: 0x004F2AA8 File Offset: 0x004F0CA8
		public void SetSelectedRow(int index)
		{
			bool flag = !this.RowInteractionEnabled;
			if (!flag)
			{
				bool flag2 = index < -1 || index >= this._dataList.Count;
				if (!flag2)
				{
					int oldIndex = this._selectedIndex;
					this._selectedIndex = index;
					bool flag3 = oldIndex >= 0 && oldIndex < this._dataList.Count;
					if (flag3)
					{
						GameObject oldRow = this.infiniteScroll.GetActiveCell(oldIndex);
						bool flag4 = oldRow;
						if (flag4)
						{
							RowItem oldRowItem = oldRow.GetComponent<RowItem>();
							if (oldRowItem != null)
							{
								oldRowItem.SetSelected(false);
							}
						}
					}
					bool flag5 = index >= 0 && index < this._dataList.Count;
					if (flag5)
					{
						GameObject newRow = this.infiniteScroll.GetActiveCell(index);
						bool flag6 = newRow;
						if (flag6)
						{
							RowItem newRowItem = newRow.GetComponent<RowItem>();
							if (newRowItem != null)
							{
								newRowItem.SetSelected(true);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600ADA2 RID: 44450 RVA: 0x004F2B9C File Offset: 0x004F0D9C
		private void OnRowButtonClicked(int index, RowItem rowItem)
		{
			bool flag = this.RowSelectedProvider == null;
			if (flag)
			{
				this.SetSelectedRow(index);
			}
			Action<int, RowItem> onRowClicked = this.OnRowClicked;
			if (onRowClicked != null)
			{
				onRowClicked(index, rowItem);
			}
		}

		// Token: 0x0600ADA3 RID: 44451 RVA: 0x004F2BD8 File Offset: 0x004F0DD8
		private void OnDestroy()
		{
			bool initialized = this._initialized;
			if (initialized)
			{
				this.infiniteScroll.OnItemRender -= this.OnItemRender;
				this.infiniteScroll.OnItemHide -= this.OnItemHide;
			}
			this.UnbindSortController();
		}

		// Token: 0x0600ADA4 RID: 44452 RVA: 0x004F2C2C File Offset: 0x004F0E2C
		public void SetSortController(ISortAndFilterController sortController)
		{
			bool flag = this._sortController != null;
			if (flag)
			{
				this._sortController.UnregisterTableHead(this);
			}
			this._sortController = sortController;
			bool flag2 = this._sortController != null;
			if (flag2)
			{
				this._sortController.RegisterTableHead(this, this._columnSortIds);
			}
		}

		// Token: 0x0600ADA5 RID: 44453 RVA: 0x004F2C80 File Offset: 0x004F0E80
		public void BindSortController(ISortAndFilterController sortController, short[] columnSortIds)
		{
			this._sortController = sortController;
			bool flag = columnSortIds != null;
			if (flag)
			{
				this._columnSortIds = columnSortIds;
			}
			this.SyncSortStateFromController();
		}

		// Token: 0x0600ADA6 RID: 44454 RVA: 0x004F2CAD File Offset: 0x004F0EAD
		public void UnbindSortController()
		{
			this._sortController = null;
			this.ClearAllSortStates();
		}

		// Token: 0x0600ADA7 RID: 44455 RVA: 0x004F2CC0 File Offset: 0x004F0EC0
		public void SyncSortStateFromController()
		{
			bool flag = this._sortController == null;
			if (flag)
			{
				this.ClearAllSortStates();
			}
			else
			{
				SortStateData sortData = this._sortController.SortAndFilterState.SortData;
				bool flag2 = ((sortData != null) ? sortData.ItemStates : null) == null || sortData.ItemStates.Count == 0;
				if (flag2)
				{
					this.ClearAllSortStates();
				}
				else
				{
					foreach (TableHeadCell headCell in this._tableHeadCells)
					{
						short sortId = headCell.SortId;
						bool flag3 = sortId < 0;
						if (flag3)
						{
							headCell.ClearSortState();
						}
						else
						{
							int sortOrder = -1;
							ESortDirection direction = ESortDirection.Descending;
							for (int i = 0; i < sortData.ItemStates.Count; i++)
							{
								bool flag4 = sortData.ItemStates[i].SortId == sortId;
								if (flag4)
								{
									sortOrder = i;
									direction = sortData.ItemStates[i].SortDirection;
									break;
								}
							}
							headCell.SetSortState(sortOrder, direction);
						}
					}
				}
			}
		}

		// Token: 0x0600ADA8 RID: 44456 RVA: 0x004F2E00 File Offset: 0x004F1000
		private void ClearAllSortStates()
		{
			foreach (TableHeadCell headCell in this._tableHeadCells)
			{
				headCell.ClearSortState();
			}
		}

		// Token: 0x0600ADA9 RID: 44457 RVA: 0x004F2E58 File Offset: 0x004F1058
		private void OnTableHeadCellClicked(int columnIndex, short sortId)
		{
			bool flag = this._sortController == null || sortId < 0 || !this._sortEnabled;
			if (!flag)
			{
				SortStateData currentState = this._sortController.SortAndFilterState.SortData;
				List<SortItemState> newItemStates = new List<SortItemState>();
				int existingIndex = -1;
				ESortDirection existingDirection = ESortDirection.Descending;
				bool flag2 = ((currentState != null) ? currentState.ItemStates : null) != null;
				if (flag2)
				{
					for (int i = 0; i < currentState.ItemStates.Count; i++)
					{
						bool flag3 = currentState.ItemStates[i].SortId == sortId;
						if (flag3)
						{
							existingIndex = i;
							existingDirection = currentState.ItemStates[i].SortDirection;
							break;
						}
					}
				}
				bool flag4 = existingIndex >= 0;
				if (flag4)
				{
					bool flag5 = ((currentState != null) ? currentState.ItemStates : null) != null;
					if (flag5)
					{
						foreach (SortItemState item in currentState.ItemStates)
						{
							bool flag6 = item.SortId != sortId;
							if (flag6)
							{
								newItemStates.Add(item);
							}
						}
					}
					bool flag7 = existingDirection == ESortDirection.Descending;
					if (flag7)
					{
						newItemStates.Insert(existingIndex, new SortItemState
						{
							SortId = sortId,
							SortDirection = ESortDirection.Ascending
						});
					}
				}
				else
				{
					bool flag8 = ((currentState != null) ? currentState.ItemStates : null) != null;
					if (flag8)
					{
						newItemStates.AddRange(currentState.ItemStates);
					}
					newItemStates.Add(new SortItemState
					{
						SortId = sortId,
						SortDirection = ESortDirection.Descending
					});
				}
				this._sortController.SetSortState(new SortStateData
				{
					ItemStates = newItemStates
				});
			}
		}

		// Token: 0x0600ADAA RID: 44458 RVA: 0x004F302C File Offset: 0x004F122C
		public void SetTableHeadSortEnabled(bool enabled)
		{
			this._sortEnabled = enabled;
			foreach (TableHeadCell headCell in this._tableHeadCells)
			{
				headCell.SetInteractable(enabled);
			}
		}

		// Token: 0x0400860F RID: 34319
		[SerializeField]
		private InfinityScroll infiniteScroll;

		// Token: 0x04008610 RID: 34320
		[SerializeField]
		private HorizontalLayoutGroup tableHeadRoot;

		// Token: 0x04008611 RID: 34321
		[SerializeField]
		private TableHeadCell tableHeadCellTemplate;

		// Token: 0x04008612 RID: 34322
		public bool ShowBottomLine = false;

		// Token: 0x04008613 RID: 34323
		[Tooltip("勾选的话空列表时会自动显示emptyState")]
		[SerializeField]
		private bool autoShowEmptyState;

		// Token: 0x04008614 RID: 34324
		[SerializeField]
		private GameObject emptyState;

		// Token: 0x04008615 RID: 34325
		private readonly List<ColumnDefinition> _columnDefinitions = new List<ColumnDefinition>();

		// Token: 0x04008616 RID: 34326
		private readonly List<TableHeadCell> _tableHeadCells = new List<TableHeadCell>();

		// Token: 0x04008617 RID: 34327
		private List<object> _dataList;

		// Token: 0x04008618 RID: 34328
		private bool _initialized;

		// Token: 0x04008619 RID: 34329
		private int _selectedIndex = -1;

		// Token: 0x0400861D RID: 34333
		private ISortAndFilterController _sortController;

		// Token: 0x0400861E RID: 34334
		private short[] _columnSortIds;

		// Token: 0x0400861F RID: 34335
		private bool _sortEnabled = true;

		// Token: 0x02002514 RID: 9492
		public readonly struct CellStyleContext
		{
			// Token: 0x06010AA5 RID: 68261 RVA: 0x00669D59 File Offset: 0x00667F59
			public CellStyleContext(object rowData, int rowIndex, int columnIndex, ColumnDefinition columnDefinition, object cellData)
			{
				this.RowData = rowData;
				this.RowIndex = rowIndex;
				this.ColumnIndex = columnIndex;
				this.ColumnDefinition = columnDefinition;
				this.CellData = cellData;
			}

			// Token: 0x0400E697 RID: 59031
			public readonly object RowData;

			// Token: 0x0400E698 RID: 59032
			public readonly int RowIndex;

			// Token: 0x0400E699 RID: 59033
			public readonly int ColumnIndex;

			// Token: 0x0400E69A RID: 59034
			public readonly ColumnDefinition ColumnDefinition;

			// Token: 0x0400E69B RID: 59035
			public readonly object CellData;
		}

		// Token: 0x02002515 RID: 9493
		public readonly struct CellStyle
		{
			// Token: 0x06010AA6 RID: 68262 RVA: 0x00669D81 File Offset: 0x00667F81
			public CellStyle(bool showSpecialBg)
			{
				this.ShowSpecialBg = showSpecialBg;
			}

			// Token: 0x17001B87 RID: 7047
			// (get) Token: 0x06010AA7 RID: 68263 RVA: 0x00669D8B File Offset: 0x00667F8B
			public static ListStyleGeneralScroll.CellStyle Default
			{
				get
				{
					return new ListStyleGeneralScroll.CellStyle(false);
				}
			}

			// Token: 0x0400E69C RID: 59036
			public readonly bool ShowSpecialBg;
		}
	}
}
