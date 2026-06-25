using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;

namespace Game.Components.SortAndFilter.Prison
{
	// Token: 0x02000D15 RID: 3349
	public class PunishmentTypeMenu : DynamicDetailedFilterMenuLogic<CharacterDisplayDataForSettlementPrisoner>
	{
		// Token: 0x17001191 RID: 4497
		// (get) Token: 0x0600A72E RID: 42798 RVA: 0x004DD24F File Offset: 0x004DB44F
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001192 RID: 4498
		// (get) Token: 0x0600A72F RID: 42799 RVA: 0x004DD252 File Offset: 0x004DB452
		public override int Id
		{
			get
			{
				return EMainMenuId.PunishmentType.ToInt();
			}
		}

		// Token: 0x0600A730 RID: 42800 RVA: 0x004DD260 File Offset: 0x004DB460
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Prison_1;
		}

		// Token: 0x0600A731 RID: 42801 RVA: 0x004DD27C File Offset: 0x004DB47C
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return null;
		}

		// Token: 0x0600A732 RID: 42802 RVA: 0x004DD290 File Offset: 0x004DB490
		public override List<FilterDropdownItemConfig> GetDynamicMenuConfigs(IEnumerable<CharacterDisplayDataForSettlementPrisoner> dataList)
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
			select new FilterDropdownItemConfig
			{
				Text = StringKey.CreateDirect(PunishmentType.Instance[type].ShortName)
			}).ToList<FilterDropdownItemConfig>();
		}

		// Token: 0x0600A733 RID: 42803 RVA: 0x004DD348 File Offset: 0x004DB548
		public override bool IsDataMatch(CharacterDisplayDataForSettlementPrisoner data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => this._punishmentTypes[index] == data.SettlementPrisoner.PunishmentType);
		}

		// Token: 0x04008358 RID: 33624
		private readonly List<short> _punishmentTypes = new List<short>();
	}
}
