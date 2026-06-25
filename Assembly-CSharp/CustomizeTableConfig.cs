using System;
using System.Collections.Generic;

// Token: 0x020001FD RID: 509
public class CustomizeTableConfig
{
	// Token: 0x0400194E RID: 6478
	public Refers TableHeadTemplate;

	// Token: 0x0400194F RID: 6479
	public Refers TableHeadFirstTemplate;

	// Token: 0x04001950 RID: 6480
	public Refers TableRowTemplate;

	// Token: 0x04001951 RID: 6481
	public Refers TableHeaderRowTemplate;

	// Token: 0x04001952 RID: 6482
	public Dictionary<int, Refers> CellTemplates = new Dictionary<int, Refers>();

	// Token: 0x04001953 RID: 6483
	public Action<int, CommonCustomizeTableRow> OnItemRender;
}
