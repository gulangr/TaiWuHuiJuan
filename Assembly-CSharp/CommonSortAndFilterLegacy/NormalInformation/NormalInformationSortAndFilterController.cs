using System;
using System.Collections.Generic;
using CommonSortAndFilterLegacy.Information;
using GameData.Domains.Information;

namespace CommonSortAndFilterLegacy.NormalInformation
{
	// Token: 0x0200047E RID: 1150
	public class NormalInformationSortAndFilterController : CommonSortAndFilterController<NormalInformationDisplayData>
	{
		// Token: 0x060040FD RID: 16637 RVA: 0x00200ADD File Offset: 0x001FECDD
		public NormalInformationSortAndFilterController(CommonSortAndFilter sortAndFilter) : base(sortAndFilter)
		{
			this.SortController = new NormalInformationSortController();
		}

		// Token: 0x170006E4 RID: 1764
		// (get) Token: 0x060040FE RID: 16638 RVA: 0x00200AF3 File Offset: 0x001FECF3
		protected override string FilterCustomOrderKey
		{
			get
			{
				return "NormalInformationFilterCustomOrder";
			}
		}

		// Token: 0x060040FF RID: 16639 RVA: 0x00200AFA File Offset: 0x001FECFA
		protected override IEnumerable<FilterLineBase<NormalInformationDisplayData>> GenerateFilterLines()
		{
			yield return new TypeFilter();
			yield return new AreaFilter();
			yield return new SectFilter();
			yield return new LifeSkillTypeFilter();
			yield return new WesternFilter();
			yield return new SwordTombFilter();
			yield return new ProfessionFilter();
			yield break;
		}
	}
}
