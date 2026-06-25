using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter.Item;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Book
{
	// Token: 0x02000E7E RID: 3710
	public class CombatSkillBookSortAndFilterController : ItemSortAndFilterController
	{
		// Token: 0x0600ACD1 RID: 44241 RVA: 0x004EF914 File Offset: 0x004EDB14
		public CombatSkillBookSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.LK_Reading_Select_Book_FilterTitle)
		{
		}

		// Token: 0x0600ACD2 RID: 44242 RVA: 0x004EF924 File Offset: 0x004EDB24
		protected override IEnumerable<FilterLineBase<ITradeableContent>> GenerateFilterLines()
		{
			yield return new CombatSkillBookFilterLine();
			yield break;
		}
	}
}
