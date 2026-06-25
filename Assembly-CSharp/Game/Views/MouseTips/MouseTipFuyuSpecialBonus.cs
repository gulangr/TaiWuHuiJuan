using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Game.Views.MouseTips
{
	// Token: 0x02000864 RID: 2148
	public class MouseTipFuyuSpecialBonus : MouseTipFuyuPropertyGrid
	{
		// Token: 0x060067C1 RID: 26561 RVA: 0x002F6384 File Offset: 0x002F4584
		public void Set([TupleElementNames(new string[]
		{
			"icon",
			"title",
			"value",
			"percent"
		})] List<ValueTuple<string, string, int, int>> items)
		{
			bool hasItems = items != null && items.Count > 0;
			base.gameObject.SetActive(hasItems);
			bool flag = !hasItems;
			if (!flag)
			{
				base.PrepareItems(items.Count);
				for (int i = 0; i < items.Count; i++)
				{
					MouseTipFuyuPropertyItem item = base.GetItem(i);
					ValueTuple<string, string, int, int> data = items[i];
					item.Set(data.Item1, data.Item2, data.Item3, data.Item4, false);
				}
			}
		}
	}
}
