using System;

namespace Game.Views.Encyclopedia.Views
{
	// Token: 0x02000A6F RID: 2671
	internal class SearchResultItem : IEquatable<SearchResultItem>
	{
		// Token: 0x06008391 RID: 33681 RVA: 0x003D3C14 File Offset: 0x003D1E14
		public SearchResultItem(NodeData nodeData, SearchIndex index)
		{
			this.NodeData = nodeData;
			this.SearchIndex = index;
		}

		// Token: 0x06008392 RID: 33682 RVA: 0x003D3C2C File Offset: 0x003D1E2C
		public bool Equals(SearchResultItem other)
		{
			bool flag = ((other != null) ? other.NodeData : null) == null;
			return !flag && other.NodeData.Equals(this.NodeData) && other.SearchIndex.Equals(this.SearchIndex);
		}

		// Token: 0x040064BD RID: 25789
		public readonly NodeData NodeData;

		// Token: 0x040064BE RID: 25790
		public readonly SearchIndex SearchIndex;
	}
}
