using System;
using UnityEngine;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x02000FFC RID: 4092
	public class PolygonChecker : MonoBehaviour
	{
		// Token: 0x0600BAC2 RID: 47810 RVA: 0x00551044 File Offset: 0x0054F244
		private void Awake()
		{
			bool flag = this.polygonCollider2D == null;
			if (flag)
			{
				this.polygonCollider2D = base.GetComponent<PolygonCollider2D>();
			}
		}

		// Token: 0x0600BAC3 RID: 47811 RVA: 0x00551070 File Offset: 0x0054F270
		private void Update()
		{
			Vector2 worldPos = UIManager.Instance.UiCamera.ScreenToWorldPoint(Input.mousePosition);
			bool hit = this.polygonCollider2D.OverlapPoint(worldPos);
			bool flag = this._isInside && !hit;
			if (flag)
			{
				this._isInside = false;
				Action onLeavePolygon = this.OnLeavePolygon;
				if (onLeavePolygon != null)
				{
					onLeavePolygon();
				}
			}
			else
			{
				bool flag2 = !this._isInside && hit;
				if (flag2)
				{
					this._isInside = true;
					Action onEnterPolygon = this.OnEnterPolygon;
					if (onEnterPolygon != null)
					{
						onEnterPolygon();
					}
				}
			}
		}

		// Token: 0x0600BAC4 RID: 47812 RVA: 0x005510FF File Offset: 0x0054F2FF
		private void OnDisable()
		{
			this._isInside = false;
		}

		// Token: 0x04009045 RID: 36933
		[SerializeField]
		private PolygonCollider2D polygonCollider2D;

		// Token: 0x04009046 RID: 36934
		public Action OnEnterPolygon;

		// Token: 0x04009047 RID: 36935
		public Action OnLeavePolygon;

		// Token: 0x04009048 RID: 36936
		private bool _isInside = false;
	}
}
