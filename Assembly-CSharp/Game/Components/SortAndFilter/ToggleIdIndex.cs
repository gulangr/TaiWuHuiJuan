using System;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000CAC RID: 3244
	public struct ToggleIdIndex
	{
		// Token: 0x0600A4FD RID: 42237 RVA: 0x004CF14B File Offset: 0x004CD34B
		public ToggleIdIndex(int lineId, ToggleKey toggleKey)
		{
			this.LineId = lineId;
			this.ToggleKey = toggleKey;
		}

		// Token: 0x0400825D RID: 33373
		public int LineId;

		// Token: 0x0400825E RID: 33374
		public ToggleKey ToggleKey;
	}
}
