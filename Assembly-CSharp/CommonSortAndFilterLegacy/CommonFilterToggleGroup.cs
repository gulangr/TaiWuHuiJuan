using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000426 RID: 1062
	[RequireComponent(typeof(CToggleGroupObsolete))]
	public class CommonFilterToggleGroup : MonoBehaviour, ISortAndFilterLine
	{
		// Token: 0x06003EF5 RID: 16117 RVA: 0x001F7A44 File Offset: 0x001F5C44
		public void Setup(FilterToggleGroupConfig config, Action onFilterChanged)
		{
			this._onFilterChanged = onFilterChanged;
			this._config = config;
			this._internalConfig = new FilterToggleGroupConfig
			{
				FilterToggleConfigs = new List<FilterToggleConfig>(config.FilterToggleConfigs)
			};
			this._internalConfig.FilterToggleConfigs.Insert(0, FilterToggleConfig.CreateNamedAll());
			this.RefreshToggles(this._internalConfig.FilterToggleConfigs);
			this.ToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnActiveToggleChange);
		}

		// Token: 0x06003EF6 RID: 16118 RVA: 0x001F7AC4 File Offset: 0x001F5CC4
		public void RefreshToggles(List<FilterToggleConfig> toggleConfigs)
		{
			CommonUtils.PrepareEnoughChildren(this.toggleRoot, this.toggleTemplate.gameObject, toggleConfigs.Count, null);
			for (int i = 0; i < toggleConfigs.Count; i++)
			{
				CommonFilterToggle toggle = this.toggleRoot.GetChild(i).GetComponent<CommonFilterToggle>();
				toggle.Refresh(toggleConfigs[i]);
			}
			this.ToggleGroup.AddAllChildToggles();
			this.ToggleGroup.InitPreOnToggle(-1);
		}

		// Token: 0x06003EF7 RID: 16119 RVA: 0x001F7B48 File Offset: 0x001F5D48
		private int GetActiveToggleKey()
		{
			CToggleObsolete active = this.ToggleGroup.GetActive();
			return (active != null) ? active.Key : -1;
		}

		// Token: 0x06003EF8 RID: 16120 RVA: 0x001F7B71 File Offset: 0x001F5D71
		private void OnActiveToggleChange(CToggleObsolete newToggle, CToggleObsolete oldToggle)
		{
			Action onFilterChanged = this._onFilterChanged;
			if (onFilterChanged != null)
			{
				onFilterChanged();
			}
		}

		// Token: 0x17000667 RID: 1639
		// (get) Token: 0x06003EF9 RID: 16121 RVA: 0x001F7B86 File Offset: 0x001F5D86
		private CToggleGroupObsolete ToggleGroup
		{
			get
			{
				return base.GetComponent<CToggleGroupObsolete>();
			}
		}

		// Token: 0x06003EFA RID: 16122 RVA: 0x001F7B90 File Offset: 0x001F5D90
		public LineState GetLineState()
		{
			return new LineState
			{
				IsActive = this.IsActive(),
				Type = ESortAndFilterOneLineType.ToggleGroup,
				ToggleGroupState = this.GetGroupState()
			};
		}

		// Token: 0x06003EFB RID: 16123 RVA: 0x001F7BCD File Offset: 0x001F5DCD
		public void SetActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x06003EFC RID: 16124 RVA: 0x001F7BE0 File Offset: 0x001F5DE0
		public bool IsActive()
		{
			return base.gameObject.activeSelf;
		}

		// Token: 0x06003EFD RID: 16125 RVA: 0x001F7BFD File Offset: 0x001F5DFD
		public void ApplyCustomOrder(IFilterLineCustomOrderData orderData)
		{
		}

		// Token: 0x06003EFE RID: 16126 RVA: 0x001F7C00 File Offset: 0x001F5E00
		public void ResetCustomOrder()
		{
		}

		// Token: 0x06003EFF RID: 16127 RVA: 0x001F7C04 File Offset: 0x001F5E04
		public IFilterLineCustomOrderData GetCustomOrderData()
		{
			return null;
		}

		// Token: 0x06003F00 RID: 16128 RVA: 0x001F7C17 File Offset: 0x001F5E17
		public void OnSwitchCustomOrderSettingMode(bool isSettingMode)
		{
			this._isSettingMode = isSettingMode;
			this.RefreshToggleInteractable();
		}

		// Token: 0x06003F01 RID: 16129 RVA: 0x001F7C28 File Offset: 0x001F5E28
		public void ClearAllFilter()
		{
		}

		// Token: 0x06003F02 RID: 16130 RVA: 0x001F7C2B File Offset: 0x001F5E2B
		public void ApplyDynamicConfig(DynamicLineConfig dynamicConfig)
		{
		}

		// Token: 0x06003F03 RID: 16131 RVA: 0x001F7C30 File Offset: 0x001F5E30
		private ToggleKey GetGroupState()
		{
			return new ToggleKey
			{
				IsAll = (this.GetActiveToggleKey() == 0),
				Index = this.GetActiveToggleKey() - 1
			};
		}

		// Token: 0x06003F04 RID: 16132 RVA: 0x001F7C6C File Offset: 0x001F5E6C
		private bool CanInteractToggle()
		{
			return !this._isSettingMode;
		}

		// Token: 0x06003F05 RID: 16133 RVA: 0x001F7C87 File Offset: 0x001F5E87
		private void RefreshToggleInteractable()
		{
			this.ToggleGroup.SetInteractable(this.CanInteractToggle(), null);
		}

		// Token: 0x06003F06 RID: 16134 RVA: 0x001F7CA0 File Offset: 0x001F5EA0
		public void SetToggleVisible(ToggleKey toggleKey, bool isVisible)
		{
			int internalKey = toggleKey.IsAll ? 0 : (toggleKey.Index + 1);
			CToggleObsolete toggle = this.ToggleGroup.Get(internalKey);
			bool flag = toggle != null;
			if (flag)
			{
				toggle.gameObject.SetActive(isVisible);
			}
		}

		// Token: 0x06003F07 RID: 16135 RVA: 0x001F7CEC File Offset: 0x001F5EEC
		public void SetToggleIsOn(ToggleKey toggleIndexToggleKey, bool isOn)
		{
			int internalKey = toggleIndexToggleKey.IsAll ? 0 : (toggleIndexToggleKey.Index + 1);
			this.ToggleGroup.Set(internalKey, isOn, false);
		}

		// Token: 0x04002D38 RID: 11576
		private FilterToggleGroupConfig _config;

		// Token: 0x04002D39 RID: 11577
		private FilterToggleGroupConfig _internalConfig;

		// Token: 0x04002D3A RID: 11578
		private Action _onFilterChanged;

		// Token: 0x04002D3B RID: 11579
		private bool _isSettingMode;

		// Token: 0x04002D3C RID: 11580
		[SerializeField]
		private CommonFilterToggle toggleTemplate;

		// Token: 0x04002D3D RID: 11581
		[SerializeField]
		private RectTransform toggleRoot;
	}
}
