using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000515 RID: 1301
	internal abstract class CommonMaterialFilterTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007C2 RID: 1986
		// (get) Token: 0x06004313 RID: 17171 RVA: 0x00205EE9 File Offset: 0x002040E9
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x06004314 RID: 17172
		protected abstract IEnumerable<EMaterialFilterType> GenerateMaterialFilterTypes();

		// Token: 0x06004315 RID: 17173 RVA: 0x00205EEC File Offset: 0x002040EC
		public sealed override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x06004316 RID: 17174 RVA: 0x00205F10 File Offset: 0x00204110
		public sealed override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			this._materialFilterTypes.Clear();
			this._materialFilterTypes.AddRange(this.GenerateMaterialFilterTypes());
			return (from type in this._materialFilterTypes
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = null,
				Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Material_Filter_FilterType_{0}", (int)type))
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x06004317 RID: 17175 RVA: 0x00205F70 File Offset: 0x00204170
		public sealed override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			MaterialItem materialConfig = Material.Instance[data.Key.TemplateId];
			EMaterialFilterType filterType = materialConfig.FilterType;
			return selectedIndices.Any((int index) => filterType == this._materialFilterTypes[index]);
		}

		// Token: 0x04002FB0 RID: 12208
		private readonly List<EMaterialFilterType> _materialFilterTypes = new List<EMaterialFilterType>();
	}
}
