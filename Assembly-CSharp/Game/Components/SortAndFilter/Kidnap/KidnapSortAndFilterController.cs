using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Kidnap
{
	// Token: 0x02000D20 RID: 3360
	public class KidnapSortAndFilterController : SortAndFilterController<KidnapCharDisplayData>
	{
		// Token: 0x0600A75B RID: 42843 RVA: 0x004DDAE3 File Offset: 0x004DBCE3
		public KidnapSortAndFilterController(SortAndFilterLegacy sortAndFilter, Func<int, bool> isTaiwuFunc) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this._isTaiwuFunc = isTaiwuFunc;
			this.SortController = new KidnapSortController(isTaiwuFunc);
		}

		// Token: 0x0600A75C RID: 42844 RVA: 0x004DDB02 File Offset: 0x004DBD02
		protected override IEnumerable<FilterLineBase<KidnapCharDisplayData>> GenerateFilterLines()
		{
			yield break;
		}

		// Token: 0x04008368 RID: 33640
		private readonly Func<int, bool> _isTaiwuFunc;
	}
}
