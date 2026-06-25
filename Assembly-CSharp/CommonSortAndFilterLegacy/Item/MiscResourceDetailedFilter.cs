using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000553 RID: 1363
	public class MiscResourceDetailedFilter : DetailedFilterLine<ItemDisplayData>
	{
		// Token: 0x17000809 RID: 2057
		// (get) Token: 0x060043F9 RID: 17401 RVA: 0x00208A76 File Offset: 0x00206C76
		public override int Id
		{
			get
			{
				return 32;
			}
		}

		// Token: 0x060043FA RID: 17402 RVA: 0x00208A7A File Offset: 0x00206C7A
		protected override IEnumerable<DetailedFilterMenuBase<ItemDisplayData>> GenerateMenus()
		{
			yield break;
		}

		// Token: 0x1700080A RID: 2058
		// (get) Token: 0x060043FB RID: 17403 RVA: 0x00208A8A File Offset: 0x00206C8A
		protected override int Level
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x1700080B RID: 2059
		// (get) Token: 0x060043FC RID: 17404 RVA: 0x00208A8D File Offset: 0x00206C8D
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060043FD RID: 17405 RVA: 0x00208A90 File Offset: 0x00206C90
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(7, ToggleKey.CreateIndexKey(0))
			};
		}
	}
}
