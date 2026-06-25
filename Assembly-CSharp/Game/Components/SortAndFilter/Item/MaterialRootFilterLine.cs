using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D9F RID: 3487
	public class MaterialRootFilterLine : MaterialSecondaryFilterLine
	{
		// Token: 0x17001258 RID: 4696
		// (get) Token: 0x0600A937 RID: 43319 RVA: 0x004E4DC8 File Offset: 0x004E2FC8
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A938 RID: 43320 RVA: 0x004E4DCC File Offset: 0x004E2FCC
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
