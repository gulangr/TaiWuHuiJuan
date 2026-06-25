using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Game.Views.MouseTips
{
	// Token: 0x02000859 RID: 2137
	public class MouseTipFuyuBasicBonus : MouseTipFuyuPropertyGrid
	{
		// Token: 0x060067A6 RID: 26534 RVA: 0x002F5C7C File Offset: 0x002F3E7C
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
