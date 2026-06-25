using System;
using Config;

namespace AdventureEditor.Beta
{
	// Token: 0x020006A7 RID: 1703
	public class EditingAdventureTransferNode : EditingAdventureNode
	{
		// Token: 0x170009B6 RID: 2486
		// (get) Token: 0x06004F9A RID: 20378 RVA: 0x00253AA8 File Offset: 0x00251CA8
		protected override LanguageKey UnSpecifiedTerrainLanguageId
		{
			get
			{
				return LanguageKey.UI_AdventureEditor_Terrain_NotSpecified_TransferEndNode;
			}
		}

		// Token: 0x170009B7 RID: 2487
		// (get) Token: 0x06004F9B RID: 20379 RVA: 0x00253AB0 File Offset: 0x00251CB0
		public override string Name
		{
			get
			{
				bool flag = string.IsNullOrEmpty(this.Key);
				string result;
				if (flag)
				{
					result = "TransferNode_" + this.SortIndex.ToString();
				}
				else
				{
					result = "TransferNode_" + this.Key;
				}
				return result;
			}
		}

		// Token: 0x06004F9C RID: 20380 RVA: 0x00253AFC File Offset: 0x00251CFC
		public AdventureTransferNode ToAdventureTransferNode()
		{
			return new AdventureTransferNode(this.EventId, this.Key, this.NodeTitle, this.TerrainId);
		}

		// Token: 0x040036CC RID: 14028
		private const string KeyPrefix = "TransferNode_";
	}
}
