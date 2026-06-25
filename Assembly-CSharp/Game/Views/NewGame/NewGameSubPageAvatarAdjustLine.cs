using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.NewGame
{
	// Token: 0x020007EC RID: 2028
	public class NewGameSubPageAvatarAdjustLine : MonoBehaviour
	{
		// Token: 0x17000BED RID: 3053
		// (get) Token: 0x060062FF RID: 25343 RVA: 0x002D4EFC File Offset: 0x002D30FC
		// (set) Token: 0x06006300 RID: 25344 RVA: 0x002D4F20 File Offset: 0x002D3120
		public float Value
		{
			get
			{
				return (this.slider != null) ? this.slider.value : 0f;
			}
			set
			{
				bool flag = this.slider != null;
				if (flag)
				{
					this.slider.value = value;
				}
			}
		}

		// Token: 0x06006301 RID: 25345 RVA: 0x002D4F4B File Offset: 0x002D314B
		public void SetActive(bool active)
		{
			base.gameObject.SetActive(active);
		}

		// Token: 0x06006302 RID: 25346 RVA: 0x002D4F5C File Offset: 0x002D315C
		public void Init(float[] offsetRange, short defaultShortVal, UnityAction<short> onValueChanged, UnityAction onRandomClick, UnityAction onResetClick)
		{
			this._offsetRange = offsetRange;
			this._defaultShortVal = defaultShortVal;
			bool flag = this.slider != null;
			if (flag)
			{
				this.slider.minValue = 0f;
				this.slider.maxValue = 1f;
				this.slider.wholeNumbers = false;
				this.slider.onValueChanged.RemoveAllListeners();
				this.slider.onValueChanged.AddListener(delegate(float sliderVal)
				{
					bool flag4 = this._offsetRange == null;
					if (!flag4)
					{
						short shortVal = NewGameSubPageAvatarAdjustLine.SliderToShort(this._offsetRange, this._defaultShortVal, sliderVal);
						onValueChanged(shortVal);
					}
				});
			}
			bool flag2 = this.randomButton != null;
			if (flag2)
			{
				this.randomButton.onClick.RemoveAllListeners();
				this.randomButton.onClick.AddListener(onRandomClick);
			}
			bool flag3 = this.resetButton != null;
			if (flag3)
			{
				this.resetButton.onClick.RemoveAllListeners();
				this.resetButton.onClick.AddListener(onResetClick);
			}
		}

		// Token: 0x06006303 RID: 25347 RVA: 0x002D5068 File Offset: 0x002D3268
		public void Init(UnityAction<float> onValueChanged, UnityAction onRandomClick, UnityAction onResetClick)
		{
			this._offsetRange = null;
			this._defaultShortVal = 0;
			bool flag = this.slider != null;
			if (flag)
			{
				this.slider.minValue = 0f;
				this.slider.maxValue = 1f;
				this.slider.wholeNumbers = false;
				this.slider.onValueChanged.RemoveAllListeners();
				this.slider.onValueChanged.AddListener(onValueChanged);
			}
			bool flag2 = this.randomButton != null;
			if (flag2)
			{
				this.randomButton.onClick.RemoveAllListeners();
				this.randomButton.onClick.AddListener(onRandomClick);
			}
			bool flag3 = this.resetButton != null;
			if (flag3)
			{
				this.resetButton.onClick.RemoveAllListeners();
				this.resetButton.onClick.AddListener(onResetClick);
			}
		}

		// Token: 0x06006304 RID: 25348 RVA: 0x002D5154 File Offset: 0x002D3354
		public void SetShortValueWithoutNotify(float[] offsetRange, short defaultShortVal, short val)
		{
			this._offsetRange = offsetRange;
			this._defaultShortVal = defaultShortVal;
			float sliderVal = NewGameSubPageAvatarAdjustLine.ShortToSlider(offsetRange, defaultShortVal, val);
			bool flag = this.slider != null;
			if (flag)
			{
				this.slider.SetValueWithoutNotify(sliderVal);
			}
		}

		// Token: 0x06006305 RID: 25349 RVA: 0x002D5198 File Offset: 0x002D3398
		public void SetValueWithoutNotify(float value)
		{
			bool flag = this.slider != null;
			if (flag)
			{
				this.slider.SetValueWithoutNotify(value);
			}
		}

		// Token: 0x06006306 RID: 25350 RVA: 0x002D51C8 File Offset: 0x002D33C8
		public static short SliderToShort(float[] offsetRange, short defaultShortVal, float sliderVal)
		{
			float defaultOffset = (float)defaultShortVal / 100f;
			float minOffset = offsetRange[0];
			float maxOffset = offsetRange[offsetRange.Length - 1];
			bool flag = sliderVal <= 0.5f;
			float offset;
			if (flag)
			{
				float t = sliderVal * 2f;
				offset = Mathf.LerpUnclamped(minOffset, defaultOffset, t);
			}
			else
			{
				float t2 = (sliderVal - 0.5f) * 2f;
				offset = Mathf.LerpUnclamped(defaultOffset, maxOffset, t2);
			}
			return (short)Math.Round((double)(offset * 100f));
		}

		// Token: 0x06006307 RID: 25351 RVA: 0x002D5244 File Offset: 0x002D3444
		public static float ShortToSlider(float[] offsetRange, short defaultShortVal, short val)
		{
			float offset = (float)val / 100f;
			float defaultOffset = (float)defaultShortVal / 100f;
			float minOffset = offsetRange[0];
			float maxOffset = offsetRange[offsetRange.Length - 1];
			bool flag = Math.Abs(maxOffset - minOffset) < 1E-07f;
			float result;
			if (flag)
			{
				result = 0.5f;
			}
			else
			{
				bool flag2 = Math.Abs(offset - defaultOffset) < 1E-07f;
				if (flag2)
				{
					result = 0.5f;
				}
				else
				{
					bool flag3 = offset < defaultOffset;
					if (flag3)
					{
						bool flag4 = Math.Abs(defaultOffset - minOffset) < 1E-07f;
						if (flag4)
						{
							result = 0f;
						}
						else
						{
							float t = (offset - minOffset) / (defaultOffset - minOffset);
							result = t * 0.5f;
						}
					}
					else
					{
						bool flag5 = Math.Abs(maxOffset - defaultOffset) < 1E-07f;
						if (flag5)
						{
							result = 1f;
						}
						else
						{
							float t2 = (offset - defaultOffset) / (maxOffset - defaultOffset);
							result = 0.5f + t2 * 0.5f;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x040044F7 RID: 17655
		[SerializeField]
		private CSlider slider;

		// Token: 0x040044F8 RID: 17656
		[SerializeField]
		private CButton randomButton;

		// Token: 0x040044F9 RID: 17657
		[SerializeField]
		private CButton resetButton;

		// Token: 0x040044FA RID: 17658
		private float[] _offsetRange;

		// Token: 0x040044FB RID: 17659
		private short _defaultShortVal;
	}
}
