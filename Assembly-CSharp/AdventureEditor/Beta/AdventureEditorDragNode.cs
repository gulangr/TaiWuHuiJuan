using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AdventureEditor.Beta
{
	// Token: 0x020006A3 RID: 1699
	public class AdventureEditorDragNode : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
	{
		// Token: 0x06004F7B RID: 20347 RVA: 0x00252F65 File Offset: 0x00251165
		public void OnBeginDrag(PointerEventData eventData)
		{
			Action onDragBegin = this.OnDragBegin;
			if (onDragBegin != null)
			{
				onDragBegin();
			}
			this.Dragging = true;
		}

		// Token: 0x06004F7C RID: 20348 RVA: 0x00252F81 File Offset: 0x00251181
		public void OnDrag(PointerEventData eventData)
		{
			Action onDragMove = this.OnDragMove;
			if (onDragMove != null)
			{
				onDragMove();
			}
		}

		// Token: 0x06004F7D RID: 20349 RVA: 0x00252F98 File Offset: 0x00251198
		public void OnEndDrag(PointerEventData eventData)
		{
			Vector3 pos = base.transform.localPosition;
			pos.x = Mathf.Round(pos.x / this.GridSize) * this.GridSize + this.GridOffsetX;
			pos.y = Mathf.Round(pos.y / this.GridSize) * this.GridSize + this.GridOffsetY;
			base.transform.localPosition = pos;
			Action onDragEnd = this.OnDragEnd;
			if (onDragEnd != null)
			{
				onDragEnd();
			}
			this.Dragging = false;
		}

		// Token: 0x06004F7E RID: 20350 RVA: 0x00253028 File Offset: 0x00251228
		private void Update()
		{
			bool dragging = this.Dragging;
			if (dragging)
			{
				Vector2 mousePos = Input.mousePosition;
				mousePos.x = Mathf.Clamp(mousePos.x, 0f, (float)Screen.width);
				mousePos.y = Mathf.Clamp(mousePos.y, 0f, (float)Screen.height);
				Vector3 worldPos = UIManager.Instance.UiCamera.ScreenToWorldPoint(mousePos);
				Vector3 localPos = base.transform.parent.InverseTransformPoint(worldPos);
				localPos.z = 0f;
				base.transform.localPosition = localPos;
			}
		}

		// Token: 0x040036A3 RID: 13987
		public float GridSize = 128f;

		// Token: 0x040036A4 RID: 13988
		public float GridOffsetX;

		// Token: 0x040036A5 RID: 13989
		public float GridOffsetY;

		// Token: 0x040036A6 RID: 13990
		public Action OnDragBegin;

		// Token: 0x040036A7 RID: 13991
		public Action OnDragMove;

		// Token: 0x040036A8 RID: 13992
		public Action OnDragEnd;

		// Token: 0x040036A9 RID: 13993
		[ReadOnly]
		public bool Dragging;
	}
}
