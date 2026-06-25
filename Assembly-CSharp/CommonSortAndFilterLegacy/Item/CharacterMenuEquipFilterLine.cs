using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004CC RID: 1228
	public class CharacterMenuEquipFilterLine : EquipFilterLine
	{
		// Token: 0x1700075C RID: 1884
		// (get) Token: 0x06004223 RID: 16931 RVA: 0x00203501 File Offset: 0x00201701
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06004224 RID: 16932 RVA: 0x00203504 File Offset: 0x00201704
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
