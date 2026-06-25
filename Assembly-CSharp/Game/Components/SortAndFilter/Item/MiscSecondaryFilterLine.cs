using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DE7 RID: 3559
	public class MiscSecondaryFilterLine : SecondaryFilterLineLogic<ITradeableContent>
	{
		// Token: 0x170012B9 RID: 4793
		// (get) Token: 0x0600AA72 RID: 43634 RVA: 0x004E8381 File Offset: 0x004E6581
		public override int Id
		{
			get
			{
				return 7;
			}
		}

		// Token: 0x170012BA RID: 4794
		// (get) Token: 0x0600AA73 RID: 43635 RVA: 0x004E8384 File Offset: 0x004E6584
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AA74 RID: 43636 RVA: 0x004E8387 File Offset: 0x004E6587
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new MiscTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x0600AA75 RID: 43637 RVA: 0x004E8398 File Offset: 0x004E6598
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(6))
			};
		}
	}
}
