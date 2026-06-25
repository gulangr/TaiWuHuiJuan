using System;
using System.Collections.Generic;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item.Apply
{
	// Token: 0x02000DEC RID: 3564
	public class CraftsmanProductSortController : SortController<ITradeableContent>
	{
		// Token: 0x0600AA8A RID: 43658 RVA: 0x004E8824 File Offset: 0x004E6A24
		public override Comparison<ITradeableContent> GenerateComparer(SortStateData sortData)
		{
			return (ITradeableContent x, ITradeableContent y) => this.CompareData(x, y, sortData);
		}

		// Token: 0x0600AA8B RID: 43659 RVA: 0x004E8858 File Offset: 0x004E6A58
		private int CompareData(ITradeableContent x, ITradeableContent y, SortStateData sortData)
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
						bool flag4 = x.ProductionData.CanProduce != y.ProductionData.CanProduce;
						if (flag4)
						{
							result = y.ProductionData.CanProduce.CompareTo(x.ProductionData.CanProduce);
						}
						else
						{
							bool flag5 = ((sortData != null) ? sortData.ItemStates : null) != null;
							if (flag5)
							{
								foreach (SortItemState itemState in sortData.ItemStates)
								{
									short sortId = itemState.SortId;
									ESortDirection order = itemState.SortDirection;
									short num = sortId;
									short num2 = num;
									int comparisonResult;
									if (num2 != 0)
									{
										if (num2 != 169)
										{
											continue;
										}
										comparisonResult = CraftsmanProductSortController.CompareByProductRate(x, y);
									}
									else
									{
										comparisonResult = ItemSortController.CompareByName(x, y);
									}
									bool flag6 = comparisonResult != 0;
									if (flag6)
									{
										return (order == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
									}
								}
							}
							sbyte gradeA = ItemTemplateHelper.GetGrade(x.Key.ItemType, x.Key.TemplateId);
							sbyte gradeB = ItemTemplateHelper.GetGrade(y.Key.ItemType, y.Key.TemplateId);
							bool flag7 = gradeA != gradeB;
							if (flag7)
							{
								result = gradeB.CompareTo(gradeA);
							}
							else
							{
								result = x.Key.Id.CompareTo(y.Key.Id);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600AA8C RID: 43660 RVA: 0x004E8A18 File Offset: 0x004E6C18
		public static int CompareByProductRate(ITradeableContent x, ITradeableContent y)
		{
			return x.ProductionData.Weight.CompareTo(y.ProductionData.Weight);
		}

		// Token: 0x0600AA8D RID: 43661 RVA: 0x004E8A48 File Offset: 0x004E6C48
		public override SortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				0,
				169
			};
			List<int> sortNameIndexList = new List<int>();
			for (int i = 0; i < sortIds.Count; i++)
			{
				sortNameIndexList.Add(0);
			}
			return new SortUiConfig
			{
				SortIds = sortIds,
				SortNameIndexList = sortNameIndexList,
				DefaultSortState = default(SortUiState),
				DefaultSortIds = new List<short>()
			};
		}
	}
}
