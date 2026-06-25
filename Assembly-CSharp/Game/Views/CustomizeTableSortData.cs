using System;

namespace Game.Views
{
	// Token: 0x020006E7 RID: 1767
	public class CustomizeTableSortData
	{
		// Token: 0x060053D3 RID: 21459 RVA: 0x0026D6FD File Offset: 0x0026B8FD
		public CustomizeTableSortData(int columnId)
		{
			this.ColumnId = columnId;
			this.IsDescending = true;
		}

		// Token: 0x040038B7 RID: 14519
		public int ColumnId;

		// Token: 0x040038B8 RID: 14520
		public bool IsDescending;
	}
}
