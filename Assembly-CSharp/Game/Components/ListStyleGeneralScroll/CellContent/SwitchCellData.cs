using System;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EE7 RID: 3815
	public class SwitchCellData
	{
		// Token: 0x040087DA RID: 34778
		public int Id;

		// Token: 0x040087DB RID: 34779
		public Func<int, bool> GetAction;

		// Token: 0x040087DC RID: 34780
		public Action<int, bool> SetAction;
	}
}
