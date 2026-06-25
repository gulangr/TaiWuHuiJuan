using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D5E RID: 3422
	public class BracersMetalHardnessMenu : ArmorCommonMakeTypeMenu
	{
		// Token: 0x170011F9 RID: 4601
		// (get) Token: 0x0600A829 RID: 43049 RVA: 0x004E04C2 File Offset: 0x004DE6C2
		public override int Id
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x170011FA RID: 4602
		// (get) Token: 0x0600A82A RID: 43050 RVA: 0x004E04C5 File Offset: 0x004DE6C5
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 3));
			}
		}

		// Token: 0x170011FB RID: 4603
		// (get) Token: 0x0600A82B RID: 43051 RVA: 0x004E04D3 File Offset: 0x004DE6D3
		protected override sbyte MyResourceType
		{
			get
			{
				return 2;
			}
		}
	}
}
