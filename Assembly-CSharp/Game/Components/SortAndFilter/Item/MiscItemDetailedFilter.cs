using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DCA RID: 3530
	public class MiscItemDetailedFilter : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x17001289 RID: 4745
		// (get) Token: 0x0600A9C9 RID: 43465 RVA: 0x004E673D File Offset: 0x004E493D
		public override int Id
		{
			get
			{
				return 36;
			}
		}

		// Token: 0x1700128A RID: 4746
		// (get) Token: 0x0600A9CA RID: 43466 RVA: 0x004E6741 File Offset: 0x004E4941
		protected override int Level
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x0600A9CB RID: 43467 RVA: 0x004E6744 File Offset: 0x004E4944
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new MiscItemTypeMenu();
			yield break;
		}

		// Token: 0x0600A9CC RID: 43468 RVA: 0x004E6754 File Offset: 0x004E4954
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(7, ToggleKey.CreateIndexKey(MiscTypeSecondaryMenu.GetSecondaryFilterIndex(EMiscFilterKey.Item)))
			};
		}
	}
}
