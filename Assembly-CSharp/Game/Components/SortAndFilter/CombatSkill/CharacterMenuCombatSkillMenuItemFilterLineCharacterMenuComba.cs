using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E2D RID: 3629
	public class CharacterMenuCombatSkillMenuItemFilterLineCharacterMenuCombatSkill : DetailedFilterLineLogic<CombatSkillDisplayDataCharacterMenuListItem>
	{
		// Token: 0x1700130A RID: 4874
		// (get) Token: 0x0600AB8A RID: 43914 RVA: 0x004EBD08 File Offset: 0x004E9F08
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AB8B RID: 43915 RVA: 0x004EBD0B File Offset: 0x004E9F0B
		protected override IEnumerable<DetailedFilterMenuLogic<CombatSkillDisplayDataCharacterMenuListItem>> GenerateMenus()
		{
			yield return new CombatSkillCharacterMenuItemTypeMenu();
			yield return new CharacterMenuItemSectMenu();
			yield return new CharacterMenuItemFiveElementsMenu();
			yield break;
		}

		// Token: 0x1700130B RID: 4875
		// (get) Token: 0x0600AB8C RID: 43916 RVA: 0x004EBD1B File Offset: 0x004E9F1B
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700130C RID: 4876
		// (get) Token: 0x0600AB8D RID: 43917 RVA: 0x004EBD1E File Offset: 0x004E9F1E
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600AB8E RID: 43918 RVA: 0x004EBD24 File Offset: 0x004E9F24
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
