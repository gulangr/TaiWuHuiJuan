using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000427 RID: 1063
	public class CommonSecondaryFilter : CommonDetailedFilter
	{
		// Token: 0x06003F09 RID: 16137 RVA: 0x001F7D28 File Offset: 0x001F5F28
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
					State = base.GetState()
				}
			};
		}

		// Token: 0x06003F0A RID: 16138 RVA: 0x001F7E40 File Offset: 0x001F6040
		protected override void SetupDropdownScrollInfo(DetailFilterMultiSelectDropdown dropdown)
		{
		}

		// Token: 0x06003F0B RID: 16139 RVA: 0x001F7E43 File Offset: 0x001F6043
		protected override void SetLeftAndRightButtonsVisible(bool showLeftRightButton)
		{
		}

		// Token: 0x06003F0C RID: 16140 RVA: 0x001F7E46 File Offset: 0x001F6046
		protected override void TickLeftRightButtonVisible()
		{
		}
	}
}
