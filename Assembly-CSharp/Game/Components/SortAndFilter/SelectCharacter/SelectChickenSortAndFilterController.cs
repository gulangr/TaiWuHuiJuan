using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter.SelectChicken;
using GameData.Domains.Building;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000D02 RID: 3330
	public class SelectChickenSortAndFilterController : SortAndFilterController<Chicken>
	{
		// Token: 0x0600A6F0 RID: 42736 RVA: 0x004DAB6C File Offset: 0x004D8D6C
		public SelectChickenSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this.SortController = new SelectChickenSortController();
		}

		// Token: 0x0600A6F1 RID: 42737 RVA: 0x004DAB83 File Offset: 0x004D8D83
		protected override IEnumerable<FilterLineBase<Chicken>> GenerateFilterLines()
		{
			yield return new SelectChickenMainFilterLine();
			yield break;
		}
	}
}
