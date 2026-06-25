using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DA2 RID: 3490
	public abstract class CommonMaterialFilterTypeMenuNew : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x1700125B RID: 4699
		// (get) Token: 0x0600A945 RID: 43333 RVA: 0x004E4FF9 File Offset: 0x004E31F9
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x0600A946 RID: 43334
		protected abstract IEnumerable<EMaterialFilterType> GenerateMaterialFilterTypes();

		// Token: 0x0600A947 RID: 43335 RVA: 0x004E4FFC File Offset: 0x004E31FC
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A948 RID: 43336 RVA: 0x004E5018 File Offset: 0x004E3218
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			this._materialFilterTypes.Clear();
			this._materialFilterTypes.AddRange(this.GenerateMaterialFilterTypes());
			return (from type in this._materialFilterTypes
			select new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Material_Filter_FilterType_{0}", (int)type)))).ToList<FilterDropdownItemConfig>();
		}

		// Token: 0x0600A949 RID: 43337 RVA: 0x004E5078 File Offset: 0x004E3278
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			MaterialItem materialConfig = Material.Instance[data.Key.TemplateId];
			EMaterialFilterType filterType = materialConfig.FilterType;
			return selectedIndices.Any((int index) => filterType == this._materialFilterTypes[index]);
		}

		// Token: 0x04008485 RID: 33925
		private readonly List<EMaterialFilterType> _materialFilterTypes = new List<EMaterialFilterType>();
	}
}
