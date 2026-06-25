using System;

namespace Game.Components.ListStyleGeneralScroll
{
	// Token: 0x02000E9F RID: 3743
	public abstract class ColumnDefinition
	{
		// Token: 0x0600AD7B RID: 44411
		public abstract object CreateCellData(object rowData);

		// Token: 0x0600AD7C RID: 44412
		public abstract void SetCell(ICellContent cell, object cellData);

		// Token: 0x0600AD7D RID: 44413
		public abstract void SetSelected(ICellContent cell, bool isSelected);

		// Token: 0x0600AD7E RID: 44414
		public abstract bool SetDisabled(ICellContent cell, bool disabled);

		// Token: 0x04008605 RID: 34309
		public LayoutOption LayoutOption;

		// Token: 0x04008606 RID: 34310
		public Func<string> TableHeadLabel;

		// Token: 0x04008607 RID: 34311
		public Action<TooltipInvoker> RefreshTips;

		// Token: 0x04008608 RID: 34312
		public short SortId = -1;
	}
}
