using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004D4 RID: 1236
	public abstract class HelmCommonMakeTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000767 RID: 1895
		// (get) Token: 0x06004242 RID: 16962 RVA: 0x00203CB1 File Offset: 0x00201EB1
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x17000768 RID: 1896
		// (get) Token: 0x06004243 RID: 16963
		protected abstract sbyte MyResourceType { get; }

		// Token: 0x06004244 RID: 16964 RVA: 0x00203CB4 File Offset: 0x00201EB4
		public sealed override void OnInit()
		{
			this._optionNames.Clear();
			foreach (ArmorItem armorConfig in ((IEnumerable<ArmorItem>)Armor.Instance))
			{
				bool flag = armorConfig.ResourceType != this.MyResourceType;
				if (!flag)
				{
					short makeItemSubType = armorConfig.MakeItemSubType;
					string filterName = MakeItemSubType.Instance[makeItemSubType].FilterName;
					bool flag2 = this._optionNames.Contains(filterName);
					if (!flag2)
					{
						this._optionNames.Add(filterName);
					}
				}
			}
		}

		// Token: 0x06004245 RID: 16965 RVA: 0x00203D5C File Offset: 0x00201F5C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Hardness);
		}

		// Token: 0x06004246 RID: 16966 RVA: 0x00203D80 File Offset: 0x00201F80
		public sealed override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			foreach (string optionName in this._optionNames)
			{
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateDirect(optionName)
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x06004247 RID: 16967 RVA: 0x00203E08 File Offset: 0x00202008
		public sealed override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			ArmorItem armorConfig = Armor.Instance[data.Key.TemplateId];
			short makeItemSubType = armorConfig.MakeItemSubType;
			foreach (int index in selectedIndices)
			{
				string selectedOptionName = this._optionNames[index];
				bool flag = MakeItemSubType.Instance[makeItemSubType].FilterName == selectedOptionName;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04002EDD RID: 11997
		private readonly List<string> _optionNames = new List<string>();
	}
}
