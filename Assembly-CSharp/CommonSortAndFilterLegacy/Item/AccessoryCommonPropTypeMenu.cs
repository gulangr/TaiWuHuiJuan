using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004A8 RID: 1192
	public abstract class AccessoryCommonPropTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000715 RID: 1813
		// (get) Token: 0x06004182 RID: 16770 RVA: 0x00201C31 File Offset: 0x001FFE31
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x06004183 RID: 16771 RVA: 0x00201C34 File Offset: 0x001FFE34
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Prop);
		}

		// Token: 0x06004184 RID: 16772 RVA: 0x00201C58 File Offset: 0x001FFE58
		public sealed override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			foreach (string optionName in this._optionNames)
			{
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(optionName)
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x06004185 RID: 16773 RVA: 0x00201CE0 File Offset: 0x001FFEE0
		public sealed override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			AccessoryItem accessoryConfig = Accessory.Instance[data.Key.TemplateId];
			short makeItemSubType = accessoryConfig.MakeItemSubType;
			foreach (int index in selectedIndices)
			{
				bool flag = this.CheckOptionValid(index, accessoryConfig);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004186 RID: 16774
		protected abstract bool CheckOptionValid(int index, AccessoryItem armorConfig);

		// Token: 0x04002EA3 RID: 11939
		protected readonly List<string> _optionNames = new List<string>();
	}
}
