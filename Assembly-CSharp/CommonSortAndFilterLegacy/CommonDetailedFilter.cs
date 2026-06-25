using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000421 RID: 1057
	public class CommonDetailedFilter : MonoBehaviour, ISortAndFilterLine
	{
		// Token: 0x17000661 RID: 1633
		// (get) Token: 0x06003EA7 RID: 16039 RVA: 0x001F60A1 File Offset: 0x001F42A1
		public DetailedFilterConfig Config
		{
			get
			{
				return this._config;
			}
		}

		// Token: 0x06003EA8 RID: 16040 RVA: 0x001F60AC File Offset: 0x001F42AC
		public void Setup(DetailedFilterConfig config, Action<int> onMenuSelectionChange, Action onCustomOrderChange, UIElement parentElement, bool disableAutoHeight)
		{
			this._config = config;
			this._onMenuSelectionChange = onMenuSelectionChange;
			this._onCustomOrderChange = onCustomOrderChange;
			this._dropdowns.Clear();
			this._dropdownDict.Clear();
			CommonUtils.PrepareEnoughChildren(this.dropdownRoot.transform, this.dropdownTemplate.gameObject, config.MenuConfigs.Count, null);
			this._detailedFilterMenuBarDragHandler = new DetailedFilterMenuBarDragHandler(this.scrollRect, this.dropdownRoot, this.dropdownLayoutMask, this._dropdowns, delegate()
			{
				this.UpdateMenuBarSwapTogglesVisibility(false);
				Action onCustomOrderChange2 = this._onCustomOrderChange;
				if (onCustomOrderChange2 != null)
				{
					onCustomOrderChange2();
				}
			}, delegate(int index)
			{
				this.UpdateMenuBarSwapTogglesVisibility(true);
				this._dropdowns[index].RefreshMenuBarSwapToggle(true, true);
			});
			for (int i = 0; i < config.MenuConfigs.Count; i++)
			{
				DetailFilterMultiSelectDropdown dropdown = this.GetDropdownRaw(i);
				dropdown.ParentElement = parentElement;
				dropdown.gameObject.name = string.Format("dropdown_{0}_{1}", i, config.MenuConfigs[i].Id);
				dropdown.Setup(config.MenuConfigs[i]);
				int ii = i;
				DetailFilterMultiSelectDropdown dropdown2 = dropdown;
				dropdown2.OnSelectionChanged = (Action)Delegate.Combine(dropdown2.OnSelectionChanged, new Action(delegate()
				{
					this.OnMenuSelectionChange(ii);
				}));
				bool flag = this.scrollRect;
				if (flag)
				{
					dropdown.SetupMenuBarHoldDrag(this.scrollRect.gameObject, this.scrollRect.GetComponent<RectTransform>(), delegate(GameObject go, PointerEventData eventData)
					{
						bool flag2 = eventData != null && eventData.button == PointerEventData.InputButton.Left;
						if (flag2)
						{
							this._detailedFilterMenuBarDragHandler.OnMenuBarPointerDown(dropdown);
						}
					}, delegate(GameObject go, PointerEventData eventData)
					{
						this._detailedFilterMenuBarDragHandler.OnMenuBarPointerUp(eventData);
					}, delegate(GameObject _, PointerEventData _)
					{
						this._detailedFilterMenuBarDragHandler.OnMenuBarBeginDrag();
					}, delegate(GameObject _, PointerEventData _)
					{
						this._detailedFilterMenuBarDragHandler.OnMenuBarEndDrag();
					}, delegate(GameObject _, PointerEventData _)
					{
						this._detailedFilterMenuBarDragHandler.OnMenuBarDrag();
					});
				}
				this.SetupDropdownScrollInfo(dropdown);
				dropdown.SetupOptionsHoldDragHandler(onCustomOrderChange);
				dropdown.SetupSwapToggle(delegate
				{
					this.OnMenuBarSwapToggleClicked(dropdown);
				});
				dropdown.OnMouseEnter = new Action(this.OnDropdownMouseEnter);
				dropdown.OnMouseExit = new Action(this.OnDropdownMouseExit);
				dropdown.SetupCanShowMenuChecker(new DetailFilterMultiSelectDropdown.CanShowMenuChecker(this.CanDropdownShowMenu));
				dropdown.DisableAutoHeight = disableAutoHeight;
				this._dropdowns.Add(dropdown);
				this._dropdownDict[config.MenuConfigs[i].Id] = dropdown;
			}
			this.SetLeftAndRightButtonsVisible(false);
		}

		// Token: 0x06003EA9 RID: 16041 RVA: 0x001F634C File Offset: 0x001F454C
		protected virtual void SetupDropdownScrollInfo(DetailFilterMultiSelectDropdown dropdown)
		{
			dropdown.SetupParentScrollView(this.dropdownRoot.GetComponent<RectTransform>(), this.scrollRect);
		}

		// Token: 0x06003EAA RID: 16042 RVA: 0x001F6368 File Offset: 0x001F4568
		private bool CanDropdownShowMenu()
		{
			return !this._detailedFilterMenuBarDragHandler.IsSettingMode || !this._detailedFilterMenuBarDragHandler.IsWaitingForSecondMenuSelection();
		}

		// Token: 0x06003EAB RID: 16043 RVA: 0x001F6398 File Offset: 0x001F4598
		private void OnDropdownMouseEnter()
		{
		}

		// Token: 0x06003EAC RID: 16044 RVA: 0x001F639C File Offset: 0x001F459C
		private void OnDropdownMouseExit()
		{
			bool flag = !this._detailedFilterMenuBarDragHandler.IsWaitingForSecondMenuSelection();
			if (flag)
			{
				this.UpdateMenuBarSwapTogglesVisibility(false);
			}
		}

		// Token: 0x06003EAD RID: 16045 RVA: 0x001F63C4 File Offset: 0x001F45C4
		protected virtual void SetLeftAndRightButtonsVisible(bool showLeftRightButton)
		{
			this.leftButton.gameObject.SetActive(showLeftRightButton);
			this.rightButton.gameObject.SetActive(showLeftRightButton);
			RectTransform rect = this.dropdownLayoutMask.GetComponent<RectTransform>();
			if (showLeftRightButton)
			{
				rect.offsetMin = new Vector2(58f, rect.offsetMin.y);
				rect.offsetMax = new Vector2(-58f, rect.offsetMax.y);
			}
			else
			{
				rect.offsetMin = new Vector2(0f, rect.offsetMin.y);
				rect.offsetMax = new Vector2(0f, rect.offsetMax.y);
			}
		}

		// Token: 0x06003EAE RID: 16046 RVA: 0x001F6480 File Offset: 0x001F4680
		public DetailedFilterState GetState()
		{
			Dictionary<int, DetailedFilterMenuState> menuSelections = new Dictionary<int, DetailedFilterMenuState>();
			for (int menuIndex = 0; menuIndex < this._config.MenuConfigs.Count; menuIndex++)
			{
				int menuId = this._config.MenuConfigs[menuIndex].Id;
				DetailFilterMultiSelectDropdown dropdown = this.GetDropdownById(menuId);
				bool isActive = dropdown.gameObject.activeSelf;
				menuSelections[menuId] = new DetailedFilterMenuState(dropdown.GetSelectedIndices(), isActive);
			}
			return new DetailedFilterState
			{
				MenuStateDict = menuSelections
			};
		}

		// Token: 0x06003EAF RID: 16047 RVA: 0x001F6510 File Offset: 0x001F4710
		private void OnMenuSelectionChange(int menuIndex)
		{
			Action<int> onMenuSelectionChange = this._onMenuSelectionChange;
			if (onMenuSelectionChange != null)
			{
				onMenuSelectionChange(menuIndex);
			}
		}

		// Token: 0x06003EB0 RID: 16048 RVA: 0x001F6528 File Offset: 0x001F4728
		private DetailFilterMultiSelectDropdown GetDropdownRaw(int menuIndex)
		{
			return this.dropdownRoot.transform.GetChild(menuIndex).GetComponent<DetailFilterMultiSelectDropdown>();
		}

		// Token: 0x06003EB1 RID: 16049 RVA: 0x001F6550 File Offset: 0x001F4750
		private DetailFilterMultiSelectDropdown GetDropdown(int menuIndex)
		{
			return this._dropdowns[menuIndex];
		}

		// Token: 0x06003EB2 RID: 16050 RVA: 0x001F6570 File Offset: 0x001F4770
		private DetailFilterMultiSelectDropdown GetDropdownById(int menuId)
		{
			DetailFilterMultiSelectDropdown dropdown;
			bool flag = !this._dropdownDict.TryGetValue(menuId, out dropdown);
			if (flag)
			{
				throw new KeyNotFoundException(string.Format("Dropdown with id {0} not found.", menuId));
			}
			return dropdown;
		}

		// Token: 0x06003EB3 RID: 16051 RVA: 0x001F65B0 File Offset: 0x001F47B0
		public void UpdateMenuVisibility(DetailedFilterState myState, HashSet<int> menuInvisibleSet)
		{
			for (int i = 0; i < this._config.MenuConfigs.Count; i++)
			{
				MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig> menuConfig = this._config.MenuConfigs[i];
				int menuId = menuConfig.Id;
				MenuOptionIndex? dependency = menuConfig.Dependency;
				bool flag = dependency == null;
				if (!flag)
				{
					DetailedFilterMenuState dependentMenuState = myState.MenuStateDict[dependency.Value.MenuId];
					bool isInvisibleByOutside = menuInvisibleSet != null && menuInvisibleSet.Contains(menuId);
					bool isActive = dependentMenuState.IsActive && !isInvisibleByOutside;
					bool flag2 = !isActive;
					if (flag2)
					{
						this.GetDropdownById(menuId).gameObject.SetActive(false);
					}
					else
					{
						bool meActive = dependentMenuState.SelectedIndices.Contains(dependency.Value.OptionIndex);
						this.GetDropdownById(menuId).gameObject.SetActive(meActive);
					}
				}
			}
		}

		// Token: 0x06003EB4 RID: 16052 RVA: 0x001F66A6 File Offset: 0x001F48A6
		private void Update()
		{
			DetailedFilterMenuBarDragHandler detailedFilterMenuBarDragHandler = this._detailedFilterMenuBarDragHandler;
			if (detailedFilterMenuBarDragHandler != null)
			{
				detailedFilterMenuBarDragHandler.TickHoldDrag();
			}
			this.TickLeftRightButtonVisible();
		}

		// Token: 0x06003EB5 RID: 16053 RVA: 0x001F66C4 File Offset: 0x001F48C4
		protected virtual void TickLeftRightButtonVisible()
		{
			float layoutWidth = this.dropdownRoot.GetComponent<RectTransform>().rect.width;
			float maskWidth = this.dropdownLayoutMask.rect.width;
			bool flag = Mathf.Approximately(layoutWidth, this._lastLayoutAreaWidth) && Mathf.Approximately(maskWidth, this._lastLayoutMaskAreaWidth);
			if (!flag)
			{
				bool showLeftRightButton = layoutWidth > maskWidth;
				this.SetLeftAndRightButtonsVisible(showLeftRightButton);
				this._lastLayoutAreaWidth = layoutWidth;
				this._lastLayoutMaskAreaWidth = maskWidth;
			}
		}

		// Token: 0x06003EB6 RID: 16054 RVA: 0x001F6740 File Offset: 0x001F4940
		public virtual LineState GetLineState()
		{
			return new LineState
			{
				IsActive = this.IsActive(),
				Type = ESortAndFilterOneLineType.DetailedFilter,
				DetailedFilterState = new DetailedFilterLineState
				{
					State = this.GetState()
				}
			};
		}

		// Token: 0x06003EB7 RID: 16055 RVA: 0x001F6788 File Offset: 0x001F4988
		public void SetActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06003EB8 RID: 16056 RVA: 0x001F6798 File Offset: 0x001F4998
		public bool IsActive()
		{
			return base.gameObject.activeSelf;
		}

		// Token: 0x06003EB9 RID: 16057 RVA: 0x001F67B8 File Offset: 0x001F49B8
		public void ApplyCustomOrder(IFilterLineCustomOrderData orderData)
		{
			DetailedLineCustomOrderData customOrderData;
			bool flag;
			if (orderData is DetailedLineCustomOrderData)
			{
				customOrderData = (DetailedLineCustomOrderData)orderData;
				flag = (1 == 0);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (flag2)
			{
				throw new ArgumentException("Invalid order data for DetailedFilter");
			}
			List<int> menuOrders = customOrderData.MenuOrderList;
			this._dropdowns.Clear();
			HashSet<int> usedMenuIds = new HashSet<int>();
			foreach (int menuId in menuOrders)
			{
				DetailFilterMultiSelectDropdown dropdown4;
				bool flag3 = !this._dropdownDict.TryGetValue(menuId, out dropdown4);
				if (!flag3)
				{
					this._dropdowns.Add(dropdown4);
					usedMenuIds.Add(menuId);
				}
			}
			foreach (KeyValuePair<int, DetailFilterMultiSelectDropdown> pair in this._dropdownDict)
			{
				bool flag4 = !usedMenuIds.Contains(pair.Key);
				if (flag4)
				{
					this._dropdowns.Add(pair.Value);
				}
			}
			this.ApplyOptionOrder(customOrderData.MenuOptionsOrderDict);
			Dictionary<DetailFilterMultiSelectDropdown, int> groupIndexMap = new Dictionary<DetailFilterMultiSelectDropdown, int>();
			int currentGroupIndex = -1;
			foreach (DetailFilterMultiSelectDropdown dropdown2 in this._dropdowns)
			{
				bool flag5 = dropdown2.Config.Dependency == null;
				if (flag5)
				{
					currentGroupIndex++;
				}
				bool flag6 = currentGroupIndex != -1;
				if (flag6)
				{
					groupIndexMap[dropdown2] = currentGroupIndex;
				}
			}
			int[] orderArr = new int[this._dropdowns.Count];
			for (int i = 0; i < this._dropdowns.Count; i++)
			{
				MenuOptionIndex? dep = this._dropdowns[i].Config.Dependency;
				bool flag7 = dep != null && this._dropdownDict.ContainsKey(dep.Value.MenuId);
				if (flag7)
				{
					orderArr[i] = this._dropdownDict[dep.Value.MenuId].GetOptionOrder(dep.Value.OptionIndex);
				}
				else
				{
					orderArr[i] = ((dep == null) ? 0 : int.MaxValue);
				}
			}
			List<DetailFilterMultiSelectDropdown> temp = (from item in this._dropdowns.Select((DetailFilterMultiSelectDropdown dropdown, int index) => new
			{
				Dropdown = dropdown,
				GroupIndex = groupIndexMap.GetValueOrDefault(dropdown, int.MaxValue),
				DependencyOrder = orderArr[index]
			})
			orderby item.GroupIndex, item.DependencyOrder
			select item.Dropdown).ToList<DetailFilterMultiSelectDropdown>();
			this._dropdowns.Clear();
			this._dropdowns.AddRange(temp);
			for (int j = 0; j < this._dropdowns.Count; j++)
			{
				DetailFilterMultiSelectDropdown dropdown3 = this._dropdowns[j];
				dropdown3.transform.SetSiblingIndex(j);
			}
		}

		// Token: 0x06003EBA RID: 16058 RVA: 0x001F6B44 File Offset: 0x001F4D44
		public void ResetCustomOrder()
		{
			List<MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig>> menuConfigs = this.Config.MenuConfigs;
			this._dropdowns.Clear();
			foreach (MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig> menuConfig in from m in menuConfigs
			orderby m.Id
			select m)
			{
				DetailFilterMultiSelectDropdown dropdown = this._dropdownDict[menuConfig.Id];
				this._dropdowns.Add(dropdown);
			}
			for (int i = 0; i < this._dropdowns.Count; i++)
			{
				DetailFilterMultiSelectDropdown dropdown2 = this._dropdowns[i];
				dropdown2.transform.SetSiblingIndex(i);
			}
			this.ResetOptionCustomOrder();
		}

		// Token: 0x06003EBB RID: 16059 RVA: 0x001F6C2C File Offset: 0x001F4E2C
		private void ApplyOptionOrder(Dictionary<int, MenuOptionsCustomOrderData> menuOptionsOrderDict)
		{
			foreach (KeyValuePair<int, MenuOptionsCustomOrderData> keyValuePair in menuOptionsOrderDict)
			{
				int num;
				MenuOptionsCustomOrderData menuOptionsCustomOrderData;
				keyValuePair.Deconstruct(out num, out menuOptionsCustomOrderData);
				int menuId = num;
				MenuOptionsCustomOrderData orderData = menuOptionsCustomOrderData;
				DetailFilterMultiSelectDropdown dropdown;
				bool flag = !this._dropdownDict.TryGetValue(menuId, out dropdown);
				if (flag)
				{
					Debug.LogWarning(string.Format("Dropdown with id {0} not found for custom order.", menuId));
				}
				else
				{
					dropdown.ApplyOptionCustomOrder(orderData);
				}
			}
		}

		// Token: 0x06003EBC RID: 16060 RVA: 0x001F6CC4 File Offset: 0x001F4EC4
		private void ResetOptionCustomOrder()
		{
			foreach (DetailFilterMultiSelectDropdown dropdown in this._dropdowns)
			{
				dropdown.ResetOptionCustomOrder();
			}
		}

		// Token: 0x06003EBD RID: 16061 RVA: 0x001F6D1C File Offset: 0x001F4F1C
		public IFilterLineCustomOrderData GetCustomOrderData()
		{
			List<int> menuOrders = new List<int>();
			Dictionary<int, MenuOptionsCustomOrderData> menuOptionsOrderDict = new Dictionary<int, MenuOptionsCustomOrderData>();
			using (List<DetailFilterMultiSelectDropdown>.Enumerator enumerator = this._dropdowns.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DetailFilterMultiSelectDropdown dropdown = enumerator.Current;
					int id = this._dropdownDict.FirstOrDefault((KeyValuePair<int, DetailFilterMultiSelectDropdown> pair) => pair.Value == dropdown).Key;
					menuOrders.Add(id);
					MenuOptionsCustomOrderData? optionsOrderData = dropdown.GetOptionCustomOrderData();
					bool flag = optionsOrderData != null;
					if (flag)
					{
						menuOptionsOrderDict[id] = optionsOrderData.Value;
					}
				}
			}
			return new DetailedLineCustomOrderData
			{
				MenuOrderList = menuOrders,
				MenuOptionsOrderDict = menuOptionsOrderDict
			};
		}

		// Token: 0x06003EBE RID: 16062 RVA: 0x001F6E00 File Offset: 0x001F5000
		public void OnSwitchCustomOrderSettingMode(bool isSettingMode)
		{
			this._detailedFilterMenuBarDragHandler.SetSettingMode(isSettingMode);
			foreach (DetailFilterMultiSelectDropdown dropdown in this._dropdowns)
			{
				dropdown.OnSwitchCustomOrderSettingMode(isSettingMode);
			}
			bool flag = !isSettingMode;
			if (flag)
			{
				this.UpdateMenuBarSwapTogglesVisibility(false);
			}
		}

		// Token: 0x06003EBF RID: 16063 RVA: 0x001F6E78 File Offset: 0x001F5078
		private void UpdateMenuBarSwapTogglesVisibility(bool isVisible)
		{
			foreach (DetailFilterMultiSelectDropdown dropdown in this._dropdowns)
			{
				dropdown.UpdateMenuBarSwapToggleVisibility(isVisible);
			}
		}

		// Token: 0x06003EC0 RID: 16064 RVA: 0x001F6ED4 File Offset: 0x001F50D4
		private void OnMenuBarSwapToggleClicked(DetailFilterMultiSelectDropdown dropdown)
		{
			bool flag = this._detailedFilterMenuBarDragHandler == null;
			if (!flag)
			{
				int menuIndex = this._dropdowns.IndexOf(dropdown);
				this._detailedFilterMenuBarDragHandler.HandleMenuBarToggleClick(menuIndex);
			}
		}

		// Token: 0x06003EC1 RID: 16065 RVA: 0x001F6F0C File Offset: 0x001F510C
		public void ClearAllFilter()
		{
			foreach (DetailFilterMultiSelectDropdown dropdown in this._dropdowns)
			{
				dropdown.UnSelectAll(false);
			}
		}

		// Token: 0x06003EC2 RID: 16066 RVA: 0x001F6F68 File Offset: 0x001F5168
		public void ApplyDynamicConfig(DynamicLineConfig dynamicConfig)
		{
			bool flag = dynamicConfig.Type != ESortAndFilterOneLineType.DetailedFilter;
			if (!flag)
			{
				DynamicDetailedFilterLineConfig config = dynamicConfig.DetailedFilterLineConfig;
				bool flag2 = config == null;
				if (flag2)
				{
					throw new ArgumentException("Dynamic config for DetailedFilterLine is null.");
				}
				foreach (KeyValuePair<int, MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig>> keyValuePair in config.ChangedMenuConfigs)
				{
					int num;
					MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig> multiSelectDropdownConfig;
					keyValuePair.Deconstruct(out num, out multiSelectDropdownConfig);
					int id = num;
					MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig> menuConfig = multiSelectDropdownConfig;
					DetailFilterMultiSelectDropdown dropdown;
					bool flag3 = this._dropdownDict.TryGetValue(id, out dropdown);
					if (flag3)
					{
						dropdown.Setup(menuConfig);
					}
					else
					{
						Debug.LogWarning(string.Format("Dropdown with id {0} not found for dynamic config.", menuConfig.Id));
					}
				}
			}
		}

		// Token: 0x06003EC3 RID: 16067 RVA: 0x001F7040 File Offset: 0x001F5240
		public void SetOptionInteractable(int menuId, int optionIndex, bool interactable, string disabledTooltip)
		{
			DetailFilterMultiSelectDropdown dropdown;
			bool flag = this._dropdownDict.TryGetValue(menuId, out dropdown);
			if (flag)
			{
				dropdown.SetOptionInteractable(optionIndex, interactable, disabledTooltip);
			}
		}

		// Token: 0x06003EC4 RID: 16068 RVA: 0x001F7070 File Offset: 0x001F5270
		public void SetOption(int menuId, int optionIndex)
		{
			DetailFilterMultiSelectDropdown dropdown;
			bool flag = this._dropdownDict.TryGetValue(menuId, out dropdown);
			if (flag)
			{
				dropdown.SetOption(optionIndex);
			}
		}

		// Token: 0x06003EC5 RID: 16069 RVA: 0x001F709C File Offset: 0x001F529C
		public void Awake()
		{
			bool flag = this.leftButton != null;
			if (flag)
			{
				this.leftButton.ClearAndAddListener(new Action(this.OnLeftButtonClick));
			}
			bool flag2 = this.rightButton != null;
			if (flag2)
			{
				this.rightButton.ClearAndAddListener(new Action(this.OnRightButtonClick));
			}
		}

		// Token: 0x06003EC6 RID: 16070 RVA: 0x001F70FC File Offset: 0x001F52FC
		private void OnDisable()
		{
			this.UpdateMenuBarSwapTogglesVisibility(false);
			DetailedFilterMenuBarDragHandler detailedFilterMenuBarDragHandler = this._detailedFilterMenuBarDragHandler;
			if (detailedFilterMenuBarDragHandler != null)
			{
				detailedFilterMenuBarDragHandler.ForceResetState();
			}
			DetailedFilterMenuBarDragHandler detailedFilterMenuBarDragHandler2 = this._detailedFilterMenuBarDragHandler;
			if (detailedFilterMenuBarDragHandler2 != null)
			{
				detailedFilterMenuBarDragHandler2.SetSettingMode(false);
			}
			bool flag = this._dropdowns != null;
			if (flag)
			{
				foreach (DetailFilterMultiSelectDropdown dd in this._dropdowns)
				{
					if (dd != null)
					{
						dd.ForceResetOptionDragState();
					}
					if (dd != null)
					{
						dd.OnSwitchCustomOrderSettingMode(false);
					}
				}
			}
		}

		// Token: 0x06003EC7 RID: 16071 RVA: 0x001F71A4 File Offset: 0x001F53A4
		private void OnLeftButtonClick()
		{
			this.ScrollByMenuWidth(-1);
		}

		// Token: 0x06003EC8 RID: 16072 RVA: 0x001F71AF File Offset: 0x001F53AF
		private void OnRightButtonClick()
		{
			this.ScrollByMenuWidth(1);
		}

		// Token: 0x06003EC9 RID: 16073 RVA: 0x001F71BC File Offset: 0x001F53BC
		private void ScrollByMenuWidth(int direction)
		{
			bool flag = this._dropdowns.Count == 0 || this.scrollRect == null || this.dropdownRoot == null;
			if (!flag)
			{
				RectTransform first = this._dropdowns[0].GetComponent<RectTransform>();
				float menuWidth = first.rect.width;
				float spacing = this.dropdownRoot.spacing;
				float scrollAmount = menuWidth + spacing;
				float maskWidth = this.dropdownLayoutMask.rect.width;
				float contentWidth = this.dropdownRoot.GetComponent<RectTransform>().rect.width;
				float normalizedStep = scrollAmount / (contentWidth - maskWidth);
				float target = this.scrollRect.horizontalNormalizedPosition + (float)direction * normalizedStep;
				target = Mathf.Clamp01(target);
				bool flag2 = this._scrollCoroutine != null;
				if (flag2)
				{
					base.StopCoroutine(this._scrollCoroutine);
				}
				this._scrollCoroutine = base.StartCoroutine(this.SmoothScrollTo(target));
			}
		}

		// Token: 0x06003ECA RID: 16074 RVA: 0x001F72B7 File Offset: 0x001F54B7
		private IEnumerator SmoothScrollTo(float target)
		{
			float elapsed = 0f;
			float start = this.scrollRect.horizontalNormalizedPosition;
			while (elapsed < 0.2f)
			{
				elapsed += Time.unscaledDeltaTime;
				float t = Mathf.Clamp01(elapsed / 0.2f);
				this.scrollRect.horizontalNormalizedPosition = Mathf.Lerp(start, target, t);
				yield return null;
			}
			this.scrollRect.horizontalNormalizedPosition = target;
			yield break;
		}

		// Token: 0x04002D0C RID: 11532
		private DetailedFilterConfig _config;

		// Token: 0x04002D0D RID: 11533
		private Action<int> _onMenuSelectionChange;

		// Token: 0x04002D0E RID: 11534
		private Action _onCustomOrderChange;

		// Token: 0x04002D0F RID: 11535
		private readonly List<DetailFilterMultiSelectDropdown> _dropdowns = new List<DetailFilterMultiSelectDropdown>();

		// Token: 0x04002D10 RID: 11536
		private readonly Dictionary<int, DetailFilterMultiSelectDropdown> _dropdownDict = new Dictionary<int, DetailFilterMultiSelectDropdown>();

		// Token: 0x04002D11 RID: 11537
		private DetailedFilterMenuBarDragHandler _detailedFilterMenuBarDragHandler;

		// Token: 0x04002D12 RID: 11538
		private float _lastLayoutAreaWidth;

		// Token: 0x04002D13 RID: 11539
		private float _lastLayoutMaskAreaWidth;

		// Token: 0x04002D14 RID: 11540
		[SerializeField]
		private CButtonObsolete leftButton;

		// Token: 0x04002D15 RID: 11541
		[SerializeField]
		private CButtonObsolete rightButton;

		// Token: 0x04002D16 RID: 11542
		[SerializeField]
		private HorizontalLayoutGroup dropdownRoot;

		// Token: 0x04002D17 RID: 11543
		[SerializeField]
		private DetailFilterMultiSelectDropdown dropdownTemplate;

		// Token: 0x04002D18 RID: 11544
		[SerializeField]
		private RectTransform dropdownLayoutMask;

		// Token: 0x04002D19 RID: 11545
		[SerializeField]
		private ScrollRect scrollRect;

		// Token: 0x04002D1A RID: 11546
		private Coroutine _scrollCoroutine;
	}
}
