using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Prison
{
	// Token: 0x02000D13 RID: 3347
	public class PrisonSortAndFilterController : SortAndFilterController<CharacterDisplayDataForSettlementPrisoner>
	{
		// Token: 0x0600A724 RID: 42788 RVA: 0x004DBEBC File Offset: 0x004DA0BC
		public PrisonSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.LK_SettlementPrison)
		{
			this.SortController = new PrisonerSortController();
		}

		// Token: 0x0600A725 RID: 42789 RVA: 0x004DBED7 File Offset: 0x004DA0D7
		protected override IEnumerable<FilterLineBase<CharacterDisplayDataForSettlementPrisoner>> GenerateFilterLines()
		{
			yield return new MainFilterLine();
			yield break;
		}
	}
}
