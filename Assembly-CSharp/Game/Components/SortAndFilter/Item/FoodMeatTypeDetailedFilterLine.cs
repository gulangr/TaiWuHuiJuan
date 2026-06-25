using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D83 RID: 3459
	public class FoodMeatTypeDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x1700123E RID: 4670
		// (get) Token: 0x0600A8C8 RID: 43208 RVA: 0x004E1B45 File Offset: 0x004DFD45
		public override int Id
		{
			get
			{
				return 9;
			}
		}

		// Token: 0x1700123F RID: 4671
		// (get) Token: 0x0600A8C9 RID: 43209 RVA: 0x004E1B49 File Offset: 0x004DFD49
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A8CA RID: 43210 RVA: 0x004E1B4C File Offset: 0x004DFD4C
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new MeatFoodRecoverTypeMenu();
			yield return new MeatFoodBuffTypeMenu();
			yield break;
		}

		// Token: 0x0600A8CB RID: 43211 RVA: 0x004E1B5C File Offset: 0x004DFD5C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(1, ToggleKey.CreateIndexKey(1))
			};
		}
	}
}
