using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FrameWork.UISystem.Components;
using Game.Components.SortAndFilter;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.Item
{
	// Token: 0x02000EA9 RID: 3753
	public class CardStyleGeneralScroll : MonoBehaviour, ITableHead
	{
		// Token: 0x170013B0 RID: 5040
		// (get) Token: 0x0600ADF0 RID: 44528 RVA: 0x004F40D8 File Offset: 0x004F22D8
		public IReadOnlyList<object> DataList
		{
			get
			{
				return this._dataList;
			}
		}

		// Token: 0x170013B1 RID: 5041
		// (get) Token: 0x0600ADF1 RID: 44529 RVA: 0x004F40E0 File Offset: 0x004F22E0
		// (set) Token: 0x0600ADF2 RID: 44530 RVA: 0x004F40E8 File Offset: 0x004F22E8
		public Func<int, object, bool> RowDisabledProvider { get; set; }

		// Token: 0x170013B2 RID: 5042
		// (get) Token: 0x0600ADF3 RID: 44531 RVA: 0x004F40F1 File Offset: 0x004F22F1
		// (set) Token: 0x0600ADF4 RID: 44532 RVA: 0x004F40F9 File Offset: 0x004F22F9
		public Func<int, object, bool> RowSelectedProvider { get; set; }

		// Token: 0x170013B3 RID: 5043
		// (get) Token: 0x0600ADF5 RID: 44533 RVA: 0x004F4102 File Offset: 0x004F2302
		// (set) Token: 0x0600ADF6 RID: 44534 RVA: 0x004F410A File Offset: 0x004F230A
		public bool RowInteractionEnabled { get; set; }

		// Token: 0x170013B4 RID: 5044
		// (get) Token: 0x0600ADF7 RID: 44535 RVA: 0x004F4113 File Offset: 0x004F2313
		public int SelectedIndex
		{
			get
			{
				return this._selectedIndex;
			}
		}

		// Token: 0x170013B5 RID: 5045
		// (get) Token: 0x0600ADF8 RID: 44536 RVA: 0x004F411B File Offset: 0x004F231B
		public InfinityScroll InfiniteScroll
		{
			get
			{
				return this.infiniteScroll;
			}
		}

		// Token: 0x170013B6 RID: 5046
		// (get) Token: 0x0600ADF9 RID: 44537 RVA: 0x004F4123 File Offset: 0x004F2323
		public RectTransform RectTransform
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x14000087 RID: 135
		// (add) Token: 0x0600ADFA RID: 44538 RVA: 0x004F4130 File Offset: 0x004F2330
		// (remove) Token: 0x0600ADFB RID: 44539 RVA: 0x004F4168 File Offset: 0x004F2368
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int, RowItem> OnRowClicked;

		// Token: 0x0600ADFC RID: 44540 RVA: 0x004F41A0 File Offset: 0x004F23A0
		public void Init<TRow>(IEnumerable<ColumnDefinition> columnDefinitions, bool enableRowInteraction = true, Action<int, GameObject> onItemRender = null, Action<int, RowItem> onClick = null)
		{
			this._columnDefinitions.Clear();
			this._columnDefinitions.AddRange(columnDefinitions);
			if (this._dataList == null)
			{
				this._dataList = new List<object>();
			}
			this._columnSortIds = (from c in this._columnDefinitions
			select c.SortId).ToArray<short>();
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

		// Token: 0x0600ADFD RID: 44541 RVA: 0x004F42B9 File Offset: 0x004F24B9
		public void ClearInfinityScrollCache()
		{
			this.infiniteScroll.ClearCache();
		}

		// Token: 0x0600ADFE RID: 44542 RVA: 0x004F42C8 File Offset: 0x004F24C8
		public void SetRowTemplate(RowItem rowTemplate)
		{
			bool flag = rowTemplate == null;
			if (!flag)
			{
				this.infiniteScroll.srcPrefab = rowTemplate.gameObject;
				this.ClearInfinityScrollCache();
			}
		}

		// Token: 0x0600ADFF RID: 44543 RVA: 0x004F42FC File Offset: 0x004F24FC
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
		}

		// Token: 0x0600AE00 RID: 44544 RVA: 0x004F4394 File Offset: 0x004F2594
		private void OnItemRender(int index, GameObject rowObject)
		{
			RowItemLine rowItem = rowObject.GetComponent<RowItemLine>();
			rowItem.Init(this._columnDefinitions, false);
			object rowData = this._dataList[index];
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

		// Token: 0x0600AE01 RID: 44545 RVA: 0x004F4460 File Offset: 0x004F2660
		private void OnItemHide(GameObject rowObject)
		{
			RowItemLine rowItem = rowObject.GetComponent<RowItemLine>();
			rowItem.OnItemHide();
		}

		// Token: 0x0600AE02 RID: 44546 RVA: 0x004F447C File Offset: 0x004F267C
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

		// Token: 0x0600AE03 RID: 44547 RVA: 0x004F4570 File Offset: 0x004F2770
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

		// Token: 0x0600AE04 RID: 44548 RVA: 0x004F45AC File Offset: 0x004F27AC
		private void OnDestroy()
		{
			bool initialized = this._initialized;
			if (initialized)
			{
				this.infiniteScroll.OnItemRender -= this.OnItemRender;
				this.infiniteScroll.OnItemHide -= this.OnItemHide;
			}
		}

		// Token: 0x0600AE05 RID: 44549 RVA: 0x004F45F8 File Offset: 0x004F27F8
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

		// Token: 0x0600AE06 RID: 44550 RVA: 0x004F464C File Offset: 0x004F284C
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

		// Token: 0x0600AE07 RID: 44551 RVA: 0x004F4679 File Offset: 0x004F2879
		public void UnbindSortController()
		{
			this._sortController = null;
			this.ClearAllSortStates();
		}

		// Token: 0x0600AE08 RID: 44552 RVA: 0x004F468C File Offset: 0x004F288C
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
			}
		}

		// Token: 0x0600AE09 RID: 44553 RVA: 0x004F46EF File Offset: 0x004F28EF
		private void ClearAllSortStates()
		{
		}

		// Token: 0x0600AE0A RID: 44554 RVA: 0x004F46F2 File Offset: 0x004F28F2
		public void SetTableHeadSortEnabled(bool enabled)
		{
		}

		// Token: 0x04008658 RID: 34392
		[SerializeField]
		private InfinityScroll infiniteScroll;

		// Token: 0x04008659 RID: 34393
		private readonly List<ColumnDefinition> _columnDefinitions = new List<ColumnDefinition>();

		// Token: 0x0400865A RID: 34394
		private List<object> _dataList;

		// Token: 0x0400865B RID: 34395
		private bool _initialized;

		// Token: 0x0400865C RID: 34396
		private int _selectedIndex = -1;

		// Token: 0x0400865F RID: 34399
		private ISortAndFilterController _sortController;

		// Token: 0x04008660 RID: 34400
		private short[] _columnSortIds;
	}
}
