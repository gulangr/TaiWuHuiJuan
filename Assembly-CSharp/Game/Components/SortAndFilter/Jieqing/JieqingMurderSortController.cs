using System;
using System.Collections.Generic;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Jieqing
{
	// Token: 0x02000D23 RID: 3363
	public class JieqingMurderSortController : SortController<CharacterDisplayData>
	{
		// Token: 0x0600A766 RID: 42854 RVA: 0x004DEE00 File Offset: 0x004DD000
		public override Comparison<CharacterDisplayData> GenerateComparer(SortStateData sortData)
		{
			return (CharacterDisplayData x, CharacterDisplayData y) => this.CompareData(x, y, sortData);
		}

		// Token: 0x0600A767 RID: 42855 RVA: 0x004DEE34 File Offset: 0x004DD034
		private int CompareData(CharacterDisplayData x, CharacterDisplayData y, SortStateData sortData)
		{
			bool flag = x == null && y == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = x == null;
				if (flag2)
				{
					result = 1;
				}
				else
				{
					bool flag3 = y == null;
					if (flag3)
					{
						result = -1;
					}
					else
					{
						bool flag4 = ((sortData != null) ? sortData.ItemStates : null) != null;
						if (flag4)
						{
							foreach (SortItemState itemState in sortData.ItemStates)
							{
								short sortId = itemState.SortId;
								ESortDirection order = itemState.SortDirection;
								int comparisonResult = this.CompareBySortId(x, y, sortId);
								bool flag5 = comparisonResult != 0;
								if (flag5)
								{
									return (order == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
								}
							}
						}
						result = x.CharacterId.CompareTo(y.CharacterId);
					}
				}
			}
			return result;
		}

		// Token: 0x0600A768 RID: 42856 RVA: 0x004DEF20 File Offset: 0x004DD120
		private int CompareBySortId(CharacterDisplayData x, CharacterDisplayData y, short sortId)
		{
			return this.CompareBySortId(x, y, sortId);
		}

		// Token: 0x0600A769 RID: 42857 RVA: 0x004DEF3C File Offset: 0x004DD13C
		public override SortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				0,
				1
			};
			return new SortUiConfig
			{
				SortIds = sortIds,
				SortNameIndexList = new List<int>(new int[sortIds.Count]),
				DefaultSortIds = new List<short>
				{
					0,
					1
				},
				DefaultSortState = default(SortUiState)
			};
		}

		// Token: 0x02002450 RID: 9296
		public static class SortIds
		{
			// Token: 0x0400E3D2 RID: 58322
			public const short Sect = 0;

			// Token: 0x0400E3D3 RID: 58323
			public const short MapState = 1;
		}
	}
}
