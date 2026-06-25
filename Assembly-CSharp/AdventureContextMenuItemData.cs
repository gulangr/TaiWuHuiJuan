using System;
using System.Collections.Generic;

// Token: 0x02000168 RID: 360
public class AdventureContextMenuItemData
{
	// Token: 0x040010DE RID: 4318
	public string ItemName;

	// Token: 0x040010DF RID: 4319
	public Action ItemAction;

	// Token: 0x040010E0 RID: 4320
	public List<AdventureContextMenuItemData> ChildItems;

	// Token: 0x040010E1 RID: 4321
	public int GroupIndex;

	// Token: 0x040010E2 RID: 4322
	public bool HideContextMenu = true;
}
