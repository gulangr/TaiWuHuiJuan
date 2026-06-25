using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.CharacterLocationDisplayData
{
	// Token: 0x02000E57 RID: 3671
	public class CharacterLocationDisplayDataSortAndFilterControllerController : SortAndFilterController<CharacterLocationDisplayData>
	{
		// Token: 0x0600AC3C RID: 44092 RVA: 0x004EDE36 File Offset: 0x004EC036
		public CharacterLocationDisplayDataSortAndFilterControllerController(ISortAndFilterView sortAndFilter, LanguageKey panelTitleKey = LanguageKey.EventEditor_Error_DuplicateGroupKey) : base(sortAndFilter, panelTitleKey)
		{
			this.SortController = new CharacterLocationDisplayDataSortController();
		}

		// Token: 0x0600AC3D RID: 44093 RVA: 0x004EDE4D File Offset: 0x004EC04D
		protected override IEnumerable<FilterLineBase<CharacterLocationDisplayData>> GenerateFilterLines()
		{
			yield return new CharacterLocationDisplayDataMainTypeFilterLine();
			yield break;
		}
	}
}
