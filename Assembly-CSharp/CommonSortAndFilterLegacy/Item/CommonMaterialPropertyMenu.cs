using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000519 RID: 1305
	public abstract class CommonMaterialPropertyMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x170007C5 RID: 1989
		// (get) Token: 0x06004326 RID: 17190 RVA: 0x00206297 File Offset: 0x00204497
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x06004327 RID: 17191 RVA: 0x0020629C File Offset: 0x0020449C
		public sealed override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_HerbProperty);
		}

		// Token: 0x06004328 RID: 17192 RVA: 0x002062C0 File Offset: 0x002044C0
		public sealed override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			return (from property in CommonMaterialPropertyMenu.HerbProperties
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = null,
				Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Material_Filter_HerbProperty_{0}", (int)property))
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x06004329 RID: 17193 RVA: 0x00206300 File Offset: 0x00204500
		public sealed override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			MaterialItem materialConfig = Material.Instance[data.Key.TemplateId];
			EMaterialProperty property = materialConfig.Property;
			return selectedIndices.Any((int index) => property == CommonMaterialPropertyMenu.HerbProperties[index]);
		}

		// Token: 0x04002FB5 RID: 12213
		private static readonly List<EMaterialProperty> HerbProperties = new List<EMaterialProperty>
		{
			EMaterialProperty.Yin,
			EMaterialProperty.Yang
		};
	}
}
