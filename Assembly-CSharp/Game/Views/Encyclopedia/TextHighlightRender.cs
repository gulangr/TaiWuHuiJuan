using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Game.Views.Encyclopedia
{
	// Token: 0x02000A64 RID: 2660
	public class TextHighlightRender : MonoBehaviour
	{
		// Token: 0x060082BE RID: 33470 RVA: 0x003CE6F5 File Offset: 0x003CC8F5
		public void SetText(string text)
		{
			this.newText.text = (TextHighlightRender.Marks.Replace(text, string.Empty) ?? "");
		}

		// Token: 0x060082BF RID: 33471 RVA: 0x003CE720 File Offset: 0x003CC920
		public void Init(TMP_Text text)
		{
			base.transform.SetParent(text.transform, false);
			base.transform.SetAsFirstSibling();
			this.rectTransform.anchoredPosition = new Vector2(0f, 0f);
			this.rectTransform.anchorMin = new Vector2(0f, 0f);
			this.rectTransform.anchorMax = new Vector2(1f, 1f);
			this.rectTransform.sizeDelta = new Vector2(0f, 0f);
			TextHighlightRender.CopyTMProProperties(text, this.newText, false);
			this.newText.fontSize = text.fontSize;
			this.newText.fontStyle = text.fontStyle;
			base.gameObject.SetActive(true);
			base.name = text.name + "ForHighlightFg";
		}

		// Token: 0x060082C0 RID: 33472 RVA: 0x003CE810 File Offset: 0x003CCA10
		private static void CopyTMProProperties(TMP_Text sourceText, TMP_Text targetText, bool copyMaterial)
		{
			bool flag = !sourceText || !targetText;
			if (!flag)
			{
				targetText.text = sourceText.text;
				targetText.font = sourceText.font;
				targetText.fontStyle = sourceText.fontStyle;
				targetText.fontSize = sourceText.fontSize;
				targetText.enableAutoSizing = sourceText.enableAutoSizing;
				targetText.fontSizeMin = sourceText.fontSizeMin;
				targetText.fontSizeMax = sourceText.fontSizeMax;
				targetText.color = sourceText.color;
				bool flag2 = targetText.enableVertexGradient = sourceText.enableVertexGradient;
				if (flag2)
				{
					targetText.colorGradient = sourceText.colorGradient;
				}
				targetText.alignment = sourceText.alignment;
				targetText.characterSpacing = sourceText.characterSpacing;
				targetText.wordSpacing = sourceText.wordSpacing;
				targetText.lineSpacing = sourceText.lineSpacing;
				targetText.paragraphSpacing = sourceText.paragraphSpacing;
				targetText.margin = sourceText.margin;
				targetText.enableWordWrapping = sourceText.enableWordWrapping;
				targetText.wordWrappingRatios = sourceText.wordWrappingRatios;
				targetText.overflowMode = sourceText.overflowMode;
				if (copyMaterial)
				{
					targetText.fontMaterial = sourceText.fontMaterial;
					targetText.fontSharedMaterial = sourceText.fontSharedMaterial;
				}
				targetText.richText = sourceText.richText;
				targetText.parseCtrlCharacters = sourceText.parseCtrlCharacters;
			}
		}

		// Token: 0x0400644D RID: 25677
		[SerializeField]
		internal TMP_Text newText;

		// Token: 0x0400644E RID: 25678
		[SerializeField]
		internal RectTransform rectTransform;

		// Token: 0x0400644F RID: 25679
		public static readonly Regex Marks = new Regex("<mark=[^>]+>|</mark>", RegexOptions.Compiled);
	}
}
