using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Character.Display;

namespace CommonSortAndFilterLegacy.Bounty
{
	// Token: 0x020005AF RID: 1455
	public class PunishmentSeverityMenu : DynamicDetailedFilterMenuBase<CharacterDisplayDataForSettlementBounty>
	{
		// Token: 0x170008B4 RID: 2228
		// (get) Token: 0x060045A7 RID: 17831 RVA: 0x0020CBE0 File Offset: 0x0020ADE0
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060045A8 RID: 17832 RVA: 0x0020CBE4 File Offset: 0x0020ADE4
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Bounty_1));
		}

		// Token: 0x060045A9 RID: 17833 RVA: 0x0020CC08 File Offset: 0x0020AE08
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetDynamicMenuConfigs(List<CharacterDisplayDataForSettlementBounty> dataList)
		{
			HashSet<sbyte> severitySet = new HashSet<sbyte>();
			foreach (CharacterDisplayDataForSettlementBounty data in dataList)
			{
				severitySet.Add(data.SettlementBounty.PunishmentSeverity);
			}
			this._punishmentSeverities.Clear();
			this._punishmentSeverities.AddRange(severitySet);
			this._punishmentSeverities.Sort();
			return (from type in this._punishmentSeverities
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = null,
				Text = StringKey.CreateDirect(PunishmentSeverity.Instance[type].Name)
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x060045AA RID: 17834 RVA: 0x0020CCC4 File Offset: 0x0020AEC4
		public override bool IsDataMatch(CharacterDisplayDataForSettlementBounty data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => this._punishmentSeverities[index] == data.SettlementBounty.PunishmentSeverity);
		}

		// Token: 0x170008B5 RID: 2229
		// (get) Token: 0x060045AB RID: 17835 RVA: 0x0020CCFC File Offset: 0x0020AEFC
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0400307C RID: 12412
		private readonly List<sbyte> _punishmentSeverities = new List<sbyte>();
	}
}
