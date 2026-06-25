using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.Mod
{
	// Token: 0x020008D3 RID: 2259
	public class ViewSaveChangedSettingDialog : UIBase
	{
		// Token: 0x06006C3D RID: 27709 RVA: 0x0031E6F4 File Offset: 0x0031C8F4
		private void Awake()
		{
			this.confirmBtn.ClearAndAddListener(delegate
			{
				Action confirmAction = this._confirmAction;
				if (confirmAction != null)
				{
					confirmAction();
				}
				this.QuickHide();
			});
			this.cancelBtn.ClearAndAddListener(delegate
			{
				this.QuickHide();
			});
			this.saveBtn.ClearAndAddListener(delegate
			{
				Action saveAction = this._saveAction;
				if (saveAction != null)
				{
					saveAction();
				}
				this.QuickHide();
			});
		}

		// Token: 0x06006C3E RID: 27710 RVA: 0x0031E74A File Offset: 0x0031C94A
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<Action>("ConfirmAction", out this._confirmAction);
			argsBox.Get<Action>("SaveAction", out this._saveAction);
		}

		// Token: 0x04004E7C RID: 20092
		[SerializeField]
		private CButton confirmBtn;

		// Token: 0x04004E7D RID: 20093
		[SerializeField]
		private CButton cancelBtn;

		// Token: 0x04004E7E RID: 20094
		[SerializeField]
		private CButton saveBtn;

		// Token: 0x04004E7F RID: 20095
		private Action _confirmAction;

		// Token: 0x04004E80 RID: 20096
		private Action _saveAction;
	}
}
