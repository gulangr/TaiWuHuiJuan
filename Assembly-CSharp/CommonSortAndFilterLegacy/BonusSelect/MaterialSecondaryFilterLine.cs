using System;
using System.Collections.Generic;
using GameData.Domains.Taiwu.Display;

namespace CommonSortAndFilterLegacy.BonusSelect
{
	// Token: 0x020005BC RID: 1468
	public class MaterialSecondaryFilterLine : SecondaryFilterLineBase<SkillBreakBonusSelectableItem>
	{
		// Token: 0x170008C9 RID: 2249
		// (get) Token: 0x060045ED RID: 17901 RVA: 0x0020D89F File Offset: 0x0020BA9F
		public override int Id
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x170008CA RID: 2250
		// (get) Token: 0x060045EE RID: 17902 RVA: 0x0020D8A2 File Offset: 0x0020BAA2
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060045EF RID: 17903 RVA: 0x0020D8A5 File Offset: 0x0020BAA5
		protected override IEnumerable<DetailedFilterMenuBase<SkillBreakBonusSelectableItem>> GenerateMenus()
		{
			yield return new MaterialTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x060045F0 RID: 17904 RVA: 0x0020D8B8 File Offset: 0x0020BAB8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(4))
			};
		}
	}
}
