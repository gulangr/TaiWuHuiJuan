using System;
using UnityEngine;

namespace AiEditor
{
	// Token: 0x02000675 RID: 1653
	public class AiEditorMouseWheelScale : MonoBehaviour
	{
		// Token: 0x17000985 RID: 2437
		// (get) Token: 0x06004E29 RID: 20009 RVA: 0x0024C8FB File Offset: 0x0024AAFB
		private RectTransform Rect
		{
			get
			{
				return (RectTransform)base.transform;
			}
		}

		// Token: 0x06004E2A RID: 20010 RVA: 0x0024C908 File Offset: 0x0024AB08
		private void Awake()
		{
			bool flag = this.raycast == null;
			if (flag)
			{
				this.raycast = base.gameObject.GetOrAddComponent<AiEditorRaycast>();
			}
		}

		// Token: 0x06004E2B RID: 20011 RVA: 0x0024C938 File Offset: 0x0024AB38
		private void Update()
		{
			bool flag = !this.raycast.IsPointerOver(this.responseArea);
			if (!flag)
			{
				float scrollValue = Input.GetAxis("Mouse ScrollWheel");
				bool flag2 = Mathf.Approximately(scrollValue, 0f);
				if (!flag2)
				{
					bool flag3 = this.responseArea != null && !RectTransformUtility.RectangleContainsScreenPoint(this.responseArea, Input.mousePosition, UIManager.Instance.UiCamera);
					if (!flag3)
					{
						this.ScaleByWheel(scrollValue);
					}
				}
			}
		}

		// Token: 0x06004E2C RID: 20012 RVA: 0x0024C9C0 File Offset: 0x0024ABC0
		public void ScaleByWheel(float wheel)
		{
			Vector2 prevPos = UIManager.Instance.MousePosToLocalPos(this.Rect);
			float delta = this.maxScale - this.minScale;
			float oldScale = this.Rect.localScale.x;
			float newScale = Mathf.Clamp(oldScale + wheel * delta * this.speed, this.minScale, this.maxScale);
			this.Rect.localScale = Vector3.one * newScale;
			Vector2 currPos = UIManager.Instance.MousePosToLocalPos(this.Rect);
			this.Rect.anchoredPosition += (currPos - prevPos) * newScale;
		}

		// Token: 0x04003612 RID: 13842
		[SerializeField]
		private RectTransform responseArea;

		// Token: 0x04003613 RID: 13843
		[SerializeField]
		private AiEditorRaycast raycast;

		// Token: 0x04003614 RID: 13844
		[SerializeField]
		private float minScale = 0.5f;

		// Token: 0x04003615 RID: 13845
		[SerializeField]
		private float maxScale = 1f;

		// Token: 0x04003616 RID: 13846
		[SerializeField]
		public float speed = 1f;
	}
}
