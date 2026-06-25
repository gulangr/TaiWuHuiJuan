using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000503 RID: 1283
	public class FoodVegetarianTypeDetailedFilterLine : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x170007B4 RID: 1972
		// (get) Token: 0x060042E3 RID: 17123 RVA: 0x00205609 File Offset: 0x00203809
		public override int Id
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x170007B5 RID: 1973
		// (get) Token: 0x060042E4 RID: 17124 RVA: 0x0020560C File Offset: 0x0020380C
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x060042E5 RID: 17125 RVA: 0x0020560F File Offset: 0x0020380F
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new FoodRecoverTypeMenu();
			yield return new FoodBuffTypeMenu();
			yield break;
		}

		// Token: 0x060042E6 RID: 17126 RVA: 0x00205620 File Offset: 0x00203820
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(1, ToggleKey.CreateIndexKey(0))
			};
		}
	}
}
