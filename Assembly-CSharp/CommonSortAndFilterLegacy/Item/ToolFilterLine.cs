using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Item.Display;
using UICommon.Character.Elements;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200055F RID: 1375
	public class ToolFilterLine : FilterToggleGroupLine<ItemDisplayData>
	{
		// Token: 0x1700081E RID: 2078
		// (get) Token: 0x0600443B RID: 17467 RVA: 0x00209309 File Offset: 0x00207509
		public override int Id
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x0600443C RID: 17468 RVA: 0x0020930C File Offset: 0x0020750C
		public override bool IsDataMatch(ItemDisplayData data, LineState lineState)
		{
			ToggleKey toggleGroupState = lineState.ToggleGroupState;
			bool isAll = toggleGroupState.IsAll;
			bool result;
			if (isAll)
			{
				result = true;
			}
			else
			{
				bool flag = data.Key.ItemType != 6;
				if (flag)
				{
					result = false;
				}
				else
				{
					sbyte selectedLifeSkillTypeId = this._skillTypeIds[toggleGroupState.Index];
					CraftToolItem toolConfig = CraftTool.Instance[data.Key.TemplateId];
					List<sbyte> requiredLifeSkillTypes = toolConfig.RequiredLifeSkillTypes;
					result = (requiredLifeSkillTypes != null && requiredLifeSkillTypes.Contains(selectedLifeSkillTypeId));
				}
			}
			return result;
		}

		// Token: 0x0600443D RID: 17469 RVA: 0x00209390 File Offset: 0x00207590
		protected override List<FilterToggleConfig> GetFilterToggleConfigs()
		{
			this._skillTypeIds.Clear();
			this._skillTypeIds.AddRange(from id in CraftTool.Instance.SelectMany(delegate(CraftToolItem pair)
			{
				IEnumerable<sbyte> requiredLifeSkillTypes = pair.RequiredLifeSkillTypes;
				return requiredLifeSkillTypes ?? Enumerable.Empty<sbyte>();
			}).Distinct<sbyte>()
			where id != 9
			orderby id
			select id);
			return (from skillTypeId in this._skillTypeIds
			select LifeSkillType.Instance[skillTypeId] into config
			select new FilterToggleConfig(ToggleTransitionIconSpriteNames.CreateWithSameIcon(config.Icon), (config.TemplateId == 8) ? (config.Name + LifeSkillType.Instance[9].Name) : config.Name)).ToList<FilterToggleConfig>();
		}

		// Token: 0x0600443E RID: 17470 RVA: 0x00209484 File Offset: 0x00207684
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(4))
			};
		}

		// Token: 0x1700081F RID: 2079
		// (get) Token: 0x0600443F RID: 17471 RVA: 0x002094AE File Offset: 0x002076AE
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x04003009 RID: 12297
		private readonly List<sbyte> _skillTypeIds = new List<sbyte>();
	}
}
