using System;
using System.Runtime.CompilerServices;

namespace BetterSearch.Contracts
{
	// Token: 0x02000003 RID: 3
	public struct SearchRequest
	{
		// Token: 0x04000006 RID: 6
		public SearchScope Scope;

		// Token: 0x04000007 RID: 7
		public short AreaId;

		// Token: 0x04000008 RID: 8
		public short BlockId;

		// Token: 0x04000009 RID: 9
		[Nullable(1)]
		public string Keyword;
	}
}
