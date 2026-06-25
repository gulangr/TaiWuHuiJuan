using System;
using System.Diagnostics;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Legacy.AdventureEditor.Migrate
{
	// Token: 0x02000A0E RID: 2574
	public class InputCSlider : MonoBehaviour
	{
		// Token: 0x17000DB7 RID: 3511
		// (get) Token: 0x06007DDE RID: 32222 RVA: 0x003A6A82 File Offset: 0x003A4C82
		// (set) Token: 0x06007DDF RID: 32223 RVA: 0x003A6A8F File Offset: 0x003A4C8F
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

		// Token: 0x17000DB8 RID: 3512
		// (get) Token: 0x06007DE0 RID: 32224 RVA: 0x003A6A99 File Offset: 0x003A4C99
		// (set) Token: 0x06007DE1 RID: 32225 RVA: 0x003A6AA7 File Offset: 0x003A4CA7
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

		// Token: 0x14000081 RID: 129
		// (add) Token: 0x06007DE2 RID: 32226 RVA: 0x003A6AB4 File Offset: 0x003A4CB4
		// (remove) Token: 0x06007DE3 RID: 32227 RVA: 0x003A6AEC File Offset: 0x003A4CEC
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnValueChanged;

		// Token: 0x06007DE4 RID: 32228 RVA: 0x003A6B21 File Offset: 0x003A4D21
		private void Awake()
		{
			this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnInputChanged));
			this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderChanged));
		}

		// Token: 0x06007DE5 RID: 32229 RVA: 0x003A6B60 File Offset: 0x003A4D60
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

		// Token: 0x06007DE6 RID: 32230 RVA: 0x003A6BBD File Offset: 0x003A4DBD
		public void SetInteractable(bool interactable)
		{
			this.slider.interactable = interactable;
			this.inputField.interactable = interactable;
		}

		// Token: 0x06007DE7 RID: 32231 RVA: 0x003A6BDC File Offset: 0x003A4DDC
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

		// Token: 0x06007DE8 RID: 32232 RVA: 0x003A6C20 File Offset: 0x003A4E20
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

		// Token: 0x06007DE9 RID: 32233 RVA: 0x003A6CDC File Offset: 0x003A4EDC
		private void SyncValueToInputField()
		{
			this.inputField.text = (this.slider.wholeNumbers ? ((int)this.slider.value).ToString() : this.slider.value.ToString("F"));
		}

		// Token: 0x04006007 RID: 24583
		[SerializeField]
		private CSlider slider;

		// Token: 0x04006008 RID: 24584
		[SerializeField]
		private TMP_InputField inputField;

		// Token: 0x0400600A RID: 24586
		private bool _internalSetting;
	}
}
