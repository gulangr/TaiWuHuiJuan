using System;
using System.Collections.Generic;

// Token: 0x020001FE RID: 510
public class CustomizeTablePageConfig
{
	// Token: 0x17000351 RID: 849
	// (get) Token: 0x060020F9 RID: 8441 RVA: 0x000F05B1 File Offset: 0x000EE7B1
	public string Title
	{
		get
		{
			return (this.TitleKey == LanguageKey.Invalid) ? string.Empty : LocalStringManager.Get(this.TitleKey);
		}
	}

	// Token: 0x060020FA RID: 8442 RVA: 0x000F05CE File Offset: 0x000EE7CE
	public CustomizeTablePageConfig(List<CustomizeTableColumeConfig> columnConfigs, LanguageKey titleKey, Action<List<CustomizeTableSortData>> onSort)
	{
		this.ColumnConfigs = columnConfigs;
		this.TitleKey = titleKey;
		this.OnSort = onSort;
	}

	// Token: 0x04001954 RID: 6484
	public List<CustomizeTableColumeConfig> ColumnConfigs;

	// Token: 0x04001955 RID: 6485
	public LanguageKey TitleKey;

	// Token: 0x04001956 RID: 6486
	public Action<List<CustomizeTableSortData>> OnSort;
}
