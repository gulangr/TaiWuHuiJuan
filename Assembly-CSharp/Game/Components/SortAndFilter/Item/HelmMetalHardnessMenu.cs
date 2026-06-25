using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D4E RID: 3406
	public class HelmMetalHardnessMenu : ArmorCommonMakeTypeMenu
	{
		// Token: 0x170011DB RID: 4571
		// (get) Token: 0x0600A7FB RID: 43003 RVA: 0x004E032A File Offset: 0x004DE52A
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x170011DC RID: 4572
		// (get) Token: 0x0600A7FC RID: 43004 RVA: 0x004E032D File Offset: 0x004DE52D
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 3));
			}
		}

		// Token: 0x170011DD RID: 4573
		// (get) Token: 0x0600A7FD RID: 43005 RVA: 0x004E033B File Offset: 0x004DE53B
		protected override sbyte MyResourceType
		{
			get
			{
				return 2;
			}
		}
	}
}
