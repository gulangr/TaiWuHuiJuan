using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DA1 RID: 3489
	public abstract class CommonMaterialPropertyMenuNew : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x1700125A RID: 4698
		// (get) Token: 0x0600A93F RID: 43327 RVA: 0x004E4F29 File Offset: 0x004E3129
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x0600A940 RID: 43328 RVA: 0x004E4F2C File Offset: 0x004E312C
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_HerbProperty;
		}

		// Token: 0x0600A941 RID: 43329 RVA: 0x004E4F48 File Offset: 0x004E3148
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return (from property in CommonMaterialPropertyMenuNew.HerbProperties
			select new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Material_Filter_HerbProperty_{0}", (int)property)))).ToList<FilterDropdownItemConfig>();
		}

		// Token: 0x0600A942 RID: 43330 RVA: 0x004E4F88 File Offset: 0x004E3188
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
		{
			MaterialItem materialConfig = Material.Instance[data.Key.TemplateId];
			EMaterialProperty property = materialConfig.Property;
			return selectedIndices.Any((int index) => property == CommonMaterialPropertyMenuNew.HerbProperties[index]);
		}

		// Token: 0x04008484 RID: 33924
		private static readonly List<EMaterialProperty> HerbProperties = new List<EMaterialProperty>
		{
			EMaterialProperty.Yin,
			EMaterialProperty.Yang
		};
	}
}
