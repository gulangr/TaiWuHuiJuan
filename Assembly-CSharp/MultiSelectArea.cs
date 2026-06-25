using System;
using FrameWork.Tools;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class MultiSelectArea : MonoBehaviour
{
	// Token: 0x17000280 RID: 640
	// (get) Token: 0x060016AA RID: 5802 RVA: 0x0008B109 File Offset: 0x00089309
	private RectTransform RectTransform
	{
		get
		{
			return (RectTransform)base.transform;
		}
	}

	// Token: 0x060016AB RID: 5803 RVA: 0x0008B118 File Offset: 0x00089318
	public void Set(Vector2 posA, Vector2 posB)
	{
		posA = this.RectTransform.ScreenToLocalPointInParent(posA);
		posB = this.RectTransform.ScreenToLocalPointInParent(posB);
		Rect rect = RectTransformUtilityAdvanced.PointToRect(posA, posB);
		this.RectTransform.anchoredPosition = rect.position;
		this.RectTransform.sizeDelta = rect.size;
	}
}
