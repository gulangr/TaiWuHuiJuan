using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DC6 RID: 3526
	public class MedicineBuffTypeDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x17001285 RID: 4741
		// (get) Token: 0x0600A9BE RID: 43454 RVA: 0x004E634D File Offset: 0x004E454D
		public override int Id
		{
			get
			{
				return 13;
			}
		}

		// Token: 0x17001286 RID: 4742
		// (get) Token: 0x0600A9BF RID: 43455 RVA: 0x004E6351 File Offset: 0x004E4551
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A9C0 RID: 43456 RVA: 0x004E6354 File Offset: 0x004E4554
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new MedicineBuffTypeMenu();
			yield break;
		}

		// Token: 0x0600A9C1 RID: 43457 RVA: 0x004E6364 File Offset: 0x004E4564
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(2, ToggleKey.CreateIndexKey(6))
			};
		}
	}
}
