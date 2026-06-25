using System;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EE2 RID: 3810
	[Flags]
	public enum SingleButtonCellStatus
	{
		// Token: 0x040087C8 RID: 34760
		None = 0,
		// Token: 0x040087C9 RID: 34761
		Raycast = 1,
		// Token: 0x040087CA RID: 34762
		Interactable = 2,
		// Token: 0x040087CB RID: 34763
		Tip = 4,
		// Token: 0x040087CC RID: 34764
		DisableInteractable = 5,
		// Token: 0x040087CD RID: 34765
		EnableInteractable = 3
	}
}
