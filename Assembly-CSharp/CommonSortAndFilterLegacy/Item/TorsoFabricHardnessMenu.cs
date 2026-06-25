using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004DF RID: 1247
	public class TorsoFabricHardnessMenu : TorsoCommonMakeTypeMenu
	{
		// Token: 0x1700077E RID: 1918
		// (get) Token: 0x06004270 RID: 17008 RVA: 0x00204320 File Offset: 0x00202520
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700077F RID: 1919
		// (get) Token: 0x06004271 RID: 17009 RVA: 0x00204323 File Offset: 0x00202523
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 0));
			}
		}

		// Token: 0x17000780 RID: 1920
		// (get) Token: 0x06004272 RID: 17010 RVA: 0x00204331 File Offset: 0x00202531
		protected override sbyte MyResourceType
		{
			get
			{
				return 4;
			}
		}
	}
}
