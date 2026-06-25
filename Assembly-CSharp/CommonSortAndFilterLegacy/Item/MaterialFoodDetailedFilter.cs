using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000522 RID: 1314
	internal class MaterialFoodDetailedFilter : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x170007D0 RID: 2000
		// (get) Token: 0x06004347 RID: 17223 RVA: 0x0020671B File Offset: 0x0020491B
		public override int Id
		{
			get
			{
				return 31;
			}
		}

		// Token: 0x06004348 RID: 17224 RVA: 0x0020671F File Offset: 0x0020491F
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new MaterialFoodTypeMenu();
			yield break;
		}

		// Token: 0x170007D1 RID: 2001
		// (get) Token: 0x06004349 RID: 17225 RVA: 0x0020672F File Offset: 0x0020492F
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600434A RID: 17226 RVA: 0x00206734 File Offset: 0x00204934
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(6, ToggleKey.CreateIndexKey(6))
			};
		}
	}
}
