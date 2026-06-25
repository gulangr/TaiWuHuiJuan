using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DAA RID: 3498
	public class MetalHardnessMenu : MaterialCommonHardnessMenu
	{
		// Token: 0x17001263 RID: 4707
		// (get) Token: 0x0600A95C RID: 43356 RVA: 0x004E51AB File Offset: 0x004E33AB
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600A95D RID: 43357 RVA: 0x004E51AE File Offset: 0x004E33AE
		protected override IEnumerable<EMaterialFilterHardness> GenerateMaterialHardnessList()
		{
			yield return EMaterialFilterHardness.Icon;
			yield return EMaterialFilterHardness.GoldSilver;
			yield break;
		}
	}
}
