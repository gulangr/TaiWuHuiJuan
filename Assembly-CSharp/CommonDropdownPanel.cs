using System;
using UnityEngine;

// Token: 0x02000342 RID: 834
[ExecuteInEditMode]
public class CommonDropdownPanel : MonoBehaviour
{
	// Token: 0x060030E5 RID: 12517 RVA: 0x0017FC20 File Offset: 0x0017DE20
	private void LateUpdate()
	{
		float currentViewportPaddingRight = (this.verticalScrollBar != null && this.verticalScrollBar.gameObject.activeSelf) ? this.viewportPaddingRight : this.paddingRightWhenHidden;
		float contentHeight = this.content.rect.height;
		float contentWidth = this.content.rect.width;
		float myHeight = contentHeight + this.scrollPaddingTop + this.scrollPaddingBottom + this.viewportPaddingTop + this.viewportPaddingBottom;
		float myWidth = contentWidth + this.scrollPaddingLeft + this.scrollPaddingRight + this.viewportPaddingLeft + currentViewportPaddingRight;
		float finalHeight = (this.maxHeight <= 0f) ? myHeight : Mathf.Min(myHeight, this.maxHeight);
		Vector2 size = new Vector2(myWidth, finalHeight);
		Vector2 oldSize = ((RectTransform)base.transform).rect.size;
		bool flag = Math.Abs(size.x - oldSize.x) > 0.01f || Math.Abs(size.y - oldSize.y) > 0.01f;
		if (flag)
		{
			((RectTransform)base.transform).SetSize(size);
		}
		Vector2 scrollViewSize = new Vector2(size.x - this.scrollPaddingLeft - this.scrollPaddingRight, size.y - this.scrollPaddingTop - this.scrollPaddingBottom);
		bool flag2 = Math.Abs(this.scrollView.rect.size.x - scrollViewSize.x) > 0.01f || Math.Abs(this.scrollView.rect.size.y - scrollViewSize.y) > 0.01f;
		if (flag2)
		{
			this.scrollView.SetSize(scrollViewSize);
			this.scrollView.anchoredPosition = new Vector2(this.scrollPaddingLeft, -this.scrollPaddingTop);
		}
		Vector2 viewPortSize = new Vector2(scrollViewSize.x - this.viewportPaddingLeft - currentViewportPaddingRight, scrollViewSize.y - this.viewportPaddingTop - this.viewportPaddingBottom);
		bool flag3 = Math.Abs(this.viewPort.rect.size.x - viewPortSize.x) > 0.01f || (double)Math.Abs(this.viewPort.rect.size.y - viewPortSize.y) > 0.01;
		if (flag3)
		{
			this.viewPort.SetSize(viewPortSize);
			this.viewPort.anchoredPosition = new Vector2(this.viewportPaddingLeft, -this.viewportPaddingTop);
		}
	}

	// Token: 0x040023BB RID: 9147
	[Header("最大高度。设为<=0代表高度由内容决定；如果存在合法的最大高度，则内容高度超过最大高度时会启用滚动条")]
	[SerializeField]
	private float maxHeight;

	// Token: 0x040023BC RID: 9148
	[Header("一些padding设置，正常不用改")]
	[SerializeField]
	private float scrollPaddingTop = 10f;

	// Token: 0x040023BD RID: 9149
	[SerializeField]
	private float scrollPaddingBottom = 10f;

	// Token: 0x040023BE RID: 9150
	[SerializeField]
	private float scrollPaddingLeft = 10f;

	// Token: 0x040023BF RID: 9151
	[SerializeField]
	private float scrollPaddingRight = 6f;

	// Token: 0x040023C0 RID: 9152
	[SerializeField]
	private float viewportPaddingTop;

	// Token: 0x040023C1 RID: 9153
	[SerializeField]
	private float viewportPaddingBottom;

	// Token: 0x040023C2 RID: 9154
	[SerializeField]
	private float viewportPaddingLeft;

	// Token: 0x040023C3 RID: 9155
	[SerializeField]
	private float viewportPaddingRight = 34f;

	// Token: 0x040023C4 RID: 9156
	[Header("滚动条隐藏时右侧的padding")]
	[SerializeField]
	private float paddingRightWhenHidden = 4f;

	// Token: 0x040023C5 RID: 9157
	[SerializeField]
	private RectTransform scrollView;

	// Token: 0x040023C6 RID: 9158
	[SerializeField]
	private RectTransform viewPort;

	// Token: 0x040023C7 RID: 9159
	[SerializeField]
	private RectTransform content;

	// Token: 0x040023C8 RID: 9160
	[SerializeField]
	private RectTransform verticalScrollBar;
}
