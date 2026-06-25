using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Jieqing
{
	// Token: 0x02000D22 RID: 3362
	public class JieqingMurderSortAndFilterController : SortAndFilterController<CharacterDisplayData>
	{
		// Token: 0x0600A764 RID: 42852 RVA: 0x004DEDD4 File Offset: 0x004DCFD4
		public JieqingMurderSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.LK_SettlementPrison)
		{
			this.SortController = new JieqingMurderSortController();
		}

		// Token: 0x0600A765 RID: 42853 RVA: 0x004DEDEF File Offset: 0x004DCFEF
		protected override IEnumerable<FilterLineBase<CharacterDisplayData>> GenerateFilterLines()
		{
			yield return new JieqingMurderMainFilterLine();
			yield break;
		}
	}
}
