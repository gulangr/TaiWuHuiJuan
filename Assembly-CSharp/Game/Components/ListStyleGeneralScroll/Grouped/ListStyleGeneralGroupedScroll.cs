using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FrameWork.UISystem.Components;
using Game.Components.SortAndFilter;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.ListStyleGeneralScroll.Grouped
{
	// Token: 0x02000EAE RID: 3758
	public class ListStyleGeneralGroupedScroll : MonoBehaviour, ITableHead
	{
		// Token: 0x170013CA RID: 5066
		// (get) Token: 0x0600AE9B RID: 44699 RVA: 0x004F910D File Offset: 0x004F730D
		// (set) Token: 0x0600AE9C RID: 44700 RVA: 0x004F9115 File Offset: 0x004F7315
		public Func<ListStyleGeneralScroll.CellStyleContext, ListStyleGeneralScroll.CellStyle> CellStyleProvider { get; set; }

		// Token: 0x170013CB RID: 5067
		// (get) Token: 0x0600AE9D RID: 44701 RVA: 0x004F911E File Offset: 0x004F731E
		// (set) Token: 0x0600AE9E RID: 44702 RVA: 0x004F9126 File Offset: 0x004F7326
		public bool RowInteractionEnabled { get; set; }

		// Token: 0x170013CC RID: 5068
		// (get) Token: 0x0600AE9F RID: 44703 RVA: 0x004F912F File Offset: 0x004F732F
		public int SelectedIndex
		{
			get
			{
				return this._selectedIndex;
			}
		}

		// Token: 0x170013CD RID: 5069
		// (get) Token: 0x0600AEA0 RID: 44704 RVA: 0x004F9137 File Offset: 0x004F7337
		public IReadOnlyList<object> DataList
		{
			get
			{
				return this._contentDataList;
			}
		}

		// Token: 0x170013CE RID: 5070
		// (get) Token: 0x0600AEA1 RID: 44705 RVA: 0x004F913F File Offset: 0x004F733F
		public InfinityScroll InfiniteScroll
		{
			get
			{
				return this.infiniteScroll;
			}
		}

		// Token: 0x170013CF RID: 5071
		// (get) Token: 0x0600AEA2 RID: 44706 RVA: 0x004F9147 File Offset: 0x004F7347
		public RectTransform RectTransform
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x170013D0 RID: 5072
		// (get) Token: 0x0600AEA3 RID: 44707 RVA: 0x004F9154 File Offset: 0x004F7354
		// (set) Token: 0x0600AEA4 RID: 44708 RVA: 0x004F915C File Offset: 0x004F735C
		public bool ShowGroupTitles
		{
			get
			{
				return this._showGroupTitles;
			}
			set
			{
				bool flag = this._showGroupTitles == value;
				if (!flag)
				{
					this._showGroupTitles = value;
					this.RebuildFlatRows();
					this.infiniteScroll.SetDataCount(this._flatRows.Count);
				}
			}
		}

		// Token: 0x14000088 RID: 136
		// (add) Token: 0x0600AEA5 RID: 44709 RVA: 0x004F91A0 File Offset: 0x004F73A0
		// (remove) Token: 0x0600AEA6 RID: 44710 RVA: 0x004F91D8 File Offset: 0x004F73D8
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int, RowItem> OnRowClicked;

		// Token: 0x0600AEA7 RID: 44711 RVA: 0x004F920D File Offset: 0x004F740D
		private void Awake()
		{
			this.tableHeadCellTemplate.gameObject.SetActive(false);
		}

		// Token: 0x0600AEA8 RID: 44712 RVA: 0x004F9224 File Offset: 0x004F7424
		public void Init<TRow>(IEnumerable<ColumnDefinition> columnDefinitions, bool enableRowInteraction = true, Action<int, GameObject> onItemRender = null, Action<int, RowItem> onClick = null)
		{
			this._columnDefinitions.Clear();
			this._columnDefinitions.AddRange(columnDefinitions);
			this._tableHeadCells.Clear();
			this._externalOnItemRender = onItemRender;
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
				this.infiniteScroll.OnItemRender += this.OnItemRender;
				bool flag3 = onClick != null;
				if (flag3)
				{
					this.OnRowClicked += onClick;
				}
				this._initialized = true;
			}
			this.RowInteractionEnabled = enableRowInteraction;
		}

		// Token: 0x0600AEA9 RID: 44713 RVA: 0x004F93B7 File Offset: 0x004F75B7
		public void ClearInfinityScrollCache()
		{
			this.infiniteScroll.ClearCache();
		}

		// Token: 0x0600AEAA RID: 44714 RVA: 0x004F93C8 File Offset: 0x004F75C8
		public void SetData<TRow>(IEnumerable<ListStyleGeneralGroupedScroll.Group<TRow>> groups, int selectedIndex = -1)
		{
			this._groups.Clear();
			bool flag = groups != null;
			if (flag)
			{
				int groupIndex = 0;
				foreach (ListStyleGeneralGroupedScroll.Group<TRow> g in groups)
				{
					List<ListStyleGeneralGroupedScroll.Group> groups2 = this._groups;
					int groupIndex2 = groupIndex;
					string title = g.Title;
					IReadOnlyList<TRow> items = g.Items;
					groups2.Add(new ListStyleGeneralGroupedScroll.Group(groupIndex2, title, ((items != null) ? items.Cast<object>().ToList<object>() : null) ?? new List<object>(), g.Available));
					groupIndex++;
				}
			}
			this._selectedIndex = selectedIndex;
			this.RebuildFlatRows();
			this.infiniteScroll.SetDataCount(this._flatRows.Count);
		}

		// Token: 0x0600AEAB RID: 44715 RVA: 0x004F9490 File Offset: 0x004F7690
		public void SetSelectedRow(int index)
		{
			bool flag = !this.RowInteractionEnabled;
			if (!flag)
			{
				int contentRowCount = this._contentDataList.Count;
				bool flag2 = index < -1 || index >= contentRowCount;
				if (!flag2)
				{
					int oldIndex = this._selectedIndex;
					this._selectedIndex = index;
					bool flag3 = oldIndex >= 0;
					if (flag3)
					{
						int oldFlatIndex = this.FindFlatIndexByContentIndex(oldIndex);
						bool flag4 = oldFlatIndex >= 0;
						if (flag4)
						{
							GameObject oldRowObj = this.infiniteScroll.GetActiveCell(oldFlatIndex);
							bool flag5 = oldRowObj;
							if (flag5)
							{
								GroupedRowWrapper wrapper = oldRowObj.GetComponent<GroupedRowWrapper>();
								if (wrapper != null)
								{
									RowItem contentRowItem = wrapper.ContentRowItem;
									if (contentRowItem != null)
									{
										contentRowItem.SetSelected(false);
									}
								}
							}
						}
					}
					bool flag6 = index >= 0;
					if (flag6)
					{
						int newFlatIndex = this.FindFlatIndexByContentIndex(index);
						bool flag7 = newFlatIndex >= 0;
						if (flag7)
						{
							GameObject newRowObj = this.infiniteScroll.GetActiveCell(newFlatIndex);
							bool flag8 = newRowObj;
							if (flag8)
							{
								GroupedRowWrapper wrapper2 = newRowObj.GetComponent<GroupedRowWrapper>();
								if (wrapper2 != null)
								{
									RowItem contentRowItem2 = wrapper2.ContentRowItem;
									if (contentRowItem2 != null)
									{
										contentRowItem2.SetSelected(true);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600AEAC RID: 44716 RVA: 0x004F95B4 File Offset: 0x004F77B4
		private void OnItemRender(int flatIndex, GameObject rowObject)
		{
			bool flag = flatIndex < 0 || flatIndex >= this._flatRows.Count;
			if (!flag)
			{
				GroupedRowWrapper wrapper = rowObject.GetComponent<GroupedRowWrapper>();
				bool flag2 = wrapper == null;
				if (!flag2)
				{
					ListStyleGeneralGroupedScroll.FlatRow flatRow = this._flatRows[flatIndex];
					bool isTitle = flatRow.IsTitle;
					if (isTitle)
					{
						wrapper.ShowTitle(flatRow.Title);
						wrapper.SetBottomLine(flatIndex < this._flatRows.Count - 1);
						bool flag3 = this.groupTitleBackEnabled;
						if (flag3)
						{
							Sprite sprite = flatRow.Available ? this.spriteBackAvailable : this.spriteBackNotAvailable;
							bool flag4 = sprite;
							if (flag4)
							{
								wrapper.SetTitleBack(sprite);
							}
						}
					}
					else
					{
						wrapper.ShowContent();
						bool flag5 = this.groupTitleBackEnabled;
						if (flag5)
						{
							wrapper.SetTitleBack(null);
						}
						RowItem rowItem = wrapper.ContentRowItem;
						rowItem.Init(this._columnDefinitions, true);
						object rowData = flatRow.RowData;
						rowItem.SetData(rowData, !wrapper.HasBottomLine && flatIndex < this._flatRows.Count - 1, delegate(int columnIndex, ColumnDefinition columnDefinition, object cellData)
						{
							bool flag6 = this.CellStyleProvider == null;
							ListStyleGeneralScroll.CellStyle result;
							if (flag6)
							{
								result = ListStyleGeneralScroll.CellStyle.Default;
							}
							else
							{
								result = this.CellStyleProvider(new ListStyleGeneralScroll.CellStyleContext(rowData, flatRow.ContentIndex, columnIndex, columnDefinition, cellData));
							}
							return result;
						});
						bool hasBottomLine = wrapper.HasBottomLine;
						if (hasBottomLine)
						{
							wrapper.SetBottomLine(flatIndex < this._flatRows.Count - 1);
						}
						bool rowInteractionEnabled = this.RowInteractionEnabled;
						if (rowInteractionEnabled)
						{
							rowItem.SetRowInteraction(true, flatRow.ContentIndex, new Action<int, RowItem>(this.OnRowButtonClicked));
							rowItem.SetSelected(flatRow.ContentIndex == this._selectedIndex);
						}
						else
						{
							rowItem.SetRowInteraction(false, -1, null);
							rowItem.SetSelected(false);
						}
						Action<int, GameObject> externalOnItemRender = this._externalOnItemRender;
						if (externalOnItemRender != null)
						{
							externalOnItemRender(flatRow.ContentIndex, rowItem.gameObject);
						}
					}
				}
			}
		}

		// Token: 0x0600AEAD RID: 44717 RVA: 0x004F97B4 File Offset: 0x004F79B4
		private void OnRowButtonClicked(int contentIndex, RowItem rowItem)
		{
			this.SetSelectedRow(contentIndex);
			Action<int, RowItem> onRowClicked = this.OnRowClicked;
			if (onRowClicked != null)
			{
				onRowClicked(contentIndex, rowItem);
			}
		}

		// Token: 0x0600AEAE RID: 44718 RVA: 0x004F97D4 File Offset: 0x004F79D4
		private void OnDestroy()
		{
			bool initialized = this._initialized;
			if (initialized)
			{
				this.infiniteScroll.OnItemRender -= this.OnItemRender;
			}
			this.UnbindSortController();
		}

		// Token: 0x0600AEAF RID: 44719 RVA: 0x004F980C File Offset: 0x004F7A0C
		private void RebuildFlatRows()
		{
			this._flatRows.Clear();
			this._contentDataList.Clear();
			int contentIndex = 0;
			foreach (ListStyleGeneralGroupedScroll.Group group in this._groups)
			{
				bool showGroupTitles = this._showGroupTitles;
				if (showGroupTitles)
				{
					this._flatRows.Add(new ListStyleGeneralGroupedScroll.FlatRow
					{
						IsTitle = true,
						Title = group.Title,
						GroupIndex = group.GroupIndex,
						Available = group.Available
					});
				}
				foreach (object rowData in group.Items)
				{
					this._flatRows.Add(new ListStyleGeneralGroupedScroll.FlatRow
					{
						IsTitle = false,
						Title = group.Title,
						GroupIndex = group.GroupIndex,
						RowData = rowData,
						ContentIndex = contentIndex,
						Available = group.Available
					});
					this._contentDataList.Add(rowData);
					contentIndex++;
				}
			}
			bool flag = this._selectedIndex >= this._contentDataList.Count;
			if (flag)
			{
				this._selectedIndex = -1;
			}
		}

		// Token: 0x0600AEB0 RID: 44720 RVA: 0x004F99B4 File Offset: 0x004F7BB4
		public int FindFlatIndexByContentIndex(int contentIndex)
		{
			for (int i = 0; i < this._flatRows.Count; i++)
			{
				bool flag = !this._flatRows[i].IsTitle && this._flatRows[i].ContentIndex == contentIndex;
				if (flag)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x0600AEB1 RID: 44721 RVA: 0x004F9A18 File Offset: 0x004F7C18
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

		// Token: 0x0600AEB2 RID: 44722 RVA: 0x004F9A68 File Offset: 0x004F7C68
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

		// Token: 0x0600AEB3 RID: 44723 RVA: 0x004F9A93 File Offset: 0x004F7C93
		public void UnbindSortController()
		{
			this._sortController = null;
			this.ClearAllSortStates();
		}

		// Token: 0x0600AEB4 RID: 44724 RVA: 0x004F9AA4 File Offset: 0x004F7CA4
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

		// Token: 0x0600AEB5 RID: 44725 RVA: 0x004F9BE4 File Offset: 0x004F7DE4
		private void ClearAllSortStates()
		{
			foreach (TableHeadCell headCell in this._tableHeadCells)
			{
				headCell.ClearSortState();
			}
		}

		// Token: 0x0600AEB6 RID: 44726 RVA: 0x004F9C3C File Offset: 0x004F7E3C
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

		// Token: 0x0600AEB7 RID: 44727 RVA: 0x004F9E0C File Offset: 0x004F800C
		public void SetTableHeadSortEnabled(bool enabled)
		{
			this._sortEnabled = enabled;
			foreach (TableHeadCell headCell in this._tableHeadCells)
			{
				headCell.SetInteractable(enabled);
			}
		}

		// Token: 0x04008701 RID: 34561
		[SerializeField]
		private InfinityScroll infiniteScroll;

		// Token: 0x04008702 RID: 34562
		[SerializeField]
		private HorizontalLayoutGroup tableHeadRoot;

		// Token: 0x04008703 RID: 34563
		[SerializeField]
		private TableHeadCell tableHeadCellTemplate;

		// Token: 0x04008704 RID: 34564
		[SerializeField]
		private Sprite spriteBackAvailable;

		// Token: 0x04008705 RID: 34565
		[SerializeField]
		private Sprite spriteBackNotAvailable;

		// Token: 0x04008706 RID: 34566
		[SerializeField]
		private bool groupTitleBackEnabled;

		// Token: 0x04008707 RID: 34567
		private readonly List<ColumnDefinition> _columnDefinitions = new List<ColumnDefinition>();

		// Token: 0x04008708 RID: 34568
		private readonly List<TableHeadCell> _tableHeadCells = new List<TableHeadCell>();

		// Token: 0x04008709 RID: 34569
		private bool _initialized;

		// Token: 0x0400870A RID: 34570
		private int _selectedIndex = -1;

		// Token: 0x0400870B RID: 34571
		private readonly List<ListStyleGeneralGroupedScroll.Group> _groups = new List<ListStyleGeneralGroupedScroll.Group>();

		// Token: 0x0400870C RID: 34572
		private readonly List<ListStyleGeneralGroupedScroll.FlatRow> _flatRows = new List<ListStyleGeneralGroupedScroll.FlatRow>();

		// Token: 0x0400870D RID: 34573
		private readonly List<object> _contentDataList = new List<object>();

		// Token: 0x0400870E RID: 34574
		private bool _showGroupTitles = true;

		// Token: 0x0400870F RID: 34575
		private Action<int, GameObject> _externalOnItemRender;

		// Token: 0x04008711 RID: 34577
		private ISortAndFilterController _sortController;

		// Token: 0x04008712 RID: 34578
		private short[] _columnSortIds;

		// Token: 0x04008713 RID: 34579
		private bool _sortEnabled = true;

		// Token: 0x0200252A RID: 9514
		public readonly struct Group<TRow>
		{
			// Token: 0x06010B1C RID: 68380 RVA: 0x0066B64C File Offset: 0x0066984C
			public Group(string title, IReadOnlyList<TRow> items, bool available)
			{
				this.Title = title;
				this.Items = items;
				this.Available = available;
			}

			// Token: 0x0400E736 RID: 59190
			public readonly string Title;

			// Token: 0x0400E737 RID: 59191
			public readonly IReadOnlyList<TRow> Items;

			// Token: 0x0400E738 RID: 59192
			public readonly bool Available;
		}

		// Token: 0x0200252B RID: 9515
		private readonly struct Group
		{
			// Token: 0x06010B1D RID: 68381 RVA: 0x0066B664 File Offset: 0x00669864
			public Group(int groupIndex, string title, List<object> items, bool available)
			{
				this.GroupIndex = groupIndex;
				this.Title = title;
				this.Items = (items ?? new List<object>());
				this.Available = available;
			}

			// Token: 0x0400E739 RID: 59193
			public readonly int GroupIndex;

			// Token: 0x0400E73A RID: 59194
			public readonly string Title;

			// Token: 0x0400E73B RID: 59195
			public readonly List<object> Items;

			// Token: 0x0400E73C RID: 59196
			public readonly bool Available;
		}

		// Token: 0x0200252C RID: 9516
		private struct FlatRow
		{
			// Token: 0x0400E73D RID: 59197
			public bool IsTitle;

			// Token: 0x0400E73E RID: 59198
			public int GroupIndex;

			// Token: 0x0400E73F RID: 59199
			public string Title;

			// Token: 0x0400E740 RID: 59200
			public object RowData;

			// Token: 0x0400E741 RID: 59201
			public int ContentIndex;

			// Token: 0x0400E742 RID: 59202
			public bool Available;
		}
	}
}
