using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000763 RID: 1891
	public class IntSettingItem : SettingItemBase
	{
		// Token: 0x06005B76 RID: 23414 RVA: 0x002A7634 File Offset: 0x002A5834
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

		// Token: 0x06005B77 RID: 23415 RVA: 0x002A7708 File Offset: 0x002A5908
		public override void Initialize(ISettingItemInfo info)
		{
			base.Initialize(info);
			this._attr = (info.Attribute as IntSliderSettingAttribute);
			IntSliderSettingAttribute attr = this._attr;
			this._min = ((attr != null) ? attr.Min : 0);
			IntSliderSettingAttribute attr2 = this._attr;
			this._max = ((attr2 != null) ? attr2.Max : 100);
			IntSliderSettingAttribute attr3 = this._attr;
			this._step = ((attr3 != null) ? attr3.Step : 1);
			bool flag = this._step < 1;
			if (flag)
			{
				this._step = 1;
			}
			this.slider.minValue = (float)this._min;
			this.slider.maxValue = (float)this._max;
			this.slider.wholeNumbers = true;
			bool flag2 = info.PropertyType == typeof(sbyte);
			if (flag2)
			{
				SettingItemInfo<sbyte> typedInfo = (SettingItemInfo<sbyte>)info;
				this._getValueAction = (() => (int)typedInfo.GetValue());
				this._setValueAction = delegate(int v)
				{
					typedInfo.SetValue((sbyte)v);
				};
				this._min = Math.Max(this._min, -128);
				this._max = Math.Min(this._max, 127);
			}
			else
			{
				SettingItemInfo<int> typedInfo = (SettingItemInfo<int>)info;
				this._getValueAction = (() => typedInfo.GetValue());
				this._setValueAction = delegate(int v)
				{
					typedInfo.SetValue(v);
				};
			}
			this._value = this._getValueAction();
			this._value = this.ClampValue(this._value);
			this.slider.value = (float)this._value;
			Transform transform = this.lineHolder.transform;
			GameObject gameObject = this.lineHolder.transform.GetChild(0).gameObject;
			IntSliderSettingAttribute attr4 = this._attr;
			CommonUtils.PrepareEnoughChildren(transform, gameObject, (attr4 != null && attr4.ShowSliderLines) ? ((this._max - this._min) / this._step - 1) : 0, null);
			for (int i = 0; i < this.lineHolder.childCount; i++)
			{
				RectTransform line = this.lineHolder.GetChild(i).GetComponent<RectTransform>();
				line.anchoredPosition = line.anchoredPosition.SetX(this.lineHolder.rect.width / (float)((this._max - this._min) / this._step) * (float)(i + 1));
			}
			this.UpdateValueText(this._value);
			this.UpdateButtonStates();
			this.SetMouseTip(info);
		}

		// Token: 0x06005B78 RID: 23416 RVA: 0x002A799C File Offset: 0x002A5B9C
		private void UpdateButtonStates()
		{
			bool flag = this.addBtn != null;
			if (flag)
			{
				this.addBtn.interactable = (this._value < this._max);
			}
			bool flag2 = this.reduceBtn != null;
			if (flag2)
			{
				this.reduceBtn.interactable = (this._value > this._min);
			}
		}

		// Token: 0x06005B79 RID: 23417 RVA: 0x002A7A04 File Offset: 0x002A5C04
		private void OnSliderChanged(float value)
		{
			bool flag = this._step > 1;
			if (flag)
			{
				value = (float)this.SnapToStep((int)value);
			}
			this._value = (int)value;
			this.slider.SetValueWithoutNotify((float)this._value);
			this.UpdateValueText(this._value);
			this.UpdateButtonStates();
			this._setValueAction(this._value);
			base.NotifyChanged();
		}

		// Token: 0x06005B7A RID: 23418 RVA: 0x002A7A74 File Offset: 0x002A5C74
		private void OnAddBtnClick()
		{
			this.SetValueInternal(this._value + this._step);
		}

		// Token: 0x06005B7B RID: 23419 RVA: 0x002A7A8B File Offset: 0x002A5C8B
		private void OnReduceBtnClick()
		{
			this.SetValueInternal(this._value - this._step);
		}

		// Token: 0x06005B7C RID: 23420 RVA: 0x002A7AA4 File Offset: 0x002A5CA4
		private void OnInputSubmit(string input)
		{
			int newValue;
			bool flag = int.TryParse(input, out newValue);
			if (flag)
			{
				this.SetValueInternal(newValue);
			}
			else
			{
				this.UpdateValueText(this._value);
			}
		}

		// Token: 0x06005B7D RID: 23421 RVA: 0x002A7ADC File Offset: 0x002A5CDC
		private void SetValueInternal(int newValue)
		{
			bool flag = this._step > 1;
			if (flag)
			{
				newValue = this.SnapToStep(newValue);
			}
			newValue = this.ClampValue(newValue);
			bool flag2 = newValue == this._value;
			if (!flag2)
			{
				this._value = newValue;
				this.slider.SetValueWithoutNotify((float)this._value);
				this.UpdateValueText(this._value);
				this.UpdateButtonStates();
				this._setValueAction(this._value);
				base.NotifyChanged();
			}
		}

		// Token: 0x06005B7E RID: 23422 RVA: 0x002A7B64 File Offset: 0x002A5D64
		private int ClampValue(int value)
		{
			return Math.Clamp(value, this._min, this._max);
		}

		// Token: 0x06005B7F RID: 23423 RVA: 0x002A7B88 File Offset: 0x002A5D88
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

		// Token: 0x06005B80 RID: 23424 RVA: 0x002A7BC4 File Offset: 0x002A5DC4
		private void UpdateValueText(int value)
		{
			bool flag = this.valueText != null;
			if (flag)
			{
				this.valueText.SetTextWithoutNotify(value.ToString());
			}
		}

		// Token: 0x06005B81 RID: 23425 RVA: 0x002A7BF7 File Offset: 0x002A5DF7
		public override object GetValue()
		{
			return this._value;
		}

		// Token: 0x06005B82 RID: 23426 RVA: 0x002A7C04 File Offset: 0x002A5E04
		public override void SetValue(object value)
		{
			int newValue = Convert.ToInt32(value);
			this.SetValueInternal(newValue);
		}

		// Token: 0x06005B83 RID: 23427 RVA: 0x002A7C21 File Offset: 0x002A5E21
		public void SetTypedValue(int value)
		{
			this.SetValueInternal(value);
		}

		// Token: 0x06005B84 RID: 23428 RVA: 0x002A7C2C File Offset: 0x002A5E2C
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
				this.addBtn.interactable = (interactable && this._value < this._max);
			}
			bool flag3 = this.reduceBtn != null;
			if (flag3)
			{
				this.reduceBtn.interactable = (interactable && this._value > this._min);
			}
			bool flag4 = this.valueText != null;
			if (flag4)
			{
				this.valueText.interactable = interactable;
			}
		}

		// Token: 0x06005B85 RID: 23429 RVA: 0x002A7CD4 File Offset: 0x002A5ED4
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

		// Token: 0x06005B86 RID: 23430 RVA: 0x002A7DB8 File Offset: 0x002A5FB8
		private void SetMouseTip(ISettingItemInfo info)
		{
			TooltipInvoker tip;
			bool flag = !this.slider.handleRect.TryGetComponent<TooltipInvoker>(out tip);
			if (flag)
			{
				tip = this.slider.handleRect.gameObject.AddComponent<TooltipInvoker>();
			}
			IntSliderSettingAttribute attribute = info.Attribute as IntSliderSettingAttribute;
			bool flag2 = attribute != null && attribute.ExtraTipLanguageKeys != null;
			if (flag2)
			{
				tip.enabled = true;
				LanguageKey[] languageKeys = attribute.ExtraTipLanguageKeys;
				tip.Type = attribute.TipType;
				bool flag3 = tip.Type == TipType.Simple;
				if (flag3)
				{
					bool flag4 = tip.PresetParam == null || tip.PresetParam.Length == 0;
					if (flag4)
					{
						tip.PresetParam = new string[2];
					}
					tip.PresetParam[0] = languageKeys[0].Tr();
					tip.PresetParam[1] = languageKeys[1].Tr();
				}
			}
			else
			{
				tip.enabled = false;
			}
		}

		// Token: 0x04003F19 RID: 16153
		[SerializeField]
		private CSlider slider;

		// Token: 0x04003F1A RID: 16154
		[SerializeField]
		private CButton addBtn;

		// Token: 0x04003F1B RID: 16155
		[SerializeField]
		private CButton reduceBtn;

		// Token: 0x04003F1C RID: 16156
		[SerializeField]
		private TMP_InputField valueText;

		// Token: 0x04003F1D RID: 16157
		[SerializeField]
		private RectTransform lineHolder;

		// Token: 0x04003F1E RID: 16158
		private int _value;

		// Token: 0x04003F1F RID: 16159
		private int _min;

		// Token: 0x04003F20 RID: 16160
		private int _max;

		// Token: 0x04003F21 RID: 16161
		private int _step;

		// Token: 0x04003F22 RID: 16162
		private IntSliderSettingAttribute _attr;

		// Token: 0x04003F23 RID: 16163
		private Action<int> _setValueAction;

		// Token: 0x04003F24 RID: 16164
		private Func<int> _getValueAction;
	}
}
