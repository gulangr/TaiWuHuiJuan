using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D8D RID: 3469
	public class FoodSecondaryFilterLine : SecondaryFilterLineLogic<ITradeableContent>
	{
		// Token: 0x1700124F RID: 4687
		// (get) Token: 0x0600A8F6 RID: 43254 RVA: 0x004E23F0 File Offset: 0x004E05F0
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17001250 RID: 4688
		// (get) Token: 0x0600A8F7 RID: 43255 RVA: 0x004E23F3 File Offset: 0x004E05F3
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600A8F8 RID: 43256 RVA: 0x004E23F6 File Offset: 0x004E05F6
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new FoodTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x0600A8F9 RID: 43257 RVA: 0x004E2408 File Offset: 0x004E0608
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(0))
			};
		}
	}
}
