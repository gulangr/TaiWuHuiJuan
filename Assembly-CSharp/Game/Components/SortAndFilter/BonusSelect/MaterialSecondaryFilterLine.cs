using System;
using System.Collections.Generic;
using GameData.Domains.Taiwu.Display;

namespace Game.Components.SortAndFilter.BonusSelect
{
	// Token: 0x02000E93 RID: 3731
	public class MaterialSecondaryFilterLine : SecondaryFilterLineLogic<SkillBreakBonusSelectableItem>
	{
		// Token: 0x17001388 RID: 5000
		// (get) Token: 0x0600AD27 RID: 44327 RVA: 0x004F0DC7 File Offset: 0x004EEFC7
		public override int Id
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x17001389 RID: 5001
		// (get) Token: 0x0600AD28 RID: 44328 RVA: 0x004F0DCA File Offset: 0x004EEFCA
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AD29 RID: 44329 RVA: 0x004F0DCD File Offset: 0x004EEFCD
		protected override IEnumerable<DetailedFilterMenuLogic<SkillBreakBonusSelectableItem>> GenerateMenus()
		{
			yield return new MaterialTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x0600AD2A RID: 44330 RVA: 0x004F0DE0 File Offset: 0x004EEFE0
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(4))
			};
		}
	}
}
