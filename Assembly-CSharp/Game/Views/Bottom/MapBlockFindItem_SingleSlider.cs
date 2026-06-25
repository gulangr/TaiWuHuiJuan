using System;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Map;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Bottom
{
	// Token: 0x02000C3A RID: 3130
	public class MapBlockFindItem_SingleSlider : MapBlockFindItemBase
	{
		// Token: 0x06009EFF RID: 40703 RVA: 0x004A5658 File Offset: 0x004A3858
		protected override void InitComponents()
		{
			base.InitComponents();
			this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderChanged));
			this.inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnInputFieldEndEdit));
			this.slider.wholeNumbers = true;
		}

		// Token: 0x06009F00 RID: 40704 RVA: 0x004A56B4 File Offset: 0x004A38B4
		public override void Set(ViewFindMapBlock mapBlockFind, EFilterItemKey key, FilterItemConfig itemConfig)
		{
			base.Set(mapBlockFind, key, itemConfig);
			this._isInitializing = true;
			this.slider.minValue = (float)itemConfig.SliderRange.First;
			this.slider.maxValue = (float)itemConfig.SliderRange.Second;
			this._isInitializing = false;
			this.SetWithoutNotify();
		}

		// Token: 0x06009F01 RID: 40705 RVA: 0x004A5714 File Offset: 0x004A3914
		private void OnSliderChanged(float value)
		{
			bool isInitializing = this._isInitializing;
			if (!isInitializing)
			{
				this.UpdateInputFieldDisplay();
				this._mapBlockFind.UpdateSingleSliderData(base.EFilterItemKey, (int)value);
			}
		}

		// Token: 0x06009F02 RID: 40706 RVA: 0x004A574C File Offset: 0x004A394C
		private void OnInputFieldEndEdit(string text)
		{
			int value;
			bool flag = int.TryParse(text, out value) && value >= (int)this.slider.minValue && value <= (int)this.slider.maxValue;
			if (flag)
			{
				this.slider.SetValueWithoutNotify((float)value);
				this.UpdateInputFieldDisplay();
				this._mapBlockFind.UpdateSingleSliderData(base.EFilterItemKey, value);
			}
			else
			{
				this.UpdateInputFieldDisplay();
			}
		}

		// Token: 0x06009F03 RID: 40707 RVA: 0x004A57C4 File Offset: 0x004A39C4
		private void UpdateInputFieldDisplay()
		{
			this.inputField.text = ((int)this.slider.value).ToString();
		}

		// Token: 0x06009F04 RID: 40708 RVA: 0x004A57F2 File Offset: 0x004A39F2
		public override void Reset()
		{
			this.slider.SetValueWithoutNotify((float)this._itemConfig.SliderRange.Second);
			this.UpdateInputFieldDisplay();
			base.Reset();
		}

		// Token: 0x06009F05 RID: 40709 RVA: 0x004A5820 File Offset: 0x004A3A20
		public override void SetWithoutNotify()
		{
			int storedValue;
			bool flag = base.Data.SingleSliderData.TryGetValue(base.EFilterItemKey, out storedValue);
			int value;
			if (flag)
			{
				value = storedValue;
			}
			else
			{
				value = this._itemConfig.SliderRange.Second;
			}
			this.slider.SetValueWithoutNotify((float)value);
			this.UpdateInputFieldDisplay();
		}

		// Token: 0x06009F06 RID: 40710 RVA: 0x004A5878 File Offset: 0x004A3A78
		public override bool HasFilterValue()
		{
			return (int)this.slider.value < this._itemConfig.SliderRange.Second;
		}

		// Token: 0x04007AFC RID: 31484
		[SerializeField]
		private CSlider slider;

		// Token: 0x04007AFD RID: 31485
		[SerializeField]
		private TMP_InputField inputField;

		// Token: 0x04007AFE RID: 31486
		private bool _isInitializing;
	}
}
