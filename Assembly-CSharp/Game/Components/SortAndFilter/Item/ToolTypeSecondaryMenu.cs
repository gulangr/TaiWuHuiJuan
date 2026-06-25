using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DE1 RID: 3553
	public class ToolTypeSecondaryMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x170012AF RID: 4783
		// (get) Token: 0x0600AA56 RID: 43606 RVA: 0x004E7EBB File Offset: 0x004E60BB
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x170012B0 RID: 4784
		// (get) Token: 0x0600AA57 RID: 43607 RVA: 0x004E7EBE File Offset: 0x004E60BE
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AA58 RID: 43608 RVA: 0x004E7EC4 File Offset: 0x004E60C4
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600AA59 RID: 43609 RVA: 0x004E7EE0 File Offset: 0x004E60E0
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
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
			select new FilterDropdownItemConfig
			{
				Text = LifeSkillType.Instance[skillTypeId].Name
			}).ToList<FilterDropdownItemConfig>();
		}

		// Token: 0x0600AA5A RID: 43610 RVA: 0x004E7F8C File Offset: 0x004E618C
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
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
				foreach (int index in selectedIndices)
				{
					bool flag2 = toolConfig.RequiredLifeSkillTypes != null && toolConfig.RequiredLifeSkillTypes.Contains(this._skillTypeIds[index]);
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x040084C1 RID: 33985
		private readonly List<sbyte> _skillTypeIds = new List<sbyte>();
	}
}
