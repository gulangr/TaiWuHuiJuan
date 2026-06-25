using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000C8F RID: 3215
	public abstract class SecondaryFilterLineLogic<TData> : DetailedFilterLineLogic<TData>
	{
		// Token: 0x0600A3E3 RID: 41955 RVA: 0x004CA19C File Offset: 0x004C839C
		protected override LineConfig CreateConfig(List<DetailedFilterMenuConfig> menuConfigs)
		{
			return LineConfig.CreateDetailedSecondaryLineConfig(new DetailedFilterLineConfig(new DetailedFilterConfig
			{
				MenuConfigs = menuConfigs
			}));
		}
	}
}
