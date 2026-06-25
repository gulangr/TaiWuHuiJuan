using System;
using FrameWork;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002D6 RID: 726
public class MouseTipSingleDesc : MouseTipBase
{
	// Token: 0x06002B47 RID: 11079 RVA: 0x00151BE8 File Offset: 0x0014FDE8
	public override bool CanShowWithArgumentBox(ArgumentBox argumentBox)
	{
		string text;
		return argumentBox.Get("arg0", out text);
	}

	// Token: 0x170004BC RID: 1212
	// (get) Token: 0x06002B48 RID: 11080 RVA: 0x00151C07 File Offset: 0x0014FE07
	protected override bool CanStick
	{
		get
		{
			return this._canStick;
		}
	}

	// Token: 0x06002B49 RID: 11081 RVA: 0x00151C10 File Offset: 0x0014FE10
	protected override void Init(ArgumentBox argsBox)
	{
		TextMeshProUGUI textTips = base.CGet<TextMeshProUGUI>("TipContent");
		string content;
		argsBox.Get("arg0", out content);
		bool canStick;
		this._canStick = (!argsBox.Get("CanStick", out canStick) || canStick);
		textTips.text = content.ColorReplace();
		textTips.Rebuild(CanvasUpdate.Prelayout);
		TMPTextSpriteHelper tmpTextSpriteHelper = textTips.GetComponent<TMPTextSpriteHelper>();
		Enum sizeFitType;
		tmpTextSpriteHelper.SpriteSizeFitType = (argsBox.Get("IconSizeFitType", out sizeFitType) ? ((TMPTextSpriteHelper.SizeFitType)sizeFitType) : TMPTextSpriteHelper.SizeFitType.Auto);
		float offsetX;
		float offsetY;
		tmpTextSpriteHelper.Offset = new Vector2(argsBox.Get("IconOffsetX", out offsetX) ? offsetX : 0f, argsBox.Get("IconOffsetY", out offsetY) ? offsetY : 0f);
		tmpTextSpriteHelper.Parse();
		float argWidth;
		Vector2 size = textTips.rectTransform.sizeDelta.SetX(Mathf.Min(argsBox.Get("Width", out argWidth) ? argWidth : textTips.preferredWidth, this.MaxWidth));
		textTips.rectTransform.SetSize(size);
		LayoutRebuilder.MarkLayoutForRebuild(textTips.rectTransform);
	}

	// Token: 0x06002B4A RID: 11082 RVA: 0x00151D1E File Offset: 0x0014FF1E
	public override void Refresh(ArgumentBox argBox)
	{
		this.Init(argBox);
	}

	// Token: 0x06002B4B RID: 11083 RVA: 0x00151D29 File Offset: 0x0014FF29
	public override void UpdateOffsetPos()
	{
	}

	// Token: 0x04001F6A RID: 8042
	private bool _canStick;

	// Token: 0x04001F6B RID: 8043
	private const bool DefaultCanStick = true;

	// Token: 0x04001F6C RID: 8044
	public float MaxWidth;

	// Token: 0x04001F6D RID: 8045
	private const TMPTextSpriteHelper.SizeFitType DefaultSizeFitType = TMPTextSpriteHelper.SizeFitType.Auto;

	// Token: 0x04001F6E RID: 8046
	private const float DefaultOffsetX = 0f;

	// Token: 0x04001F6F RID: 8047
	private const float DefaultOffsetY = 0f;
}
