using System;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D6F RID: 3439
	public class AccessoryJadeHardnessMenu : AccessoryCommonMakeTypeMenu
	{
		// Token: 0x17001219 RID: 4633
		// (get) Token: 0x0600A865 RID: 43109 RVA: 0x004E0CDA File Offset: 0x004DEEDA
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x1700121A RID: 4634
		// (get) Token: 0x0600A866 RID: 43110 RVA: 0x004E0CDD File Offset: 0x004DEEDD
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 2));
			}
		}

		// Token: 0x1700121B RID: 4635
		// (get) Token: 0x0600A867 RID: 43111 RVA: 0x004E0CEB File Offset: 0x004DEEEB
		protected override sbyte MyResourceType
		{
			get
			{
				return 3;
			}
		}
	}
}
