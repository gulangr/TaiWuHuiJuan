using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200055B RID: 1371
	public class ShopPriceSecondaryMenu : SecondaryFilterLineBase<ItemDisplayData>
	{
		// Token: 0x17000817 RID: 2071
		// (get) Token: 0x06004429 RID: 17449 RVA: 0x00209157 File Offset: 0x00207357
		public override int Id
		{
			get
			{
				return 40;
			}
		}

		// Token: 0x17000818 RID: 2072
		// (get) Token: 0x0600442A RID: 17450 RVA: 0x0020915B File Offset: 0x0020735B
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600442B RID: 17451 RVA: 0x0020915E File Offset: 0x0020735E
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new ShopPriceStatusMenu();
			yield break;
		}

		// Token: 0x0600442C RID: 17452 RVA: 0x00209170 File Offset: 0x00207370
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
