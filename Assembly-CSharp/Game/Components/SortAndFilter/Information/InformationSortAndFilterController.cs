using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Information
{
	// Token: 0x02000E10 RID: 3600
	public class InformationSortAndFilterController : SortAndFilterController<InformationSortAndFilterData>
	{
		// Token: 0x0600AB25 RID: 43813 RVA: 0x004EA569 File Offset: 0x004E8769
		public InformationSortAndFilterController(SortAndFilter sortAndFilter, bool isFull) : base(sortAndFilter, LanguageKey.LK_Information)
		{
			this.SortController = new InformationSortController();
			this._isFull = isFull;
		}

		// Token: 0x0600AB26 RID: 43814 RVA: 0x004EA58B File Offset: 0x004E878B
		protected override IEnumerable<FilterLineBase<InformationSortAndFilterData>> GenerateFilterLines()
		{
			bool isFull = this._isFull;
			if (isFull)
			{
				yield return new FullInformationFilterLine();
			}
			else
			{
				yield return new InformationFilterLine();
			}
			yield break;
		}

		// Token: 0x040084E5 RID: 34021
		private bool _isFull;
	}
}
