using System;

namespace BetterSearch.Contracts
{
	// Token: 0x02000005 RID: 5
	public struct SearchRequest
	{
		// Token: 0x04000007 RID: 7
		public SearchScope Scope;

		// Token: 0x04000008 RID: 8
		public short AreaId;

		// Token: 0x04000009 RID: 9
		public short BlockId;

		// Token: 0x0400000A RID: 10
		public string Keyword;
	}
}
