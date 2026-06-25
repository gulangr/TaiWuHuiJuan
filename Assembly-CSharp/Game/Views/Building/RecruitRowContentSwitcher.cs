using System;
using TMPro;
using UnityEngine;

namespace Game.Views.Building
{
	// Token: 0x02000BE9 RID: 3049
	public class RecruitRowContentSwitcher : MonoBehaviour
	{
		// Token: 0x06009A8E RID: 39566 RVA: 0x00486105 File Offset: 0x00484305
		public void SetRowContentActive(bool isContent)
		{
			this._isContent = isContent;
			this.RefreshDisplay();
		}

		// Token: 0x06009A8F RID: 39567 RVA: 0x00486116 File Offset: 0x00484316
		private void RefreshDisplay()
		{
			this.Content.SetActive(this._isContent);
			this.Empty.SetActive(!this._isContent);
		}

		// Token: 0x0400778E RID: 30606
		[SerializeField]
		private GameObject Content;

		// Token: 0x0400778F RID: 30607
		[SerializeField]
		private GameObject Empty;

		// Token: 0x04007790 RID: 30608
		public TooltipInvoker MouseTip;

		// Token: 0x04007791 RID: 30609
		public TextMeshProUGUI txtAmount;

		// Token: 0x04007792 RID: 30610
		public TextMeshProUGUI txtTitle;

		// Token: 0x04007793 RID: 30611
		private bool _isContent = true;
	}
}
