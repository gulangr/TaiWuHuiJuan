using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000506 RID: 1286
	public class FoodSecondaryFilterLine : SecondaryFilterLineBase<ItemDisplayData>
	{
		// Token: 0x170007BA RID: 1978
		// (get) Token: 0x060042F4 RID: 17140 RVA: 0x002059F1 File Offset: 0x00203BF1
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170007BB RID: 1979
		// (get) Token: 0x060042F5 RID: 17141 RVA: 0x002059F4 File Offset: 0x00203BF4
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060042F6 RID: 17142 RVA: 0x002059F7 File Offset: 0x00203BF7
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new FoodTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x060042F7 RID: 17143 RVA: 0x00205A08 File Offset: 0x00203C08
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(0))
			};
		}
	}
}
