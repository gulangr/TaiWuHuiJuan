using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000516 RID: 1302
	public abstract class CommonMaterialHardnessMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007C3 RID: 1987
		// (get) Token: 0x06004319 RID: 17177 RVA: 0x00205FD7 File Offset: 0x002041D7
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x0600431A RID: 17178 RVA: 0x00205FDC File Offset: 0x002041DC
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Hardness);
		}

		// Token: 0x0600431B RID: 17179
		protected abstract IEnumerable<EMaterialFilterHardness> GenerateMaterialHardnessList();

		// Token: 0x0600431C RID: 17180 RVA: 0x00206000 File Offset: 0x00204200
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			this._materialHardnessList.Clear();
			this._materialHardnessList.AddRange(this.GenerateMaterialHardnessList());
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			foreach (EMaterialFilterHardness hardness in this._materialHardnessList)
			{
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Material_FilterHardness_{0}", (int)hardness))
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x0600431D RID: 17181 RVA: 0x002060B0 File Offset: 0x002042B0
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			MaterialItem materialConfig = Material.Instance[data.Key.TemplateId];
			return selectedIndices.Any((int index) => materialConfig.FilterHardness == this._materialHardnessList[index]);
		}

		// Token: 0x04002FB1 RID: 12209
		private readonly List<EMaterialFilterHardness> _materialHardnessList = new List<EMaterialFilterHardness>();
	}
}
