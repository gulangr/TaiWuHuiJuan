using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004E7 RID: 1255
	public class WeaponSubTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x1700078C RID: 1932
		// (get) Token: 0x06004285 RID: 17029 RVA: 0x002043EE File Offset: 0x002025EE
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700078D RID: 1933
		// (get) Token: 0x06004286 RID: 17030 RVA: 0x002043F1 File Offset: 0x002025F1
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x06004287 RID: 17031 RVA: 0x002043F4 File Offset: 0x002025F4
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x06004288 RID: 17032 RVA: 0x00204418 File Offset: 0x00202618
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < 15; i++)
			{
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(string.Format("LK_ItemSubType_{0}", i))
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x06004289 RID: 17033 RVA: 0x0020447C File Offset: 0x0020267C
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			WeaponItem weaponConfig = Weapon.Instance[data.Key.TemplateId];
			short weaponSubType = weaponConfig.ItemSubType;
			foreach (int selectedSubType in selectedIndices)
			{
				bool flag = selectedSubType == (int)weaponSubType;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04002F04 RID: 12036
		private const int FilterWeaponSubtypeCount = 15;
	}
}
