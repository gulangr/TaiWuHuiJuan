using System;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004E0 RID: 1248
	public class TorsoWoodHardnessMenu : TorsoCommonMakeTypeMenu
	{
		// Token: 0x17000781 RID: 1921
		// (get) Token: 0x06004274 RID: 17012 RVA: 0x0020433D File Offset: 0x0020253D
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x17000782 RID: 1922
		// (get) Token: 0x06004275 RID: 17013 RVA: 0x00204340 File Offset: 0x00202540
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 1));
			}
		}

		// Token: 0x17000783 RID: 1923
		// (get) Token: 0x06004276 RID: 17014 RVA: 0x0020434E File Offset: 0x0020254E
		protected override sbyte MyResourceType
		{
			get
			{
				return 1;
			}
		}
	}
}
