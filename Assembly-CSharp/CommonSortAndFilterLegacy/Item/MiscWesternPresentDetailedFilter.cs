using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000558 RID: 1368
	public class MiscWesternPresentDetailedFilter : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x17000812 RID: 2066
		// (get) Token: 0x06004412 RID: 17426 RVA: 0x00208E55 File Offset: 0x00207055
		public override int Id
		{
			get
			{
				return 38;
			}
		}

		// Token: 0x06004413 RID: 17427 RVA: 0x00208E59 File Offset: 0x00207059
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new MiscWesternPresentTypeMenu();
			yield break;
		}

		// Token: 0x17000813 RID: 2067
		// (get) Token: 0x06004414 RID: 17428 RVA: 0x00208E69 File Offset: 0x00207069
		protected override int Level
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x06004415 RID: 17429 RVA: 0x00208E6C File Offset: 0x0020706C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(7, ToggleKey.CreateIndexKey(6))
			};
		}
	}
}
