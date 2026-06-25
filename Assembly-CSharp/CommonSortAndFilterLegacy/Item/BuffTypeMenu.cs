using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000541 RID: 1345
	public class BuffTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007F1 RID: 2033
		// (get) Token: 0x0600439C RID: 17308 RVA: 0x002075CF File Offset: 0x002057CF
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170007F2 RID: 2034
		// (get) Token: 0x0600439D RID: 17309 RVA: 0x002075D2 File Offset: 0x002057D2
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600439E RID: 17310 RVA: 0x002075D8 File Offset: 0x002057D8
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x0600439F RID: 17311 RVA: 0x002075FC File Offset: 0x002057FC
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return new List<DetailFilterMultiSelectDropdownItemConfig>
			{
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_Penetrate_Outer")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_Penetrate_Inner")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_HitType_0")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_HitType_1")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_HitType_2")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_Penetrate_Resist_Outer")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_Penetrate_Resist_Inner")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_AvoidType_0")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_AvoidType_1")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_AvoidType_2")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Medicine_Buff_0")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Medicine_Buff_1")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Medicine_Buff_2")
				},
				new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey("LK_CommonSortAndFilter_Filter_Item_Medicine_Buff_3")
				}
			};
		}

		// Token: 0x060043A0 RID: 17312 RVA: 0x0020788C File Offset: 0x00205A8C
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
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
