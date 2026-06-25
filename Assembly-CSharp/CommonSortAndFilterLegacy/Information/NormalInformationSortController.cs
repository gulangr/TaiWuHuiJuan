using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Information;

namespace CommonSortAndFilterLegacy.Information
{
	// Token: 0x02000476 RID: 1142
	public class NormalInformationSortController : CommonSortController<NormalInformationDisplayData>
	{
		// Token: 0x060040E1 RID: 16609 RVA: 0x0020065C File Offset: 0x001FE85C
		public override void Sort(List<NormalInformationDisplayData> dataList, SortStateData sortData, Action actionAfterSort)
		{
			dataList.Sort((NormalInformationDisplayData x, NormalInformationDisplayData y) => this.CompareData(x, y, sortData));
			if (actionAfterSort != null)
			{
				actionAfterSort();
			}
		}

		// Token: 0x060040E2 RID: 16610 RVA: 0x002006A0 File Offset: 0x001FE8A0
		private int CompareData(NormalInformationDisplayData x, NormalInformationDisplayData y, SortStateData sortData)
		{
			InformationItem xConfig = Information.Instance[x.NormalInformation.TemplateId];
			InformationItem yConfig = Information.Instance[y.NormalInformation.TemplateId];
			InformationInfoItem xInfoConfig = InformationInfo.Instance[xConfig.InfoIds[(int)x.NormalInformation.Level]];
			InformationInfoItem yInfoConfig = InformationInfo.Instance[yConfig.InfoIds[(int)y.NormalInformation.Level]];
			foreach (SortItemState itemState in sortData.ItemStates)
			{
				short sortId = itemState.SortId;
				ESortDirection order = itemState.SortDirection;
				short num = sortId;
				short num2 = num;
				int comparisonResult;
				if (num2 != 1)
				{
					if (num2 != 7)
					{
						continue;
					}
					comparisonResult = (x.MaxCount - x.UsedCount).CompareTo(y.MaxCount - y.UsedCount);
				}
				else
				{
					comparisonResult = xInfoConfig.Grade.CompareTo(yInfoConfig.Grade);
				}
				bool flag = comparisonResult != 0;
				if (flag)
				{
					return (order == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
				}
			}
			return 0;
		}

		// Token: 0x060040E3 RID: 16611 RVA: 0x002007E8 File Offset: 0x001FE9E8
		public override CommonSortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				1,
				7
			};
			List<int> sortNameIndexList = new List<int>
			{
				0,
				0
			};
			return new CommonSortUiConfig
			{
				SortIds = sortIds,
				SortNameIndexList = sortNameIndexList,
				DefaultSortState = null,
				DefaultSortIds = sortIds
			};
		}
	}
}
