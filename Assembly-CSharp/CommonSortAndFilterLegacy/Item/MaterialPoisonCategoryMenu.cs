using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000539 RID: 1337
	internal class MaterialPoisonCategoryMenu : CommonMaterialFilterTypeMenu
	{
		// Token: 0x170007EA RID: 2026
		// (get) Token: 0x0600438A RID: 17290 RVA: 0x002074F5 File Offset: 0x002056F5
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600438B RID: 17291 RVA: 0x002074F8 File Offset: 0x002056F8
		protected override IEnumerable<EMaterialFilterType> GenerateMaterialFilterTypes()
		{
			yield return EMaterialFilterType.Poison;
			yield return EMaterialFilterType.JiaoEgg;
			yield return EMaterialFilterType.Jiao;
			yield break;
		}
	}
}
