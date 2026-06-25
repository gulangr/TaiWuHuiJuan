using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CA9 RID: 3241
	public class SortAndFilterConfig
	{
		// Token: 0x04008255 RID: 33365
		public List<LineConfig> LineConfigs;

		// Token: 0x04008256 RID: 33366
		public Action OnSortChanged;

		// Token: 0x04008257 RID: 33367
		public Action<int> OnFilterChanged;

		// Token: 0x04008258 RID: 33368
		public SortConfig SortConfig;

		// Token: 0x04008259 RID: 33369
		public Func<int, DynamicLineConfig> DynamicConfigProvider;
	}
}
