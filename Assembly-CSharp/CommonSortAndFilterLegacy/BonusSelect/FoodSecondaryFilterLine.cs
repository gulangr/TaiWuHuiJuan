using System;
using System.Collections.Generic;
using GameData.Domains.Taiwu.Display;

namespace CommonSortAndFilterLegacy.BonusSelect
{
	// Token: 0x020005B9 RID: 1465
	public class FoodSecondaryFilterLine : SecondaryFilterLineBase<SkillBreakBonusSelectableItem>
	{
		// Token: 0x170008C3 RID: 2243
		// (get) Token: 0x060045DB RID: 17883 RVA: 0x0020D5C2 File Offset: 0x0020B7C2
		public override int Id
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x170008C4 RID: 2244
		// (get) Token: 0x060045DC RID: 17884 RVA: 0x0020D5C5 File Offset: 0x0020B7C5
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060045DD RID: 17885 RVA: 0x0020D5C8 File Offset: 0x0020B7C8
		protected override IEnumerable<DetailedFilterMenuBase<SkillBreakBonusSelectableItem>> GenerateMenus()
		{
			yield return new FoodTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x060045DE RID: 17886 RVA: 0x0020D5D8 File Offset: 0x0020B7D8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(5))
			};
		}
	}
}
