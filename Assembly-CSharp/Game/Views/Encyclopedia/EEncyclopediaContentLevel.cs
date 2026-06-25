using System;

namespace Game.Views.Encyclopedia
{
	// Token: 0x02000A57 RID: 2647
	[Flags]
	public enum EEncyclopediaContentLevel
	{
		// Token: 0x040063F8 RID: 25592
		None = 0,
		// Token: 0x040063F9 RID: 25593
		Low = 1,
		// Token: 0x040063FA RID: 25594
		Mid = 2,
		// Token: 0x040063FB RID: 25595
		LowMid = 3,
		// Token: 0x040063FC RID: 25596
		High = 4,
		// Token: 0x040063FD RID: 25597
		LowHigh = 5,
		// Token: 0x040063FE RID: 25598
		MidHigh = 6,
		// Token: 0x040063FF RID: 25599
		LowMidHigh = 7,
		// Token: 0x04006400 RID: 25600
		Inherit = 8
	}
}
