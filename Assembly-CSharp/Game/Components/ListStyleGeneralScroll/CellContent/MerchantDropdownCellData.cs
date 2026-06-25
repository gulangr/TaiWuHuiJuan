using System;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000ECB RID: 3787
	public struct MerchantDropdownCellData
	{
		// Token: 0x0600AF02 RID: 44802 RVA: 0x004FBED0 File Offset: 0x004FA0D0
		public MerchantDropdownCellData(Action<int, int> dropdownSetAction, int charId)
		{
			this.DropdownSetAction = dropdownSetAction;
			this.CharacterId = charId;
		}

		// Token: 0x04008788 RID: 34696
		public readonly Action<int, int> DropdownSetAction;

		// Token: 0x04008789 RID: 34697
		public int CharacterId;
	}
}
