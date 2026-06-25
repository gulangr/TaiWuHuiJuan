using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DB7 RID: 3511
	public class MaterialPoisonCategoryMenu : CommonMaterialFilterTypeMenuNew
	{
		// Token: 0x17001273 RID: 4723
		// (get) Token: 0x0600A986 RID: 43398 RVA: 0x004E5CF5 File Offset: 0x004E3EF5
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A987 RID: 43399 RVA: 0x004E5CF8 File Offset: 0x004E3EF8
		protected override IEnumerable<EMaterialFilterType> GenerateMaterialFilterTypes()
		{
			yield return EMaterialFilterType.Poison;
			yield return EMaterialFilterType.JiaoEgg;
			yield return EMaterialFilterType.Jiao;
			yield break;
		}
	}
}
