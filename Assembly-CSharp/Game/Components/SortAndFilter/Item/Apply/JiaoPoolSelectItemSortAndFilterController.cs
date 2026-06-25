using System;
using System.Collections.Generic;
using GameData.DLC.FiveLoong;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item.Apply
{
	// Token: 0x02000DF2 RID: 3570
	public class JiaoPoolSelectItemSortAndFilterController : ItemSortAndFilterController
	{
		// Token: 0x0600AA9C RID: 43676 RVA: 0x004E8DDB File Offset: 0x004E6FDB
		public JiaoPoolSelectItemSortAndFilterController(ISortAndFilterView sortAndFilter, bool isInvestMode, IReadOnlyDictionary<ItemKey, Jiao> jiaoByKey) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this._isInvestMode = isInvestMode;
			this._jiaoByKey = jiaoByKey;
			this.SortController = new ItemSortController();
		}

		// Token: 0x0600AA9D RID: 43677 RVA: 0x004E8E00 File Offset: 0x004E7000
		protected override IEnumerable<FilterLineBase<ITradeableContent>> GenerateFilterLines()
		{
			bool isInvestMode = this._isInvestMode;
			if (isInvestMode)
			{
				yield return new JiaoPoolInvestFilterLine(this._jiaoByKey);
			}
			else
			{
				yield return new JiaoPoolBreedingSourceFilterLine();
			}
			yield break;
		}

		// Token: 0x040084D6 RID: 34006
		private readonly bool _isInvestMode;

		// Token: 0x040084D7 RID: 34007
		private readonly IReadOnlyDictionary<ItemKey, Jiao> _jiaoByKey;
	}
}
