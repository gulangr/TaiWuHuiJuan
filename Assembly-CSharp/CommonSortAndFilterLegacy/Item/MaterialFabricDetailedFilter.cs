using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200051B RID: 1307
	public class MaterialFabricDetailedFilter : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x170007C6 RID: 1990
		// (get) Token: 0x0600432C RID: 17196 RVA: 0x00206371 File Offset: 0x00204571
		public override int Id
		{
			get
			{
				return 25;
			}
		}

		// Token: 0x0600432D RID: 17197 RVA: 0x00206375 File Offset: 0x00204575
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new FabricMakeTypeMenu();
			yield return new FabricHardnessMenu();
			yield break;
		}

		// Token: 0x170007C7 RID: 1991
		// (get) Token: 0x0600432E RID: 17198 RVA: 0x00206385 File Offset: 0x00204585
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600432F RID: 17199 RVA: 0x00206388 File Offset: 0x00204588
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(6, ToggleKey.CreateIndexKey(0))
			};
		}
	}
}
