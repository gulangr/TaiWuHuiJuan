using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter.Item;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Book
{
	// Token: 0x02000E86 RID: 3718
	public class ReadingLifeSkillBookSortAndFilterController : ItemSortAndFilterController
	{
		// Token: 0x0600ACE8 RID: 44264 RVA: 0x004EFD28 File Offset: 0x004EDF28
		public ReadingLifeSkillBookSortAndFilterController(ISortAndFilterView sortAndFilter, Comparison<ITradeableContent> businessComparer = null) : base(sortAndFilter, LanguageKey.LK_Reading_Select_Book_FilterTitle)
		{
			bool flag = businessComparer != null;
			if (flag)
			{
				this.SortController = new ReadingMergedItemSortController(businessComparer);
			}
		}

		// Token: 0x0600ACE9 RID: 44265 RVA: 0x004EFD57 File Offset: 0x004EDF57
		protected override IEnumerable<FilterLineBase<ITradeableContent>> GenerateFilterLines()
		{
			yield return new ReadingLifeSkillBookFilterLine();
			yield break;
		}
	}
}
