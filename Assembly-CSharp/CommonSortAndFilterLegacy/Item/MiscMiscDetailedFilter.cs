using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000550 RID: 1360
	public class MiscMiscDetailedFilter : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x17000805 RID: 2053
		// (get) Token: 0x060043EE RID: 17390 RVA: 0x00208916 File Offset: 0x00206B16
		public override int Id
		{
			get
			{
				return 39;
			}
		}

		// Token: 0x060043EF RID: 17391 RVA: 0x0020891A File Offset: 0x00206B1A
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new MiscMiscTypeMenu();
			yield break;
		}

		// Token: 0x17000806 RID: 2054
		// (get) Token: 0x060043F0 RID: 17392 RVA: 0x0020892A File Offset: 0x00206B2A
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x060043F1 RID: 17393 RVA: 0x00208930 File Offset: 0x00206B30
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(7, ToggleKey.CreateIndexKey(7))
			};
		}
	}
}
