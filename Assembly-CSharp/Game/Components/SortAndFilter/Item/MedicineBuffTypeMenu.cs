using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DC8 RID: 3528
	public class MedicineBuffTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x17001287 RID: 4743
		// (get) Token: 0x0600A9C3 RID: 43459 RVA: 0x004E6397 File Offset: 0x004E4597
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001288 RID: 4744
		// (get) Token: 0x0600A9C4 RID: 43460 RVA: 0x004E639A File Offset: 0x004E459A
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A9C5 RID: 43461 RVA: 0x004E63A0 File Offset: 0x004E45A0
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A9C6 RID: 43462 RVA: 0x004E63BC File Offset: 0x004E45BC
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_Penetrate_Outer")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_Penetrate_Inner")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_HitType_0")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_HitType_1")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_HitType_2")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_Penetrate_Resist_Outer")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_Penetrate_Resist_Inner")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_AvoidType_0")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_AvoidType_1")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_AvoidType_2")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Medicine_Buff_0")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Medicine_Buff_1")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Medicine_Buff_2")),
				new FilterDropdownItemConfig(StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Medicine_Buff_3"))
			};
		}

		// Token: 0x0600A9C7 RID: 43463 RVA: 0x004E6508 File Offset: 0x004E4708
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			MedicineItem medicineConfig = Medicine.Instance[data.Key.TemplateId];
			using (IEnumerator<int> enumerator = selectedIndices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case 0:
					{
						bool flag = medicineConfig.PenetrateOfOuter != 0;
						if (flag)
						{
							return true;
						}
						break;
					}
					case 1:
					{
						bool flag2 = medicineConfig.PenetrateOfInner != 0;
						if (flag2)
						{
							return true;
						}
						break;
					}
					case 2:
					{
						bool flag3 = medicineConfig.HitRateStrength != 0;
						if (flag3)
						{
							return true;
						}
						break;
					}
					case 3:
					{
						bool flag4 = medicineConfig.HitRateTechnique != 0;
						if (flag4)
						{
							return true;
						}
						break;
					}
					case 4:
					{
						bool flag5 = medicineConfig.HitRateSpeed != 0;
						if (flag5)
						{
							return true;
						}
						break;
					}
					case 5:
					{
						bool flag6 = medicineConfig.PenetrateResistOfOuter != 0;
						if (flag6)
						{
							return true;
						}
						break;
					}
					case 6:
					{
						bool flag7 = medicineConfig.PenetrateResistOfInner != 0;
						if (flag7)
						{
							return true;
						}
						break;
					}
					case 7:
					{
						bool flag8 = medicineConfig.AvoidRateStrength != 0;
						if (flag8)
						{
							return true;
						}
						break;
					}
					case 8:
					{
						bool flag9 = medicineConfig.AvoidRateTechnique != 0;
						if (flag9)
						{
							return true;
						}
						break;
					}
					case 9:
					{
						bool flag10 = medicineConfig.AvoidRateSpeed != 0;
						if (flag10)
						{
							return true;
						}
						break;
					}
					case 10:
					{
						bool flag11 = medicineConfig.BreakBonusEffect == 25;
						if (flag11)
						{
							return true;
						}
						break;
					}
					case 11:
					{
						bool flag12 = medicineConfig.BreakBonusEffect == 26;
						if (flag12)
						{
							return true;
						}
						break;
					}
					case 12:
					{
						bool flag13 = medicineConfig.BreakBonusEffect == 27;
						if (flag13)
						{
							return true;
						}
						break;
					}
					case 13:
					{
						bool flag14 = medicineConfig.BreakBonusEffect == 28;
						if (flag14)
						{
							return true;
						}
						break;
					}
					}
				}
			}
			return false;
		}
	}
}
