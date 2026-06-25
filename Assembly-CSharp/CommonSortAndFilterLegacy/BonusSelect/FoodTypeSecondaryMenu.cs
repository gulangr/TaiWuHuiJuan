using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Taiwu.Display;

namespace CommonSortAndFilterLegacy.BonusSelect
{
	// Token: 0x020005BA RID: 1466
	public class FoodTypeSecondaryMenu : StaticDetailedFilterMenuBase<SkillBreakBonusSelectableItem>
	{
		// Token: 0x170008C5 RID: 2245
		// (get) Token: 0x060045E0 RID: 17888 RVA: 0x0020D60B File Offset: 0x0020B80B
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170008C6 RID: 2246
		// (get) Token: 0x060045E1 RID: 17889 RVA: 0x0020D60E File Offset: 0x0020B80E
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060045E2 RID: 17890 RVA: 0x0020D614 File Offset: 0x0020B814
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x060045E3 RID: 17891 RVA: 0x0020D638 File Offset: 0x0020B838
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < FoodTypeSecondaryMenu.FoodFilterTypes.Length; i++)
			{
				SkillBreakBonusEffectItem effectConfig = SkillBreakBonusEffect.Instance[FoodTypeSecondaryMenu.FoodFilterTypes[i]];
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateDirect(effectConfig.ShortName)
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x060045E4 RID: 17892 RVA: 0x0020D6B0 File Offset: 0x0020B8B0
		public override bool IsDataMatch(SkillBreakBonusSelectableItem data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = data.Type != EBonusItemType.Food;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (int selectedIndex in selectedIndices)
				{
					bool flag2 = selectedIndex >= 0 && selectedIndex < FoodTypeSecondaryMenu.FoodFilterTypes.Length;
					if (flag2)
					{
						bool flag3 = data.BonusData.Effect.TemplateId == FoodTypeSecondaryMenu.FoodFilterTypes[selectedIndex];
						if (flag3)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0400308C RID: 12428
		public static readonly sbyte[] FoodFilterTypes = new sbyte[]
		{
			35,
			36,
			38,
			39
		};
	}
}
