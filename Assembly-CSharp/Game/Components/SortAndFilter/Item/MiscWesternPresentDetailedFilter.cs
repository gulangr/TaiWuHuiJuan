using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DD6 RID: 3542
	public class MiscWesternPresentDetailedFilter : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x17001299 RID: 4761
		// (get) Token: 0x0600AA11 RID: 43537 RVA: 0x004E7392 File Offset: 0x004E5592
		public override int Id
		{
			get
			{
				return 41;
			}
		}

		// Token: 0x1700129A RID: 4762
		// (get) Token: 0x0600AA12 RID: 43538 RVA: 0x004E7396 File Offset: 0x004E5596
		protected override int Level
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x0600AA13 RID: 43539 RVA: 0x004E7399 File Offset: 0x004E5599
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new MiscWesternPresentTypeMenu();
			yield break;
		}

		// Token: 0x0600AA14 RID: 43540 RVA: 0x004E73AC File Offset: 0x004E55AC
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(7, ToggleKey.CreateIndexKey(MiscTypeSecondaryMenu.GetSecondaryFilterIndex(EMiscFilterKey.WesternPresent)))
			};
		}
	}
}
