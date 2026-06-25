using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D79 RID: 3449
	public class BeastCarrierDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x1700122B RID: 4651
		// (get) Token: 0x0600A890 RID: 43152 RVA: 0x004E1097 File Offset: 0x004DF297
		public override int Id
		{
			get
			{
				return 23;
			}
		}

		// Token: 0x1700122C RID: 4652
		// (get) Token: 0x0600A891 RID: 43153 RVA: 0x004E109B File Offset: 0x004DF29B
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A892 RID: 43154 RVA: 0x004E109E File Offset: 0x004DF29E
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new BeastCarrierSubTypeMenu();
			yield break;
		}

		// Token: 0x0600A893 RID: 43155 RVA: 0x004E10B0 File Offset: 0x004DF2B0
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(9))
			};
		}
	}
}
