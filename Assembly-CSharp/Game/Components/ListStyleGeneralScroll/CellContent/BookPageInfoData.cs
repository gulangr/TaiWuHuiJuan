using System;
using GameData.Domains.Item.Display;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EB4 RID: 3764
	public readonly struct BookPageInfoData
	{
		// Token: 0x0600AED0 RID: 44752 RVA: 0x004FA6C4 File Offset: 0x004F88C4
		public BookPageInfoData(ITradeableContent itemData)
		{
			this.ItemData = itemData;
		}

		// Token: 0x0400872A RID: 34602
		public readonly ITradeableContent ItemData;
	}
}
