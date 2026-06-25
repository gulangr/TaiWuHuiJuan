using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DDE RID: 3550
	public class BookSecondaryFilterLine : SecondaryFilterLineLogic<ITradeableContent>
	{
		// Token: 0x170012A9 RID: 4777
		// (get) Token: 0x0600AA46 RID: 43590 RVA: 0x004E7D03 File Offset: 0x004E5F03
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x170012AA RID: 4778
		// (get) Token: 0x0600AA47 RID: 43591 RVA: 0x004E7D06 File Offset: 0x004E5F06
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AA48 RID: 43592 RVA: 0x004E7D09 File Offset: 0x004E5F09
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new BookTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x0600AA49 RID: 43593 RVA: 0x004E7D1C File Offset: 0x004E5F1C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(3))
			};
		}
	}
}
