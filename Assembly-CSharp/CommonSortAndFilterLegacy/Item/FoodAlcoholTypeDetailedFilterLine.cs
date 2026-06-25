using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004F3 RID: 1267
	public class FoodAlcoholTypeDetailedFilterLine : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x170007A4 RID: 1956
		// (get) Token: 0x060042B6 RID: 17078 RVA: 0x00204B77 File Offset: 0x00202D77
		public override int Id
		{
			get
			{
				return 11;
			}
		}

		// Token: 0x170007A5 RID: 1957
		// (get) Token: 0x060042B7 RID: 17079 RVA: 0x00204B7B File Offset: 0x00202D7B
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x060042B8 RID: 17080 RVA: 0x00204B7E File Offset: 0x00202D7E
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new AlcoholFoodTypeMenu();
			yield break;
		}

		// Token: 0x060042B9 RID: 17081 RVA: 0x00204B90 File Offset: 0x00202D90
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(1, ToggleKey.CreateIndexKey(3))
			};
		}
	}
}
