using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000C99 RID: 3225
	public abstract class FilterLineBase<TData>
	{
		// Token: 0x17001124 RID: 4388
		// (get) Token: 0x0600A42B RID: 42027
		public abstract int Id { get; }

		// Token: 0x0600A42C RID: 42028
		public abstract LineConfig GenerateConfig();

		// Token: 0x0600A42D RID: 42029
		public abstract bool IsDataMatch(TData data, LineState lineState);

		// Token: 0x0600A42E RID: 42030
		public abstract DynamicLineConfig GenerateDynamicConfig(IEnumerable<TData> dataList);
	}
}
