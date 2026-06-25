using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200052C RID: 1324
	public class MaterialHerbCureMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007DA RID: 2010
		// (get) Token: 0x06004362 RID: 17250 RVA: 0x0020712D File Offset: 0x0020532D
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x170007DB RID: 2011
		// (get) Token: 0x06004363 RID: 17251 RVA: 0x00207130 File Offset: 0x00205330
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06004364 RID: 17252 RVA: 0x00207134 File Offset: 0x00205334
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x06004365 RID: 17253 RVA: 0x00207158 File Offset: 0x00205358
		public sealed override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			foreach (object item in Enum.GetValues(typeof(EMaterialHerbCureOption)))
			{
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(string.Format("LK_Poison_Name_{0}", (int)item))
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x06004366 RID: 17254 RVA: 0x00207200 File Offset: 0x00205400
		public sealed override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			List<MedicineItem> makeItems = CommonUtils.GetResultMedicine(data.Key.TemplateId);
			using (IEnumerator<int> enumerator = selectedIndices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int index = enumerator.Current;
					bool flag = makeItems.Any((MedicineItem t) => t.EffectSubType == (EMedicineEffectSubType)index);
					if (flag)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x170007DC RID: 2012
		// (get) Token: 0x06004367 RID: 17255 RVA: 0x00207284 File Offset: 0x00205484
		public override MenuOptionIndex? Dependency
		{
			get
			{
				return new MenuOptionIndex?(new MenuOptionIndex(1, 4));
			}
		}
	}
}
