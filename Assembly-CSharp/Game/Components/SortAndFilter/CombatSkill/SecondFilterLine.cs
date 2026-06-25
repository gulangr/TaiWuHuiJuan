using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E39 RID: 3641
	public class SecondFilterLine : DetailedFilterLineLogic<IFilterableCombatSkill>
	{
		// Token: 0x1700131B RID: 4891
		// (get) Token: 0x0600ABBD RID: 43965 RVA: 0x004EC721 File Offset: 0x004EA921
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600ABBE RID: 43966 RVA: 0x004EC724 File Offset: 0x004EA924
		protected override IEnumerable<DetailedFilterMenuLogic<IFilterableCombatSkill>> GenerateMenus()
		{
			yield return new CombatSkillStatusMenu();
			yield break;
		}

		// Token: 0x1700131C RID: 4892
		// (get) Token: 0x0600ABBF RID: 43967 RVA: 0x004EC734 File Offset: 0x004EA934
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700131D RID: 4893
		// (get) Token: 0x0600ABC0 RID: 43968 RVA: 0x004EC737 File Offset: 0x004EA937
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600ABC1 RID: 43969 RVA: 0x004EC73C File Offset: 0x004EA93C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
