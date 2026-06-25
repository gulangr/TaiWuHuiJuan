using System;
using Game.Views.MouseTips;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003F8 RID: 1016
public class CommonTipSimpleContentLineView : CommonAtomBase
{
	// Token: 0x06003D00 RID: 15616 RVA: 0x001EACF8 File Offset: 0x001E8EF8
	public override void SetMarginLeft(int marginLeft)
	{
		bool flag = !this._cachedPadding;
		if (flag)
		{
			RectOffset padding = this.marginLayoutGroup.padding;
			this._defaultPaddingLeft = padding.left;
			this._defaultPaddingRight = padding.right;
			this._defaultPaddingTop = padding.top;
			this._defaultPaddingBottom = padding.bottom;
			this._cachedPadding = true;
		}
		this.marginLayoutGroup.padding = new RectOffset(this._defaultPaddingLeft + marginLeft, this._defaultPaddingRight, this._defaultPaddingTop, this._defaultPaddingBottom);
	}

	// Token: 0x06003D01 RID: 15617 RVA: 0x001EAD83 File Offset: 0x001E8F83
	public void SetText(string content)
	{
		CommonAtomBase.SetLabelText(this.textLabel, content);
	}

	// Token: 0x04002BAE RID: 11182
	[SerializeField]
	private LayoutGroup marginLayoutGroup;

	// Token: 0x04002BAF RID: 11183
	[SerializeField]
	private TextMeshProUGUI textLabel;

	// Token: 0x04002BB0 RID: 11184
	private bool _cachedPadding;

	// Token: 0x04002BB1 RID: 11185
	private int _defaultPaddingLeft;

	// Token: 0x04002BB2 RID: 11186
	private int _defaultPaddingRight;

	// Token: 0x04002BB3 RID: 11187
	private int _defaultPaddingTop;

	// Token: 0x04002BB4 RID: 11188
	private int _defaultPaddingBottom;
}
