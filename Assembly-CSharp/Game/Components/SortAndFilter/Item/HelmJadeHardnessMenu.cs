using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D4D RID: 3405
	public class HelmJadeHardnessMenu : ArmorCommonMakeTypeMenu
	{
		// Token: 0x170011D8 RID: 4568
		// (get) Token: 0x0600A7F7 RID: 42999 RVA: 0x004E030D File Offset: 0x004DE50D
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170011D9 RID: 4569
		// (get) Token: 0x0600A7F8 RID: 43000 RVA: 0x004E0310 File Offset: 0x004DE510
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 2));
			}
		}

		// Token: 0x170011DA RID: 4570
		// (get) Token: 0x0600A7F9 RID: 43001 RVA: 0x004E031E File Offset: 0x004DE51E
		protected override sbyte MyResourceType
		{
			get
			{
				return 3;
			}
		}
	}
}
