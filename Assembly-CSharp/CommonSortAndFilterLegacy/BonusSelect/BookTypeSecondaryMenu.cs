using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Taiwu.Display;

namespace CommonSortAndFilterLegacy.BonusSelect
{
	// Token: 0x020005B7 RID: 1463
	public class BookTypeSecondaryMenu : StaticDetailedFilterMenuBase<SkillBreakBonusSelectableItem>
	{
		// Token: 0x170008C1 RID: 2241
		// (get) Token: 0x060045D4 RID: 17876 RVA: 0x0020D45F File Offset: 0x0020B65F
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170008C2 RID: 2242
		// (get) Token: 0x060045D5 RID: 17877 RVA: 0x0020D462 File Offset: 0x0020B662
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060045D6 RID: 17878 RVA: 0x0020D468 File Offset: 0x0020B668
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x060045D7 RID: 17879 RVA: 0x0020D48C File Offset: 0x0020B68C
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < BookTypeSecondaryMenu.LifeSkillBookFilterTypes.Length; i++)
			{
				SkillBreakBonusEffectItem effectConfig = SkillBreakBonusEffect.Instance[BookTypeSecondaryMenu.LifeSkillBookFilterTypes[i]];
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateDirect(effectConfig.ShortName)
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x060045D8 RID: 17880 RVA: 0x0020D504 File Offset: 0x0020B704
		public override bool IsDataMatch(SkillBreakBonusSelectableItem data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = data.Type != EBonusItemType.Book;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (int selectedIndex in selectedIndices)
				{
					bool flag2 = selectedIndex >= 0 && selectedIndex < BookTypeSecondaryMenu.LifeSkillBookFilterTypes.Length;
					if (flag2)
					{
						bool flag3 = data.BonusData.Effect.TemplateId == BookTypeSecondaryMenu.LifeSkillBookFilterTypes[selectedIndex];
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

		// Token: 0x04003081 RID: 12417
		public static readonly sbyte[] LifeSkillBookFilterTypes = new sbyte[]
		{
			0,
			1,
			2,
			3,
			10,
			11,
			6,
			7,
			4,
			5,
			8,
			9,
			15,
			14,
			12,
			13
		};
	}
}
