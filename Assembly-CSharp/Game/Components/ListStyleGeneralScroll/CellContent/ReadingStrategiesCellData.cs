using System;
using System.Collections.Generic;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000ED9 RID: 3801
	public readonly struct ReadingStrategiesCellData
	{
		// Token: 0x170013D2 RID: 5074
		// (get) Token: 0x0600AF28 RID: 44840 RVA: 0x004FCD89 File Offset: 0x004FAF89
		public static ReadingStrategiesCellData Empty
		{
			get
			{
				return new ReadingStrategiesCellData(Array.Empty<string>(), Array.Empty<ReadingStrategyTipData>());
			}
		}

		// Token: 0x0600AF29 RID: 44841 RVA: 0x004FCD9A File Offset: 0x004FAF9A
		public ReadingStrategiesCellData(IReadOnlyList<string> strategyNames, IReadOnlyList<ReadingStrategyTipData> strategyTips)
		{
			this.StrategyNames = strategyNames;
			this.StrategyTips = strategyTips;
		}

		// Token: 0x040087AE RID: 34734
		public readonly IReadOnlyList<string> StrategyNames;

		// Token: 0x040087AF RID: 34735
		public readonly IReadOnlyList<ReadingStrategyTipData> StrategyTips;
	}
}
