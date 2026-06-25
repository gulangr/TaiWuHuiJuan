using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter.Item;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Book
{
	// Token: 0x02000E82 RID: 3714
	public class ModifyBookSortAndFilterController : ItemSortAndFilterController
	{
		// Token: 0x0600ACD9 RID: 44249 RVA: 0x004EF98C File Offset: 0x004EDB8C
		public ModifyBookSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.LK_CombatSkill)
		{
		}

		// Token: 0x0600ACDA RID: 44250 RVA: 0x004EF99C File Offset: 0x004EDB9C
		protected override IEnumerable<FilterLineBase<ITradeableContent>> GenerateFilterLines()
		{
			yield return new ModifyBookDetailedFilterLine();
			yield break;
		}
	}
}
