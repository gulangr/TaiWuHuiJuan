using System;

namespace Game.Components.SortAndFilter.Information
{
	// Token: 0x02000E07 RID: 3591
	public class SecondProfessionMenu : ProfessionMenu
	{
		// Token: 0x170012DC RID: 4828
		// (get) Token: 0x0600AAF2 RID: 43762 RVA: 0x004E9C43 File Offset: 0x004E7E43
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(-1, 6));
			}
		}
	}
}
