using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200033F RID: 831
public class CommonButtonWithImage : MonoBehaviour
{
	// Token: 0x060030D0 RID: 12496 RVA: 0x0017F722 File Offset: 0x0017D922
	public void ClearAndAddListener(UnityAction call)
	{
		this.button.onClick.RemoveAllListeners();
		this.button.onClick.AddListener(call);
	}

	// Token: 0x060030D1 RID: 12497 RVA: 0x0017F748 File Offset: 0x0017D948
	public void OnHover(bool isHovering)
	{
		this.SyncTransformIfAvailable(isHovering, this.hoverRect, this.hoverTextRect);
	}

	// Token: 0x060030D2 RID: 12498 RVA: 0x0017F75E File Offset: 0x0017D95E
	public void OnSelect(bool isSelecting)
	{
		this.SyncTransformIfAvailable(isSelecting, this.selectedRect, this.selectedTextRect);
	}

	// Token: 0x060030D3 RID: 12499 RVA: 0x0017F774 File Offset: 0x0017D974
	private void SyncTransformIfAvailable(bool isOn, RectTransform baseRect, RectTransform textRect)
	{
		bool flag = !this.button.interactable;
		if (!flag)
		{
			if (isOn)
			{
				CommonButtonWithImage.SyncTransform(baseRect, this.enableRect);
				CommonButtonWithImage.SyncTransform(textRect, this.enableTextRect);
			}
			baseRect.gameObject.SetActive(isOn);
		}
	}

	// Token: 0x060030D4 RID: 12500 RVA: 0x0017F7C4 File Offset: 0x0017D9C4
	private static void SyncTransform(RectTransform from, RectTransform to)
	{
		from.anchorMin = to.anchorMin;
		from.anchorMax = to.anchorMax;
		from.pivot = to.pivot;
		from.sizeDelta = to.sizeDelta;
		from.localPosition = to.localPosition;
	}

	// Token: 0x060030D5 RID: 12501 RVA: 0x0017F814 File Offset: 0x0017DA14
	public void Refresh()
	{
		bool activeInHierarchy = this.hoverRect.gameObject.activeInHierarchy;
		if (activeInHierarchy)
		{
			this.OnHover(true);
		}
		bool activeInHierarchy2 = this.selectedRect.gameObject.activeInHierarchy;
		if (activeInHierarchy2)
		{
			this.OnSelect(true);
		}
	}

	// Token: 0x060030D6 RID: 12502 RVA: 0x0017F85C File Offset: 0x0017DA5C
	public void SetText(string text)
	{
		TMP_Text tmp_Text = this.enableText;
		TMP_Text tmp_Text2 = this.hoverText;
		TMP_Text tmp_Text3 = this.selectedText;
		this.disabledText.text = text;
		tmp_Text3.text = text;
		tmp_Text2.text = text;
		tmp_Text.text = text;
		this.enableTextRect.SetWidth(this.rect.rect.width - this.padding);
		this.enableText.ForceMeshUpdate(false, false);
		float num = this.baseHeight;
		float num2 = this.lineHeight;
		int val = 1;
		TMP_TextInfo textInfo = this.enableText.textInfo;
		float height = num + num2 * (float)Math.Max(val, (textInfo != null) ? textInfo.lineCount : 1);
		this.rect.SetHeight(height);
		this.Refresh();
	}

	// Token: 0x060030D7 RID: 12503 RVA: 0x0017F91C File Offset: 0x0017DB1C
	public void SetAllSprite(Sprite value, bool setNativeSize = true)
	{
		Image image = this.enable;
		Image image2 = this.hover;
		Image image3 = this.selected;
		this.disabled.sprite = value;
		image3.sprite = value;
		image2.sprite = value;
		image.sprite = value;
		if (setNativeSize)
		{
			this.SetNativeSize(this.enable);
			this.SetNativeSize(this.hover);
			this.SetNativeSize(this.selected);
			this.SetNativeSize(this.disabled);
		}
		this.Refresh();
	}

	// Token: 0x060030D8 RID: 12504 RVA: 0x0017F9A8 File Offset: 0x0017DBA8
	public void SetNativeSize(CImage image)
	{
		image.SetNativeSize();
		PositionFollower follower = image.gameObject.GetComponent<PositionFollower>();
		bool flag = follower != null;
		if (flag)
		{
			follower.Offset.x = image.rectTransform.rect.width / 2f;
		}
	}

	// Token: 0x060030D9 RID: 12505 RVA: 0x0017F9F8 File Offset: 0x0017DBF8
	public void Sprites(Sprite enableSprite, Sprite hoverSprite, Sprite selectedSprite, Sprite disabledSprite, bool setNativeSize = true)
	{
		this.enable.sprite = enableSprite;
		this.hover.sprite = hoverSprite;
		this.selected.sprite = selectedSprite;
		this.disabled.sprite = disabledSprite;
		if (setNativeSize)
		{
			this.enable.SetNativeSize();
			this.hover.SetNativeSize();
			this.selected.SetNativeSize();
			this.disabled.SetNativeSize();
		}
		this.Refresh();
	}

	// Token: 0x060030DA RID: 12506 RVA: 0x0017FA7A File Offset: 0x0017DC7A
	public void SetInteractive(bool isInteractive)
	{
		this.button.interactable = isInteractive;
	}

	// Token: 0x040023A0 RID: 9120
	[SerializeField]
	public float baseHeight = 36f;

	// Token: 0x040023A1 RID: 9121
	[SerializeField]
	public float lineHeight = 20f;

	// Token: 0x040023A2 RID: 9122
	[SerializeField]
	public float padding = 40f;

	// Token: 0x040023A3 RID: 9123
	[SerializeField]
	private CImage enable;

	// Token: 0x040023A4 RID: 9124
	[SerializeField]
	private CImage hover;

	// Token: 0x040023A5 RID: 9125
	[SerializeField]
	private CImage selected;

	// Token: 0x040023A6 RID: 9126
	[SerializeField]
	private CImage disabled;

	// Token: 0x040023A7 RID: 9127
	[SerializeField]
	private TMP_Text enableText;

	// Token: 0x040023A8 RID: 9128
	[SerializeField]
	private TMP_Text hoverText;

	// Token: 0x040023A9 RID: 9129
	[SerializeField]
	private TMP_Text selectedText;

	// Token: 0x040023AA RID: 9130
	[SerializeField]
	private TMP_Text disabledText;

	// Token: 0x040023AB RID: 9131
	[SerializeField]
	private RectTransform rect;

	// Token: 0x040023AC RID: 9132
	[SerializeField]
	private RectTransform enableRect;

	// Token: 0x040023AD RID: 9133
	[SerializeField]
	private RectTransform hoverRect;

	// Token: 0x040023AE RID: 9134
	[SerializeField]
	private RectTransform selectedRect;

	// Token: 0x040023AF RID: 9135
	[SerializeField]
	private RectTransform enableTextRect;

	// Token: 0x040023B0 RID: 9136
	[SerializeField]
	private RectTransform hoverTextRect;

	// Token: 0x040023B1 RID: 9137
	[SerializeField]
	private RectTransform selectedTextRect;

	// Token: 0x040023B2 RID: 9138
	[SerializeField]
	private CButton button;
}
