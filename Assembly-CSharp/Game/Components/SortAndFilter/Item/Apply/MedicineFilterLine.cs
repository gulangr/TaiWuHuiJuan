using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item.Apply
{
	// Token: 0x02000DF5 RID: 3573
	public class MedicineFilterLine : FilterToggleGroupLineLogic<ITradeableContent>
	{
		// Token: 0x170012C4 RID: 4804
		// (get) Token: 0x0600AAAA RID: 43690 RVA: 0x004E9024 File Offset: 0x004E7224
		public override int Id
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600AAAB RID: 43691 RVA: 0x004E9028 File Offset: 0x004E7228
		public override bool IsDataMatch(ITradeableContent data, LineState lineState)
		{
			ToggleKey medicineToggleState = lineState.ToggleGroupState;
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

		// Token: 0x0600AAAC RID: 43692 RVA: 0x004E9188 File Offset: 0x004E7388
		protected override List<FilterToggleConfig> GetFilterToggleConfigs()
		{
			return new List<FilterToggleConfig>
			{
				new FilterToggleConfig("ui9_btn_filter_outside", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_0),
				new FilterToggleConfig("ui9_btn_filter_inside", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_1),
				new FilterToggleConfig("ui9_btn_filter_antidote", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_2),
				new FilterToggleConfig("ui9_btn_filter_poison", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_3),
				new FilterToggleConfig("ui9_btn_filter_breath", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_4),
				new FilterToggleConfig("ui9_btn_filter_healthy", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_5),
				new FilterToggleConfig("ui9_btn_filter_benefit", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_6),
				new FilterToggleConfig("ui9_btn_filter_misc", LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_7)
			};
		}

		// Token: 0x0600AAAD RID: 43693 RVA: 0x004E9278 File Offset: 0x004E7478
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(1))
			};
		}

		// Token: 0x170012C5 RID: 4805
		// (get) Token: 0x0600AAAE RID: 43694 RVA: 0x004E92A2 File Offset: 0x004E74A2
		protected override int Level
		{
			get
			{
				return 1;
			}
		}
	}
}
