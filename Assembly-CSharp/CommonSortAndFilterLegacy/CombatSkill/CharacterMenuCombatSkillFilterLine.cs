using System;
using System.Collections.Generic;
using GameData.Domains.CombatSkill;

namespace CommonSortAndFilterLegacy.CombatSkill
{
	// Token: 0x0200056D RID: 1389
	public class CharacterMenuCombatSkillFilterLine : DetailedFilterLine<CombatSkillDisplayData>
	{
		// Token: 0x1700082F RID: 2095
		// (get) Token: 0x06004472 RID: 17522 RVA: 0x00209C96 File Offset: 0x00207E96
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06004473 RID: 17523 RVA: 0x00209C99 File Offset: 0x00207E99
		protected override IEnumerable<DetailedFilterMenuBase<CombatSkillDisplayData>> GenerateMenus()
		{
			yield return new CombatSkillTypeMenu();
			yield return new SectMenu();
			yield return new FiveElementsMenu();
			yield return new CombatSkillStatusMenuForCharacterMenu();
			yield break;
		}

		// Token: 0x17000830 RID: 2096
		// (get) Token: 0x06004474 RID: 17524 RVA: 0x00209CA9 File Offset: 0x00207EA9
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000831 RID: 2097
		// (get) Token: 0x06004475 RID: 17525 RVA: 0x00209CAC File Offset: 0x00207EAC
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004476 RID: 17526 RVA: 0x00209CB0 File Offset: 0x00207EB0
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
