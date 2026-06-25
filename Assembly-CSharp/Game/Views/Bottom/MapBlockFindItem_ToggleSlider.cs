using System;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Map;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Bottom
{
	// Token: 0x02000C3B RID: 3131
	public class MapBlockFindItem_ToggleSlider : MapBlockFindItemBase
	{
		// Token: 0x06009F08 RID: 40712 RVA: 0x004A58B4 File Offset: 0x004A3AB4
		protected override void InitComponents()
		{
			base.InitComponents();
			this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleChanged));
			this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderChanged));
			this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnInputFieldEndEdit));
			this.slider.wholeNumbers = true;
		}

		// Token: 0x06009F09 RID: 40713 RVA: 0x004A5930 File Offset: 0x004A3B30
		public override void Set(ViewFindMapBlock mapBlockFind, EFilterItemKey key, FilterItemConfig itemConfig)
		{
			base.Set(mapBlockFind, key, itemConfig);
			this._isInitializing = true;
			this.slider.minValue = (float)itemConfig.SliderRange.First;
			this.slider.maxValue = (float)itemConfig.SliderRange.Second;
			this._isInitializing = false;
			this.SetWithoutNotify();
		}

		// Token: 0x06009F0A RID: 40714 RVA: 0x004A598D File Offset: 0x004A3B8D
		private void OnToggleChanged(bool isOn)
		{
			this.UpdateSliderInteractable();
			this._mapBlockFind.UpdateToggleSliderData(base.EFilterItemKey, new ToggleSliderValue(isOn, (int)this.slider.value));
		}

		// Token: 0x06009F0B RID: 40715 RVA: 0x004A59BC File Offset: 0x004A3BBC
		private void OnSliderChanged(float value)
		{
			bool isInitializing = this._isInitializing;
			if (!isInitializing)
			{
				this.UpdateInputFieldDisplay();
				this._mapBlockFind.UpdateToggleSliderData(base.EFilterItemKey, new ToggleSliderValue(this.toggle.isOn, (int)value));
			}
		}

		// Token: 0x06009F0C RID: 40716 RVA: 0x004A5A04 File Offset: 0x004A3C04
		private void OnInputFieldEndEdit(string text)
		{
			int value;
			bool flag = int.TryParse(text, out value) && value >= (int)this.slider.minValue && value <= (int)this.slider.maxValue;
			if (flag)
			{
				this.slider.SetValueWithoutNotify((float)value);
				this.UpdateInputFieldDisplay();
				this._mapBlockFind.UpdateToggleSliderData(base.EFilterItemKey, new ToggleSliderValue(this.toggle.isOn, value));
			}
			else
			{
				this.UpdateInputFieldDisplay();
			}
		}

		// Token: 0x06009F0D RID: 40717 RVA: 0x004A5A89 File Offset: 0x004A3C89
		private void UpdateSliderInteractable()
		{
			this.slider.interactable = this.toggle.isOn;
			this.inputField.interactable = this.toggle.isOn;
		}

		// Token: 0x06009F0E RID: 40718 RVA: 0x004A5ABC File Offset: 0x004A3CBC
		private void UpdateInputFieldDisplay()
		{
			this.inputField.text = ((int)this.slider.value).ToString();
		}

		// Token: 0x06009F0F RID: 40719 RVA: 0x004A5AEC File Offset: 0x004A3CEC
		public override void Reset()
		{
			this.toggle.SetIsOnWithoutNotify(false);
			this.slider.SetValueWithoutNotify((float)this._itemConfig.SliderRange.Second);
			this.UpdateInputFieldDisplay();
			this.UpdateSliderInteractable();
			base.Reset();
		}

		// Token: 0x06009F10 RID: 40720 RVA: 0x004A5B3C File Offset: 0x004A3D3C
		public override void SetWithoutNotify()
		{
			ToggleSliderValue storedValue;
			bool flag = base.Data.ToggleSliderData.TryGetValue(base.EFilterItemKey, out storedValue);
			ToggleSliderValue value;
			if (flag)
			{
				value = storedValue;
			}
			else
			{
				value = new ToggleSliderValue(false, this._itemConfig.SliderRange.Second);
			}
			this.toggle.SetIsOnWithoutNotify(value.IsOn);
			this.slider.SetValueWithoutNotify((float)value.Value);
			this.UpdateInputFieldDisplay();
		}

		// Token: 0x06009F11 RID: 40721 RVA: 0x004A5BB4 File Offset: 0x004A3DB4
		public override bool HasFilterValue()
		{
			return this.toggle.isOn;
		}

		// Token: 0x04007AFF RID: 31487
		[SerializeField]
		private CToggle toggle;

		// Token: 0x04007B00 RID: 31488
		[SerializeField]
		private CSlider slider;

		// Token: 0x04007B01 RID: 31489
		[SerializeField]
		private TMP_InputField inputField;

		// Token: 0x04007B02 RID: 31490
		private bool _isInitializing;
	}
}
