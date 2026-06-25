using System;
using Config;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UICommon.Character.Elements
{
	// Token: 0x020005F1 RID: 1521
	public class AttributeSlider
	{
		// Token: 0x17000908 RID: 2312
		// (get) Token: 0x060047B0 RID: 18352 RVA: 0x0021982E File Offset: 0x00217A2E
		// (set) Token: 0x060047B1 RID: 18353 RVA: 0x00219838 File Offset: 0x00217A38
		public float Value
		{
			get
			{
				return this._value;
			}
			set
			{
				CommonUtils.TryKillTween(this._tweener, true);
				this.ResetCompareInfo();
				bool flag = Math.Abs(this._value - value) > 0.01f;
				if (flag)
				{
					this.TweenTo(value);
				}
				else
				{
					this._value = value;
				}
				this.RefreshValueColor(value);
			}
		}

		// Token: 0x060047B2 RID: 18354 RVA: 0x00219890 File Offset: 0x00217A90
		public AttributeSlider(Refers refers, short propertyId, float value)
		{
			CharacterPropertyDisplayItem propertyItem = CharacterPropertyDisplay.Instance.GetItem(propertyId);
			bool flag = propertyItem == null;
			if (!flag)
			{
				this._valueUpColor = "23E6C2".HexStringToColor();
				this._valueDownColor = "A14137".HexStringToColor();
				refers.CGet<TextMeshProUGUI>("AttrName").text = propertyItem.Name;
				this._progressValueText = refers.CGet<TextMeshProUGUI>("AttrValue");
				this._progressAreaTrans = refers.CGet<RectTransform>("ProgressArea");
				this._sliderTrans = refers.CGet<RectTransform>("SliderProgress_normal");
				this._previewValueUp = refers.CGet<RectTransform>("SliderProgress_benefit");
				this._previewValueDown = refers.CGet<RectTransform>("SliderProgress_block");
				this.ResetCompareInfo();
				this.SetProgress(value);
				TooltipInvoker mouseTip = refers.GetComponent<TooltipInvoker>();
				bool flag2 = mouseTip != null;
				if (flag2)
				{
					mouseTip.PresetParam = new string[]
					{
						propertyItem.Name,
						propertyItem.Desc
					};
				}
			}
		}

		// Token: 0x060047B3 RID: 18355 RVA: 0x002199A0 File Offset: 0x00217BA0
		public void SetCompareValue(float compareValue)
		{
			bool flag = this._tweener != null && this._tweener.IsPlaying();
			if (flag)
			{
				this._tweener.Complete();
			}
			this.ResetCompareInfo();
			bool flag2 = compareValue > this._value;
			if (flag2)
			{
				this._progressValueText.color = this._valueUpColor;
				this._tweener = DOVirtual.Float(this._value, compareValue, this.TweenTime, delegate(float stepValue)
				{
					this._previewValueUp.anchoredPosition.x = Mathf.Lerp(0f, this._progressAreaTrans.rect.width, stepValue / this.MaxValue);
					this._previewValueUp.GetComponent<CImage>().fillAmount = stepValue / this.MaxValue;
					bool flag4 = this.GetShowValueString != null;
					string progressString;
					if (flag4)
					{
						progressString = this.GetShowValueString(stepValue);
					}
					else
					{
						progressString = stepValue.ToString("F2");
						bool flag5 = this.Type == AttributeSlider.ShowType.Percent;
						if (flag5)
						{
							progressString = ((int)(stepValue * 100f)).ToString();
						}
						else
						{
							bool flag6 = Mathf.Abs((float)((int)stepValue) - stepValue) <= 0.02f;
							if (flag6)
							{
								progressString = ((int)stepValue).ToString();
							}
						}
					}
					this._progressValueText.text = ((AttributeSlider.ShowType.Percent == this.Type) ? (progressString + "%") : progressString);
				}).SetAutoKill(true);
			}
			else
			{
				bool flag3 = compareValue < this._value;
				if (flag3)
				{
					this._progressValueText.color = this._valueDownColor;
					this._previewValueDown.GetComponent<CImage>().fillAmount = this._sliderTrans.GetComponent<CImage>().fillAmount;
					this._tweener = DOVirtual.Float(this._value, compareValue, this.TweenTime, delegate(float stepValue)
					{
						this._sliderTrans.anchoredPosition.x = Mathf.Lerp(0f, this._progressAreaTrans.rect.width, stepValue / this.MaxValue);
						this._sliderTrans.GetComponent<CImage>().fillAmount = stepValue / this.MaxValue;
						bool flag4 = this.GetShowValueString != null;
						string progressString;
						if (flag4)
						{
							progressString = this.GetShowValueString(stepValue);
						}
						else
						{
							progressString = stepValue.ToString("F2");
							bool flag5 = this.Type == AttributeSlider.ShowType.Percent;
							if (flag5)
							{
								progressString = ((int)(stepValue * 100f)).ToString();
							}
							else
							{
								bool flag6 = Mathf.Abs((float)((int)stepValue) - stepValue) <= 0.02f;
								if (flag6)
								{
									progressString = ((int)stepValue).ToString();
								}
							}
						}
						this._progressValueText.text = ((AttributeSlider.ShowType.Percent == this.Type) ? (progressString + "%") : progressString);
					}).SetAutoKill(true);
				}
			}
		}

		// Token: 0x060047B4 RID: 18356 RVA: 0x00219A94 File Offset: 0x00217C94
		private void TweenTo(float targetValue)
		{
			this._tweener = DOVirtual.Float(this._value, targetValue, this.TweenTime, new TweenCallback<float>(this.SetProgress)).SetAutoKill(true);
		}

		// Token: 0x060047B5 RID: 18357 RVA: 0x00219AC4 File Offset: 0x00217CC4
		private void SetProgress(float value)
		{
			this._value = value;
			this._sliderTrans.anchoredPosition.x = Mathf.Lerp(0f, this._progressAreaTrans.rect.width, value / this.MaxValue);
			this._sliderTrans.GetComponent<CImage>().fillAmount = value / this.MaxValue;
			this._progressValueText.text = this.GetProgressValueText(value);
		}

		// Token: 0x060047B6 RID: 18358 RVA: 0x00219B40 File Offset: 0x00217D40
		private string GetProgressValueText(float value)
		{
			bool flag = this.GetShowValueString != null;
			string progressString;
			if (flag)
			{
				progressString = this.GetShowValueString(value);
			}
			else
			{
				progressString = value.ToString("F2");
				bool flag2 = this.Type == AttributeSlider.ShowType.Percent;
				if (flag2)
				{
					progressString = ((int)(value * 100f)).ToString();
				}
				else
				{
					bool flag3 = Mathf.Abs((float)((int)value) - value) <= 0.02f;
					if (flag3)
					{
						progressString = ((int)value).ToString();
					}
				}
			}
			return (AttributeSlider.ShowType.Percent == this.Type) ? (progressString + "%") : progressString;
		}

		// Token: 0x060047B7 RID: 18359 RVA: 0x00219BE0 File Offset: 0x00217DE0
		private void ResetCompareInfo()
		{
			this._previewValueUp.anchoredPosition.x = 0f;
			this._previewValueUp.GetComponent<CImage>().fillAmount = 0f;
			this._previewValueDown.GetComponent<CImage>().fillAmount = 0f;
			this.SetProgress(this._value);
		}

		// Token: 0x060047B8 RID: 18360 RVA: 0x00219C40 File Offset: 0x00217E40
		private void RefreshValueColor(float value)
		{
			string color = "brightred";
			bool flag = value >= 100f;
			if (flag)
			{
				color = "pinkyellow";
			}
			bool flag2 = Math.Abs(value - this.MaxValue) < 0.02f;
			if (flag2)
			{
				color = "brightblue";
			}
			this._progressValueText.color = Colors.Instance[color];
		}

		// Token: 0x060047B9 RID: 18361 RVA: 0x00219CA0 File Offset: 0x00217EA0
		public void UpdateValue(float value, float bonus = 0f)
		{
			CommonUtils.TryKillTween(this._tweener, true);
			float startValue = this._value;
			float startBonus = this._bonusValue;
			bool flag = bonus == 0f;
			if (flag)
			{
				startBonus = 0f;
				this._bonusValue = 0f;
			}
			this._tweener = DOVirtual.Float(0f, 1f, this.TweenTime, delegate(float stepValue)
			{
				this._value = Mathf.Lerp(startValue, value, stepValue);
				this._bonusValue = Mathf.Lerp(startBonus, bonus, stepValue);
				this.SetAttributeValueText();
			}).SetAutoKill(true);
		}

		// Token: 0x060047BA RID: 18362 RVA: 0x00219D44 File Offset: 0x00217F44
		public void UpdateValueImmediately(float value, float bonus)
		{
			bool flag = bonus == 0f;
			if (flag)
			{
				this.SetAttributeValueText();
			}
			else
			{
				bool flag2 = bonus > 0f;
				if (flag2)
				{
					this._progressValueText.text = (this.GetShowValueString(value) + " <color=#lightblue>+" + this.GetShowValueString(bonus) + "</color>").ColorReplace();
				}
				else
				{
					this._progressValueText.text = (this.GetShowValueString(value) + " <color=#red>" + this.GetShowValueString(bonus) + "</color>").ColorReplace();
				}
			}
		}

		// Token: 0x060047BB RID: 18363 RVA: 0x00219DE8 File Offset: 0x00217FE8
		private void SetAttributeValueText()
		{
			string valueString = this.GetProgressValueText(this._value);
			bool flag = this._bonusValue > 0f;
			if (flag)
			{
				valueString = string.Format("{0} <color=#{1}>+{2}</color>", this._value, "lightgreen", this.GetProgressValueText(this._bonusValue));
			}
			else
			{
				bool flag2 = this._bonusValue < 0f;
				if (flag2)
				{
					valueString = string.Format("{0} <color=#{1}>{2}</color>", this._value, "red", this.GetProgressValueText(this._bonusValue));
				}
			}
			this._progressValueText.text = valueString.ColorReplace();
		}

		// Token: 0x04003173 RID: 12659
		public const float DefaultTweenTime = 0.3f;

		// Token: 0x04003174 RID: 12660
		public float TweenTime = 0.3f;

		// Token: 0x04003175 RID: 12661
		public AttributeSlider.ShowType Type;

		// Token: 0x04003176 RID: 12662
		public float MaxValue = 300f;

		// Token: 0x04003177 RID: 12663
		public Func<float, string> GetShowValueString;

		// Token: 0x04003178 RID: 12664
		private readonly RectTransform _progressAreaTrans;

		// Token: 0x04003179 RID: 12665
		private readonly RectTransform _previewValueUp;

		// Token: 0x0400317A RID: 12666
		private readonly RectTransform _previewValueDown;

		// Token: 0x0400317B RID: 12667
		private readonly RectTransform _sliderTrans;

		// Token: 0x0400317C RID: 12668
		private readonly TextMeshProUGUI _progressValueText;

		// Token: 0x0400317D RID: 12669
		private const string ValueUpTextColorString = "23E6C2";

		// Token: 0x0400317E RID: 12670
		private const string ValueDownTextColorString = "A14137";

		// Token: 0x0400317F RID: 12671
		private readonly Color _valueUpColor;

		// Token: 0x04003180 RID: 12672
		private readonly Color _valueDownColor;

		// Token: 0x04003181 RID: 12673
		private Tweener _tweener;

		// Token: 0x04003182 RID: 12674
		private float _value;

		// Token: 0x04003183 RID: 12675
		private float _bonusValue;

		// Token: 0x020019A0 RID: 6560
		public enum ShowType
		{
			// Token: 0x0400B2E7 RID: 45799
			Number,
			// Token: 0x0400B2E8 RID: 45800
			Percent
		}
	}
}
