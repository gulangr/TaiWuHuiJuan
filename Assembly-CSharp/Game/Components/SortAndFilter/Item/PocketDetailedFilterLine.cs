using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D7B RID: 3451
	public class PocketDetailedFilterLine : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x1700122F RID: 4655
		// (get) Token: 0x0600A89C RID: 43164 RVA: 0x004E1213 File Offset: 0x004DF413
		public override int Id
		{
			get
			{
				return 24;
			}
		}

		// Token: 0x17001230 RID: 4656
		// (get) Token: 0x0600A89D RID: 43165 RVA: 0x004E1217 File Offset: 0x004DF417
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A89E RID: 43166 RVA: 0x004E121A File Offset: 0x004DF41A
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new PocketSubTypeMenu();
			yield break;
		}

		// Token: 0x0600A89F RID: 43167 RVA: 0x004E122C File Offset: 0x004DF42C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(3, ToggleKey.CreateIndexKey(10))
			};
		}
	}
}
