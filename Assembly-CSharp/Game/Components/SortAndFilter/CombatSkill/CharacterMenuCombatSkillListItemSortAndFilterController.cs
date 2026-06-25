using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E26 RID: 3622
	public class CharacterMenuCombatSkillListItemSortAndFilterController : SortAndFilterController<CombatSkillDisplayDataCharacterMenuListItem>
	{
		// Token: 0x0600AB72 RID: 43890 RVA: 0x004EBBF8 File Offset: 0x004E9DF8
		public CharacterMenuCombatSkillListItemSortAndFilterController(SortAndFilter sortAndFilter, bool singleLineFilter, bool isCharacterMenuCombatSkill) : base(sortAndFilter, LanguageKey.EventEditor_Error_DuplicateGroupKey)
		{
			this._singleLine = singleLineFilter;
			this._isCharacterMenuCombatSkill = isCharacterMenuCombatSkill;
			this.SortController = new CombatSkillCharacterMenuListItemSortController();
		}

		// Token: 0x0600AB73 RID: 43891 RVA: 0x004EBC1D File Offset: 0x004E9E1D
		protected override IEnumerable<FilterLineBase<CombatSkillDisplayDataCharacterMenuListItem>> GenerateFilterLines()
		{
			bool isCharacterMenuCombatSkill = this._isCharacterMenuCombatSkill;
			if (isCharacterMenuCombatSkill)
			{
				yield return new CharacterMenuCombatSkillMenuItemFilterLineCharacterMenuCombatSkill();
			}
			else
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
			}
			yield break;
		}

		// Token: 0x04008510 RID: 34064
		private bool _singleLine;

		// Token: 0x04008511 RID: 34065
		private bool _isCharacterMenuCombatSkill;
	}
}
