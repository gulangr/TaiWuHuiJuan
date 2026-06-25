using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter;
using GameData.Domains.Character.Display;

namespace Game.Views
{
	// Token: 0x02000716 RID: 1814
	public class VillagerSortAndFilterController : SortAndFilterController<VillagerCharDisplayData>
	{
		// Token: 0x06005690 RID: 22160 RVA: 0x00281C7D File Offset: 0x0027FE7D
		public VillagerSortAndFilterController(SortAndFilterLegacy sortAndFilter, Func<int, bool> isTaiwuFunc, Func<int, bool> isSpecialTeammateFunc) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this._isTaiwuFunc = isTaiwuFunc;
			this._isSpecialTeammateFunc = isSpecialTeammateFunc;
			this.SortController = new VillagerSortController(isTaiwuFunc, isSpecialTeammateFunc);
		}

		// Token: 0x06005691 RID: 22161 RVA: 0x00281CAB File Offset: 0x0027FEAB
		public void SetSubPage(VillagerSubPage subPage)
		{
			this._currentSubPage = subPage;
		}

		// Token: 0x06005692 RID: 22162 RVA: 0x00281CB5 File Offset: 0x0027FEB5
		protected override IEnumerable<FilterLineBase<VillagerCharDisplayData>> GenerateFilterLines()
		{
			yield return new VillagerFilterLineLogic();
			yield break;
		}

		// Token: 0x04003B22 RID: 15138
		private readonly Func<int, bool> _isTaiwuFunc;

		// Token: 0x04003B23 RID: 15139
		private readonly Func<int, bool> _isSpecialTeammateFunc;

		// Token: 0x04003B24 RID: 15140
		private VillagerSubPage _currentSubPage = VillagerSubPage.Villager;
	}
}
