using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy.Villager
{
	// Token: 0x02000467 RID: 1127
	public class MainFilterLine<T> : DetailedFilterLine<T> where T : IVillagerSortAndFilterData
	{
		// Token: 0x170006B8 RID: 1720
		// (get) Token: 0x06004096 RID: 16534 RVA: 0x001FFB93 File Offset: 0x001FDD93
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06004097 RID: 16535 RVA: 0x001FFB96 File Offset: 0x001FDD96
		protected override IEnumerable<DetailedFilterMenuBase<T>> GenerateMenus()
		{
			yield return new GenderMenu<T>();
			yield return new BehaviorTypeMenu<T>();
			yield return new RelationMenu<T>();
			yield return new AdoredRelationMenu<T>();
			yield return new EnemyRelationMenu<T>();
			yield return new WorkStatusMenu<T>();
			yield return new RoleArrangementMenu<T>();
			yield break;
		}

		// Token: 0x06004098 RID: 16536 RVA: 0x001FFBA8 File Offset: 0x001FDDA8
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x170006B9 RID: 1721
		// (get) Token: 0x06004099 RID: 16537 RVA: 0x001FFBBB File Offset: 0x001FDDBB
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170006BA RID: 1722
		// (get) Token: 0x0600409A RID: 16538 RVA: 0x001FFBBE File Offset: 0x001FDDBE
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}
	}
}
