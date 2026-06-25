using System;
using System.Collections.Generic;
using GameData.Domains.Taiwu.Display;

namespace CommonSortAndFilterLegacy.BonusSelect
{
	// Token: 0x020005B2 RID: 1458
	public class BloodDewSecondaryFilterLine : SecondaryFilterLineBase<SkillBreakBonusSelectableItem>
	{
		// Token: 0x170008BA RID: 2234
		// (get) Token: 0x060045B9 RID: 17849 RVA: 0x0020CF23 File Offset: 0x0020B123
		public override int Id
		{
			get
			{
				return 7;
			}
		}

		// Token: 0x170008BB RID: 2235
		// (get) Token: 0x060045BA RID: 17850 RVA: 0x0020CF26 File Offset: 0x0020B126
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060045BB RID: 17851 RVA: 0x0020CF29 File Offset: 0x0020B129
		protected override IEnumerable<DetailedFilterMenuBase<SkillBreakBonusSelectableItem>> GenerateMenus()
		{
			yield return new BloodDewTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x060045BC RID: 17852 RVA: 0x0020CF3C File Offset: 0x0020B13C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(6))
			};
		}
	}
}
