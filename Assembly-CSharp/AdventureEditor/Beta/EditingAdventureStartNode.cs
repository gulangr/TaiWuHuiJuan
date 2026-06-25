using System;
using Config;

namespace AdventureEditor.Beta
{
	// Token: 0x020006A6 RID: 1702
	public class EditingAdventureStartNode : EditingAdventureNode
	{
		// Token: 0x170009B5 RID: 2485
		// (get) Token: 0x06004F97 RID: 20375 RVA: 0x00253A24 File Offset: 0x00251C24
		public override string Name
		{
			get
			{
				bool flag = string.IsNullOrEmpty(this.Key);
				string result;
				if (flag)
				{
					result = "StartNode_" + this.SortIndex.ToString();
				}
				else
				{
					result = "StartNode_" + this.Key;
				}
				return result;
			}
		}

		// Token: 0x06004F98 RID: 20376 RVA: 0x00253A70 File Offset: 0x00251C70
		public AdventureStartNode ToAdventureStartNode()
		{
			return new AdventureStartNode(this.EventId, this.Key, this.NodeTitle, this.TerrainId);
		}

		// Token: 0x040036CB RID: 14027
		private const string KeyPrefix = "StartNode_";
	}
}
