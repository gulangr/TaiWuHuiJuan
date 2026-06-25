using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200051D RID: 1309
	public class FabricHardnessMenu : CommonMaterialHardnessMenu
	{
		// Token: 0x170007C9 RID: 1993
		// (get) Token: 0x06004333 RID: 17203 RVA: 0x002063C7 File Offset: 0x002045C7
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06004334 RID: 17204 RVA: 0x002063CA File Offset: 0x002045CA
		protected override IEnumerable<EMaterialFilterHardness> GenerateMaterialHardnessList()
		{
			yield return EMaterialFilterHardness.Fur;
			yield return EMaterialFilterHardness.Woven;
			yield break;
		}
	}
}
