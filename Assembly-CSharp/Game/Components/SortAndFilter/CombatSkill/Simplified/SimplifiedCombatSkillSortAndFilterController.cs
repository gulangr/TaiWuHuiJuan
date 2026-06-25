using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill.Simplified
{
	// Token: 0x02000E42 RID: 3650
	public class SimplifiedCombatSkillSortAndFilterController : SortAndFilterController<CombatSkillDisplayDataForList>
	{
		// Token: 0x0600ABDF RID: 43999 RVA: 0x004ECA77 File Offset: 0x004EAC77
		public SimplifiedCombatSkillSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.LK_CombatSkill)
		{
			this.SortController = new SimplifiedCombatSkillSortController();
		}

		// Token: 0x0600ABE0 RID: 44000 RVA: 0x004ECA92 File Offset: 0x004EAC92
		protected override IEnumerable<FilterLineBase<CombatSkillDisplayDataForList>> GenerateFilterLines()
		{
			yield return new SimplifiedCombatSkillFilterLine();
			yield break;
		}
	}
}
