using System;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EBD RID: 3773
	public class InputIntegerCellData
	{
		// Token: 0x0400874B RID: 34635
		public int Id;

		// Token: 0x0400874C RID: 34636
		public int MinValue = 0;

		// Token: 0x0400874D RID: 34637
		public int MaxValue = 9999;

		// Token: 0x0400874E RID: 34638
		public Func<int, int> GetAction;

		// Token: 0x0400874F RID: 34639
		public Action<int, int> SetAction;
	}
}
