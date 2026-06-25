using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DBA RID: 3514
	public class MaterialFoodTypeMenu : CommonMaterialFilterTypeMenuNew
	{
		// Token: 0x17001276 RID: 4726
		// (get) Token: 0x0600A98E RID: 43406 RVA: 0x004E5D5B File Offset: 0x004E3F5B
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A98F RID: 43407 RVA: 0x004E5D5E File Offset: 0x004E3F5E
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
