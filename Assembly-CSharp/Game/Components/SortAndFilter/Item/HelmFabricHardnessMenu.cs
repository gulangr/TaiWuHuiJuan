using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D4B RID: 3403
	public class HelmFabricHardnessMenu : ArmorCommonMakeTypeMenu
	{
		// Token: 0x170011D2 RID: 4562
		// (get) Token: 0x0600A7EF RID: 42991 RVA: 0x004E02D3 File Offset: 0x004DE4D3
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170011D3 RID: 4563
		// (get) Token: 0x0600A7F0 RID: 42992 RVA: 0x004E02D6 File Offset: 0x004DE4D6
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 0));
			}
		}

		// Token: 0x170011D4 RID: 4564
		// (get) Token: 0x0600A7F1 RID: 42993 RVA: 0x004E02E4 File Offset: 0x004DE4E4
		protected override sbyte MyResourceType
		{
			get
			{
				return 4;
			}
		}
	}
}
