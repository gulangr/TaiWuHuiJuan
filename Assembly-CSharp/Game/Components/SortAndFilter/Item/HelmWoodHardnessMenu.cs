using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D4C RID: 3404
	public class HelmWoodHardnessMenu : ArmorCommonMakeTypeMenu
	{
		// Token: 0x170011D5 RID: 4565
		// (get) Token: 0x0600A7F3 RID: 42995 RVA: 0x004E02F0 File Offset: 0x004DE4F0
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170011D6 RID: 4566
		// (get) Token: 0x0600A7F4 RID: 42996 RVA: 0x004E02F3 File Offset: 0x004DE4F3
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 1));
			}
		}

		// Token: 0x170011D7 RID: 4567
		// (get) Token: 0x0600A7F5 RID: 42997 RVA: 0x004E0301 File Offset: 0x004DE501
		protected override sbyte MyResourceType
		{
			get
			{
				return 1;
			}
		}
	}
}
