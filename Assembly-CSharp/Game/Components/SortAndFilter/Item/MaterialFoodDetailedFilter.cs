using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DB9 RID: 3513
	public class MaterialFoodDetailedFilter : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x17001274 RID: 4724
		// (get) Token: 0x0600A989 RID: 43401 RVA: 0x004E5D11 File Offset: 0x004E3F11
		public override int Id
		{
			get
			{
				return 34;
			}
		}

		// Token: 0x17001275 RID: 4725
		// (get) Token: 0x0600A98A RID: 43402 RVA: 0x004E5D15 File Offset: 0x004E3F15
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A98B RID: 43403 RVA: 0x004E5D18 File Offset: 0x004E3F18
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new MaterialFoodTypeMenu();
			yield break;
		}

		// Token: 0x0600A98C RID: 43404 RVA: 0x004E5D28 File Offset: 0x004E3F28
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(6, ToggleKey.CreateIndexKey(6))
			};
		}
	}
}
