using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200054D RID: 1357
	public class MiscKeyItemDetailedFilter : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x17000801 RID: 2049
		// (get) Token: 0x060043CF RID: 17359 RVA: 0x00208410 File Offset: 0x00206610
		public override int Id
		{
			get
			{
				return 35;
			}
		}

		// Token: 0x060043D0 RID: 17360 RVA: 0x00208414 File Offset: 0x00206614
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new MiscKeyItemTypeMenu();
			yield break;
		}

		// Token: 0x17000802 RID: 2050
		// (get) Token: 0x060043D1 RID: 17361 RVA: 0x00208424 File Offset: 0x00206624
		protected override int Level
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x060043D2 RID: 17362 RVA: 0x00208428 File Offset: 0x00206628
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(7, ToggleKey.CreateIndexKey(3))
			};
		}
	}
}
