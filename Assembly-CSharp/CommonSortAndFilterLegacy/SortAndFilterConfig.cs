using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000446 RID: 1094
	public class SortAndFilterConfig
	{
		// Token: 0x04002DD6 RID: 11734
		public List<LineConfig> LineConfigs;

		// Token: 0x04002DD7 RID: 11735
		public Action OnSortChanged;

		// Token: 0x04002DD8 RID: 11736
		public Action<int> OnFilterChanged;

		// Token: 0x04002DD9 RID: 11737
		public Action<int> OnFilterCustomOrderChanged;

		// Token: 0x04002DDA RID: 11738
		public Action OnFilterCustomOrderReset;

		// Token: 0x04002DDB RID: 11739
		public SortConfig SortConfig;

		// Token: 0x04002DDC RID: 11740
		public Func<int, DynamicLineConfig> DynamicConfigProvider;
	}
}
