using System;
using FrameWork;
using TMPro;

namespace AiEditor
{
	// Token: 0x02000692 RID: 1682
	public class UI_AiShortNumberInputField : UIBase
	{
		// Token: 0x170009AD RID: 2477
		// (get) Token: 0x06004F01 RID: 20225 RVA: 0x00250428 File Offset: 0x0024E628
		private TMP_InputField InputField
		{
			get
			{
				return base.CGet<TMP_InputField>("InputField");
			}
		}

		// Token: 0x170009AE RID: 2478
		// (get) Token: 0x06004F02 RID: 20226 RVA: 0x00250435 File Offset: 0x0024E635
		private CButtonObsolete Confirm
		{
			get
			{
				return base.CGet<CButtonObsolete>("Confirm");
			}
		}

		// Token: 0x170009AF RID: 2479
		// (get) Token: 0x06004F03 RID: 20227 RVA: 0x00250442 File Offset: 0x0024E642
		private TextMeshProUGUI PlaceHolder
		{
			get
			{
				return this.InputField.placeholder.GetComponent<TextMeshProUGUI>();
			}
		}

		// Token: 0x170009B0 RID: 2480
		// (get) Token: 0x06004F04 RID: 20228 RVA: 0x00250454 File Offset: 0x0024E654
		private string InputText
		{
			get
			{
				return this.InputField.text;
			}
		}

		// Token: 0x06004F05 RID: 20229 RVA: 0x00250464 File Offset: 0x0024E664
		public override void OnInit(ArgumentBox argsBox)
		{
			this._minValue = (argsBox.Get("MinValue", out this._minValue) ? this._minValue : int.MinValue);
			this._maxValue = (argsBox.Get("MaxValue", out this._maxValue) ? this._maxValue : int.MaxValue);
			argsBox.Get<Action<int>>("SelectCallback", out this._selectCallback);
			this.InputField.text = string.Empty;
			this.PlaceHolder.text = this._minValue.ToString() + "~" + this._maxValue.ToString();
			this.OnInputChanged();
		}

		// Token: 0x06004F06 RID: 20230 RVA: 0x00250514 File Offset: 0x0024E714
		public void OnInputChanged()
		{
			this.Confirm.interactable = this.InputIsValid();
		}

		// Token: 0x06004F07 RID: 20231 RVA: 0x0025052C File Offset: 0x0024E72C
		public void OnConfirm()
		{
			bool flag = !this.InputIsValid();
			if (!flag)
			{
				Action<int> selectCallback = this._selectCallback;
				if (selectCallback != null)
				{
					selectCallback(int.Parse(this.InputText));
				}
				this.QuickHide();
			}
		}

		// Token: 0x06004F08 RID: 20232 RVA: 0x0025056D File Offset: 0x0024E76D
		public void OnCancel()
		{
			this.QuickHide();
		}

		// Token: 0x06004F09 RID: 20233 RVA: 0x00250578 File Offset: 0x0024E778
		private bool InputIsValid()
		{
			int inputValue;
			return int.TryParse(this.InputText, out inputValue) && this._minValue <= inputValue && inputValue <= this._maxValue;
		}

		// Token: 0x04003660 RID: 13920
		private int _minValue;

		// Token: 0x04003661 RID: 13921
		private int _maxValue;

		// Token: 0x04003662 RID: 13922
		private Action<int> _selectCallback;
	}
}
