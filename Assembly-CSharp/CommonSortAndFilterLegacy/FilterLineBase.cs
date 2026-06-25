using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x0200043E RID: 1086
	public abstract class FilterLineBase<TData>
	{
		// Token: 0x1700068D RID: 1677
		// (get) Token: 0x06004000 RID: 16384
		public abstract int Id { get; }

		// Token: 0x06004001 RID: 16385
		public abstract LineConfig GenerateConfig();

		// Token: 0x06004002 RID: 16386
		public abstract bool IsDataMatch(TData data, LineState lineState);

		// Token: 0x06004003 RID: 16387
		public abstract DynamicLineConfig GenerateDynamicConfig(List<TData> dataList);
	}
}
