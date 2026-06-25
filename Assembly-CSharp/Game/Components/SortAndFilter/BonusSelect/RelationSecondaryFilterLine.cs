using System;
using System.Collections.Generic;
using GameData.Domains.Taiwu.Display;

namespace Game.Components.SortAndFilter.BonusSelect
{
	// Token: 0x02000E97 RID: 3735
	public class RelationSecondaryFilterLine : SecondaryFilterLineLogic<SkillBreakBonusSelectableItem>
	{
		// Token: 0x17001390 RID: 5008
		// (get) Token: 0x0600AD3F RID: 44351 RVA: 0x004F11D0 File Offset: 0x004EF3D0
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x17001391 RID: 5009
		// (get) Token: 0x0600AD40 RID: 44352 RVA: 0x004F11D3 File Offset: 0x004EF3D3
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17001392 RID: 5010
		// (get) Token: 0x0600AD41 RID: 44353 RVA: 0x004F11D6 File Offset: 0x004EF3D6
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600AD42 RID: 44354 RVA: 0x004F11D9 File Offset: 0x004EF3D9
		protected override IEnumerable<DetailedFilterMenuLogic<SkillBreakBonusSelectableItem>> GenerateMenus()
		{
			yield return new RelationTypeSecondaryMenu();
			yield break;
		}

		// Token: 0x0600AD43 RID: 44355 RVA: 0x004F11EC File Offset: 0x004EF3EC
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(1))
			};
		}
	}
}
