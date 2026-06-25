using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D54 RID: 3412
	public class TorsoWoodHardnessMenu : ArmorCommonMakeTypeMenu
	{
		// Token: 0x170011E4 RID: 4580
		// (get) Token: 0x0600A80A RID: 43018 RVA: 0x004E03BC File Offset: 0x004DE5BC
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x170011E5 RID: 4581
		// (get) Token: 0x0600A80B RID: 43019 RVA: 0x004E03BF File Offset: 0x004DE5BF
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 1));
			}
		}

		// Token: 0x170011E6 RID: 4582
		// (get) Token: 0x0600A80C RID: 43020 RVA: 0x004E03CD File Offset: 0x004DE5CD
		protected override sbyte MyResourceType
		{
			get
			{
				return 1;
			}
		}
	}
}
