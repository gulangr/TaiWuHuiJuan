using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CA4 RID: 3236
	public interface IFilteredCountView
	{
		// Token: 0x0600A489 RID: 42121
		void SetFilteredCount(int count);

		// Token: 0x0600A48A RID: 42122
		void SetFilterOptionCounts(IReadOnlyList<OptionCountData> optionCounts);
	}
}
