using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.Components;
using GameData.Domains.Character.Display;

namespace CommonSortAndFilterLegacy.Bounty
{
	// Token: 0x020005B1 RID: 1457
	public class StatusMenu : StaticDetailedFilterMenuBase<CharacterDisplayDataForSettlementBounty>
	{
		// Token: 0x170008B8 RID: 2232
		// (get) Token: 0x060045B3 RID: 17843 RVA: 0x0020CE47 File Offset: 0x0020B047
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060045B4 RID: 17844 RVA: 0x0020CE4C File Offset: 0x0020B04C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Bounty_2));
		}

		// Token: 0x060045B5 RID: 17845 RVA: 0x0020CE70 File Offset: 0x0020B070
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return (from status in this._statusList
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = null,
				Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Bounty_Third_Status_{0}", status))
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x060045B6 RID: 17846 RVA: 0x0020CEB4 File Offset: 0x0020B0B4
		public override bool IsDataMatch(CharacterDisplayDataForSettlementBounty data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => this._statusList[index] == data.HunterState);
		}

		// Token: 0x170008B9 RID: 2233
		// (get) Token: 0x060045B7 RID: 17847 RVA: 0x0020CEEC File Offset: 0x0020B0EC
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0400307E RID: 12414
		private readonly List<sbyte> _statusList = new List<sbyte>
		{
			0,
			1,
			2,
			3
		};
	}
}
