using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Relationship
{
	// Token: 0x02000CE6 RID: 3302
	public class RelationshipSortAndFilterController : SortAndFilterController<CharacterDisplayDataForRelations>
	{
		// Token: 0x0600A665 RID: 42597 RVA: 0x004D6F40 File Offset: 0x004D5140
		public RelationshipSortAndFilterController(SortAndFilterLegacy sortAndFilter, Func<int, bool> isTaiwuFunc, Func<int, bool> isSpecialTeammateFunc) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this._isTaiwuFunc = isTaiwuFunc;
			this._isSpecialTeammateFunc = isSpecialTeammateFunc;
			this.SortController = new RelationshipSortController(isTaiwuFunc, isSpecialTeammateFunc);
		}

		// Token: 0x0600A666 RID: 42598 RVA: 0x004D6F67 File Offset: 0x004D5167
		protected override IEnumerable<FilterLineBase<CharacterDisplayDataForRelations>> GenerateFilterLines()
		{
			yield break;
		}

		// Token: 0x04008314 RID: 33556
		private readonly Func<int, bool> _isTaiwuFunc;

		// Token: 0x04008315 RID: 33557
		private readonly Func<int, bool> _isSpecialTeammateFunc;
	}
}
