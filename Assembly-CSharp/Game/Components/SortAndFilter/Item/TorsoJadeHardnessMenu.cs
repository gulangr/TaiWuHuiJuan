using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D55 RID: 3413
	public class TorsoJadeHardnessMenu : ArmorCommonMakeTypeMenu
	{
		// Token: 0x170011E7 RID: 4583
		// (get) Token: 0x0600A80E RID: 43022 RVA: 0x004E03D9 File Offset: 0x004DE5D9
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170011E8 RID: 4584
		// (get) Token: 0x0600A80F RID: 43023 RVA: 0x004E03DC File Offset: 0x004DE5DC
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 2));
			}
		}

		// Token: 0x170011E9 RID: 4585
		// (get) Token: 0x0600A810 RID: 43024 RVA: 0x004E03EA File Offset: 0x004DE5EA
		protected override sbyte MyResourceType
		{
			get
			{
				return 3;
			}
		}
	}
}
