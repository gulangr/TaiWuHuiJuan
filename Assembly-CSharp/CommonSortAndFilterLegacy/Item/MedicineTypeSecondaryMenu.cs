using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000544 RID: 1348
	public class MedicineTypeSecondaryMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007F7 RID: 2039
		// (get) Token: 0x060043AD RID: 17325 RVA: 0x00207D7B File Offset: 0x00205F7B
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170007F8 RID: 2040
		// (get) Token: 0x060043AE RID: 17326 RVA: 0x00207D7E File Offset: 0x00205F7E
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060043AF RID: 17327 RVA: 0x00207D84 File Offset: 0x00205F84
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x060043B0 RID: 17328 RVA: 0x00207DA8 File Offset: 0x00205FA8
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < 8; i++)
			{
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_{0}", i))
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x060043B1 RID: 17329 RVA: 0x00207E10 File Offset: 0x00206010
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			MedicineItem config = Medicine.Instance[data.Key.TemplateId];
			foreach (int selectedSubType in selectedIndices)
			{
				switch ((sbyte)selectedSubType)
				{
				case 0:
				{
					bool flag = config.EffectType == EMedicineEffectType.RecoverOuterInjury;
					if (flag)
					{
						return true;
					}
					break;
				}
				case 1:
				{
					bool flag2 = config.EffectType == EMedicineEffectType.RecoverInnerInjury;
					if (flag2)
					{
						return true;
					}
					break;
				}
				case 2:
				{
					bool flag3 = config.EffectType == EMedicineEffectType.DetoxPoison;
					if (flag3)
					{
						return true;
					}
					break;
				}
				case 3:
				{
					bool flag4 = config.EffectType == EMedicineEffectType.ApplyPoison;
					if (flag4)
					{
						return true;
					}
					break;
				}
				case 4:
				{
					bool flag5 = config.EffectType == EMedicineEffectType.ChangeDisorderOfQi;
					if (flag5)
					{
						return true;
					}
					break;
				}
				case 5:
				{
					bool flag6 = config.EffectType == EMedicineEffectType.RecoverHealth;
					if (flag6)
					{
						return true;
					}
					break;
				}
				case 6:
				{
					bool flag7 = config.BuffAndOtherMedicine == 1;
					if (flag7)
					{
						return true;
					}
					break;
				}
				case 7:
				{
					bool flag8 = config.BuffAndOtherMedicine == 2;
					if (flag8)
					{
						return true;
					}
					break;
				}
				}
			}
			return false;
		}
	}
}
