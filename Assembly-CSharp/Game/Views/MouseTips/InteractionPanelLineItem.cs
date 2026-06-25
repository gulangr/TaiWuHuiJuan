using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000843 RID: 2115
	public class InteractionPanelLineItem : MonoBehaviour
	{
		// Token: 0x060066E9 RID: 26345 RVA: 0x002EF2A5 File Offset: 0x002ED4A5
		public void Set(string content, bool available)
		{
			this.text.text = content;
			this.disable.SetStyleEffect(!available, false);
		}

		// Token: 0x04004867 RID: 18535
		[SerializeField]
		private TMP_Text text;

		// Token: 0x04004868 RID: 18536
		[SerializeField]
		private DisableStyleRoot disable;
	}
}
