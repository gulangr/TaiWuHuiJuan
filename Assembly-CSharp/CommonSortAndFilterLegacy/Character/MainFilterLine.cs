using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy.Character
{
	// Token: 0x0200059E RID: 1438
	public class MainFilterLine<T> : DetailedFilterLine<T> where T : ICharacterSortAndFilterData
	{
		// Token: 0x17000890 RID: 2192
		// (get) Token: 0x06004557 RID: 17751 RVA: 0x0020BF8F File Offset: 0x0020A18F
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000891 RID: 2193
		// (get) Token: 0x06004558 RID: 17752 RVA: 0x0020BF92 File Offset: 0x0020A192
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000892 RID: 2194
		// (get) Token: 0x06004559 RID: 17753 RVA: 0x0020BF95 File Offset: 0x0020A195
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600455A RID: 17754 RVA: 0x0020BF98 File Offset: 0x0020A198
		protected override IEnumerable<DetailedFilterMenuBase<T>> GenerateMenus()
		{
			yield return new GenderMenu<T>();
			yield return new BehaviorTypeMenu<T>();
			yield return new RelationMenu<T>();
			yield return new AdoredRelationMenu<T>();
			yield return new EnemyRelationMenu<T>();
			yield return new OrganizationMenu<T>();
			yield return new OrganizationSectMenu<T>();
			yield return new OrganizationCityMenu<T>();
			yield return new OrganizationTownMenu<T>();
			yield return new OrganizationWalledTownMenu<T>();
			yield return new OrganizationVillageMenu<T>();
			yield break;
		}

		// Token: 0x0600455B RID: 17755 RVA: 0x0020BFA8 File Offset: 0x0020A1A8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
