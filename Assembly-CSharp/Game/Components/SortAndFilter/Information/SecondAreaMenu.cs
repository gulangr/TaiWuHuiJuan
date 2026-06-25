using System;

namespace Game.Components.SortAndFilter.Information
{
	// Token: 0x02000E02 RID: 3586
	public class SecondAreaMenu : AreaMenu
	{
		// Token: 0x170012D7 RID: 4823
		// (get) Token: 0x0600AAE8 RID: 43752 RVA: 0x004E9BD0 File Offset: 0x004E7DD0
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(-1, 0));
			}
		}
	}
}
