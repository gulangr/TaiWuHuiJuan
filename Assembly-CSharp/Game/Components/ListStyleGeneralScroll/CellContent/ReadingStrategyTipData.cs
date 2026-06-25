using System;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000ED8 RID: 3800
	public readonly struct ReadingStrategyTipData
	{
		// Token: 0x0600AF27 RID: 44839 RVA: 0x004FCD78 File Offset: 0x004FAF78
		public ReadingStrategyTipData(string name, string desc)
		{
			this.Name = name;
			this.Desc = desc;
		}

		// Token: 0x040087AC RID: 34732
		public readonly string Name;

		// Token: 0x040087AD RID: 34733
		public readonly string Desc;
	}
}
