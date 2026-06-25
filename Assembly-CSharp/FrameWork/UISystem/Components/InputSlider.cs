using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.UISystem.Components
{
	// Token: 0x0200101B RID: 4123
	public class InputSlider : MonoBehaviour
	{
		// Token: 0x1700154D RID: 5453
		// (get) Token: 0x0600BCE4 RID: 48356 RVA: 0x0055E1E6 File Offset: 0x0055C3E6
		// (set) Token: 0x0600BCE5 RID: 48357 RVA: 0x0055E1F3 File Offset: 0x0055C3F3
		public float Value
		{
			get
			{
				return this.slider.value;
			}
			set
			{
				this.Set(value);
			}
		}

		// Token: 0x1700154E RID: 5454
		// (get) Token: 0x0600BCE6 RID: 48358 RVA: 0x0055E1FD File Offset: 0x0055C3FD
		// (set) Token: 0x0600BCE7 RID: 48359 RVA: 0x0055E20B File Offset: 0x0055C40B
		public int ValueInt
		{
			get
			{
				return (int)this.slider.value;
			}
			set
			{
				this.Set((float)value);
			}
		}

		// Token: 0x14000098 RID: 152
		// (add) Token: 0x0600BCE8 RID: 48360 RVA: 0x0055E218 File Offset: 0x0055C418
		// (remove) Token: 0x0600BCE9 RID: 48361 RVA: 0x0055E250 File Offset: 0x0055C450
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnValueChanged;

		// Token: 0x0600BCEA RID: 48362 RVA: 0x0055E285 File Offset: 0x0055C485
		private void Awake()
		{
			this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnInputChanged));
			this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderChanged));
		}

		// Token: 0x0600BCEB RID: 48363 RVA: 0x0055E2C4 File Offset: 0x0055C4C4
		public void Set(float value)
		{
			this._internalSetting = true;
			this.slider.value = Mathf.Clamp(value, this.slider.minValue, this.slider.maxValue);
			this.SyncValueToInputField();
			this._internalSetting = false;
			Action onValueChanged = this.OnValueChanged;
			if (onValueChanged != null)
			{
				onValueChanged();
			}
		}

		// Token: 0x0600BCEC RID: 48364 RVA: 0x0055E324 File Offset: 0x0055C524
		private void OnSliderChanged(float arg0)
		{
			bool internalSetting = this._internalSetting;
			if (!internalSetting)
			{
				this._internalSetting = true;
				this.SyncValueToInputField();
				this._internalSetting = false;
				Action onValueChanged = this.OnValueChanged;
				if (onValueChanged != null)
				{
					onValueChanged();
				}
			}
		}

		// Token: 0x0600BCED RID: 48365 RVA: 0x0055E368 File Offset: 0x0055C568
		private void OnInputChanged(string arg0)
		{
			bool internalSetting = this._internalSetting;
			if (!internalSetting)
			{
				this._internalSetting = true;
				int valueInt = 0;
				float valueFloat = 0f;
				bool flag = this.slider.wholeNumbers ? int.TryParse(arg0, out valueInt) : float.TryParse(arg0, out valueFloat);
				if (flag)
				{
					float value = this.slider.wholeNumbers ? ((float)valueInt) : valueFloat;
					float clampedValue = Mathf.Clamp(value, this.slider.minValue, this.slider.maxValue);
					this.slider.value = clampedValue;
					this.SyncValueToInputField();
				}
				else
				{
					this.SyncValueToInputField();
				}
				this._internalSetting = false;
				Action onValueChanged = this.OnValueChanged;
				if (onValueChanged != null)
				{
					onValueChanged();
				}
			}
		}

		// Token: 0x0600BCEE RID: 48366 RVA: 0x0055E424 File Offset: 0x0055C624
		private void SyncValueToInputField()
		{
			this.inputField.text = (this.slider.wholeNumbers ? ((int)this.slider.value).ToString() : this.slider.value.ToString("F"));
		}

		// Token: 0x0400914D RID: 37197
		[SerializeField]
		private CSliderLegacy slider;

		// Token: 0x0400914E RID: 37198
		[SerializeField]
		private TMP_InputField inputField;

		// Token: 0x04009150 RID: 37200
		private bool _internalSetting;
	}
}
