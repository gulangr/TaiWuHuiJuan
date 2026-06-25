using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy
{
	// Token: 0x0200043B RID: 1083
	public struct DetailedLineCustomOrderData : IFilterLineCustomOrderData
	{
		// Token: 0x04002DBF RID: 11711
		public List<int> MenuOrderList;

		// Token: 0x04002DC0 RID: 11712
		public Dictionary<int, MenuOptionsCustomOrderData> MenuOptionsOrderDict;
	}
}
