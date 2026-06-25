using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Taiwu.Display;

namespace CommonSortAndFilterLegacy.BonusSelect
{
	// Token: 0x020005B3 RID: 1459
	public class BloodDewTypeSecondaryMenu : StaticDetailedFilterMenuBase<SkillBreakBonusSelectableItem>
	{
		// Token: 0x170008BC RID: 2236
		// (get) Token: 0x060045BE RID: 17854 RVA: 0x0020CF6F File Offset: 0x0020B16F
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170008BD RID: 2237
		// (get) Token: 0x060045BF RID: 17855 RVA: 0x0020CF72 File Offset: 0x0020B172
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060045C0 RID: 17856 RVA: 0x0020CF78 File Offset: 0x0020B178
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x060045C1 RID: 17857 RVA: 0x0020CF9C File Offset: 0x0020B19C
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < BloodDewTypeSecondaryMenu.BloodDewFilterTypes.Length; i++)
			{
				SkillBreakBonusEffectItem effectConfig = SkillBreakBonusEffect.Instance[BloodDewTypeSecondaryMenu.BloodDewFilterTypes[i]];
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateDirect(effectConfig.ShortName)
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x060045C2 RID: 17858 RVA: 0x0020D014 File Offset: 0x0020B214
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

		// Token: 0x0400307F RID: 12415
		public static readonly sbyte[] BloodDewFilterTypes = new sbyte[]
		{
			40
		};
	}
}
