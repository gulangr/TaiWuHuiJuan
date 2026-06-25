using System;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EC2 RID: 3778
	public struct LegendaryBookFeatureCellData
	{
		// Token: 0x0600AEF9 RID: 44793 RVA: 0x004FB84D File Offset: 0x004F9A4D
		public LegendaryBookFeatureCellData(short featureId, int characterId)
		{
			this.FeatureId = featureId;
			this.CharacterId = characterId;
		}

		// Token: 0x04008761 RID: 34657
		public short FeatureId;

		// Token: 0x04008762 RID: 34658
		public int CharacterId;
	}
}
