using System;
using FrameWork.Tools;
using UnityEngine;

namespace AiEditor
{
	// Token: 0x02000679 RID: 1657
	public interface ISelectAndDragComponent
	{
		// Token: 0x06004E60 RID: 20064 RVA: 0x0024D69D File Offset: 0x0024B89D
		void PointerEnter()
		{
		}

		// Token: 0x06004E61 RID: 20065 RVA: 0x0024D6A0 File Offset: 0x0024B8A0
		void PointerExit()
		{
		}

		// Token: 0x1700098C RID: 2444
		// (get) Token: 0x06004E62 RID: 20066
		RectTransform RectTransform { get; }

		// Token: 0x06004E63 RID: 20067 RVA: 0x0024D6A4 File Offset: 0x0024B8A4
		bool OverlapsMouse()
		{
			Camera uiCamera = UIManager.Instance.UiCamera;
			Vector3 mousePos = Input.mousePosition;
			return RectTransformUtility.RectangleContainsScreenPoint(this.RectTransform, mousePos, uiCamera);
		}

		// Token: 0x06004E64 RID: 20068 RVA: 0x0024D6D9 File Offset: 0x0024B8D9
		bool Overlaps(Rect rect)
		{
			return rect.Overlaps(this.RectTransform.ToRectInParent());
		}
	}
}
