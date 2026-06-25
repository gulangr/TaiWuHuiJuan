using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Character.Display;

namespace CommonSortAndFilterLegacy.Prison
{
	// Token: 0x02000475 RID: 1141
	public class PunishmentTypeMenu : DynamicDetailedFilterMenuBase<CharacterDisplayDataForSettlementPrisoner>
	{
		// Token: 0x170006D8 RID: 1752
		// (get) Token: 0x060040DB RID: 16603 RVA: 0x00200528 File Offset: 0x001FE728
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060040DC RID: 16604 RVA: 0x0020052C File Offset: 0x001FE72C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Prison_1));
		}

		// Token: 0x060040DD RID: 16605 RVA: 0x00200550 File Offset: 0x001FE750
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetDynamicMenuConfigs(List<CharacterDisplayDataForSettlementPrisoner> dataList)
		{
			HashSet<short> typeSet = new HashSet<short>();
			foreach (CharacterDisplayDataForSettlementPrisoner prisoner in dataList)
			{
				typeSet.Add(prisoner.SettlementPrisoner.PunishmentType);
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

		// Token: 0x060040DE RID: 16606 RVA: 0x0020060C File Offset: 0x001FE80C
		public override bool IsDataMatch(CharacterDisplayDataForSettlementPrisoner data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => this._punishmentTypes[index] == data.SettlementPrisoner.PunishmentType);
		}

		// Token: 0x170006D9 RID: 1753
		// (get) Token: 0x060040DF RID: 16607 RVA: 0x00200644 File Offset: 0x001FE844
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x04002E3A RID: 11834
		private readonly List<short> _punishmentTypes = new List<short>();
	}
}
