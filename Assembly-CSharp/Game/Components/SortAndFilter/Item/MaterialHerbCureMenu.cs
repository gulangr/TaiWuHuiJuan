using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DB2 RID: 3506
	public class MaterialHerbCureMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x1700126B RID: 4715
		// (get) Token: 0x0600A972 RID: 43378 RVA: 0x004E59A9 File Offset: 0x004E3BA9
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700126C RID: 4716
		// (get) Token: 0x0600A973 RID: 43379 RVA: 0x004E59AC File Offset: 0x004E3BAC
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x1700126D RID: 4717
		// (get) Token: 0x0600A974 RID: 43380 RVA: 0x004E59AF File Offset: 0x004E3BAF
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(0, 4));
			}
		}

		// Token: 0x0600A975 RID: 43381 RVA: 0x004E59C0 File Offset: 0x004E3BC0
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A976 RID: 43382 RVA: 0x004E59DC File Offset: 0x004E3BDC
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			foreach (object item in Enum.GetValues(typeof(EMaterialHerbCureOption)))
			{
				configs.Add(new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_Poison_Name_{0}", (int)item))));
			}
			return configs;
		}

		// Token: 0x0600A977 RID: 43383 RVA: 0x004E5A6C File Offset: 0x004E3C6C
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			List<MedicineItem> makeItems = CommonUtils.GetResultMedicine(data.Key.TemplateId);
			foreach (MedicineItem makeItem in makeItems)
			{
				EMedicineEffectSubType effectSubType = makeItem.EffectSubType;
				if (!true)
				{
				}
				sbyte b;
				switch (effectSubType)
				{
				case EMedicineEffectSubType.DetoxPoisonHotValue:
					b = 0;
					break;
				case EMedicineEffectSubType.DetoxPoisonGloomyValue:
					b = 1;
					break;
				case EMedicineEffectSubType.DetoxPoisonColdValue:
					b = 2;
					break;
				case EMedicineEffectSubType.DetoxPoisonRedValue:
					b = 3;
					break;
				case EMedicineEffectSubType.DetoxPoisonRottenValue:
					b = 4;
					break;
				case EMedicineEffectSubType.DetoxPoisonIllusoryValue:
					b = 5;
					break;
				case EMedicineEffectSubType.ApplyPoisonHotValue:
				case EMedicineEffectSubType.ApplyPoisonGloomyValue:
				case EMedicineEffectSubType.ApplyPoisonColdValue:
				case EMedicineEffectSubType.ApplyPoisonRedValue:
				case EMedicineEffectSubType.ApplyPoisonRottenValue:
				case EMedicineEffectSubType.ApplyPoisonIllusoryValue:
				case EMedicineEffectSubType.RecoverHealthPercentage:
				case EMedicineEffectSubType.ChangeDisorderOfQiPercentage:
					goto IL_CB;
				case EMedicineEffectSubType.DetoxPoisonHotPercentage:
					b = 0;
					break;
				case EMedicineEffectSubType.DetoxPoisonGloomyPercentage:
					b = 1;
					break;
				case EMedicineEffectSubType.DetoxPoisonColdPercentage:
					b = 2;
					break;
				case EMedicineEffectSubType.DetoxPoisonRedPercentage:
					b = 3;
					break;
				case EMedicineEffectSubType.DetoxPoisonRottenPercentage:
					b = 4;
					break;
				case EMedicineEffectSubType.DetoxPoisonIllusoryPercentage:
					b = 5;
					break;
				default:
					goto IL_CB;
				}
				IL_D0:
				if (!true)
				{
				}
				sbyte poisonType = b;
				bool flag = selectedIndices.Contains((int)poisonType);
				if (flag)
				{
					return true;
				}
				continue;
				IL_CB:
				b = -1;
				goto IL_D0;
			}
			return false;
		}
	}
}
