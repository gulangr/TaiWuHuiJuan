using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E29 RID: 3625
	public class CharacterMenuCombatSkillMenuItemFilterLineSingleLine : DetailedFilterLineLogic<CombatSkillDisplayDataCharacterMenuListItem>
	{
		// Token: 0x17001300 RID: 4864
		// (get) Token: 0x0600AB76 RID: 43894 RVA: 0x004EBC54 File Offset: 0x004E9E54
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AB77 RID: 43895 RVA: 0x004EBC57 File Offset: 0x004E9E57
		protected override IEnumerable<DetailedFilterMenuLogic<CombatSkillDisplayDataCharacterMenuListItem>> GenerateMenus()
		{
			yield return new CombatSkillCharacterMenuItemTypeMenu();
			yield return new CharacterMenuItemSectMenu();
			yield return new CharacterMenuItemFiveElementsMenu();
			yield return new CombatSkillMenuItemStatusMenuForCharacterMenu();
			yield break;
		}

		// Token: 0x17001301 RID: 4865
		// (get) Token: 0x0600AB78 RID: 43896 RVA: 0x004EBC67 File Offset: 0x004E9E67
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17001302 RID: 4866
		// (get) Token: 0x0600AB79 RID: 43897 RVA: 0x004EBC6A File Offset: 0x004E9E6A
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600AB7A RID: 43898 RVA: 0x004EBC70 File Offset: 0x004E9E70
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
