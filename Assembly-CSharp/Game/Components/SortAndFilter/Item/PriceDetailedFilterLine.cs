using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DD8 RID: 3544
	public class PriceDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x1700129D RID: 4765
		// (get) Token: 0x0600AA25 RID: 43557 RVA: 0x004E7631 File Offset: 0x004E5831
		public override int Id
		{
			get
			{
				return 44;
			}
		}

		// Token: 0x1700129E RID: 4766
		// (get) Token: 0x0600AA26 RID: 43558 RVA: 0x004E7635 File Offset: 0x004E5835
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600AA27 RID: 43559 RVA: 0x004E7638 File Offset: 0x004E5838
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new PriceMenu();
			yield break;
		}

		// Token: 0x0600AA28 RID: 43560 RVA: 0x004E7648 File Offset: 0x004E5848
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
