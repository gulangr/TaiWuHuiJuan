using System;
using System.Collections.Generic;
using FrameWork;

namespace CommonSortAndFilterLegacy.Character
{
	// Token: 0x02000598 RID: 1432
	public class CharacterSortController<T> : CommonSortController<T> where T : ICharacterSortAndFilterData
	{
		// Token: 0x0600453A RID: 17722 RVA: 0x0020BBF0 File Offset: 0x00209DF0
		public override void Sort(List<T> dataList, SortStateData sortData, Action actionAfterSort)
		{
			dataList.Sort(delegate(T x, T y)
			{
				foreach (SortItemState itemState in sortData.ItemStates)
				{
					short sortId = itemState.SortId;
					ESortDirection order = itemState.SortDirection;
					int result;
					switch (sortId)
					{
					case 0:
					{
						string xName = x.Name;
						string yName = y.Name;
						result = Utils_Sorting.CompareByCurrentLangEncoding(xName, yName);
						goto IL_196;
					}
					case 1:
						result = x.Grade.CompareTo(y.Grade);
						goto IL_196;
					case 8:
						result = x.Age.CompareTo(y.Age);
						goto IL_196;
					case 9:
						result = x.Charm.CompareTo(y.Charm);
						goto IL_196;
					case 10:
						result = x.Health.CompareTo(y.Health);
						goto IL_196;
					case 11:
						result = x.FavorabilityToTaiwu.CompareTo(y.FavorabilityToTaiwu);
						goto IL_196;
					case 12:
						result = x.Happiness.CompareTo(y.Happiness);
						goto IL_196;
					}
					continue;
					IL_196:
					bool flag = result != 0;
					if (flag)
					{
						return (order == ESortDirection.Ascending) ? result : (-result);
					}
				}
				return 0;
			});
			if (actionAfterSort != null)
			{
				actionAfterSort();
			}
		}

		// Token: 0x0600453B RID: 17723 RVA: 0x0020BC2C File Offset: 0x00209E2C
		public override CommonSortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				0,
				1,
				8,
				9,
				10,
				11,
				12
			};
			return new CommonSortUiConfig
			{
				SortIds = sortIds,
				SortNameIndexList = new List<int>
				{
					0,
					1,
					0,
					0,
					0,
					0,
					0
				},
				DefaultSortState = null,
				DefaultSortIds = sortIds
			};
		}
	}
}
