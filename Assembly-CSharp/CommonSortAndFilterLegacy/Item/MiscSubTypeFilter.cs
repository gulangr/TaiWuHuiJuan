using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;
using UICommon.Character.Elements;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000554 RID: 1364
	public class MiscSubTypeFilter : FilterToggleGroupLine<ItemDisplayData>
	{
		// Token: 0x1700080C RID: 2060
		// (get) Token: 0x060043FF RID: 17407 RVA: 0x00208AC3 File Offset: 0x00206CC3
		public override int Id
		{
			get
			{
				return 7;
			}
		}

		// Token: 0x06004400 RID: 17408 RVA: 0x00208AC8 File Offset: 0x00206CC8
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
				sbyte itemType = data.Key.ItemType;
				bool flag = itemType != 12 && itemType != 11;
				if (flag)
				{
					result = false;
				}
				else
				{
					EMiscFilterKey itemIndexKey = this.GetToggleIndexKey(data.Key.ItemType, data.Key.TemplateId);
					result = (toggleGroupState.Index == (int)itemIndexKey);
				}
			}
			return result;
		}

		// Token: 0x06004401 RID: 17409 RVA: 0x00208B40 File Offset: 0x00206D40
		private EMiscFilterKey GetToggleIndexKey(sbyte itemType, short templateId)
		{
			bool flag = itemType == 11;
			EMiscFilterKey result;
			if (flag)
			{
				result = EMiscFilterKey.Cricket;
			}
			else
			{
				MiscItem miscConfig = Misc.Instance[templateId];
				switch (miscConfig.FilterType)
				{
				case EMiscFilterType.Resource:
					result = EMiscFilterKey.Resource;
					break;
				case EMiscFilterType.Item:
					result = EMiscFilterKey.Item;
					break;
				case EMiscFilterType.Collection:
					result = EMiscFilterKey.Collection;
					break;
				case EMiscFilterType.KeyItem:
					result = EMiscFilterKey.KeyItem;
					break;
				case EMiscFilterType.LegendaryBook:
					result = EMiscFilterKey.LegendaryBook;
					break;
				case EMiscFilterType.WesternPresent:
					result = EMiscFilterKey.WesternPresent;
					break;
				case EMiscFilterType.Misc:
					result = EMiscFilterKey.Misc;
					break;
				default:
					result = EMiscFilterKey.Count;
					break;
				}
			}
			return result;
		}

		// Token: 0x06004402 RID: 17410 RVA: 0x00208BBC File Offset: 0x00206DBC
		protected override List<FilterToggleConfig> GetFilterToggleConfigs()
		{
			List<FilterToggleConfig> configs = new List<FilterToggleConfig>();
			for (EMiscFilterKey t = EMiscFilterKey.Resource; t < EMiscFilterKey.Count; t += 1)
			{
				StringKey key = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Misc_{0}", (int)t));
				configs.Add(new FilterToggleConfig(ToggleTransitionIconSpriteNames.Default(), key));
			}
			return configs;
		}

		// Token: 0x06004403 RID: 17411 RVA: 0x00208C14 File Offset: 0x00206E14
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(6))
			};
		}

		// Token: 0x1700080D RID: 2061
		// (get) Token: 0x06004404 RID: 17412 RVA: 0x00208C3E File Offset: 0x00206E3E
		protected override int Level
		{
			get
			{
				return 1;
			}
		}
	}
}
