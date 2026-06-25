using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x0200042B RID: 1067
	public abstract class CommonSortAndFilterController<TData> : ICommonSortAndFilterController
	{
		// Token: 0x1700066C RID: 1644
		// (get) Token: 0x06003F3E RID: 16190 RVA: 0x001F8ECB File Offset: 0x001F70CB
		public SortAndFilterState SortAndFilterState
		{
			get
			{
				return this._sortAndFilterState;
			}
		}

		// Token: 0x1700066D RID: 1645
		// (get) Token: 0x06003F3F RID: 16191 RVA: 0x001F8ED3 File Offset: 0x001F70D3
		public bool IsEmpty
		{
			get
			{
				List<TData> originalDataList = this._originalDataList;
				return originalDataList == null || originalDataList.Count <= 0;
			}
		}

		// Token: 0x1700066E RID: 1646
		// (get) Token: 0x06003F40 RID: 16192 RVA: 0x001F8EED File Offset: 0x001F70ED
		public List<FilterLineBase<TData>> FilterLines
		{
			get
			{
				return this._filterLines;
			}
		}

		// Token: 0x1700066F RID: 1647
		// (get) Token: 0x06003F41 RID: 16193
		protected abstract string FilterCustomOrderKey { get; }

		// Token: 0x06003F42 RID: 16194 RVA: 0x001F8EF8 File Offset: 0x001F70F8
		protected CommonSortAndFilterController(CommonSortAndFilter sortAndFilter)
		{
			this._sortAndFilter = sortAndFilter;
			GameSort gameSortInstance = SingletonObject.getInstance<GameSort>();
			this._filterCustomOrderStore = new FilterCustomOrderStore(gameSortInstance);
		}

		// Token: 0x17000670 RID: 1648
		// (get) Token: 0x06003F43 RID: 16195 RVA: 0x001F8F52 File Offset: 0x001F7152
		public List<TData> OutputDataList
		{
			get
			{
				return this._sortedFilteredDataList;
			}
		}

		// Token: 0x17000671 RID: 1649
		// (get) Token: 0x06003F44 RID: 16196 RVA: 0x001F8F5A File Offset: 0x001F715A
		public IReadOnlyList<TData> ReadOnlyOriginalDataList
		{
			get
			{
				return this._originalDataList;
			}
		}

		// Token: 0x17000672 RID: 1650
		// (get) Token: 0x06003F45 RID: 16197 RVA: 0x001F8F62 File Offset: 0x001F7162
		public SortAndFilterConfig OriginalConfig
		{
			get
			{
				return this._sortAndFilter.Config;
			}
		}

		// Token: 0x06003F46 RID: 16198 RVA: 0x001F8F70 File Offset: 0x001F7170
		public void Init(Action onDataListChanged, string sortSaveKey)
		{
			bool isInit = this._isInit;
			if (!isInit)
			{
				this.RegisterFilterLines();
				SortAndFilterConfig config = this.GenerateSortAndFilterConfig();
				this._sortAndFilter.Setup(config);
				this._onDataListChanged = onDataListChanged;
				this.SortController.Init(this._sortAndFilter.SortDropdown, sortSaveKey);
				FilterCustomOrderData customOrderData = this._filterCustomOrderStore.LoadFilterCustomOrderData(this.FilterCustomOrderKey);
				this._sortAndFilter.ApplyCustomOrder(customOrderData.LineCustomOrder);
				this._customOrderData = customOrderData;
				this._isInit = true;
			}
		}

		// Token: 0x06003F47 RID: 16199 RVA: 0x001F8FF6 File Offset: 0x001F71F6
		public void SetDataList(List<TData> dataList, bool enableBasicSort = true)
		{
			this._enableBasicSort = enableBasicSort;
			this._originalDataList = dataList;
			this.FilterAndSortDataList();
		}

		// Token: 0x06003F48 RID: 16200 RVA: 0x001F9010 File Offset: 0x001F7210
		public DynamicLineConfig GetDynamicConfigForLine(int lineId)
		{
			List<FilterLineBase<TData>> filterLines = this._filterLines;
			FilterLineBase<TData> filterLine = (filterLines != null) ? filterLines.Find((FilterLineBase<TData> f) => f.Id == lineId) : null;
			return (filterLine != null) ? filterLine.GenerateDynamicConfig(this._originalDataList) : null;
		}

		// Token: 0x06003F49 RID: 16201 RVA: 0x001F9060 File Offset: 0x001F7260
		private SortAndFilterConfig GenerateSortAndFilterConfig()
		{
			List<LineConfig> lineConfigs = new List<LineConfig>();
			bool flag = this._filterLines != null;
			if (flag)
			{
				foreach (FilterLineBase<TData> filterLine in this._filterLines)
				{
					lineConfigs.Add(filterLine.GenerateConfig());
				}
			}
			return new SortAndFilterConfig
			{
				LineConfigs = lineConfigs,
				SortConfig = new SortConfig
				{
					UiConfig = this.SortController.GenerateConfig()
				},
				OnSortChanged = new Action(this.OnSortChanged),
				OnFilterChanged = new Action<int>(this.OnFilterChanged),
				OnFilterCustomOrderChanged = new Action<int>(this.OnFilterCustomOrderChanged),
				OnFilterCustomOrderReset = new Action(this.OnFilterCustomOrderReset),
				DynamicConfigProvider = new Func<int, DynamicLineConfig>(this.GetDynamicConfigForLine)
			};
		}

		// Token: 0x06003F4A RID: 16202 RVA: 0x001F915C File Offset: 0x001F735C
		private void OnFilterCustomOrderReset()
		{
			this._filterCustomOrderStore.ClearFilterCustomOrderData(this.FilterCustomOrderKey);
		}

		// Token: 0x06003F4B RID: 16203 RVA: 0x001F9174 File Offset: 0x001F7374
		private void OnFilterCustomOrderChanged(int lineId)
		{
			IFilterLineCustomOrderData lineOrder = this._sortAndFilter.GetCustomOrderFromLine(lineId);
			this._customOrderData.LineCustomOrder[lineId] = lineOrder;
			this._sortAndFilter.ApplyCustomOrderForLine(lineId, lineOrder);
			this._filterCustomOrderStore.SaveFilterCustomOrderData(this.FilterCustomOrderKey, this._customOrderData);
		}

		// Token: 0x06003F4C RID: 16204 RVA: 0x001F91C8 File Offset: 0x001F73C8
		protected virtual void OnFilterChanged(int lineId)
		{
			bool flag = lineId >= 0;
			if (flag)
			{
				this._sortAndFilter.UpdateLineActive(this.GetLineIndexById(lineId));
			}
			this.FilterAndSortDataList();
		}

		// Token: 0x06003F4D RID: 16205 RVA: 0x001F91FC File Offset: 0x001F73FC
		private void RefreshSort()
		{
			List<FilterLineBase<TData>> activeSort = new List<FilterLineBase<TData>>();
			List<LineState> lineStates = new List<LineState>();
			bool flag = this._filterLines != null;
			if (flag)
			{
				foreach (FilterLineBase<TData> item in this._filterLines)
				{
					int index = this.GetLineIndexById(item.Id);
					bool flag2 = this._sortAndFilterState.LineStates.Count > index && this._sortAndFilterState.LineStates[index].IsActive;
					if (flag2)
					{
						activeSort.Add(item);
						lineStates.Add(this._sortAndFilterState.LineStates[index]);
					}
				}
			}
			this._sortAndFilter.RefreshSortDisplay<TData>(activeSort, lineStates);
		}

		// Token: 0x06003F4E RID: 16206 RVA: 0x001F92E0 File Offset: 0x001F74E0
		private void OnSortChanged()
		{
			this.SortController.SaveState();
			this.FilterAndSortDataList();
			this.SyncSortStateToTableHeads();
		}

		// Token: 0x06003F4F RID: 16207 RVA: 0x001F9300 File Offset: 0x001F7500
		protected int GetLineIndexById(int id)
		{
			bool flag = this._filterLines != null;
			if (flag)
			{
				for (int i = 0; i < this._filterLines.Count; i++)
				{
					bool flag2 = this._filterLines[i].Id == id;
					if (flag2)
					{
						return i;
					}
				}
			}
			throw new ArgumentException(string.Format("No filter line found with id {0}", id));
		}

		// Token: 0x06003F50 RID: 16208
		protected abstract IEnumerable<FilterLineBase<TData>> GenerateFilterLines();

		// Token: 0x06003F51 RID: 16209 RVA: 0x001F9370 File Offset: 0x001F7570
		private void RegisterFilterLines()
		{
			this._filterLines.Clear();
			IEnumerable<FilterLineBase<TData>> filterLines = this.GenerateFilterLines();
			bool flag = filterLines != null;
			if (flag)
			{
				this._filterLines.AddRange(filterLines);
			}
		}

		// Token: 0x06003F52 RID: 16210 RVA: 0x001F93A8 File Offset: 0x001F75A8
		private bool IsDataMatchFilter(TData data)
		{
			bool flag = this._filterLines == null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				foreach (FilterLineBase<TData> filterLine in this._filterLines)
				{
					int index = this.GetLineIndexById(filterLine.Id);
					bool flag2 = !this._sortAndFilterState.LineStates[index].IsActive;
					if (!flag2)
					{
						LineState lineState = this._sortAndFilterState.LineStates[index];
						bool flag3 = !filterLine.IsDataMatch(data, lineState);
						if (flag3)
						{
							return false;
						}
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06003F53 RID: 16211 RVA: 0x001F946C File Offset: 0x001F766C
		private void FilterAndSortDataList()
		{
			CommonSortAndFilterController<TData>.<>c__DisplayClass41_0 CS$<>8__locals1 = new CommonSortAndFilterController<TData>.<>c__DisplayClass41_0();
			CS$<>8__locals1.<>4__this = this;
			bool flag = this._originalDataList == null;
			if (!flag)
			{
				this._sortAndFilterState = this._sortAndFilter.GetStateFromUI();
				this.RefreshSort();
				this._sortedFilteredDataList.Clear();
				this._sortedFilteredDataList.AddRange(from data in this._originalDataList
				where CS$<>8__locals1.<>4__this.IsDataMatchFilter(data)
				select data);
				CommonSortAndFilterController<TData>.<>c__DisplayClass41_0 CS$<>8__locals2 = CS$<>8__locals1;
				CommonSortDropdown sortDropdown = this._sortAndFilter.SortDropdown;
				CS$<>8__locals2.displayingSortIds = (((sortDropdown != null) ? sortDropdown.DisplayingSortIds : null) ?? new HashSet<short>());
				SortStateData sortStateData = new SortStateData();
				SortStateData sortData = this._sortAndFilterState.SortData;
				List<SortItemState> list;
				if (sortData == null)
				{
					list = null;
				}
				else
				{
					List<SortItemState> itemStates = sortData.ItemStates;
					list = ((itemStates != null) ? (from s in itemStates
					where CS$<>8__locals1.<>4__this.ShouldSortIdParticipate(s.SortId, CS$<>8__locals1.displayingSortIds)
					select s).ToList<SortItemState>() : null);
				}
				sortStateData.ItemStates = (list ?? new List<SortItemState>());
				SortStateData effectiveSortData = sortStateData;
				bool flag2 = this._enableBasicSort || effectiveSortData.ItemStates.Count > 0;
				if (flag2)
				{
					this.SortController.Sort(this._sortedFilteredDataList, effectiveSortData, delegate
					{
						Action onDataListChanged2 = CS$<>8__locals1.<>4__this._onDataListChanged;
						if (onDataListChanged2 != null)
						{
							onDataListChanged2();
						}
					});
				}
				else
				{
					Action onDataListChanged = this._onDataListChanged;
					if (onDataListChanged != null)
					{
						onDataListChanged();
					}
				}
			}
		}

		// Token: 0x06003F54 RID: 16212 RVA: 0x001F95A3 File Offset: 0x001F77A3
		public void SetVisible(bool isVisible)
		{
			this._sortAndFilter.gameObject.SetActive(isVisible);
		}

		// Token: 0x06003F55 RID: 16213 RVA: 0x001F95B8 File Offset: 0x001F77B8
		public void SetSortState(SortStateData sortData)
		{
			this.SortController.SetSortData(sortData);
			this.OnSortChanged();
		}

		// Token: 0x06003F56 RID: 16214 RVA: 0x001F95CF File Offset: 0x001F77CF
		public void SetDropdownOptionInteractable(int lineId, int menuId, int optionIndex, bool interactable, string disabledTooltip)
		{
			this._sortAndFilter.SetDropdownOptionInteractable(lineId, menuId, optionIndex, interactable, disabledTooltip);
		}

		// Token: 0x06003F57 RID: 16215 RVA: 0x001F95E5 File Offset: 0x001F77E5
		public void SetDropdownMenuVisible(int lineId, int menuId, bool visible)
		{
			this._sortAndFilter.SetDropdownMenuVisible(lineId, menuId, visible);
		}

		// Token: 0x06003F58 RID: 16216 RVA: 0x001F95F7 File Offset: 0x001F77F7
		public void SetDropdownOption(int lineId, int menuId, int optionIndex)
		{
			this._sortAndFilter.SetDropdownOption(lineId, menuId, optionIndex);
		}

		// Token: 0x06003F59 RID: 16217 RVA: 0x001F9609 File Offset: 0x001F7809
		public void SetToggleVisible(ToggleIdIndex toggleIndex, bool isVisible)
		{
			this._sortAndFilter.SetToggleVisible(toggleIndex, isVisible);
		}

		// Token: 0x06003F5A RID: 16218 RVA: 0x001F961C File Offset: 0x001F781C
		public void SetToggleVisible(int lineId, int toggleIndex)
		{
			bool flag = this.FilterLines == null;
			if (!flag)
			{
				FilterLineBase<TData> line = this.FilterLines.Find((FilterLineBase<TData> l) => l.Id == lineId);
				List<ToggleIdIndex> toggleIdIndexList = new List<ToggleIdIndex>
				{
					new ToggleIdIndex(line.Id, ToggleKey.AllKey)
				};
				List<FilterToggleConfig> toggleConfigs = line.GenerateConfig().ToggleGroupLineConfig.Config.FilterToggleConfigs;
				for (int index = 0; index < toggleConfigs.Count; index++)
				{
					toggleIdIndexList.Add(new ToggleIdIndex(line.Id, new ToggleKey
					{
						Index = index,
						IsAll = false
					}));
				}
				foreach (ToggleIdIndex toggleIdIndex in toggleIdIndexList)
				{
					bool visible = toggleIndex < 0 || toggleIndex == toggleIdIndex.ToggleKey.Index;
					this.SetToggleVisible(toggleIdIndex, visible);
				}
			}
		}

		// Token: 0x06003F5B RID: 16219 RVA: 0x001F9744 File Offset: 0x001F7944
		public void SetToggleVisible(int lineId, List<int> toggleIndexList)
		{
			bool flag = this.FilterLines == null;
			if (!flag)
			{
				FilterLineBase<TData> line = this.FilterLines.Find((FilterLineBase<TData> l) => l.Id == lineId);
				List<ToggleIdIndex> toggleIdIndexList = new List<ToggleIdIndex>
				{
					new ToggleIdIndex(line.Id, ToggleKey.AllKey)
				};
				List<FilterToggleConfig> toggleConfigs = line.GenerateConfig().ToggleGroupLineConfig.Config.FilterToggleConfigs;
				for (int index = 0; index < toggleConfigs.Count; index++)
				{
					toggleIdIndexList.Add(new ToggleIdIndex(line.Id, new ToggleKey
					{
						Index = index,
						IsAll = false
					}));
				}
				for (int index2 = 0; index2 < toggleIdIndexList.Count; index2++)
				{
					ToggleIdIndex toggleIdIndex = toggleIdIndexList[index2];
					bool isAll = index2 == 0;
					bool visible = (isAll && toggleIndexList.Count > 1) || toggleIndexList.Contains(toggleIdIndex.ToggleKey.Index);
					this.SetToggleVisible(toggleIdIndex, visible);
				}
			}
		}

		// Token: 0x06003F5C RID: 16220 RVA: 0x001F986C File Offset: 0x001F7A6C
		private void SetToggleIsOn(ToggleIdIndex toggleIndex, bool isOn)
		{
			this._sortAndFilter.SetToggleIsOn(toggleIndex, isOn);
		}

		// Token: 0x06003F5D RID: 16221 RVA: 0x001F9880 File Offset: 0x001F7A80
		public void SetToggleIsOn(int lineId, int toggleIndex)
		{
			bool flag = this.FilterLines == null;
			if (!flag)
			{
				FilterLineBase<TData> line = this.FilterLines.Find((FilterLineBase<TData> l) => l.Id == lineId);
				ToggleIdIndex allToggleIdIndex = new ToggleIdIndex(line.Id, ToggleKey.AllKey);
				bool flag2 = toggleIndex < 0;
				if (flag2)
				{
					this.SetToggleIsOn(allToggleIdIndex, true);
				}
				else
				{
					List<ToggleIdIndex> toggleIdIndexList = new List<ToggleIdIndex>
					{
						allToggleIdIndex
					};
					List<FilterToggleConfig> toggleConfigs = line.GenerateConfig().ToggleGroupLineConfig.Config.FilterToggleConfigs;
					for (int index = 0; index < toggleConfigs.Count; index++)
					{
						toggleIdIndexList.Add(new ToggleIdIndex(line.Id, new ToggleKey
						{
							Index = index,
							IsAll = false
						}));
					}
					foreach (ToggleIdIndex toggleIdIndex in toggleIdIndexList)
					{
						bool isOn = toggleIndex == toggleIdIndex.ToggleKey.Index;
						this.SetToggleIsOn(toggleIdIndex, isOn);
					}
				}
			}
		}

		// Token: 0x06003F5E RID: 16222 RVA: 0x001F99BC File Offset: 0x001F7BBC
		public void ClearAllFilter()
		{
			this._sortAndFilter.ClearAllFilter();
		}

		// Token: 0x17000673 RID: 1651
		// (get) Token: 0x06003F5F RID: 16223 RVA: 0x001F99CB File Offset: 0x001F7BCB
		SortAndFilterState ICommonSortAndFilterController.SortAndFilterState
		{
			get
			{
				return this._sortAndFilterState;
			}
		}

		// Token: 0x17000674 RID: 1652
		// (get) Token: 0x06003F60 RID: 16224 RVA: 0x001F99D3 File Offset: 0x001F7BD3
		IReadOnlyList<object> ICommonSortAndFilterController.ReadOnlyOriginalDataList
		{
			get
			{
				List<TData> originalDataList = this._originalDataList;
				return (originalDataList != null) ? originalDataList.Cast<object>().ToList<object>().AsReadOnly() : null;
			}
		}

		// Token: 0x06003F61 RID: 16225 RVA: 0x001F99F1 File Offset: 0x001F7BF1
		void ICommonSortAndFilterController.SetSortState(SortStateData sortData)
		{
			this.SetSortState(sortData);
		}

		// Token: 0x06003F62 RID: 16226 RVA: 0x001F99FC File Offset: 0x001F7BFC
		private bool ShouldSortIdParticipate(short sortId, HashSet<short> displayingSortIds)
		{
			bool flag = this._sortIdsUsedByTableHead.Contains(sortId);
			return flag || displayingSortIds.Count == 0 || displayingSortIds.Contains(sortId);
		}

		// Token: 0x06003F63 RID: 16227 RVA: 0x001F9A34 File Offset: 0x001F7C34
		public void RegisterTableHead(CommonTableHead tableHead, short[] columnSortIds)
		{
			bool flag = tableHead == null;
			if (!flag)
			{
				bool flag2 = this._registeredTableHeads.Contains(tableHead);
				if (!flag2)
				{
					this._registeredTableHeads.Add(tableHead);
					tableHead.BindSortController(this, columnSortIds);
					bool flag3 = columnSortIds != null;
					if (flag3)
					{
						foreach (short sortId in columnSortIds)
						{
							bool flag4 = sortId >= 0;
							if (flag4)
							{
								this._sortIdsUsedByTableHead.Add(sortId);
							}
						}
					}
				}
			}
		}

		// Token: 0x06003F64 RID: 16228 RVA: 0x001F9ABC File Offset: 0x001F7CBC
		public void UnregisterTableHead(CommonTableHead tableHead)
		{
			bool flag = tableHead == null;
			if (!flag)
			{
				bool flag2 = this._registeredTableHeads.Remove(tableHead);
				if (flag2)
				{
					tableHead.UnbindSortController();
				}
			}
		}

		// Token: 0x06003F65 RID: 16229 RVA: 0x001F9AF0 File Offset: 0x001F7CF0
		private void SyncSortStateToTableHeads()
		{
			foreach (CommonTableHead tableHead in this._registeredTableHeads)
			{
				bool flag = tableHead != null;
				if (flag)
				{
					tableHead.SyncSortStateFromController();
				}
			}
		}

		// Token: 0x04002D4F RID: 11599
		private readonly List<FilterLineBase<TData>> _filterLines = new List<FilterLineBase<TData>>();

		// Token: 0x04002D50 RID: 11600
		private readonly CommonSortAndFilter _sortAndFilter;

		// Token: 0x04002D51 RID: 11601
		private readonly List<TData> _sortedFilteredDataList = new List<TData>();

		// Token: 0x04002D52 RID: 11602
		private Action _onDataListChanged;

		// Token: 0x04002D53 RID: 11603
		private List<TData> _originalDataList;

		// Token: 0x04002D54 RID: 11604
		private SortAndFilterState _sortAndFilterState;

		// Token: 0x04002D55 RID: 11605
		protected CommonSortController<TData> SortController;

		// Token: 0x04002D56 RID: 11606
		private readonly List<CommonTableHead> _registeredTableHeads = new List<CommonTableHead>();

		// Token: 0x04002D57 RID: 11607
		private readonly HashSet<short> _sortIdsUsedByTableHead = new HashSet<short>();

		// Token: 0x04002D58 RID: 11608
		private readonly IFilterCustomOrderStore _filterCustomOrderStore;

		// Token: 0x04002D59 RID: 11609
		private FilterCustomOrderData _customOrderData;

		// Token: 0x04002D5A RID: 11610
		private bool _isInit;

		// Token: 0x04002D5B RID: 11611
		private bool _enableBasicSort;

		// Token: 0x020018CC RID: 6348
		// (Invoke) Token: 0x0600D7E8 RID: 55272
		public delegate bool FilterFunction(TData data);
	}
}
