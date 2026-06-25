using System;

namespace Game.Views.SectInteract
{
	// Token: 0x020009A7 RID: 2471
	[Flags]
	public enum EJieQingSignState
	{
		// Token: 0x040059EA RID: 23018
		None = 0,
		// Token: 0x040059EB RID: 23019
		Shaolin = 2,
		// Token: 0x040059EC RID: 23020
		Emei = 4,
		// Token: 0x040059ED RID: 23021
		Baihua = 8,
		// Token: 0x040059EE RID: 23022
		Wudang = 16,
		// Token: 0x040059EF RID: 23023
		Yuanshan = 32,
		// Token: 0x040059F0 RID: 23024
		Shixiang = 64,
		// Token: 0x040059F1 RID: 23025
		Ranshan = 128,
		// Token: 0x040059F2 RID: 23026
		Xuannv = 256,
		// Token: 0x040059F3 RID: 23027
		Zhujian = 512,
		// Token: 0x040059F4 RID: 23028
		Kongsang = 1024,
		// Token: 0x040059F5 RID: 23029
		Jingang = 2048,
		// Token: 0x040059F6 RID: 23030
		Wuxian = 4096,
		// Token: 0x040059F7 RID: 23031
		Jieqing = 8192,
		// Token: 0x040059F8 RID: 23032
		Fulong = 16384,
		// Token: 0x040059F9 RID: 23033
		Xuehou = 32768,
		// Token: 0x040059FA RID: 23034
		HonourSect = 262144,
		// Token: 0x040059FB RID: 23035
		NeutralSect = 524288,
		// Token: 0x040059FC RID: 23036
		DishonourSect = 1048576,
		// Token: 0x040059FD RID: 23037
		AllSect = 2097152
	}
}
