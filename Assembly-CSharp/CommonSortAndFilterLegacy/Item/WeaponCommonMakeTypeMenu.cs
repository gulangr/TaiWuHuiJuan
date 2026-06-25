using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004E9 RID: 1257
	public abstract class WeaponCommonMakeTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000790 RID: 1936
		// (get) Token: 0x06004291 RID: 17041
		protected abstract sbyte MyResourceType { get; }

		// Token: 0x06004292 RID: 17042 RVA: 0x0020465C File Offset: 0x0020285C
		public sealed override void OnInit()
		{
			this._optionNames.Clear();
			foreach (WeaponItem weaponConfig in ((IEnumerable<WeaponItem>)Weapon.Instance))
			{
				bool flag = weaponConfig.ResourceType != this.MyResourceType;
				if (!flag)
				{
					short makeItemSubType = weaponConfig.MakeItemSubType;
					string filterName = MakeItemSubType.Instance[makeItemSubType].FilterName;
					bool flag2 = this._optionNames.Contains(filterName);
					if (!flag2)
					{
						this._optionNames.Add(filterName);
					}
				}
			}
		}

		// Token: 0x06004293 RID: 17043 RVA: 0x00204704 File Offset: 0x00202904
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Hardness);
		}

		// Token: 0x06004294 RID: 17044 RVA: 0x00204728 File Offset: 0x00202928
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

		// Token: 0x06004295 RID: 17045 RVA: 0x002047B0 File Offset: 0x002029B0
		public sealed override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			WeaponItem weaponConfig = Weapon.Instance[data.Key.TemplateId];
			short makeItemSubType = weaponConfig.MakeItemSubType;
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

		// Token: 0x17000791 RID: 1937
		// (get) Token: 0x06004296 RID: 17046 RVA: 0x0020484C File Offset: 0x00202A4C
		public sealed override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x04002F06 RID: 12038
		private readonly List<string> _optionNames = new List<string>();
	}
}
