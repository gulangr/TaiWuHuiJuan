using System;
using TMPro;
using UnityEngine;

// Token: 0x02000308 RID: 776
public class BubbleHeightCalc
{
	// Token: 0x06002DD1 RID: 11729 RVA: 0x0016AC28 File Offset: 0x00168E28
	public BubbleHeightCalc(TextMeshProUGUI textMeshProUGUI, RectTransform obj)
	{
		this.rectTransform = obj;
		this.textMeshPro = textMeshProUGUI;
		this.singleBackGroundHeight = obj.rect.height;
		this.textInitialHeight = this.textMeshPro.rectTransform.rect.height;
	}

	// Token: 0x06002DD2 RID: 11730 RVA: 0x0016AC88 File Offset: 0x00168E88
	public void SetText(string textContent)
	{
		this.textMeshPro.text = textContent;
		this.preferredHeight = this.textMeshPro.preferredHeight;
		bool flag = this.preferredHeight > this.textInitialHeight;
		if (flag)
		{
			this.rect = this.rectTransform.rect;
			this.rectTransform.sizeDelta = new Vector2(this.rect.width, this.singleBackGroundHeight + (this.preferredHeight - this.textInitialHeight));
		}
		else
		{
			this.rect = this.rectTransform.rect;
			this.rectTransform.sizeDelta = new Vector2(this.rect.width, this.singleBackGroundHeight);
		}
	}

	// Token: 0x04002120 RID: 8480
	private float singleBackGroundHeight;

	// Token: 0x04002121 RID: 8481
	private float textInitialHeight;

	// Token: 0x04002122 RID: 8482
	private RectTransform rectTransform;

	// Token: 0x04002123 RID: 8483
	private float preferredHeight = 0f;

	// Token: 0x04002124 RID: 8484
	private Rect rect;

	// Token: 0x04002125 RID: 8485
	private readonly TextMeshProUGUI textMeshPro;
}
