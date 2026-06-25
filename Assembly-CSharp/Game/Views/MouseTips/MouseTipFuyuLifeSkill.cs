using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Game.Views.MouseTips
{
	// Token: 0x0200085F RID: 2143
	public class MouseTipFuyuLifeSkill : MouseTipFuyuPropertyGrid
	{
		// Token: 0x060067B2 RID: 26546 RVA: 0x002F5F1C File Offset: 0x002F411C
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
