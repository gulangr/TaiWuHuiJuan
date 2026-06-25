using System;
using System.Collections.Generic;
using GameData.Domains.Taiwu.Display;

namespace Game.Components.SortAndFilter.BonusSelect
{
	// Token: 0x02000E95 RID: 3733
	public class MedicineSecondaryFilterLine : SecondaryFilterLineLogic<SkillBreakBonusSelectableItem>
	{
		// Token: 0x1700138C RID: 5004
		// (get) Token: 0x0600AD33 RID: 44339 RVA: 0x004F0F61 File Offset: 0x004EF161
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x1700138D RID: 5005
		// (get) Token: 0x0600AD34 RID: 44340 RVA: 0x004F0F64 File Offset: 0x004EF164
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AD35 RID: 44341 RVA: 0x004F0F67 File Offset: 0x004EF167
		protected override IEnumerable<DetailedFilterMenuLogic<SkillBreakBonusSelectableItem>> GenerateMenus()
		{
			yield return new MedicineTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x0600AD36 RID: 44342 RVA: 0x004F0F78 File Offset: 0x004EF178
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(3))
			};
		}
	}
}
