using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004C1 RID: 1217
	public abstract class BracersCommonMakeTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000746 RID: 1862
		// (get) Token: 0x060041F5 RID: 16885 RVA: 0x00202F19 File Offset: 0x00201119
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x17000747 RID: 1863
		// (get) Token: 0x060041F6 RID: 16886
		protected abstract sbyte MyResourceType { get; }

		// Token: 0x060041F7 RID: 16887 RVA: 0x00202F1C File Offset: 0x0020111C
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

		// Token: 0x060041F8 RID: 16888 RVA: 0x00202FC4 File Offset: 0x002011C4
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Hardness);
		}

		// Token: 0x060041F9 RID: 16889 RVA: 0x00202FE8 File Offset: 0x002011E8
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

		// Token: 0x060041FA RID: 16890 RVA: 0x00203070 File Offset: 0x00201270
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

		// Token: 0x04002EC3 RID: 11971
		private readonly List<string> _optionNames = new List<string>();
	}
}
