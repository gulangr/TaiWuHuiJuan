using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DC5 RID: 3525
	public class PoisonTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x17001283 RID: 4739
		// (get) Token: 0x0600A9B8 RID: 43448 RVA: 0x004E6196 File Offset: 0x004E4396
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001284 RID: 4740
		// (get) Token: 0x0600A9B9 RID: 43449 RVA: 0x004E6199 File Offset: 0x004E4399
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A9BA RID: 43450 RVA: 0x004E619C File Offset: 0x004E439C
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A9BB RID: 43451 RVA: 0x004E61B8 File Offset: 0x004E43B8
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			for (int i = 0; i <= 5; i++)
			{
				configs.Add(new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(string.Format("LK_Poison_Name_{0}", i))
				});
			}
			return configs;
		}

		// Token: 0x0600A9BC RID: 43452 RVA: 0x004E6214 File Offset: 0x004E4414
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			MedicineItem medicineConfig = Medicine.Instance[data.Key.TemplateId];
			EMedicineEffectSubType effectSubType = medicineConfig.EffectSubType;
			if (!true)
			{
			}
			int num;
			switch (effectSubType)
			{
			case EMedicineEffectSubType.DetoxPoisonHotValue:
				num = 0;
				goto IL_C8;
			case EMedicineEffectSubType.DetoxPoisonGloomyValue:
				num = 1;
				goto IL_C8;
			case EMedicineEffectSubType.DetoxPoisonColdValue:
				num = 2;
				goto IL_C8;
			case EMedicineEffectSubType.DetoxPoisonRedValue:
				num = 3;
				goto IL_C8;
			case EMedicineEffectSubType.DetoxPoisonRottenValue:
				num = 4;
				goto IL_C8;
			case EMedicineEffectSubType.DetoxPoisonIllusoryValue:
				num = 5;
				goto IL_C8;
			case EMedicineEffectSubType.ApplyPoisonHotValue:
				num = 0;
				goto IL_C8;
			case EMedicineEffectSubType.ApplyPoisonGloomyValue:
				num = 1;
				goto IL_C8;
			case EMedicineEffectSubType.ApplyPoisonColdValue:
				num = 2;
				goto IL_C8;
			case EMedicineEffectSubType.ApplyPoisonRedValue:
				num = 3;
				goto IL_C8;
			case EMedicineEffectSubType.ApplyPoisonRottenValue:
				num = 4;
				goto IL_C8;
			case EMedicineEffectSubType.ApplyPoisonIllusoryValue:
				num = 5;
				goto IL_C8;
			case EMedicineEffectSubType.DetoxPoisonHotPercentage:
				num = 0;
				goto IL_C8;
			case EMedicineEffectSubType.DetoxPoisonGloomyPercentage:
				num = 1;
				goto IL_C8;
			case EMedicineEffectSubType.DetoxPoisonColdPercentage:
				num = 2;
				goto IL_C8;
			case EMedicineEffectSubType.DetoxPoisonRedPercentage:
				num = 3;
				goto IL_C8;
			case EMedicineEffectSubType.DetoxPoisonRottenPercentage:
				num = 4;
				goto IL_C8;
			case EMedicineEffectSubType.DetoxPoisonIllusoryPercentage:
				num = 5;
				goto IL_C8;
			}
			num = -1;
			IL_C8:
			if (!true)
			{
			}
			int poisonType = num;
			foreach (int selectedSubType in selectedIndices)
			{
				bool flag = selectedSubType == poisonType;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}
	}
}
