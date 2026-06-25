using System;
using System.Text;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x020006F4 RID: 1780
	[DisallowMultipleComponent]
	public class SwitchDateDigitColumn : MonoBehaviour
	{
		// Token: 0x0600546C RID: 21612 RVA: 0x0027231C File Offset: 0x0027051C
		private void Awake()
		{
			bool flag = this.content != null;
			if (flag)
			{
				this._baseAnchoredY = this.content.anchoredPosition.y;
			}
		}

		// Token: 0x0600546D RID: 21613 RVA: 0x00272350 File Offset: 0x00270550
		public void ApplyVisualSettings(float height, float fontSize, Color digitColor)
		{
			bool flag = this.label == null;
			if (!flag)
			{
				bool flag2 = fontSize > 0f;
				if (flag2)
				{
					this.label.fontSize = fontSize;
				}
				this.label.color = digitColor;
				bool flag3 = height > 0f && this.viewport != null;
				if (flag3)
				{
					this.viewport.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
				}
			}
		}

		// Token: 0x0600546E RID: 21614 RVA: 0x002723C0 File Offset: 0x002705C0
		public void PrepareHidden()
		{
			bool flag = this.canvasGroup != null;
			if (flag)
			{
				this.canvasGroup.alpha = 0f;
			}
		}

		// Token: 0x0600546F RID: 21615 RVA: 0x002723F0 File Offset: 0x002705F0
		public void PrepareStrip(char targetChar, int minScrollCycles)
		{
			int targetLineIndex = SwitchDateDigitColumn.GetTargetLineIndex(targetChar, minScrollCycles);
			int startLineIndex = SwitchDateDigitColumn.GetStartLineIndex(targetChar, minScrollCycles, targetLineIndex);
			this.BuildStripText(targetChar, startLineIndex + 2);
			this.SnapToLine(startLineIndex);
			this.PrepareHidden();
		}

		// Token: 0x06005470 RID: 21616 RVA: 0x0027242C File Offset: 0x0027062C
		public Tween PlayScroll(char targetChar, float duration, Ease ease, int minScrollCycles, float overshootLineRatio, float settleDurationRatio, float digitFadeInDuration)
		{
			CommonUtils.TryKillTween(this._scrollSequence, true);
			this.PrepareStrip(targetChar, minScrollCycles);
			int targetLineIndex = SwitchDateDigitColumn.GetTargetLineIndex(targetChar, minScrollCycles);
			int startLineIndex = SwitchDateDigitColumn.GetStartLineIndex(targetChar, minScrollCycles, targetLineIndex);
			float startY = this.GetAnchoredYForLine(startLineIndex);
			float endY = this.GetAnchoredYForLine(targetLineIndex);
			this.content.anchoredPosition = new Vector2(this.content.anchoredPosition.x, startY);
			this.label.ForceMeshUpdate(false, false);
			float lineStep = SwitchDateDigitColumn.MeasureLineStep(this.label.textInfo);
			float overshootY = endY - lineStep * overshootLineRatio;
			float settleRatio = Mathf.Clamp01(settleDurationRatio);
			float mainDuration = duration * (1f - settleRatio);
			float settleDuration = duration * settleRatio;
			this._scrollSequence = DOTween.Sequence();
			TweenerCore<Vector2, Vector2, VectorOptions> moveTween = this.content.DOAnchorPosY(overshootY, mainDuration, false).SetEase(ease);
			this._scrollSequence.Append(moveTween);
			bool flag = this.canvasGroup != null;
			if (flag)
			{
				this._scrollSequence.Join(this.canvasGroup.DOFade(1f, digitFadeInDuration));
			}
			bool flag2 = settleDuration > 0f;
			if (flag2)
			{
				this._scrollSequence.Append(this.content.DOAnchorPosY(endY, settleDuration, false).SetEase(Ease.OutQuad));
			}
			else
			{
				this._scrollSequence.AppendCallback(delegate
				{
					this.SnapToLine(targetLineIndex);
				});
			}
			this._scrollSequence.OnComplete(delegate
			{
				this.SnapToLine(targetLineIndex);
			});
			this._scrollSequence.SetAutoKill(true);
			return this._scrollSequence;
		}

		// Token: 0x06005471 RID: 21617 RVA: 0x002725D4 File Offset: 0x002707D4
		private static int GetTargetLineIndex(char targetChar, int minScrollCycles)
		{
			bool flag = char.IsDigit(targetChar);
			int result;
			if (flag)
			{
				result = minScrollCycles * 10 + (int)('\t' - (targetChar - '0'));
			}
			else
			{
				result = minScrollCycles * 11 - 1;
			}
			return result;
		}

		// Token: 0x06005472 RID: 21618 RVA: 0x00272608 File Offset: 0x00270808
		private static int GetStartLineIndex(char targetChar, int minScrollCycles, int targetLineIndex)
		{
			int cycleLines = char.IsDigit(targetChar) ? 10 : 11;
			return targetLineIndex + minScrollCycles * cycleLines;
		}

		// Token: 0x06005473 RID: 21619 RVA: 0x00272630 File Offset: 0x00270830
		private void BuildStripText(char targetChar, int minLines)
		{
			int cycleLines = char.IsDigit(targetChar) ? 10 : 11;
			string unit = char.IsDigit(targetChar) ? "9\n8\n7\n6\n5\n4\n3\n2\n1\n0" : ("9\n8\n7\n6\n5\n4\n3\n2\n1\n0\n" + targetChar.ToString());
			int cycles = Mathf.Max(Mathf.CeilToInt((float)minLines / (float)cycleLines), 1);
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < cycles; i++)
			{
				bool flag = i > 0;
				if (flag)
				{
					builder.Append('\n');
				}
				builder.Append(unit);
			}
			this.label.text = builder.ToString();
		}

		// Token: 0x06005474 RID: 21620 RVA: 0x002726CB File Offset: 0x002708CB
		private void SnapToLine(int lineIndex)
		{
			this.content.anchoredPosition = new Vector2(this.content.anchoredPosition.x, this.GetAnchoredYForLine(lineIndex));
		}

		// Token: 0x06005475 RID: 21621 RVA: 0x002726F8 File Offset: 0x002708F8
		private float GetAnchoredYForLine(int lineIndex)
		{
			this.label.ForceMeshUpdate(false, false);
			TMP_TextInfo textInfo = this.label.textInfo;
			bool flag = textInfo.lineCount == 0;
			float result;
			if (flag)
			{
				result = this._baseAnchoredY;
			}
			else
			{
				lineIndex = Mathf.Clamp(lineIndex, 0, textInfo.lineCount - 1);
				float lineStep = SwitchDateDigitColumn.MeasureLineStep(textInfo);
				float viewportHeight = this.viewport.rect.height;
				result = this._baseAnchoredY + (float)lineIndex * lineStep + lineStep * 0.5f - viewportHeight * 0.5f;
			}
			return result;
		}

		// Token: 0x06005476 RID: 21622 RVA: 0x00272788 File Offset: 0x00270988
		private static float MeasureLineStep(TMP_TextInfo textInfo)
		{
			bool flag = textInfo.lineCount >= 2;
			float result;
			if (flag)
			{
				result = Mathf.Abs(textInfo.lineInfo[0].ascender - textInfo.lineInfo[1].ascender);
			}
			else
			{
				result = textInfo.lineInfo[0].lineHeight;
			}
			return result;
		}

		// Token: 0x06005477 RID: 21623 RVA: 0x002727E6 File Offset: 0x002709E6
		public void KillScroll()
		{
			CommonUtils.TryKillTween(this._scrollSequence, false);
		}

		// Token: 0x06005478 RID: 21624 RVA: 0x002727F6 File Offset: 0x002709F6
		private void OnDisable()
		{
			this.KillScroll();
		}

		// Token: 0x0400392E RID: 14638
		private const string DigitStripUnit = "9\n8\n7\n6\n5\n4\n3\n2\n1\n0";

		// Token: 0x0400392F RID: 14639
		[SerializeField]
		private RectTransform viewport;

		// Token: 0x04003930 RID: 14640
		[SerializeField]
		private RectTransform content;

		// Token: 0x04003931 RID: 14641
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x04003932 RID: 14642
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x04003933 RID: 14643
		private float _baseAnchoredY;

		// Token: 0x04003934 RID: 14644
		private Sequence _scrollSequence;
	}
}
