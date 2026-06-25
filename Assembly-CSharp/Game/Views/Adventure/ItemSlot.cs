using System;
using FrameWork.UISystem.UIElements;
using Game.Components.Item;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C77 RID: 3191
	public class ItemSlot : MonoBehaviour
	{
		// Token: 0x0600A221 RID: 41505 RVA: 0x004BC2E8 File Offset: 0x004BA4E8
		public void SetData(ItemDisplayData displayData)
		{
			bool flag = displayData == null;
			if (flag)
			{
				this.itemBack.gameObject.SetActive(false);
				this.empty.SetActive(true);
			}
			else
			{
				this.itemBack.gameObject.SetActive(true);
				this.empty.SetActive(false);
				this.itemBack.Set(displayData, false);
			}
		}

		// Token: 0x04007E0A RID: 32266
		[SerializeField]
		public ItemBack itemBack;

		// Token: 0x04007E0B RID: 32267
		[SerializeField]
		public CButton button;

		// Token: 0x04007E0C RID: 32268
		[SerializeField]
		public TooltipInvoker mouseTip;

		// Token: 0x04007E0D RID: 32269
		[SerializeField]
		public GameObject empty;
	}
}
