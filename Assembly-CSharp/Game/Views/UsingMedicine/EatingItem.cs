using System;
using Game.Components.Item;
using GameData.Domains.Item;
using UnityEngine;

namespace Game.Views.UsingMedicine
{
	// Token: 0x020007D1 RID: 2001
	public class EatingItem : MonoBehaviour
	{
		// Token: 0x060061CD RID: 25037 RVA: 0x002CE138 File Offset: 0x002CC338
		public void Set(ItemKey itemKey, int duration)
		{
			this.propertyCells[0].Set("ui9_icon_event_action_point_0", "", duration.ToString());
			switch (itemKey.ItemType)
			{
			}
		}

		// Token: 0x040043E0 RID: 17376
		[SerializeField]
		private ItemBack itemBack;

		// Token: 0x040043E1 RID: 17377
		[SerializeField]
		private EatingItemPropertyCell[] propertyCells = new EatingItemPropertyCell[9];

		// Token: 0x040043E2 RID: 17378
		private const int MaxPropertyCount = 9;
	}
}
