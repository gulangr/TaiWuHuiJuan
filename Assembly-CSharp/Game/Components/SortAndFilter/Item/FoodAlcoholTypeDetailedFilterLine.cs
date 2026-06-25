using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D87 RID: 3463
	public class FoodAlcoholTypeDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x17001246 RID: 4678
		// (get) Token: 0x0600A8DE RID: 43230 RVA: 0x004E1FDB File Offset: 0x004E01DB
		public override int Id
		{
			get
			{
				return 11;
			}
		}

		// Token: 0x17001247 RID: 4679
		// (get) Token: 0x0600A8DF RID: 43231 RVA: 0x004E1FDF File Offset: 0x004E01DF
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A8E0 RID: 43232 RVA: 0x004E1FE2 File Offset: 0x004E01E2
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new AlcoholFoodTypeMenu();
			yield break;
		}

		// Token: 0x0600A8E1 RID: 43233 RVA: 0x004E1FF4 File Offset: 0x004E01F4
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(1, ToggleKey.CreateIndexKey(3))
			};
		}
	}
}
