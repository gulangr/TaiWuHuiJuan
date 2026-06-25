using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy.Villager
{
	// Token: 0x0200046C RID: 1132
	public class VillagerSortAndFilterController<T> : CommonSortAndFilterController<T> where T : IVillagerSortAndFilterData
	{
		// Token: 0x060040AC RID: 16556 RVA: 0x001FFE8B File Offset: 0x001FE08B
		public VillagerSortAndFilterController(CommonSortAndFilter sortAndFilter) : base(sortAndFilter)
		{
			this.SortController = new VillagerSortController<T>();
		}

		// Token: 0x060040AD RID: 16557 RVA: 0x001FFEA1 File Offset: 0x001FE0A1
		protected override IEnumerable<FilterLineBase<T>> GenerateFilterLines()
		{
			yield return new MainFilterLine<T>();
			yield break;
		}

		// Token: 0x170006C0 RID: 1728
		// (get) Token: 0x060040AE RID: 16558 RVA: 0x001FFEB1 File Offset: 0x001FE0B1
		protected override string FilterCustomOrderKey
		{
			get
			{
				return "VillagerFilterCustomOrder";
			}
		}
	}
}
