using System;
using System.Collections.Generic;
using GameData.Domains.Taiwu.Display;

namespace Game.Components.SortAndFilter.BonusSelect
{
	// Token: 0x02000E90 RID: 3728
	public class FoodSecondaryFilterLine : SecondaryFilterLineLogic<SkillBreakBonusSelectableItem>
	{
		// Token: 0x17001382 RID: 4994
		// (get) Token: 0x0600AD15 RID: 44309 RVA: 0x004F0A82 File Offset: 0x004EEC82
		public override int Id
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x17001383 RID: 4995
		// (get) Token: 0x0600AD16 RID: 44310 RVA: 0x004F0A85 File Offset: 0x004EEC85
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AD17 RID: 44311 RVA: 0x004F0A88 File Offset: 0x004EEC88
		protected override IEnumerable<DetailedFilterMenuLogic<SkillBreakBonusSelectableItem>> GenerateMenus()
		{
			yield return new FoodTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x0600AD18 RID: 44312 RVA: 0x004F0A98 File Offset: 0x004EEC98
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(5))
			};
		}
	}
}
