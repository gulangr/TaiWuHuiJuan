using System;
using System.Collections.Generic;
using GameData.Domains.Taiwu;

namespace Game.Components.SortAndFilter.CharacterLocationDisplayData
{
	// Token: 0x02000E50 RID: 3664
	public class CharacterDetailDisplayDataSortAndFilterControllerController : SortAndFilterController<int>
	{
		// Token: 0x0600AC29 RID: 44073 RVA: 0x004EDB05 File Offset: 0x004EBD05
		public CharacterDetailDisplayDataSortAndFilterControllerController(ISortAndFilterView sortAndFilter, LanguageKey panelTitleKey = LanguageKey.EventEditor_Error_DuplicateGroupKey) : base(sortAndFilter, panelTitleKey)
		{
			this.SortController = new CharacterDetailDisplayDataSortController(this);
		}

		// Token: 0x0600AC2A RID: 44074 RVA: 0x004EDB1D File Offset: 0x004EBD1D
		public void SetData(TaiwuVillagerRoleDisplayData data)
		{
			this.Data = data;
		}

		// Token: 0x0600AC2B RID: 44075 RVA: 0x004EDB26 File Offset: 0x004EBD26
		protected override IEnumerable<FilterLineBase<int>> GenerateFilterLines()
		{
			yield return new CharacterTaskStatusLine(this);
			yield break;
		}

		// Token: 0x04008532 RID: 34098
		public TaiwuVillagerRoleDisplayData Data;
	}
}
