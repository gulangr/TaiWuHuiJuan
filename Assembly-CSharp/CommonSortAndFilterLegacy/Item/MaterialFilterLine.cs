using System;
using System.Collections.Generic;
using System.Linq;
using GameData.Domains.Item.Display;
using UICommon.Character.Elements;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200051E RID: 1310
	public class MaterialFilterLine : FilterToggleGroupLine<ItemDisplayData>
	{
		// Token: 0x170007CA RID: 1994
		// (get) Token: 0x06004336 RID: 17206 RVA: 0x002063E3 File Offset: 0x002045E3
		public override int Id
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x06004337 RID: 17207 RVA: 0x002063E8 File Offset: 0x002045E8
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
				bool flag = data.Key.ItemType != 5;
				if (flag)
				{
					result = false;
				}
				else
				{
					short selectedItemSubType = this._itemSubTypes[toggleGroupState.Index];
					result = ItemFilterCommon.IsItemMatchItemSubType(data, selectedItemSubType);
				}
			}
			return result;
		}

		// Token: 0x06004338 RID: 17208 RVA: 0x00206444 File Offset: 0x00204644
		protected override List<FilterToggleConfig> GetFilterToggleConfigs()
		{
			return (from itemSubType in this._itemSubTypes
			select new FilterToggleConfig(ToggleTransitionIconSpriteNames.Default(), StringKey.CreateKey(string.Format("LK_ItemSubType_{0}", itemSubType)))).ToList<FilterToggleConfig>();
		}

		// Token: 0x06004339 RID: 17209 RVA: 0x00206488 File Offset: 0x00204688
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return new List<ToggleIdIndex>
			{
				new ToggleIdIndex(0, ToggleKey.CreateIndexKey(5))
			};
		}

		// Token: 0x170007CB RID: 1995
		// (get) Token: 0x0600433A RID: 17210 RVA: 0x002064B2 File Offset: 0x002046B2
		protected override int Level
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x04002FB9 RID: 12217
		private readonly List<short> _itemSubTypes = new List<short>
		{
			504,
			501,
			503,
			502,
			505,
			506,
			500
		};
	}
}
