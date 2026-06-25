using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.Components;
using GameData.Domains.CombatSkill;

namespace CommonSortAndFilterLegacy.CombatSkill
{
	// Token: 0x02000579 RID: 1401
	public class CombatSkillStatusMenu : StaticDetailedFilterMenuBase<CombatSkillDisplayData>
	{
		// Token: 0x17000841 RID: 2113
		// (get) Token: 0x0600449E RID: 17566 RVA: 0x0020A358 File Offset: 0x00208558
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x17000842 RID: 2114
		// (get) Token: 0x0600449F RID: 17567 RVA: 0x0020A35B File Offset: 0x0020855B
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060044A0 RID: 17568 RVA: 0x0020A360 File Offset: 0x00208560
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_CombatSKill_2);
		}

		// Token: 0x060044A1 RID: 17569 RVA: 0x0020A384 File Offset: 0x00208584
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				Capacity = this._combatSkillStatusConfigs.Count
			};
			foreach (ValueTuple<CommonSortAndFilterController<CombatSkillDisplayData>.FilterFunction, StringKey> config in this._combatSkillStatusConfigs)
			{
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = config.Item2
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x060044A2 RID: 17570 RVA: 0x0020A41C File Offset: 0x0020861C
		public override bool IsDataMatch(CombatSkillDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = selectedIndices == null || selectedIndices.Count == 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				foreach (int selectionIndex in selectedIndices)
				{
					CommonSortAndFilterController<CombatSkillDisplayData>.FilterFunction filterFunction = this._combatSkillStatusConfigs[selectionIndex].Item1;
					bool flag2 = filterFunction(data);
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060044A3 RID: 17571 RVA: 0x0020A4A4 File Offset: 0x002086A4
		private static bool IsCombatSkillBroken(CombatSkillDisplayData data)
		{
			return CombatSkillStateHelper.IsBrokenOut(data.ActivationState);
		}

		// Token: 0x060044A4 RID: 17572 RVA: 0x0020A4C4 File Offset: 0x002086C4
		private static bool IsCombatSkillNotBroken(CombatSkillDisplayData data)
		{
			return !CombatSkillStateHelper.IsBrokenOut(data.ActivationState);
		}

		// Token: 0x060044A5 RID: 17573 RVA: 0x0020A4E4 File Offset: 0x002086E4
		private static bool IsCombatSkillEquipped(CombatSkillDisplayData data)
		{
			return data.IsInAnyEquipPlans;
		}

		// Token: 0x060044A6 RID: 17574 RVA: 0x0020A4FC File Offset: 0x002086FC
		private static bool IsCombatSkillNotEquipped(CombatSkillDisplayData data)
		{
			return !data.IsInAnyEquipPlans;
		}

		// Token: 0x060044A7 RID: 17575 RVA: 0x0020A518 File Offset: 0x00208718
		private static bool IsCombatSkillFavorite(CombatSkillDisplayData data)
		{
			return data.IsFavorite;
		}

		// Token: 0x060044A8 RID: 17576 RVA: 0x0020A530 File Offset: 0x00208730
		private static bool IsCombatSkillHasEmeiSkillBreakBonus(CombatSkillDisplayData data)
		{
			return data.HasSectEmeiSkillBreakBonus;
		}

		// Token: 0x0400301D RID: 12317
		[TupleElementNames(new string[]
		{
			"filter",
			"label"
		})]
		private readonly List<ValueTuple<CommonSortAndFilterController<CombatSkillDisplayData>.FilterFunction, StringKey>> _combatSkillStatusConfigs = new List<ValueTuple<CommonSortAndFilterController<CombatSkillDisplayData>.FilterFunction, StringKey>>
		{
			new ValueTuple<CommonSortAndFilterController<CombatSkillDisplayData>.FilterFunction, StringKey>(new CommonSortAndFilterController<CombatSkillDisplayData>.FilterFunction(CombatSkillStatusMenu.IsCombatSkillBroken), LanguageKey.LK_CommonSortAndFilter_DetailFilterOption_2_0),
			new ValueTuple<CommonSortAndFilterController<CombatSkillDisplayData>.FilterFunction, StringKey>(new CommonSortAndFilterController<CombatSkillDisplayData>.FilterFunction(CombatSkillStatusMenu.IsCombatSkillNotBroken), LanguageKey.LK_CommonSortAndFilter_DetailFilterOption_2_2),
			new ValueTuple<CommonSortAndFilterController<CombatSkillDisplayData>.FilterFunction, StringKey>(new CommonSortAndFilterController<CombatSkillDisplayData>.FilterFunction(CombatSkillStatusMenu.IsCombatSkillEquipped), LanguageKey.LK_CommonSortAndFilter_DetailFilterOption_2_1),
			new ValueTuple<CommonSortAndFilterController<CombatSkillDisplayData>.FilterFunction, StringKey>(new CommonSortAndFilterController<CombatSkillDisplayData>.FilterFunction(CombatSkillStatusMenu.IsCombatSkillNotEquipped), LanguageKey.LK_CommonSortAndFilter_DetailFilterOption_2_3),
			new ValueTuple<CommonSortAndFilterController<CombatSkillDisplayData>.FilterFunction, StringKey>(new CommonSortAndFilterController<CombatSkillDisplayData>.FilterFunction(CombatSkillStatusMenu.IsCombatSkillFavorite), LanguageKey.LK_CommonSortAndFilter_DetailFilterOption_2_5),
			new ValueTuple<CommonSortAndFilterController<CombatSkillDisplayData>.FilterFunction, StringKey>(new CommonSortAndFilterController<CombatSkillDisplayData>.FilterFunction(CombatSkillStatusMenu.IsCombatSkillHasEmeiSkillBreakBonus), LanguageKey.LK_CommonSortAndFilter_DetailFilterOption_2_4)
		};
	}
}
