using System;

namespace CommonSortAndFilterLegacy.Character
{
	// Token: 0x020005A6 RID: 1446
	public class OrganizationWalledTownMenu<T> : OrganizationSettlementTypeBaseMenu<T> where T : ICharacterSortAndFilterData
	{
		// Token: 0x170008A8 RID: 2216
		// (get) Token: 0x06004586 RID: 17798 RVA: 0x0020C550 File Offset: 0x0020A750
		protected override LanguageKey MenuBarLabelKey
		{
			get
			{
				return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Character_9;
			}
		}

		// Token: 0x170008A9 RID: 2217
		// (get) Token: 0x06004587 RID: 17799 RVA: 0x0020C557 File Offset: 0x0020A757
		protected override sbyte OrganizationKey
		{
			get
			{
				return 38;
			}
		}

		// Token: 0x170008AA RID: 2218
		// (get) Token: 0x06004588 RID: 17800 RVA: 0x0020C55B File Offset: 0x0020A75B
		protected override EOrganizationMenuOption DependencyMenuOption
		{
			get
			{
				return EOrganizationMenuOption.WalledTown;
			}
		}

		// Token: 0x170008AB RID: 2219
		// (get) Token: 0x06004589 RID: 17801 RVA: 0x0020C55E File Offset: 0x0020A75E
		public override int Id
		{
			get
			{
				return 9;
			}
		}
	}
}
