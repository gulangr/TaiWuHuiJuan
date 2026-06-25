using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x0200045E RID: 1118
	public abstract class StaticDetailedFilterMenuBase<TData> : DetailedFilterMenuBase<TData>
	{
		// Token: 0x0600406A RID: 16490 RVA: 0x001FF670 File Offset: 0x001FD870
		public sealed override List<DetailFilterMultiSelectDropdownItemConfig> GetDynamicMenuConfigs(List<TData> dataList)
		{
			return null;
		}

		// Token: 0x170006A0 RID: 1696
		// (get) Token: 0x0600406B RID: 16491 RVA: 0x001FF683 File Offset: 0x001FD883
		public sealed override bool EnableDragHold
		{
			get
			{
				return true;
			}
		}
	}
}
