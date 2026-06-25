using System;
using GameData.Domains.Item.Display;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EB2 RID: 3762
	public readonly struct BookPageInfoAndStateData
	{
		// Token: 0x0600AECA RID: 44746 RVA: 0x004FA43A File Offset: 0x004F863A
		public BookPageInfoAndStateData(ITradeableContent itemData, bool[] states)
		{
			this.ItemData = itemData;
			this.States = states;
		}

		// Token: 0x04008724 RID: 34596
		public readonly ITradeableContent ItemData;

		// Token: 0x04008725 RID: 34597
		public readonly bool[] States;
	}
}
