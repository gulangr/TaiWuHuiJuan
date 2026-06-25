using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DCD RID: 3533
	public class MiscMiscDetailedFilter : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x1700128D RID: 4749
		// (get) Token: 0x0600A9D9 RID: 43481 RVA: 0x004E6A2C File Offset: 0x004E4C2C
		public override int Id
		{
			get
			{
				return 42;
			}
		}

		// Token: 0x1700128E RID: 4750
		// (get) Token: 0x0600A9DA RID: 43482 RVA: 0x004E6A30 File Offset: 0x004E4C30
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600A9DB RID: 43483 RVA: 0x004E6A33 File Offset: 0x004E4C33
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new MiscMiscTypeMenu();
			yield break;
		}

		// Token: 0x0600A9DC RID: 43484 RVA: 0x004E6A44 File Offset: 0x004E4C44
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(7, ToggleKey.CreateIndexKey(MiscTypeSecondaryMenu.GetSecondaryFilterIndex(EMiscFilterKey.Misc)))
			};
		}
	}
}
