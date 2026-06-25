using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E20 RID: 3616
	public class FirstFilterLine : DetailedFilterLineLogic<IFilterableCombatSkill>
	{
		// Token: 0x170012F2 RID: 4850
		// (get) Token: 0x0600AB4D RID: 43853 RVA: 0x004EB3B8 File Offset: 0x004E95B8
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AB4E RID: 43854 RVA: 0x004EB3BB File Offset: 0x004E95BB
		protected override IEnumerable<DetailedFilterMenuLogic<IFilterableCombatSkill>> GenerateMenus()
		{
			yield return new CombatSkillTypeMenu();
			yield return new AttackCombatSkillTypeMenu();
			yield return new SectMenu();
			yield return new FiveElementsMenu();
			yield break;
		}

		// Token: 0x170012F3 RID: 4851
		// (get) Token: 0x0600AB4F RID: 43855 RVA: 0x004EB3CB File Offset: 0x004E95CB
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170012F4 RID: 4852
		// (get) Token: 0x0600AB50 RID: 43856 RVA: 0x004EB3CE File Offset: 0x004E95CE
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600AB51 RID: 43857 RVA: 0x004EB3D4 File Offset: 0x004E95D4
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
