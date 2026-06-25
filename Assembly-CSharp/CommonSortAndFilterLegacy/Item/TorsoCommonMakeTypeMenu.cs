using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004DE RID: 1246
	public abstract class TorsoCommonMakeTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x1700077C RID: 1916
		// (get) Token: 0x06004269 RID: 17001 RVA: 0x00204119 File Offset: 0x00202319
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x1700077D RID: 1917
		// (get) Token: 0x0600426A RID: 17002
		protected abstract sbyte MyResourceType { get; }

		// Token: 0x0600426B RID: 17003 RVA: 0x0020411C File Offset: 0x0020231C
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

		// Token: 0x0600426C RID: 17004 RVA: 0x002041C4 File Offset: 0x002023C4
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Hardness);
		}

		// Token: 0x0600426D RID: 17005 RVA: 0x002041E8 File Offset: 0x002023E8
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

		// Token: 0x0600426E RID: 17006 RVA: 0x00204270 File Offset: 0x00202470
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

		// Token: 0x04002EEC RID: 12012
		private readonly List<string> _optionNames = new List<string>();
	}
}
