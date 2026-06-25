using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CBD RID: 3261
	public class SortAndFilterLegacy : MonoBehaviour, ISortAndFilterView
	{
		// Token: 0x1700113F RID: 4415
		// (get) Token: 0x0600A553 RID: 42323 RVA: 0x004D0F95 File Offset: 0x004CF195
		// (set) Token: 0x0600A554 RID: 42324 RVA: 0x004D0F9D File Offset: 0x004CF19D
		public SortAndFilterConfig Config { get; private set; }

		// Token: 0x17001140 RID: 4416
		// (get) Token: 0x0600A555 RID: 42325 RVA: 0x004D0FA6 File Offset: 0x004CF1A6
		public SortDropdown SortDropdown
		{
			get
			{
				return this.sortDropdown;
			}
		}

		// Token: 0x17001141 RID: 4417
		// (get) Token: 0x0600A556 RID: 42326 RVA: 0x004D0FAE File Offset: 0x004CF1AE
		public ISortUi SortUi
		{
			get
			{
				return this.sortDropdown;
			}
		}

		// Token: 0x0600A557 RID: 42327 RVA: 0x004D0FB8 File Offset: 0x004CF1B8
		public SortAndFilterState GetStateFromUI()
		{
			List<LineState> lineStates = new List<LineState>();
			SortStateData sortData = this.sortDropdown.GetSortData();
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
					ISortAndFilterLine line;
					bool flag2 = this._lines.TryGetValue(i, out line);
					if (flag2)
					{
						lineStates.Add(line.GetLineState());
					}
					else
					{
						LineConfig lineConfig = this.Config.LineConfigs[i];
						DetailedFilterState detailedFilterState = new DetailedFilterState
						{
							MenuStateDict = new Dictionary<int, DetailedFilterMenuState>()
						};
						bool flag3 = (lineConfig.Type == ESortAndFilterOneLineType.DetailedFilter || lineConfig.Type == ESortAndFilterOneLineType.SingleSelectFilter) && lineConfig.DetailedFilterLineConfig != null;
						if (flag3)
						{
							foreach (DetailedFilterMenuConfig menuConfig in lineConfig.DetailedFilterLineConfig.Config.MenuConfigs)
							{
								detailedFilterState.MenuStateDict[menuConfig.Id] = new DetailedFilterMenuState(new List<int>(), false);
							}
						}
						LineState defaultState = new LineState
						{
							IsActive = false,
							Type = lineConfig.Type,
							ToggleGroupState = new ToggleKey
							{
								IsAll = true,
								Index = -1
							},
							DetailedFilterState = new DetailedFilterLineState
							{
								State = detailedFilterState
							}
						};
						lineStates.Add(defaultState);
					}
				}
				result = new SortAndFilterState
				{
					LineStates = lineStates,
					SortData = sortData
				};
			}
			return result;
		}

		// Token: 0x0600A558 RID: 42328 RVA: 0x004D11A4 File Offset: 0x004CF3A4
		public void Reset()
		{
			foreach (ISortAndFilterLine line in this._lines.Values)
			{
				MonoBehaviour mono = line as MonoBehaviour;
				bool flag = mono != null && mono != null;
				if (flag)
				{
					Object.Destroy(mono.gameObject);
				}
			}
			this._lines.Clear();
			foreach (FilterLineLayout layout in this._lineLayouts.Values)
			{
				bool flag2 = layout != null;
				if (flag2)
				{
					Object.Destroy(layout.gameObject);
				}
			}
			this._lineLayouts.Clear();
			this._activeCache.Clear();
			this._outsideMenuInvisibleDict.Clear();
			this.Config = null;
		}

		// Token: 0x0600A559 RID: 42329 RVA: 0x004D12BC File Offset: 0x004CF4BC
		public void RebuildForLanguageChange(SortAndFilterConfig config)
		{
			this.Reset();
			this.Setup(config);
		}

		// Token: 0x0600A55A RID: 42330 RVA: 0x004D12D0 File Offset: 0x004CF4D0
		public void Setup(SortAndFilterConfig config)
		{
			this.Config = config;
			UIBase componentInParent = base.GetComponentInParent<UIBase>();
			UIElement parentElement = (componentInParent != null) ? componentInParent.Element : null;
			this.sortDropdown.Setup(this.Config.SortConfig.UiConfig, config.OnSortChanged, new Action(this.OnShowSortMenu), new Action(SortAndFilterLegacy.OnHideSortMenu));
			this.sortDropdown.ParentElement = parentElement;
			this.UpdateLineActive(0);
		}

		// Token: 0x0600A55B RID: 42331 RVA: 0x004D1348 File Offset: 0x004CF548
		private ISortAndFilterLine GetOrCreateLine(int lineIndex)
		{
			ISortAndFilterLine line;
			bool flag = this._lines.TryGetValue(lineIndex, out line);
			ISortAndFilterLine result;
			if (flag)
			{
				result = line;
			}
			else
			{
				LineConfig lineConfig = this.Config.LineConfigs[lineIndex];
				ISortAndFilterLine newLine;
				switch (lineConfig.Type)
				{
				case ESortAndFilterOneLineType.ToggleGroup:
				{
					FilterLineLayout layout = this.GetOrCreateLineLayout(0);
					Transform parentTransform = lineConfig.IndividualLine ? this.lineRoot : layout.transform;
					ToggleGroupLine toggleGroupLine = Object.Instantiate<ToggleGroupLine>(this.toggleGroupLineTemplate, parentTransform);
					toggleGroupLine.Setup(lineConfig.ToggleGroupLineConfig.Config, delegate
					{
						this.OnToggleGroupFilterChanged(lineConfig.Id);
					});
					bool flag2 = !lineConfig.IndividualLine;
					if (flag2)
					{
						layout.AddChild(toggleGroupLine.GetComponent<RectTransform>(), true);
					}
					newLine = toggleGroupLine;
					break;
				}
				case ESortAndFilterOneLineType.DetailedFilter:
				{
					FilterLineLayout layout2 = this.GetOrCreateLineLayout(1);
					Transform parentTransform2 = lineConfig.IndividualLine ? this.lineRoot : layout2.transform;
					DetailedFilterLine detailedFilterLine = Object.Instantiate<DetailedFilterLine>(this.detailedFilterLineTemplate, parentTransform2);
					detailedFilterLine.Setup(lineConfig.DetailedFilterLineConfig.Config, delegate(int _)
					{
						this.OnDetailedFilterChanged(lineConfig.Id);
					});
					bool flag3 = !lineConfig.IndividualLine;
					if (flag3)
					{
						layout2.AddChild(detailedFilterLine.GetComponent<RectTransform>(), false);
					}
					newLine = detailedFilterLine;
					break;
				}
				case ESortAndFilterOneLineType.SingleSelectFilter:
				{
					FilterLineLayout layout3 = this.GetOrCreateLineLayout(1);
					Transform parentTransform3 = lineConfig.IndividualLine ? this.lineRoot : layout3.transform;
					SecondaryFilterLine secondaryFilterLine = Object.Instantiate<SecondaryFilterLine>(this.secondaryFilterLineTemplate, parentTransform3);
					secondaryFilterLine.Setup(lineConfig.DetailedFilterLineConfig.Config, delegate(int _)
					{
						this.OnDetailedFilterChanged(lineConfig.Id);
					});
					bool flag4 = !lineConfig.IndividualLine;
					if (flag4)
					{
						layout3.AddChild(secondaryFilterLine.GetComponent<RectTransform>(), true);
					}
					newLine = secondaryFilterLine;
					break;
				}
				default:
					throw new ArgumentOutOfRangeException("Type", string.Format("Unsupported line type: {0}", lineConfig.Type));
				}
				Func<int, DynamicLineConfig> dynamicConfigProvider = this.Config.DynamicConfigProvider;
				DynamicLineConfig dynamicConfig = (dynamicConfigProvider != null) ? dynamicConfigProvider(lineConfig.Id) : null;
				bool flag5 = dynamicConfig != null;
				if (flag5)
				{
					newLine.ApplyDynamicConfig(dynamicConfig);
				}
				newLine.SetActive(lineConfig.DefaultActive);
				this._lines[lineIndex] = newLine;
				result = newLine;
			}
			return result;
		}

		// Token: 0x0600A55C RID: 42332 RVA: 0x004D15D8 File Offset: 0x004CF7D8
		private FilterLineLayout GetOrCreateLineLayout(int level)
		{
			FilterLineLayout layout;
			bool flag = this._lineLayouts.TryGetValue(level, out layout);
			FilterLineLayout result;
			if (flag)
			{
				result = layout;
			}
			else
			{
				FilterLineLayout newLayout = Object.Instantiate<FilterLineLayout>(this.filterLineLayoutTemplate, this.lineRoot);
				newLayout.gameObject.SetActive(false);
				newLayout.Init(this.lineRoot.rect.width);
				this._lineLayouts[level] = newLayout;
				result = newLayout;
			}
			return result;
		}

		// Token: 0x0600A55D RID: 42333 RVA: 0x004D164A File Offset: 0x004CF84A
		private void OnShowSortMenu()
		{
			UIManager.Instance.SetCommonSortAndFilterEscHandler(new Action(this.HandleUiEsc));
		}

		// Token: 0x0600A55E RID: 42334 RVA: 0x004D1664 File Offset: 0x004CF864
		private void HandleUiEsc()
		{
			this.sortDropdown.HideMenu();
		}

		// Token: 0x0600A55F RID: 42335 RVA: 0x004D1673 File Offset: 0x004CF873
		private static void OnHideSortMenu()
		{
			UIManager.Instance.SetCommonSortAndFilterEscHandler(null);
		}

		// Token: 0x0600A560 RID: 42336 RVA: 0x004D1682 File Offset: 0x004CF882
		private void OnDetailedFilterChanged(int lineId)
		{
			Action<int> onFilterChanged = this.Config.OnFilterChanged;
			if (onFilterChanged != null)
			{
				onFilterChanged(lineId);
			}
		}

		// Token: 0x0600A561 RID: 42337 RVA: 0x004D169D File Offset: 0x004CF89D
		private void OnToggleGroupFilterChanged(int lineId)
		{
			Action<int> onFilterChanged = this.Config.OnFilterChanged;
			if (onFilterChanged != null)
			{
				onFilterChanged(lineId);
			}
		}

		// Token: 0x0600A562 RID: 42338 RVA: 0x004D16B8 File Offset: 0x004CF8B8
		public void UpdateLineActive(int changedLineIndex)
		{
			SortAndFilterState state = this.GetStateFromUI();
			this._activeCache.Clear();
			for (int lineIndex = 0; lineIndex < this.Config.LineConfigs.Count; lineIndex++)
			{
				this.CalcLineActive(lineIndex, this._activeCache, state, this.Config);
			}
			for (int lineIndex2 = 0; lineIndex2 < this.Config.LineConfigs.Count; lineIndex2++)
			{
				bool shouldBeActive = this._activeCache[lineIndex2];
				int lineId = SortAndFilterLegacy.GetIdFromIndex(lineIndex2, this.Config);
				bool flag = shouldBeActive;
				if (flag)
				{
					ISortAndFilterLine line = this.GetOrCreateLine(lineIndex2);
					line.SetActive(true);
					DetailedFilterLine detailedFilter = line as DetailedFilterLine;
					bool flag2 = detailedFilter != null;
					if (flag2)
					{
						HashSet<int> outsideInvisibleSet = this._outsideMenuInvisibleDict.GetValueOrDefault(lineId);
						SecondaryFilterLine secondaryFilterLine = line as SecondaryFilterLine;
						bool flag3 = secondaryFilterLine != null;
						if (flag3)
						{
							detailedFilter.UpdateMenuVisibility(detailedFilter.GetState(), null);
							bool shouldHideByOutside = outsideInvisibleSet != null && outsideInvisibleSet.Count > 0 && secondaryFilterLine.Config.MenuConfigs.All((DetailedFilterMenuConfig m) => outsideInvisibleSet.Contains(m.Id));
							SortAndFilterLegacy.SetSecondaryFilterLineLayoutVisible(secondaryFilterLine, !shouldHideByOutside);
						}
						else
						{
							detailedFilter.UpdateMenuVisibility(detailedFilter.GetState(), outsideInvisibleSet);
						}
					}
				}
				else
				{
					ISortAndFilterLine line2;
					bool flag4 = this._lines.TryGetValue(lineIndex2, out line2);
					if (flag4)
					{
						line2.SetActive(false);
					}
				}
			}
			foreach (FilterLineLayout layout in this._lineLayouts.Values)
			{
				bool hasActiveChild = layout.transform.Cast<Transform>().Any((Transform child) => child.gameObject.activeSelf);
				layout.gameObject.SetActive(hasActiveChild);
			}
			this.OnLineActiveChanged();
		}

		// Token: 0x0600A563 RID: 42339 RVA: 0x004D18E4 File Offset: 0x004CFAE4
		private static void SetSecondaryFilterLineLayoutVisible(SecondaryFilterLine line, bool isVisible)
		{
			bool flag = line == null;
			if (!flag)
			{
				LayoutElement layoutElement = line.GetComponent<LayoutElement>();
				bool flag2 = layoutElement != null;
				if (flag2)
				{
					layoutElement.ignoreLayout = !isVisible;
				}
				CanvasGroup canvasGroup = line.GetComponent<CanvasGroup>();
				bool flag3 = canvasGroup == null;
				if (flag3)
				{
					canvasGroup = line.gameObject.AddComponent<CanvasGroup>();
				}
				canvasGroup.alpha = (isVisible ? 1f : 0f);
				canvasGroup.blocksRaycasts = isVisible;
				canvasGroup.interactable = isVisible;
			}
		}

		// Token: 0x0600A564 RID: 42340 RVA: 0x004D1966 File Offset: 0x004CFB66
		private void OnLineActiveChanged()
		{
		}

		// Token: 0x0600A565 RID: 42341 RVA: 0x004D196C File Offset: 0x004CFB6C
		private bool CalcLineActive(int lineIndex, Dictionary<int, bool> activeCache, SortAndFilterState state, SortAndFilterConfig config)
		{
			bool active;
			bool flag = activeCache.TryGetValue(lineIndex, out active);
			bool result2;
			if (flag)
			{
				result2 = active;
			}
			else
			{
				LineConfig lineConfig = config.LineConfigs[lineIndex];
				ActiveCondition? condition = lineConfig.ActiveCondition;
				bool flag2 = condition == null;
				if (flag2)
				{
					activeCache[lineIndex] = lineConfig.DefaultActive;
					result2 = lineConfig.DefaultActive;
				}
				else
				{
					bool result = condition.Value.ActiveDependsOn.Any((ToggleIdIndex dependency) => this.CheckActiveConditionForOneDependency(activeCache, state, config, dependency));
					activeCache[lineIndex] = result;
					result2 = result;
				}
			}
			return result2;
		}

		// Token: 0x0600A566 RID: 42342 RVA: 0x004D1A34 File Offset: 0x004CFC34
		private bool CheckActiveConditionForOneDependency(Dictionary<int, bool> activeCache, SortAndFilterState state, SortAndFilterConfig config, ToggleIdIndex dependency)
		{
			int lineIndex = SortAndFilterLegacy.GetIndexFromId(dependency.LineId, config);
			bool isDependencyLineActive = this.CalcLineActive(lineIndex, activeCache, state, config);
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

		// Token: 0x0600A567 RID: 42343 RVA: 0x004D1AA8 File Offset: 0x004CFCA8
		private static int GetIndexFromId(int id, SortAndFilterConfig config)
		{
			for (int i = 0; i < config.LineConfigs.Count; i++)
			{
				bool flag = config.LineConfigs[i].Id == id;
				if (flag)
				{
					return i;
				}
			}
			throw new ArgumentException(string.Format("No line found with id {0}", id));
		}

		// Token: 0x0600A568 RID: 42344 RVA: 0x004D1B08 File Offset: 0x004CFD08
		private static int GetIdFromIndex(int index, SortAndFilterConfig config)
		{
			bool flag = index < 0 || index >= config.LineConfigs.Count;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Index {0} is out of range for line configs.", index));
			}
			return config.LineConfigs[index].Id;
		}

		// Token: 0x0600A569 RID: 42345 RVA: 0x004D1B5E File Offset: 0x004CFD5E
		public void RefreshSortDisplay<TData>(List<FilterLineBase<TData>> activeSort, List<LineState> lineStates)
		{
			this.sortDropdown.RefreshDisplayOptions<TData>(activeSort, lineStates);
		}

		// Token: 0x0600A56A RID: 42346 RVA: 0x004D1B70 File Offset: 0x004CFD70
		public void ClearAllFilter()
		{
			foreach (ISortAndFilterLine line in this._lines.Values)
			{
				line.ClearAllFilter();
			}
			Action<int> onFilterChanged = this.Config.OnFilterChanged;
			if (onFilterChanged != null)
			{
				onFilterChanged(-1);
			}
		}

		// Token: 0x0600A56B RID: 42347 RVA: 0x004D1BE8 File Offset: 0x004CFDE8
		public void SetToggleVisible(ToggleIdIndex toggleIndex, bool isVisible)
		{
			int lineIndex = SortAndFilterLegacy.GetIndexFromId(toggleIndex.LineId, this.Config);
			ISortAndFilterLine line = this.GetOrCreateLine(lineIndex);
			ToggleGroupLine toggleGroupLine = line as ToggleGroupLine;
			bool flag = toggleGroupLine != null;
			if (flag)
			{
				toggleGroupLine.SetToggleVisible(toggleIndex.ToggleKey, isVisible);
			}
			else
			{
				Debug.LogWarning(string.Format("[SortAndFilter] SetToggleVisible failed, line {0} is not ToggleGroupLine", lineIndex));
			}
		}

		// Token: 0x0600A56C RID: 42348 RVA: 0x004D1C4C File Offset: 0x004CFE4C
		public void SetToggleIsOn(ToggleIdIndex toggleIndex, bool isOn)
		{
			int lineIndex = SortAndFilterLegacy.GetIndexFromId(toggleIndex.LineId, this.Config);
			ISortAndFilterLine line = this.GetOrCreateLine(lineIndex);
			ToggleGroupLine toggleGroupLine = line as ToggleGroupLine;
			bool flag = toggleGroupLine != null;
			if (flag)
			{
				toggleGroupLine.SetToggleIsOn(toggleIndex.ToggleKey, isOn);
			}
			else
			{
				Debug.LogWarning(string.Format("[SortAndFilter] SetToggleIsOn failed, line {0} is not ToggleGroupLine", lineIndex));
			}
		}

		// Token: 0x0600A56D RID: 42349 RVA: 0x004D1CB0 File Offset: 0x004CFEB0
		public void SetToggleIsOnWithoutNotify(int lineId, int toggleIndex)
		{
			int lineIndex = SortAndFilterLegacy.GetIndexFromId(lineId, this.Config);
			ISortAndFilterLine line = this.GetOrCreateLine(lineIndex);
			ToggleGroupLine toggleGroupLine = line as ToggleGroupLine;
			bool flag = toggleGroupLine != null;
			if (flag)
			{
				ToggleKey toggleKey = (toggleIndex < 0) ? ToggleKey.AllKey : new ToggleKey
				{
					Index = toggleIndex,
					IsAll = false
				};
				toggleGroupLine.SetToggleIsOnWithoutNotify(toggleKey);
			}
			else
			{
				Debug.LogWarning(string.Format("[SortAndFilter] SetToggleIsOnWithoutNotify failed, line {0} is not ToggleGroupLine", lineIndex));
			}
		}

		// Token: 0x0600A56E RID: 42350 RVA: 0x004D1D30 File Offset: 0x004CFF30
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
			this.UpdateLineActive(SortAndFilterLegacy.GetIndexFromId(lineId, this.Config));
		}

		// Token: 0x0600A56F RID: 42351 RVA: 0x004D1DD5 File Offset: 0x004CFFD5
		public void SetVisibleDropdownMenus(int lineId, IEnumerable<int> visibleMenuIds)
		{
		}

		// Token: 0x0600A570 RID: 42352 RVA: 0x004D1DD8 File Offset: 0x004CFFD8
		public void SetDropdownOptionInteractable(int lineId, int menuId, int optionIndex, bool interactable, string disabledTooltip)
		{
			int lineIndex = SortAndFilterLegacy.GetIndexFromId(lineId, this.Config);
			ISortAndFilterLine line = this.GetOrCreateLine(lineIndex);
			DetailedFilterLine detailedFilter = line as DetailedFilterLine;
			bool flag = detailedFilter != null;
			if (flag)
			{
				detailedFilter.SetOptionInteractable(menuId, optionIndex, interactable, disabledTooltip);
			}
		}

		// Token: 0x0600A571 RID: 42353 RVA: 0x004D1E1C File Offset: 0x004D001C
		public void SetDropdownOption(int lineId, int menuId, int optionIndex)
		{
			int lineIndex = SortAndFilterLegacy.GetIndexFromId(lineId, this.Config);
			ISortAndFilterLine line = this.GetOrCreateLine(lineIndex);
			DetailedFilterLine detailedFilter = line as DetailedFilterLine;
			bool flag = detailedFilter != null;
			if (flag)
			{
				detailedFilter.SetOption(menuId, optionIndex);
			}
			else
			{
				Debug.LogWarning(string.Format("[SortAndFilter] SetDropdownOption failed, line {0} is not DetailedFilterLine", lineIndex));
			}
		}

		// Token: 0x0600A572 RID: 42354 RVA: 0x004D1E73 File Offset: 0x004D0073
		public void ApplyFilterLineStates(List<LineState> lineStates)
		{
		}

		// Token: 0x0600A573 RID: 42355 RVA: 0x004D1E76 File Offset: 0x004D0076
		public void RefreshDynamicConfigs()
		{
		}

		// Token: 0x0600A575 RID: 42357 RVA: 0x004D1EAE File Offset: 0x004D00AE
		GameObject ISortAndFilterView.get_gameObject()
		{
			return base.gameObject;
		}

		// Token: 0x0400828C RID: 33420
		private readonly Dictionary<int, bool> _activeCache = new Dictionary<int, bool>();

		// Token: 0x0400828D RID: 33421
		private readonly Dictionary<int, ISortAndFilterLine> _lines = new Dictionary<int, ISortAndFilterLine>();

		// Token: 0x0400828E RID: 33422
		private readonly Dictionary<int, HashSet<int>> _outsideMenuInvisibleDict = new Dictionary<int, HashSet<int>>();

		// Token: 0x04008290 RID: 33424
		private readonly Dictionary<int, FilterLineLayout> _lineLayouts = new Dictionary<int, FilterLineLayout>();

		// Token: 0x04008291 RID: 33425
		[Header("Templates")]
		[SerializeField]
		private ToggleGroupLine toggleGroupLineTemplate;

		// Token: 0x04008292 RID: 33426
		[SerializeField]
		private DetailedFilterLine detailedFilterLineTemplate;

		// Token: 0x04008293 RID: 33427
		[SerializeField]
		private SecondaryFilterLine secondaryFilterLineTemplate;

		// Token: 0x04008294 RID: 33428
		[SerializeField]
		private FilterLineLayout filterLineLayoutTemplate;

		// Token: 0x04008295 RID: 33429
		[Header("Roots")]
		[SerializeField]
		private RectTransform lineRoot;

		// Token: 0x04008296 RID: 33430
		[Header("Components")]
		[SerializeField]
		private SortDropdown sortDropdown;
	}
}
