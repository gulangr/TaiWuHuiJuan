using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DB1 RID: 3505
	public class MaterialHerbBuffMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x17001269 RID: 4713
		// (get) Token: 0x0600A96C RID: 43372 RVA: 0x004E5475 File Offset: 0x004E3675
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700126A RID: 4714
		// (get) Token: 0x0600A96D RID: 43373 RVA: 0x004E5478 File Offset: 0x004E3678
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600A96E RID: 43374 RVA: 0x004E547C File Offset: 0x004E367C
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_6;
		}

		// Token: 0x0600A96F RID: 43375 RVA: 0x004E5498 File Offset: 0x004E3698
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

		// Token: 0x0600A970 RID: 43376 RVA: 0x004E55E4 File Offset: 0x004E37E4
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			List<MedicineItem> makeItems = CommonUtils.GetResultMedicine(data.Key.TemplateId);
			using (IEnumerator<int> enumerator = selectedIndices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case 0:
					{
						bool flag = makeItems.Any((MedicineItem t) => t.PenetrateOfOuter != 0);
						if (flag)
						{
							return true;
						}
						break;
					}
					case 1:
					{
						bool flag2 = makeItems.Any((MedicineItem t) => t.PenetrateOfInner != 0);
						if (flag2)
						{
							return true;
						}
						break;
					}
					case 2:
					{
						bool flag3 = makeItems.Any((MedicineItem t) => t.HitRateStrength != 0);
						if (flag3)
						{
							return true;
						}
						break;
					}
					case 3:
					{
						bool flag4 = makeItems.Any((MedicineItem t) => t.HitRateTechnique != 0);
						if (flag4)
						{
							return true;
						}
						break;
					}
					case 4:
					{
						bool flag5 = makeItems.Any((MedicineItem t) => t.HitRateSpeed != 0);
						if (flag5)
						{
							return true;
						}
						break;
					}
					case 5:
					{
						bool flag6 = makeItems.Any((MedicineItem t) => t.PenetrateResistOfOuter != 0);
						if (flag6)
						{
							return true;
						}
						break;
					}
					case 6:
					{
						bool flag7 = makeItems.Any((MedicineItem t) => t.PenetrateResistOfInner != 0);
						if (flag7)
						{
							return true;
						}
						break;
					}
					case 7:
					{
						bool flag8 = makeItems.Any((MedicineItem t) => t.AvoidRateStrength != 0);
						if (flag8)
						{
							return true;
						}
						break;
					}
					case 8:
					{
						bool flag9 = makeItems.Any((MedicineItem t) => t.AvoidRateTechnique != 0);
						if (flag9)
						{
							return true;
						}
						break;
					}
					case 9:
					{
						bool flag10 = makeItems.Any((MedicineItem t) => t.AvoidRateSpeed != 0);
						if (flag10)
						{
							return true;
						}
						break;
					}
					case 10:
					{
						bool flag11 = makeItems.Any((MedicineItem t) => t.BreakBonusEffect == 25);
						if (flag11)
						{
							return true;
						}
						break;
					}
					case 11:
					{
						bool flag12 = makeItems.Any((MedicineItem t) => t.BreakBonusEffect == 26);
						if (flag12)
						{
							return true;
						}
						break;
					}
					case 12:
					{
						bool flag13 = makeItems.Any((MedicineItem t) => t.BreakBonusEffect == 27);
						if (flag13)
						{
							return true;
						}
						break;
					}
					case 13:
					{
						bool flag14 = makeItems.Any((MedicineItem t) => t.BreakBonusEffect == 28);
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
