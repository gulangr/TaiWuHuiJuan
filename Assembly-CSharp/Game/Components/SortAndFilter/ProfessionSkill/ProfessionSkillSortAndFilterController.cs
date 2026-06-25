using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.ProfessionSkill
{
	// Token: 0x02000D0C RID: 3340
	public class ProfessionSkillSortAndFilterController : SortAndFilterController<ProfessionSkillSortData>
	{
		// Token: 0x0600A710 RID: 42768 RVA: 0x004DBB19 File Offset: 0x004D9D19
		public ProfessionSkillSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this.SortController = new ProfessionSkillSortController();
		}

		// Token: 0x0600A711 RID: 42769 RVA: 0x004DBB30 File Offset: 0x004D9D30
		protected override IEnumerable<FilterLineBase<ProfessionSkillSortData>> GenerateFilterLines()
		{
			yield break;
		}
	}
}
