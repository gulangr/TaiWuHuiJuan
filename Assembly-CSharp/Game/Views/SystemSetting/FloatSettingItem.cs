using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.SystemSetting
{
	// Token: 0x0200075F RID: 1887
	public class FloatSettingItem : SettingItemBase<float>
	{
		// Token: 0x06005B4D RID: 23373 RVA: 0x002A6434 File Offset: 0x002A4634
		private void Awake()
		{
			this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderChanged));
			bool flag = this.addBtn != null;
			if (flag)
			{
				this.addBtn.onClick.AddListener(new UnityAction(this.OnAddBtnClick));
			}
			bool flag2 = this.reduceBtn != null;
			if (flag2)
			{
				this.reduceBtn.onClick.AddListener(new UnityAction(this.OnReduceBtnClick));
			}
			bool flag3 = this.valueText != null;
			if (flag3)
			{
				this.valueText.onSubmit.AddListener(new UnityAction<string>(this.OnInputSubmit));
				this.valueText.onEndEdit.AddListener(new UnityAction<string>(this.OnInputSubmit));
			}
		}

		// Token: 0x06005B4E RID: 23374 RVA: 0x002A6508 File Offset: 0x002A4708
		public override void Initialize(ISettingItemInfo info)
		{
			base.Initialize(info);
			this._attr = (info.Attribute as FloatSliderSettingAttribute);
			FloatSliderSettingAttribute attr = this._attr;
			this._min = ((attr != null) ? attr.Min : 0f);
			FloatSliderSettingAttribute attr2 = this._attr;
			this._max = ((attr2 != null) ? attr2.Max : 100f);
			FloatSliderSettingAttribute attr3 = this._attr;
			this._format = (((attr3 != null) ? attr3.Format : null) ?? "F1");
			FloatSliderSettingAttribute attr4 = this._attr;
			this._snapValues = ((attr4 != null) ? attr4.SnapValues : null);
			FloatSliderSettingAttribute attr5 = this._attr;
			this._step = ((attr5 != null) ? attr5.Step : 0.5f);
			this.slider.minValue = this._min;
			this.slider.maxValue = this._max;
			this.slider.wholeNumbers = false;
			this._value = this._typedInfo.GetValue();
			this._value = this.ClampValue(this._value);
			this.slider.value = this._value;
			this.UpdateValueText(this._value);
			this.UpdateButtonStates();
			Transform transform = this.lineHolder.transform;
			GameObject gameObject = this.lineHolder.transform.GetChild(0).gameObject;
			FloatSliderSettingAttribute attr6 = this._attr;
			CommonUtils.PrepareEnoughChildren(transform, gameObject, (attr6 != null && attr6.ShowSliderLines) ? ((int)((this._max - this._min) / this._step) - 1) : 0, null);
			for (int i = 0; i < this.lineHolder.childCount; i++)
			{
				RectTransform line = this.lineHolder.GetChild(i).GetComponent<RectTransform>();
				line.anchoredPosition = line.anchoredPosition.SetX(this.lineHolder.rect.width / ((this._max - this._min) / this._step) * (float)(i + 1));
			}
		}

		// Token: 0x06005B4F RID: 23375 RVA: 0x002A6704 File Offset: 0x002A4904
		private void UpdateButtonStates()
		{
			bool flag = this.addBtn;
			if (flag)
			{
				bool flag2 = this._snapValues != null && this._snapValues.Length != 0;
				if (flag2)
				{
					int currentIndex = this.GetCurrentSnapValueIndex();
					this.addBtn.interactable = (currentIndex < this._snapValues.Length - 1);
				}
				else
				{
					this.addBtn.interactable = (this._value < this._max);
				}
			}
			bool flag3 = this.reduceBtn;
			if (flag3)
			{
				bool flag4 = this._snapValues != null && this._snapValues.Length != 0;
				if (flag4)
				{
					int currentIndex2 = this.GetCurrentSnapValueIndex();
					this.reduceBtn.interactable = (currentIndex2 > 0);
				}
				else
				{
					this.reduceBtn.interactable = (this._value > this._min);
				}
			}
		}

		// Token: 0x06005B50 RID: 23376 RVA: 0x002A67E4 File Offset: 0x002A49E4
		private void OnSliderChanged(float value)
		{
			bool flag = this._snapValues != null && this._snapValues.Length != 0;
			if (flag)
			{
				value = this.SnapToNearest(value);
				this.slider.SetValueWithoutNotify(value);
			}
			else
			{
				value = this.SnapToStep(value);
				this.slider.SetValueWithoutNotify(value);
			}
			this._value = value;
			this.UpdateValueText(this._value);
			this.UpdateButtonStates();
			base.InvokeTypedValueChanged(this._value);
		}

		// Token: 0x06005B51 RID: 23377 RVA: 0x002A6868 File Offset: 0x002A4A68
		private void OnAddBtnClick()
		{
			bool flag = this._snapValues != null && this._snapValues.Length != 0;
			if (flag)
			{
				int currentIndex = this.GetCurrentSnapValueIndex();
				bool flag2 = currentIndex < this._snapValues.Length - 1;
				if (flag2)
				{
					this.SetValueInternal(this._snapValues[currentIndex + 1]);
				}
			}
			else
			{
				this.SetValueInternal(this._value + this.GetStep());
			}
		}

		// Token: 0x06005B52 RID: 23378 RVA: 0x002A68D4 File Offset: 0x002A4AD4
		private void OnReduceBtnClick()
		{
			bool flag = this._snapValues != null && this._snapValues.Length != 0;
			if (flag)
			{
				int currentIndex = this.GetCurrentSnapValueIndex();
				bool flag2 = currentIndex > 0;
				if (flag2)
				{
					this.SetValueInternal(this._snapValues[currentIndex - 1]);
				}
			}
			else
			{
				this.SetValueInternal(this._value - this.GetStep());
			}
		}

		// Token: 0x06005B53 RID: 23379 RVA: 0x002A6938 File Offset: 0x002A4B38
		private int GetCurrentSnapValueIndex()
		{
			bool flag = this._snapValues == null || this._snapValues.Length == 0;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				int nearestIndex = 0;
				float minDist = Mathf.Abs(this._value - this._snapValues[0]);
				for (int i = 1; i < this._snapValues.Length; i++)
				{
					float dist = Mathf.Abs(this._value - this._snapValues[i]);
					bool flag2 = dist < minDist;
					if (flag2)
					{
						minDist = dist;
						nearestIndex = i;
					}
				}
				result = nearestIndex;
			}
			return result;
		}

		// Token: 0x06005B54 RID: 23380 RVA: 0x002A69C8 File Offset: 0x002A4BC8
		private void OnInputSubmit(string input)
		{
			float newValue;
			bool flag = float.TryParse(input, out newValue);
			if (flag)
			{
				newValue = Mathf.Round(newValue * 100f) / 100f;
				this.SetValueInternal(newValue);
			}
			else
			{
				this.UpdateValueText(this._value);
			}
		}

		// Token: 0x06005B55 RID: 23381 RVA: 0x002A6A10 File Offset: 0x002A4C10
		private void SetValueInternal(float newValue)
		{
			bool flag = this._snapValues != null && this._snapValues.Length != 0;
			if (flag)
			{
				newValue = this.SnapToNearest(newValue);
			}
			else
			{
				newValue = this.SnapToStep(newValue);
			}
			newValue = this.ClampValue(newValue);
			bool flag2 = Math.Abs(newValue - this._value) < float.Epsilon;
			if (flag2)
			{
				this.UpdateValueText(this._value);
			}
			else
			{
				this._value = newValue;
				this.slider.SetValueWithoutNotify(this._value);
				this.UpdateValueText(this._value);
				this.UpdateButtonStates();
				base.InvokeTypedValueChanged(this._value);
			}
		}

		// Token: 0x06005B56 RID: 23382 RVA: 0x002A6ABC File Offset: 0x002A4CBC
		private float ClampValue(float value)
		{
			return Mathf.Clamp(value, this._min, this._max);
		}

		// Token: 0x06005B57 RID: 23383 RVA: 0x002A6AE0 File Offset: 0x002A4CE0
		private float SnapToNearest(float value)
		{
			bool flag = this._snapValues == null || this._snapValues.Length == 0;
			float result;
			if (flag)
			{
				result = value;
			}
			else
			{
				float nearest = this._snapValues[0];
				float minDist = Mathf.Abs(value - nearest);
				for (int i = 1; i < this._snapValues.Length; i++)
				{
					float dist = Mathf.Abs(value - this._snapValues[i]);
					bool flag2 = dist < minDist;
					if (flag2)
					{
						minDist = dist;
						nearest = this._snapValues[i];
					}
				}
				result = nearest;
			}
			return result;
		}

		// Token: 0x06005B58 RID: 23384 RVA: 0x002A6B70 File Offset: 0x002A4D70
		private float SnapToStep(float value)
		{
			bool flag = this._step <= 0f;
			float result;
			if (flag)
			{
				result = value;
			}
			else
			{
				result = Mathf.Round((value - this._min) / this._step) * this._step + this._min;
			}
			return result;
		}

		// Token: 0x06005B59 RID: 23385 RVA: 0x002A6BBC File Offset: 0x002A4DBC
		private float GetStep()
		{
			bool flag = this._snapValues != null && this._snapValues.Length > 1;
			float result;
			if (flag)
			{
				float minGap = float.MaxValue;
				for (int i = 1; i < this._snapValues.Length; i++)
				{
					float gap = this._snapValues[i] - this._snapValues[i - 1];
					bool flag2 = gap > 0f && gap < minGap;
					if (flag2)
					{
						minGap = gap;
					}
				}
				result = minGap;
			}
			else
			{
				result = this._step;
			}
			return result;
		}

		// Token: 0x06005B5A RID: 23386 RVA: 0x002A6C44 File Offset: 0x002A4E44
		private void UpdateValueText(float value)
		{
			bool flag = this.valueText != null;
			if (flag)
			{
				this.valueText.SetTextWithoutNotify(value.ToString(this._format));
			}
		}

		// Token: 0x06005B5B RID: 23387 RVA: 0x002A6C7D File Offset: 0x002A4E7D
		public override object GetValue()
		{
			return this._value;
		}

		// Token: 0x06005B5C RID: 23388 RVA: 0x002A6C8C File Offset: 0x002A4E8C
		public override void SetValue(object value)
		{
			float newValue = (float)value;
			this.SetValueInternal(newValue);
		}

		// Token: 0x06005B5D RID: 23389 RVA: 0x002A6CA9 File Offset: 0x002A4EA9
		public override void SetTypedValue(float value)
		{
			this.SetValueInternal(value);
		}

		// Token: 0x06005B5E RID: 23390 RVA: 0x002A6CB4 File Offset: 0x002A4EB4
		public override void SetInteractable(bool interactable)
		{
			bool flag = this.slider != null;
			if (flag)
			{
				this.slider.interactable = interactable;
			}
			bool flag2 = this.addBtn != null;
			if (flag2)
			{
				bool flag3 = this._snapValues != null && this._snapValues.Length != 0;
				if (flag3)
				{
					int currentIndex = this.GetCurrentSnapValueIndex();
					this.addBtn.interactable = (interactable && currentIndex < this._snapValues.Length - 1);
				}
				else
				{
					this.addBtn.interactable = (interactable && this._value < this._max);
				}
			}
			bool flag4 = this.reduceBtn != null;
			if (flag4)
			{
				bool flag5 = this._snapValues != null && this._snapValues.Length != 0;
				if (flag5)
				{
					int currentIndex2 = this.GetCurrentSnapValueIndex();
					this.reduceBtn.interactable = (interactable && currentIndex2 > 0);
				}
				else
				{
					this.reduceBtn.interactable = (interactable && this._value > this._min);
				}
			}
			bool flag6 = this.valueText != null;
			if (flag6)
			{
				this.valueText.interactable = interactable;
			}
		}

		// Token: 0x06005B5F RID: 23391 RVA: 0x002A6DEC File Offset: 0x002A4FEC
		private void OnDestroy()
		{
			bool flag = this.slider != null;
			if (flag)
			{
				this.slider.onValueChanged.RemoveListener(new UnityAction<float>(this.OnSliderChanged));
			}
			bool flag2 = this.addBtn != null;
			if (flag2)
			{
				this.addBtn.onClick.RemoveListener(new UnityAction(this.OnAddBtnClick));
			}
			bool flag3 = this.reduceBtn != null;
			if (flag3)
			{
				this.reduceBtn.onClick.RemoveListener(new UnityAction(this.OnReduceBtnClick));
			}
			bool flag4 = this.valueText != null;
			if (flag4)
			{
				this.valueText.onSubmit.RemoveListener(new UnityAction<string>(this.OnInputSubmit));
				this.valueText.onEndEdit.RemoveListener(new UnityAction<string>(this.OnInputSubmit));
			}
		}

		// Token: 0x04003EF8 RID: 16120
		[SerializeField]
		private CSlider slider;

		// Token: 0x04003EF9 RID: 16121
		[SerializeField]
		private CButton addBtn;

		// Token: 0x04003EFA RID: 16122
		[SerializeField]
		private CButton reduceBtn;

		// Token: 0x04003EFB RID: 16123
		[SerializeField]
		private TMP_InputField valueText;

		// Token: 0x04003EFC RID: 16124
		[SerializeField]
		private RectTransform lineHolder;

		// Token: 0x04003EFD RID: 16125
		private float _value;

		// Token: 0x04003EFE RID: 16126
		private float _min;

		// Token: 0x04003EFF RID: 16127
		private float _max;

		// Token: 0x04003F00 RID: 16128
		private string _format;

		// Token: 0x04003F01 RID: 16129
		private FloatSliderSettingAttribute _attr;

		// Token: 0x04003F02 RID: 16130
		private float[] _snapValues;

		// Token: 0x04003F03 RID: 16131
		private float _step;
	}
}
