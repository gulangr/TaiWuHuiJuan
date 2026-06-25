using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000434 RID: 1076
	public abstract class SecondaryFilterLineBase<TData> : DetailedFilterLine<TData>
	{
		// Token: 0x06003FD3 RID: 16339 RVA: 0x001FBDF8 File Offset: 0x001F9FF8
		protected override LineConfig CreateConfig(List<MultiSelectDropdownConfig<DetailFilterMultiSelectDropdownMenuBarConfig, DetailFilterMultiSelectDropdownItemConfig>> menuConfigs)
		{
			return LineConfig.CreateDetailedSecondaryLineConfig(new DetailedFilterLineConfig(new DetailedFilterConfig
			{
				MenuConfigs = menuConfigs
			}));
		}
	}
}
