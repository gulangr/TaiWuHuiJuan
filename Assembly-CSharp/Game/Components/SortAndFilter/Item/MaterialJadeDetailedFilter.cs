using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DA4 RID: 3492
	public class MaterialJadeDetailedFilter : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x1700125C RID: 4700
		// (get) Token: 0x0600A94B RID: 43339 RVA: 0x004E50DF File Offset: 0x004E32DF
		public override int Id
		{
			get
			{
				return 30;
			}
		}

		// Token: 0x1700125D RID: 4701
		// (get) Token: 0x0600A94C RID: 43340 RVA: 0x004E50E3 File Offset: 0x004E32E3
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A94D RID: 43341 RVA: 0x004E50E6 File Offset: 0x004E32E6
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new JadeHardnessMenu();
			yield break;
		}

		// Token: 0x0600A94E RID: 43342 RVA: 0x004E50F8 File Offset: 0x004E32F8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(6, ToggleKey.CreateIndexKey(2))
			};
		}
	}
}
