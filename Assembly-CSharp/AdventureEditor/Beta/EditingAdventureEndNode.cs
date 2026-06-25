using System;
using Config;

namespace AdventureEditor.Beta
{
	// Token: 0x020006A8 RID: 1704
	public class EditingAdventureEndNode : EditingAdventureNode
	{
		// Token: 0x170009B8 RID: 2488
		// (get) Token: 0x06004F9E RID: 20382 RVA: 0x00253B34 File Offset: 0x00251D34
		protected override LanguageKey UnSpecifiedTerrainLanguageId
		{
			get
			{
				return LanguageKey.UI_AdventureEditor_Terrain_NotSpecified_TransferEndNode;
			}
		}

		// Token: 0x170009B9 RID: 2489
		// (get) Token: 0x06004F9F RID: 20383 RVA: 0x00253B3C File Offset: 0x00251D3C
		public override string Name
		{
			get
			{
				bool flag = string.IsNullOrEmpty(this.Key);
				string result;
				if (flag)
				{
					result = "EndNode_" + this.SortIndex.ToString();
				}
				else
				{
					result = "EndNode_" + this.Key;
				}
				return result;
			}
		}

		// Token: 0x06004FA0 RID: 20384 RVA: 0x00253B88 File Offset: 0x00251D88
		public AdventureEndNode ToAdventureEndNode()
		{
			return new AdventureEndNode(this.EventId, this.Key, this.NodeTitle, this.TerrainId);
		}

		// Token: 0x040036CD RID: 14029
		private const string KeyPrefix = "EndNode_";
	}
}
