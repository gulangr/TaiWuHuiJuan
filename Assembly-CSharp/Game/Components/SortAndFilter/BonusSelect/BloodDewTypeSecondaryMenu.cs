using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Taiwu.Display;

namespace Game.Components.SortAndFilter.BonusSelect
{
	// Token: 0x02000E8A RID: 3722
	public class BloodDewTypeSecondaryMenu : DetailedFilterMenuLogic<SkillBreakBonusSelectableItem>
	{
		// Token: 0x1700137C RID: 4988
		// (get) Token: 0x0600ACF5 RID: 44277 RVA: 0x004EFDF3 File Offset: 0x004EDFF3
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700137D RID: 4989
		// (get) Token: 0x0600ACF6 RID: 44278 RVA: 0x004EFDF6 File Offset: 0x004EDFF6
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600ACF7 RID: 44279 RVA: 0x004EFDFC File Offset: 0x004EDFFC
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600ACF8 RID: 44280 RVA: 0x004EFE18 File Offset: 0x004EE018
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			for (int i = 0; i < BloodDewTypeSecondaryMenu.BloodDewFilterTypes.Length; i++)
			{
				SkillBreakBonusEffectItem effectConfig = SkillBreakBonusEffect.Instance[BloodDewTypeSecondaryMenu.BloodDewFilterTypes[i]];
				dropdownConfigs.Add(new FilterDropdownItemConfig
				{
					Text = StringKey.CreateDirect(effectConfig.ShortName)
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x0600ACF9 RID: 44281 RVA: 0x004EFE84 File Offset: 0x004EE084
		public override bool IsDataMatch(SkillBreakBonusSelectableItem data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = data.Type != EBonusItemType.BloodDew;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (int selectedIndex in selectedIndices)
				{
					bool flag2 = selectedIndex >= 0 && selectedIndex < BloodDewTypeSecondaryMenu.BloodDewFilterTypes.Length;
					if (flag2)
					{
						bool flag3 = data.BonusData.Effect.TemplateId == BloodDewTypeSecondaryMenu.BloodDewFilterTypes[selectedIndex];
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

		// Token: 0x040085C1 RID: 34241
		public static readonly sbyte[] BloodDewFilterTypes = new sbyte[]
		{
			40
		};
	}
}
