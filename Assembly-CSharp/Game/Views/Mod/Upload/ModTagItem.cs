using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Mod.Upload
{
	// Token: 0x020008D8 RID: 2264
	public class ModTagItem : MonoBehaviour
	{
		// Token: 0x06006C6B RID: 27755 RVA: 0x0031FA4F File Offset: 0x0031DC4F
		public void Refresh(string title, Action onRemove)
		{
			this.textTitle.SetText(title, true);
			this.buttonRemove.ClearAndAddListener(onRemove);
		}

		// Token: 0x04004EAA RID: 20138
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x04004EAB RID: 20139
		[SerializeField]
		private CButton buttonRemove;
	}
}
