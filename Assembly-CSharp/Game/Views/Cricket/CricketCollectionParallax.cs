using System;
using UnityEngine;

namespace Game.Views.Cricket
{
	// Token: 0x02000AB0 RID: 2736
	public class CricketCollectionParallax : MonoBehaviour
	{
		// Token: 0x17000EC1 RID: 3777
		// (get) Token: 0x06008658 RID: 34392 RVA: 0x003E8951 File Offset: 0x003E6B51
		public RectTransform MiddleLayer
		{
			get
			{
				return this.middleLayer;
			}
		}

		// Token: 0x17000EC2 RID: 3778
		// (get) Token: 0x06008659 RID: 34393 RVA: 0x003E8959 File Offset: 0x003E6B59
		public Vector2 MiddleAtRightPanel
		{
			get
			{
				return this.middleAtRightPanel;
			}
		}

		// Token: 0x17000EC3 RID: 3779
		// (get) Token: 0x0600865A RID: 34394 RVA: 0x003E8961 File Offset: 0x003E6B61
		public Vector2 MiddleAtLeftPanel
		{
			get
			{
				return this.middleAtLeftPanel;
			}
		}

		// Token: 0x0600865B RID: 34395 RVA: 0x003E8969 File Offset: 0x003E6B69
		private void Awake()
		{
			this.UpdateRates();
		}

		// Token: 0x0600865C RID: 34396 RVA: 0x003E8974 File Offset: 0x003E6B74
		private void UpdateRates()
		{
			this._middleTravel = this.middleAtLeftPanel - this.middleAtRightPanel;
			float mx = this._middleTravel.x;
			this._frontMoveRate = ((mx != 0f) ? ((this.frontAtLeftPanel.x - this.frontAtRightPanel.x) / mx) : 1.5f);
		}

		// Token: 0x0600865D RID: 34397 RVA: 0x003E89D4 File Offset: 0x003E6BD4
		private void Update()
		{
			bool flag = this.backLayer == null || this.middleLayer == null || this.frontLayer == null;
			if (!flag)
			{
				Vector2 offsetFromRight = new Vector2(this.middleLayer.anchoredPosition.x - this.middleAtRightPanel.x, this.middleLayer.anchoredPosition.y - this.middleAtRightPanel.y);
				this.frontLayer.anchoredPosition = new Vector2(this.frontAtRightPanel.x + offsetFromRight.x * this._frontMoveRate, this.frontAtRightPanel.y + offsetFromRight.y * this._frontMoveRate);
				float ratio = (this._middleTravel.x != 0f) ? (offsetFromRight.x / this._middleTravel.x) : 0f;
				Vector2 backOffset = this._middleTravel * this.backMoveRate * ratio;
				this.backLayer.anchoredPosition = new Vector2(this.middleAtRightPanel.x + backOffset.x, this.middleAtRightPanel.y + backOffset.y);
			}
		}

		// Token: 0x0600865E RID: 34398 RVA: 0x003E8B14 File Offset: 0x003E6D14
		public void SnapToRightPanel()
		{
			bool flag = this.middleLayer == null;
			if (!flag)
			{
				this.middleLayer.anchoredPosition = this.middleAtRightPanel;
			}
		}

		// Token: 0x0600865F RID: 34399 RVA: 0x003E8B48 File Offset: 0x003E6D48
		public void SnapToLeftPanel()
		{
			bool flag = this.middleLayer == null;
			if (!flag)
			{
				this.middleLayer.anchoredPosition = this.middleAtLeftPanel;
			}
		}

		// Token: 0x06008660 RID: 34400 RVA: 0x003E8B7C File Offset: 0x003E6D7C
		public void AddMiddleOffsetX(float deltaX)
		{
			bool flag = this.middleLayer == null;
			if (!flag)
			{
				Vector2 pos = this.middleLayer.anchoredPosition;
				pos.x += deltaX;
				this.middleLayer.anchoredPosition = pos;
			}
		}

		// Token: 0x06008661 RID: 34401 RVA: 0x003E8BC4 File Offset: 0x003E6DC4
		public void SetMiddleX(float x)
		{
			bool flag = this.middleLayer == null;
			if (!flag)
			{
				Vector2 pos = this.middleLayer.anchoredPosition;
				pos.x = x;
				this.middleLayer.anchoredPosition = pos;
			}
		}

		// Token: 0x06008662 RID: 34402 RVA: 0x003E8C05 File Offset: 0x003E6E05
		public float GetMiddleTravelX()
		{
			return this._middleTravel.x;
		}

		// Token: 0x06008663 RID: 34403 RVA: 0x003E8C14 File Offset: 0x003E6E14
		public Vector2 CalculateCenteredPosition(RectTransform target, RectTransform viewport)
		{
			float goLocalX = target.localPosition.x;
			float cw = this.middleLayer.rect.width;
			float vw = viewport.rect.width;
			float desiredX = vw * 0.5f - goLocalX;
			float minX = -(cw - vw);
			float maxX = 0f;
			float clampedX = Mathf.Clamp(desiredX, minX, maxX);
			return new Vector2(clampedX, this.middleLayer.anchoredPosition.y);
		}

		// Token: 0x0400672C RID: 26412
		[SerializeField]
		private RectTransform backLayer;

		// Token: 0x0400672D RID: 26413
		[SerializeField]
		private RectTransform middleLayer;

		// Token: 0x0400672E RID: 26414
		[SerializeField]
		private RectTransform frontLayer;

		// Token: 0x0400672F RID: 26415
		[Header("关键帧 —— 右面板 (offset=0)")]
		[SerializeField]
		private Vector2 middleAtRightPanel;

		// Token: 0x04006730 RID: 26416
		[SerializeField]
		private Vector2 frontAtRightPanel;

		// Token: 0x04006731 RID: 26417
		[Header("关键帧 —— 左面板 (offset=1)")]
		[SerializeField]
		private Vector2 middleAtLeftPanel;

		// Token: 0x04006732 RID: 26418
		[SerializeField]
		private Vector2 frontAtLeftPanel;

		// Token: 0x04006733 RID: 26419
		[Header("倍率")]
		[SerializeField]
		private float backMoveRate = 0.3f;

		// Token: 0x04006734 RID: 26420
		private float _frontMoveRate;

		// Token: 0x04006735 RID: 26421
		private Vector2 _middleTravel;
	}
}
