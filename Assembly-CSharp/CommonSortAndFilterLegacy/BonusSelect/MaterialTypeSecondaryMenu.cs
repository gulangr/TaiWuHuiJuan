using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Taiwu.Display;

namespace CommonSortAndFilterLegacy.BonusSelect
{
	// Token: 0x020005BD RID: 1469
	public class MaterialTypeSecondaryMenu : StaticDetailedFilterMenuBase<SkillBreakBonusSelectableItem>
	{
		// Token: 0x170008CB RID: 2251
		// (get) Token: 0x060045F2 RID: 17906 RVA: 0x0020D8EB File Offset: 0x0020BAEB
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170008CC RID: 2252
		// (get) Token: 0x060045F3 RID: 17907 RVA: 0x0020D8EE File Offset: 0x0020BAEE
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060045F4 RID: 17908 RVA: 0x0020D8F4 File Offset: 0x0020BAF4
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x060045F5 RID: 17909 RVA: 0x0020D918 File Offset: 0x0020BB18
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < MaterialTypeSecondaryMenu.MaterialFilterTypes.Length; i++)
			{
				SkillBreakBonusEffectItem effectConfig = SkillBreakBonusEffect.Instance[MaterialTypeSecondaryMenu.MaterialFilterTypes[i]];
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateDirect(effectConfig.ShortName)
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x060045F6 RID: 17910 RVA: 0x0020D990 File Offset: 0x0020BB90
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

		// Token: 0x0400308D RID: 12429
		public static readonly sbyte[] MaterialFilterTypes = new sbyte[]
		{
			29,
			30,
			31,
			32
		};
	}
}
