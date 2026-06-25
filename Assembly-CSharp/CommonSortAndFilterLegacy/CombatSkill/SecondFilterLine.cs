using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace CommonSortAndFilterLegacy.CombatSkill
{
	// Token: 0x02000578 RID: 1400
	public class SecondFilterLine : DetailedFilterLine<CombatSkillDisplayData>
	{
		// Token: 0x1700083E RID: 2110
		// (get) Token: 0x06004498 RID: 17560 RVA: 0x0020A320 File Offset: 0x00208520
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06004499 RID: 17561 RVA: 0x0020A323 File Offset: 0x00208523
		protected override IEnumerable<DetailedFilterMenuBase<CombatSkillDisplayData>> GenerateMenus()
		{
			yield return new CombatSkillStatusMenu();
			yield break;
		}

		// Token: 0x1700083F RID: 2111
		// (get) Token: 0x0600449A RID: 17562 RVA: 0x0020A333 File Offset: 0x00208533
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000840 RID: 2112
		// (get) Token: 0x0600449B RID: 17563 RVA: 0x0020A336 File Offset: 0x00208536
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600449C RID: 17564 RVA: 0x0020A33C File Offset: 0x0020853C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
