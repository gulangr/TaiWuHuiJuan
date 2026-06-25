using System;
using TMPro;
using UnityEngine;

// Token: 0x02000097 RID: 151
[Serializable]
public class TextStyleData
{
	// Token: 0x0600054B RID: 1355 RVA: 0x00023EFC File Offset: 0x000220FC
	public void ExtractFrom(TextMeshProUGUI text)
	{
		this.fontAsset = text.font;
		this.fontSize = text.fontSize;
		this.fontStyle = text.fontStyle;
		this.sharedMaterial = text.fontSharedMaterial;
		this.fontWeight = text.fontWeight;
		this.fontColor = text.color;
		this.enableVertexGradient = text.enableVertexGradient;
		this.fontColorGradient = text.colorGradient;
		this.fontColorGradientPreset = text.colorGradientPreset;
		this.isRichText = text.richText;
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x00023F88 File Offset: 0x00022188
	public void ApplyTo(TextMeshProUGUI text)
	{
		bool flag = this.fontAsset != null;
		if (flag)
		{
			text.font = this.fontAsset;
		}
		text.fontSize = this.fontSize;
		text.fontStyle = this.fontStyle;
		bool flag2 = this.sharedMaterial != null;
		if (flag2)
		{
			text.fontSharedMaterial = this.sharedMaterial;
		}
		text.fontWeight = this.fontWeight;
		text.color = this.fontColor;
		text.enableVertexGradient = this.enableVertexGradient;
		text.colorGradient = this.fontColorGradient;
		bool flag3 = this.fontColorGradientPreset != null;
		if (flag3)
		{
			text.colorGradientPreset = this.fontColorGradientPreset;
		}
		text.richText = this.isRichText;
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x00024050 File Offset: 0x00022250
	public TextStyleData Clone()
	{
		return new TextStyleData
		{
			fontAsset = this.fontAsset,
			fontSize = this.fontSize,
			fontStyle = this.fontStyle,
			sharedMaterial = this.sharedMaterial,
			fontWeight = this.fontWeight,
			fontColor = this.fontColor,
			enableVertexGradient = this.enableVertexGradient,
			fontColorGradient = this.fontColorGradient,
			fontColorGradientPreset = this.fontColorGradientPreset,
			isRichText = this.isRichText
		};
	}

	// Token: 0x0400044B RID: 1099
	public TMP_FontAsset fontAsset;

	// Token: 0x0400044C RID: 1100
	public float fontSize;

	// Token: 0x0400044D RID: 1101
	public FontStyles fontStyle;

	// Token: 0x0400044E RID: 1102
	public Material sharedMaterial;

	// Token: 0x0400044F RID: 1103
	public FontWeight fontWeight;

	// Token: 0x04000450 RID: 1104
	public Color32 fontColor;

	// Token: 0x04000451 RID: 1105
	public bool enableVertexGradient;

	// Token: 0x04000452 RID: 1106
	public VertexGradient fontColorGradient;

	// Token: 0x04000453 RID: 1107
	public TMP_ColorGradient fontColorGradientPreset;

	// Token: 0x04000454 RID: 1108
	public bool isRichText;
}
