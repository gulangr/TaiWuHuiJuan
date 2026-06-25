using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Character.Display;

namespace CommonSortAndFilterLegacy.Bounty
{
	// Token: 0x020005AA RID: 1450
	public class BountySortController : CommonSortController<CharacterDisplayDataForSettlementBounty>
	{
		// Token: 0x06004596 RID: 17814 RVA: 0x0020C750 File Offset: 0x0020A950
		public override void Sort(List<CharacterDisplayDataForSettlementBounty> dataList, SortStateData sortData, Action actionAfterSort)
		{
			dataList.Sort((CharacterDisplayDataForSettlementBounty x, CharacterDisplayDataForSettlementBounty y) => this.CompareData(x, y, sortData));
			if (actionAfterSort != null)
			{
				actionAfterSort();
			}
		}

		// Token: 0x06004597 RID: 17815 RVA: 0x0020C794 File Offset: 0x0020A994
		private int CompareData(CharacterDisplayDataForSettlementBounty x, CharacterDisplayDataForSettlementBounty y, SortStateData sortData)
		{
			float xHealth = (float)CommonUtils.GetCharacterHealthInfo(x.Health, x.LeftMaxHealth, x.SettlementBounty.CharId).Item3;
			float yHealth = (float)CommonUtils.GetCharacterHealthInfo(y.Health, y.LeftMaxHealth, y.SettlementBounty.CharId).Item3;
			string xName = NameCenter.GetMonasticTitleOrDisplayName(ref x.NameRelatedData, false, false);
			string yName = NameCenter.GetMonasticTitleOrDisplayName(ref y.NameRelatedData, false, false);
			foreach (SortItemState itemState in sortData.ItemStates)
			{
				short sortId = itemState.SortId;
				ESortDirection order = itemState.SortDirection;
				short num = sortId;
				short num2 = num;
				int comparisonResult;
				if (num2 != 0)
				{
					if (num2 != 1)
					{
						switch (num2)
						{
						case 10:
							comparisonResult = xHealth.CompareTo(yHealth);
							goto IL_178;
						case 11:
							comparisonResult = x.FavorabilityToTaiwu.CompareTo(y.FavorabilityToTaiwu);
							goto IL_178;
						case 12:
							comparisonResult = x.Happiness.CompareTo(y.Happiness);
							goto IL_178;
						case 15:
							comparisonResult = x.SettlementBounty.PunishmentSeverity.CompareTo(y.SettlementBounty.PunishmentSeverity);
							goto IL_178;
						case 16:
							comparisonResult = x.SettlementBounty.BountyAmount.CompareTo(y.SettlementBounty.BountyAmount);
							goto IL_178;
						}
						continue;
					}
					comparisonResult = x.OrgInfo.Grade.CompareTo(y.OrgInfo.Grade);
				}
				else
				{
					comparisonResult = Utils_Sorting.CompareByCurrentLangEncoding(xName, yName);
				}
				IL_178:
				bool flag = comparisonResult != 0;
				if (flag)
				{
					return (order == ESortDirection.Ascending) ? comparisonResult : (-comparisonResult);
				}
			}
			return 0;
		}

		// Token: 0x06004598 RID: 17816 RVA: 0x0020C978 File Offset: 0x0020AB78
		public override CommonSortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				15,
				16,
				1,
				0,
				10,
				12,
				11
			};
			return new CommonSortUiConfig
			{
				SortIds = sortIds,
				SortNameIndexList = new List<int>
				{
					0,
					0,
					1,
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
