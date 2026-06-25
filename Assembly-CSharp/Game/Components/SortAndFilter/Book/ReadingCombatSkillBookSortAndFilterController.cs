using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter.Item;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Book
{
	// Token: 0x02000E85 RID: 3717
	public class ReadingCombatSkillBookSortAndFilterController : ItemSortAndFilterController
	{
		// Token: 0x0600ACE6 RID: 44262 RVA: 0x004EFCE8 File Offset: 0x004EDEE8
		public ReadingCombatSkillBookSortAndFilterController(ISortAndFilterView sortAndFilter, Comparison<ITradeableContent> businessComparer = null) : base(sortAndFilter, LanguageKey.LK_Reading_Select_Book_FilterTitle)
		{
			bool flag = businessComparer != null;
			if (flag)
			{
				this.SortController = new ReadingMergedItemSortController(businessComparer);
			}
		}

		// Token: 0x0600ACE7 RID: 44263 RVA: 0x004EFD17 File Offset: 0x004EDF17
		protected override IEnumerable<FilterLineBase<ITradeableContent>> GenerateFilterLines()
		{
			yield return new ReadingCombatSkillBookFilterLine();
			yield break;
		}
	}
}
