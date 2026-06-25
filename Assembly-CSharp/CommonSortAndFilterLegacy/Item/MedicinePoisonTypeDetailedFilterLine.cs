using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000546 RID: 1350
	public class MedicinePoisonTypeDetailedFilterLine : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x170007F9 RID: 2041
		// (get) Token: 0x060043B3 RID: 17331 RVA: 0x00207F81 File Offset: 0x00206181
		public override int Id
		{
			get
			{
				return 12;
			}
		}

		// Token: 0x170007FA RID: 2042
		// (get) Token: 0x060043B4 RID: 17332 RVA: 0x00207F85 File Offset: 0x00206185
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x060043B5 RID: 17333 RVA: 0x00207F88 File Offset: 0x00206188
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new PoisonTypeMenu();
			yield break;
		}

		// Token: 0x060043B6 RID: 17334 RVA: 0x00207F98 File Offset: 0x00206198
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(2, ToggleKey.CreateIndexKey(3)),
				new ToggleIdIndex(2, ToggleKey.CreateIndexKey(2))
			};
		}
	}
}
