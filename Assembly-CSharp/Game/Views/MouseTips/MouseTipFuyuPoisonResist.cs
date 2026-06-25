using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Game.Views.MouseTips
{
	// Token: 0x02000861 RID: 2145
	public class MouseTipFuyuPoisonResist : MouseTipFuyuPropertyGrid
	{
		// Token: 0x060067B6 RID: 26550 RVA: 0x002F6054 File Offset: 0x002F4254
		public void Set([TupleElementNames(new string[]
		{
			"icon",
			"title",
			"value",
			"percent",
			"isImmune"
		})] List<ValueTuple<string, string, int, int, bool>> items)
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
					ValueTuple<string, string, int, int, bool> data = items[i];
					item.Set(data.Item1, data.Item2, data.Item3, data.Item4, data.Item5);
				}
			}
		}
	}
}
