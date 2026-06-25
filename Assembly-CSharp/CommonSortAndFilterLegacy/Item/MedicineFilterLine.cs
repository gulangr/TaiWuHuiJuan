using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;
using UICommon.Character.Elements;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000542 RID: 1346
	public class MedicineFilterLine : FilterToggleGroupLine<ItemDisplayData>
	{
		// Token: 0x170007F3 RID: 2035
		// (get) Token: 0x060043A2 RID: 17314 RVA: 0x00207AC1 File Offset: 0x00205CC1
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x060043A3 RID: 17315 RVA: 0x00207AC4 File Offset: 0x00205CC4
		public override bool IsDataMatch(ItemDisplayData data, LineState medicineLineState)
		{
			ToggleKey medicineToggleState = medicineLineState.ToggleGroupState;
			bool isAll = medicineToggleState.IsAll;
			bool result;
			if (isAll)
			{
				result = true;
			}
			else
			{
				bool flag = data.Key.ItemType != 8;
				if (flag)
				{
					result = false;
				}
				else
				{
					MedicineItem config = Medicine.Instance[data.Key.TemplateId];
					switch (medicineToggleState.Index)
					{
					case 0:
					{
						bool flag2 = config.EffectType > EMedicineEffectType.RecoverOuterInjury;
						if (flag2)
						{
							return false;
						}
						break;
					}
					case 1:
					{
						bool flag3 = config.EffectType != EMedicineEffectType.RecoverInnerInjury;
						if (flag3)
						{
							return false;
						}
						break;
					}
					case 2:
					{
						bool flag4 = config.EffectType != EMedicineEffectType.DetoxPoison;
						if (flag4)
						{
							return false;
						}
						break;
					}
					case 3:
					{
						bool flag5 = config.EffectType != EMedicineEffectType.ApplyPoison;
						if (flag5)
						{
							return false;
						}
						break;
					}
					case 4:
					{
						bool flag6 = config.EffectType != EMedicineEffectType.ChangeDisorderOfQi;
						if (flag6)
						{
							return false;
						}
						break;
					}
					case 5:
					{
						bool flag7 = config.EffectType != EMedicineEffectType.RecoverHealth;
						if (flag7)
						{
							return false;
						}
						break;
					}
					case 6:
					{
						bool flag8 = config.BuffAndOtherMedicine != 1;
						if (flag8)
						{
							return false;
						}
						break;
					}
					case 7:
					{
						bool flag9 = config.BuffAndOtherMedicine != 2;
						if (flag9)
						{
							return false;
						}
						break;
					}
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060043A4 RID: 17316 RVA: 0x00207C24 File Offset: 0x00205E24
		protected override List<FilterToggleConfig> GetFilterToggleConfigs()
		{
			ToggleTransitionIconSpriteNames defaultIconNames = ToggleTransitionIconSpriteNames.Default();
			return new List<FilterToggleConfig>
			{
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_0),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_1),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_2),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_3),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_4),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_5),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_6),
				new FilterToggleConfig(defaultIconNames, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_7)
			};
		}

		// Token: 0x060043A5 RID: 17317 RVA: 0x00207CFC File Offset: 0x00205EFC
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(1))
			};
		}

		// Token: 0x170007F4 RID: 2036
		// (get) Token: 0x060043A6 RID: 17318 RVA: 0x00207D26 File Offset: 0x00205F26
		protected override int Level
		{
			get
			{
				return 1;
			}
		}
	}
}
