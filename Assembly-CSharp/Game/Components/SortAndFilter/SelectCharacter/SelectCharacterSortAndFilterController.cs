using System;
using System.Collections.Generic;
using Game.Views.Select;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.SelectCharacter
{
	// Token: 0x02000CFF RID: 3327
	public class SelectCharacterSortAndFilterController : SortAndFilterController<ISelectCharacterData>
	{
		// Token: 0x0600A6CE RID: 42702 RVA: 0x004D88FB File Offset: 0x004D6AFB
		public SelectCharacterSortAndFilterController(ISortAndFilterView sortAndFilter, List<ESelectCharacterFilterMenuId> menuIds, List<short> defaultSortIds = null, bool skipFallbackSort = false) : base(sortAndFilter, LanguageKey.LK_CommonSortAndFilter_FilterPanel_Title_Character)
		{
			this._basicMenuIds = (menuIds ?? new List<ESelectCharacterFilterMenuId>());
			this.SortController = new SelectCharacterSortController(defaultSortIds, skipFallbackSort);
		}

		// Token: 0x0600A6CF RID: 42703 RVA: 0x004D8929 File Offset: 0x004D6B29
		protected override IEnumerable<FilterLineBase<ISelectCharacterData>> GenerateFilterLines()
		{
			bool flag = this._basicMenuIds.Count > 0;
			if (flag)
			{
				yield return new SelectCharacterMainFilterLine(this._basicMenuIds);
			}
			yield break;
		}

		// Token: 0x0400832F RID: 33583
		private readonly List<ESelectCharacterFilterMenuId> _basicMenuIds;
	}
}
