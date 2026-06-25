using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace CommonSortAndFilterLegacy.CombatSkill
{
	// Token: 0x02000571 RID: 1393
	public class CombatSkillSortAndFilterController : CommonSortAndFilterController<CombatSkillDisplayData>
	{
		// Token: 0x17000834 RID: 2100
		// (get) Token: 0x0600447D RID: 17533 RVA: 0x00209D05 File Offset: 0x00207F05
		protected override string FilterCustomOrderKey
		{
			get
			{
				return "CombatSkillFilterCustomOrder";
			}
		}

		// Token: 0x0600447E RID: 17534 RVA: 0x00209D0C File Offset: 0x00207F0C
		public CombatSkillSortAndFilterController(CommonSortAndFilter sortAndFilter) : base(sortAndFilter)
		{
			this.SortController = new CombatSkillSortController();
		}

		// Token: 0x0600447F RID: 17535 RVA: 0x00209D22 File Offset: 0x00207F22
		protected override IEnumerable<FilterLineBase<CombatSkillDisplayData>> GenerateFilterLines()
		{
			yield return new FirstFilterLine();
			yield return new SecondFilterLine();
			yield break;
		}
	}
}
