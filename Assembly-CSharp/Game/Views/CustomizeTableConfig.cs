using System;
using System.Collections.Generic;

namespace Game.Views
{
	// Token: 0x020006E4 RID: 1764
	public class CustomizeTableConfig
	{
		// Token: 0x040038A8 RID: 14504
		public Refers TableHeadTemplate;

		// Token: 0x040038A9 RID: 14505
		public Refers TableHeadFirstTemplate;

		// Token: 0x040038AA RID: 14506
		public Refers TableRowTemplate;

		// Token: 0x040038AB RID: 14507
		public Refers TableHeaderRowTemplate;

		// Token: 0x040038AC RID: 14508
		public Dictionary<int, Refers> CellTemplates = new Dictionary<int, Refers>();

		// Token: 0x040038AD RID: 14509
		public Action<int, CommonCustomizeTableRowComponent> OnItemRender;
	}
}
