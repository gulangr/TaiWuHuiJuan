using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter;
using GameData.Domains.Character.Display;

namespace Game.Views
{
	// Token: 0x02000720 RID: 1824
	public class VillagerCharacterScrollController : SortAndFilterController<VillagerCharDisplayData>
	{
		// Token: 0x06005744 RID: 22340 RVA: 0x00288910 File Offset: 0x00286B10
		public VillagerCharacterScrollController(SortAndFilter sortAndFilter, Func<int, bool> isTaiwuFunc, Func<int, bool> isSpecialTeammateFunc) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this._isTaiwuFunc = isTaiwuFunc;
			this._isSpecialTeammateFunc = isSpecialTeammateFunc;
			this.SortController = new VillagerSortController(isTaiwuFunc, isSpecialTeammateFunc);
		}

		// Token: 0x06005745 RID: 22341 RVA: 0x0028893E File Offset: 0x00286B3E
		public void SetSubPage(VillagerSubPage subPage)
		{
			this._currentSubPage = subPage;
		}

		// Token: 0x06005746 RID: 22342 RVA: 0x00288948 File Offset: 0x00286B48
		protected override IEnumerable<FilterLineBase<VillagerCharDisplayData>> GenerateFilterLines()
		{
			yield return new VillagerFilterLineLogic();
			yield break;
		}

		// Token: 0x04003B9E RID: 15262
		private readonly Func<int, bool> _isTaiwuFunc;

		// Token: 0x04003B9F RID: 15263
		private readonly Func<int, bool> _isSpecialTeammateFunc;

		// Token: 0x04003BA0 RID: 15264
		private VillagerSubPage _currentSubPage = VillagerSubPage.Villager;
	}
}
