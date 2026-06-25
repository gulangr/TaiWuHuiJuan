using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CCA RID: 3274
	public interface ISortUiWithDisplayingSortIds : ISortUi
	{
		// Token: 0x17001143 RID: 4419
		// (get) Token: 0x0600A593 RID: 42387
		HashSet<short> DisplayingSortIds { get; }
	}
}
