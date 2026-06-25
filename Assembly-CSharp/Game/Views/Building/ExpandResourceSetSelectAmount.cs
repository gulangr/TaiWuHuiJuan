using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Building
{
	// Token: 0x02000BD9 RID: 3033
	public class ExpandResourceSetSelectAmount : MonoBehaviour
	{
		// Token: 0x1700105B RID: 4187
		// (get) Token: 0x060098AC RID: 39084 RVA: 0x0047182B File Offset: 0x0046FA2B
		// (set) Token: 0x060098AD RID: 39085 RVA: 0x00471833 File Offset: 0x0046FA33
		public int CurCount
		{
			get
			{
				return this._curCount;
			}
			private set
			{
				this._curCount = Math.Clamp(value, this._minCount, this.MaxSelectCount);
				this.OnCountChange();
			}
		}

		// Token: 0x1700105C RID: 4188
		// (get) Token: 0x060098AE RID: 39086 RVA: 0x00471855 File Offset: 0x0046FA55
		private int MaxSelectCount
		{
			get
			{
				return (this._limitCount <= 0) ? this._maxCount : Math.Min(this._limitCount, this._maxCount);
			}
		}

		// Token: 0x1700105D RID: 4189
		// (get) Token: 0x060098AF RID: 39087 RVA: 0x00471879 File Offset: 0x0046FA79
		private CanvasGroup CanvasGroup
		{
			get
			{
				return base.gameObject.GetOrAddComponent<CanvasGroup>();
			}
		}

		// Token: 0x060098B0 RID: 39088 RVA: 0x00471886 File Offset: 0x0046FA86
		public void Rerfresh(int maxAmount, int minAmount, int initAmount, bool zeroValid, bool loop, int changeValue, Action<int> onValueChanged)
		{
			this._onValueChanged = onValueChanged;
			this._initCount = initAmount;
			this.Rerfresh(maxAmount, minAmount, zeroValid, loop, changeValue);
			this.CurCount = Math.Min(this.MaxSelectCount, this._initCount);
		}

		// Token: 0x060098B1 RID: 39089 RVA: 0x004718C0 File Offset: 0x0046FAC0
		public void Rerfresh(int maxAmount, int minAmount, bool zeroValid, bool loop, int changeValue)
		{
			this._maxCount = maxAmount;
			this._minCount = minAmount;
			this._loop = loop;
			this._zeroValid = zeroValid;
			this._changeValue = changeValue;
			this.slider.wholeNumbers = true;
			this.slider.maxValue = (float)this._maxCount;
			this.slider.minValue = (float)this._minCount;
		}

		// Token: 0x060098B2 RID: 39090 RVA: 0x00471926 File Offset: 0x0046FB26
		public void SetCurrentCount(int count)
		{
			this.CurCount = count;
		}

		// Token: 0x060098B3 RID: 39091 RVA: 0x00471934 File Offset: 0x0046FB34
		private void Awake()
		{
			this.slider.onValueChanged.RemoveAllListeners();
			this.slider.onValueChanged.AddListener(delegate(float value)
			{
				this.CurCount = (int)value;
			});
			this.buttonMore.ClearAndAddListener(new Action(this.More));
			this.buttonLess.ClearAndAddListener(new Action(this.Less));
		}

		// Token: 0x060098B4 RID: 39092 RVA: 0x004719A0 File Offset: 0x0046FBA0
		private void More()
		{
			bool flag = this.CurCount >= this.MaxSelectCount;
			if (flag)
			{
				bool flag2 = !this._loop;
				if (!flag2)
				{
					this.CurCount = this._minCount;
				}
			}
			else
			{
				bool flag3 = this.CurCount < this._changeValue;
				if (flag3)
				{
					this.CurCount = this._changeValue;
				}
				else
				{
					this.CurCount += this._changeValue;
				}
			}
		}

		// Token: 0x060098B5 RID: 39093 RVA: 0x00471A1C File Offset: 0x0046FC1C
		private void Less()
		{
			bool flag = this.CurCount <= this._minCount;
			if (flag)
			{
				bool flag2 = !this._loop;
				if (!flag2)
				{
					this.CurCount = this.MaxSelectCount;
				}
			}
			else
			{
				this.CurCount -= this._changeValue;
			}
		}

		// Token: 0x060098B6 RID: 39094 RVA: 0x00471A73 File Offset: 0x0046FC73
		private void OnCountChange()
		{
			this.UpdateButtonState();
			Action<int> onValueChanged = this._onValueChanged;
			if (onValueChanged != null)
			{
				onValueChanged(this.CurCount);
			}
			this.UpdateContent();
		}

		// Token: 0x060098B7 RID: 39095 RVA: 0x00471A9C File Offset: 0x0046FC9C
		private void UpdateContent()
		{
			bool flag = this.txtAmount != null;
			if (flag)
			{
				this.txtAmount.text = this.CurCount.ToString();
			}
			this.slider.SetValueWithoutNotify((float)this.CurCount);
		}

		// Token: 0x060098B8 RID: 39096 RVA: 0x00471AE8 File Offset: 0x0046FCE8
		private void UpdateButtonState()
		{
			bool flag = this.buttonMore;
			if (flag)
			{
				this.buttonMore.interactable = (this.CurCount < this.MaxSelectCount);
				this.buttonMore.GetComponent<TooltipInvoker>().enabled = (this.CurCount >= this._limitCount);
			}
			bool flag2 = this.buttonLess;
			if (flag2)
			{
				this.buttonLess.interactable = (this.CurCount > this._minCount);
			}
		}

		// Token: 0x060098B9 RID: 39097 RVA: 0x00471B6E File Offset: 0x0046FD6E
		public void SetInteractable(bool interactable)
		{
			this.slider.interactable = interactable;
		}

		// Token: 0x0400757A RID: 30074
		private int _initCount;

		// Token: 0x0400757B RID: 30075
		private int _curCount;

		// Token: 0x0400757C RID: 30076
		private int _maxCount;

		// Token: 0x0400757D RID: 30077
		private int _limitCount = 0;

		// Token: 0x0400757E RID: 30078
		private int _minCount;

		// Token: 0x0400757F RID: 30079
		private bool _zeroValid;

		// Token: 0x04007580 RID: 30080
		private bool _loop;

		// Token: 0x04007581 RID: 30081
		private Action<int> _onValueChanged;

		// Token: 0x04007582 RID: 30082
		private int _changeValue = 1;

		// Token: 0x04007583 RID: 30083
		[SerializeField]
		private CButton buttonMore;

		// Token: 0x04007584 RID: 30084
		[SerializeField]
		private CButton buttonLess;

		// Token: 0x04007585 RID: 30085
		[SerializeField]
		private CSlider slider;

		// Token: 0x04007586 RID: 30086
		[SerializeField]
		private TextMeshProUGUI txtAmount;
	}
}
