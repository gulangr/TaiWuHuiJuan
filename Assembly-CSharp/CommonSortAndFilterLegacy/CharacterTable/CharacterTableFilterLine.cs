using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy.CharacterTable
{
	// Token: 0x0200057B RID: 1403
	public class CharacterTableFilterLine : DetailedFilterLine<CharacterTableSortAndFilterData>
	{
		// Token: 0x17000846 RID: 2118
		// (get) Token: 0x060044B0 RID: 17584 RVA: 0x0020A66C File Offset: 0x0020886C
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060044B1 RID: 17585 RVA: 0x0020A66F File Offset: 0x0020886F
		protected override IEnumerable<DetailedFilterMenuBase<CharacterTableSortAndFilterData>> GenerateMenus()
		{
			yield return new GenderMenu();
			yield return new BehaviorTypeMenu();
			yield return new RelationMenu();
			yield return new AdoredRelationMenu();
			yield return new EnemyRelationMenu();
			yield return new WorkStatusMenu();
			yield return new RoleArrangementMenu();
			yield return new IdentityMenu();
			yield return new SettlementMenu();
			yield return new SectMenu();
			yield return new CityMenu();
			yield return new FeastMenu();
			yield break;
		}

		// Token: 0x17000847 RID: 2119
		// (get) Token: 0x060044B2 RID: 17586 RVA: 0x0020A67F File Offset: 0x0020887F
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000848 RID: 2120
		// (get) Token: 0x060044B3 RID: 17587 RVA: 0x0020A682 File Offset: 0x00208882
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060044B4 RID: 17588 RVA: 0x0020A688 File Offset: 0x00208888
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
