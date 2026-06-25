using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000540 RID: 1344
	public class MedicineBuffTypeDetailedFilterLine : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x170007EF RID: 2031
		// (get) Token: 0x06004397 RID: 17303 RVA: 0x00207583 File Offset: 0x00205783
		public override int Id
		{
			get
			{
				return 13;
			}
		}

		// Token: 0x170007F0 RID: 2032
		// (get) Token: 0x06004398 RID: 17304 RVA: 0x00207587 File Offset: 0x00205787
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06004399 RID: 17305 RVA: 0x0020758A File Offset: 0x0020578A
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new BuffTypeMenu();
			yield break;
		}

		// Token: 0x0600439A RID: 17306 RVA: 0x0020759C File Offset: 0x0020579C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(2, ToggleKey.CreateIndexKey(6))
			};
		}
	}
}
