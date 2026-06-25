using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200048E RID: 1166
	public class AllBookDetailedFilterLine : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x170006FB RID: 1787
		// (get) Token: 0x06004137 RID: 16695 RVA: 0x002010A1 File Offset: 0x001FF2A1
		public override int Id
		{
			get
			{
				return 22;
			}
		}

		// Token: 0x170006FC RID: 1788
		// (get) Token: 0x06004138 RID: 16696 RVA: 0x002010A5 File Offset: 0x001FF2A5
		protected override int Level
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06004139 RID: 16697 RVA: 0x002010A8 File Offset: 0x001FF2A8
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield return new BookReadStatusMenu();
			yield break;
		}

		// Token: 0x0600413A RID: 16698 RVA: 0x002010B8 File Offset: 0x001FF2B8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(4, new ToggleKey
				{
					IsAll = false,
					Index = -1
				})
			};
		}
	}
}
