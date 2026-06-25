using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DDB RID: 3547
	public class MedicineTypeSecondaryMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x170012A3 RID: 4771
		// (get) Token: 0x0600AA35 RID: 43573 RVA: 0x004E780B File Offset: 0x004E5A0B
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012A4 RID: 4772
		// (get) Token: 0x0600AA36 RID: 43574 RVA: 0x004E780E File Offset: 0x004E5A0E
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AA37 RID: 43575 RVA: 0x004E7814 File Offset: 0x004E5A14
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600AA38 RID: 43576 RVA: 0x004E7830 File Offset: 0x004E5A30
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			for (int i = 0; i < 8; i++)
			{
				configs.Add(new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_{0}", i))));
			}
			return configs;
		}

		// Token: 0x0600AA39 RID: 43577 RVA: 0x004E787C File Offset: 0x004E5A7C
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = data.Key.ItemType != 8;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				MedicineItem config = Medicine.Instance[data.Key.TemplateId];
				foreach (int selectedSubType in selectedIndices)
				{
					switch ((sbyte)selectedSubType)
					{
					case 0:
					{
						bool flag2 = config.EffectType == EMedicineEffectType.RecoverOuterInjury;
						if (flag2)
						{
							return true;
						}
						break;
					}
					case 1:
					{
						bool flag3 = config.EffectType == EMedicineEffectType.RecoverInnerInjury;
						if (flag3)
						{
							return true;
						}
						break;
					}
					case 2:
					{
						bool flag4 = config.EffectType == EMedicineEffectType.DetoxPoison;
						if (flag4)
						{
							return true;
						}
						break;
					}
					case 3:
					{
						bool flag5 = config.EffectType == EMedicineEffectType.ApplyPoison;
						if (flag5)
						{
							return true;
						}
						break;
					}
					case 4:
					{
						bool flag6 = config.EffectType == EMedicineEffectType.ChangeDisorderOfQi;
						if (flag6)
						{
							return true;
						}
						break;
					}
					case 5:
					{
						bool flag7 = config.EffectType == EMedicineEffectType.RecoverHealth;
						if (flag7)
						{
							return true;
						}
						break;
					}
					case 6:
					{
						bool flag8 = config.BuffAndOtherMedicine == 1;
						if (flag8)
						{
							return true;
						}
						break;
					}
					case 7:
					{
						bool flag9 = config.BuffAndOtherMedicine == 2;
						if (flag9)
						{
							return true;
						}
						break;
					}
					}
				}
				result = false;
			}
			return result;
		}
	}
}
