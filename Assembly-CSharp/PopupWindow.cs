using System;
using TMPro;
using UnityEngine;

// Token: 0x0200034B RID: 843
public class PopupWindow : MonoBehaviour
{
	// Token: 0x0600314F RID: 12623 RVA: 0x00184719 File Offset: 0x00182919
	public void OnConfirm()
	{
		Action onConfirmClick = this.OnConfirmClick;
		if (onConfirmClick != null)
		{
			onConfirmClick();
		}
	}

	// Token: 0x06003150 RID: 12624 RVA: 0x0018472E File Offset: 0x0018292E
	public void OnCancel()
	{
		Action onCancelClick = this.OnCancelClick;
		if (onCancelClick != null)
		{
			onCancelClick();
		}
	}

	// Token: 0x06003151 RID: 12625 RVA: 0x00184743 File Offset: 0x00182943
	public void SetCancelBtnShow(bool showState)
	{
		this.CancelButton.gameObject.SetActive(showState);
	}

	// Token: 0x06003152 RID: 12626 RVA: 0x00184758 File Offset: 0x00182958
	public void SetTitle(string title)
	{
		this.TitleLabel.text = title;
	}

	// Token: 0x04002416 RID: 9238
	public float TitleExpand;

	// Token: 0x04002417 RID: 9239
	public TextMeshProUGUI TitleLabel;

	// Token: 0x04002418 RID: 9240
	public CButtonObsolete ConfirmButton;

	// Token: 0x04002419 RID: 9241
	public CButtonObsolete CancelButton;

	// Token: 0x0400241A RID: 9242
	public CButtonObsolete CloseButton;

	// Token: 0x0400241B RID: 9243
	public TooltipInvoker ConfirmBtnTips;

	// Token: 0x0400241C RID: 9244
	public TooltipInvoker CancelBtnTips;

	// Token: 0x0400241D RID: 9245
	public RectTransform ElementRoot;

	// Token: 0x0400241E RID: 9246
	public Action OnConfirmClick;

	// Token: 0x0400241F RID: 9247
	public Action OnCancelClick;
}
