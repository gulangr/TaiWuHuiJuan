using System;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C7A RID: 3194
	public class TaiwuIconRoot : MonoBehaviour
	{
		// Token: 0x0600A227 RID: 41511 RVA: 0x004BC3CC File Offset: 0x004BA5CC
		public void SetDialogPosY()
		{
			RectTransform dialogRect = this.taiwuDialog.GetComponent<RectTransform>();
			dialogRect.localPosition = dialogRect.localPosition.SetY(this.paramProgressTemplate.activeSelf ? 133f : 83f);
		}

		// Token: 0x04007E13 RID: 32275
		[SerializeField]
		public AdventureDialog taiwuDialog;

		// Token: 0x04007E14 RID: 32276
		[SerializeField]
		public CImage taiwuIcon;

		// Token: 0x04007E15 RID: 32277
		[SerializeField]
		public CImage influenceIcon;

		// Token: 0x04007E16 RID: 32278
		[SerializeField]
		public GameObject paramProgressTemplate;

		// Token: 0x04007E17 RID: 32279
		private const float DialogPosY0 = 83f;

		// Token: 0x04007E18 RID: 32280
		private const float DialogPosY1 = 133f;
	}
}
