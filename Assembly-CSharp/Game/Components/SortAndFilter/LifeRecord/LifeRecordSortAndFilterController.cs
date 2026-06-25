using System;
using System.Collections.Generic;
using GameData.Domains.LifeRecord;

namespace Game.Components.SortAndFilter.LifeRecord
{
	// Token: 0x02000D1B RID: 3355
	public class LifeRecordSortAndFilterController : SortAndFilterController<TransferableRecord>
	{
		// Token: 0x0600A748 RID: 42824 RVA: 0x004DD7F5 File Offset: 0x004DB9F5
		public LifeRecordSortAndFilterController(ISortAndFilterView sortAndFilter) : base(sortAndFilter, LanguageKey.LK_SettlementPrison)
		{
			this.SortController = new LifeRecordSortController();
		}

		// Token: 0x0600A749 RID: 42825 RVA: 0x004DD810 File Offset: 0x004DBA10
		protected override IEnumerable<FilterLineBase<TransferableRecord>> GenerateFilterLines()
		{
			yield return new LifeRecordMainFilterLine();
			yield break;
		}
	}
}
