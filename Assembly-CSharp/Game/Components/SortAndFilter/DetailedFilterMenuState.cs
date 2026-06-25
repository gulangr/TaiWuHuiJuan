using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CC3 RID: 3267
	public struct DetailedFilterMenuState
	{
		// Token: 0x0600A578 RID: 42360 RVA: 0x004D1EC8 File Offset: 0x004D00C8
		public DetailedFilterMenuState(IReadOnlyCollection<int> selectedIndices, bool isActive)
		{
			this.SelectedIndices = selectedIndices;
			this.IsActive = isActive;
		}

		// Token: 0x040082A1 RID: 33441
		public readonly IReadOnlyCollection<int> SelectedIndices;

		// Token: 0x040082A2 RID: 33442
		public readonly bool IsActive;
	}
}
