using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Taiwu.Display;

namespace Game.Components.SortAndFilter.BonusSelect
{
	// Token: 0x02000E8E RID: 3726
	public class BookTypeSecondaryMenu : DetailedFilterMenuLogic<SkillBreakBonusSelectableItem>
	{
		// Token: 0x17001380 RID: 4992
		// (get) Token: 0x0600AD0E RID: 44302 RVA: 0x004F0933 File Offset: 0x004EEB33
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001381 RID: 4993
		// (get) Token: 0x0600AD0F RID: 44303 RVA: 0x004F0936 File Offset: 0x004EEB36
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AD10 RID: 44304 RVA: 0x004F093C File Offset: 0x004EEB3C
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600AD11 RID: 44305 RVA: 0x004F0958 File Offset: 0x004EEB58
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			for (int i = 0; i < BookTypeSecondaryMenu.LifeSkillBookFilterTypes.Length; i++)
			{
				SkillBreakBonusEffectItem effectConfig = SkillBreakBonusEffect.Instance[BookTypeSecondaryMenu.LifeSkillBookFilterTypes[i]];
				dropdownConfigs.Add(new FilterDropdownItemConfig
				{
					Text = StringKey.CreateDirect(effectConfig.ShortName)
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x0600AD12 RID: 44306 RVA: 0x004F09C4 File Offset: 0x004EEBC4
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

		// Token: 0x040085C3 RID: 34243
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
