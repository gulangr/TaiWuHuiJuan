using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter.Item;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Book
{
	// Token: 0x02000E80 RID: 3712
	public class LifeSkillBookSortAndFilterController : ItemSortAndFilterController
	{
		// Token: 0x0600ACD5 RID: 44245 RVA: 0x004EF950 File Offset: 0x004EDB50
		public LifeSkillBookSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.LK_Reading_Select_Book_FilterTitle)
		{
		}

		// Token: 0x0600ACD6 RID: 44246 RVA: 0x004EF960 File Offset: 0x004EDB60
		protected override IEnumerable<FilterLineBase<ITradeableContent>> GenerateFilterLines()
		{
			yield return new LifeSkillBookFilterLine();
			yield break;
		}
	}
}
