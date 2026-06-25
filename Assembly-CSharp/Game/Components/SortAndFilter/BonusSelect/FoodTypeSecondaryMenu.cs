using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Taiwu.Display;

namespace Game.Components.SortAndFilter.BonusSelect
{
	// Token: 0x02000E91 RID: 3729
	public class FoodTypeSecondaryMenu : DetailedFilterMenuLogic<SkillBreakBonusSelectableItem>
	{
		// Token: 0x17001384 RID: 4996
		// (get) Token: 0x0600AD1A RID: 44314 RVA: 0x004F0ACB File Offset: 0x004EECCB
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001385 RID: 4997
		// (get) Token: 0x0600AD1B RID: 44315 RVA: 0x004F0ACE File Offset: 0x004EECCE
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AD1C RID: 44316 RVA: 0x004F0AD4 File Offset: 0x004EECD4
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600AD1D RID: 44317 RVA: 0x004F0AF0 File Offset: 0x004EECF0
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			for (int i = 0; i < FoodTypeSecondaryMenu.FoodFilterTypes.Length; i++)
			{
				SkillBreakBonusEffectItem effectConfig = SkillBreakBonusEffect.Instance[FoodTypeSecondaryMenu.FoodFilterTypes[i]];
				dropdownConfigs.Add(new FilterDropdownItemConfig
				{
					Text = StringKey.CreateDirect(effectConfig.ShortName)
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x0600AD1E RID: 44318 RVA: 0x004F0B5C File Offset: 0x004EED5C
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

		// Token: 0x040085CE RID: 34254
		public static readonly sbyte[] FoodFilterTypes = new sbyte[]
		{
			35,
			36,
			38,
			39
		};
	}
}
