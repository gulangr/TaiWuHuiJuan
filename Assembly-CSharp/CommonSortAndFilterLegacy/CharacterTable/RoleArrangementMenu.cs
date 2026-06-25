using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.Components;
using GameData.Domains.Character.Display;

namespace CommonSortAndFilterLegacy.CharacterTable
{
	// Token: 0x0200058E RID: 1422
	public class RoleArrangementMenu : StaticDetailedFilterMenuBase<CharacterTableSortAndFilterData>
	{
		// Token: 0x1700085A RID: 2138
		// (get) Token: 0x060044ED RID: 17645 RVA: 0x0020B0F8 File Offset: 0x002092F8
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060044EE RID: 17646 RVA: 0x0020B0FC File Offset: 0x002092FC
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig
			{
				MenuBarLabel = StringKey.CreateKey(LanguageKey.LK_CommonSortAndFilter_DetailFilterMenu_Villager_6)
			};
		}

		// Token: 0x060044EF RID: 17647 RVA: 0x0020B128 File Offset: 0x00209328
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> configs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < Enum.GetValues(typeof(EArrangementMenuOption)).Length; i++)
			{
				configs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Villager_Third_RoleArrangement_{0}", i))
				});
			}
			return configs;
		}

		// Token: 0x060044F0 RID: 17648 RVA: 0x0020B19C File Offset: 0x0020939C
		public override bool IsDataMatch(CharacterTableSortAndFilterData data, IReadOnlyCollection<int> selectedIndices)
		{
			return selectedIndices.Any((int index) => RoleArrangementMenu.IsArrangementTypeMatch(data, (EArrangementMenuOption)index));
		}

		// Token: 0x060044F1 RID: 17649 RVA: 0x0020B1D0 File Offset: 0x002093D0
		private static bool IsArrangementTypeMatch(CharacterTableSortAndFilterData data, EArrangementMenuOption arrangementMenuOption)
		{
			CharacterTableWorkData workData = data.Data.GetWork(99);
			if (!true)
			{
			}
			bool result;
			switch (arrangementMenuOption)
			{
			case EArrangementMenuOption.Collect:
				result = (workData.WorkType == 10);
				break;
			case EArrangementMenuOption.Migrate:
				result = (workData.WorkType == 14);
				break;
			case EArrangementMenuOption.Healing:
				result = (workData.ArrangementTemplateId == 6);
				break;
			case EArrangementMenuOption.Sell:
				result = (workData.ArrangementTemplateId == 8 && workData.BoolValue);
				break;
			case EArrangementMenuOption.Buy:
				result = (workData.ArrangementTemplateId == 8 && !workData.BoolValue);
				break;
			case EArrangementMenuOption.Entertain:
				result = (workData.ArrangementTemplateId == 11);
				break;
			case EArrangementMenuOption.GuardSwordTomb:
				result = (workData.ArrangementTemplateId == 13);
				break;
			case EArrangementMenuOption.Envoy:
				result = (workData.ArrangementTemplateId == 15);
				break;
			default:
				result = false;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x1700085B RID: 2139
		// (get) Token: 0x060044F2 RID: 17650 RVA: 0x0020B29F File Offset: 0x0020949F
		public override int Id
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x1700085C RID: 2140
		// (get) Token: 0x060044F3 RID: 17651 RVA: 0x0020B2A2 File Offset: 0x002094A2
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(5, 2));
			}
		}
	}
}
