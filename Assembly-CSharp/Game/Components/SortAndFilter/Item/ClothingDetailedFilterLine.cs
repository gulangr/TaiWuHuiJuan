using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D72 RID: 3442
	public class ClothingDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x1700121F RID: 4639
		// (get) Token: 0x0600A86D RID: 43117 RVA: 0x004E0D14 File Offset: 0x004DEF14
		public override int Id
		{
			get
			{
				return 20;
			}
		}

		// Token: 0x17001220 RID: 4640
		// (get) Token: 0x0600A86E RID: 43118 RVA: 0x004E0D18 File Offset: 0x004DEF18
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A86F RID: 43119 RVA: 0x004E0D1B File Offset: 0x004DEF1B
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new ClothingWeaveTypeMenu();
			yield break;
		}

		// Token: 0x0600A870 RID: 43120 RVA: 0x004E0D2C File Offset: 0x004DEF2C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(6))
			};
		}
	}
}
