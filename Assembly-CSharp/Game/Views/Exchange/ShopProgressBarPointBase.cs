using System;
using UnityEngine;

namespace Game.Views.Exchange
{
	// Token: 0x02000A2B RID: 2603
	internal class ShopProgressBarPointBase : MonoBehaviour
	{
		// Token: 0x06007F7E RID: 32638 RVA: 0x003B6060 File Offset: 0x003B4260
		public void Set(int progress)
		{
			bool activated = progress >= this.threshold;
			this.point.sprite = (activated ? this.active : this.inactive);
			bool flag = this.activeToggle;
			if (flag)
			{
				this.activeToggle.Set(activated);
			}
			this.displayer.enabled = !activated;
		}

		// Token: 0x040061D4 RID: 25044
		[SerializeField]
		internal int threshold;

		// Token: 0x040061D5 RID: 25045
		[SerializeField]
		internal RectTransform rectTransform;

		// Token: 0x040061D6 RID: 25046
		[SerializeField]
		private CImage point;

		// Token: 0x040061D7 RID: 25047
		[SerializeField]
		private Sprite active;

		// Token: 0x040061D8 RID: 25048
		[SerializeField]
		private Sprite activeLocked;

		// Token: 0x040061D9 RID: 25049
		[SerializeField]
		private Sprite inactive;

		// Token: 0x040061DA RID: 25050
		[SerializeField]
		private ShopToggle activeToggle;

		// Token: 0x040061DB RID: 25051
		[SerializeField]
		private TooltipInvoker displayer;
	}
}
