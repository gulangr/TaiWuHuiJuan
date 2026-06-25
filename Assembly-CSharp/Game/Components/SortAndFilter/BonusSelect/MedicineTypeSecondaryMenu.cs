using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Taiwu.Display;

namespace Game.Components.SortAndFilter.BonusSelect
{
	// Token: 0x02000E96 RID: 3734
	public class MedicineTypeSecondaryMenu : DetailedFilterMenuLogic<SkillBreakBonusSelectableItem>
	{
		// Token: 0x1700138E RID: 5006
		// (get) Token: 0x0600AD38 RID: 44344 RVA: 0x004F0FAB File Offset: 0x004EF1AB
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700138F RID: 5007
		// (get) Token: 0x0600AD39 RID: 44345 RVA: 0x004F0FAE File Offset: 0x004EF1AE
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AD3A RID: 44346 RVA: 0x004F0FB4 File Offset: 0x004EF1B4
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600AD3B RID: 44347 RVA: 0x004F0FD0 File Offset: 0x004EF1D0
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> dropdownConfigs = new List<FilterDropdownItemConfig>();
			for (int i = 0; i < MedicineTypeSecondaryMenu.MedicineFilterTypes.Length; i++)
			{
				SkillBreakBonusEffectItem effectConfig = SkillBreakBonusEffect.Instance[MedicineTypeSecondaryMenu.MedicineFilterTypes[i]];
				dropdownConfigs.Add(new FilterDropdownItemConfig
				{
					Text = StringKey.CreateDirect(effectConfig.ShortName)
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x0600AD3C RID: 44348 RVA: 0x004F103C File Offset: 0x004EF23C
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

		// Token: 0x040085D0 RID: 34256
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

		// Token: 0x040085D1 RID: 34257
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
