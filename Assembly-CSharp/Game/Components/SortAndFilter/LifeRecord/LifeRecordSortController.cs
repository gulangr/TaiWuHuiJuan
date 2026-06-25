using System;
using GameData.Domains.LifeRecord;

namespace Game.Components.SortAndFilter.LifeRecord
{
	// Token: 0x02000D1C RID: 3356
	public class LifeRecordSortController : SortController<TransferableRecord>
	{
		// Token: 0x0600A74A RID: 42826 RVA: 0x004DD820 File Offset: 0x004DBA20
		public override Comparison<TransferableRecord> GenerateComparer(SortStateData sortData)
		{
			return (TransferableRecord x, TransferableRecord y) => this.CompareData(x, y, sortData);
		}

		// Token: 0x0600A74B RID: 42827 RVA: 0x004DD854 File Offset: 0x004DBA54
		private int CompareData(TransferableRecord x, TransferableRecord y, SortStateData sortData)
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
						result = x.Date.CompareTo(y.Date);
					}
				}
			}
			return result;
		}

		// Token: 0x0600A74C RID: 42828 RVA: 0x004DD940 File Offset: 0x004DBB40
		private int CompareBySortId(TransferableRecord x, TransferableRecord y, short sortId)
		{
			return this.CompareBySortId(x, y, sortId);
		}

		// Token: 0x0600A74D RID: 42829 RVA: 0x004DD95C File Offset: 0x004DBB5C
		public override SortUiConfig GenerateConfig()
		{
			return new SortUiConfig
			{
				DefaultSortState = default(SortUiState)
			};
		}
	}
}
