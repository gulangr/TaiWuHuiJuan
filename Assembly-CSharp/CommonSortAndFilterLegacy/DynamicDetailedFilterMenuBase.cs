using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000438 RID: 1080
	public abstract class DynamicDetailedFilterMenuBase<TData> : DetailedFilterMenuBase<TData>
	{
		// Token: 0x06003FF8 RID: 16376 RVA: 0x001FCF64 File Offset: 0x001FB164
		public sealed override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return null;
		}

		// Token: 0x1700068B RID: 1675
		// (get) Token: 0x06003FF9 RID: 16377 RVA: 0x001FCF77 File Offset: 0x001FB177
		public sealed override bool EnableDragHold
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700068C RID: 1676
		// (get) Token: 0x06003FFA RID: 16378 RVA: 0x001FCF7A File Offset: 0x001FB17A
		public sealed override int Version
		{
			get
			{
				return 1;
			}
		}
	}
}
