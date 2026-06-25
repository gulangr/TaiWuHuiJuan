using System;

namespace Game.Components.SortAndFilter.Information
{
	// Token: 0x02000E04 RID: 3588
	public class SecondLifeSkillMenu : LifeSkillMenu
	{
		// Token: 0x170012D9 RID: 4825
		// (get) Token: 0x0600AAEC RID: 43756 RVA: 0x004E9BFE File Offset: 0x004E7DFE
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(-1, 2));
			}
		}
	}
}
