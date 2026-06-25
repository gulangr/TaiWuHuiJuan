using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter.Heal;

namespace CommonSortAndFilterLegacy.Heal
{
	// Token: 0x02000568 RID: 1384
	public class DoctorSortAndFilterController : CommonSortAndFilterController<HealDoctorSortData>
	{
		// Token: 0x06004466 RID: 17510 RVA: 0x00209AB4 File Offset: 0x00207CB4
		public DoctorSortAndFilterController(CommonSortAndFilter sortAndFilter) : base(sortAndFilter)
		{
			this.SortController = new DoctorSortController();
		}

		// Token: 0x06004467 RID: 17511 RVA: 0x00209ACC File Offset: 0x00207CCC
		protected override IEnumerable<FilterLineBase<HealDoctorSortData>> GenerateFilterLines()
		{
			return null;
		}

		// Token: 0x1700082D RID: 2093
		// (get) Token: 0x06004468 RID: 17512 RVA: 0x00209ADF File Offset: 0x00207CDF
		protected override string FilterCustomOrderKey
		{
			get
			{
				return "DoctorFilterCustomOrder";
			}
		}
	}
}
