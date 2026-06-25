using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000C9D RID: 3229
	public class FilterPanel : MonoBehaviour
	{
		// Token: 0x0600A442 RID: 42050 RVA: 0x004CB61C File Offset: 0x004C981C
		private void Awake()
		{
			this._panelRectTransform = (base.transform as RectTransform);
		}

		// Token: 0x0600A443 RID: 42051 RVA: 0x004CB630 File Offset: 0x004C9830
		public void Setup(SortAndFilter owner)
		{
			this._owner = owner;
			this._sections.Clear();
			this.closeButton.ClearAndAddListener(new Action(this.OnCloseClicked));
			this.clearButton.ClearAndAddListener(new Action(this.ClearAll));
			this.Refresh();
		}

		// Token: 0x0600A444 RID: 42052 RVA: 0x004CB688 File Offset: 0x004C9888
		public void Refresh()
		{
			SortAndFilter owner = this._owner;
			bool flag = ((owner != null) ? owner.Config : null) == null;
			if (!flag)
			{
				this.RefreshHeader();
				this.RefreshSections();
			}
		}

		// Token: 0x0600A445 RID: 42053 RVA: 0x004CB6C0 File Offset: 0x004C98C0
		public void RefreshHeader()
		{
			SortAndFilter owner = this._owner;
			bool flag = ((owner != null) ? owner.Config : null) == null;
			if (!flag)
			{
				bool shouldShowPanelHeader = this._owner.ShouldShowPanelHeader;
				if (shouldShowPanelHeader)
				{
					string panelTitle = this.GetPanelTitle();
					this.titleText.SetText(panelTitle, true);
					this.summaryText.SetText(LanguageKey.LK_Brackets_Fix.TrFormat(this._owner.GetFilteredCountText()), true);
				}
				else
				{
					this.titleText.SetText(string.Empty, true);
					this.summaryText.SetText(string.Empty, true);
				}
			}
		}

		// Token: 0x0600A446 RID: 42054 RVA: 0x004CB75C File Offset: 0x004C995C
		public void RefreshFilterOptionCounts(IReadOnlyList<OptionCountData> optionCounts)
		{
			foreach (KeyValuePair<ValueTuple<int, int>, FilterSection> kvp in this._sectionMap)
			{
				ValueTuple<int, int> key = kvp.Key;
				int lineId = key.Item1;
				int menuId = key.Item2;
				FilterSection section = kvp.Value;
				int allCount = 0;
				List<int> optionCountList = new List<int>();
				bool found = false;
				foreach (OptionCountData data in optionCounts)
				{
					bool flag = data.LineId != lineId || data.MenuId != menuId;
					if (!flag)
					{
						found = true;
						bool flag2 = data.OptionIndex < 0;
						if (flag2)
						{
							allCount = data.Count;
						}
						else
						{
							optionCountList.Add(data.Count);
						}
					}
				}
				bool flag3 = !found;
				if (!flag3)
				{
					int[] counts = new int[optionCountList.Count + 1];
					counts[0] = allCount;
					for (int i = 0; i < optionCountList.Count; i++)
					{
						counts[i + 1] = optionCountList[i];
					}
					section.RefreshOptionCounts(counts);
				}
			}
		}

		// Token: 0x0600A447 RID: 42055 RVA: 0x004CB8D8 File Offset: 0x004C9AD8
		public void ClearAll()
		{
			List<FilterSection> sectionSnapshot = new List<FilterSection>(this._sections);
			foreach (FilterSection section in sectionSnapshot)
			{
				bool flag = section == null;
				if (!flag)
				{
					section.ClearSelection();
				}
			}
			SortAndFilter owner = this._owner;
			if (owner != null)
			{
				owner.ClearAllFilter();
			}
			this.Refresh();
		}

		// Token: 0x0600A448 RID: 42056 RVA: 0x004CB960 File Offset: 0x004C9B60
		private void OnCloseClicked()
		{
			SortAndFilter owner = this._owner;
			if (owner != null)
			{
				owner.CloseFilterPanel();
			}
		}

		// Token: 0x0600A449 RID: 42057 RVA: 0x004CB978 File Offset: 0x004C9B78
		private void Update()
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(1);
			if (mouseButtonDown)
			{
				SortAndFilter owner = this._owner;
				if (owner != null)
				{
					owner.CloseFilterPanel();
				}
			}
			else
			{
				bool flag = !Input.GetMouseButtonDown(0);
				if (!flag)
				{
					bool flag2 = RectTransformUtility.RectangleContainsScreenPoint(this._panelRectTransform, Input.mousePosition, UIManager.Instance.UiCamera);
					if (!flag2)
					{
						bool flag3 = this._owner != null && this._owner.IsPointOverEntryButton(Input.mousePosition);
						if (!flag3)
						{
							SortAndFilter owner2 = this._owner;
							if (owner2 != null)
							{
								owner2.CloseFilterPanel();
							}
						}
					}
				}
			}
		}

		// Token: 0x0600A44A RID: 42058 RVA: 0x004CBA18 File Offset: 0x004C9C18
		private void RefreshSections()
		{
			bool flag = this._owner == null;
			if (!flag)
			{
				this._sections.Clear();
				this._sectionMap.Clear();
				IReadOnlyList<SortAndFilter.SectionViewData> sections = this._owner.Sections;
				List<FilterPanel.SectionData> sectionDataList = new List<FilterPanel.SectionData>();
				foreach (SortAndFilter.SectionViewData sectionData in sections)
				{
					bool flag2 = !sectionData.IsActive;
					if (!flag2)
					{
						bool flag3 = sectionData.Type == ESortAndFilterOneLineType.ToggleGroup;
						if (!flag3)
						{
							DetailedFilterMenuConfig? menuConfig = this._owner.GetMenuConfig(sectionData.LineIndex, sectionData.MenuId);
							bool flag4 = menuConfig == null;
							if (!flag4)
							{
								sectionDataList.Add(new FilterPanel.SectionData
								{
									LineId = sectionData.LineId,
									MenuId = sectionData.MenuId,
									MenuConfig = menuConfig.Value
								});
							}
						}
					}
				}
				CommonUtils.PrepareEnoughChildren(this.sectionRoot, this.sectionTemplate.gameObject, sectionDataList.Count, null);
				for (int i = 0; i < sectionDataList.Count; i++)
				{
					FilterPanel.SectionData data = sectionDataList[i];
					Transform sectionGo = this.sectionRoot.GetChild(i);
					FilterSection section = sectionGo.GetComponent<FilterSection>();
					bool flag5 = section == null;
					if (!flag5)
					{
						section.gameObject.SetActive(true);
						this.SetupSection(section, data);
						this._sections.Add(section);
						this._sectionMap[new ValueTuple<int, int>(data.LineId, data.MenuId)] = section;
					}
				}
				for (int j = sectionDataList.Count; j < this.sectionRoot.childCount; j++)
				{
					Transform child = this.sectionRoot.GetChild(j);
					bool flag6 = child != null;
					if (flag6)
					{
						child.gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x0600A44B RID: 42059 RVA: 0x004CBC48 File Offset: 0x004C9E48
		private void SetupSection(FilterSection section, FilterPanel.SectionData data)
		{
			DetailedFilterMenuConfig menuConfig = data.MenuConfig;
			string sectionTitle = menuConfig.DropdownConfig.MenuBarLabel.GetString();
			List<FilterDropdownItemConfig> itemConfigs = menuConfig.DropdownConfig.ItemConfigs;
			bool flag = itemConfigs == null;
			if (!flag)
			{
				Action<int> onSelectionChanged = delegate(int selectedIndex)
				{
					this.OnSectionSelectionChanged(data.LineId, menuConfig.Id, selectedIndex);
				};
				bool flag2 = !section.IsSameConfig(menuConfig.Id, sectionTitle, itemConfigs);
				if (flag2)
				{
					section.Setup(menuConfig.Id, sectionTitle, itemConfigs, onSelectionChanged);
				}
				else
				{
					section.SetSelectionChangedCallback(onSelectionChanged);
				}
				int initialState = this._owner.GetInitialSectionState(data.LineId, menuConfig.Id);
				section.SetSelectedIndex(initialState, false);
				for (int i = 0; i < itemConfigs.Count; i++)
				{
					bool interactable;
					string disabledTooltip;
					bool flag3 = !this._owner.TryGetOptionInteractableState(data.LineId, menuConfig.Id, i, out interactable, out disabledTooltip);
					if (!flag3)
					{
						section.SetOptionInteractable(i, interactable, disabledTooltip);
					}
				}
			}
		}

		// Token: 0x0600A44C RID: 42060 RVA: 0x004CBD86 File Offset: 0x004C9F86
		private void OnSectionSelectionChanged(int lineId, int menuId, int selectedIndex)
		{
			SortAndFilter owner = this._owner;
			if (owner != null)
			{
				owner.OnSectionSelectionChanged(lineId, menuId, selectedIndex);
			}
		}

		// Token: 0x0600A44D RID: 42061 RVA: 0x004CBDA0 File Offset: 0x004C9FA0
		private string GetPanelTitle()
		{
			return this._owner.GetPanelTitle();
		}

		// Token: 0x0600A44E RID: 42062 RVA: 0x004CBDC0 File Offset: 0x004C9FC0
		public void SetOptionInteractable(int lineId, int menuId, int optionIndex, bool interactable, string disabledTooltip)
		{
			FilterSection section;
			bool flag = !this._sectionMap.TryGetValue(new ValueTuple<int, int>(lineId, menuId), out section);
			if (!flag)
			{
				section.SetOptionInteractable(optionIndex, interactable, disabledTooltip);
			}
		}

		// Token: 0x0400820D RID: 33293
		[SerializeField]
		private CButton closeButton;

		// Token: 0x0400820E RID: 33294
		[SerializeField]
		private CButton clearButton;

		// Token: 0x0400820F RID: 33295
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x04008210 RID: 33296
		[SerializeField]
		private TextMeshProUGUI summaryText;

		// Token: 0x04008211 RID: 33297
		[SerializeField]
		private RectTransform sectionRoot;

		// Token: 0x04008212 RID: 33298
		[SerializeField]
		private FilterSection sectionTemplate;

		// Token: 0x04008213 RID: 33299
		private SortAndFilter _owner;

		// Token: 0x04008214 RID: 33300
		private RectTransform _panelRectTransform;

		// Token: 0x04008215 RID: 33301
		private readonly List<FilterSection> _sections = new List<FilterSection>();

		// Token: 0x04008216 RID: 33302
		[TupleElementNames(new string[]
		{
			"LineId",
			"MenuId"
		})]
		private readonly Dictionary<ValueTuple<int, int>, FilterSection> _sectionMap = new Dictionary<ValueTuple<int, int>, FilterSection>();

		// Token: 0x020023F7 RID: 9207
		private struct SectionData
		{
			// Token: 0x0400E112 RID: 57618
			public int LineId;

			// Token: 0x0400E113 RID: 57619
			public int MenuId;

			// Token: 0x0400E114 RID: 57620
			public DetailedFilterMenuConfig MenuConfig;
		}
	}
}
