using System;

namespace Game.Components.SortAndFilter.Information
{
	// Token: 0x02000E06 RID: 3590
	public class SecondSwordTombMenu : SwordTombMenu
	{
		// Token: 0x170012DB RID: 4827
		// (get) Token: 0x0600AAF0 RID: 43760 RVA: 0x004E9C2C File Offset: 0x004E7E2C
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(-1, 5));
			}
		}
	}
}
