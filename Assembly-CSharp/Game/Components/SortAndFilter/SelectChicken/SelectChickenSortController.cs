using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Building;

namespace Game.Components.SortAndFilter.SelectChicken
{
	// Token: 0x02000CDB RID: 3291
	public class SelectChickenSortController : SortController<GameData.Domains.Building.Chicken>
	{
		// Token: 0x0600A634 RID: 42548 RVA: 0x004D61CC File Offset: 0x004D43CC
		public override Comparison<GameData.Domains.Building.Chicken> GenerateComparer(SortStateData sortData)
		{
			return (GameData.Domains.Building.Chicken x, GameData.Domains.Building.Chicken y) => this.CompareData(x, y, sortData);
		}

		// Token: 0x0600A635 RID: 42549 RVA: 0x004D6200 File Offset: 0x004D4400
		private int CompareData(GameData.Domains.Building.Chicken x, GameData.Domains.Building.Chicken y, SortStateData sortData)
		{
			bool flag = ((sortData != null) ? sortData.ItemStates : null) != null;
			if (flag)
			{
				foreach (SortItemState itemState in sortData.ItemStates)
				{
					short sortId = itemState.SortId;
					ESortDirection order = itemState.SortDirection;
					int comparisonResult = this.CompareBySortId(x, y, sortId);
					bool flag2 = comparisonResult != 0;
					if (flag2)
					{
						return (order == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
					}
				}
			}
			return x.Id.CompareTo(y.Id);
		}

		// Token: 0x0600A636 RID: 42550 RVA: 0x004D62B4 File Offset: 0x004D44B4
		private int CompareBySortId(GameData.Domains.Building.Chicken x, GameData.Domains.Building.Chicken y, short sortId)
		{
			ChickenItem configX = Config.Chicken.Instance[x.TemplateId];
			ChickenItem configY = Config.Chicken.Instance[y.TemplateId];
			bool flag = configX == null && configY == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = configX == null;
				if (flag2)
				{
					result = 1;
				}
				else
				{
					bool flag3 = configY == null;
					if (flag3)
					{
						result = -1;
					}
					else
					{
						bool flag4 = sortId == 150;
						if (flag4)
						{
							result = configX.PersonalityValue.CompareTo(configY.PersonalityValue);
						}
						else
						{
							result = x.Id.CompareTo(y.Id);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600A637 RID: 42551 RVA: 0x004D634C File Offset: 0x004D454C
		public override SortUiConfig GenerateConfig()
		{
			return new SortUiConfig
			{
				SortIds = new List<short>
				{
					150
				},
				SortNameIndexList = new List<int>
				{
					0
				},
				DefaultSortIds = new List<short>
				{
					150
				},
				DefaultSortState = new SortUiState
				{
					ItemStates = new List<SortItemState>
					{
						new SortItemState
						{
							SortId = 150,
							SortDirection = ESortDirection.Ascending
						}
					}
				}
			};
		}

		// Token: 0x02002421 RID: 9249
		public static class SortIds
		{
			// Token: 0x0400E1CD RID: 57805
			public const short Personality = 150;
		}
	}
}
