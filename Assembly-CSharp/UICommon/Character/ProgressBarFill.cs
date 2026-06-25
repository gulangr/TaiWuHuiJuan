using System;
using DG.Tweening;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;

namespace UICommon.Character
{
	// Token: 0x020005EE RID: 1518
	public class ProgressBarFill
	{
		// Token: 0x17000907 RID: 2311
		// (get) Token: 0x060047A0 RID: 18336 RVA: 0x00218EDC File Offset: 0x002170DC
		// (set) Token: 0x060047A1 RID: 18337 RVA: 0x00218EE4 File Offset: 0x002170E4
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

		// Token: 0x060047A2 RID: 18338 RVA: 0x00218F24 File Offset: 0x00217124
		public ProgressBarFill(float maxValue, Refers refers, TextMeshProUGUI label = null, TextMeshProUGUI resistLabel = null)
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
			bool flag5 = refers.Names.Contains("NewlyProgress");
			if (flag5)
			{
				RectTransform barNewlyProgress = refers.CGet<RectTransform>("NewlyProgress");
				this._progressNewlyBar = barNewlyProgress.GetComponent<CImage>();
				this._progressNewlyBar.fillAmount = 0f;
			}
			this._progressBar = progress.GetComponent<CImage>();
			this.MaxValue = maxValue;
			this._valueLabel = label;
			this._valueResistLabel = resistLabel;
		}

		// Token: 0x060047A3 RID: 18339 RVA: 0x00219020 File Offset: 0x00217220
		public void SetInitValue(int poisonValue)
		{
			sbyte poisonLevel = PoisonsAndLevels.CalcPoisonedLevel(poisonValue);
			short maxValue = 25000;
			bool flag = GlobalConfig.Instance.PoisonLevelThresholds.CheckIndex((int)poisonLevel);
			if (flag)
			{
				maxValue = GlobalConfig.Instance.PoisonLevelThresholds[(int)poisonLevel];
			}
			this.MaxValue = (float)maxValue;
			this._value = (float)poisonValue;
			bool flag2 = this.MaxValue < 1E-05f;
			if (!flag2)
			{
				float progress = this._value / this.MaxValue;
				float targetProgress = progress;
				this._progressBar.fillAmount = targetProgress;
			}
		}

		// Token: 0x060047A4 RID: 18340 RVA: 0x002190A0 File Offset: 0x002172A0
		public void SetValueWithoutAnimation(float value)
		{
			this._value = value;
			bool flag = this.MaxValue < 1E-05f;
			if (!flag)
			{
				float progress = this._value / this.MaxValue;
				float targetProgress = progress;
				this._progressBar.fillAmount = targetProgress;
				bool flag2 = null != this._progressNewlyBar;
				if (flag2)
				{
					this._progressNewlyBar.fillAmount = targetProgress;
				}
				this.SetLabel();
			}
		}

		// Token: 0x060047A5 RID: 18341 RVA: 0x0021910C File Offset: 0x0021730C
		public void SetProgressWithNewlyValue(float oldValue, float newlyValue)
		{
			CommonUtils.TryKillTween(this._tweener, false);
			float oldStartPos = this._progressBar.fillAmount;
			float newStartPos = this._progressNewlyBar.fillAmount;
			float targetPosOld = Mathf.Clamp01(oldValue / this.MaxValue);
			float targetPosNew = Mathf.Clamp01(newlyValue / this.MaxValue);
			this.SetLabel();
			bool flag = Mathf.Approximately(Time.timeScale, 0f);
			if (flag)
			{
				this._progressBar.fillAmount = targetPosOld;
				this._progressNewlyBar.fillAmount = targetPosNew;
				this._value = newlyValue;
				this.SetLabel();
			}
			else
			{
				this._tweener = DOVirtual.Float(0f, 1f, 0.3f, delegate(float stepValue)
				{
					float x = Mathf.Lerp(oldStartPos, targetPosOld, stepValue);
					this._progressBar.fillAmount = x;
					this._value = this.MaxValue * x;
					this.SetLabel();
					x = Mathf.Lerp(newStartPos, targetPosNew, stepValue);
					this._progressNewlyBar.fillAmount = x;
				}).OnComplete(delegate
				{
					this._value = newlyValue;
					this.SetLabel();
				}).SetAutoKill(true);
			}
		}

		// Token: 0x060047A6 RID: 18342 RVA: 0x0021921C File Offset: 0x0021741C
		private void SetProgress(float progress)
		{
			CommonUtils.TryKillTween(this._tweener, false);
			bool flag = null != this._progressNewlyBar;
			if (flag)
			{
				this._progressNewlyBar.fillAmount = 0f;
			}
			float startPos = this._progressBar.fillAmount;
			this.SetLabel();
			float targetPos;
			this._tweener = DOVirtual.Float(0f, 1f, 0.3f, delegate(float stepValue)
			{
				targetPos = Mathf.Clamp01(progress);
				float x = Mathf.Lerp(startPos, targetPos, stepValue);
				this._progressBar.fillAmount = x;
				this._value = this.MaxValue * x;
				this.SetLabel();
			}).OnComplete(delegate
			{
				this._value = this.MaxValue * progress;
				this.SetLabel();
			}).SetAutoKill(true);
		}

		// Token: 0x060047A7 RID: 18343 RVA: 0x002192C4 File Offset: 0x002174C4
		private void SetLabel()
		{
			bool flag = this.GetProgressString != null;
			if (flag)
			{
				ValueTuple<string, string> tuple = this.GetProgressString(this._value);
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
					this._valueLabel.text = this._value.ToString("F2");
				}
				bool flag5 = null != this._valueResistLabel;
				if (flag5)
				{
					this._valueResistLabel.text = this.MaxValue.ToString("F2");
				}
			}
		}

		// Token: 0x04003160 RID: 12640
		private CImage _progressBar;

		// Token: 0x04003161 RID: 12641
		private CImage _progressNewlyBar;

		// Token: 0x04003162 RID: 12642
		public Func<float, ValueTuple<string, string>> GetProgressString;

		// Token: 0x04003163 RID: 12643
		private readonly TextMeshProUGUI _valueLabel;

		// Token: 0x04003164 RID: 12644
		private readonly TextMeshProUGUI _valueResistLabel;

		// Token: 0x04003165 RID: 12645
		public float MaxValue;

		// Token: 0x04003166 RID: 12646
		private Tweener _tweener;

		// Token: 0x04003167 RID: 12647
		private float _value = float.MinValue;
	}
}
