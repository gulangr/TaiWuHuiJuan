using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200054A RID: 1354
	public class MiscItemDetailedFilter : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x170007FD RID: 2045
		// (get) Token: 0x060043BF RID: 17343 RVA: 0x002080FE File Offset: 0x002062FE
		public override int Id
		{
			get
			{
				return 33;
			}
		}

		// Token: 0x060043C0 RID: 17344 RVA: 0x00208102 File Offset: 0x00206302
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new MiscItemTypeMenu();
			yield break;
		}

		// Token: 0x170007FE RID: 2046
		// (get) Token: 0x060043C1 RID: 17345 RVA: 0x00208112 File Offset: 0x00206312
		protected override int Level
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x060043C2 RID: 17346 RVA: 0x00208118 File Offset: 0x00206318
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(7, ToggleKey.CreateIndexKey(1))
			};
		}
	}
}
