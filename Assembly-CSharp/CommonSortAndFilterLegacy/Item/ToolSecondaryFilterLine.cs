using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000560 RID: 1376
	public class ToolSecondaryFilterLine : SecondaryFilterLineBase<ItemDisplayData>
	{
		// Token: 0x17000820 RID: 2080
		// (get) Token: 0x06004441 RID: 17473 RVA: 0x002094C5 File Offset: 0x002076C5
		public override int Id
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x17000821 RID: 2081
		// (get) Token: 0x06004442 RID: 17474 RVA: 0x002094C8 File Offset: 0x002076C8
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06004443 RID: 17475 RVA: 0x002094CB File Offset: 0x002076CB
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new ToolTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x06004444 RID: 17476 RVA: 0x002094DC File Offset: 0x002076DC
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(4))
			};
		}
	}
}
