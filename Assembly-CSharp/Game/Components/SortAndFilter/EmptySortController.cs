using System;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000C93 RID: 3219
	public class EmptySortController : SortController<ITradeableContent>
	{
		// Token: 0x0600A3F1 RID: 41969 RVA: 0x004CA21D File Offset: 0x004C841D
		public override Comparison<ITradeableContent> GenerateComparer(SortStateData sortData)
		{
			return null;
		}

		// Token: 0x0600A3F2 RID: 41970 RVA: 0x004CA220 File Offset: 0x004C8420
		public override SortUiConfig GenerateConfig()
		{
			return default(SortUiConfig);
		}
	}
}
