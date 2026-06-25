using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F8D RID: 3981
	[RequireComponent(typeof(CToggle))]
	public class CToggleNameAndLineHelper : MonoBehaviour
	{
		// Token: 0x170014AB RID: 5291
		// (get) Token: 0x0600B722 RID: 46882 RVA: 0x005378EE File Offset: 0x00535AEE
		public TooltipInvoker Tips
		{
			get
			{
				return this.tips;
			}
		}

		// Token: 0x0600B723 RID: 46883 RVA: 0x005378F6 File Offset: 0x00535AF6
		public void SetName(string text)
		{
			this.nameLabel.text = text;
		}

		// Token: 0x0600B724 RID: 46884 RVA: 0x00537906 File Offset: 0x00535B06
		public void SetLine(bool value)
		{
			this.line.SetActive(value);
		}

		// Token: 0x04008E3B RID: 36411
		public CToggle toggle;

		// Token: 0x04008E3C RID: 36412
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x04008E3D RID: 36413
		[SerializeField]
		private GameObject line;

		// Token: 0x04008E3E RID: 36414
		[SerializeField]
		private TooltipInvoker tips;
	}
}
