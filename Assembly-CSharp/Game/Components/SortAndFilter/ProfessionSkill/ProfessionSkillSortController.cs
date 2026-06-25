using System;
using System.Collections.Generic;
using FrameWork;

namespace Game.Components.SortAndFilter.ProfessionSkill
{
	// Token: 0x02000D0D RID: 3341
	public class ProfessionSkillSortController : SortController<ProfessionSkillSortData>
	{
		// Token: 0x0600A712 RID: 42770 RVA: 0x004DBB40 File Offset: 0x004D9D40
		public override Comparison<ProfessionSkillSortData> GenerateComparer(SortStateData sortData)
		{
			return (ProfessionSkillSortData x, ProfessionSkillSortData y) => ProfessionSkillSortController.CompareData(x, y, sortData);
		}

		// Token: 0x0600A713 RID: 42771 RVA: 0x004DBB6C File Offset: 0x004D9D6C
		private static int CompareData(ProfessionSkillSortData x, ProfessionSkillSortData y, SortStateData sortData)
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
								short num = sortId;
								int comparisonResult;
								if (num != 152)
								{
									if (num != 153)
									{
										continue;
									}
									comparisonResult = x.Seniority.CompareTo(y.Seniority);
								}
								else
								{
									comparisonResult = Utils_Sorting.CompareByCurrentLangEncoding(x.SkillName, y.SkillName);
								}
								bool flag5 = comparisonResult != 0;
								if (flag5)
								{
									return (itemState.SortDirection == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
								}
							}
						}
						int nameCompare = Utils_Sorting.CompareByCurrentLangEncoding(x.SkillName, y.SkillName);
						bool flag6 = nameCompare != 0;
						if (flag6)
						{
							result = nameCompare;
						}
						else
						{
							result = x.SkillId.CompareTo(y.SkillId);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600A714 RID: 42772 RVA: 0x004DBCB4 File Offset: 0x004D9EB4
		public override SortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				152,
				153
			};
			return new SortUiConfig
			{
				SortIds = sortIds,
				SortNameIndexList = new List<int>(new int[sortIds.Count]),
				DefaultSortIds = sortIds,
				DefaultSortState = default(SortUiState)
			};
		}

		// Token: 0x0200243C RID: 9276
		public static class SortIds
		{
			// Token: 0x0400E2EE RID: 58094
			public const short Name = 152;

			// Token: 0x0400E2EF RID: 58095
			public const short Seniority = 153;
		}
	}
}
