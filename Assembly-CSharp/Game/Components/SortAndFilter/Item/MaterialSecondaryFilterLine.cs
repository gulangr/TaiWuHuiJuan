using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DE2 RID: 3554
	public class MaterialSecondaryFilterLine : SecondaryFilterLineLogic<ITradeableContent>
	{
		// Token: 0x170012B1 RID: 4785
		// (get) Token: 0x0600AA5C RID: 43612 RVA: 0x004E8048 File Offset: 0x004E6248
		public override int Id
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x170012B2 RID: 4786
		// (get) Token: 0x0600AA5D RID: 43613 RVA: 0x004E804B File Offset: 0x004E624B
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AA5E RID: 43614 RVA: 0x004E804E File Offset: 0x004E624E
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new MaterialTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x0600AA5F RID: 43615 RVA: 0x004E8060 File Offset: 0x004E6260
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(5))
			};
		}
	}
}
