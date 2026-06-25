using System;

namespace CommonSortAndFilterLegacy.Character
{
	// Token: 0x020005A4 RID: 1444
	public class OrganizationTownMenu<T> : OrganizationSettlementTypeBaseMenu<T> where T : ICharacterSortAndFilterData
	{
		// Token: 0x170008A0 RID: 2208
		// (get) Token: 0x0600457C RID: 17788 RVA: 0x0020C51B File Offset: 0x0020A71B
		protected override LanguageKey MenuBarLabelKey
		{
			get
			{
				return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_8;
			}
		}

		// Token: 0x170008A1 RID: 2209
		// (get) Token: 0x0600457D RID: 17789 RVA: 0x0020C522 File Offset: 0x0020A722
		protected override sbyte OrganizationKey
		{
			get
			{
				return 37;
			}
		}

		// Token: 0x170008A2 RID: 2210
		// (get) Token: 0x0600457E RID: 17790 RVA: 0x0020C526 File Offset: 0x0020A726
		protected override EOrganizationMenuOption DependencyMenuOption
		{
			get
			{
				return EOrganizationMenuOption.Town;
			}
		}

		// Token: 0x170008A3 RID: 2211
		// (get) Token: 0x0600457F RID: 17791 RVA: 0x0020C529 File Offset: 0x0020A729
		public override int Id
		{
			get
			{
				return 8;
			}
		}
	}
}
