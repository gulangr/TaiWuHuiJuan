using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E2A RID: 3626
	public class CharacterMenuCombatSkillMenuItemFilterLine : DetailedFilterLineLogic<CombatSkillDisplayDataCharacterMenuListItem>
	{
		// Token: 0x17001303 RID: 4867
		// (get) Token: 0x0600AB7C RID: 43900 RVA: 0x004EBC8C File Offset: 0x004E9E8C
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AB7D RID: 43901 RVA: 0x004EBC8F File Offset: 0x004E9E8F
		protected override IEnumerable<DetailedFilterMenuLogic<CombatSkillDisplayDataCharacterMenuListItem>> GenerateMenus()
		{
			yield return new CombatSkillCharacterMenuItemTypeMenu();
			yield return new CharacterMenuItemSectMenu();
			yield return new CombatSkillMenuItemStatusMenuForCharacterMenu();
			yield break;
		}

		// Token: 0x17001304 RID: 4868
		// (get) Token: 0x0600AB7E RID: 43902 RVA: 0x004EBC9F File Offset: 0x004E9E9F
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17001305 RID: 4869
		// (get) Token: 0x0600AB7F RID: 43903 RVA: 0x004EBCA2 File Offset: 0x004E9EA2
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600AB80 RID: 43904 RVA: 0x004EBCA8 File Offset: 0x004E9EA8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
