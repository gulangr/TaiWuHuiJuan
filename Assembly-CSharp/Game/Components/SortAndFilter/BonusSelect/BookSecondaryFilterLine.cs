using System;
using System.Collections.Generic;
using GameData.Domains.Taiwu.Display;

namespace Game.Components.SortAndFilter.BonusSelect
{
	// Token: 0x02000E8D RID: 3725
	public class BookSecondaryFilterLine : SecondaryFilterLineLogic<SkillBreakBonusSelectableItem>
	{
		// Token: 0x1700137E RID: 4990
		// (get) Token: 0x0600AD09 RID: 44297 RVA: 0x004F08E9 File Offset: 0x004EEAE9
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x1700137F RID: 4991
		// (get) Token: 0x0600AD0A RID: 44298 RVA: 0x004F08EC File Offset: 0x004EEAEC
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AD0B RID: 44299 RVA: 0x004F08EF File Offset: 0x004EEAEF
		protected override IEnumerable<DetailedFilterMenuLogic<SkillBreakBonusSelectableItem>> GenerateMenus()
		{
			yield return new BookTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x0600AD0C RID: 44300 RVA: 0x004F0900 File Offset: 0x004EEB00
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(2))
			};
		}
	}
}
