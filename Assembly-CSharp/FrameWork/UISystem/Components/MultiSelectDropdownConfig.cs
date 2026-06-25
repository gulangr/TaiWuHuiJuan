using System;
using System.Collections.Generic;
using CommonSortAndFilterLegacy;

namespace FrameWork.UISystem.Components
{
	// Token: 0x0200101C RID: 4124
	public struct MultiSelectDropdownConfig<TBarConfig, TItemConfig>
	{
		// Token: 0x04009151 RID: 37201
		public TBarConfig MenuBarConfig;

		// Token: 0x04009152 RID: 37202
		public List<TItemConfig> MenuItemConfigs;

		// Token: 0x04009153 RID: 37203
		public int Id;

		// Token: 0x04009154 RID: 37204
		public MenuOptionIndex? Dependency;

		// Token: 0x04009155 RID: 37205
		public HashSet<int> DefaultSelectedIndices;

		// Token: 0x04009156 RID: 37206
		public int Version;

		// Token: 0x04009157 RID: 37207
		public bool EnableDragHold;

		// Token: 0x04009158 RID: 37208
		public FilterLogic LogicType;
	}
}
