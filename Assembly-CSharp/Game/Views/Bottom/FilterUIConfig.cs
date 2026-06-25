using System;
using System.Collections.Generic;
using GameData.Domains.Map;

namespace Game.Views.Bottom
{
	// Token: 0x02000C54 RID: 3156
	public class FilterUIConfig
	{
		// Token: 0x0600A09D RID: 41117 RVA: 0x004AFB74 File Offset: 0x004ADD74
		public FilterUIConfig()
		{
			this.ItemConfigs = new Dictionary<EFilterItemKey, FilterItemConfig>();
			this.TertiaryConfigs = new Dictionary<FilterLevelKey, EFilterItemKey[]>();
		}

		// Token: 0x04007C91 RID: 31889
		public Dictionary<EFilterItemKey, FilterItemConfig> ItemConfigs;

		// Token: 0x04007C92 RID: 31890
		public Dictionary<FilterLevelKey, EFilterItemKey[]> TertiaryConfigs;
	}
}
