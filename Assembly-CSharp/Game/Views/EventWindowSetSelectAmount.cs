using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x020006E9 RID: 1769
	public class EventWindowSetSelectAmount : MonoBehaviour
	{
		// Token: 0x17000A5A RID: 2650
		// (get) Token: 0x060053DC RID: 21468 RVA: 0x0026DD5A File Offset: 0x0026BF5A
		// (set) Token: 0x060053DD RID: 21469 RVA: 0x0026DD62 File Offset: 0x0026BF62
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

		// Token: 0x17000A5B RID: 2651
		// (get) Token: 0x060053DE RID: 21470 RVA: 0x0026DD84 File Offset: 0x0026BF84
		private int MaxSelectCount
		{
			get
			{
				return (this._limitCount <= 0) ? this._maxCount : Math.Min(this._limitCount, this._maxCount);
			}
		}

		// Token: 0x17000A5C RID: 2652
		// (get) Token: 0x060053DF RID: 21471 RVA: 0x0026DDA8 File Offset: 0x0026BFA8
		private CanvasGroup CanvasGroup
		{
			get
			{
				return base.gameObject.GetOrAddComponent<CanvasGroup>();
			}
		}

		// Token: 0x060053E0 RID: 21472 RVA: 0x0026DDB5 File Offset: 0x0026BFB5
		public void Rerfresh(int maxAmount, int minAmount, int initAmount, bool zeroValid, bool loop, int changeValue, Action<int> onValueChanged)
		{
			this._onValueChanged = onValueChanged;
			this._initCount = initAmount;
			this.Rerfresh(maxAmount, minAmount, zeroValid, loop, changeValue);
			this.CurCount = Math.Min(this.MaxSelectCount, this._initCount);
		}

		// Token: 0x060053E1 RID: 21473 RVA: 0x0026DDF0 File Offset: 0x0026BFF0
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

		// Token: 0x060053E2 RID: 21474 RVA: 0x0026DE56 File Offset: 0x0026C056
		public void SetCurrentCount(int count)
		{
			this.CurCount = count;
		}

		// Token: 0x060053E3 RID: 21475 RVA: 0x0026DE64 File Offset: 0x0026C064
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

		// Token: 0x060053E4 RID: 21476 RVA: 0x0026DED0 File Offset: 0x0026C0D0
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

		// Token: 0x060053E5 RID: 21477 RVA: 0x0026DF4C File Offset: 0x0026C14C
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

		// Token: 0x060053E6 RID: 21478 RVA: 0x0026DFA3 File Offset: 0x0026C1A3
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

		// Token: 0x060053E7 RID: 21479 RVA: 0x0026DFCC File Offset: 0x0026C1CC
		private void UpdateContent()
		{
			this.txtAmount.text = this.CurCount.ToString();
			this.slider.SetValueWithoutNotify((float)this.CurCount);
		}

		// Token: 0x060053E8 RID: 21480 RVA: 0x0026E008 File Offset: 0x0026C208
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

		// Token: 0x040038C3 RID: 14531
		private int _initCount;

		// Token: 0x040038C4 RID: 14532
		private int _curCount;

		// Token: 0x040038C5 RID: 14533
		private int _maxCount;

		// Token: 0x040038C6 RID: 14534
		private int _limitCount = 0;

		// Token: 0x040038C7 RID: 14535
		private int _minCount;

		// Token: 0x040038C8 RID: 14536
		private bool _zeroValid;

		// Token: 0x040038C9 RID: 14537
		private bool _loop;

		// Token: 0x040038CA RID: 14538
		private Action<int> _onValueChanged;

		// Token: 0x040038CB RID: 14539
		private int _changeValue = 1;

		// Token: 0x040038CC RID: 14540
		[SerializeField]
		private CButton buttonMore;

		// Token: 0x040038CD RID: 14541
		[SerializeField]
		private CButton buttonLess;

		// Token: 0x040038CE RID: 14542
		[SerializeField]
		private CSlider slider;

		// Token: 0x040038CF RID: 14543
		[SerializeField]
		private TextMeshProUGUI txtAmount;
	}
}
