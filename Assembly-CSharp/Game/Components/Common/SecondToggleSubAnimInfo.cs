using System;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000FA3 RID: 4003
	public class SecondToggleSubAnimInfo
	{
		// Token: 0x0600B7C9 RID: 47049 RVA: 0x0053C668 File Offset: 0x0053A868
		public void Init()
		{
			this.Cg.alpha = this.StartAlpha;
			this.RectTs.localScale = Vector3.one * this.StartScale;
			this.RectTs.anchoredPosition = this.StartAnchorPos;
			this.RectTs.gameObject.SetActive(true);
		}

		// Token: 0x04008ECD RID: 36557
		public CanvasGroup Cg;

		// Token: 0x04008ECE RID: 36558
		public RectTransform RectTs;

		// Token: 0x04008ECF RID: 36559
		public float StartAlpha = 1f;

		// Token: 0x04008ED0 RID: 36560
		public float EndAlpha = 1f;

		// Token: 0x04008ED1 RID: 36561
		public float FadeDuration = 0f;

		// Token: 0x04008ED2 RID: 36562
		public Vector2 StartAnchorPos;

		// Token: 0x04008ED3 RID: 36563
		public Vector2 EndAnchorPos;

		// Token: 0x04008ED4 RID: 36564
		public float MoveDuration = 0f;

		// Token: 0x04008ED5 RID: 36565
		public float StartScale = 1f;

		// Token: 0x04008ED6 RID: 36566
		public float EndScale = 1f;

		// Token: 0x04008ED7 RID: 36567
		public float ScaleDuration = 0f;
	}
}
