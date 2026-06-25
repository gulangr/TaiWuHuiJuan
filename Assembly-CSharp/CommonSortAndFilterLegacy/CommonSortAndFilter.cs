using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.UISystem.Components;
using UnityEngine;
using UnityEngine.Events;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000429 RID: 1065
	public class CommonSortAndFilter : MonoBehaviour
	{
		// Token: 0x17000668 RID: 1640
		// (get) Token: 0x06003F17 RID: 16151 RVA: 0x001F7E52 File Offset: 0x001F6052
		// (set) Token: 0x06003F18 RID: 16152 RVA: 0x001F7E5A File Offset: 0x001F605A
		public SortAndFilterConfig Config { get; private set; }

		// Token: 0x17000669 RID: 1641
		// (get) Token: 0x06003F19 RID: 16153 RVA: 0x001F7E63 File Offset: 0x001F6063
		public CommonSortDropdown SortDropdown
		{
			get
			{
				return this.sortDropdown;
			}
		}

		// Token: 0x06003F1A RID: 16154 RVA: 0x001F7E6C File Offset: 0x001F606C
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
							foreach (MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig> menuConfig in lineConfig.DetailedFilterLineConfig.Config.MenuConfigs)
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

		// Token: 0x06003F1B RID: 16155 RVA: 0x001F8058 File Offset: 0x001F6258
		public IFilterLineCustomOrderData GetCustomOrderFromLine(int lineId)
		{
			int lineIndex = CommonSortAndFilter.GetIndexFromId(lineId, this.Config);
			ISortAndFilterLine line = this.GetOrCreateLine(lineIndex);
			return line.GetCustomOrderData();
		}

		// Token: 0x06003F1C RID: 16156 RVA: 0x001F8088 File Offset: 0x001F6288
		public void Setup(SortAndFilterConfig config)
		{
			this.Config = config;
			UIBase componentInParent = base.GetComponentInParent<UIBase>();
			UIElement parentElement = (componentInParent != null) ? componentInParent.Element : null;
			this.sortDropdown.Setup(this.Config.SortConfig.UiConfig, config.OnSortChanged, new Action(this.OnShowSortMenu), new Action(CommonSortAndFilter.OnHideSortMenu));
			this.sortDropdown.ParentElement = parentElement;
			this.sortDropdown.DisableAutoHeight = this.disableAutoHeight;
			this.UpdateLineActive(0);
			this.SetupTools();
		}

		// Token: 0x06003F1D RID: 16157 RVA: 0x001F8118 File Offset: 0x001F6318
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
				UIBase componentInParent = base.GetComponentInParent<UIBase>();
				UIElement parentElement = (componentInParent != null) ? componentInParent.Element : null;
				ISortAndFilterLine newLine;
				switch (lineConfig.Type)
				{
				case ESortAndFilterOneLineType.ToggleGroup:
				{
					CommonFilterLineLayout layout = this.GetOrCreateLineLayout(0);
					Transform parentTransform = lineConfig.IndividualLine ? this.lineRoot : layout.transform;
					CommonFilterToggleGroup toggleGroupLine = Object.Instantiate<CommonFilterToggleGroup>(this.toggleGroupLineTemplate, parentTransform);
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
					CommonFilterLineLayout layout2 = this.GetOrCreateLineLayout(1);
					Transform parentTransform2 = lineConfig.IndividualLine ? this.lineRoot : layout2.transform;
					CommonDetailedFilter detailedFilterLine = Object.Instantiate<CommonDetailedFilter>(this.detailedFilterTemplate, parentTransform2);
					detailedFilterLine.Setup(lineConfig.DetailedFilterLineConfig.Config, delegate(int _)
					{
						this.OnDetailedFilterChanged(lineConfig.Id);
					}, delegate
					{
						this.OnDetailedFilterOrderChanged(lineConfig.Id);
					}, parentElement, this.disableAutoHeight);
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
					CommonFilterLineLayout layout3 = this.GetOrCreateLineLayout(1);
					Transform parentTransform3 = lineConfig.IndividualLine ? this.lineRoot : layout3.transform;
					CommonSecondaryFilter secondaryFilterLine = Object.Instantiate<CommonSecondaryFilter>(this.secondaryFilterTemplate, parentTransform3);
					secondaryFilterLine.Setup(lineConfig.DetailedFilterLineConfig.Config, delegate(int _)
					{
						this.OnDetailedFilterChanged(lineConfig.Id);
					}, delegate
					{
						this.OnDetailedFilterOrderChanged(lineConfig.Id);
					}, parentElement, this.disableAutoHeight);
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

		// Token: 0x06003F1E RID: 16158 RVA: 0x001F83E4 File Offset: 0x001F65E4
		private CommonFilterLineLayout GetOrCreateLineLayout(int level)
		{
			CommonFilterLineLayout layout;
			bool flag = this._lineLayouts.TryGetValue(level, out layout);
			CommonFilterLineLayout result;
			if (flag)
			{
				result = layout;
			}
			else
			{
				CommonFilterLineLayout newLayout = Object.Instantiate<CommonFilterLineLayout>(this.filterLineLayoutTemplate, this.lineRoot);
				newLayout.gameObject.SetActive(false);
				newLayout.Init(this.lineRoot.rect.width);
				this._lineLayouts[level] = newLayout;
				result = newLayout;
			}
			return result;
		}

		// Token: 0x06003F1F RID: 16159 RVA: 0x001F8458 File Offset: 0x001F6658
		public void ApplyCustomOrder(Dictionary<int, IFilterLineCustomOrderData> detailedLineOrderDict)
		{
			bool flag = detailedLineOrderDict == null || detailedLineOrderDict.Count == 0;
			if (!flag)
			{
				foreach (KeyValuePair<int, IFilterLineCustomOrderData> kvp in detailedLineOrderDict)
				{
					this.ApplyCustomOrderForLine(kvp.Key, kvp.Value);
				}
			}
		}

		// Token: 0x06003F20 RID: 16160 RVA: 0x001F84D0 File Offset: 0x001F66D0
		public void ApplyCustomOrderForLine(int lineId, IFilterLineCustomOrderData orderData)
		{
			int lineIndex = CommonSortAndFilter.GetIndexFromId(lineId, this.Config);
			ISortAndFilterLine line = this.GetOrCreateLine(lineIndex);
			line.ApplyCustomOrder(orderData);
		}

		// Token: 0x06003F21 RID: 16161 RVA: 0x001F84FB File Offset: 0x001F66FB
		private void OnShowSortMenu()
		{
			UIManager.Instance.SetCommonSortAndFilterEscHandler(new Action(this.HandleUiEsc));
		}

		// Token: 0x06003F22 RID: 16162 RVA: 0x001F8515 File Offset: 0x001F6715
		private void HandleUiEsc()
		{
			this.customOrderSettingModeToggle.isOn = false;
			this.sortDropdown.HideMenu();
		}

		// Token: 0x06003F23 RID: 16163 RVA: 0x001F8531 File Offset: 0x001F6731
		private static void OnHideSortMenu()
		{
			UIManager.Instance.SetCommonSortAndFilterEscHandler(null);
		}

		// Token: 0x06003F24 RID: 16164 RVA: 0x001F8540 File Offset: 0x001F6740
		private void SetupTools()
		{
			this.customOrderSettingModeToggle.onValueChanged.RemoveAllListeners();
			this.customOrderSettingModeToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnCustomOrderSettingModeToggleChanged));
			this.clearFilterButton.ClearAndAddListener(new Action(this.OnClickClearFilterButton));
			this.clearCustomOrderButton.ClearAndAddListener(new Action(this.OnClickClearCustomOrderButton));
		}

		// Token: 0x06003F25 RID: 16165 RVA: 0x001F85AC File Offset: 0x001F67AC
		private void OnClickClearCustomOrderButton()
		{
			foreach (ISortAndFilterLine line in this._lines.Values)
			{
				line.ResetCustomOrder();
			}
			Action onFilterCustomOrderReset = this.Config.OnFilterCustomOrderReset;
			if (onFilterCustomOrderReset != null)
			{
				onFilterCustomOrderReset();
			}
		}

		// Token: 0x06003F26 RID: 16166 RVA: 0x001F8620 File Offset: 0x001F6820
		private void OnCustomOrderSettingModeToggleChanged(bool isOn)
		{
			this.sortDropdown.OnSwitchCustomOrderSettingMode(isOn);
			if (isOn)
			{
				for (int i = 0; i < this.Config.LineConfigs.Count; i++)
				{
					this.GetOrCreateLine(i).OnSwitchCustomOrderSettingMode(true);
				}
				UIManager.Instance.SetCommonSortAndFilterEscHandler(new Action(this.HandleUiEsc));
				ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
				argumentBox.SetObject("HighlightObj", base.gameObject.GetComponent<RectTransform>());
				UIElement.UI_FullScreenCover.SetOnInitArgs(argumentBox);
				UIManager.Instance.ShowUI(UIElement.UI_FullScreenCover, true);
			}
			else
			{
				GEvent.OnEvent(UiEvents.ReleaseCoverObject, null);
				foreach (ISortAndFilterLine line in this._lines.Values)
				{
					line.OnSwitchCustomOrderSettingMode(false);
				}
			}
		}

		// Token: 0x06003F27 RID: 16167 RVA: 0x001F8730 File Offset: 0x001F6930
		private void OnClickClearFilterButton()
		{
			this.ClearAllFilter();
		}

		// Token: 0x06003F28 RID: 16168 RVA: 0x001F873A File Offset: 0x001F693A
		private void OnDetailedFilterOrderChanged(int lineId)
		{
			Action<int> onFilterCustomOrderChanged = this.Config.OnFilterCustomOrderChanged;
			if (onFilterCustomOrderChanged != null)
			{
				onFilterCustomOrderChanged(lineId);
			}
		}

		// Token: 0x06003F29 RID: 16169 RVA: 0x001F8755 File Offset: 0x001F6955
		private void OnDetailedFilterChanged(int lineId)
		{
			Action<int> onFilterChanged = this.Config.OnFilterChanged;
			if (onFilterChanged != null)
			{
				onFilterChanged(lineId);
			}
		}

		// Token: 0x06003F2A RID: 16170 RVA: 0x001F8770 File Offset: 0x001F6970
		private void OnToggleGroupFilterChanged(int lineId)
		{
			Action<int> onFilterChanged = this.Config.OnFilterChanged;
			if (onFilterChanged != null)
			{
				onFilterChanged(lineId);
			}
		}

		// Token: 0x06003F2B RID: 16171 RVA: 0x001F878C File Offset: 0x001F698C
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
				int lineId = CommonSortAndFilter.GetIdFromIndex(lineIndex2, this.Config);
				bool flag = shouldBeActive;
				if (flag)
				{
					ISortAndFilterLine line = this.GetOrCreateLine(lineIndex2);
					line.SetActive(true);
					CommonDetailedFilter detailedFilter = line as CommonDetailedFilter;
					bool flag2 = detailedFilter != null;
					if (flag2)
					{
						detailedFilter.UpdateMenuVisibility(state.LineStates[lineIndex2].DetailedFilterState.State, this._outsideMenuInvisibleDict.GetValueOrDefault(lineId));
					}
				}
				else
				{
					ISortAndFilterLine line2;
					bool flag3 = this._lines.TryGetValue(lineIndex2, out line2);
					if (flag3)
					{
						line2.SetActive(false);
					}
				}
			}
			foreach (CommonFilterLineLayout layout in this._lineLayouts.Values)
			{
				bool hasActiveChild = layout.transform.Cast<Transform>().Any((Transform child) => child.gameObject.activeSelf);
				layout.gameObject.SetActive(hasActiveChild);
			}
			this.OnLineActiveChanged();
		}

		// Token: 0x06003F2C RID: 16172 RVA: 0x001F893C File Offset: 0x001F6B3C
		private void OnLineActiveChanged()
		{
			this.CheckToolBarActive();
		}

		// Token: 0x06003F2D RID: 16173 RVA: 0x001F8948 File Offset: 0x001F6B48
		private void CheckToolBarActive()
		{
			bool flag = this.hideToolBar;
			if (flag)
			{
				this.toolBar.SetActive(false);
			}
			else
			{
				bool isToolBarActive = false;
				foreach (KeyValuePair<int, ISortAndFilterLine> pair in this._lines)
				{
					ISortAndFilterLine line = pair.Value;
					LineConfig config = this.Config.LineConfigs[pair.Key];
					bool flag2 = config.Type == ESortAndFilterOneLineType.DetailedFilter && line.IsActive();
					if (flag2)
					{
						isToolBarActive = true;
						break;
					}
				}
				this.toolBar.SetActive(isToolBarActive);
			}
		}

		// Token: 0x06003F2E RID: 16174 RVA: 0x001F8A08 File Offset: 0x001F6C08
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

		// Token: 0x06003F2F RID: 16175 RVA: 0x001F8AD0 File Offset: 0x001F6CD0
		private bool CheckActiveConditionForOneDependency(Dictionary<int, bool> activeCache, SortAndFilterState state, SortAndFilterConfig config, ToggleIdIndex dependency)
		{
			int lineIndex = CommonSortAndFilter.GetIndexFromId(dependency.LineId, config);
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

		// Token: 0x06003F30 RID: 16176 RVA: 0x001F8B44 File Offset: 0x001F6D44
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

		// Token: 0x06003F31 RID: 16177 RVA: 0x001F8BA4 File Offset: 0x001F6DA4
		private static int GetIdFromIndex(int index, SortAndFilterConfig config)
		{
			bool flag = index < 0 || index >= config.LineConfigs.Count;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Index {0} is out of range for line configs.", index));
			}
			return config.LineConfigs[index].Id;
		}

		// Token: 0x06003F32 RID: 16178 RVA: 0x001F8BFA File Offset: 0x001F6DFA
		public void RefreshSortDisplay<TData>(List<FilterLineBase<TData>> activeSort, List<LineState> lineStates)
		{
			this.sortDropdown.RefreshDisplayOptions<TData>(activeSort, lineStates);
		}

		// Token: 0x06003F33 RID: 16179 RVA: 0x001F8C0C File Offset: 0x001F6E0C
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

		// Token: 0x06003F34 RID: 16180 RVA: 0x001F8C84 File Offset: 0x001F6E84
		public void SetToggleVisible(ToggleIdIndex toggleIndex, bool isVisible)
		{
			int lineIndex = CommonSortAndFilter.GetIndexFromId(toggleIndex.LineId, this.Config);
			ISortAndFilterLine line = this.GetOrCreateLine(lineIndex);
			CommonFilterToggleGroup toggleGroupLine = line as CommonFilterToggleGroup;
			bool flag = toggleGroupLine != null;
			if (flag)
			{
				toggleGroupLine.SetToggleVisible(toggleIndex.ToggleKey, isVisible);
			}
			else
			{
				Debug.LogWarning(string.Format("[CommonSortAndFilter] SetToggleVisible failed, line {0} is not ToggleGroup", lineIndex));
			}
		}

		// Token: 0x06003F35 RID: 16181 RVA: 0x001F8CE8 File Offset: 0x001F6EE8
		public void SetToggleIsOn(ToggleIdIndex toggleIndex, bool isOn)
		{
			int lineIndex = CommonSortAndFilter.GetIndexFromId(toggleIndex.LineId, this.Config);
			ISortAndFilterLine line = this.GetOrCreateLine(lineIndex);
			CommonFilterToggleGroup toggleGroupLine = line as CommonFilterToggleGroup;
			bool flag = toggleGroupLine != null;
			if (flag)
			{
				toggleGroupLine.SetToggleIsOn(toggleIndex.ToggleKey, isOn);
			}
			else
			{
				Debug.LogWarning(string.Format("[CommonSortAndFilter] SetToggleIsOn failed, line {0} is not ToggleGroup", lineIndex));
			}
		}

		// Token: 0x06003F36 RID: 16182 RVA: 0x001F8D4C File Offset: 0x001F6F4C
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
			this.UpdateLineActive(CommonSortAndFilter.GetIndexFromId(lineId, this.Config));
		}

		// Token: 0x06003F37 RID: 16183 RVA: 0x001F8DF4 File Offset: 0x001F6FF4
		public void SetDropdownOptionInteractable(int lineId, int menuId, int optionIndex, bool interactable, string disabledTooltip)
		{
			int lineIndex = CommonSortAndFilter.GetIndexFromId(lineId, this.Config);
			ISortAndFilterLine line = this.GetOrCreateLine(lineIndex);
			CommonDetailedFilter detailedFilter = line as CommonDetailedFilter;
			bool flag = detailedFilter != null;
			if (flag)
			{
				detailedFilter.SetOptionInteractable(menuId, optionIndex, interactable, disabledTooltip);
			}
		}

		// Token: 0x06003F38 RID: 16184 RVA: 0x001F8E38 File Offset: 0x001F7038
		public void SetDropdownOption(int lineId, int menuId, int optionIndex)
		{
			int lineIndex = CommonSortAndFilter.GetIndexFromId(lineId, this.Config);
			ISortAndFilterLine line = this.GetOrCreateLine(lineIndex);
			CommonDetailedFilter detailedFilter = line as CommonDetailedFilter;
			bool flag = detailedFilter != null;
			if (flag)
			{
				detailedFilter.SetOption(menuId, optionIndex);
			}
			else
			{
				Debug.LogWarning(string.Format("[CommonSortAndFilter] SetDropdownOption failed, line {0} is not CommonDetailedFilter", lineIndex));
			}
		}

		// Token: 0x04002D3E RID: 11582
		private readonly Dictionary<int, bool> _activeCache = new Dictionary<int, bool>();

		// Token: 0x04002D3F RID: 11583
		private readonly Dictionary<int, ISortAndFilterLine> _lines = new Dictionary<int, ISortAndFilterLine>();

		// Token: 0x04002D40 RID: 11584
		private readonly Dictionary<int, HashSet<int>> _outsideMenuInvisibleDict = new Dictionary<int, HashSet<int>>();

		// Token: 0x04002D42 RID: 11586
		private readonly Dictionary<int, CommonFilterLineLayout> _lineLayouts = new Dictionary<int, CommonFilterLineLayout>();

		// Token: 0x04002D43 RID: 11587
		[Header("Templates")]
		[SerializeField]
		private CommonFilterToggleGroup toggleGroupLineTemplate;

		// Token: 0x04002D44 RID: 11588
		[SerializeField]
		private CommonDetailedFilter detailedFilterTemplate;

		// Token: 0x04002D45 RID: 11589
		[SerializeField]
		private CommonSecondaryFilter secondaryFilterTemplate;

		// Token: 0x04002D46 RID: 11590
		[SerializeField]
		private CommonFilterLineLayout filterLineLayoutTemplate;

		// Token: 0x04002D47 RID: 11591
		[Header("Roots")]
		[SerializeField]
		private RectTransform lineRoot;

		// Token: 0x04002D48 RID: 11592
		[Header("Components")]
		[SerializeField]
		private CommonSortDropdown sortDropdown;

		// Token: 0x04002D49 RID: 11593
		[SerializeField]
		private GameObject toolBar;

		// Token: 0x04002D4A RID: 11594
		[SerializeField]
		private CToggleObsolete customOrderSettingModeToggle;

		// Token: 0x04002D4B RID: 11595
		[SerializeField]
		private CButtonObsolete clearFilterButton;

		// Token: 0x04002D4C RID: 11596
		[SerializeField]
		private CButtonObsolete clearCustomOrderButton;

		// Token: 0x04002D4D RID: 11597
		public bool hideToolBar;

		// Token: 0x04002D4E RID: 11598
		public bool disableAutoHeight = true;
	}
}
