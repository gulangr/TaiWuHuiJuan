using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Taiwu.Display;

namespace CommonSortAndFilterLegacy.BonusSelect
{
	// Token: 0x020005BF RID: 1471
	public class MedicineTypeSecondaryMenu : StaticDetailedFilterMenuBase<SkillBreakBonusSelectableItem>
	{
		// Token: 0x170008CF RID: 2255
		// (get) Token: 0x060045FE RID: 17918 RVA: 0x0020DA97 File Offset: 0x0020BC97
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170008D0 RID: 2256
		// (get) Token: 0x060045FF RID: 17919 RVA: 0x0020DA9A File Offset: 0x0020BC9A
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06004600 RID: 17920 RVA: 0x0020DAA0 File Offset: 0x0020BCA0
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x06004601 RID: 17921 RVA: 0x0020DAC4 File Offset: 0x0020BCC4
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < MedicineTypeSecondaryMenu.MedicineFilterTypes.Length; i++)
			{
				SkillBreakBonusEffectItem effectConfig = SkillBreakBonusEffect.Instance[MedicineTypeSecondaryMenu.MedicineFilterTypes[i]];
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateDirect(effectConfig.ShortName)
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x06004602 RID: 17922 RVA: 0x0020DB3C File Offset: 0x0020BD3C
		public override bool IsDataMatch(SkillBreakBonusSelectableItem data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = data.Type != EBonusItemType.Medicine;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (int selectedIndex in selectedIndices)
				{
					bool flag2 = selectedIndex >= 0 && selectedIndex < MedicineTypeSecondaryMenu.MedicineFilterTypes.Length;
					if (flag2)
					{
						sbyte defKey = MedicineTypeSecondaryMenu.MedicineFilterTypes[selectedIndex];
						bool flag3 = defKey == 16;
						if (flag3)
						{
							sbyte templateId = data.BonusData.Effect.TemplateId;
							bool flag4 = templateId == 16 || templateId == 17;
							if (flag4)
							{
								return true;
							}
						}
						else
						{
							bool flag5 = defKey == 41;
							if (flag5)
							{
								bool flag6 = MedicineTypeSecondaryMenu.PoisonGroup.Contains(data.BonusData.Effect.TemplateId);
								if (flag6)
								{
									return true;
								}
							}
							else
							{
								bool flag7 = data.BonusData.Effect.TemplateId == defKey;
								if (flag7)
								{
									return true;
								}
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0400308E RID: 12430
		public static readonly sbyte[] MedicineFilterTypes = new sbyte[]
		{
			16,
			18,
			19,
			20,
			21,
			22,
			23,
			24,
			25,
			26,
			27,
			28,
			41
		};

		// Token: 0x0400308F RID: 12431
		public static readonly HashSet<sbyte> PoisonGroup = new HashSet<sbyte>
		{
			41,
			42,
			43,
			44,
			45,
			46
		};
	}
}
