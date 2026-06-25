using System;
using FrameWork.ModSystem;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Mod
{
	// Token: 0x020008CB RID: 2251
	public class SliderSettingItem : SettingItemBase
	{
		// Token: 0x06006B5E RID: 27486 RVA: 0x003193EC File Offset: 0x003175EC
		private void Awake()
		{
			this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderChanged));
		}

		// Token: 0x06006B5F RID: 27487 RVA: 0x0031940C File Offset: 0x0031760C
		public override void Initialize(SettingEntry entry)
		{
			base.Initialize(entry);
			this._typedEntry = (SliderSetting)entry;
			this._step = this._typedEntry.StepSize;
			bool flag = this._step < 1;
			if (flag)
			{
				this._step = 1;
			}
			this.slider.minValue = (float)this._typedEntry.MinValue;
			this.slider.maxValue = (float)this._typedEntry.MaxValue;
			this.slider.wholeNumbers = true;
			this._tempValue = this._typedEntry.Value;
			this.SetWithoutNotify();
			this._isInitialized = true;
		}

		// Token: 0x06006B60 RID: 27488 RVA: 0x003194AF File Offset: 0x003176AF
		public override void SetWithoutNotify()
		{
			this.slider.SetValueWithoutNotify((float)this._tempValue);
			this.UpdateValueLabel(this._tempValue);
		}

		// Token: 0x06006B61 RID: 27489 RVA: 0x003194D4 File Offset: 0x003176D4
		private void OnSliderChanged(float value)
		{
			bool flag = !this._isInitialized;
			if (!flag)
			{
				int newValue = this.SnapToStep((int)value);
				newValue = Mathf.Clamp(newValue, this._typedEntry.MinValue, this._typedEntry.MaxValue);
				this.slider.SetValueWithoutNotify((float)newValue);
				bool changed = this._tempValue != newValue;
				this._tempValue = newValue;
				this.UpdateValueLabel(newValue);
				base.NotifyValueChanged(changed);
			}
		}

		// Token: 0x06006B62 RID: 27490 RVA: 0x0031954C File Offset: 0x0031774C
		private int SnapToStep(int value)
		{
			bool flag = this._step <= 1;
			int result;
			if (flag)
			{
				result = value;
			}
			else
			{
				result = Mathf.RoundToInt((float)value / (float)this._step) * this._step;
			}
			return result;
		}

		// Token: 0x06006B63 RID: 27491 RVA: 0x00319588 File Offset: 0x00317788
		private void UpdateValueLabel(int value)
		{
			bool flag = this.valueLabel != null;
			if (flag)
			{
				this.valueLabel.text = value.ToString();
			}
		}

		// Token: 0x06006B64 RID: 27492 RVA: 0x003195BC File Offset: 0x003177BC
		public override void SetInteractable(bool interactable)
		{
			bool flag = this.slider != null;
			if (flag)
			{
				this.slider.interactable = interactable;
			}
		}

		// Token: 0x06006B65 RID: 27493 RVA: 0x003195E8 File Offset: 0x003177E8
		public override void ApplyValue()
		{
			bool flag = this._typedEntry != null;
			if (flag)
			{
				this._typedEntry.Value = this._tempValue;
			}
		}

		// Token: 0x06006B66 RID: 27494 RVA: 0x00319618 File Offset: 0x00317818
		public override void ResetValue()
		{
			bool flag = this._typedEntry != null;
			if (flag)
			{
				this._tempValue = this._typedEntry.Value;
				this.SetWithoutNotify();
			}
		}

		// Token: 0x06006B67 RID: 27495 RVA: 0x00319650 File Offset: 0x00317850
		private void OnDestroy()
		{
			bool flag = this.slider != null;
			if (flag)
			{
				this.slider.onValueChanged.RemoveListener(new UnityAction<float>(this.OnSliderChanged));
			}
		}

		// Token: 0x04004DEC RID: 19948
		[SerializeField]
		private CSlider slider;

		// Token: 0x04004DED RID: 19949
		[SerializeField]
		private TextMeshProUGUI valueLabel;

		// Token: 0x04004DEE RID: 19950
		private SliderSetting _typedEntry;

		// Token: 0x04004DEF RID: 19951
		private int _step;

		// Token: 0x04004DF0 RID: 19952
		private bool _isInitialized;

		// Token: 0x04004DF1 RID: 19953
		private int _tempValue;
	}
}
