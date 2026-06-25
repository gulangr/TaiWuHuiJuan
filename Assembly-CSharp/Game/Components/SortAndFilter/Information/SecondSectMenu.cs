using System;

namespace Game.Components.SortAndFilter.Information
{
	// Token: 0x02000E03 RID: 3587
	public class SecondSectMenu : SectMenu
	{
		// Token: 0x170012D8 RID: 4824
		// (get) Token: 0x0600AAEA RID: 43754 RVA: 0x004E9BE7 File Offset: 0x004E7DE7
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(-1, 1));
			}
		}
	}
}
