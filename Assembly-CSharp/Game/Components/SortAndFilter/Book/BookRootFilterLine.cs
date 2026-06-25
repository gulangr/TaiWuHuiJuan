using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter.Item;

namespace Game.Components.SortAndFilter.Book
{
	// Token: 0x02000E7D RID: 3709
	public class BookRootFilterLine : BookSecondaryFilterLine
	{
		// Token: 0x17001376 RID: 4982
		// (get) Token: 0x0600ACCE RID: 44238 RVA: 0x004EF8F5 File Offset: 0x004EDAF5
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600ACCF RID: 44239 RVA: 0x004EF8F8 File Offset: 0x004EDAF8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
