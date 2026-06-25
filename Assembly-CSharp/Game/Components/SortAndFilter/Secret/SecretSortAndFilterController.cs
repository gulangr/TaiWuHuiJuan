using System;
using System.Collections.Generic;

namespace Game.Components.SortAndFilter.Secret
{
	// Token: 0x02000CE0 RID: 3296
	public class SecretSortAndFilterController : SortAndFilterController<SecretSortAndFilterData>
	{
		// Token: 0x0600A64A RID: 42570 RVA: 0x004D6897 File Offset: 0x004D4A97
		public SecretSortAndFilterController(SortAndFilter sortAndFilter) : base(sortAndFilter, LanguageKey.LK_SecretInformation)
		{
			this.SortController = new SecretSortController();
		}

		// Token: 0x0600A64B RID: 42571 RVA: 0x004D68B2 File Offset: 0x004D4AB2
		protected override IEnumerable<FilterLineBase<SecretSortAndFilterData>> GenerateFilterLines()
		{
			yield return new RelationFilterLine();
			yield return new TypeFilterLine();
			yield break;
		}
	}
}
