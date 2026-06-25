using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D28 RID: 3368
	public class AllBookDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x170011A5 RID: 4517
		// (get) Token: 0x0600A781 RID: 42881 RVA: 0x004DF2F8 File Offset: 0x004DD4F8
		public override int Id
		{
			get
			{
				return 25;
			}
		}

		// Token: 0x170011A6 RID: 4518
		// (get) Token: 0x0600A782 RID: 42882 RVA: 0x004DF2FC File Offset: 0x004DD4FC
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A783 RID: 42883 RVA: 0x004DF2FF File Offset: 0x004DD4FF
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new BookReadStatusMenu();
			yield break;
		}

		// Token: 0x0600A784 RID: 42884 RVA: 0x004DF310 File Offset: 0x004DD510
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(4, new ToggleKey
				{
					IsAll = false,
					Index = -1
				})
			};
		}
	}
}
