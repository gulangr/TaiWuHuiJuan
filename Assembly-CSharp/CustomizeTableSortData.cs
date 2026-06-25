using System;

// Token: 0x02000200 RID: 512
public class CustomizeTableSortData
{
	// Token: 0x060020FE RID: 8446 RVA: 0x000F0688 File Offset: 0x000EE888
	public CustomizeTableSortData(int columnId)
	{
		this.ColumnId = columnId;
		this.IsDescending = true;
	}

	// Token: 0x0400195D RID: 6493
	public int ColumnId;

	// Token: 0x0400195E RID: 6494
	public bool IsDescending;
}
