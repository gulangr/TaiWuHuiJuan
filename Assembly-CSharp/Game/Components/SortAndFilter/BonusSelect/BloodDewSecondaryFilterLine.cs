using System;
using System.Collections.Generic;
using GameData.Domains.Taiwu.Display;

namespace Game.Components.SortAndFilter.BonusSelect
{
	// Token: 0x02000E89 RID: 3721
	public class BloodDewSecondaryFilterLine : SecondaryFilterLineLogic<SkillBreakBonusSelectableItem>
	{
		// Token: 0x1700137A RID: 4986
		// (get) Token: 0x0600ACF0 RID: 44272 RVA: 0x004EFDA8 File Offset: 0x004EDFA8
		public override int Id
		{
			get
			{
				return 7;
			}
		}

		// Token: 0x1700137B RID: 4987
		// (get) Token: 0x0600ACF1 RID: 44273 RVA: 0x004EFDAB File Offset: 0x004EDFAB
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600ACF2 RID: 44274 RVA: 0x004EFDAE File Offset: 0x004EDFAE
		protected override IEnumerable<DetailedFilterMenuLogic<SkillBreakBonusSelectableItem>> GenerateMenus()
		{
			yield return new BloodDewTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x0600ACF3 RID: 44275 RVA: 0x004EFDC0 File Offset: 0x004EDFC0
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(6))
			};
		}
	}
}
