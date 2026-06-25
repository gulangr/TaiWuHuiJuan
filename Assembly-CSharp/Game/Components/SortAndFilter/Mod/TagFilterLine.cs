using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Mod
{
	// Token: 0x02000D19 RID: 3353
	public class TagFilterLine : DetailedFilterLineLogic<ModSortAndFilterData>
	{
		// Token: 0x17001193 RID: 4499
		// (get) Token: 0x0600A73C RID: 42812 RVA: 0x004DD6C5 File Offset: 0x004DB8C5
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17001194 RID: 4500
		// (get) Token: 0x0600A73D RID: 42813 RVA: 0x004DD6C8 File Offset: 0x004DB8C8
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17001195 RID: 4501
		// (get) Token: 0x0600A73E RID: 42814 RVA: 0x004DD6CB File Offset: 0x004DB8CB
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600A73F RID: 42815 RVA: 0x004DD6D0 File Offset: 0x004DB8D0
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x0600A740 RID: 42816 RVA: 0x004DD6E3 File Offset: 0x004DB8E3
		protected override IEnumerable<DetailedFilterMenuLogic<ModSortAndFilterData>> GenerateMenus()
		{
			yield return new TagFilterMenu();
			yield break;
		}
	}
}
