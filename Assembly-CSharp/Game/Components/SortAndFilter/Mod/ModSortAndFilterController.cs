using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Mod
{
	// Token: 0x02000D17 RID: 3351
	public class ModSortAndFilterController : SortAndFilterController<ModSortAndFilterData>
	{
		// Token: 0x0600A736 RID: 42806 RVA: 0x004DD39D File Offset: 0x004DB59D
		public ModSortAndFilterController(SortAndFilter sortAndFilter) : base(sortAndFilter, LanguageKey.LK_Mod)
		{
			this.SortController = new ModSortController();
		}

		// Token: 0x0600A737 RID: 42807 RVA: 0x004DD3B8 File Offset: 0x004DB5B8
		protected override IEnumerable<FilterLineBase<ModSortAndFilterData>> GenerateFilterLines()
		{
			yield return new TagFilterLine();
			yield break;
		}
	}
}
