using System;
using System.Collections.Generic;

namespace CommonSortAndFilterLegacy.Character
{
	// Token: 0x02000597 RID: 1431
	public class CharacterSortAndFilterController<T> : CommonSortAndFilterController<T> where T : ICharacterSortAndFilterData
	{
		// Token: 0x06004537 RID: 17719 RVA: 0x0020BBC1 File Offset: 0x00209DC1
		public CharacterSortAndFilterController(CommonSortAndFilter sortAndFilter) : base(sortAndFilter)
		{
			this.SortController = new CharacterSortController<T>();
		}

		// Token: 0x06004538 RID: 17720 RVA: 0x0020BBD7 File Offset: 0x00209DD7
		protected override IEnumerable<FilterLineBase<T>> GenerateFilterLines()
		{
			yield return new MainFilterLine<T>();
			yield break;
		}

		// Token: 0x1700087D RID: 2173
		// (get) Token: 0x06004539 RID: 17721 RVA: 0x0020BBE7 File Offset: 0x00209DE7
		protected override string FilterCustomOrderKey
		{
			get
			{
				return "CharacterFilterCustomOrder";
			}
		}
	}
}
