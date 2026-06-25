using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill.Simplified
{
	// Token: 0x02000E44 RID: 3652
	public class SimplifiedCombatSkillFilterLine : DetailedFilterLineLogic<CombatSkillDisplayDataForList>
	{
		// Token: 0x17001328 RID: 4904
		// (get) Token: 0x0600ABE7 RID: 44007 RVA: 0x004ECD0C File Offset: 0x004EAF0C
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600ABE8 RID: 44008 RVA: 0x004ECD0F File Offset: 0x004EAF0F
		protected override IEnumerable<DetailedFilterMenuLogic<CombatSkillDisplayDataForList>> GenerateMenus()
		{
			yield return new CombatSkillQiTypeMenu();
			yield return new AttackCombatSkillTypeMenu();
			yield return new SectMenu();
			yield return new FiveElementsMenu();
			yield return new CombatSkillStatusMenu();
			yield break;
		}

		// Token: 0x17001329 RID: 4905
		// (get) Token: 0x0600ABE9 RID: 44009 RVA: 0x004ECD1F File Offset: 0x004EAF1F
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700132A RID: 4906
		// (get) Token: 0x0600ABEA RID: 44010 RVA: 0x004ECD22 File Offset: 0x004EAF22
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600ABEB RID: 44011 RVA: 0x004ECD28 File Offset: 0x004EAF28
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
