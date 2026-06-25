using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000555 RID: 1365
	public class MiscSecondaryFilterLine : SecondaryFilterLineBase<ItemDisplayData>
	{
		// Token: 0x1700080E RID: 2062
		// (get) Token: 0x06004406 RID: 17414 RVA: 0x00208C4A File Offset: 0x00206E4A
		public override int Id
		{
			get
			{
				return 7;
			}
		}

		// Token: 0x1700080F RID: 2063
		// (get) Token: 0x06004407 RID: 17415 RVA: 0x00208C4D File Offset: 0x00206E4D
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06004408 RID: 17416 RVA: 0x00208C50 File Offset: 0x00206E50
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new MisclTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x06004409 RID: 17417 RVA: 0x00208C60 File Offset: 0x00206E60
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(6))
			};
		}
	}
}
