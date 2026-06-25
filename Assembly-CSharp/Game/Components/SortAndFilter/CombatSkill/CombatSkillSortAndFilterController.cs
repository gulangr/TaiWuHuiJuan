using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E1D RID: 3613
	public class CombatSkillSortAndFilterController : SortAndFilterController<IFilterableCombatSkill>
	{
		// Token: 0x0600AB40 RID: 43840 RVA: 0x004EACF1 File Offset: 0x004E8EF1
		public CombatSkillSortAndFilterController(ISortAndFilterView sortAndFilter, bool singleLine = false, EFilterType filterType = EFilterType.Common) : base(sortAndFilter, LanguageKey.LK_CombatSkill)
		{
			this.SortController = new CombatSkillSortController(filterType);
			this._singleLine = singleLine;
			this._filterType = filterType;
		}

		// Token: 0x0600AB41 RID: 43841 RVA: 0x004EAD1B File Offset: 0x004E8F1B
		protected override IEnumerable<FilterLineBase<IFilterableCombatSkill>> GenerateFilterLines()
		{
			bool singleLine = this._singleLine;
			if (singleLine)
			{
				yield return new SingleFilterLine(this._filterType);
			}
			else
			{
				yield return new FirstFilterLine();
				yield return new SecondFilterLine();
			}
			yield break;
		}

		// Token: 0x04008502 RID: 34050
		private bool _singleLine;

		// Token: 0x04008503 RID: 34051
		private EFilterType _filterType;
	}
}
