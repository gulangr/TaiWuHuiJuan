using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000491 RID: 1169
	public class BookSecondaryFilterLine : SecondaryFilterLineBase<ItemDisplayData>
	{
		// Token: 0x17000700 RID: 1792
		// (get) Token: 0x06004144 RID: 16708 RVA: 0x0020123F File Offset: 0x001FF43F
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000701 RID: 1793
		// (get) Token: 0x06004145 RID: 16709 RVA: 0x00201242 File Offset: 0x001FF442
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06004146 RID: 16710 RVA: 0x00201245 File Offset: 0x001FF445
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new BookTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x06004147 RID: 16711 RVA: 0x00201258 File Offset: 0x001FF458
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(3))
			};
		}
	}
}
