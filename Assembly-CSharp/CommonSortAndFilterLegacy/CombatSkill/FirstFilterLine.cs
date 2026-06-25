using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace CommonSortAndFilterLegacy.CombatSkill
{
	// Token: 0x02000573 RID: 1395
	public class FirstFilterLine : DetailedFilterLine<CombatSkillDisplayData>
	{
		// Token: 0x17000835 RID: 2101
		// (get) Token: 0x06004480 RID: 17536 RVA: 0x00209D32 File Offset: 0x00207F32
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06004481 RID: 17537 RVA: 0x00209D35 File Offset: 0x00207F35
		protected override IEnumerable<DetailedFilterMenuBase<CombatSkillDisplayData>> GenerateMenus()
		{
			yield return new CombatSkillTypeMenu();
			yield return new SectMenu();
			yield return new FiveElementsMenu();
			yield break;
		}

		// Token: 0x17000836 RID: 2102
		// (get) Token: 0x06004482 RID: 17538 RVA: 0x00209D45 File Offset: 0x00207F45
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000837 RID: 2103
		// (get) Token: 0x06004483 RID: 17539 RVA: 0x00209D48 File Offset: 0x00207F48
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004484 RID: 17540 RVA: 0x00209D4C File Offset: 0x00207F4C
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
