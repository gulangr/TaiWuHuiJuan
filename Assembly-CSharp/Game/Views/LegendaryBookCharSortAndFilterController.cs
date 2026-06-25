using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter;
using GameData.Domains.LegendaryBook;

namespace Game.Views
{
	// Token: 0x020006EC RID: 1772
	public class LegendaryBookCharSortAndFilterController : SortAndFilterController<LegendaryBookCharacterRelatedData>
	{
		// Token: 0x06005444 RID: 21572 RVA: 0x00270659 File Offset: 0x0026E859
		public LegendaryBookCharSortAndFilterController(SortAndFilterLegacy sortAndFilter, Func<int, bool> isTaiwuFunc, Func<int, bool> isSpecialTeammateFunc) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this._isTaiwuFunc = isTaiwuFunc;
			this._isSpecialTeammateFunc = isSpecialTeammateFunc;
			this.SortController = new LegendaryBookCharacterSortController(isTaiwuFunc, isSpecialTeammateFunc);
		}

		// Token: 0x06005445 RID: 21573 RVA: 0x00270687 File Offset: 0x0026E887
		public void SetSubPage(LegendaryBookCharacterSubPage subPage)
		{
			this._currentSubPage = subPage;
		}

		// Token: 0x06005446 RID: 21574 RVA: 0x00270691 File Offset: 0x0026E891
		protected override IEnumerable<FilterLineBase<LegendaryBookCharacterRelatedData>> GenerateFilterLines()
		{
			yield return new LegendaryBookCharacterFilterLineLogic();
			yield break;
		}

		// Token: 0x0400390C RID: 14604
		private readonly Func<int, bool> _isTaiwuFunc;

		// Token: 0x0400390D RID: 14605
		private readonly Func<int, bool> _isSpecialTeammateFunc;

		// Token: 0x0400390E RID: 14606
		private LegendaryBookCharacterSubPage _currentSubPage = LegendaryBookCharacterSubPage.LegendaryBook;
	}
}
