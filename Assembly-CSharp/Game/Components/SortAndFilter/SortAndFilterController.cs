using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using UnityEngine;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CBC RID: 3260
	public abstract class SortAndFilterController<TData> : ISortAndFilterController
	{
		// Token: 0x17001139 RID: 4409
		// (get) Token: 0x0600A519 RID: 42265 RVA: 0x004CF3DB File Offset: 0x004CD5DB
		public SortAndFilterState SortAndFilterState
		{
			get
			{
				return this._sortAndFilterState;
			}
		}

		// Token: 0x1700113A RID: 4410
		// (get) Token: 0x0600A51A RID: 42266 RVA: 0x004CF3E3 File Offset: 0x004CD5E3
		public List<FilterLineBase<TData>> FilterLines
		{
			get
			{
				return this._filterLines;
			}
		}

		// Token: 0x0600A51B RID: 42267 RVA: 0x004CF3EC File Offset: 0x004CD5EC
		protected SortAndFilterController(ISortAndFilterView sortAndFilter, LanguageKey panelTitleKey = LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this._sortAndFilter = sortAndFilter;
			this._sortAndFilterBehaviour = (sortAndFilter as MonoBehaviour);
			this._panelTitleKey = panelTitleKey;
		}

		// Token: 0x1700113B RID: 4411
		// (get) Token: 0x0600A51C RID: 42268 RVA: 0x004CF43C File Offset: 0x004CD63C
		public SortAndFilterConfig OriginalConfig
		{
			get
			{
				return this._sortAndFilter.Config;
			}
		}

		// Token: 0x0600A51D RID: 42269 RVA: 0x004CF44C File Offset: 0x004CD64C
		public void Init(Action onStateChanged, string sortSaveKey)
		{
			bool isInit = this._isInit;
			if (!isInit)
			{
				this.RegisterFilterLines();
				SortAndFilterConfig config = this.GenerateSortAndFilterConfig();
				this._sortAndFilter.Setup(config);
				this._onStateChanged = onStateChanged;
				this.SortController.Init(this._sortAndFilter.SortUi, sortSaveKey);
				this._isInit = true;
				SortAndFilter sortAndFilter2 = this._sortAndFilter as SortAndFilter;
				bool flag = sortAndFilter2 != null && this._panelTitleKey > LanguageKey.EventEditor_Error_DuplicateGroupKey;
				if (flag)
				{
					sortAndFilter2.SetPanelTitleKey(this._panelTitleKey);
				}
				GEvent.Add(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
			}
		}

		// Token: 0x0600A51E RID: 42270 RVA: 0x004CF4F4 File Offset: 0x004CD6F4
		public void InitSaveSortKey(string sortSaveKey)
		{
			this.SortController.Init(this._sortAndFilter.SortUi, sortSaveKey);
		}

		// Token: 0x0600A51F RID: 42271 RVA: 0x004CF510 File Offset: 0x004CD710
		public void UninitForReplace()
		{
			bool flag = !this._isInit;
			if (!flag)
			{
				GEvent.Remove(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
				this._isInit = false;
				this._onStateChanged = null;
				this._cachedDataList = null;
			}
		}

		// Token: 0x0600A520 RID: 42272 RVA: 0x004CF560 File Offset: 0x004CD760
		public void NotifyDataChanged(IEnumerable<TData> dataList)
		{
			this._cachedDataList = dataList;
			this._sortAndFilter.RefreshDynamicConfigs();
			SortAndFilter sortAndFilter2 = this._sortAndFilter as SortAndFilter;
			bool flag = sortAndFilter2 != null;
			if (flag)
			{
				sortAndFilter2.UpdateLineActive(0);
			}
			this.RefreshSortDisplay();
		}

		// Token: 0x0600A521 RID: 42273 RVA: 0x004CF5A8 File Offset: 0x004CD7A8
		public Func<TData, bool> GenerateFilter()
		{
			this._sortAndFilterState = this._sortAndFilter.GetStateFromUI();
			return new Func<TData, bool>(this.IsDataMatchFilter);
		}

		// Token: 0x0600A522 RID: 42274 RVA: 0x004CF5D8 File Offset: 0x004CD7D8
		public Comparison<TData> GenerateComparer(IReadOnlyList<TData> dataList)
		{
			SortAndFilterController<TData>.<>c__DisplayClass24_0 CS$<>8__locals1 = new SortAndFilterController<TData>.<>c__DisplayClass24_0();
			CS$<>8__locals1.<>4__this = this;
			this._sortAndFilterState = this._sortAndFilter.GetStateFromUI();
			this.RefreshSortDisplay();
			SortAndFilterController<TData>.<>c__DisplayClass24_0 CS$<>8__locals2 = CS$<>8__locals1;
			ISortUiWithDisplayingSortIds sortUiWithDisplayingSortIds = this._sortAndFilter.SortUi as ISortUiWithDisplayingSortIds;
			CS$<>8__locals2.displayingSortIds = (((sortUiWithDisplayingSortIds != null) ? sortUiWithDisplayingSortIds.DisplayingSortIds : null) ?? new HashSet<short>());
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
			return this.SortController.GenerateComparer(effectiveSortData);
		}

		// Token: 0x0600A523 RID: 42275 RVA: 0x004CF68F File Offset: 0x004CD88F
		public void SetFilteredCount(int filteredCount)
		{
			this.SyncFilteredCount(filteredCount);
		}

		// Token: 0x0600A524 RID: 42276 RVA: 0x004CF69C File Offset: 0x004CD89C
		public void AfterFilter(IEnumerable<TData> allData)
		{
			IReadOnlyList<TData> list = (allData as IReadOnlyList<TData>) ?? allData.ToList<TData>();
			int totalCount = list.Count(new Func<TData, bool>(this.IsDataMatchFilter));
			this.SyncFilteredCount(totalCount);
			this.RefreshFilterOptionCounts(list);
		}

		// Token: 0x0600A525 RID: 42277 RVA: 0x004CF6E0 File Offset: 0x004CD8E0
		private void RefreshFilterOptionCounts(IReadOnlyList<TData> allData)
		{
			List<OptionCountData> optionCounts = new List<OptionCountData>();
			foreach (FilterLineBase<TData> filterLine in this._filterLines)
			{
				DetailedFilterLineLogic<TData> detailedLine = filterLine as DetailedFilterLineLogic<TData>;
				bool flag = detailedLine != null;
				if (flag)
				{
					int lineIndex = this.GetLineIndexById(filterLine.Id);
					bool flag2 = lineIndex < 0 || lineIndex >= this._sortAndFilterState.LineStates.Count;
					if (!flag2)
					{
						LineState lineState = this._sortAndFilterState.LineStates[lineIndex];
						bool flag3 = !lineState.IsActive;
						if (!flag3)
						{
							bool flag4 = lineState.DetailedFilterState == null || lineState.DetailedFilterState.State.MenuStateDict == null;
							if (!flag4)
							{
								foreach (DetailedFilterMenuLogic<TData> menu in detailedLine.Menus)
								{
									DetailedFilterMenuState menuState;
									bool flag5 = !lineState.DetailedFilterState.State.MenuStateDict.TryGetValue(menu.Id, out menuState);
									if (!flag5)
									{
										bool flag6 = !menuState.IsActive;
										if (!flag6)
										{
											DynamicDetailedFilterMenuLogic<TData> dynamicMenu = menu as DynamicDetailedFilterMenuLogic<TData>;
											bool flag7 = dynamicMenu != null;
											List<FilterDropdownItemConfig> itemConfigs;
											if (flag7)
											{
												itemConfigs = dynamicMenu.GetDynamicMenuConfigs(allData);
											}
											else
											{
												itemConfigs = menu.GetMenuItemConfigs();
											}
											bool flag8 = itemConfigs == null;
											if (!flag8)
											{
												int allCount = this.CountWithSimulatedSelection(allData, filterLine.Id, menu.Id, new List<int>());
												optionCounts.Add(new OptionCountData
												{
													LineId = filterLine.Id,
													MenuId = menu.Id,
													OptionIndex = -1,
													Count = allCount
												});
												for (int optIdx = 0; optIdx < itemConfigs.Count; optIdx++)
												{
													int count = this.CountWithSimulatedSelection(allData, filterLine.Id, menu.Id, new List<int>
													{
														optIdx
													});
													optionCounts.Add(new OptionCountData
													{
														LineId = filterLine.Id,
														MenuId = menu.Id,
														OptionIndex = optIdx,
														Count = count
													});
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			IFilteredCountView filteredCountView = this._sortAndFilter as IFilteredCountView;
			bool flag9 = filteredCountView != null;
			if (flag9)
			{
				filteredCountView.SetFilterOptionCounts(optionCounts);
			}
		}

		// Token: 0x0600A526 RID: 42278 RVA: 0x004CF9B0 File Offset: 0x004CDBB0
		private int CountWithSimulatedSelection(IEnumerable<TData> allData, int lineId, int menuId, List<int> simulatedSelection)
		{
			int lineIndex = this.GetLineIndexById(lineId);
			List<LineState> modifiedStates = new List<LineState>(this._sortAndFilterState.LineStates.Count);
			for (int i = 0; i < this._sortAndFilterState.LineStates.Count; i++)
			{
				LineState ls = this._sortAndFilterState.LineStates[i];
				bool flag = i == lineIndex && ls.DetailedFilterState != null && ls.DetailedFilterState.State.MenuStateDict != null;
				if (flag)
				{
					Dictionary<int, DetailedFilterMenuState> dictCopy = new Dictionary<int, DetailedFilterMenuState>(ls.DetailedFilterState.State.MenuStateDict);
					bool flag2 = dictCopy.ContainsKey(menuId);
					if (flag2)
					{
						dictCopy[menuId] = new DetailedFilterMenuState(simulatedSelection, true);
					}
					ToggleKey toggleGroupState = ls.ToggleGroupState;
					bool flag3 = ls.Type == ESortAndFilterOneLineType.SingleSelectFilter;
					if (flag3)
					{
						toggleGroupState = ((simulatedSelection.Count > 0) ? ToggleKey.CreateIndexKey(simulatedSelection[0]) : new ToggleKey
						{
							IsAll = false,
							Index = -1
						});
					}
					DetailedFilterLineLogic<TData> targetLine = this._filterLines[i] as DetailedFilterLineLogic<TData>;
					bool flag4 = targetLine != null;
					if (flag4)
					{
						foreach (DetailedFilterMenuLogic<TData> j in targetLine.Menus)
						{
							bool flag5 = j.Dependency != null && j.Dependency.Value.MenuId == menuId;
							if (flag5)
							{
								DetailedFilterMenuState ds;
								bool flag6 = !simulatedSelection.Contains(j.Dependency.Value.OptionIndex) && dictCopy.TryGetValue(j.Id, out ds);
								if (flag6)
								{
									dictCopy[j.Id] = new DetailedFilterMenuState(ds.SelectedIndices, false);
								}
							}
						}
					}
					modifiedStates.Add(new LineState
					{
						IsActive = ls.IsActive,
						Type = ls.Type,
						ToggleGroupState = toggleGroupState,
						DetailedFilterState = new DetailedFilterLineState
						{
							State = new DetailedFilterState
							{
								MenuStateDict = dictCopy
							}
						}
					});
				}
				else
				{
					modifiedStates.Add(ls);
				}
			}
			ToggleKey depToggle = modifiedStates[lineIndex].ToggleGroupState;
			SortAndFilterConfig config = this._sortAndFilter.Config;
			bool flag7 = ((config != null) ? config.LineConfigs : null) != null;
			if (flag7)
			{
				for (int k = 0; k < modifiedStates.Count; k++)
				{
					bool flag8 = k == lineIndex;
					if (!flag8)
					{
						LineConfig lineCfg = this._sortAndFilter.Config.LineConfigs[k];
						bool flag9 = lineCfg.ActiveCondition == null || lineCfg.ActiveCondition.Value.ActiveDependsOn == null;
						if (!flag9)
						{
							foreach (ToggleIdIndex dep in lineCfg.ActiveCondition.Value.ActiveDependsOn)
							{
								try
								{
									bool flag10 = this.GetLineIndexById(dep.LineId) == lineIndex;
									if (flag10)
									{
										bool satisfied = dep.ToggleKey.IsAll || dep.ToggleKey.Index == depToggle.Index;
										bool flag11 = !satisfied;
										if (flag11)
										{
											LineState ms = modifiedStates[k];
											modifiedStates[k] = new LineState
											{
												IsActive = false,
												Type = ms.Type,
												ToggleGroupState = ms.ToggleGroupState,
												DetailedFilterState = ms.DetailedFilterState
											};
										}
										break;
									}
								}
								catch
								{
								}
							}
						}
					}
				}
			}
			return allData.Count(delegate(TData data)
			{
				foreach (FilterLineBase<TData> fl in this._filterLines)
				{
					int idx = this.GetLineIndexById(fl.Id);
					bool flag12 = !modifiedStates[idx].IsActive;
					if (!flag12)
					{
						bool flag13 = !fl.IsDataMatch(data, modifiedStates[idx]);
						if (flag13)
						{
							return false;
						}
					}
				}
				return true;
			});
		}

		// Token: 0x0600A527 RID: 42279 RVA: 0x004CFE30 File Offset: 0x004CE030
		private void SyncFilteredCount(int filteredCount)
		{
			IFilteredCountView filteredCountView = this._sortAndFilter as IFilteredCountView;
			bool flag = filteredCountView != null;
			if (flag)
			{
				filteredCountView.SetFilteredCount(filteredCount);
			}
		}

		// Token: 0x0600A528 RID: 42280 RVA: 0x004CFE5C File Offset: 0x004CE05C
		private DynamicLineConfig GetDynamicConfigForLine(int lineId)
		{
			bool flag = this._cachedDataList == null;
			DynamicLineConfig result;
			if (flag)
			{
				result = null;
			}
			else
			{
				List<FilterLineBase<TData>> filterLines = this._filterLines;
				FilterLineBase<TData> filterLine = (filterLines != null) ? filterLines.Find((FilterLineBase<TData> f) => f.Id == lineId) : null;
				result = ((filterLine != null) ? filterLine.GenerateDynamicConfig(this._cachedDataList) : null);
			}
			return result;
		}

		// Token: 0x0600A529 RID: 42281 RVA: 0x004CFEC0 File Offset: 0x004CE0C0
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
				DynamicConfigProvider = new Func<int, DynamicLineConfig>(this.GetDynamicConfigForLine)
			};
		}

		// Token: 0x0600A52A RID: 42282 RVA: 0x004CFF98 File Offset: 0x004CE198
		protected virtual void OnFilterChanged(int lineId)
		{
			bool flag = lineId >= 0;
			if (flag)
			{
				this._sortAndFilter.UpdateLineActive(this.GetLineIndexById(lineId));
			}
			this._sortAndFilterState = this._sortAndFilter.GetStateFromUI();
			Action onStateChanged = this._onStateChanged;
			if (onStateChanged != null)
			{
				onStateChanged();
			}
		}

		// Token: 0x0600A52B RID: 42283 RVA: 0x004CFFE7 File Offset: 0x004CE1E7
		protected void UpdateStateFromUI()
		{
			this._sortAndFilterState = this._sortAndFilter.GetStateFromUI();
		}

		// Token: 0x0600A52C RID: 42284 RVA: 0x004CFFFB File Offset: 0x004CE1FB
		protected void InvokeStateChanged()
		{
			Action onStateChanged = this._onStateChanged;
			if (onStateChanged != null)
			{
				onStateChanged();
			}
		}

		// Token: 0x1700113C RID: 4412
		// (get) Token: 0x0600A52D RID: 42285 RVA: 0x004D0010 File Offset: 0x004CE210
		protected ISortAndFilterView SortAndFilter
		{
			get
			{
				return this._sortAndFilter;
			}
		}

		// Token: 0x0600A52E RID: 42286 RVA: 0x004D0018 File Offset: 0x004CE218
		private void RefreshSortDisplay()
		{
			this._sortAndFilterState = this._sortAndFilter.GetStateFromUI();
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

		// Token: 0x0600A52F RID: 42287 RVA: 0x004D0110 File Offset: 0x004CE310
		private void OnSortChanged()
		{
			this.SortController.SaveState();
			this._sortAndFilterState = this._sortAndFilter.GetStateFromUI();
			this.SyncSortStateToTableHeads();
			Action onStateChanged = this._onStateChanged;
			if (onStateChanged != null)
			{
				onStateChanged();
			}
		}

		// Token: 0x0600A530 RID: 42288 RVA: 0x004D014C File Offset: 0x004CE34C
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

		// Token: 0x0600A531 RID: 42289
		protected abstract IEnumerable<FilterLineBase<TData>> GenerateFilterLines();

		// Token: 0x0600A532 RID: 42290 RVA: 0x004D01BC File Offset: 0x004CE3BC
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

		// Token: 0x0600A533 RID: 42291 RVA: 0x004D01F4 File Offset: 0x004CE3F4
		private void OnLanguageChange(ArgumentBox _)
		{
			bool flag = this._sortAndFilterBehaviour == null;
			if (flag)
			{
				GEvent.Remove(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChange));
			}
			else
			{
				bool flag2 = !this._isInit;
				if (!flag2)
				{
					this._filterLines.Clear();
					this.RegisterFilterLines();
					SortAndFilterConfig config = this.GenerateSortAndFilterConfig();
					this._sortAndFilter.RebuildForLanguageChange(config);
				}
			}
		}

		// Token: 0x0600A534 RID: 42292 RVA: 0x004D0268 File Offset: 0x004CE468
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

		// Token: 0x0600A535 RID: 42293 RVA: 0x004D032C File Offset: 0x004CE52C
		public void SetVisible(bool isVisible)
		{
			this._sortAndFilter.gameObject.SetActive(isVisible);
		}

		// Token: 0x0600A536 RID: 42294 RVA: 0x004D0341 File Offset: 0x004CE541
		public void SetSortState(SortStateData sortData)
		{
			this.SortController.SetSortData(sortData);
			this.OnSortChanged();
		}

		// Token: 0x0600A537 RID: 42295 RVA: 0x004D0358 File Offset: 0x004CE558
		public void SetDropdownOptionInteractable(int lineId, int menuId, int optionIndex, bool interactable, string disabledTooltip)
		{
			this._sortAndFilter.SetDropdownOptionInteractable(lineId, menuId, optionIndex, interactable, disabledTooltip);
		}

		// Token: 0x0600A538 RID: 42296 RVA: 0x004D036E File Offset: 0x004CE56E
		public void SetDropdownMenuVisible(int lineId, int menuId, bool visible)
		{
			this._sortAndFilter.SetDropdownMenuVisible(lineId, menuId, visible);
		}

		// Token: 0x0600A539 RID: 42297 RVA: 0x004D0380 File Offset: 0x004CE580
		public void SetVisibleDropdownMenus(int lineId, IEnumerable<int> visibleMenuIds)
		{
			this._sortAndFilter.SetVisibleDropdownMenus(lineId, visibleMenuIds);
		}

		// Token: 0x0600A53A RID: 42298 RVA: 0x004D0394 File Offset: 0x004CE594
		public void SetVisibleSortIds(IEnumerable<short> visibleSortIds)
		{
			SortAndFilter sortAndFilter2 = this._sortAndFilter as SortAndFilter;
			bool flag = sortAndFilter2 != null;
			if (flag)
			{
				sortAndFilter2.SetVisibleSortIds(visibleSortIds);
				this._sortAndFilterState = this._sortAndFilter.GetStateFromUI();
				this.SyncSortStateToTableHeads();
				Action onStateChanged = this._onStateChanged;
				if (onStateChanged != null)
				{
					onStateChanged();
				}
			}
		}

		// Token: 0x0600A53B RID: 42299 RVA: 0x004D03EA File Offset: 0x004CE5EA
		public void SetDropdownOption(int lineId, int menuId, int optionIndex)
		{
			this._sortAndFilter.SetDropdownOption(lineId, menuId, optionIndex);
		}

		// Token: 0x0600A53C RID: 42300 RVA: 0x004D03FC File Offset: 0x004CE5FC
		public void SetToggleVisible(ToggleIdIndex toggleIndex, bool isVisible)
		{
			this._sortAndFilter.SetToggleVisible(toggleIndex, isVisible);
		}

		// Token: 0x0600A53D RID: 42301 RVA: 0x004D0410 File Offset: 0x004CE610
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

		// Token: 0x0600A53E RID: 42302 RVA: 0x004D0538 File Offset: 0x004CE738
		public void SetToggleVisible(int lineId, List<int> toggleIndexList, bool isForceShowAll = false)
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
					bool visible = (isAll && (toggleIndexList.Count > 1 || isForceShowAll)) || toggleIndexList.Contains(toggleIdIndex.ToggleKey.Index);
					this.SetToggleVisible(toggleIdIndex, visible);
				}
			}
		}

		// Token: 0x0600A53F RID: 42303 RVA: 0x004D0664 File Offset: 0x004CE864
		public void ResetAllTogglesVisible(int lineId)
		{
			bool flag = this.FilterLines == null;
			if (!flag)
			{
				FilterLineBase<TData> line = this.FilterLines.Find((FilterLineBase<TData> l) => l.Id == lineId);
				bool flag2 = line == null;
				if (!flag2)
				{
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
						this.SetToggleVisible(toggleIdIndex, true);
					}
				}
			}
		}

		// Token: 0x0600A540 RID: 42304 RVA: 0x004D0780 File Offset: 0x004CE980
		public void SetToggleInteractable(ToggleIdIndex toggleIndex, bool interactable)
		{
			SortAndFilter sortAndFilter2 = this._sortAndFilter as SortAndFilter;
			bool flag = sortAndFilter2 != null;
			if (flag)
			{
				sortAndFilter2.SetToggleInteractable(toggleIndex, interactable);
			}
		}

		// Token: 0x0600A541 RID: 42305 RVA: 0x004D07B0 File Offset: 0x004CE9B0
		public void SetTogglesInteractable(int lineId, List<int> toggleIndexList)
		{
			bool flag = this.FilterLines == null;
			if (!flag)
			{
				FilterLineBase<TData> line = this.FilterLines.Find((FilterLineBase<TData> l) => l.Id == lineId);
				bool flag2 = line == null;
				if (!flag2)
				{
					List<int> allowed = toggleIndexList ?? new List<int>();
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
						bool interactable = toggleIdIndex.ToggleKey.IsAll || allowed.Contains(toggleIdIndex.ToggleKey.Index);
						this.SetToggleInteractable(toggleIdIndex, interactable);
					}
				}
			}
		}

		// Token: 0x0600A542 RID: 42306 RVA: 0x004D0900 File Offset: 0x004CEB00
		public void ResetAllTogglesInteractable(int lineId)
		{
			bool flag = this.FilterLines == null;
			if (!flag)
			{
				FilterLineBase<TData> line = this.FilterLines.Find((FilterLineBase<TData> l) => l.Id == lineId);
				bool flag2 = line == null;
				if (!flag2)
				{
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
						this.SetToggleInteractable(toggleIdIndex, true);
					}
				}
			}
		}

		// Token: 0x0600A543 RID: 42307 RVA: 0x004D0A1C File Offset: 0x004CEC1C
		private void SetToggleIsOn(ToggleIdIndex toggleIndex, bool isOn)
		{
			this._sortAndFilter.SetToggleIsOn(toggleIndex, isOn);
		}

		// Token: 0x0600A544 RID: 42308 RVA: 0x004D0A2D File Offset: 0x004CEC2D
		public void SetToggleIsOnWithoutNotify(int lineId, int toggleIndex)
		{
			this._sortAndFilter.SetToggleIsOnWithoutNotify(lineId, toggleIndex);
			this._sortAndFilter.UpdateLineActive(this.GetLineIndexById(lineId));
			this.UpdateStateFromUI();
		}

		// Token: 0x0600A545 RID: 42309 RVA: 0x004D0A58 File Offset: 0x004CEC58
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

		// Token: 0x0600A546 RID: 42310 RVA: 0x004D0B94 File Offset: 0x004CED94
		public void ClearAllFilter()
		{
			this._sortAndFilter.ClearAllFilter();
		}

		// Token: 0x1700113D RID: 4413
		// (get) Token: 0x0600A547 RID: 42311 RVA: 0x004D0BA3 File Offset: 0x004CEDA3
		public bool HasSavedFilterState
		{
			get
			{
				return this._savedFilterLineStates != null;
			}
		}

		// Token: 0x0600A548 RID: 42312 RVA: 0x004D0BAE File Offset: 0x004CEDAE
		public void SaveFilterStateFromUI()
		{
			this._sortAndFilterState = this._sortAndFilter.GetStateFromUI();
			this._savedFilterLineStates = SortAndFilterController<TData>.DeepCopyLineStates(this._sortAndFilterState.LineStates);
		}

		// Token: 0x0600A549 RID: 42313 RVA: 0x004D0BD8 File Offset: 0x004CEDD8
		public void RestoreFilterState()
		{
			bool flag = this._savedFilterLineStates == null;
			if (!flag)
			{
				this._sortAndFilter.ApplyFilterLineStates(this._savedFilterLineStates);
				this._savedFilterLineStates = null;
				this._sortAndFilterState = this._sortAndFilter.GetStateFromUI();
				Action onStateChanged = this._onStateChanged;
				if (onStateChanged != null)
				{
					onStateChanged();
				}
			}
		}

		// Token: 0x0600A54A RID: 42314 RVA: 0x004D0C34 File Offset: 0x004CEE34
		private static List<LineState> DeepCopyLineStates(List<LineState> source)
		{
			bool flag = source == null;
			List<LineState> result2;
			if (flag)
			{
				result2 = null;
			}
			else
			{
				List<LineState> result = new List<LineState>(source.Count);
				foreach (LineState ls in source)
				{
					DetailedFilterLineState detailedFilterState = ls.DetailedFilterState;
					bool flag2 = ((detailedFilterState != null) ? detailedFilterState.State.MenuStateDict : null) != null;
					if (flag2)
					{
						Dictionary<int, DetailedFilterMenuState> clonedDict = new Dictionary<int, DetailedFilterMenuState>();
						foreach (KeyValuePair<int, DetailedFilterMenuState> kvp in detailedFilterState.State.MenuStateDict)
						{
							IReadOnlyCollection<int> selectedIndices = kvp.Value.SelectedIndices;
							List<int> clonedIndices = ((selectedIndices != null) ? selectedIndices.ToList<int>() : null) ?? new List<int>();
							clonedDict[kvp.Key] = new DetailedFilterMenuState(clonedIndices, kvp.Value.IsActive);
						}
						detailedFilterState = new DetailedFilterLineState
						{
							State = new DetailedFilterState
							{
								MenuStateDict = clonedDict
							}
						};
					}
					result.Add(new LineState
					{
						IsActive = ls.IsActive,
						Type = ls.Type,
						ToggleGroupState = ls.ToggleGroupState,
						DetailedFilterState = detailedFilterState
					});
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x1700113E RID: 4414
		// (get) Token: 0x0600A54B RID: 42315 RVA: 0x004D0DE0 File Offset: 0x004CEFE0
		SortAndFilterState ISortAndFilterController.SortAndFilterState
		{
			get
			{
				return this._sortAndFilterState;
			}
		}

		// Token: 0x0600A54C RID: 42316 RVA: 0x004D0DE8 File Offset: 0x004CEFE8
		void ISortAndFilterController.SetSortState(SortStateData sortData)
		{
			this.SetSortState(sortData);
		}

		// Token: 0x0600A54D RID: 42317 RVA: 0x004D0DF3 File Offset: 0x004CEFF3
		void ISortAndFilterController.RegisterTableHead(ITableHead tableHead, short[] columnSortIds)
		{
			this.RegisterTableHead(tableHead, columnSortIds);
		}

		// Token: 0x0600A54E RID: 42318 RVA: 0x004D0E00 File Offset: 0x004CF000
		private bool ShouldSortIdParticipate(short sortId, HashSet<short> displayingSortIds)
		{
			bool flag = this._sortIdsUsedByTableHead.Contains(sortId);
			return flag || displayingSortIds.Count == 0 || displayingSortIds.Contains(sortId);
		}

		// Token: 0x0600A54F RID: 42319 RVA: 0x004D0E38 File Offset: 0x004CF038
		public void RegisterTableHead(ITableHead tableHead, short[] columnSortIds)
		{
			bool flag = tableHead == null;
			if (!flag)
			{
				this._cachedCurrentTableHeadIds = columnSortIds;
				this.AddSortIdByHead();
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

		// Token: 0x0600A550 RID: 42320 RVA: 0x004D0ECC File Offset: 0x004CF0CC
		public void UnregisterTableHead(ITableHead tableHead)
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

		// Token: 0x0600A551 RID: 42321 RVA: 0x004D0F00 File Offset: 0x004CF100
		private void SyncSortStateToTableHeads()
		{
			foreach (ITableHead tableHead in this._registeredTableHeads)
			{
				bool flag = tableHead != null;
				if (flag)
				{
					tableHead.SyncSortStateFromController();
				}
			}
		}

		// Token: 0x0600A552 RID: 42322 RVA: 0x004D0F64 File Offset: 0x004CF164
		public void AddSortIdByHead()
		{
			SortAndFilter sortAndFilter2 = this._sortAndFilter as SortAndFilter;
			bool flag = sortAndFilter2 != null;
			if (flag)
			{
				sortAndFilter2.SetupAdditionalSortIds(this._cachedCurrentTableHeadIds);
			}
		}

		// Token: 0x0400827F RID: 33407
		private readonly List<FilterLineBase<TData>> _filterLines = new List<FilterLineBase<TData>>();

		// Token: 0x04008280 RID: 33408
		private readonly ISortAndFilterView _sortAndFilter;

		// Token: 0x04008281 RID: 33409
		private readonly MonoBehaviour _sortAndFilterBehaviour;

		// Token: 0x04008282 RID: 33410
		private Action _onStateChanged;

		// Token: 0x04008283 RID: 33411
		private SortAndFilterState _sortAndFilterState;

		// Token: 0x04008284 RID: 33412
		private IEnumerable<TData> _cachedDataList;

		// Token: 0x04008285 RID: 33413
		protected SortController<TData> SortController;

		// Token: 0x04008286 RID: 33414
		private readonly List<ITableHead> _registeredTableHeads = new List<ITableHead>();

		// Token: 0x04008287 RID: 33415
		private readonly HashSet<short> _sortIdsUsedByTableHead = new HashSet<short>();

		// Token: 0x04008288 RID: 33416
		private short[] _cachedCurrentTableHeadIds;

		// Token: 0x04008289 RID: 33417
		private readonly LanguageKey _panelTitleKey;

		// Token: 0x0400828A RID: 33418
		private bool _isInit;

		// Token: 0x0400828B RID: 33419
		private List<LineState> _savedFilterLineStates;
	}
}
