using System;
using GameData.Domains.Item.Display;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000ED4 RID: 3796
	public readonly struct ReadingBookIconAndNameCellData
	{
		// Token: 0x0600AF1F RID: 44831 RVA: 0x004FC9D7 File Offset: 0x004FABD7
		public ReadingBookIconAndNameCellData(ITradeableContent itemData, bool isFinished)
		{
			this.ItemData = itemData;
			this.IsFinished = isFinished;
		}

		// Token: 0x0400879E RID: 34718
		public readonly ITradeableContent ItemData;

		// Token: 0x0400879F RID: 34719
		public readonly bool IsFinished;
	}
}
