using System;
using UnityEngine;

namespace FrameWork.UISystem.Components
{
	// Token: 0x02001021 RID: 4129
	[RequireComponent(typeof(RectTransform))]
	public class ScaleKeeper : MonoBehaviour
	{
		// Token: 0x0600BD0E RID: 48398 RVA: 0x0055EB84 File Offset: 0x0055CD84
		private void LateUpdate()
		{
			bool flag = null != this.BasedOn;
			if (flag)
			{
				RectTransform rectTransform = base.GetComponent<RectTransform>();
				float keepZ = rectTransform.localScale.z;
				rectTransform.localScale = Vector3.one;
				Vector3 basedOnScale = this.BasedOn.localScale;
				Vector3 selfParentScale = rectTransform.parent.localScale;
				rectTransform.localScale = new Vector3(this.TargetScale.x / basedOnScale.x / selfParentScale.x, this.TargetScale.y / basedOnScale.y / selfParentScale.y, keepZ);
			}
		}

		// Token: 0x04009172 RID: 37234
		public RectTransform BasedOn;

		// Token: 0x04009173 RID: 37235
		public Vector2 TargetScale = Vector2.one;
	}
}
