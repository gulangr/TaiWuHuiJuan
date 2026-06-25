using System;

namespace CommonSortAndFilterLegacy.Character
{
	// Token: 0x020005A5 RID: 1445
	public class OrganizationVillageMenu<T> : OrganizationSettlementTypeBaseMenu<T> where T : ICharacterSortAndFilterData
	{
		// Token: 0x170008A4 RID: 2212
		// (get) Token: 0x06004581 RID: 17793 RVA: 0x0020C535 File Offset: 0x0020A735
		protected override LanguageKey MenuBarLabelKey
		{
			get
			{
				return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_10;
			}
		}

		// Token: 0x170008A5 RID: 2213
		// (get) Token: 0x06004582 RID: 17794 RVA: 0x0020C53C File Offset: 0x0020A73C
		protected override sbyte OrganizationKey
		{
			get
			{
				return 36;
			}
		}

		// Token: 0x170008A6 RID: 2214
		// (get) Token: 0x06004583 RID: 17795 RVA: 0x0020C540 File Offset: 0x0020A740
		protected override EOrganizationMenuOption DependencyMenuOption
		{
			get
			{
				return EOrganizationMenuOption.Village;
			}
		}

		// Token: 0x170008A7 RID: 2215
		// (get) Token: 0x06004584 RID: 17796 RVA: 0x0020C543 File Offset: 0x0020A743
		public override int Id
		{
			get
			{
				return 10;
			}
		}
	}
}
