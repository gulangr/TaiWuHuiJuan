using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D86 RID: 3462
	public class FoodTeaTypeDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x17001244 RID: 4676
		// (get) Token: 0x0600A8D9 RID: 43225 RVA: 0x004E1F91 File Offset: 0x004E0191
		public override int Id
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x17001245 RID: 4677
		// (get) Token: 0x0600A8DA RID: 43226 RVA: 0x004E1F95 File Offset: 0x004E0195
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A8DB RID: 43227 RVA: 0x004E1F98 File Offset: 0x004E0198
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new TeaFoodTypeMenu();
			yield break;
		}

		// Token: 0x0600A8DC RID: 43228 RVA: 0x004E1FA8 File Offset: 0x004E01A8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(1, ToggleKey.CreateIndexKey(2))
			};
		}
	}
}
