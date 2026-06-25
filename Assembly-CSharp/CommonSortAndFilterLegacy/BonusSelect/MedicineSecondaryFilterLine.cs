using System;
using System.Collections.Generic;
using GameData.Domains.Taiwu.Display;

namespace CommonSortAndFilterLegacy.BonusSelect
{
	// Token: 0x020005BE RID: 1470
	public class MedicineSecondaryFilterLine : SecondaryFilterLineBase<SkillBreakBonusSelectableItem>
	{
		// Token: 0x170008CD RID: 2253
		// (get) Token: 0x060045F9 RID: 17913 RVA: 0x0020DA4D File Offset: 0x0020BC4D
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x170008CE RID: 2254
		// (get) Token: 0x060045FA RID: 17914 RVA: 0x0020DA50 File Offset: 0x0020BC50
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060045FB RID: 17915 RVA: 0x0020DA53 File Offset: 0x0020BC53
		protected override IEnumerable<DetailedFilterMenuBase<SkillBreakBonusSelectableItem>> GenerateMenus()
		{
			yield return new MedicineTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x060045FC RID: 17916 RVA: 0x0020DA64 File Offset: 0x0020BC64
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(3))
			};
		}
	}
}
