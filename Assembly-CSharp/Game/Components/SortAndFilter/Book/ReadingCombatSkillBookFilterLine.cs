using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter.Item;

namespace Game.Components.SortAndFilter.Book
{
	// Token: 0x02000E87 RID: 3719
	public class ReadingCombatSkillBookFilterLine : CombatSkillBookDetailedFilterLine
	{
		// Token: 0x17001378 RID: 4984
		// (get) Token: 0x0600ACEA RID: 44266 RVA: 0x004EFD67 File Offset: 0x004EDF67
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600ACEB RID: 44267 RVA: 0x004EFD6C File Offset: 0x004EDF6C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
