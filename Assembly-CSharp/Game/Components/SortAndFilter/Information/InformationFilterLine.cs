using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Information
{
	// Token: 0x02000E08 RID: 3592
	public class InformationFilterLine : DetailedFilterLineLogic<InformationSortAndFilterData>
	{
		// Token: 0x170012DD RID: 4829
		// (get) Token: 0x0600AAF4 RID: 43764 RVA: 0x004E9C5A File Offset: 0x004E7E5A
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170012DE RID: 4830
		// (get) Token: 0x0600AAF5 RID: 43765 RVA: 0x004E9C5D File Offset: 0x004E7E5D
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170012DF RID: 4831
		// (get) Token: 0x0600AAF6 RID: 43766 RVA: 0x004E9C60 File Offset: 0x004E7E60
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600AAF7 RID: 43767 RVA: 0x004E9C64 File Offset: 0x004E7E64
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x0600AAF8 RID: 43768 RVA: 0x004E9C77 File Offset: 0x004E7E77
		protected override IEnumerable<DetailedFilterMenuLogic<InformationSortAndFilterData>> GenerateMenus()
		{
			yield return new AreaMenu();
			yield return new SectMenu();
			yield return new LifeSkillMenu();
			yield return new WesternMenu();
			yield return new SwordTombMenu();
			yield return new ProfessionMenu();
			yield break;
		}
	}
}
