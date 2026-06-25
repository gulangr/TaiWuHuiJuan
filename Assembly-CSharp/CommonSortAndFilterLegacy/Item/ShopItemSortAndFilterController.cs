using System;
using System.Collections.Generic;
using System.Linq;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200055A RID: 1370
	public class ShopItemSortAndFilterController : ItemSortAndFilterController
	{
		// Token: 0x17000816 RID: 2070
		// (get) Token: 0x06004426 RID: 17446 RVA: 0x0020910C File Offset: 0x0020730C
		protected override string FilterCustomOrderKey
		{
			get
			{
				return "ShopItemFilterCustomOrder";
			}
		}

		// Token: 0x06004427 RID: 17447 RVA: 0x00209113 File Offset: 0x00207313
		public ShopItemSortAndFilterController(CommonSortAndFilter sortAndFilter) : base(sortAndFilter)
		{
			this.SortController = new ItemSortController();
		}

		// Token: 0x06004428 RID: 17448 RVA: 0x0020912C File Offset: 0x0020732C
		protected override IEnumerable<FilterLineBase<ItemDisplayData>> GenerateFilterLines()
		{
			List<FilterLineBase<ItemDisplayData>> lines = base.GenerateFilterLines().ToList<FilterLineBase<ItemDisplayData>>();
			lines.Add(new ShopPriceSecondaryMenu());
			return lines;
		}
	}
}
