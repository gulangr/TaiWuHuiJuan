using System;

namespace Game.Components.SortAndFilter.Information
{
	// Token: 0x02000E05 RID: 3589
	public class SecondWesternMenu : WesternMenu
	{
		// Token: 0x170012DA RID: 4826
		// (get) Token: 0x0600AAEE RID: 43758 RVA: 0x004E9C15 File Offset: 0x004E7E15
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(-1, 3));
			}
		}
	}
}
