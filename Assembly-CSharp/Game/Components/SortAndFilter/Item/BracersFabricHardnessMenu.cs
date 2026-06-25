using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D5B RID: 3419
	public class BracersFabricHardnessMenu : ArmorCommonMakeTypeMenu
	{
		// Token: 0x170011F0 RID: 4592
		// (get) Token: 0x0600A81D RID: 43037 RVA: 0x004E046B File Offset: 0x004DE66B
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x170011F1 RID: 4593
		// (get) Token: 0x0600A81E RID: 43038 RVA: 0x004E046E File Offset: 0x004DE66E
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 0));
			}
		}

		// Token: 0x170011F2 RID: 4594
		// (get) Token: 0x0600A81F RID: 43039 RVA: 0x004E047C File Offset: 0x004DE67C
		protected override sbyte MyResourceType
		{
			get
			{
				return 4;
			}
		}
	}
}
