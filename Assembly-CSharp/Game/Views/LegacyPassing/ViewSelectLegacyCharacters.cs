using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.LegacyPassing
{
	// Token: 0x02000998 RID: 2456
	public class ViewSelectLegacyCharacters : MonoBehaviour
	{
		// Token: 0x06007651 RID: 30289 RVA: 0x00372B1C File Offset: 0x00370D1C
		private void Awake()
		{
			this.closeButton.onClick.ResetListener(new Action(this.Close));
			this._scrollRect = ((this.contentRectTransform != null) ? this.contentRectTransform.GetComponentInParent<CScrollRect>() : null);
			bool flag = this.leftButton != null;
			if (flag)
			{
				this.leftButton.ClearAndAddListener(delegate
				{
					this.ScrollContent(false);
				});
			}
			bool flag2 = this.rightButton != null;
			if (flag2)
			{
				this.rightButton.ClearAndAddListener(delegate
				{
					this.ScrollContent(true);
				});
			}
			bool flag3 = this._scrollRect != null;
			if (flag3)
			{
				this._scrollRect.OnScrollEnd = new Action(this.RefreshScrollButtons);
				this._scrollRect.OnScrollEvent += this.RefreshScrollButtons;
			}
		}

		// Token: 0x06007652 RID: 30290 RVA: 0x00372BFA File Offset: 0x00370DFA
		private void Close()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x06007653 RID: 30291 RVA: 0x00372C0C File Offset: 0x00370E0C
		public void Show()
		{
			UIManager.Instance.SetEscHandler(new Action(this.Close));
			base.gameObject.SetActive(true);
			bool flag = this.contentRectTransform != null;
			if (flag)
			{
				Vector2 pos = this.contentRectTransform.anchoredPosition;
				this.contentRectTransform.anchoredPosition = new Vector2(0f, pos.y);
				CScrollRect scrollRect = this._scrollRect;
				if (scrollRect != null)
				{
					scrollRect.UpdateScrollBarValue();
				}
			}
			this.RefreshScrollButtons();
		}

		// Token: 0x06007654 RID: 30292 RVA: 0x00372C94 File Offset: 0x00370E94
		private void ScrollContent(bool scrollForward)
		{
			bool flag = this._scrollRect == null || this.contentRectTransform == null;
			if (!flag)
			{
				this.UpdateScrollRange();
				float currentX = this.contentRectTransform.anchoredPosition.x;
				float offset = this.GetScrollStep();
				float targetX = scrollForward ? (currentX - offset) : (currentX + offset);
				targetX = Mathf.Clamp(targetX, this._maxScrollPosX, 0f);
				Vector2 pos = this.contentRectTransform.anchoredPosition;
				this._scrollRect.ScrollTo(new Vector2(targetX, pos.y), 0.3f);
			}
		}

		// Token: 0x06007655 RID: 30293 RVA: 0x00372D30 File Offset: 0x00370F30
		private void UpdateScrollRange()
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.contentRectTransform);
			this._maxScrollPosX = (this.contentRectTransform.rect.width - this._scrollRect.Viewport.rect.width) * -1f;
		}

		// Token: 0x06007656 RID: 30294 RVA: 0x00372D84 File Offset: 0x00370F84
		private float GetScrollStep()
		{
			HorizontalLayoutGroup layout = this.contentRectTransform.GetComponent<HorizontalLayoutGroup>();
			bool flag = this.contentRectTransform.childCount == 0 || layout == null;
			float result;
			if (flag)
			{
				result = this.GetVisibleListWidth();
			}
			else
			{
				RectTransform firstChild = this.contentRectTransform.GetChild(0) as RectTransform;
				float itemStep = firstChild.rect.width + layout.spacing;
				float visibleWidth = this.GetVisibleListWidth();
				int itemsPerPage = Mathf.Max(1, Mathf.FloorToInt((visibleWidth + layout.spacing) / itemStep));
				result = itemStep * (float)itemsPerPage;
			}
			return result;
		}

		// Token: 0x06007657 RID: 30295 RVA: 0x00372E18 File Offset: 0x00371018
		private float GetVisibleListWidth()
		{
			RectTransform viewport = this._scrollRect.Viewport;
			float blockedWidth = 0f;
			for (int i = 0; i < viewport.childCount; i++)
			{
				Transform child = viewport.GetChild(i);
				bool flag = child == this.contentRectTransform;
				if (!flag)
				{
					RectTransform rt = child as RectTransform;
					bool flag2 = rt != null;
					if (flag2)
					{
						blockedWidth = Mathf.Max(blockedWidth, rt.rect.width * rt.localScale.x);
					}
				}
			}
			return Mathf.Max(0f, viewport.rect.width - blockedWidth);
		}

		// Token: 0x06007658 RID: 30296 RVA: 0x00372EC8 File Offset: 0x003710C8
		public void RefreshScrollButtons()
		{
			bool flag = this.leftButton == null || this.rightButton == null || this.contentRectTransform == null || this._scrollRect == null;
			if (!flag)
			{
				this.UpdateScrollRange();
				bool flag2 = this.contentRectTransform.rect.width <= this._scrollRect.Viewport.rect.width;
				if (flag2)
				{
					this.leftButton.interactable = false;
					this.rightButton.interactable = false;
				}
				else
				{
					float currentX = this.contentRectTransform.anchoredPosition.x;
					this.leftButton.interactable = (currentX < -1f);
					this.rightButton.interactable = (currentX > this._maxScrollPosX + 1f);
				}
			}
		}

		// Token: 0x04005939 RID: 22841
		[SerializeField]
		private CButton closeButton;

		// Token: 0x0400593A RID: 22842
		[SerializeField]
		private CButton leftButton;

		// Token: 0x0400593B RID: 22843
		[SerializeField]
		private CButton rightButton;

		// Token: 0x0400593C RID: 22844
		[SerializeField]
		private RectTransform contentRectTransform;

		// Token: 0x0400593D RID: 22845
		private CScrollRect _scrollRect;

		// Token: 0x0400593E RID: 22846
		private float _maxScrollPosX;
	}
}
