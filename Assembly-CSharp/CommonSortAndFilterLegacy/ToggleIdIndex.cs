using System;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x02000449 RID: 1097
	public struct ToggleIdIndex
	{
		// Token: 0x06004046 RID: 16454 RVA: 0x001FF1BB File Offset: 0x001FD3BB
		public ToggleIdIndex(int lineId, ToggleKey toggleKey)
		{
			this.LineId = lineId;
			this.ToggleKey = toggleKey;
		}

		// Token: 0x04002DE0 RID: 11744
		public int LineId;

		// Token: 0x04002DE1 RID: 11745
		public ToggleKey ToggleKey;
	}
}
