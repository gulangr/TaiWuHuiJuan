using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DB4 RID: 3508
	public class MaterialPoisonDetailedFilter : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x1700126E RID: 4718
		// (get) Token: 0x0600A979 RID: 43385 RVA: 0x004E5BA1 File Offset: 0x004E3DA1
		public override int Id
		{
			get
			{
				return 33;
			}
		}

		// Token: 0x1700126F RID: 4719
		// (get) Token: 0x0600A97A RID: 43386 RVA: 0x004E5BA5 File Offset: 0x004E3DA5
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A97B RID: 43387 RVA: 0x004E5BA8 File Offset: 0x004E3DA8
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new MaterialPoisonTypeMenu();
			yield return new MaterialPoisonPropertyMenu();
			yield break;
		}

		// Token: 0x0600A97C RID: 43388 RVA: 0x004E5BB8 File Offset: 0x004E3DB8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(6, ToggleKey.CreateIndexKey(5))
			};
		}
	}
}
