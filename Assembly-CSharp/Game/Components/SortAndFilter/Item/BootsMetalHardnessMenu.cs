using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D66 RID: 3430
	public class BootsMetalHardnessMenu : ArmorCommonMakeTypeMenu
	{
		// Token: 0x17001208 RID: 4616
		// (get) Token: 0x0600A840 RID: 43072 RVA: 0x004E058E File Offset: 0x004DE78E
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17001209 RID: 4617
		// (get) Token: 0x0600A841 RID: 43073 RVA: 0x004E0591 File Offset: 0x004DE791
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 3));
			}
		}

		// Token: 0x1700120A RID: 4618
		// (get) Token: 0x0600A842 RID: 43074 RVA: 0x004E059F File Offset: 0x004DE79F
		protected override sbyte MyResourceType
		{
			get
			{
				return 2;
			}
		}
	}
}
