using System;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UISystem.Components
{
	// Token: 0x02001022 RID: 4130
	public class ScrollRectAutoHeightHelper : MonoBehaviour
	{
		// Token: 0x0600BD10 RID: 48400 RVA: 0x0055EC31 File Offset: 0x0055CE31
		private void LateUpdate()
		{
			this.AdjustScrollHeight();
		}

		// Token: 0x0600BD11 RID: 48401 RVA: 0x0055EC3C File Offset: 0x0055CE3C
		private void AdjustScrollHeight()
		{
			bool flag = !this.scrollRect;
			if (!flag)
			{
				float maxPixelHeight = this.maxHeight;
				RectTransform gameCanvas = base.GetComponentInParent<CanvasScaler>().GetComponent<RectTransform>();
				float canvasHeight = gameCanvas.rect.height;
				float bottomLocalY = -canvasHeight * gameCanvas.pivot.y + this.autoAdjustBottomMargin;
				Vector3 bottomWorld = gameCanvas.TransformPoint(new Vector3(0f, bottomLocalY, 0f));
				Vector3 topWorld = this.scrollRect.position;
				float allowedWorldHeight = topWorld.y - bottomWorld.y;
				float maxWorldHeight = this.scrollRect.lossyScale.y * maxPixelHeight;
				allowedWorldHeight = Mathf.Clamp(allowedWorldHeight, 0f, maxWorldHeight);
				float localHeight = allowedWorldHeight / this.scrollRect.lossyScale.y;
				this.scrollRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localHeight);
			}
		}

		// Token: 0x04009174 RID: 37236
		public RectTransform scrollRect;

		// Token: 0x04009175 RID: 37237
		public float maxHeight;

		// Token: 0x04009176 RID: 37238
		public float autoAdjustBottomMargin;
	}
}
