using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000534 RID: 1332
	public class MetalHardnessMenu : CommonMaterialHardnessMenu
	{
		// Token: 0x170007E4 RID: 2020
		// (get) Token: 0x0600437A RID: 17274 RVA: 0x00207367 File Offset: 0x00205567
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600437B RID: 17275 RVA: 0x0020736A File Offset: 0x0020556A
		protected override IEnumerable<EMaterialFilterHardness> GenerateMaterialHardnessList()
		{
			yield return EMaterialFilterHardness.Icon;
			yield return EMaterialFilterHardness.GoldSilver;
			yield break;
		}
	}
}
