using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E33 RID: 3635
	public class CombatSkillCharacterMenuItemFirstFilterLine : DetailedFilterLineLogic<CombatSkillDisplayDataCharacterMenuListItem>
	{
		// Token: 0x17001312 RID: 4882
		// (get) Token: 0x0600ABA0 RID: 43936 RVA: 0x004EBF5A File Offset: 0x004EA15A
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600ABA1 RID: 43937 RVA: 0x004EBF5D File Offset: 0x004EA15D
		protected override IEnumerable<DetailedFilterMenuLogic<CombatSkillDisplayDataCharacterMenuListItem>> GenerateMenus()
		{
			yield return new CombatSkillCharacterMenuItemTypeMenu();
			yield return new CharacterMenuItemSectMenu();
			yield return new CharacterMenuItemFiveElementsMenu();
			yield break;
		}

		// Token: 0x17001313 RID: 4883
		// (get) Token: 0x0600ABA2 RID: 43938 RVA: 0x004EBF6D File Offset: 0x004EA16D
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17001314 RID: 4884
		// (get) Token: 0x0600ABA3 RID: 43939 RVA: 0x004EBF70 File Offset: 0x004EA170
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600ABA4 RID: 43940 RVA: 0x004EBF74 File Offset: 0x004EA174
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
