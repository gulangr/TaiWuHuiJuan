using System;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EBE RID: 3774
	public class InputIntegerCell : MonoBehaviour, ICellContent<InputIntegerCellData>, ICellContent
	{
		// Token: 0x170013D1 RID: 5073
		// (get) Token: 0x0600AEEC RID: 44780 RVA: 0x004FAFB5 File Offset: 0x004F91B5
		private int Value
		{
			get
			{
				return this._data.GetAction(this._data.Id);
			}
		}

		// Token: 0x0600AEED RID: 44781 RVA: 0x004FAFD4 File Offset: 0x004F91D4
		public void SetData(InputIntegerCellData data)
		{
			this._data = data;
			this.inputField.SetTextWithoutNotify(this.Value.ToString());
			this.btnPrev.ClearAndAddListener(new Action(this.OnClickPrev));
			this.btnNext.ClearAndAddListener(new Action(this.OnClickNext));
			this.inputField.onValueChanged.RemoveAllListeners();
			this.inputField.onValueChanged.AddListener(new UnityAction<string>(this.OnChangeValue));
			this.btnPrev.gameObject.SetActive(this.Value > this._data.MinValue);
			this.btnNext.gameObject.SetActive(this.Value < this._data.MaxValue);
			this.btnPrev.GetComponent<HoverHelper>().Init();
			this.btnNext.GetComponent<HoverHelper>().Init();
		}

		// Token: 0x0600AEEE RID: 44782 RVA: 0x004FB0CC File Offset: 0x004F92CC
		private void SetValue(int value)
		{
			value = Math.Clamp(value, this._data.MinValue, this._data.MaxValue);
			this.inputField.SetTextWithoutNotify(value.ToString());
			this._data.SetAction(this._data.Id, value);
			this.btnPrev.gameObject.SetActive(value > this._data.MinValue);
			this.btnNext.gameObject.SetActive(value < this._data.MaxValue);
		}

		// Token: 0x0600AEEF RID: 44783 RVA: 0x004FB168 File Offset: 0x004F9368
		private void OnChangeValue(string value)
		{
			int intValue;
			bool flag = !int.TryParse(value, out intValue);
			if (flag)
			{
				bool flag2 = string.IsNullOrEmpty(value);
				if (flag2)
				{
					this.inputField.text = 0.ToString();
				}
				else
				{
					this.inputField.SetTextWithoutNotify(this.Value.ToString());
				}
			}
			else
			{
				bool flag3 = intValue > this._data.MaxValue;
				if (flag3)
				{
					this.inputField.SetTextWithoutNotify(value.Substring(0, value.Length - 1));
				}
				else
				{
					this.SetValue(intValue);
				}
			}
		}

		// Token: 0x0600AEF0 RID: 44784 RVA: 0x004FB204 File Offset: 0x004F9404
		private void OnClickPrev()
		{
			bool flag = this.Value <= this._data.MinValue;
			if (!flag)
			{
				this.SetValue(this.Value - 1);
			}
		}

		// Token: 0x0600AEF1 RID: 44785 RVA: 0x004FB240 File Offset: 0x004F9440
		private void OnClickNext()
		{
			bool flag = this.Value >= this._data.MaxValue;
			if (!flag)
			{
				this.SetValue(this.Value + 1);
			}
		}

		// Token: 0x04008750 RID: 34640
		[SerializeField]
		private TMP_InputField inputField;

		// Token: 0x04008751 RID: 34641
		[SerializeField]
		private CButton btnPrev;

		// Token: 0x04008752 RID: 34642
		[SerializeField]
		private CButton btnNext;

		// Token: 0x04008753 RID: 34643
		private InputIntegerCellData _data;
	}
}
