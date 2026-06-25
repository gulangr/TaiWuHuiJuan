using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E2B RID: 3627
	public class CharacterMenuCombatSkillMenuItemFilterLine2 : DetailedFilterLineLogic<CombatSkillDisplayDataCharacterMenuListItem>
	{
		// Token: 0x17001306 RID: 4870
		// (get) Token: 0x0600AB82 RID: 43906 RVA: 0x004EBCC4 File Offset: 0x004E9EC4
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AB83 RID: 43907 RVA: 0x004EBCC7 File Offset: 0x004E9EC7
		protected override IEnumerable<DetailedFilterMenuLogic<CombatSkillDisplayDataCharacterMenuListItem>> GenerateMenus()
		{
			yield return new CharacterMenuItemFiveElementsMenu();
			yield break;
		}

		// Token: 0x17001307 RID: 4871
		// (get) Token: 0x0600AB84 RID: 43908 RVA: 0x004EBCD7 File Offset: 0x004E9ED7
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17001308 RID: 4872
		// (get) Token: 0x0600AB85 RID: 43909 RVA: 0x004EBCDA File Offset: 0x004E9EDA
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600AB86 RID: 43910 RVA: 0x004EBCE0 File Offset: 0x004E9EE0
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
