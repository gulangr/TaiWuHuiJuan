using System;
using Config;
using FrameWork;
using TMPro;

namespace AiEditor
{
	// Token: 0x0200068F RID: 1679
	public class UI_AiParamInputField : UIBase
	{
		// Token: 0x170009A7 RID: 2471
		// (get) Token: 0x06004EEE RID: 20206 RVA: 0x0025025E File Offset: 0x0024E45E
		private TMP_InputField InputField
		{
			get
			{
				return base.CGet<TMP_InputField>("InputField");
			}
		}

		// Token: 0x170009A8 RID: 2472
		// (get) Token: 0x06004EEF RID: 20207 RVA: 0x0025026B File Offset: 0x0024E46B
		private CButtonObsolete Confirm
		{
			get
			{
				return base.CGet<CButtonObsolete>("Confirm");
			}
		}

		// Token: 0x170009A9 RID: 2473
		// (get) Token: 0x06004EF0 RID: 20208 RVA: 0x00250278 File Offset: 0x0024E478
		private TextMeshProUGUI Title
		{
			get
			{
				return base.CGet<TextMeshProUGUI>("Title");
			}
		}

		// Token: 0x170009AA RID: 2474
		// (get) Token: 0x06004EF1 RID: 20209 RVA: 0x00250285 File Offset: 0x0024E485
		private TextMeshProUGUI PlaceHolder
		{
			get
			{
				return this.InputField.placeholder.GetComponent<TextMeshProUGUI>();
			}
		}

		// Token: 0x170009AB RID: 2475
		// (get) Token: 0x06004EF2 RID: 20210 RVA: 0x00250297 File Offset: 0x0024E497
		private string InputText
		{
			get
			{
				return this.InputField.text;
			}
		}

		// Token: 0x06004EF3 RID: 20211 RVA: 0x002502A4 File Offset: 0x0024E4A4
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get("TemplateId", out this._aiParamTemplateId);
			argsBox.Get<Action<string>>("SelectCallback", out this._selectCallback);
			this.InputField.text = string.Empty;
			this.Title.text = AiParam.Instance[this._aiParamTemplateId].Name;
			this.PlaceHolder.text = AiParam.Instance[this._aiParamTemplateId].Desc;
			this.OnInputChanged();
		}

		// Token: 0x06004EF4 RID: 20212 RVA: 0x00250330 File Offset: 0x0024E530
		public void OnInputChanged()
		{
			this.Confirm.interactable = this.InputIsValid();
		}

		// Token: 0x06004EF5 RID: 20213 RVA: 0x00250348 File Offset: 0x0024E548
		public void OnConfirm()
		{
			bool flag = !this.InputIsValid();
			if (!flag)
			{
				Action<string> selectCallback = this._selectCallback;
				if (selectCallback != null)
				{
					selectCallback(this.InputText);
				}
				this.QuickHide();
			}
		}

		// Token: 0x06004EF6 RID: 20214 RVA: 0x00250384 File Offset: 0x0024E584
		public void OnCancel()
		{
			this.QuickHide();
		}

		// Token: 0x06004EF7 RID: 20215 RVA: 0x00250390 File Offset: 0x0024E590
		private bool InputIsValid()
		{
			return AiParam.Instance[this._aiParamTemplateId].IsValid(this.InputText);
		}

		// Token: 0x0400365C RID: 13916
		private int _aiParamTemplateId;

		// Token: 0x0400365D RID: 13917
		private Action<string> _selectCallback;
	}
}
