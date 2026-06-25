using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace CommonSortAndFilterLegacy.CombatSkill
{
	// Token: 0x0200056F RID: 1391
	public class CharacterMenuCombatSkillSortAndFilterController : CommonSortAndFilterController<CombatSkillDisplayData>
	{
		// Token: 0x17000833 RID: 2099
		// (get) Token: 0x0600447A RID: 17530 RVA: 0x00209CD8 File Offset: 0x00207ED8
		protected override string FilterCustomOrderKey
		{
			get
			{
				return "CombatSkillFilterForCharacterMenuCombatSkillCustomOrder";
			}
		}

		// Token: 0x0600447B RID: 17531 RVA: 0x00209CDF File Offset: 0x00207EDF
		public CharacterMenuCombatSkillSortAndFilterController(CommonSortAndFilter sortAndFilter) : base(sortAndFilter)
		{
			this.SortController = new CombatSkillSortController();
		}

		// Token: 0x0600447C RID: 17532 RVA: 0x00209CF5 File Offset: 0x00207EF5
		protected override IEnumerable<FilterLineBase<CombatSkillDisplayData>> GenerateFilterLines()
		{
			yield return new CharacterMenuCombatSkillFilterLine();
			yield break;
		}
	}
}
