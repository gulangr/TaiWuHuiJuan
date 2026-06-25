using System;
using System.Collections.Generic;

namespace Game.Views
{
	// Token: 0x020006E5 RID: 1765
	public class CustomizeTablePageConfig
	{
		// Token: 0x17000A58 RID: 2648
		// (get) Token: 0x060053CE RID: 21454 RVA: 0x0026D626 File Offset: 0x0026B826
		public string Title
		{
			get
			{
				return (this.TitleKey == LanguageKey.Invalid) ? string.Empty : LocalStringManager.Get(this.TitleKey);
			}
		}

		// Token: 0x060053CF RID: 21455 RVA: 0x0026D643 File Offset: 0x0026B843
		public CustomizeTablePageConfig(List<CustomizeTableColumeConfig> columnConfigs, LanguageKey titleKey, Action<List<CustomizeTableSortData>> onSort)
		{
			this.ColumnConfigs = columnConfigs;
			this.TitleKey = titleKey;
			this.OnSort = onSort;
		}

		// Token: 0x040038AE RID: 14510
		public List<CustomizeTableColumeConfig> ColumnConfigs;

		// Token: 0x040038AF RID: 14511
		public LanguageKey TitleKey;

		// Token: 0x040038B0 RID: 14512
		public Action<List<CustomizeTableSortData>> OnSort;
	}
}
