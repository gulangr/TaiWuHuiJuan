using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000562 RID: 1378
	public class UsingMedicineSortAndFilterController : ItemSortAndFilterController
	{
		// Token: 0x17000824 RID: 2084
		// (get) Token: 0x0600444C RID: 17484 RVA: 0x002096A4 File Offset: 0x002078A4
		protected override string FilterCustomOrderKey
		{
			get
			{
				return "UsingMedicineFilterCustomOrder";
			}
		}

		// Token: 0x0600444D RID: 17485 RVA: 0x002096AB File Offset: 0x002078AB
		public UsingMedicineSortAndFilterController(CommonSortAndFilter sortAndFilter) : base(sortAndFilter)
		{
			this.SortController = new ItemSortController();
		}
	}
}
