using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UICommon.Character
{
	// Token: 0x020005ED RID: 1517
	public class ProgressBar
	{
		// Token: 0x17000906 RID: 2310
		// (get) Token: 0x06004795 RID: 18325 RVA: 0x002185A3 File Offset: 0x002167A3
		// (set) Token: 0x06004796 RID: 18326 RVA: 0x002185AC File Offset: 0x002167AC
		public float Value
		{
			get
			{
				return this._value;
			}
			set
			{
				bool flag = Math.Abs(this._value - value) < 0.01f;
				if (flag)
				{
					this.SetLabel();
				}
				else
				{
					this.SetProgress(value / this.MaxValue);
				}
			}
		}

		// Token: 0x06004797 RID: 18327 RVA: 0x002185EC File Offset: 0x002167EC
		public ProgressBar(float maxValue, Refers refers, TextMeshProUGUI label = null, TextMeshProUGUI resistLabel = null)
		{
			bool flag = null == refers;
			if (flag)
			{
				throw new Exception("refers can not be null to create AttributeBar");
			}
			bool flag2 = maxValue < 1E-05f;
			if (flag2)
			{
				throw new Exception("maxValue is too small to create AttributeBar");
			}
			RectTransform rect = refers.CGet<RectTransform>("ProgressBar");
			bool flag3 = null == rect;
			if (flag3)
			{
				throw new Exception("rect can not be null to create AttributeBar");
			}
			RectTransform progress = refers.CGet<RectTransform>("Progress");
			bool flag4 = null == progress;
			if (flag4)
			{
				throw new Exception("progress can not be null to create AttributeBar");
			}
			RectTransform progressExtraUnder = refers.CGet<RectTransform>("ProgressExtraUnder");
			RectTransform progressExtraAbove = refers.CGet<RectTransform>("ProgressExtraAbove");
			this.MaxValue = maxValue;
			this._barRect = rect;
			this._barProgress = progress;
			this._valueLabel = label;
			this._valueResistLabel = resistLabel;
			this._barProgress.pivot = ProgressBar.ProgressPivot;
			this._barProgress.anchorMin = ProgressBar.ProgressAnchor;
			this._barProgress.anchorMax = ProgressBar.ProgressAnchor;
			this._barProgressExtraUnder = progressExtraUnder;
			this._barProgressExtraUnder.pivot = ProgressBar.ProgressPivot;
			this._barProgressExtraUnder.anchorMin = ProgressBar.ProgressAnchor;
			this._barProgressExtraUnder.anchorMax = ProgressBar.ProgressAnchor;
			this._barProgressExtraAbove = progressExtraAbove;
			this._barProgressExtraAbove.pivot = ProgressBar.ProgressPivot;
			this._barProgressExtraAbove.anchorMin = ProgressBar.ProgressAnchor;
			this._barProgressExtraAbove.anchorMax = ProgressBar.ProgressAnchor;
		}

		// Token: 0x06004798 RID: 18328 RVA: 0x00218774 File Offset: 0x00216974
		public ProgressBar(float maxValue, RectTransform rect, RectTransform progress, RectTransform progressExtraUnder, RectTransform progressExtraAbove, TextMeshProUGUI label = null, TextMeshProUGUI resistLabel = null)
		{
			bool flag = maxValue < 1E-05f;
			if (flag)
			{
				throw new Exception("maxValue is too small to create AttributeBar");
			}
			bool flag2 = null == rect;
			if (flag2)
			{
				throw new Exception("rect can not be null to create AttributeBar");
			}
			bool flag3 = null == progress;
			if (flag3)
			{
				throw new Exception("progress can not be null to create AttributeBar");
			}
			this.MaxValue = maxValue;
			this._barRect = rect;
			this._barProgress = progress;
			this._valueLabel = label;
			this._valueResistLabel = resistLabel;
			this._barProgress.pivot = ProgressBar.ProgressPivot;
			this._barProgress.anchorMin = ProgressBar.ProgressAnchor;
			this._barProgress.anchorMax = ProgressBar.ProgressAnchor;
			this._barProgressExtraUnder = progressExtraUnder;
			this._barProgressExtraUnder.pivot = ProgressBar.ProgressPivot;
			this._barProgressExtraUnder.anchorMin = ProgressBar.ProgressAnchor;
			this._barProgressExtraUnder.anchorMax = ProgressBar.ProgressAnchor;
			this._barProgressExtraAbove = progressExtraAbove;
			this._barProgressExtraAbove.pivot = ProgressBar.ProgressPivot;
			this._barProgressExtraAbove.anchorMin = ProgressBar.ProgressAnchor;
			this._barProgressExtraAbove.anchorMax = ProgressBar.ProgressAnchor;
		}

		// Token: 0x06004799 RID: 18329 RVA: 0x002188B0 File Offset: 0x00216AB0
		public void SetValueWithoutAnimation(float value)
		{
			this._value = value;
			bool flag = this.MaxValue < 1E-05f;
			if (!flag)
			{
				float progress = this._value / this.MaxValue;
				float targetPos = this._barRect.rect.width * progress;
				this._barProgress.anchoredPosition = this._barProgress.anchoredPosition.SetX(targetPos);
				bool flag2 = this.ExtraValue > 0f;
				if (flag2)
				{
					float extraProgress = this.ExtraValue / this.MaxValue;
					bool flag3 = this._value >= this.MaxValue;
					if (flag3)
					{
						float extraTargetPos = this._barRect.rect.width * extraProgress;
						this._barProgressExtraAbove.anchoredPosition = this._barProgressExtraAbove.anchoredPosition.SetX(extraTargetPos);
						this._barProgressExtraUnder.anchoredPosition = this._barProgressExtraUnder.anchoredPosition.SetX(this._barRect.rect.width * 2f);
					}
					else
					{
						float extraTargetPos2 = targetPos + this._barRect.rect.width * extraProgress;
						this._barProgressExtraUnder.anchoredPosition = this._barProgressExtraUnder.anchoredPosition.SetX(extraTargetPos2);
						this._barProgressExtraAbove.anchoredPosition = this._barProgressExtraAbove.anchoredPosition.SetX(this._barRect.rect.width * 2f);
					}
				}
				else
				{
					bool flag4 = this.ExtraValue == 0f;
					if (flag4)
					{
						this._barProgressExtraUnder.anchoredPosition = this._barProgressExtraUnder.anchoredPosition.SetX(this._barRect.rect.width * 2f);
						this._barProgressExtraAbove.anchoredPosition = this._barProgressExtraAbove.anchoredPosition.SetX(this._barRect.rect.width * 2f);
					}
				}
				this.SetLabel();
			}
		}

		// Token: 0x0600479A RID: 18330 RVA: 0x00218AC0 File Offset: 0x00216CC0
		public void SetProgressWithNewlyValue(float oldValue, float newlyValue)
		{
			CommonUtils.TryKillTween(this._tweener, false);
			float oldStartPos = this._barProgress.anchoredPosition.x;
			float targetPosOld = this._barRect.rect.width * Mathf.Clamp01(oldValue / this.MaxValue);
			float targetPosNew = this._barRect.rect.width * Mathf.Clamp01(newlyValue / this.MaxValue);
			this._tweener = DOVirtual.Float(0f, 1f, 0.3f, delegate(float stepValue)
			{
				float x = Mathf.Lerp(oldStartPos, targetPosOld, stepValue);
				this._barProgress.anchoredPosition = this._barProgress.anchoredPosition.SetX(x);
				this._value = this.MaxValue * x / this._barRect.rect.width;
				this.SetLabel();
			}).OnComplete(delegate
			{
				this._value = newlyValue;
				this.SetLabel();
			}).SetAutoKill(true);
		}

		// Token: 0x0600479B RID: 18331 RVA: 0x00218B90 File Offset: 0x00216D90
		private void SetProgress(float progress)
		{
			CommonUtils.TryKillTween(this._tweener, false);
			float extraProgress = this.ExtraValue;
			float startPos = this._barProgress.anchoredPosition.x;
			this.ExtraValue = 0f;
			float targetPos;
			this._tweener = DOVirtual.Float(0f, 1f, 0.3f, delegate(float stepValue)
			{
				targetPos = this._barRect.rect.width * Mathf.Clamp01(progress);
				float x = Mathf.Lerp(startPos, targetPos, stepValue);
				this._barProgress.anchoredPosition = this._barProgress.anchoredPosition.SetX(x);
				this._value = this.MaxValue * x / this._barRect.rect.width;
				this.SetLabel();
			}).OnComplete(delegate
			{
				this._value = this.MaxValue * progress;
				this.SetExtraProgress(extraProgress);
				this.SetLabel();
			}).SetAutoKill(true);
		}

		// Token: 0x0600479C RID: 18332 RVA: 0x00218C2C File Offset: 0x00216E2C
		private void SetExtraProgress(float progress)
		{
			bool flag = progress == 0f;
			if (!flag)
			{
				CommonUtils.TryKillTween(this._tweener, false);
				bool flag2 = this._value >= this.MaxValue;
				float startPos;
				RectTransform barProgressExtra;
				float offset;
				if (flag2)
				{
					barProgressExtra = this._barProgressExtraAbove;
					offset = 0f;
					startPos = barProgressExtra.anchoredPosition.x;
				}
				else
				{
					barProgressExtra = this._barProgressExtraUnder;
					offset = this._barRect.rect.width * this._value / this.MaxValue;
					startPos = barProgressExtra.anchoredPosition.x;
				}
				float targetPos;
				this._tweener = DOVirtual.Float(0f, 1f, 0.3f, delegate(float stepValue)
				{
					targetPos = this._barRect.rect.width * Mathf.Clamp01(progress);
					float x = Mathf.Lerp(startPos, targetPos, stepValue);
					barProgressExtra.anchoredPosition = barProgressExtra.anchoredPosition.SetX(offset + x);
					this.ExtraValue = this.MaxValue * x / this._barRect.rect.width;
					this.SetLabel();
				}).OnComplete(delegate
				{
					this.ExtraValue = this.MaxValue * progress;
					this.SetLabel();
				}).SetAutoKill(true);
			}
		}

		// Token: 0x0600479D RID: 18333 RVA: 0x00218D40 File Offset: 0x00216F40
		private void SetLabel()
		{
			bool flag = this.GetProgressString != null;
			if (flag)
			{
				ValueTuple<string, string> tuple = this.GetProgressString(new ValueTuple<float, float>(this._value, this.ExtraValue));
				bool flag2 = null != this._valueLabel;
				if (flag2)
				{
					this._valueLabel.text = tuple.Item1;
				}
				bool flag3 = null != this._valueResistLabel;
				if (flag3)
				{
					this._valueResistLabel.text = tuple.Item2;
				}
			}
			else
			{
				bool flag4 = null != this._valueLabel;
				if (flag4)
				{
					this._valueLabel.text = ((this.ExtraValue == 0f) ? this._value.ToString("F2") : (this._value.ToString("F2") + this.GetExtraProgressString()));
				}
				bool flag5 = null != this._valueResistLabel;
				if (flag5)
				{
					this._valueResistLabel.text = this.MaxValue.ToString("F2");
				}
			}
		}

		// Token: 0x0600479E RID: 18334 RVA: 0x00218E4C File Offset: 0x0021704C
		private string GetExtraProgressString()
		{
			string value = this.ExtraValue.ToString("F2");
			return (this.ExtraValue > 0f) ? ("+" + value + "%").SetColor("brightblue") : ("-" + value + "%").SetColor("brightred");
		}

		// Token: 0x04003153 RID: 12627
		public Func<ValueTuple<float, float>, ValueTuple<string, string>> GetProgressString;

		// Token: 0x04003154 RID: 12628
		private static readonly Vector2 ProgressPivot = new Vector2(1f, 0.5f);

		// Token: 0x04003155 RID: 12629
		private static readonly Vector2 ProgressAnchor = new Vector2(0f, 0.5f);

		// Token: 0x04003156 RID: 12630
		private readonly TextMeshProUGUI _valueLabel;

		// Token: 0x04003157 RID: 12631
		private readonly TextMeshProUGUI _valueResistLabel;

		// Token: 0x04003158 RID: 12632
		private readonly RectTransform _barRect;

		// Token: 0x04003159 RID: 12633
		private readonly RectTransform _barProgress;

		// Token: 0x0400315A RID: 12634
		private readonly RectTransform _barProgressExtraUnder;

		// Token: 0x0400315B RID: 12635
		private readonly RectTransform _barProgressExtraAbove;

		// Token: 0x0400315C RID: 12636
		public float MaxValue;

		// Token: 0x0400315D RID: 12637
		private Tweener _tweener;

		// Token: 0x0400315E RID: 12638
		private float _value = float.MinValue;

		// Token: 0x0400315F RID: 12639
		public float ExtraValue = 0f;
	}
}
