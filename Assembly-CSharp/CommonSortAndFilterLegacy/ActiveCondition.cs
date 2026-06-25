using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000447 RID: 1095
	public struct ActiveCondition
	{
		// Token: 0x06004041 RID: 16449 RVA: 0x001FF11A File Offset: 0x001FD31A
		public ActiveCondition(List<ToggleIdIndex> activeDependsOn)
		{
			this.ActiveDependsOn = activeDependsOn;
		}

		// Token: 0x04002DDD RID: 11741
		public List<ToggleIdIndex> ActiveDependsOn;
	}
}
