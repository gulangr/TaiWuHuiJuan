using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000C8D RID: 3213
	public class DetailedFilterLine : MonoBehaviour, ISortAndFilterLine
	{
		// Token: 0x17001116 RID: 4374
		// (get) Token: 0x0600A3BC RID: 41916 RVA: 0x004C9449 File Offset: 0x004C7649
		public DetailedFilterConfig Config
		{
			get
			{
				return this._config;
			}
		}

		// Token: 0x0600A3BD RID: 41917 RVA: 0x004C9454 File Offset: 0x004C7654
		public void Setup(DetailedFilterConfig config, Action<int> onMenuSelectionChange)
		{
			this._config = config;
			this._onMenuSelectionChange = onMenuSelectionChange;
			this._dropdowns.Clear();
			this._dropdownDict.Clear();
			CommonUtils.PrepareEnoughChildren(this.dropdownRoot.transform, this.dropdownTemplate.gameObject, config.MenuConfigs.Count, null);
			for (int i = 0; i < config.MenuConfigs.Count; i++)
			{
				DetailedFilterMenuConfig menuConfig = config.MenuConfigs[i];
				FilterDropdown dropdown = this.GetDropdownRaw(i);
				dropdown.gameObject.name = string.Format("dropdown_{0}_{1}", i, menuConfig.Id);
				dropdown.Setup(menuConfig.DropdownConfig, menuConfig.DropdownContext);
				int ii = i;
				FilterDropdown filterDropdown = dropdown;
				filterDropdown.OnSelectionChanged = (Action)Delegate.Combine(filterDropdown.OnSelectionChanged, new Action(delegate()
				{
					this.OnMenuSelectionChange(ii);
				}));
				this.SetupDropdownScrollInfo(dropdown);
				dropdown.OnMouseEnter = new Action(this.OnDropdownMouseEnter);
				dropdown.OnMouseExit = new Action(this.OnDropdownMouseExit);
				this._dropdowns.Add(dropdown);
				this._dropdownDict[menuConfig.Id] = dropdown;
			}
			this.SetLeftAndRightButtonsVisible(false);
		}

		// Token: 0x0600A3BE RID: 41918 RVA: 0x004C95C0 File Offset: 0x004C77C0
		public DetailedFilterState GetState()
		{
			Dictionary<int, DetailedFilterMenuState> menuSelections = new Dictionary<int, DetailedFilterMenuState>();
			for (int menuIndex = 0; menuIndex < this._config.MenuConfigs.Count; menuIndex++)
			{
				int menuId = this._config.MenuConfigs[menuIndex].Id;
				FilterDropdown dropdown = this.GetDropdownById(menuId);
				bool isActive = dropdown.gameObject.activeSelf;
				menuSelections[menuId] = new DetailedFilterMenuState(dropdown.GetSelectedIndices(), isActive);
			}
			return new DetailedFilterState
			{
				MenuStateDict = menuSelections
			};
		}

		// Token: 0x0600A3BF RID: 41919 RVA: 0x004C9650 File Offset: 0x004C7850
		public void UpdateMenuVisibility(DetailedFilterState myState, HashSet<int> menuInvisibleSet)
		{
			foreach (DetailedFilterMenuConfig menuConfig in this._config.MenuConfigs)
			{
				int menuId = menuConfig.Id;
				bool isInvisibleByOutside = menuInvisibleSet != null && menuInvisibleSet.Contains(menuId);
				bool flag = isInvisibleByOutside;
				if (flag)
				{
					this.GetDropdownById(menuId).gameObject.SetActive(false);
				}
				else
				{
					MenuOptionIndex? dependency = menuConfig.DropdownContext.Dependency;
					bool flag2 = dependency == null;
					if (flag2)
					{
						this.GetDropdownById(menuId).gameObject.SetActive(true);
					}
					else
					{
						DetailedFilterMenuState dependentMenuState = myState.MenuStateDict[dependency.Value.MenuId];
						bool isActive = dependentMenuState.IsActive;
						bool flag3 = !isActive;
						if (flag3)
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
		}

		// Token: 0x0600A3C0 RID: 41920 RVA: 0x004C9788 File Offset: 0x004C7988
		public void ClearAllFilter()
		{
			foreach (FilterDropdown dropdown in this._dropdowns)
			{
				dropdown.UnSelectAll(false);
			}
		}

		// Token: 0x0600A3C1 RID: 41921 RVA: 0x004C97E4 File Offset: 0x004C79E4
		public void SetOptionInteractable(int menuId, int optionIndex, bool interactable, string disabledTooltip)
		{
			FilterDropdown dropdown;
			bool flag = this._dropdownDict.TryGetValue(menuId, out dropdown);
			if (flag)
			{
				dropdown.SetOptionInteractable(optionIndex, interactable, disabledTooltip);
			}
		}

		// Token: 0x0600A3C2 RID: 41922 RVA: 0x004C9814 File Offset: 0x004C7A14
		public void SetOption(int menuId, int optionIndex)
		{
			FilterDropdown dropdown;
			bool flag = this._dropdownDict.TryGetValue(menuId, out dropdown);
			if (flag)
			{
				dropdown.SetOption(optionIndex);
			}
		}

		// Token: 0x0600A3C3 RID: 41923 RVA: 0x004C9840 File Offset: 0x004C7A40
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

		// Token: 0x0600A3C4 RID: 41924 RVA: 0x004C9888 File Offset: 0x004C7A88
		public void SetActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x0600A3C5 RID: 41925 RVA: 0x004C9898 File Offset: 0x004C7A98
		public bool IsActive()
		{
			return base.gameObject.activeSelf;
		}

		// Token: 0x0600A3C6 RID: 41926 RVA: 0x004C98B8 File Offset: 0x004C7AB8
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
				foreach (KeyValuePair<int, FilterDropdownConfig> keyValuePair in config.ChangedMenuConfigs)
				{
					int num;
					FilterDropdownConfig filterDropdownConfig;
					keyValuePair.Deconstruct(out num, out filterDropdownConfig);
					int id = num;
					FilterDropdownConfig menuConfig = filterDropdownConfig;
					FilterDropdown dropdown;
					bool flag3 = this._dropdownDict.TryGetValue(id, out dropdown);
					if (flag3)
					{
						dropdown.Setup(menuConfig);
					}
					else
					{
						Debug.LogWarning(string.Format("Dropdown with id {0} not found for dynamic config.", menuConfig.MenuBarLabel));
					}
				}
			}
		}

		// Token: 0x0600A3C7 RID: 41927 RVA: 0x004C9990 File Offset: 0x004C7B90
		private void Awake()
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

		// Token: 0x0600A3C8 RID: 41928 RVA: 0x004C99EE File Offset: 0x004C7BEE
		private void Update()
		{
			this.TickLeftRightButtonVisible();
		}

		// Token: 0x0600A3C9 RID: 41929 RVA: 0x004C99F8 File Offset: 0x004C7BF8
		private void OnDisable()
		{
		}

		// Token: 0x0600A3CA RID: 41930 RVA: 0x004C99FB File Offset: 0x004C7BFB
		protected virtual void SetupDropdownScrollInfo(FilterDropdown dropdown)
		{
			dropdown.SetupParentScrollView(this.dropdownRoot.GetComponent<RectTransform>(), this.scrollRect);
		}

		// Token: 0x0600A3CB RID: 41931 RVA: 0x004C9A16 File Offset: 0x004C7C16
		private void OnDropdownMouseEnter()
		{
		}

		// Token: 0x0600A3CC RID: 41932 RVA: 0x004C9A19 File Offset: 0x004C7C19
		private void OnDropdownMouseExit()
		{
		}

		// Token: 0x0600A3CD RID: 41933 RVA: 0x004C9A1C File Offset: 0x004C7C1C
		protected virtual void SetLeftAndRightButtonsVisible(bool showLeftRightButton)
		{
			this.leftButton.gameObject.SetActive(showLeftRightButton);
			this.rightButton.gameObject.SetActive(showLeftRightButton);
			RectTransform rect = this.dropdownLayoutMask.GetComponent<RectTransform>();
			if (showLeftRightButton)
			{
				rect.offsetMin = new Vector2(76f, rect.offsetMin.y);
				rect.offsetMax = new Vector2(-76f, rect.offsetMax.y);
			}
			else
			{
				rect.offsetMin = new Vector2(0f, rect.offsetMin.y);
				rect.offsetMax = new Vector2(0f, rect.offsetMax.y);
			}
		}

		// Token: 0x0600A3CE RID: 41934 RVA: 0x004C9AD5 File Offset: 0x004C7CD5
		private void OnMenuSelectionChange(int menuIndex)
		{
			Action<int> onMenuSelectionChange = this._onMenuSelectionChange;
			if (onMenuSelectionChange != null)
			{
				onMenuSelectionChange(menuIndex);
			}
		}

		// Token: 0x0600A3CF RID: 41935 RVA: 0x004C9AEC File Offset: 0x004C7CEC
		private FilterDropdown GetDropdownRaw(int menuIndex)
		{
			return this.dropdownRoot.transform.GetChild(menuIndex).GetComponent<FilterDropdown>();
		}

		// Token: 0x0600A3D0 RID: 41936 RVA: 0x004C9B14 File Offset: 0x004C7D14
		private FilterDropdown GetDropdownById(int menuId)
		{
			FilterDropdown dropdown;
			bool flag = !this._dropdownDict.TryGetValue(menuId, out dropdown);
			if (flag)
			{
				throw new KeyNotFoundException(string.Format("Dropdown with id {0} not found.", menuId));
			}
			return dropdown;
		}

		// Token: 0x0600A3D1 RID: 41937 RVA: 0x004C9B54 File Offset: 0x004C7D54
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

		// Token: 0x0600A3D2 RID: 41938 RVA: 0x004C9BCF File Offset: 0x004C7DCF
		private void OnLeftButtonClick()
		{
			this.ScrollByMenuWidth(-1);
		}

		// Token: 0x0600A3D3 RID: 41939 RVA: 0x004C9BDA File Offset: 0x004C7DDA
		private void OnRightButtonClick()
		{
			this.ScrollByMenuWidth(1);
		}

		// Token: 0x0600A3D4 RID: 41940 RVA: 0x004C9BE8 File Offset: 0x004C7DE8
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

		// Token: 0x0600A3D5 RID: 41941 RVA: 0x004C9CE3 File Offset: 0x004C7EE3
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

		// Token: 0x040081C1 RID: 33217
		private DetailedFilterConfig _config;

		// Token: 0x040081C2 RID: 33218
		private Action<int> _onMenuSelectionChange;

		// Token: 0x040081C3 RID: 33219
		private readonly List<FilterDropdown> _dropdowns = new List<FilterDropdown>();

		// Token: 0x040081C4 RID: 33220
		private readonly Dictionary<int, FilterDropdown> _dropdownDict = new Dictionary<int, FilterDropdown>();

		// Token: 0x040081C5 RID: 33221
		private float _lastLayoutAreaWidth;

		// Token: 0x040081C6 RID: 33222
		private float _lastLayoutMaskAreaWidth;

		// Token: 0x040081C7 RID: 33223
		private const float LeftRightButtonWidth = 76f;

		// Token: 0x040081C8 RID: 33224
		private Coroutine _scrollCoroutine;

		// Token: 0x040081C9 RID: 33225
		[SerializeField]
		private CButton leftButton;

		// Token: 0x040081CA RID: 33226
		[SerializeField]
		private CButton rightButton;

		// Token: 0x040081CB RID: 33227
		[SerializeField]
		private HorizontalLayoutGroup dropdownRoot;

		// Token: 0x040081CC RID: 33228
		[SerializeField]
		private FilterDropdown dropdownTemplate;

		// Token: 0x040081CD RID: 33229
		[SerializeField]
		private RectTransform dropdownLayoutMask;

		// Token: 0x040081CE RID: 33230
		[SerializeField]
		private ScrollRect scrollRect;
	}
}
