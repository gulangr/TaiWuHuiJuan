using System;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C2E RID: 3118
	public class BottomProfession : MonoBehaviour
	{
		// Token: 0x06009E6D RID: 40557 RVA: 0x004A1505 File Offset: 0x0049F705
		public void Init(UIBase uiBase)
		{
			this.parent = uiBase;
		}

		// Token: 0x04007A85 RID: 31365
		[SerializeField]
		private UIBase parent;
	}
}
