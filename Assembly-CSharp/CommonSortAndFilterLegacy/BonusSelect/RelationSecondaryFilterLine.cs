using System;
using System.Collections.Generic;
using GameData.Domains.Taiwu.Display;

namespace CommonSortAndFilterLegacy.BonusSelect
{
	// Token: 0x020005C0 RID: 1472
	public class RelationSecondaryFilterLine : SecondaryFilterLineBase<SkillBreakBonusSelectableItem>
	{
		// Token: 0x170008D1 RID: 2257
		// (get) Token: 0x06004605 RID: 17925 RVA: 0x0020DCD0 File Offset: 0x0020BED0
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170008D2 RID: 2258
		// (get) Token: 0x06004606 RID: 17926 RVA: 0x0020DCD3 File Offset: 0x0020BED3
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170008D3 RID: 2259
		// (get) Token: 0x06004607 RID: 17927 RVA: 0x0020DCD6 File Offset: 0x0020BED6
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004608 RID: 17928 RVA: 0x0020DCD9 File Offset: 0x0020BED9
		protected override IEnumerable<DetailedFilterMenuBase<SkillBreakBonusSelectableItem>> GenerateMenus()
		{
			yield return new RelationTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x06004609 RID: 17929 RVA: 0x0020DCEC File Offset: 0x0020BEEC
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(1))
			};
		}
	}
}
