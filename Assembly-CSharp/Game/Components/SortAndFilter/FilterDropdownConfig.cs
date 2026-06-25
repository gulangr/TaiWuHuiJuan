using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000C94 RID: 3220
	public struct FilterDropdownConfig
	{
		// Token: 0x040081D1 RID: 33233
		public StringKey MenuBarLabel;

		// Token: 0x040081D2 RID: 33234
		public List<FilterDropdownItemConfig> ItemConfigs;

		// Token: 0x040081D3 RID: 33235
		public HashSet<int> DefaultSelectedIndices;

		// Token: 0x040081D4 RID: 33236
		public bool IsMultiSelect;
	}
}
