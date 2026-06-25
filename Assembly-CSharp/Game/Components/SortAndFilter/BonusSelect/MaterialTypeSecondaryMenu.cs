using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Taiwu.Display;

namespace Game.Components.SortAndFilter.BonusSelect
{
	// Token: 0x02000E94 RID: 3732
	public class MaterialTypeSecondaryMenu : DetailedFilterMenuLogic<SkillBreakBonusSelectableItem>
	{
		// Token: 0x1700138A RID: 5002
		// (get) Token: 0x0600AD2C RID: 44332 RVA: 0x004F0E13 File Offset: 0x004EF013
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700138B RID: 5003
		// (get) Token: 0x0600AD2D RID: 44333 RVA: 0x004F0E16 File Offset: 0x004EF016
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AD2E RID: 44334 RVA: 0x004F0E1C File Offset: 0x004EF01C
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600AD2F RID: 44335 RVA: 0x004F0E38 File Offset: 0x004EF038
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			for (int i = 0; i < MaterialTypeSecondaryMenu.MaterialFilterTypes.Length; i++)
			{
				SkillBreakBonusEffectItem effectConfig = SkillBreakBonusEffect.Instance[MaterialTypeSecondaryMenu.MaterialFilterTypes[i]];
				dropdownConfigs.Add(new FilterDropdownItemConfig
				{
					Text = StringKey.CreateDirect(effectConfig.ShortName)
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x0600AD30 RID: 44336 RVA: 0x004F0EA4 File Offset: 0x004EF0A4
		public override bool IsDataMatch(SkillBreakBonusSelectableItem data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = data.Type != EBonusItemType.Material;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (int selectedIndex in selectedIndices)
				{
					bool flag2 = selectedIndex >= 0 && selectedIndex < MaterialTypeSecondaryMenu.MaterialFilterTypes.Length;
					if (flag2)
					{
						bool flag3 = data.BonusData.Effect.TemplateId == MaterialTypeSecondaryMenu.MaterialFilterTypes[selectedIndex];
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

		// Token: 0x040085CF RID: 34255
		public static readonly sbyte[] MaterialFilterTypes = new sbyte[]
		{
			29,
			30,
			31,
			32
		};
	}
}
