using System;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000ED6 RID: 3798
	public readonly struct ReadingBookPageInfoCellData
	{
		// Token: 0x0600AF22 RID: 44834 RVA: 0x004FCAE6 File Offset: 0x004FACE6
		public ReadingBookPageInfoCellData(sbyte[] pageStates, sbyte[] pageProgress, sbyte[] pageTypes, bool isCombatBook, bool[] supplyStates)
		{
			this.PageStates = pageStates;
			this.PageProgress = pageProgress;
			this.PageTypes = pageTypes;
			this.IsCombatBook = isCombatBook;
			this.SupplyStates = supplyStates;
		}

		// Token: 0x040087A4 RID: 34724
		public readonly sbyte[] PageStates;

		// Token: 0x040087A5 RID: 34725
		public readonly sbyte[] PageProgress;

		// Token: 0x040087A6 RID: 34726
		public readonly sbyte[] PageTypes;

		// Token: 0x040087A7 RID: 34727
		public readonly bool IsCombatBook;

		// Token: 0x040087A8 RID: 34728
		public readonly bool[] SupplyStates;
	}
}
