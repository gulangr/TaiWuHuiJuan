using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.SystemSetting
{
	// Token: 0x02000767 RID: 1895
	public class SwitchButtonSettingItem : SettingItemBase
	{
		// Token: 0x06005BA9 RID: 23465 RVA: 0x002A886C File Offset: 0x002A6A6C
		private void Awake()
		{
			bool flag = this.leftBtn != null;
			if (flag)
			{
				this.leftBtn.onClick.AddListener(new UnityAction(this.OnLeftBtnClick));
			}
			bool flag2 = this.rightBtn != null;
			if (flag2)
			{
				this.rightBtn.onClick.AddListener(new UnityAction(this.OnRightBtnClick));
			}
		}

		// Token: 0x06005BAA RID: 23466 RVA: 0x002A88D4 File Offset: 0x002A6AD4
		public override void Initialize(ISettingItemInfo info)
		{
			base.Initialize(info);
			this._attr = (info.Attribute as SwitchButtonSettingAttribute);
			bool flag = this._attr != null;
			if (flag)
			{
				this._options = this._attr.Options;
			}
			else
			{
				ToggleGroupSettingAttribute toggleAttr = info.Attribute as ToggleGroupSettingAttribute;
				bool flag2 = toggleAttr != null;
				if (flag2)
				{
					this._options = toggleAttr.Options;
				}
			}
			bool flag3 = this._options != null && this._options.Length != 0;
			if (flag3)
			{
				this._min = 0;
				this._max = (sbyte)(this._options.Length - 1);
			}
			else
			{
				this._min = 0;
				this._max = 1;
			}
			bool flag4 = info.PropertyType == typeof(int);
			if (flag4)
			{
				SettingItemInfo<int> typedInfo = (SettingItemInfo<int>)info;
				this._getValueAction = (() => (sbyte)typedInfo.GetValue());
				this._setValueAction = delegate(sbyte v)
				{
					typedInfo.SetValue((int)v);
				};
			}
			else
			{
				bool flag5 = info.PropertyType == typeof(sbyte);
				if (!flag5)
				{
					Debug.LogError(string.Format("[SwitchButtonSettingItem] Unsupported property type: {0}", info.PropertyType));
					return;
				}
				SettingItemInfo<sbyte> typedInfo = (SettingItemInfo<sbyte>)info;
				this._getValueAction = (() => typedInfo.GetValue());
				this._setValueAction = delegate(sbyte v)
				{
					typedInfo.SetValue(v);
				};
			}
			this._value = this._getValueAction();
			this._value = this.ClampValue(this._value);
			this.UpdateValueText(this._value);
			this.UpdateButtonInteractable();
		}

		// Token: 0x06005BAB RID: 23467 RVA: 0x002A8A86 File Offset: 0x002A6C86
		private void OnLeftBtnClick()
		{
			this.SetValueInternal(this._value - 1);
		}

		// Token: 0x06005BAC RID: 23468 RVA: 0x002A8A99 File Offset: 0x002A6C99
		private void OnRightBtnClick()
		{
			this.SetValueInternal(this._value + 1);
		}

		// Token: 0x06005BAD RID: 23469 RVA: 0x002A8AAC File Offset: 0x002A6CAC
		private void SetValueInternal(sbyte newValue)
		{
			newValue = this.ClampValue(newValue);
			bool flag = newValue == this._value;
			if (!flag)
			{
				this._value = newValue;
				this.UpdateValueText(this._value);
				this.UpdateButtonInteractable();
				this._setValueAction(this._value);
				base.NotifyChanged();
			}
		}

		// Token: 0x06005BAE RID: 23470 RVA: 0x002A8B08 File Offset: 0x002A6D08
		private sbyte ClampValue(sbyte value)
		{
			bool flag = value < this._min;
			sbyte result;
			if (flag)
			{
				result = this._min;
			}
			else
			{
				bool flag2 = value > this._max;
				if (flag2)
				{
					result = this._max;
				}
				else
				{
					result = value;
				}
			}
			return result;
		}

		// Token: 0x06005BAF RID: 23471 RVA: 0x002A8B48 File Offset: 0x002A6D48
		private void UpdateValueText(sbyte value)
		{
			bool flag = this.valueText == null;
			if (!flag)
			{
				bool flag2 = this._options != null && value >= 0 && (int)value < this._options.Length;
				if (flag2)
				{
					this.valueText.text = this._options[(int)value].Tr();
				}
				else
				{
					this.valueText.text = value.ToString();
				}
			}
		}

		// Token: 0x06005BB0 RID: 23472 RVA: 0x002A8BB8 File Offset: 0x002A6DB8
		public override object GetValue()
		{
			return this._value;
		}

		// Token: 0x06005BB1 RID: 23473 RVA: 0x002A8BC8 File Offset: 0x002A6DC8
		public override void SetValue(object value)
		{
			sbyte newValue = Convert.ToSByte(value);
			this.SetValueInternal(newValue);
		}

		// Token: 0x06005BB2 RID: 23474 RVA: 0x002A8BE5 File Offset: 0x002A6DE5
		public void SetTypedValue(sbyte value)
		{
			this.SetValueInternal(value);
		}

		// Token: 0x06005BB3 RID: 23475 RVA: 0x002A8BF0 File Offset: 0x002A6DF0
		public override void SetInteractable(bool interactable)
		{
			this._interactable = interactable;
			this.UpdateButtonInteractable();
		}

		// Token: 0x06005BB4 RID: 23476 RVA: 0x002A8C04 File Offset: 0x002A6E04
		private void UpdateButtonInteractable()
		{
			bool flag = this.leftBtn != null;
			if (flag)
			{
				this.leftBtn.interactable = (this._interactable && this._value > this._min);
				bool interactable = this.leftBtn.interactable;
				if (interactable)
				{
					this.leftBtn.GetComponent<HSVStyleRoot>().SetDefault();
					this.leftBtn.GetComponent<CImage>().SetAlpha(1f);
				}
				else
				{
					this.leftBtn.GetComponent<HSVStyleRoot>().SetDefaultBlack();
					this.leftBtn.GetComponent<CImage>().SetAlpha(0.5f);
				}
			}
			bool flag2 = this.rightBtn != null;
			if (flag2)
			{
				this.rightBtn.interactable = (this._interactable && this._value < this._max);
				bool interactable2 = this.rightBtn.interactable;
				if (interactable2)
				{
					this.rightBtn.GetComponent<HSVStyleRoot>().SetDefault();
					this.rightBtn.GetComponent<CImage>().SetAlpha(1f);
				}
				else
				{
					this.rightBtn.GetComponent<HSVStyleRoot>().SetDefaultBlack();
					this.rightBtn.GetComponent<CImage>().SetAlpha(0.5f);
				}
			}
		}

		// Token: 0x06005BB5 RID: 23477 RVA: 0x002A8D4C File Offset: 0x002A6F4C
		private void OnDestroy()
		{
			bool flag = this.leftBtn != null;
			if (flag)
			{
				this.leftBtn.onClick.RemoveListener(new UnityAction(this.OnLeftBtnClick));
			}
			bool flag2 = this.rightBtn != null;
			if (flag2)
			{
				this.rightBtn.onClick.RemoveListener(new UnityAction(this.OnRightBtnClick));
			}
		}

		// Token: 0x04003F36 RID: 16182
		[SerializeField]
		private CButton leftBtn;

		// Token: 0x04003F37 RID: 16183
		[SerializeField]
		private CButton rightBtn;

		// Token: 0x04003F38 RID: 16184
		[SerializeField]
		private TextMeshProUGUI valueText;

		// Token: 0x04003F39 RID: 16185
		private sbyte _value;

		// Token: 0x04003F3A RID: 16186
		private sbyte _min;

		// Token: 0x04003F3B RID: 16187
		private sbyte _max;

		// Token: 0x04003F3C RID: 16188
		private LanguageKey[] _options;

		// Token: 0x04003F3D RID: 16189
		private SwitchButtonSettingAttribute _attr;

		// Token: 0x04003F3E RID: 16190
		private Action<sbyte> _setValueAction;

		// Token: 0x04003F3F RID: 16191
		private Func<sbyte> _getValueAction;

		// Token: 0x04003F40 RID: 16192
		private bool _interactable = true;
	}
}
