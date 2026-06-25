using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200055D RID: 1373
	public class SelectToolFilterLine : ToolSecondaryFilterLine
	{
		// Token: 0x1700081B RID: 2075
		// (get) Token: 0x06004434 RID: 17460 RVA: 0x002092B9 File Offset: 0x002074B9
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700081C RID: 2076
		// (get) Token: 0x06004435 RID: 17461 RVA: 0x002092BC File Offset: 0x002074BC
		public override int Id
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x06004436 RID: 17462 RVA: 0x002092C0 File Offset: 0x002074C0
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
