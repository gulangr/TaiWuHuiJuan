using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000532 RID: 1330
	public class MaterialMetalDetailedFilter : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x170007E1 RID: 2017
		// (get) Token: 0x06004373 RID: 17267 RVA: 0x0020730F File Offset: 0x0020550F
		public override int Id
		{
			get
			{
				return 28;
			}
		}

		// Token: 0x06004374 RID: 17268 RVA: 0x00207313 File Offset: 0x00205513
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new MetalMakeTypeMenu();
			yield return new MetalHardnessMenu();
			yield break;
		}

		// Token: 0x170007E2 RID: 2018
		// (get) Token: 0x06004375 RID: 17269 RVA: 0x00207323 File Offset: 0x00205523
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06004376 RID: 17270 RVA: 0x00207328 File Offset: 0x00205528
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(6, ToggleKey.CreateIndexKey(3))
			};
		}
	}
}
