using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.Components;
using GameData.Domains.Character.Display;

namespace CommonSortAndFilterLegacy.Bounty
{
	// Token: 0x020005AC RID: 1452
	public class LocationMenu : DynamicDetailedFilterMenuBase<CharacterDisplayDataForSettlementBounty>
	{
		// Token: 0x170008AF RID: 2223
		// (get) Token: 0x0600459A RID: 17818 RVA: 0x0020CA3B File Offset: 0x0020AC3B
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x0600459B RID: 17819 RVA: 0x0020CA40 File Offset: 0x0020AC40
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Bounty_3));
		}

		// Token: 0x0600459C RID: 17820 RVA: 0x0020CA64 File Offset: 0x0020AC64
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetDynamicMenuConfigs(List<CharacterDisplayDataForSettlementBounty> dataList)
		{
			HashSet<short> areaIdSet = new HashSet<short>();
			foreach (CharacterDisplayDataForSettlementBounty data in dataList)
			{
				areaIdSet.Add(data.Location.AreaId);
			}
			this._areaIds.Clear();
			this._areaIds.AddRange(areaIdSet);
			this._areaIds.Sort();
			return (from areaId in this._areaIds
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = null,
				Text = LocationMenu.GetAreaName(areaId)
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x0600459D RID: 17821 RVA: 0x0020CB20 File Offset: 0x0020AD20
		public override bool IsDataMatch(CharacterDisplayDataForSettlementBounty data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => this._areaIds[index] == data.Location.AreaId);
		}

		// Token: 0x0600459E RID: 17822 RVA: 0x0020CB58 File Offset: 0x0020AD58
		private static StringKey GetAreaName(short areaId)
		{
			bool flag = areaId < 0;
			StringKey result;
			if (flag)
			{
				result = StringKey.CreateKey(LanguageKey.LK_Unknown_Area_Name);
			}
			else
			{
				WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
				result = StringKey.CreateDirect(worldMapModel.GetAreaName(areaId));
			}
			return result;
		}

		// Token: 0x170008B0 RID: 2224
		// (get) Token: 0x0600459F RID: 17823 RVA: 0x0020CB91 File Offset: 0x0020AD91
		public override int Id
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x04003075 RID: 12405
		private readonly List<short> _areaIds = new List<short>();
	}
}
