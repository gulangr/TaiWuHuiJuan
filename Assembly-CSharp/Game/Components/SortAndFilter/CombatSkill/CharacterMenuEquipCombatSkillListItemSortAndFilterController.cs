using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E2E RID: 3630
	public class CharacterMenuEquipCombatSkillListItemSortAndFilterController : SortAndFilterController<CombatSkillDisplayDataCharacterMenuListItem>
	{
		// Token: 0x0600AB90 RID: 43920 RVA: 0x004EBD40 File Offset: 0x004E9F40
		public CharacterMenuEquipCombatSkillListItemSortAndFilterController(SortAndFilter sortAndFilter, bool singleLineFilter) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this._singleLine = singleLineFilter;
			this.SortController = new CombatSkillCharacterMenuListItemSortController();
		}

		// Token: 0x0600AB91 RID: 43921 RVA: 0x004EBD5E File Offset: 0x004E9F5E
		protected override IEnumerable<FilterLineBase<CombatSkillDisplayDataCharacterMenuListItem>> GenerateFilterLines()
		{
			bool singleLine = this._singleLine;
			if (singleLine)
			{
				yield return new CharacterMenuCombatSkillMenuItemFilterLineSingleLine();
			}
			else
			{
				yield return new CharacterMenuCombatSkillMenuItemFilterLine();
				yield return new CharacterMenuCombatSkillMenuItemFilterLine2();
			}
			yield break;
		}

		// Token: 0x04008517 RID: 34071
		private bool _singleLine;
	}
}
