using System;
using System.Collections.Generic;
using GameData.Domains.Taiwu.Display;

namespace CommonSortAndFilterLegacy.BonusSelect
{
	// Token: 0x020005B6 RID: 1462
	public class BookSecondaryFilterLine : SecondaryFilterLineBase<SkillBreakBonusSelectableItem>
	{
		// Token: 0x170008BF RID: 2239
		// (get) Token: 0x060045CF RID: 17871 RVA: 0x0020D414 File Offset: 0x0020B614
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170008C0 RID: 2240
		// (get) Token: 0x060045D0 RID: 17872 RVA: 0x0020D417 File Offset: 0x0020B617
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060045D1 RID: 17873 RVA: 0x0020D41A File Offset: 0x0020B61A
		protected override IEnumerable<DetailedFilterMenuBase<SkillBreakBonusSelectableItem>> GenerateMenus()
		{
			yield return new BookTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x060045D2 RID: 17874 RVA: 0x0020D42C File Offset: 0x0020B62C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(2))
			};
		}
	}
}
