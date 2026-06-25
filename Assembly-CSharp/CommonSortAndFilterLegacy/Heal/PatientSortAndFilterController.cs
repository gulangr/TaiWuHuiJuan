using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter.Heal;

namespace CommonSortAndFilterLegacy.Heal
{
	// Token: 0x0200056A RID: 1386
	public class PatientSortAndFilterController : CommonSortAndFilterController<HealPatientSortData>
	{
		// Token: 0x0600446C RID: 17516 RVA: 0x00209B94 File Offset: 0x00207D94
		public PatientSortAndFilterController(CommonSortAndFilter sortAndFilter) : base(sortAndFilter)
		{
			this.SortController = new PatientSortController();
		}

		// Token: 0x0600446D RID: 17517 RVA: 0x00209BAC File Offset: 0x00207DAC
		protected override IEnumerable<FilterLineBase<HealPatientSortData>> GenerateFilterLines()
		{
			return null;
		}

		// Token: 0x1700082E RID: 2094
		// (get) Token: 0x0600446E RID: 17518 RVA: 0x00209BBF File Offset: 0x00207DBF
		protected override string FilterCustomOrderKey
		{
			get
			{
				return "DoctorFilterCustomOrder";
			}
		}
	}
}
