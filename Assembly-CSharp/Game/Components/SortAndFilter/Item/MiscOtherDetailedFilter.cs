using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DD0 RID: 3536
	public class MiscOtherDetailedFilter : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x17001291 RID: 4753
		// (get) Token: 0x0600A9E4 RID: 43492 RVA: 0x004E6B63 File Offset: 0x004E4D63
		public override int Id
		{
			get
			{
				return 43;
			}
		}

		// Token: 0x17001292 RID: 4754
		// (get) Token: 0x0600A9E5 RID: 43493 RVA: 0x004E6B67 File Offset: 0x004E4D67
		protected override int Level
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x0600A9E6 RID: 43494 RVA: 0x004E6B6A File Offset: 0x004E4D6A
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new MiscOtherTypeMenu();
			yield break;
		}

		// Token: 0x0600A9E7 RID: 43495 RVA: 0x004E6B7C File Offset: 0x004E4D7C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(7, ToggleKey.CreateIndexKey(MiscTypeSecondaryMenu.GetSecondaryFilterIndex(EMiscFilterKey.Other)))
			};
		}
	}
}
