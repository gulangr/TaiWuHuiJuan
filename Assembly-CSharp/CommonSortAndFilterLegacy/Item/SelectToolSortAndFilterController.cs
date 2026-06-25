using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200055E RID: 1374
	public class SelectToolSortAndFilterController : ItemSortAndFilterController
	{
		// Token: 0x1700081D RID: 2077
		// (get) Token: 0x06004438 RID: 17464 RVA: 0x002092DC File Offset: 0x002074DC
		protected override string FilterCustomOrderKey
		{
			get
			{
				return "SelectToolFilterCustomOrder";
			}
		}

		// Token: 0x06004439 RID: 17465 RVA: 0x002092E3 File Offset: 0x002074E3
		public SelectToolSortAndFilterController(CommonSortAndFilter sortAndFilter) : base(sortAndFilter)
		{
			this.SortController = new ItemSortController();
		}

		// Token: 0x0600443A RID: 17466 RVA: 0x002092F9 File Offset: 0x002074F9
		protected override IEnumerable<FilterLineBase<ItemDisplayData>> GenerateFilterLines()
		{
			yield return new SelectToolFilterLine();
			yield break;
		}
	}
}
