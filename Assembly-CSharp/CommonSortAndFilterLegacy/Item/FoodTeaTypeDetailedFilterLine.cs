using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004FE RID: 1278
	public class FoodTeaTypeDetailedFilterLine : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x170007B0 RID: 1968
		// (get) Token: 0x060042D8 RID: 17112 RVA: 0x00205449 File Offset: 0x00203649
		public override int Id
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x170007B1 RID: 1969
		// (get) Token: 0x060042D9 RID: 17113 RVA: 0x0020544D File Offset: 0x0020364D
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x060042DA RID: 17114 RVA: 0x00205450 File Offset: 0x00203650
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new TeaFoodTypeMenu();
			yield break;
		}

		// Token: 0x060042DB RID: 17115 RVA: 0x00205460 File Offset: 0x00203660
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(1, ToggleKey.CreateIndexKey(2))
			};
		}
	}
}
