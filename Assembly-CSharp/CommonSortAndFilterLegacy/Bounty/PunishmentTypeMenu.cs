using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Character.Display;

namespace CommonSortAndFilterLegacy.Bounty
{
	// Token: 0x020005B0 RID: 1456
	public class PunishmentTypeMenu : DynamicDetailedFilterMenuBase<CharacterDisplayDataForSettlementBounty>
	{
		// Token: 0x170008B6 RID: 2230
		// (get) Token: 0x060045AD RID: 17837 RVA: 0x0020CD13 File Offset: 0x0020AF13
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060045AE RID: 17838 RVA: 0x0020CD18 File Offset: 0x0020AF18
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Bounty_0));
		}

		// Token: 0x060045AF RID: 17839 RVA: 0x0020CD3C File Offset: 0x0020AF3C
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetDynamicMenuConfigs(List<CharacterDisplayDataForSettlementBounty> dataList)
		{
			HashSet<short> typeSet = new HashSet<short>();
			foreach (CharacterDisplayDataForSettlementBounty data in dataList)
			{
				typeSet.Add(data.SettlementBounty.PunishmentType);
			}
			this._punishmentTypes.Clear();
			this._punishmentTypes.AddRange(typeSet);
			this._punishmentTypes.Sort();
			return (from type in this._punishmentTypes
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = null,
				Text = StringKey.CreateDirect(PunishmentType.Instance[type].ShortName)
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x060045B0 RID: 17840 RVA: 0x0020CDF8 File Offset: 0x0020AFF8
		public override bool IsDataMatch(CharacterDisplayDataForSettlementBounty data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => this._punishmentTypes[index] == data.SettlementBounty.PunishmentType);
		}

		// Token: 0x170008B7 RID: 2231
		// (get) Token: 0x060045B1 RID: 17841 RVA: 0x0020CE30 File Offset: 0x0020B030
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0400307D RID: 12413
		private readonly List<short> _punishmentTypes = new List<short>();
	}
}
