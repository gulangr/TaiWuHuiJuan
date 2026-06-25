using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E30 RID: 3632
	public class CharacterMenuItemCombatSkillSecondFilterLine : DetailedFilterLineLogic<CombatSkillDisplayDataCharacterMenuListItem>
	{
		// Token: 0x1700130D RID: 4877
		// (get) Token: 0x0600AB92 RID: 43922 RVA: 0x004EBD6E File Offset: 0x004E9F6E
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AB93 RID: 43923 RVA: 0x004EBD71 File Offset: 0x004E9F71
		protected override IEnumerable<DetailedFilterMenuLogic<CombatSkillDisplayDataCharacterMenuListItem>> GenerateMenus()
		{
			yield return new CombatSkillCharacterMenuItemStatusMenu();
			yield break;
		}

		// Token: 0x1700130E RID: 4878
		// (get) Token: 0x0600AB94 RID: 43924 RVA: 0x004EBD81 File Offset: 0x004E9F81
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700130F RID: 4879
		// (get) Token: 0x0600AB95 RID: 43925 RVA: 0x004EBD84 File Offset: 0x004E9F84
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600AB96 RID: 43926 RVA: 0x004EBD88 File Offset: 0x004E9F88
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
