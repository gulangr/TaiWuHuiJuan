using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Components.Switch;
using TMPro;
using UnityEngine;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CA8 RID: 3240
	public class SortAndFilter : MonoBehaviour, ISortAndFilterView, IFilteredCountView
	{
		// Token: 0x17001133 RID: 4403
		// (get) Token: 0x0600A4A7 RID: 42151 RVA: 0x004CCBF2 File Offset: 0x004CADF2
		// (set) Token: 0x0600A4A8 RID: 42152 RVA: 0x004CCBFA File Offset: 0x004CADFA
		public SortAndFilterConfig Config { get; private set; }

		// Token: 0x17001134 RID: 4404
		// (get) Token: 0x0600A4A9 RID: 42153 RVA: 0x004CCC03 File Offset: 0x004CAE03
		public bool ShouldShowPanelHeader
		{
			get
			{
				return this._firstToggleGroupIndex < 0 || !this.GetFirstToggleGroupState().IsAll;
			}
		}

		// Token: 0x17001135 RID: 4405
		// (get) Token: 0x0600A4AA RID: 42154 RVA: 0x004CCC1F File Offset: 0x004CAE1F
		public ISortUi SortUi
		{
			get
			{
				return this.sortButtonGroup;
			}
		}

		// Token: 0x17001136 RID: 4406
		// (get) Token: 0x0600A4AB RID: 42155 RVA: 0x004CCC27 File Offset: 0x004CAE27
		public IReadOnlyList<SortAndFilter.SectionViewData> Sections
		{
			get
			{
				return this._sections;
			}
		}

		// Token: 0x0600A4AC RID: 42156 RVA: 0x004CCC2F File Offset: 0x004CAE2F
		private void OnDisable()
		{
			this.CloseFilterPanel();
		}

		// Token: 0x17001137 RID: 4407
		// (get) Token: 0x0600A4AD RID: 42157 RVA: 0x004CCC3C File Offset: 0x004CAE3C
		public bool ShowFilterEntryButton
		{
			get
			{
				bool flag = this._firstToggleGroupIndex < 0;
				bool result;
				if (flag)
				{
					result = (this._sections.Count > 0);
				}
				else
				{
					ToggleKey state = this.GetFirstToggleGroupState();
					bool flag2 = !state.IsAll;
					result = (flag2 || this._sections.Count > 0);
				}
				return result;
			}
		}

		// Token: 0x0600A4AE RID: 42158 RVA: 0x004CCC94 File Offset: 0x004CAE94
		public SortAndFilterState GetStateFromUI()
		{
			List<LineState> lineStates = new List<LineState>();
			ISortUi sortUi = this.SortUi;
			SortStateData sortStateData;
			if ((sortStateData = ((sortUi != null) ? sortUi.GetSortData() : null)) == null)
			{
				(sortStateData = new SortStateData()).ItemStates = new List<SortItemState>();
			}
			SortStateData sortData = sortStateData;
			bool flag = this.Config == null;
			SortAndFilterState result;
			if (flag)
			{
				result = new SortAndFilterState
				{
					LineStates = lineStates,
					SortData = sortData
				};
			}
			else
			{
				for (int i = 0; i < this.Config.LineConfigs.Count; i++)
				{
					lineStates.Add(this.CreateLineStateFromCache(i));
				}
				result = new SortAndFilterState
				{
					LineStates = lineStates,
					SortData = sortData
				};
			}
			return result;
		}

		// Token: 0x0600A4AF RID: 42159 RVA: 0x004CCD4C File Offset: 0x004CAF4C
		public void Setup(SortAndFilterConfig config)
		{
			this.ResetRuntimeState();
			this.Config = config;
			this.sortButtonGroup.Setup(config.SortConfig.UiConfig, config.OnSortChanged, null, null);
			this.SetupFirstToggleGroup();
			this.RefreshDynamicConfigs();
			this.RefreshSectionViewData();
			this.filterPanel.Setup(this);
			this.BindEntryToggle();
			this.BindSummaryClearButton();
			this.RefreshSummary();
			this.RefreshEntryVisibility();
			this.CloseFilterPanel();
			this.UpdateLineActive(0);
		}

		// Token: 0x0600A4B0 RID: 42160 RVA: 0x004CCDD8 File Offset: 0x004CAFD8
		private void BindEntryToggle()
		{
			if (this.entryToggle == null)
			{
				this.entryToggle = base.GetComponentInChildren<SwitchToggleSmall>(true);
			}
			bool flag = this.entryToggle == null;
			if (!flag)
			{
				this.entryToggle.onValueChanged.RemoveAllListeners();
				this.entryToggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					this.entryToggle.OnClick(isOn);
					this.OnEntryToggleValueChanged(isOn);
				});
				this.RefreshEntryVisualState();
			}
		}

		// Token: 0x0600A4B1 RID: 42161 RVA: 0x004CCE44 File Offset: 0x004CB044
		private void OnEntryToggleValueChanged(bool isOn)
		{
			bool isSyncingEntryToggle = this._isSyncingEntryToggle;
			if (!isSyncingEntryToggle)
			{
				if (isOn)
				{
					this.OpenFilterPanel();
				}
				else
				{
					this.CloseFilterPanel();
				}
			}
		}

		// Token: 0x0600A4B2 RID: 42162 RVA: 0x004CCE74 File Offset: 0x004CB074
		private void SyncEntryToggleWithPanel()
		{
			bool flag = this.entryToggle == null;
			if (!flag)
			{
				bool isPanelOpen = this.GetPanelRoot() != null && this.GetPanelRoot().gameObject.activeSelf;
				this._isSyncingEntryToggle = true;
				this.entryToggle.SetWithoutNotify(isPanelOpen);
				this._isSyncingEntryToggle = false;
			}
		}

		// Token: 0x0600A4B3 RID: 42163 RVA: 0x004CCED4 File Offset: 0x004CB0D4
		public bool IsPointOverEntryButton(Vector2 screenPoint)
		{
			bool flag = this.entryToggle == null || !this.entryToggle.gameObject.activeInHierarchy;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				RectTransform rectTransform = this.entryToggle.transform as RectTransform;
				bool flag2 = rectTransform == null;
				result = (!flag2 && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint, UIManager.Instance.UiCamera));
			}
			return result;
		}

		// Token: 0x0600A4B4 RID: 42164 RVA: 0x004CCF43 File Offset: 0x004CB143
		private void BindSummaryClearButton()
		{
			this.clearSummaryButton.ClearAndAddListener(new Action(this.OnSummaryClearButtonClicked));
		}

		// Token: 0x0600A4B5 RID: 42165 RVA: 0x004CCF60 File Offset: 0x004CB160
		public void RefreshDynamicConfigs()
		{
			this._dynamicConfigs.Clear();
			SortAndFilterConfig config = this.Config;
			bool flag = ((config != null) ? config.LineConfigs : null) == null || this.Config.DynamicConfigProvider == null;
			if (!flag)
			{
				foreach (LineConfig lineConfig in this.Config.LineConfigs)
				{
					DynamicLineConfig dynamicConfig = this.Config.DynamicConfigProvider(lineConfig.Id);
					bool flag2 = dynamicConfig != null;
					if (flag2)
					{
						this._dynamicConfigs[lineConfig.Id] = dynamicConfig;
					}
				}
			}
		}

		// Token: 0x0600A4B6 RID: 42166 RVA: 0x004CD024 File Offset: 0x004CB224
		public void RebuildForLanguageChange(SortAndFilterConfig config)
		{
			this.ResetRuntimeState();
			this.Setup(config);
		}

		// Token: 0x0600A4B7 RID: 42167 RVA: 0x004CD038 File Offset: 0x004CB238
		public void UpdateLineActive(int changedLineIndex)
		{
			SortAndFilterConfig config = this.Config;
			bool flag = ((config != null) ? config.LineConfigs : null) == null;
			if (!flag)
			{
				SortAndFilterState state = this.GetStateFromUI();
				this._activeCache.Clear();
				for (int lineIndex = 0; lineIndex < this.Config.LineConfigs.Count; lineIndex++)
				{
					this.CalcLineActive(lineIndex, state);
				}
				this.RefreshSectionViewData();
				this.RefreshSummary();
				this.RefreshEntryVisibility();
				this.filterPanel.Refresh();
			}
		}

		// Token: 0x0600A4B8 RID: 42168 RVA: 0x004CD0BD File Offset: 0x004CB2BD
		public void SetEntryButtonForceHidden(bool forceHidden)
		{
			this._forceHideEntry = forceHidden;
			this.RefreshEntryVisibility();
		}

		// Token: 0x0600A4B9 RID: 42169 RVA: 0x004CD0D0 File Offset: 0x004CB2D0
		private void RefreshEntryVisibility()
		{
			bool visible = !this._forceHideEntry && this.ShowFilterEntryButton;
			bool flag = this.entryToggle != null;
			if (flag)
			{
				this.entryToggle.gameObject.SetActive(visible);
			}
			this.RefreshEntryVisualState();
		}

		// Token: 0x0600A4BA RID: 42170 RVA: 0x004CD119 File Offset: 0x004CB319
		public void OpenFilterPanel()
		{
			this.GetPanelRoot().gameObject.SetActive(true);
			this.RefreshEntryVisualState();
		}

		// Token: 0x0600A4BB RID: 42171 RVA: 0x004CD135 File Offset: 0x004CB335
		public void CloseFilterPanel()
		{
			this.GetPanelRoot().gameObject.SetActive(false);
			this.RefreshEntryVisualState();
		}

		// Token: 0x0600A4BC RID: 42172 RVA: 0x004CD151 File Offset: 0x004CB351
		public void RefreshSortDisplay<TData>(List<FilterLineBase<TData>> activeSort, List<LineState> lineStates)
		{
			this.sortButtonGroup.RefreshDisplayOptions<TData>(activeSort, lineStates);
			this.ApplyOutsideSortVisibility();
			this.RefreshSectionViewData();
			this.RefreshSummary();
			this.filterPanel.RefreshHeader();
		}

		// Token: 0x0600A4BD RID: 42173 RVA: 0x004CD183 File Offset: 0x004CB383
		public void ClearAllFilter()
		{
			this._selectedIndices.Clear();
			SortAndFilterConfig config = this.Config;
			if (config != null)
			{
				Action<int> onFilterChanged = config.OnFilterChanged;
				if (onFilterChanged != null)
				{
					onFilterChanged(-1);
				}
			}
			this.RefreshSectionsSummaryAndPanel();
		}

		// Token: 0x0600A4BE RID: 42174 RVA: 0x004CD1B8 File Offset: 0x004CB3B8
		public void ApplyFilterLineStates(List<LineState> lineStates)
		{
			SortAndFilterConfig config = this.Config;
			bool flag = ((config != null) ? config.LineConfigs : null) == null || lineStates == null;
			if (!flag)
			{
				for (int i = 0; i < Math.Min(this.Config.LineConfigs.Count, lineStates.Count); i++)
				{
					LineState ls = lineStates[i];
					bool flag2 = ls.Type == ESortAndFilterOneLineType.ToggleGroup && i == this._firstToggleGroupIndex && this.firstToggleGroupLine != null;
					if (flag2)
					{
						this.firstToggleGroupLine.SetToggleIsOnWithoutNotify(ls.ToggleGroupState);
					}
				}
				this._selectedIndices.Clear();
				for (int j = 0; j < Math.Min(this.Config.LineConfigs.Count, lineStates.Count); j++)
				{
					LineState ls2 = lineStates[j];
					LineConfig lc = this.Config.LineConfigs[j];
					bool flag3 = ls2.DetailedFilterState == null || ls2.DetailedFilterState.State.MenuStateDict == null;
					if (!flag3)
					{
						foreach (KeyValuePair<int, DetailedFilterMenuState> kvp in ls2.DetailedFilterState.State.MenuStateDict)
						{
							int key = SortAndFilter.GetSelectionKey(lc.Id, kvp.Key);
							Dictionary<int, List<int>> selectedIndices = this._selectedIndices;
							int key2 = key;
							IReadOnlyCollection<int> selectedIndices2 = kvp.Value.SelectedIndices;
							selectedIndices[key2] = (((selectedIndices2 != null) ? selectedIndices2.ToList<int>() : null) ?? new List<int>());
						}
					}
				}
				this.UpdateLineActive(0);
			}
		}

		// Token: 0x0600A4BF RID: 42175 RVA: 0x004CD380 File Offset: 0x004CB580
		public void SetToggleVisible(ToggleIdIndex toggleIndex, bool isVisible)
		{
			bool flag = toggleIndex.LineId == this.GetFirstToggleGroupLineId() && this.firstToggleGroupLine != null;
			if (flag)
			{
				this.firstToggleGroupLine.SetToggleVisible(toggleIndex.ToggleKey, isVisible);
			}
			this.RefreshSectionsSummaryAndPanel();
		}

		// Token: 0x0600A4C0 RID: 42176 RVA: 0x004CD3CC File Offset: 0x004CB5CC
		public void SetToggleIsOn(ToggleIdIndex toggleIndex, bool isOn)
		{
			bool flag = toggleIndex.LineId == this.GetFirstToggleGroupLineId() && this.firstToggleGroupLine != null;
			if (flag)
			{
				this.firstToggleGroupLine.SetToggleIsOn(toggleIndex.ToggleKey, isOn);
			}
			this.RefreshSectionsSummaryAndPanel();
		}

		// Token: 0x0600A4C1 RID: 42177 RVA: 0x004CD418 File Offset: 0x004CB618
		public void SetToggleIsOnWithoutNotify(int lineId, int toggleIndex)
		{
			bool flag = lineId == this.GetFirstToggleGroupLineId() && this.firstToggleGroupLine != null;
			if (flag)
			{
				ToggleKey toggleKey = (toggleIndex < 0) ? ToggleKey.AllKey : new ToggleKey
				{
					Index = toggleIndex,
					IsAll = false
				};
				this.firstToggleGroupLine.SetToggleIsOnWithoutNotify(toggleKey);
				this.UpdateLineActive(this._firstToggleGroupIndex);
			}
			this.RefreshSectionsSummaryAndPanel();
		}

		// Token: 0x0600A4C2 RID: 42178 RVA: 0x004CD48C File Offset: 0x004CB68C
		public void SetToggleInteractable(ToggleIdIndex toggleIndex, bool interactable)
		{
			bool flag = toggleIndex.LineId == this.GetFirstToggleGroupLineId() && this.firstToggleGroupLine != null;
			if (flag)
			{
				this.firstToggleGroupLine.SetToggleInteractable(toggleIndex.ToggleKey, interactable);
			}
			this.RefreshSectionsSummaryAndPanel();
		}

		// Token: 0x0600A4C3 RID: 42179 RVA: 0x004CD4D8 File Offset: 0x004CB6D8
		public void SetDropdownMenuVisible(int lineId, int menuId, bool isVisible)
		{
			if (isVisible)
			{
				HashSet<int> invisibleSet;
				bool flag = this._outsideMenuInvisibleDict.TryGetValue(lineId, out invisibleSet) && invisibleSet != null;
				if (flag)
				{
					invisibleSet.Remove(menuId);
					bool flag2 = invisibleSet.Count == 0;
					if (flag2)
					{
						this._outsideMenuInvisibleDict.Remove(lineId);
					}
				}
			}
			else
			{
				HashSet<int> invisibleSet2;
				bool flag3 = !this._outsideMenuInvisibleDict.TryGetValue(lineId, out invisibleSet2);
				if (flag3)
				{
					invisibleSet2 = new HashSet<int>();
					this._outsideMenuInvisibleDict[lineId] = invisibleSet2;
				}
				invisibleSet2.Add(menuId);
			}
			this.UpdateLineActive(this.GetIndexFromId(lineId));
		}

		// Token: 0x0600A4C4 RID: 42180 RVA: 0x004CD578 File Offset: 0x004CB778
		public void SetVisibleDropdownMenus(int lineId, IEnumerable<int> visibleMenuIds)
		{
			bool flag = !this.CanUseVisibleDropdownMenusApi();
			if (!flag)
			{
				SortAndFilterConfig config = this.Config;
				bool flag2 = ((config != null) ? config.LineConfigs : null) == null;
				if (!flag2)
				{
					int lineIndex = -1;
					for (int i = 0; i < this.Config.LineConfigs.Count; i++)
					{
						bool flag3 = this.Config.LineConfigs[i].Id == lineId;
						if (flag3)
						{
							lineIndex = i;
							break;
						}
					}
					bool flag4 = lineIndex < 0;
					if (!flag4)
					{
						LineConfig lineConfig = this.Config.LineConfigs[lineIndex];
						DetailedFilterLineConfig detailedFilterLineConfig = lineConfig.DetailedFilterLineConfig;
						List<DetailedFilterMenuConfig> menuConfigs = (detailedFilterLineConfig != null) ? detailedFilterLineConfig.Config.MenuConfigs : null;
						bool flag5 = menuConfigs == null;
						if (!flag5)
						{
							HashSet<int> visibleSet = (visibleMenuIds == null) ? new HashSet<int>() : new HashSet<int>(visibleMenuIds);
							HashSet<int> invisibleSet = new HashSet<int>();
							foreach (DetailedFilterMenuConfig menuConfig in menuConfigs)
							{
								bool flag6 = !visibleSet.Contains(menuConfig.Id);
								if (flag6)
								{
									invisibleSet.Add(menuConfig.Id);
								}
							}
							bool flag7 = invisibleSet.Count == 0;
							if (flag7)
							{
								this._outsideMenuInvisibleDict.Remove(lineId);
							}
							else
							{
								this._outsideMenuInvisibleDict[lineId] = invisibleSet;
							}
							this.UpdateLineActive(lineIndex);
						}
					}
				}
			}
		}

		// Token: 0x0600A4C5 RID: 42181 RVA: 0x004CD704 File Offset: 0x004CB904
		public void SetVisibleSortIds(IEnumerable<short> visibleSortIds)
		{
			this._outsideVisibleSortIds = ((visibleSortIds == null) ? new HashSet<short>() : new HashSet<short>(visibleSortIds));
			this.ApplyOutsideSortVisibility();
		}

		// Token: 0x0600A4C6 RID: 42182 RVA: 0x004CD724 File Offset: 0x004CB924
		public void SetupAdditionalSortIds(IEnumerable<short> sortIdsUsedByTableHead)
		{
			this.sortButtonGroup.Setup(sortIdsUsedByTableHead);
		}

		// Token: 0x0600A4C7 RID: 42183 RVA: 0x004CD734 File Offset: 0x004CB934
		public void SetDropdownOptionInteractable(int lineId, int menuId, int optionIndex, bool interactable, string disabledTooltip)
		{
			this._optionInteractableState[new ValueTuple<int, int, int>(lineId, menuId, optionIndex)] = new SortAndFilter.OptionInteractableState
			{
				Interactable = interactable,
				DisabledTooltip = disabledTooltip
			};
			this.filterPanel.SetOptionInteractable(lineId, menuId, optionIndex, interactable, disabledTooltip);
		}

		// Token: 0x0600A4C8 RID: 42184 RVA: 0x004CD784 File Offset: 0x004CB984
		public void SetDropdownOption(int lineId, int menuId, int optionIndex)
		{
			int key = SortAndFilter.GetSelectionKey(lineId, menuId);
			Dictionary<int, List<int>> selectedIndices = this._selectedIndices;
			int key2 = key;
			List<int> value;
			if (optionIndex < 0)
			{
				value = new List<int>();
			}
			else
			{
				(value = new List<int>()).Add(optionIndex);
			}
			selectedIndices[key2] = value;
			SortAndFilterConfig config = this.Config;
			if (config != null)
			{
				Action<int> onFilterChanged = config.OnFilterChanged;
				if (onFilterChanged != null)
				{
					onFilterChanged(lineId);
				}
			}
			this.RefreshSectionsSummaryAndPanel();
		}

		// Token: 0x0600A4C9 RID: 42185 RVA: 0x004CD7E4 File Offset: 0x004CB9E4
		public string GetFilteredCountText()
		{
			return (this._filteredCount >= 0) ? this._filteredCount.ToString() : string.Empty;
		}

		// Token: 0x0600A4CA RID: 42186 RVA: 0x004CD811 File Offset: 0x004CBA11
		void IFilteredCountView.SetFilteredCount(int count)
		{
			this._filteredCount = Mathf.Max(0, count);
			this.filterPanel.RefreshHeader();
		}

		// Token: 0x0600A4CB RID: 42187 RVA: 0x004CD82D File Offset: 0x004CBA2D
		void IFilteredCountView.SetFilterOptionCounts(IReadOnlyList<OptionCountData> optionCounts)
		{
			this.filterPanel.RefreshFilterOptionCounts(optionCounts);
		}

		// Token: 0x0600A4CC RID: 42188 RVA: 0x004CD840 File Offset: 0x004CBA40
		public bool TryGetOptionInteractableState(int lineId, int menuId, int optionIndex, out bool interactable, out string disabledTooltip)
		{
			SortAndFilter.OptionInteractableState state;
			bool flag = this._optionInteractableState.TryGetValue(new ValueTuple<int, int, int>(lineId, menuId, optionIndex), out state);
			bool result;
			if (flag)
			{
				interactable = state.Interactable;
				disabledTooltip = state.DisabledTooltip;
				result = true;
			}
			else
			{
				interactable = true;
				disabledTooltip = string.Empty;
				result = false;
			}
			return result;
		}

		// Token: 0x0600A4CD RID: 42189 RVA: 0x004CD890 File Offset: 0x004CBA90
		public string GetPanelTitle()
		{
			bool flag = !string.IsNullOrWhiteSpace(this.customFilterPanelLanguageKey);
			string result;
			if (flag)
			{
				result = LocalStringManager.Get(this.customFilterPanelLanguageKey);
			}
			else
			{
				SortAndFilterConfig config = this.Config;
				bool flag2 = ((config != null) ? config.LineConfigs : null) == null || this.Config.LineConfigs.Count == 0;
				if (flag2)
				{
					result = this.GetDefaultPanelTitle();
				}
				else
				{
					LineConfig firstLine = this.Config.LineConfigs[0];
					bool flag3 = firstLine.Type == ESortAndFilterOneLineType.ToggleGroup && firstLine.ToggleGroupLineConfig != null && firstLine.ToggleGroupLineConfig.Config.FilterToggleConfigs != null && firstLine.ToggleGroupLineConfig.Config.FilterToggleConfigs.Count > 0;
					if (flag3)
					{
						List<FilterToggleConfig> toggleConfigs = firstLine.ToggleGroupLineConfig.Config.FilterToggleConfigs;
						ToggleKey toggleState = this.GetFirstToggleGroupState();
						bool isAll = toggleState.IsAll;
						if (isAll)
						{
							FilterToggleConfig filterToggleConfig = toggleConfigs[0];
							result = filterToggleConfig.TipContent.GetString();
						}
						else
						{
							int index = toggleState.Index;
							bool flag4 = index >= 0 && index < toggleConfigs.Count;
							if (flag4)
							{
								FilterToggleConfig filterToggleConfig = toggleConfigs[index];
								result = filterToggleConfig.TipContent.GetString();
							}
							else
							{
								FilterToggleConfig filterToggleConfig = toggleConfigs[0];
								result = filterToggleConfig.TipContent.GetString();
							}
						}
					}
					else
					{
						result = this.GetDefaultPanelTitle();
					}
				}
			}
			return result;
		}

		// Token: 0x0600A4CE RID: 42190 RVA: 0x004CD9F7 File Offset: 0x004CBBF7
		public void SetPanelTitleKey(LanguageKey titleKey)
		{
			this._panelTitleKey = titleKey;
			this.filterPanel.RefreshHeader();
		}

		// Token: 0x0600A4CF RID: 42191 RVA: 0x004CDA10 File Offset: 0x004CBC10
		public bool ShouldShowMenu(int lineIndex, int menuId)
		{
			HashSet<int> invisibleSet;
			bool flag = !this._outsideMenuInvisibleDict.TryGetValue(this.GetIdFromIndex(lineIndex), out invisibleSet);
			return flag || !invisibleSet.Contains(menuId);
		}

		// Token: 0x0600A4D0 RID: 42192 RVA: 0x004CDA4C File Offset: 0x004CBC4C
		public DetailedFilterMenuConfig? GetMenuConfig(int lineIndex, int menuId)
		{
			SortAndFilterConfig config = this.Config;
			bool flag = ((config != null) ? config.LineConfigs : null) == null || lineIndex < 0 || lineIndex >= this.Config.LineConfigs.Count;
			DetailedFilterMenuConfig? result;
			if (flag)
			{
				result = null;
			}
			else
			{
				LineConfig lineConfig = this.Config.LineConfigs[lineIndex];
				DetailedFilterLineConfig detailedFilterLineConfig = lineConfig.DetailedFilterLineConfig;
				bool flag2 = ((detailedFilterLineConfig != null) ? detailedFilterLineConfig.Config.MenuConfigs : null) == null;
				if (flag2)
				{
					result = null;
				}
				else
				{
					foreach (DetailedFilterMenuConfig menuConfig in lineConfig.DetailedFilterLineConfig.Config.MenuConfigs)
					{
						bool flag3 = menuConfig.Id == menuId;
						if (flag3)
						{
							DynamicLineConfig dynamicConfig;
							FilterDropdownConfig newConfig;
							bool flag4;
							if (this._dynamicConfigs.TryGetValue(lineConfig.Id, out dynamicConfig))
							{
								DynamicDetailedFilterLineConfig detailedFilterLineConfig2 = dynamicConfig.DetailedFilterLineConfig;
								if (((detailedFilterLineConfig2 != null) ? detailedFilterLineConfig2.ChangedMenuConfigs : null) != null)
								{
									flag4 = dynamicConfig.DetailedFilterLineConfig.ChangedMenuConfigs.TryGetValue(menuId, out newConfig);
									goto IL_102;
								}
							}
							flag4 = false;
							IL_102:
							bool flag5 = flag4;
							if (flag5)
							{
								return new DetailedFilterMenuConfig?(new DetailedFilterMenuConfig
								{
									Id = menuId,
									DropdownConfig = newConfig,
									DropdownContext = menuConfig.DropdownContext
								});
							}
							return new DetailedFilterMenuConfig?(menuConfig);
						}
					}
					result = null;
				}
			}
			return result;
		}

		// Token: 0x0600A4D1 RID: 42193 RVA: 0x004CDBD8 File Offset: 0x004CBDD8
		public int GetInitialSectionState(int lineId, int menuId)
		{
			int key = SortAndFilter.GetSelectionKey(lineId, menuId);
			List<int> indices;
			bool flag = this._selectedIndices.TryGetValue(key, out indices) && indices.Count > 0;
			int result;
			if (flag)
			{
				result = indices[0];
			}
			else
			{
				result = -1;
			}
			return result;
		}

		// Token: 0x0600A4D2 RID: 42194 RVA: 0x004CDC20 File Offset: 0x004CBE20
		public void OnSectionSelectionChanged(int lineId, int menuId, int selectedIndex)
		{
			int key = SortAndFilter.GetSelectionKey(lineId, menuId);
			Dictionary<int, List<int>> selectedIndices = this._selectedIndices;
			int key2 = key;
			List<int> value;
			if (selectedIndex < 0)
			{
				value = new List<int>();
			}
			else
			{
				(value = new List<int>()).Add(selectedIndex);
			}
			selectedIndices[key2] = value;
			SortAndFilterConfig config = this.Config;
			if (config != null)
			{
				Action<int> onFilterChanged = config.OnFilterChanged;
				if (onFilterChanged != null)
				{
					onFilterChanged(lineId);
				}
			}
		}

		// Token: 0x0600A4D3 RID: 42195 RVA: 0x004CDC7C File Offset: 0x004CBE7C
		public List<SortAndFilter.SummaryItemData> GetSummaryItems()
		{
			List<SortAndFilter.SummaryItemData> result = new List<SortAndFilter.SummaryItemData>();
			SortAndFilterConfig config = this.Config;
			bool flag = ((config != null) ? config.LineConfigs : null) == null;
			List<SortAndFilter.SummaryItemData> result2;
			if (flag)
			{
				result2 = result;
			}
			else
			{
				foreach (SortAndFilter.SectionViewData section in this._sections)
				{
					bool flag2 = !section.IsActive;
					if (!flag2)
					{
						int key = SortAndFilter.GetSelectionKey(section.LineId, section.MenuId);
						List<int> indices;
						bool flag3 = !this._selectedIndices.TryGetValue(key, out indices) || indices.Count == 0;
						if (!flag3)
						{
							LineConfig lineConfig = this.Config.LineConfigs[section.LineIndex];
							object obj;
							if (lineConfig == null)
							{
								obj = null;
							}
							else
							{
								DetailedFilterLineConfig detailedFilterLineConfig = lineConfig.DetailedFilterLineConfig;
								obj = ((detailedFilterLineConfig != null) ? detailedFilterLineConfig.Config.MenuConfigs : null);
							}
							bool flag4 = obj == null;
							if (!flag4)
							{
								foreach (DetailedFilterMenuConfig menuConfig in lineConfig.DetailedFilterLineConfig.Config.MenuConfigs)
								{
									bool flag5 = menuConfig.Id != section.MenuId;
									if (!flag5)
									{
										DetailedFilterMenuConfig actualMenuConfig = this.GetMenuConfig(section.LineIndex, menuConfig.Id) ?? menuConfig;
										List<FilterDropdownItemConfig> itemConfigs = actualMenuConfig.DropdownConfig.ItemConfigs;
										bool flag6 = itemConfigs == null;
										if (!flag6)
										{
											foreach (int optionIndex in indices)
											{
												bool flag7 = optionIndex < 0 || optionIndex >= itemConfigs.Count;
												if (!flag7)
												{
													result.Add(new SortAndFilter.SummaryItemData
													{
														LineId = section.LineId,
														MenuId = section.MenuId,
														OptionIndex = optionIndex,
														Text = itemConfigs[optionIndex].Text.GetString()
													});
												}
											}
											break;
										}
									}
								}
							}
						}
					}
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x0600A4D4 RID: 42196 RVA: 0x004CDF2C File Offset: 0x004CC12C
		public void ClearFilterItem(int lineId, int menuId)
		{
			int key = SortAndFilter.GetSelectionKey(lineId, menuId);
			this._selectedIndices.Remove(key);
			SortAndFilterConfig config = this.Config;
			if (config != null)
			{
				Action<int> onFilterChanged = config.OnFilterChanged;
				if (onFilterChanged != null)
				{
					onFilterChanged(lineId);
				}
			}
			this.UpdateLineActive(this.GetIndexFromId(lineId));
			this.filterPanel.Refresh();
		}

		// Token: 0x0600A4D5 RID: 42197 RVA: 0x004CDF88 File Offset: 0x004CC188
		private void SetupFirstToggleGroup()
		{
			this._firstToggleGroupIndex = -1;
			SortAndFilterConfig config = this.Config;
			bool flag = ((config != null) ? config.LineConfigs : null) == null || this.Config.LineConfigs.Count == 0;
			if (flag)
			{
				this.firstToggleGroupLine.SetActive(false);
			}
			else
			{
				LineConfig firstLine = this.Config.LineConfigs[0];
				bool flag2 = firstLine.Type > ESortAndFilterOneLineType.ToggleGroup;
				if (flag2)
				{
					this.firstToggleGroupLine.SetActive(false);
				}
				else
				{
					this._firstToggleGroupIndex = 0;
					this.firstToggleGroupLine.Setup(firstLine.ToggleGroupLineConfig.Config, delegate
					{
						this.OnFirstToggleGroupChanged();
					});
					this.firstToggleGroupLine.SetActive(true);
				}
			}
		}

		// Token: 0x0600A4D6 RID: 42198 RVA: 0x004CE041 File Offset: 0x004CC241
		public void SetUpToggleGroupHotKey(UIElement uiElement, sbyte level)
		{
			ToggleGroupHotkeyController.Set(uiElement, this.firstToggleGroupLine.GetComponent<CToggleGroup>(), level, null);
		}

		// Token: 0x0600A4D7 RID: 42199 RVA: 0x004CE058 File Offset: 0x004CC258
		private void DestroyFirstToggleGroup()
		{
			this.firstToggleGroupLine.SetActive(false);
			this._firstToggleGroupIndex = -1;
		}

		// Token: 0x0600A4D8 RID: 42200 RVA: 0x004CE070 File Offset: 0x004CC270
		private void OnFirstToggleGroupChanged()
		{
			this._activeCache[0] = true;
			SortAndFilterConfig config = this.Config;
			if (config != null)
			{
				Action<int> onFilterChanged = config.OnFilterChanged;
				if (onFilterChanged != null)
				{
					onFilterChanged(this.GetFirstToggleGroupLineId());
				}
			}
			this.UpdateLineActive(0);
			this.filterPanel.Refresh();
		}

		// Token: 0x0600A4D9 RID: 42201 RVA: 0x004CE0C4 File Offset: 0x004CC2C4
		private ToggleKey GetFirstToggleGroupState()
		{
			bool flag = this.firstToggleGroupLine == null;
			ToggleKey result;
			if (flag)
			{
				result = ToggleKey.AllKey;
			}
			else
			{
				LineState lineState = this.firstToggleGroupLine.GetLineState();
				result = lineState.ToggleGroupState;
			}
			return result;
		}

		// Token: 0x0600A4DA RID: 42202 RVA: 0x004CE100 File Offset: 0x004CC300
		private int GetFirstToggleGroupLineId()
		{
			bool flag;
			if (this._firstToggleGroupIndex >= 0)
			{
				SortAndFilterConfig config = this.Config;
				flag = (((config != null) ? config.LineConfigs : null) == null);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			int result;
			if (flag2)
			{
				result = -1;
			}
			else
			{
				result = this.Config.LineConfigs[this._firstToggleGroupIndex].Id;
			}
			return result;
		}

		// Token: 0x0600A4DB RID: 42203 RVA: 0x004CE158 File Offset: 0x004CC358
		private static int GetSelectionKey(int lineId, int menuId)
		{
			return lineId * 1000 + menuId;
		}

		// Token: 0x0600A4DC RID: 42204 RVA: 0x004CE174 File Offset: 0x004CC374
		private LineState CreateLineStateFromCache(int lineIndex)
		{
			LineConfig lineConfig = this.Config.LineConfigs[lineIndex];
			bool active;
			bool isActive = this._activeCache.TryGetValue(lineIndex, out active) && active;
			bool flag = lineConfig.Type == ESortAndFilterOneLineType.ToggleGroup;
			LineState result;
			if (flag)
			{
				ToggleKey toggleState = (lineIndex == this._firstToggleGroupIndex && this.firstToggleGroupLine != null) ? this.GetFirstToggleGroupState() : new ToggleKey
				{
					IsAll = true,
					Index = -1
				};
				result = new LineState
				{
					IsActive = isActive,
					Type = lineConfig.Type,
					ToggleGroupState = toggleState,
					DetailedFilterState = new DetailedFilterLineState
					{
						State = new DetailedFilterState
						{
							MenuStateDict = new Dictionary<int, DetailedFilterMenuState>()
						}
					}
				};
			}
			else
			{
				ToggleKey toggleGroupState = (lineConfig.Type == ESortAndFilterOneLineType.SingleSelectFilter) ? this.GetSingleSelectLineToggleState(lineConfig) : new ToggleKey
				{
					IsAll = true,
					Index = -1
				};
				DetailedFilterState detailedFilterState = new DetailedFilterState
				{
					MenuStateDict = new Dictionary<int, DetailedFilterMenuState>()
				};
				List<DetailedFilterMenuConfig> menuConfigs = this.GetMenuConfigsForLine(lineConfig);
				bool flag2 = menuConfigs != null;
				if (flag2)
				{
					Dictionary<int, DetailedFilterMenuConfig> menuConfigMap = menuConfigs.ToDictionary((DetailedFilterMenuConfig menuConfig) => menuConfig.Id, (DetailedFilterMenuConfig menuConfig) => menuConfig);
					foreach (DetailedFilterMenuConfig menuConfig2 in menuConfigs)
					{
						int key = SortAndFilter.GetSelectionKey(lineConfig.Id, menuConfig2.Id);
						List<int> indices;
						List<int> selectedIndices = this._selectedIndices.TryGetValue(key, out indices) ? indices : new List<int>();
						bool menuActive = isActive && this.IsMenuVisibleByDependency(lineIndex, lineConfig.Id, menuConfig2, menuConfigMap, null);
						detailedFilterState.MenuStateDict[menuConfig2.Id] = new DetailedFilterMenuState(selectedIndices, menuActive);
					}
				}
				result = new LineState
				{
					IsActive = isActive,
					Type = lineConfig.Type,
					ToggleGroupState = toggleGroupState,
					DetailedFilterState = new DetailedFilterLineState
					{
						State = detailedFilterState
					}
				};
			}
			return result;
		}

		// Token: 0x0600A4DD RID: 42205 RVA: 0x004CE3D8 File Offset: 0x004CC5D8
		private ToggleKey GetSingleSelectLineToggleState(LineConfig lineConfig)
		{
			DetailedFilterLineConfig detailedFilterLineConfig = lineConfig.DetailedFilterLineConfig;
			bool flag = ((detailedFilterLineConfig != null) ? detailedFilterLineConfig.Config.MenuConfigs : null) == null;
			ToggleKey result;
			if (flag)
			{
				result = new ToggleKey
				{
					IsAll = false,
					Index = -1
				};
			}
			else
			{
				foreach (DetailedFilterMenuConfig menuConfig in lineConfig.DetailedFilterLineConfig.Config.MenuConfigs)
				{
					int key = SortAndFilter.GetSelectionKey(lineConfig.Id, menuConfig.Id);
					List<int> selectedIndices;
					bool flag2 = this._selectedIndices.TryGetValue(key, out selectedIndices) && selectedIndices.Count > 0;
					if (flag2)
					{
						return new ToggleKey
						{
							IsAll = false,
							Index = selectedIndices[0]
						};
					}
				}
				result = new ToggleKey
				{
					IsAll = false,
					Index = -1
				};
			}
			return result;
		}

		// Token: 0x0600A4DE RID: 42206 RVA: 0x004CE4EC File Offset: 0x004CC6EC
		private void RefreshSectionViewData()
		{
			this._sections.Clear();
			SortAndFilterConfig config = this.Config;
			bool flag = ((config != null) ? config.LineConfigs : null) == null;
			if (!flag)
			{
				for (int i = 0; i < this.Config.LineConfigs.Count; i++)
				{
					bool flag2 = i == this._firstToggleGroupIndex;
					if (!flag2)
					{
						LineConfig lineConfig = this.Config.LineConfigs[i];
						bool active;
						bool isActive = this._activeCache.TryGetValue(i, out active) && active;
						List<DetailedFilterMenuConfig> menuConfigs = this.GetMenuConfigsForLine(lineConfig);
						bool flag3 = menuConfigs != null;
						if (flag3)
						{
							Dictionary<int, DetailedFilterMenuConfig> menuConfigMap = menuConfigs.ToDictionary((DetailedFilterMenuConfig menuConfig) => menuConfig.Id, (DetailedFilterMenuConfig menuConfig) => menuConfig);
							foreach (DetailedFilterMenuConfig menuConfig2 in menuConfigs)
							{
								bool menuVisible = isActive && this.IsMenuVisibleByDependency(i, lineConfig.Id, menuConfig2, menuConfigMap, null);
								bool flag4 = !menuVisible;
								if (!flag4)
								{
									List<SortAndFilter.SectionViewData> sections = this._sections;
									SortAndFilter.SectionViewData item = default(SortAndFilter.SectionViewData);
									item.LineIndex = i;
									item.LineId = lineConfig.Id;
									item.MenuId = menuConfig2.Id;
									item.Type = lineConfig.Type;
									item.IsActive = isActive;
									StringKey menuBarLabel = menuConfig2.DropdownConfig.MenuBarLabel;
									item.Title = menuBarLabel.GetString();
									sections.Add(item);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A4DF RID: 42207 RVA: 0x004CE6C0 File Offset: 0x004CC8C0
		private List<DetailedFilterMenuConfig> GetMenuConfigsForLine(LineConfig lineConfig)
		{
			DetailedFilterLineConfig detailedFilterLineConfig = lineConfig.DetailedFilterLineConfig;
			bool flag = ((detailedFilterLineConfig != null) ? detailedFilterLineConfig.Config.MenuConfigs : null) == null;
			List<DetailedFilterMenuConfig> result2;
			if (flag)
			{
				result2 = null;
			}
			else
			{
				DynamicLineConfig dynamicConfig;
				bool flag2;
				if (this._dynamicConfigs.TryGetValue(lineConfig.Id, out dynamicConfig))
				{
					DynamicDetailedFilterLineConfig detailedFilterLineConfig2 = dynamicConfig.DetailedFilterLineConfig;
					flag2 = (((detailedFilterLineConfig2 != null) ? detailedFilterLineConfig2.ChangedMenuConfigs : null) != null);
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					List<DetailedFilterMenuConfig> result = new List<DetailedFilterMenuConfig>(lineConfig.DetailedFilterLineConfig.Config.MenuConfigs);
					foreach (KeyValuePair<int, FilterDropdownConfig> kvp in dynamicConfig.DetailedFilterLineConfig.ChangedMenuConfigs)
					{
						int menuId = kvp.Key;
						FilterDropdownConfig newConfig = kvp.Value;
						int index = result.FindIndex((DetailedFilterMenuConfig m) => m.Id == menuId);
						bool flag4 = index >= 0;
						if (flag4)
						{
							result[index] = new DetailedFilterMenuConfig
							{
								Id = menuId,
								DropdownConfig = newConfig,
								DropdownContext = result[index].DropdownContext
							};
						}
					}
					result2 = result;
				}
				else
				{
					result2 = lineConfig.DetailedFilterLineConfig.Config.MenuConfigs;
				}
			}
			return result2;
		}

		// Token: 0x0600A4E0 RID: 42208 RVA: 0x004CE82C File Offset: 0x004CCA2C
		private bool IsMenuVisibleByDependency(int lineIndex, int lineId, DetailedFilterMenuConfig menuConfig, Dictionary<int, DetailedFilterMenuConfig> menuConfigMap, HashSet<int> visitingMenuIds = null)
		{
			bool flag = !this.ShouldShowMenu(lineIndex, menuConfig.Id);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				MenuOptionIndex? dependency = menuConfig.DropdownContext.Dependency;
				bool flag2 = dependency == null;
				if (flag2)
				{
					result = true;
				}
				else
				{
					if (visitingMenuIds == null)
					{
						visitingMenuIds = new HashSet<int>();
					}
					bool flag3 = !visitingMenuIds.Add(menuConfig.Id);
					if (flag3)
					{
						result = false;
					}
					else
					{
						int dependencyMenuId = dependency.Value.MenuId;
						DetailedFilterMenuConfig dependencyMenuConfig;
						bool flag4 = !menuConfigMap.TryGetValue(dependencyMenuId, out dependencyMenuConfig);
						if (flag4)
						{
							visitingMenuIds.Remove(menuConfig.Id);
							result = false;
						}
						else
						{
							bool dependencyMenuVisible = this.IsMenuVisibleByDependency(lineIndex, lineId, dependencyMenuConfig, menuConfigMap, visitingMenuIds);
							visitingMenuIds.Remove(menuConfig.Id);
							bool flag5 = !dependencyMenuVisible;
							result = (!flag5 && this.IsMenuOptionSelected(lineId, dependencyMenuId, dependency.Value.OptionIndex));
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600A4E1 RID: 42209 RVA: 0x004CE91C File Offset: 0x004CCB1C
		private bool IsMenuOptionSelected(int lineId, int menuId, int optionIndex)
		{
			int key = SortAndFilter.GetSelectionKey(lineId, menuId);
			List<int> selectedIndices;
			return this._selectedIndices.TryGetValue(key, out selectedIndices) && selectedIndices.Contains(optionIndex);
		}

		// Token: 0x0600A4E2 RID: 42210 RVA: 0x004CE950 File Offset: 0x004CCB50
		private bool CanUseVisibleDropdownMenusApi()
		{
			SortAndFilterConfig config = this.Config;
			bool flag = ((config != null) ? config.LineConfigs : null) == null || this.Config.LineConfigs.Count == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				LineConfig firstLine = this.Config.LineConfigs[0];
				bool flag2 = firstLine.Type == ESortAndFilterOneLineType.ToggleGroup;
				if (flag2)
				{
					result = false;
				}
				else
				{
					DetailedFilterLineConfig detailedFilterLineConfig = firstLine.DetailedFilterLineConfig;
					List<DetailedFilterMenuConfig> menuConfigs = (detailedFilterLineConfig != null) ? detailedFilterLineConfig.Config.MenuConfigs : null;
					bool flag3 = menuConfigs == null;
					if (flag3)
					{
						result = false;
					}
					else
					{
						foreach (DetailedFilterMenuConfig menuConfig in menuConfigs)
						{
							bool flag4 = menuConfig.DropdownContext.Dependency != null;
							if (flag4)
							{
								return false;
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x0600A4E3 RID: 42211 RVA: 0x004CEA40 File Offset: 0x004CCC40
		private void RefreshSummary()
		{
			List<SortAndFilter.SummaryItemData> summaryItems = this.GetSummaryItems();
			this.filterSummaryArea.gameObject.SetActive(summaryItems.Count > 0);
			CommonUtils.PrepareEnoughChildren(this.filterSummaryRoot, this.summaryItemTemplate.gameObject, summaryItems.Count, null);
			for (int i = 0; i < summaryItems.Count; i++)
			{
				SortAndFilter.SummaryItemData itemData = summaryItems[i];
				Transform itemGo = this.filterSummaryRoot.GetChild(i);
				FilterSummaryItem item = itemGo.GetComponent<FilterSummaryItem>();
				bool flag = item == null;
				if (!flag)
				{
					item.gameObject.SetActive(true);
					item.Setup(itemData.LineId, itemData.MenuId, itemData.Text, new Action<int, int>(this.OnSummaryItemDelete));
				}
			}
			for (int j = summaryItems.Count; j < this.filterSummaryRoot.childCount; j++)
			{
				Transform child = this.filterSummaryRoot.GetChild(j);
				bool flag2 = child != null;
				if (flag2)
				{
					child.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0600A4E4 RID: 42212 RVA: 0x004CEB63 File Offset: 0x004CCD63
		private void RefreshSectionsSummaryAndPanel()
		{
			this.RefreshSectionViewData();
			this.RefreshSummary();
			this.RefreshEntryVisibility();
			this.filterPanel.Refresh();
		}

		// Token: 0x0600A4E5 RID: 42213 RVA: 0x004CEB88 File Offset: 0x004CCD88
		private void ResetRuntimeState()
		{
			this.DestroyFirstToggleGroup();
			this._activeCache.Clear();
			this._outsideMenuInvisibleDict.Clear();
			this._sections.Clear();
			this._selectedIndices.Clear();
			this._optionInteractableState.Clear();
			this._dynamicConfigs.Clear();
			this._outsideVisibleSortIds = null;
			this._filteredCount = -1;
			this.Config = null;
		}

		// Token: 0x0600A4E6 RID: 42214 RVA: 0x004CEBFC File Offset: 0x004CCDFC
		private void ApplyOutsideSortVisibility()
		{
			bool flag = this._outsideVisibleSortIds == null;
			if (!flag)
			{
				this.sortButtonGroup.DisplayingSortIds.Clear();
				foreach (short sortId in this._outsideVisibleSortIds)
				{
					this.sortButtonGroup.DisplayingSortIds.Add(sortId);
				}
				bool flag2 = this._outsideVisibleSortIds.Count == 0;
				if (flag2)
				{
					this.sortButtonGroup.DisplayingSortIds.Add(short.MinValue);
				}
				this.sortButtonGroup.SetSortData(this.sortButtonGroup.GetSortData());
			}
		}

		// Token: 0x0600A4E7 RID: 42215 RVA: 0x004CECC0 File Offset: 0x004CCEC0
		private void RefreshEntryVisualState()
		{
			this.SyncEntryToggleWithPanel();
			this.RefreshEntryButtonLabel();
		}

		// Token: 0x0600A4E8 RID: 42216 RVA: 0x004CECD4 File Offset: 0x004CCED4
		private void RefreshEntryButtonLabel()
		{
			bool flag = this.entryButtonLabel == null;
			if (!flag)
			{
				string stateText = this.HasCustomFilterSelection() ? LanguageKey.LK_CommonSortAndFilter_FilterPanel_EntryButton_State_HasFilter.Tr() : LanguageKey.LK_CommonSortAndFilter_FilterPanel_EntryButton_State_Default.Tr();
				this.entryButtonLabel.SetText(stateText, true);
			}
		}

		// Token: 0x0600A4E9 RID: 42217 RVA: 0x004CED24 File Offset: 0x004CCF24
		private bool HasCustomFilterSelection()
		{
			foreach (List<int> selectedIndices in this._selectedIndices.Values)
			{
				bool flag = selectedIndices != null && selectedIndices.Count > 0;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600A4EA RID: 42218 RVA: 0x004CED98 File Offset: 0x004CCF98
		private string GetDefaultPanelTitle()
		{
			string title = this._panelTitleKey.Tr();
			return string.IsNullOrEmpty(title) ? string.Empty : title;
		}

		// Token: 0x0600A4EB RID: 42219 RVA: 0x004CEDC6 File Offset: 0x004CCFC6
		private void OnSummaryItemDelete(int lineId, int menuId)
		{
			this.ClearFilterItem(lineId, menuId);
		}

		// Token: 0x0600A4EC RID: 42220 RVA: 0x004CEDD2 File Offset: 0x004CCFD2
		private void OnSummaryClearButtonClicked()
		{
			this.ClearAllFilter();
		}

		// Token: 0x0600A4ED RID: 42221 RVA: 0x004CEDDC File Offset: 0x004CCFDC
		private bool CalcLineActive(int lineIndex, SortAndFilterState state)
		{
			bool active;
			bool flag = this._activeCache.TryGetValue(lineIndex, out active);
			bool result2;
			if (flag)
			{
				result2 = active;
			}
			else
			{
				LineConfig lineConfig = this.Config.LineConfigs[lineIndex];
				ActiveCondition? condition = lineConfig.ActiveCondition;
				bool flag2 = condition == null;
				if (flag2)
				{
					this._activeCache[lineIndex] = lineConfig.DefaultActive;
					result2 = lineConfig.DefaultActive;
				}
				else
				{
					bool result = condition.Value.ActiveDependsOn.Any((ToggleIdIndex dependency) => this.CheckActiveConditionForOneDependency(state, dependency));
					this._activeCache[lineIndex] = result;
					result2 = result;
				}
			}
			return result2;
		}

		// Token: 0x0600A4EE RID: 42222 RVA: 0x004CEE94 File Offset: 0x004CD094
		private bool CheckActiveConditionForOneDependency(SortAndFilterState state, ToggleIdIndex dependency)
		{
			int lineIndex = this.GetIndexFromId(dependency.LineId);
			bool isDependencyLineActive = this.CalcLineActive(lineIndex, state);
			bool flag = !isDependencyLineActive;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				ToggleKey toggleKey = dependency.ToggleKey;
				bool isAll = toggleKey.IsAll;
				result = (isAll || toggleKey.Index == state.LineStates[lineIndex].ToggleGroupState.Index);
			}
			return result;
		}

		// Token: 0x0600A4EF RID: 42223 RVA: 0x004CEF04 File Offset: 0x004CD104
		private int GetIndexFromId(int id)
		{
			for (int i = 0; i < this.Config.LineConfigs.Count; i++)
			{
				bool flag = this.Config.LineConfigs[i].Id == id;
				if (flag)
				{
					return i;
				}
			}
			throw new ArgumentException(string.Format("No line found with id {0}", id));
		}

		// Token: 0x0600A4F0 RID: 42224 RVA: 0x004CEF6C File Offset: 0x004CD16C
		private int GetIdFromIndex(int index)
		{
			bool flag = index < 0 || index >= this.Config.LineConfigs.Count;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Index {0} is out of range for line configs.", index));
			}
			return this.Config.LineConfigs[index].Id;
		}

		// Token: 0x0600A4F1 RID: 42225 RVA: 0x004CEFCC File Offset: 0x004CD1CC
		private RectTransform GetPanelRoot()
		{
			return (this.filterPanel != null) ? (this.filterPanel.transform as RectTransform) : null;
		}

		// Token: 0x0600A4F2 RID: 42226 RVA: 0x004CEFFF File Offset: 0x004CD1FF
		public void SetSortButtonVisible(bool visible)
		{
			this.sortButtonGroup.gameObject.SetActive(visible);
		}

		// Token: 0x0600A4F4 RID: 42228 RVA: 0x004CF078 File Offset: 0x004CD278
		GameObject ISortAndFilterView.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400823D RID: 33341
		private readonly Dictionary<int, bool> _activeCache = new Dictionary<int, bool>();

		// Token: 0x0400823E RID: 33342
		private readonly Dictionary<int, HashSet<int>> _outsideMenuInvisibleDict = new Dictionary<int, HashSet<int>>();

		// Token: 0x0400823F RID: 33343
		private readonly List<SortAndFilter.SectionViewData> _sections = new List<SortAndFilter.SectionViewData>();

		// Token: 0x04008240 RID: 33344
		private readonly Dictionary<int, List<int>> _selectedIndices = new Dictionary<int, List<int>>();

		// Token: 0x04008241 RID: 33345
		[TupleElementNames(new string[]
		{
			"LineId",
			"MenuId",
			"OptionIndex"
		})]
		private readonly Dictionary<ValueTuple<int, int, int>, SortAndFilter.OptionInteractableState> _optionInteractableState = new Dictionary<ValueTuple<int, int, int>, SortAndFilter.OptionInteractableState>();

		// Token: 0x04008242 RID: 33346
		private readonly Dictionary<int, DynamicLineConfig> _dynamicConfigs = new Dictionary<int, DynamicLineConfig>();

		// Token: 0x04008243 RID: 33347
		private HashSet<short> _outsideVisibleSortIds;

		// Token: 0x04008244 RID: 33348
		private int _filteredCount = -1;

		// Token: 0x04008245 RID: 33349
		private int _firstToggleGroupIndex = -1;

		// Token: 0x04008246 RID: 33350
		private bool _forceHideEntry;

		// Token: 0x04008247 RID: 33351
		private bool _isSyncingEntryToggle;

		// Token: 0x04008248 RID: 33352
		private LanguageKey _panelTitleKey;

		// Token: 0x04008249 RID: 33353
		private const short HideAllSortIdPlaceholder = -32768;

		// Token: 0x0400824A RID: 33354
		[SerializeField]
		private SortButtonGroup sortButtonGroup;

		// Token: 0x0400824B RID: 33355
		[SerializeField]
		private FilterPanel filterPanel;

		// Token: 0x0400824C RID: 33356
		[SerializeField]
		private SwitchToggleSmall entryToggle;

		// Token: 0x0400824D RID: 33357
		[SerializeField]
		private TextMeshProUGUI entryButtonLabel;

		// Token: 0x0400824E RID: 33358
		[SerializeField]
		private ToggleGroupLine firstToggleGroupLine;

		// Token: 0x0400824F RID: 33359
		[SerializeField]
		private RectTransform filterSummaryArea;

		// Token: 0x04008250 RID: 33360
		[SerializeField]
		private RectTransform filterSummaryRoot;

		// Token: 0x04008251 RID: 33361
		[SerializeField]
		private FilterSummaryItem summaryItemTemplate;

		// Token: 0x04008252 RID: 33362
		[SerializeField]
		private CButton clearSummaryButton;

		// Token: 0x04008253 RID: 33363
		[SerializeField]
		private string customFilterPanelLanguageKey;

		// Token: 0x020023FC RID: 9212
		public struct SectionViewData
		{
			// Token: 0x0400E120 RID: 57632
			public int LineIndex;

			// Token: 0x0400E121 RID: 57633
			public int LineId;

			// Token: 0x0400E122 RID: 57634
			public int MenuId;

			// Token: 0x0400E123 RID: 57635
			public string Title;

			// Token: 0x0400E124 RID: 57636
			public bool IsActive;

			// Token: 0x0400E125 RID: 57637
			public ESortAndFilterOneLineType Type;
		}

		// Token: 0x020023FD RID: 9213
		public struct SummaryItemData
		{
			// Token: 0x0400E126 RID: 57638
			public int LineId;

			// Token: 0x0400E127 RID: 57639
			public int MenuId;

			// Token: 0x0400E128 RID: 57640
			public int OptionIndex;

			// Token: 0x0400E129 RID: 57641
			public string Text;
		}

		// Token: 0x020023FE RID: 9214
		private struct OptionInteractableState
		{
			// Token: 0x0400E12A RID: 57642
			public bool Interactable;

			// Token: 0x0400E12B RID: 57643
			public string DisabledTooltip;
		}
	}
}
