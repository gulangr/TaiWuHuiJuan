using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D5D RID: 3421
	public class BracersJadeHardnessMenu : ArmorCommonMakeTypeMenu
	{
		// Token: 0x170011F6 RID: 4598
		// (get) Token: 0x0600A825 RID: 43045 RVA: 0x004E04A5 File Offset: 0x004DE6A5
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170011F7 RID: 4599
		// (get) Token: 0x0600A826 RID: 43046 RVA: 0x004E04A8 File Offset: 0x004DE6A8
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 2));
			}
		}

		// Token: 0x170011F8 RID: 4600
		// (get) Token: 0x0600A827 RID: 43047 RVA: 0x004E04B6 File Offset: 0x004DE6B6
		protected override sbyte MyResourceType
		{
			get
			{
				return 3;
			}
		}
	}
}
