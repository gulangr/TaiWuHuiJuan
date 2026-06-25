using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000530 RID: 1328
	public class JadeHardnessMenu : CommonMaterialHardnessMenu
	{
		// Token: 0x170007E0 RID: 2016
		// (get) Token: 0x06004370 RID: 17264 RVA: 0x002072F3 File Offset: 0x002054F3
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06004371 RID: 17265 RVA: 0x002072F6 File Offset: 0x002054F6
		protected override IEnumerable<EMaterialFilterHardness> GenerateMaterialHardnessList()
		{
			yield return EMaterialFilterHardness.Stone;
			yield return EMaterialFilterHardness.Jade;
			yield break;
		}
	}
}
