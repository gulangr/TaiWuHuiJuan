using System;

namespace Game.Views.SectInteract
{
	// Token: 0x020009A9 RID: 2473
	[Flags]
	public enum EBoardGridState
	{
		// Token: 0x04005A47 RID: 23111
		None = 0,
		// Token: 0x04005A48 RID: 23112
		Top = 1,
		// Token: 0x04005A49 RID: 23113
		Left = 2,
		// Token: 0x04005A4A RID: 23114
		Right = 4,
		// Token: 0x04005A4B RID: 23115
		Bottom = 8,
		// Token: 0x04005A4C RID: 23116
		All = 15
	}
}
