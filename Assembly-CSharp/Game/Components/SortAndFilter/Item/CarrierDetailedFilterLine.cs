using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D75 RID: 3445
	public class CarrierDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x17001223 RID: 4643
		// (get) Token: 0x0600A878 RID: 43128 RVA: 0x004E0E69 File Offset: 0x004DF069
		public override int Id
		{
			get
			{
				return 21;
			}
		}

		// Token: 0x17001224 RID: 4644
		// (get) Token: 0x0600A879 RID: 43129 RVA: 0x004E0E6D File Offset: 0x004DF06D
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A87A RID: 43130 RVA: 0x004E0E70 File Offset: 0x004DF070
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new CarrierSubTypeMenu();
			yield break;
		}

		// Token: 0x0600A87B RID: 43131 RVA: 0x004E0E80 File Offset: 0x004DF080
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(7))
			};
		}
	}
}
