using System;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F90 RID: 3984
	public class HoverHelper : MonoBehaviour
	{
		// Token: 0x0600B734 RID: 46900 RVA: 0x00537EB2 File Offset: 0x005360B2
		public void Init()
		{
			this.pointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.hover.SetActive(true);
			});
			this.pointerTrigger.ExitEvent.ResetListener(delegate()
			{
				this.hover.SetActive(false);
			});
		}

		// Token: 0x04008E4D RID: 36429
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04008E4E RID: 36430
		[SerializeField]
		private GameObject hover;
	}
}
