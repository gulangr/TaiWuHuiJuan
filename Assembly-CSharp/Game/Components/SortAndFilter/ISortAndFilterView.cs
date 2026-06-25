using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CA6 RID: 3238
	public interface ISortAndFilterView
	{
		// Token: 0x17001130 RID: 4400
		// (get) Token: 0x0600A490 RID: 42128
		GameObject gameObject { get; }

		// Token: 0x17001131 RID: 4401
		// (get) Token: 0x0600A491 RID: 42129
		SortAndFilterConfig Config { get; }

		// Token: 0x17001132 RID: 4402
		// (get) Token: 0x0600A492 RID: 42130
		ISortUi SortUi { get; }

		// Token: 0x0600A493 RID: 42131
		SortAndFilterState GetStateFromUI();

		// Token: 0x0600A494 RID: 42132
		void Setup(SortAndFilterConfig config);

		// Token: 0x0600A495 RID: 42133
		void RebuildForLanguageChange(SortAndFilterConfig config);

		// Token: 0x0600A496 RID: 42134
		void UpdateLineActive(int changedLineIndex);

		// Token: 0x0600A497 RID: 42135
		void RefreshSortDisplay<TData>(List<FilterLineBase<TData>> activeSort, List<LineState> lineStates);

		// Token: 0x0600A498 RID: 42136
		void ClearAllFilter();

		// Token: 0x0600A499 RID: 42137
		void SetToggleVisible(ToggleIdIndex toggleIndex, bool isVisible);

		// Token: 0x0600A49A RID: 42138
		void SetToggleIsOn(ToggleIdIndex toggleIndex, bool isOn);

		// Token: 0x0600A49B RID: 42139
		void SetToggleIsOnWithoutNotify(int lineId, int toggleIndex);

		// Token: 0x0600A49C RID: 42140
		void SetDropdownMenuVisible(int lineId, int menuId, bool isVisible);

		// Token: 0x0600A49D RID: 42141
		void SetVisibleDropdownMenus(int lineId, IEnumerable<int> visibleMenuIds);

		// Token: 0x0600A49E RID: 42142
		void SetDropdownOptionInteractable(int lineId, int menuId, int optionIndex, bool interactable, string disabledTooltip);

		// Token: 0x0600A49F RID: 42143
		void SetDropdownOption(int lineId, int menuId, int optionIndex);

		// Token: 0x0600A4A0 RID: 42144
		void RefreshDynamicConfigs();

		// Token: 0x0600A4A1 RID: 42145
		void ApplyFilterLineStates(List<LineState> lineStates);
	}
}
