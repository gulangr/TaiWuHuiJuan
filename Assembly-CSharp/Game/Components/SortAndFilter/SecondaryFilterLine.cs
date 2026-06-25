using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CA7 RID: 3239
	public class SecondaryFilterLine : DetailedFilterLine
	{
		// Token: 0x0600A4A2 RID: 42146 RVA: 0x004CCAD0 File Offset: 0x004CACD0
		public override LineState GetLineState()
		{
			DetailedFilterState state = base.GetState();
			int selectedIndex = -1;
			foreach (KeyValuePair<int, DetailedFilterMenuState> item in state.MenuStateDict)
			{
				bool isActive = item.Value.IsActive;
				if (isActive)
				{
					using (IEnumerator<int> enumerator2 = item.Value.SelectedIndices.GetEnumerator())
					{
						if (enumerator2.MoveNext())
						{
							int result = enumerator2.Current;
							selectedIndex = result;
						}
					}
				}
			}
			return new LineState
			{
				IsActive = base.IsActive(),
				Type = ESortAndFilterOneLineType.SingleSelectFilter,
				ToggleGroupState = new ToggleKey
				{
					IsAll = false,
					Index = selectedIndex
				},
				DetailedFilterState = new DetailedFilterLineState
				{
					State = state
				}
			};
		}

		// Token: 0x0600A4A3 RID: 42147 RVA: 0x004CCBE0 File Offset: 0x004CADE0
		protected override void SetupDropdownScrollInfo(FilterDropdown dropdown)
		{
		}

		// Token: 0x0600A4A4 RID: 42148 RVA: 0x004CCBE3 File Offset: 0x004CADE3
		protected override void SetLeftAndRightButtonsVisible(bool showLeftRightButton)
		{
		}

		// Token: 0x0600A4A5 RID: 42149 RVA: 0x004CCBE6 File Offset: 0x004CADE6
		protected override void TickLeftRightButtonVisible()
		{
		}
	}
}
