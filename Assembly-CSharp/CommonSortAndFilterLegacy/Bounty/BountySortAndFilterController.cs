using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace CommonSortAndFilterLegacy.Bounty
{
	// Token: 0x020005A9 RID: 1449
	public class BountySortAndFilterController : CommonSortAndFilterController<CharacterDisplayDataForSettlementBounty>
	{
		// Token: 0x06004593 RID: 17811 RVA: 0x0020C720 File Offset: 0x0020A920
		public BountySortAndFilterController(CommonSortAndFilter sortAndFilter) : base(sortAndFilter)
		{
			this.SortController = new BountySortController();
		}

		// Token: 0x06004594 RID: 17812 RVA: 0x0020C736 File Offset: 0x0020A936
		protected override IEnumerable<FilterLineBase<CharacterDisplayDataForSettlementBounty>> GenerateFilterLines()
		{
			yield return new MainFilterLine();
			yield break;
		}

		// Token: 0x170008AE RID: 2222
		// (get) Token: 0x06004595 RID: 17813 RVA: 0x0020C746 File Offset: 0x0020A946
		protected override string FilterCustomOrderKey
		{
			get
			{
				return "BountyFilterCustomOrder";
			}
		}
	}
}
