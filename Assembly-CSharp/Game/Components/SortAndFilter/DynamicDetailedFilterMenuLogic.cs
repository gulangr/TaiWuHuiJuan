using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000C91 RID: 3217
	public abstract class DynamicDetailedFilterMenuLogic<TData> : DetailedFilterMenuLogic<TData>
	{
		// Token: 0x0600A3ED RID: 41965
		public abstract List<FilterDropdownItemConfig> GetDynamicMenuConfigs(IEnumerable<TData> dataList);
	}
}
