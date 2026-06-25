using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004F9 RID: 1273
	public class FoodMeatTypeDetailedFilterLine : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x170007AA RID: 1962
		// (get) Token: 0x060042C7 RID: 17095 RVA: 0x00204ED7 File Offset: 0x002030D7
		public override int Id
		{
			get
			{
				return 9;
			}
		}

		// Token: 0x170007AB RID: 1963
		// (get) Token: 0x060042C8 RID: 17096 RVA: 0x00204EDB File Offset: 0x002030DB
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x060042C9 RID: 17097 RVA: 0x00204EDE File Offset: 0x002030DE
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new MeatFoodRecoverTypeMenu();
			yield return new MeatFoodBuffTypeMenu();
			yield break;
		}

		// Token: 0x060042CA RID: 17098 RVA: 0x00204EF0 File Offset: 0x002030F0
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(1, ToggleKey.CreateIndexKey(1))
			};
		}
	}
}
