using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter.Item;

namespace Game.Components.SortAndFilter.Book
{
	// Token: 0x02000E88 RID: 3720
	public class ReadingLifeSkillBookFilterLine : LifeSkillBookDetailedFilterLine
	{
		// Token: 0x17001379 RID: 4985
		// (get) Token: 0x0600ACED RID: 44269 RVA: 0x004EFD88 File Offset: 0x004EDF88
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600ACEE RID: 44270 RVA: 0x004EFD8C File Offset: 0x004EDF8C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
