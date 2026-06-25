using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using Game.Views.Building.BuildingManage;

namespace Game.Components.SortAndFilter.Chicken
{
	// Token: 0x02000E4D RID: 3661
	public class PersonalityMenu : DetailedFilterMenuLogic<ChickenData>
	{
		// Token: 0x1700133B RID: 4923
		// (get) Token: 0x0600AC1D RID: 44061 RVA: 0x004ED7D4 File Offset: 0x004EB9D4
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700133C RID: 4924
		// (get) Token: 0x0600AC1E RID: 44062 RVA: 0x004ED7D7 File Offset: 0x004EB9D7
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AC1F RID: 44063 RVA: 0x004ED7DC File Offset: 0x004EB9DC
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Sort;
		}

		// Token: 0x0600AC20 RID: 44064 RVA: 0x004ED7F8 File Offset: 0x004EB9F8
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			return new List<FilterDropdownItemConfig>
			{
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_Personality_Calm_Name)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_Personality_Clever_Name)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_Personality_Enthusiastic_Name)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_Personality_Brave_Name)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_Personality_Firm_Name)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_Personality_Lucky_Name)
				},
				new FilterDropdownItemConfig
				{
					Text = StringKey.CreateKey(LanguageKey.LK_Personality_Perceptive_Name)
				}
			};
		}

		// Token: 0x0600AC21 RID: 44065 RVA: 0x004ED8F8 File Offset: 0x004EBAF8
		public override bool IsDataMatch(ChickenData data, IReadOnlyCollection<int> selectedIndices)
		{
			Config.ChickenItem chickenItem = Chicken.Instance.GetItem(data.TemplateId);
			return selectedIndices.Any((int v) => v == (int)chickenItem.PersonalityType);
		}
	}
}
