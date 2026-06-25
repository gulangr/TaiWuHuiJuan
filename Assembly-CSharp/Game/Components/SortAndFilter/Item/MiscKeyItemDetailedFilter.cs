using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DD3 RID: 3539
	public class MiscKeyItemDetailedFilter : DetailedFilterLineLogic<ITradeableContent>
	{
		// Token: 0x17001295 RID: 4757
		// (get) Token: 0x0600A9F2 RID: 43506 RVA: 0x004E6EA8 File Offset: 0x004E50A8
		public override int Id
		{
			get
			{
				return 38;
			}
		}

		// Token: 0x17001296 RID: 4758
		// (get) Token: 0x0600A9F3 RID: 43507 RVA: 0x004E6EAC File Offset: 0x004E50AC
		protected override int Level
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x0600A9F4 RID: 43508 RVA: 0x004E6EAF File Offset: 0x004E50AF
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new MiscKeyItemTypeMenu();
			yield break;
		}

		// Token: 0x0600A9F5 RID: 43509 RVA: 0x004E6EC0 File Offset: 0x004E50C0
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(7, ToggleKey.CreateIndexKey(MiscTypeSecondaryMenu.GetSecondaryFilterIndex(EMiscFilterKey.KeyItem)))
			};
		}
	}
}
