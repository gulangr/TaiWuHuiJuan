using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000561 RID: 1377
	public class ToolTypeSecondaryMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000822 RID: 2082
		// (get) Token: 0x06004446 RID: 17478 RVA: 0x0020950F File Offset: 0x0020770F
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x17000823 RID: 2083
		// (get) Token: 0x06004447 RID: 17479 RVA: 0x00209512 File Offset: 0x00207712
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06004448 RID: 17480 RVA: 0x00209518 File Offset: 0x00207718
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x06004449 RID: 17481 RVA: 0x0020953C File Offset: 0x0020773C
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			this._skillTypeIds.Clear();
			this._skillTypeIds.AddRange(from id in CraftTool.Instance.SelectMany(delegate(CraftToolItem pair)
			{
				IEnumerable<sbyte> requiredLifeSkillTypes = pair.RequiredLifeSkillTypes;
				return requiredLifeSkillTypes ?? Enumerable.Empty<sbyte>();
			}).Distinct<sbyte>()
			orderby id
			select id);
			return (from skillTypeId in this._skillTypeIds
			select new DetailFilterMultiSelectDropdownItemConfig
			{
				IconPath = LifeSkillType.Instance[skillTypeId].Icon,
				Text = LifeSkillType.Instance[skillTypeId].Name
			}).ToList<DetailFilterMultiSelectDropdownItemConfig>();
		}

		// Token: 0x0600444A RID: 17482 RVA: 0x002095E8 File Offset: 0x002077E8
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = data.Key.ItemType != 6;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				CraftToolItem toolConfig = CraftTool.Instance[data.Key.TemplateId];
				foreach (int item in selectedIndices)
				{
					bool flag2 = toolConfig.RequiredLifeSkillTypes != null && toolConfig.RequiredLifeSkillTypes.Contains(this._skillTypeIds[item]);
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0400300A RID: 12298
		private readonly List<sbyte> _skillTypeIds = new List<sbyte>();
	}
}
