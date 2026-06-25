using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CD4 RID: 3284
	[RequireComponent(typeof(CToggleGroup))]
	public class ToggleGroupLine : MonoBehaviour, ISortAndFilterLine
	{
		// Token: 0x0600A5F7 RID: 42487 RVA: 0x004D46A8 File Offset: 0x004D28A8
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
		}

		// Token: 0x0600A5F8 RID: 42488 RVA: 0x004D470E File Offset: 0x004D290E
		private void Awake()
		{
			this.ToggleGroup.OnActiveIndexChange += this.OnActiveIndexChange;
		}

		// Token: 0x0600A5F9 RID: 42489 RVA: 0x004D4729 File Offset: 0x004D2929
		private void OnDestroy()
		{
			this.ToggleGroup.OnActiveIndexChange -= this.OnActiveIndexChange;
		}

		// Token: 0x0600A5FA RID: 42490 RVA: 0x004D4744 File Offset: 0x004D2944
		public void RefreshToggles(List<FilterToggleConfig> toggleConfigs)
		{
			CommonUtils.PrepareEnoughChildren(this.toggleRoot, this.toggleTemplate.gameObject, toggleConfigs.Count, null);
			for (int i = 0; i < toggleConfigs.Count; i++)
			{
				FilterToggle toggle = this.toggleRoot.GetChild(i).GetComponent<FilterToggle>();
				toggle.Refresh(toggleConfigs[i]);
			}
			this.ToggleGroup.AddAllChildToggles();
			this.ToggleGroup.Init(-1);
		}

		// Token: 0x0600A5FB RID: 42491 RVA: 0x004D47C8 File Offset: 0x004D29C8
		private int GetActiveToggleKey()
		{
			return this.ToggleGroup.GetActiveIndex();
		}

		// Token: 0x0600A5FC RID: 42492 RVA: 0x004D47E5 File Offset: 0x004D29E5
		private void OnActiveIndexChange(int newIndex, int oldIndex)
		{
			Action onFilterChanged = this._onFilterChanged;
			if (onFilterChanged != null)
			{
				onFilterChanged();
			}
		}

		// Token: 0x17001152 RID: 4434
		// (get) Token: 0x0600A5FD RID: 42493 RVA: 0x004D47FA File Offset: 0x004D29FA
		private CToggleGroup ToggleGroup
		{
			get
			{
				return base.GetComponent<CToggleGroup>();
			}
		}

		// Token: 0x0600A5FE RID: 42494 RVA: 0x004D4804 File Offset: 0x004D2A04
		public LineState GetLineState()
		{
			return new LineState
			{
				IsActive = this.IsActive(),
				Type = ESortAndFilterOneLineType.ToggleGroup,
				ToggleGroupState = this.GetGroupState()
			};
		}

		// Token: 0x0600A5FF RID: 42495 RVA: 0x004D4841 File Offset: 0x004D2A41
		public void SetActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x0600A600 RID: 42496 RVA: 0x004D4854 File Offset: 0x004D2A54
		public bool IsActive()
		{
			return base.gameObject.activeSelf;
		}

		// Token: 0x0600A601 RID: 42497 RVA: 0x004D4871 File Offset: 0x004D2A71
		public void ClearAllFilter()
		{
		}

		// Token: 0x0600A602 RID: 42498 RVA: 0x004D4874 File Offset: 0x004D2A74
		public void ApplyDynamicConfig(DynamicLineConfig dynamicConfig)
		{
		}

		// Token: 0x0600A603 RID: 42499 RVA: 0x004D4878 File Offset: 0x004D2A78
		private ToggleKey GetGroupState()
		{
			return new ToggleKey
			{
				IsAll = (this.GetActiveToggleKey() == 0),
				Index = this.GetActiveToggleKey() - 1
			};
		}

		// Token: 0x0600A604 RID: 42500 RVA: 0x004D48B4 File Offset: 0x004D2AB4
		public void SetToggleVisible(ToggleKey toggleKey, bool isVisible)
		{
			int internalKey = toggleKey.IsAll ? 0 : (toggleKey.Index + 1);
			CToggle toggle = this.ToggleGroup.Get(internalKey);
			bool flag = toggle != null;
			if (flag)
			{
				toggle.gameObject.SetActive(isVisible);
			}
		}

		// Token: 0x0600A605 RID: 42501 RVA: 0x004D4900 File Offset: 0x004D2B00
		public void SetToggleInteractable(ToggleKey toggleKey, bool interactable)
		{
			int internalKey = toggleKey.IsAll ? 0 : (toggleKey.Index + 1);
			CToggle toggle = this.ToggleGroup.Get(internalKey);
			bool flag = toggle != null;
			if (flag)
			{
				toggle.interactable = interactable;
			}
		}

		// Token: 0x0600A606 RID: 42502 RVA: 0x004D4944 File Offset: 0x004D2B44
		public void SetToggleIsOn(ToggleKey toggleIndexToggleKey, bool isOn)
		{
			int internalKey = toggleIndexToggleKey.IsAll ? 0 : (toggleIndexToggleKey.Index + 1);
			if (isOn)
			{
				this.ToggleGroup.Set(internalKey, true);
			}
		}

		// Token: 0x0600A607 RID: 42503 RVA: 0x004D497C File Offset: 0x004D2B7C
		public void SetToggleIsOnWithoutNotify(ToggleKey toggleKey)
		{
			int internalKey = toggleKey.IsAll ? 0 : (toggleKey.Index + 1);
			this.ToggleGroup.SetWithoutNotify(internalKey);
		}

		// Token: 0x040082FD RID: 33533
		private FilterToggleGroupConfig _config;

		// Token: 0x040082FE RID: 33534
		private FilterToggleGroupConfig _internalConfig;

		// Token: 0x040082FF RID: 33535
		private Action _onFilterChanged;

		// Token: 0x04008300 RID: 33536
		[SerializeField]
		private FilterToggle toggleTemplate;

		// Token: 0x04008301 RID: 33537
		[SerializeField]
		private RectTransform toggleRoot;
	}
}
