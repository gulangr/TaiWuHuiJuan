using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004EB RID: 1259
	public class WeaponFabricHardnessMenu : WeaponCommonMakeTypeMenu
	{
		// Token: 0x17000794 RID: 1940
		// (get) Token: 0x0600429E RID: 17054 RVA: 0x00204A97 File Offset: 0x00202C97
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x17000795 RID: 1941
		// (get) Token: 0x0600429F RID: 17055 RVA: 0x00204A9A File Offset: 0x00202C9A
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(1, 0));
			}
		}

		// Token: 0x17000796 RID: 1942
		// (get) Token: 0x060042A0 RID: 17056 RVA: 0x00204AA8 File Offset: 0x00202CA8
		protected override sbyte MyResourceType
		{
			get
			{
				return 4;
			}
		}
	}
}
