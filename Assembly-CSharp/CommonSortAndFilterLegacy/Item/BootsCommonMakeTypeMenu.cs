using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004B8 RID: 1208
	public abstract class BootsCommonMakeTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000734 RID: 1844
		// (get) Token: 0x060041D3 RID: 16851 RVA: 0x00202AF1 File Offset: 0x00200CF1
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x17000735 RID: 1845
		// (get) Token: 0x060041D4 RID: 16852
		protected abstract sbyte MyResourceType { get; }

		// Token: 0x060041D5 RID: 16853 RVA: 0x00202AF4 File Offset: 0x00200CF4
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

		// Token: 0x060041D6 RID: 16854 RVA: 0x00202B9C File Offset: 0x00200D9C
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Hardness);
		}

		// Token: 0x060041D7 RID: 16855 RVA: 0x00202BC0 File Offset: 0x00200DC0
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

		// Token: 0x060041D8 RID: 16856 RVA: 0x00202C48 File Offset: 0x00200E48
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

		// Token: 0x04002EB4 RID: 11956
		private readonly List<string> _optionNames = new List<string>();
	}
}
