using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy.CharacterTable
{
	// Token: 0x0200057C RID: 1404
	public class CharacterTableSortAndFilterController : CommonSortAndFilterController<CharacterTableSortAndFilterData>
	{
		// Token: 0x060044B6 RID: 17590 RVA: 0x0020A6A4 File Offset: 0x002088A4
		public CharacterTableSortAndFilterController(CommonSortAndFilter sortAndFilter) : base(sortAndFilter)
		{
			this.SortController = new CharacterTableSortController();
		}

		// Token: 0x060044B7 RID: 17591 RVA: 0x0020A6C5 File Offset: 0x002088C5
		protected override IEnumerable<FilterLineBase<CharacterTableSortAndFilterData>> GenerateFilterLines()
		{
			foreach (FilterLineBase<CharacterTableSortAndFilterData> data in this.PreData)
			{
				yield return data;
				data = null;
			}
			List<FilterLineBase<CharacterTableSortAndFilterData>>.Enumerator enumerator = default(List<FilterLineBase<CharacterTableSortAndFilterData>>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x17000849 RID: 2121
		// (get) Token: 0x060044B8 RID: 17592 RVA: 0x0020A6D5 File Offset: 0x002088D5
		protected override string FilterCustomOrderKey
		{
			get
			{
				return "CharacterTableFilterCustomOrder";
			}
		}

		// Token: 0x0400301E RID: 12318
		public List<FilterLineBase<CharacterTableSortAndFilterData>> PreData = new List<FilterLineBase<CharacterTableSortAndFilterData>>();
	}
}
