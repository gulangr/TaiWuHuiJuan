using System;
using GameData.Domains.Item.Display;

namespace Game.Views.Select
{
	// Token: 0x020007A4 RID: 1956
	public class SelectedItemData
	{
		// Token: 0x06005E96 RID: 24214 RVA: 0x002B7C5B File Offset: 0x002B5E5B
		public SelectedItemData(ITradeableContent itemData, int amount)
		{
			this.ItemData = itemData;
			this.SelectedAmount = amount;
			this.IsCancelled = false;
		}

		// Token: 0x0400417A RID: 16762
		public ITradeableContent ItemData;

		// Token: 0x0400417B RID: 16763
		public int SelectedAmount;

		// Token: 0x0400417C RID: 16764
		public bool IsCancelled;
	}
}
