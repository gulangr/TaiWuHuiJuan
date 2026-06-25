using System;
using System.Collections.Generic;

namespace Game.Views.Encyclopedia.Views
{
	// Token: 0x02000A6C RID: 2668
	internal class SearchResult
	{
		// Token: 0x06008384 RID: 33668 RVA: 0x003D3A11 File Offset: 0x003D1C11
		internal void Clear()
		{
			this.SearchResultList.Clear();
			this.IncludeParentSearchResultList.Clear();
			this.SelfSearchResultList.Clear();
		}

		// Token: 0x040064AD RID: 25773
		internal readonly List<SearchResultItem> SearchResultList = new List<SearchResultItem>();

		// Token: 0x040064AE RID: 25774
		internal readonly List<NodeData> IncludeParentSearchResultList = new List<NodeData>();

		// Token: 0x040064AF RID: 25775
		internal readonly List<SearchResultItem> SelfSearchResultList = new List<SearchResultItem>();
	}
}
