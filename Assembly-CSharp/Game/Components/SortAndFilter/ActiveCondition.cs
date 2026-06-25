using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CAA RID: 3242
	public struct ActiveCondition
	{
		// Token: 0x0600A4F8 RID: 42232 RVA: 0x004CF0AA File Offset: 0x004CD2AA
		public ActiveCondition(List<ToggleIdIndex> activeDependsOn)
		{
			this.ActiveDependsOn = activeDependsOn;
		}

		// Token: 0x0400825A RID: 33370
		public List<ToggleIdIndex> ActiveDependsOn;
	}
}
