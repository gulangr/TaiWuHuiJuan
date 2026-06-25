using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DC4 RID: 3524
	public class MedicinePoisonTypeDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x17001281 RID: 4737
		// (get) Token: 0x0600A9B3 RID: 43443 RVA: 0x004E6139 File Offset: 0x004E4339
		public override int Id
		{
			get
			{
				return 12;
			}
		}

		// Token: 0x17001282 RID: 4738
		// (get) Token: 0x0600A9B4 RID: 43444 RVA: 0x004E613D File Offset: 0x004E433D
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A9B5 RID: 43445 RVA: 0x004E6140 File Offset: 0x004E4340
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new PoisonTypeMenu();
			yield break;
		}

		// Token: 0x0600A9B6 RID: 43446 RVA: 0x004E6150 File Offset: 0x004E4350
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(2, ToggleKey.CreateIndexKey(3)),
				new ToggleIdIndex(2, ToggleKey.CreateIndexKey(2))
			};
		}
	}
}
