using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000523 RID: 1315
	internal class MaterialFoodTypeMenu : CommonMaterialFilterTypeMenu
	{
		// Token: 0x170007D2 RID: 2002
		// (get) Token: 0x0600434C RID: 17228 RVA: 0x00206767 File Offset: 0x00204967
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600434D RID: 17229 RVA: 0x0020676A File Offset: 0x0020496A
		protected override IEnumerable<EMaterialFilterType> GenerateMaterialFilterTypes()
		{
			yield return EMaterialFilterType.Bird;
			yield return EMaterialFilterType.Beast;
			yield return EMaterialFilterType.Fish;
			yield return EMaterialFilterType.Vegetarian;
			yield break;
		}
	}
}
