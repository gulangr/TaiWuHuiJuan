using System;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Map;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Bottom
{
	// Token: 0x02000C38 RID: 3128
	public class MapBlockFindItem_RangeSlider : MapBlockFindItemBase
	{
		// Token: 0x06009EED RID: 40685 RVA: 0x004A5148 File Offset: 0x004A3348
		protected override void InitComponents()
		{
			base.InitComponents();
			bool flag = this.inputFieldMin != null;
			if (flag)
			{
				this.inputFieldMin.onEndEdit.AddListener(new UnityAction<string>(this.OnMinInputEndEdit));
			}
			bool flag2 = this.inputFieldMax != null;
			if (flag2)
			{
				this.inputFieldMax.onEndEdit.AddListener(new UnityAction<string>(this.OnMaxInputEndEdit));
			}
		}

		// Token: 0x06009EEE RID: 40686 RVA: 0x004A51B8 File Offset: 0x004A33B8
		public override void Set(ViewFindMapBlock mapBlockFind, EFilterItemKey key, FilterItemConfig itemConfig)
		{
			base.Set(mapBlockFind, key, itemConfig);
			this.rangeSlider.SetRange((float)itemConfig.SliderRange.First, (float)itemConfig.SliderRange.Second, new Action<float, float>(this.OnRangeChanged));
			this.SetWithoutNotify();
		}

		// Token: 0x06009EEF RID: 40687 RVA: 0x004A5207 File Offset: 0x004A3407
		private void OnRangeChanged(float lower, float upper)
		{
			this.UpdateInputDisplay();
			this._mapBlockFind.UpdateRangeSliderData(base.EFilterItemKey, new IntPair((int)lower, (int)upper));
		}

		// Token: 0x06009EF0 RID: 40688 RVA: 0x004A522C File Offset: 0x004A342C
		private void OnMinInputEndEdit(string text)
		{
			int value;
			bool flag = int.TryParse(text, out value) && value >= this._itemConfig.SliderRange.First && value <= (int)this.rangeSlider.UpperValue;
			if (flag)
			{
				this.rangeSlider.SetValue((float)value, this.rangeSlider.UpperValue, true);
			}
			else
			{
				this.UpdateInputDisplay();
			}
		}

		// Token: 0x06009EF1 RID: 40689 RVA: 0x004A5298 File Offset: 0x004A3498
		private void OnMaxInputEndEdit(string text)
		{
			int value;
			bool flag = int.TryParse(text, out value) && value >= (int)this.rangeSlider.LowerValue && value <= this._itemConfig.SliderRange.Second;
			if (flag)
			{
				this.rangeSlider.SetValue(this.rangeSlider.LowerValue, (float)value, true);
			}
			else
			{
				this.UpdateInputDisplay();
			}
		}

		// Token: 0x06009EF2 RID: 40690 RVA: 0x004A5304 File Offset: 0x004A3504
		private void UpdateInputDisplay()
		{
			bool flag = this.inputFieldMin != null;
			if (flag)
			{
				this.inputFieldMin.text = ((int)this.rangeSlider.LowerValue).ToString();
			}
			bool flag2 = this.inputFieldMax != null;
			if (flag2)
			{
				this.inputFieldMax.text = ((int)this.rangeSlider.UpperValue).ToString();
			}
		}

		// Token: 0x06009EF3 RID: 40691 RVA: 0x004A5372 File Offset: 0x004A3572
		public override void Reset()
		{
			this.rangeSlider.SetValue((float)this._itemConfig.SliderRange.First, (float)this._itemConfig.SliderRange.Second, false);
			this.UpdateInputDisplay();
			base.Reset();
		}

		// Token: 0x06009EF4 RID: 40692 RVA: 0x004A53B4 File Offset: 0x004A35B4
		public override void SetWithoutNotify()
		{
			IntPair value;
			bool flag = base.Data.RangeSliderData.TryGetValue(base.EFilterItemKey, out value);
			int lower;
			int upper;
			if (flag)
			{
				lower = value.First;
				upper = value.Second;
			}
			else
			{
				lower = this._itemConfig.SliderRange.First;
				upper = this._itemConfig.SliderRange.Second;
			}
			this.rangeSlider.SetValue((float)lower, (float)upper, false);
			this.UpdateInputDisplay();
		}

		// Token: 0x06009EF5 RID: 40693 RVA: 0x004A542C File Offset: 0x004A362C
		public override bool HasFilterValue()
		{
			return (int)this.rangeSlider.LowerValue > this._itemConfig.SliderRange.First || (int)this.rangeSlider.UpperValue < this._itemConfig.SliderRange.Second;
		}

		// Token: 0x04007AF8 RID: 31480
		[SerializeField]
		private RangeSlider rangeSlider;

		// Token: 0x04007AF9 RID: 31481
		[SerializeField]
		private TMP_InputField inputFieldMin;

		// Token: 0x04007AFA RID: 31482
		[SerializeField]
		private TMP_InputField inputFieldMax;
	}
}
