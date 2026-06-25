using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E27 RID: 3623
	public class CharacterMenuCombatSkillListItemSortAndFilterController2 : SortAndFilterController<CombatSkillDisplayDataCharacterMenuListItem>
	{
		// Token: 0x0600AB74 RID: 43892 RVA: 0x004EBC2D File Offset: 0x004E9E2D
		public CharacterMenuCombatSkillListItemSortAndFilterController2(SortAndFilterLegacy sortAndFilter) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this.SortController = new CombatSkillCharacterMenuListItemSortController();
		}

		// Token: 0x0600AB75 RID: 43893 RVA: 0x004EBC44 File Offset: 0x004E9E44
		protected override IEnumerable<FilterLineBase<CombatSkillDisplayDataCharacterMenuListItem>> GenerateFilterLines()
		{
			yield return new CharacterMenuCombatSkillMenuItemFilterLineSingleLine();
			yield break;
		}
	}
}
