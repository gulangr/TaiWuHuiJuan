using System;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EE3 RID: 3811
	public class SingleButtonCellData
	{
		// Token: 0x040087CE RID: 34766
		public Action OnClick;

		// Token: 0x040087CF RID: 34767
		public string LabelText;

		// Token: 0x040087D0 RID: 34768
		public string MouseTipText;

		// Token: 0x040087D1 RID: 34769
		public SingleButtonCellStatus SingleButtonCellStatus = SingleButtonCellStatus.EnableInteractable;
	}
}
