using System;
using DG.Tweening;
using UnityEngine;

namespace FrameWork.UISystem.Components.ConchShipLayout
{
	// Token: 0x02001030 RID: 4144
	public class LayoutItem : MonoBehaviour
	{
		// Token: 0x17001557 RID: 5463
		// (get) Token: 0x0600BD68 RID: 48488 RVA: 0x00560C68 File Offset: 0x0055EE68
		public float LayoutWidth
		{
			get
			{
				bool flag = null == this.RectTransform;
				if (flag)
				{
					this.Awake();
				}
				return this.AutoSize ? this.RectTransform.rect.size.x : this._layoutSize.x;
			}
		}

		// Token: 0x17001558 RID: 5464
		// (get) Token: 0x0600BD69 RID: 48489 RVA: 0x00560CC0 File Offset: 0x0055EEC0
		public float LayoutHeight
		{
			get
			{
				bool flag = null == this.RectTransform;
				if (flag)
				{
					this.Awake();
				}
				return this.AutoSize ? this.RectTransform.rect.size.y : this._layoutSize.y;
			}
		}

		// Token: 0x17001559 RID: 5465
		// (get) Token: 0x0600BD6A RID: 48490 RVA: 0x00560D16 File Offset: 0x0055EF16
		public float PivotWidth
		{
			get
			{
				return this.LayoutWidth * this.RectTransform.pivot.x;
			}
		}

		// Token: 0x1700155A RID: 5466
		// (get) Token: 0x0600BD6B RID: 48491 RVA: 0x00560D2F File Offset: 0x0055EF2F
		public float PivotHeight
		{
			get
			{
				return this.LayoutHeight * (1f - this.RectTransform.pivot.y);
			}
		}

		// Token: 0x0600BD6C RID: 48492 RVA: 0x00560D50 File Offset: 0x0055EF50
		private void Awake()
		{
			this.RectTransform = base.GetComponent<RectTransform>();
			this.RectTransform.anchorMin = this.RectTransform.anchorMin.SetX(0f);
			this.RectTransform.anchorMax = this.RectTransform.anchorMax.SetX(0f);
		}

		// Token: 0x0600BD6D RID: 48493 RVA: 0x00560DAC File Offset: 0x0055EFAC
		public void SetX(float x)
		{
			this.RectTransform.anchoredPosition = this.RectTransform.anchoredPosition.SetX(x);
		}

		// Token: 0x0600BD6E RID: 48494 RVA: 0x00560DCC File Offset: 0x0055EFCC
		public Tweener AnimToX(float x, float duration, TweenCallback onUpdate = null)
		{
			Tweener tween = this.RectTransform.DOAnchorPosX(x, duration, false);
			bool flag = onUpdate != null;
			if (flag)
			{
				tween.OnUpdate(onUpdate);
			}
			tween.SetAutoKill(true);
			return tween;
		}

		// Token: 0x0600BD6F RID: 48495 RVA: 0x00560E06 File Offset: 0x0055F006
		public void SetY(float y)
		{
			this.RectTransform.anchoredPosition = this.RectTransform.anchoredPosition.SetY(y);
		}

		// Token: 0x0600BD70 RID: 48496 RVA: 0x00560E28 File Offset: 0x0055F028
		public Tweener AnimToY(float y, float duration, TweenCallback onUpdate = null)
		{
			Tweener tween = this.RectTransform.DOAnchorPosY(y, duration, false);
			bool flag = onUpdate != null;
			if (flag)
			{
				tween.OnUpdate(onUpdate);
			}
			tween.SetAutoKill(true);
			return tween;
		}

		// Token: 0x0600BD71 RID: 48497 RVA: 0x00560E64 File Offset: 0x0055F064
		public LayoutItem GetNextItemForLayout()
		{
			bool flag = null == this.NextItem;
			LayoutItem result2;
			if (flag)
			{
				result2 = null;
			}
			else
			{
				LayoutItem result = this.NextItem;
				while (null != result && result.IgnoreLayout)
				{
					result = result.GetNextItemForLayout();
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x0600BD72 RID: 48498 RVA: 0x00560EB4 File Offset: 0x0055F0B4
		public LayoutItem GetPrevItemForLayout()
		{
			bool flag = null == this.PrevItem;
			LayoutItem result2;
			if (flag)
			{
				result2 = null;
			}
			else
			{
				LayoutItem result = this.PrevItem;
				while (null != result && result.IgnoreLayout)
				{
					result = result.GetPrevItemForLayout();
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x0600BD73 RID: 48499 RVA: 0x00560F01 File Offset: 0x0055F101
		public void StopAllMove()
		{
			this.RectTransform.DOKill(false);
		}

		// Token: 0x040091C8 RID: 37320
		public bool IgnoreLayout;

		// Token: 0x040091C9 RID: 37321
		public bool AutoSize;

		// Token: 0x040091CA RID: 37322
		[SerializeField]
		private Vector2 _layoutSize;

		// Token: 0x040091CB RID: 37323
		public RectTransform RectTransform;

		// Token: 0x040091CC RID: 37324
		public LayoutItem PrevItem;

		// Token: 0x040091CD RID: 37325
		public LayoutItem NextItem;
	}
}
