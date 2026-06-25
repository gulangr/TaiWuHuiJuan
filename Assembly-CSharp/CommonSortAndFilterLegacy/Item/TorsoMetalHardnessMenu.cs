using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004E2 RID: 1250
	public class TorsoMetalHardnessMenu : TorsoCommonMakeTypeMenu
	{
		// Token: 0x17000787 RID: 1927
		// (get) Token: 0x0600427C RID: 17020 RVA: 0x00204377 File Offset: 0x00202577
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000788 RID: 1928
		// (get) Token: 0x0600427D RID: 17021 RVA: 0x0020437A File Offset: 0x0020257A
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 3));
			}
		}

		// Token: 0x17000789 RID: 1929
		// (get) Token: 0x0600427E RID: 17022 RVA: 0x00204388 File Offset: 0x00202588
		protected override sbyte MyResourceType
		{
			get
			{
				return 2;
			}
		}
	}
}
