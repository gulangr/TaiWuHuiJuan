using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace Game.Components.SortAndFilter.CombatSkill
{
	// Token: 0x02000E3C RID: 3644
	public class SingleFilterLine : DetailedFilterLineLogic<IFilterableCombatSkill>
	{
		// Token: 0x17001320 RID: 4896
		// (get) Token: 0x0600ABCF RID: 43983 RVA: 0x004EC9FD File Offset: 0x004EABFD
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600ABD0 RID: 43984 RVA: 0x004ECA00 File Offset: 0x004EAC00
		public SingleFilterLine(EFilterType filterType)
		{
			this.FilterType = filterType;
		}

		// Token: 0x0600ABD1 RID: 43985 RVA: 0x004ECA11 File Offset: 0x004EAC11
		protected override IEnumerable<DetailedFilterMenuLogic<IFilterableCombatSkill>> GenerateMenus()
		{
			bool flag = this.FilterType != EFilterType.Looping && this.FilterType != EFilterType.Reference;
			if (flag)
			{
				yield return new SingleFilterCombatSkillSlotTypeMenu();
			}
			yield return new SingleFilterCombatSkillTypeMenu();
			yield return new SingleFilterSectMenu();
			yield return new SingleFilterFiveElementsMenu();
			yield return new SingleFilterCombatSkillStatusMenu();
			yield break;
		}

		// Token: 0x17001321 RID: 4897
		// (get) Token: 0x0600ABD2 RID: 43986 RVA: 0x004ECA21 File Offset: 0x004EAC21
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17001322 RID: 4898
		// (get) Token: 0x0600ABD3 RID: 43987 RVA: 0x004ECA24 File Offset: 0x004EAC24
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600ABD4 RID: 43988 RVA: 0x004ECA28 File Offset: 0x004EAC28
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x0400852A RID: 34090
		public EFilterType FilterType;
	}
}
